using Internal_Portal.Data;
using Internal_Portal.Interface;
using Internal_Portal.Models;
using Internal_Portal.Repository;
using Internal_Portal.Swagger;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Internal Portal API",
        Version = "v1",
        Description = "API for internal portal with Customer, Vendor, Employee, Project, Timesheet modules"
    });

    // Enable Swagger annotations
    c.EnableAnnotations();

    // Include XML comments for API documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});



// DB connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register connection factory and repository
builder.Services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
builder.Services.AddScoped<ILogin, LoginRepository>();
builder.Services.AddScoped<IRegister, RegisterRepository>();
builder.Services.AddScoped<ICompany, CompanyRepository>();
builder.Services.AddScoped<ICustomer, CustomerRepository>();
builder.Services.AddScoped<IEmployee, EmployeeRepository>();
builder.Services.AddScoped<IVendor, VendorRepository>();
builder.Services.AddScoped<IProjectEmployee, ProjectEmployeeRepository>();
builder.Services.AddScoped<IProjectMaster, ProjectMasterRepository>();
builder.Services.AddScoped<ITimesheet, TimesheetRepository>();
builder.Services.AddScoped<ITimesheetEntry, TimesheetEntryRepository>();
builder.Services.AddScoped<IDropdownMaster, DropdownMasterRepository>();
builder.Services.AddScoped<IService, ServiceRepository>();
builder.Services.AddScoped<ISolution, SolutionRepository>();
builder.Services.AddScoped<ICareer, CareerRepository>();

builder.Services.AddScoped<ISubService, SubServiceRepository>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins(
           "http://localhost:5169",
           "https://localhost:7040",
           "https://192.168.1.109:7574"
       )
             .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Internal Portal API v1");
        // Uncomment if you want Swagger UI at root URL:
        // c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors("AllowSpecificOrigin");

app.UseAuthorization();

app.MapControllers();

app.Run();
