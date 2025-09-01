using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Internal_Portal.Swagger
{
    public class SwaggerFileOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasFormFile = false;
            var formFileProperties = new Dictionary<string, OpenApiSchema>();

            // Examine action parameters: if a parameter is a complex type, check its properties for IFormFile
            foreach (var param in context.MethodInfo.GetParameters())
            {
                var paramType = param.ParameterType;
                // if it's a complex type (class) check its properties
                if (!paramType.IsPrimitive && paramType != typeof(string))
                {
                    foreach (var prop in paramType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        if (typeof(IFormFile).IsAssignableFrom(prop.PropertyType))
                        {
                            hasFormFile = true;
                            formFileProperties[prop.Name] = new OpenApiSchema { Type = "string", Format = "binary" };
                        }
                        else if (typeof(IEnumerable<IFormFile>).IsAssignableFrom(prop.PropertyType))
                        {
                            hasFormFile = true;
                            formFileProperties[prop.Name] = new OpenApiSchema { Type = "array", Items = new OpenApiSchema { Type = "string", Format = "binary" } };
                        }
                    }
                }
            }

            if (!hasFormFile) return;

            // Set request body to multipart/form-data with the file properties
            operation.RequestBody = new OpenApiRequestBody
            {
                Content =
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = formFileProperties.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
                        }
                    }
                }
            };
        }
    }
}