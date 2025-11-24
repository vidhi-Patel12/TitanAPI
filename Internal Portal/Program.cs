using Internal_Portal.Auth;
using Internal_Portal.Data;
using Internal_Portal.Interface;
using Internal_Portal.Models;
using Internal_Portal.Repository;
using Internal_Portal.Services;
using Internal_Portal.Swagger;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add Controllers
builder.Services.AddControllers();

// ✅ Swagger Configuration
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Internal Portal API",
        Version = "v1",
        Description = "API for internal portal with Customer, Vendor, Employee, Project, Timesheet modules"
    });

    // Enable annotations
    c.EnableAnnotations();

    // XML Docs (optional)
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // ✅ JWT Bearer Authentication in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new OpenApiInfo
//    {
//        Title = "Internal Portal API",
//        Version = "v1",
//        Description = "API for internal portal with Customer, Vendor, Employee, Project, Timesheet modules"
//    });

//    // Enable Swagger annotations
//    c.EnableAnnotations();

//    // XML Docs (optional)
//    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
//    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
//    if (File.Exists(xmlPath))
//    {
//        c.IncludeXmlComments(xmlPath);
//    }

//    // ✅ Cookie Authentication in Swagger
//    c.AddSecurityDefinition("cookieAuth", new OpenApiSecurityScheme
//    {
//        Type = SecuritySchemeType.ApiKey,
//        In = ParameterLocation.Cookie,
//        Name = "InternalPortalAuth",
//        Description = "Cookie-based authentication"
//    });

//    c.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "cookieAuth"
//                }
//            },
//            Array.Empty<string>()
//        }
//    });
//});


// ✅ Database Connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



// ✅ Repositories
builder.Services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
builder.Services.AddSingleton<SmsService>();
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
builder.Services.AddScoped<IUserRole, UserRoleRepository>();
builder.Services.AddScoped<IUserRolePermission, UserRolePermissionRepository>();
builder.Services.AddScoped<ISubService, SubServiceRepository>();
builder.Services.AddScoped<IContact, ContactRepository>();
builder.Services.AddScoped<IJobApplication, JobApplicationRepository>();

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Configuration.AddEnvironmentVariables();


// ✅ Authorization Handler + Policy Provider
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();


// ✅ JWT Authentication Setup
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

// ✅ Cookie Authentication Setup (for APIs)
//builder.Services.AddAuthentication("MyCookieScheme")
//    .AddCookie("MyCookieScheme", options =>
//    {
//        options.Cookie.Name = "InternalPortalAuth";
//        //options.Cookie.Domain = ".titentechnology.com";
//        options.Cookie.SameSite = SameSiteMode.None;
//        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
//        options.LoginPath = "/Auth/Login";
//        options.AccessDeniedPath = "/Account/AccessDenied";
//        options.SlidingExpiration = true;
//        options.ExpireTimeSpan = TimeSpan.FromHours(1);

//        // Critical for APIs — no redirects, just 401/403
//        options.Events = new CookieAuthenticationEvents
//        {
//            OnRedirectToLogin = context =>
//            {
//                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
//                return Task.CompletedTask;
//            },
//            OnRedirectToAccessDenied = context =>
//            {
//                context.Response.StatusCode = StatusCodes.Status403Forbidden;
//                return Task.CompletedTask;
//            }
//        };
//    });

// ✅ Temporary — allow all policies (for testing)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Company.View", policy => policy.RequireAssertion(_ => true));
    options.AddPolicy("Company.GetByCode", policy => policy.RequireAssertion(_ => true));
    options.AddPolicy("Company.AddUpdate", policy => policy.RequireAssertion(_ => true));
    options.AddPolicy("Company.Delete", policy => policy.RequireAssertion(_ => true));

    options.AddPolicy("Customer.View", policy => policy.RequireAssertion(_ => true));
    options.AddPolicy("Customer.ViewById", policy => policy.RequireAssertion(_ => true));
    options.AddPolicy("Customer.InsertUpdate", policy => policy.RequireAssertion(_ => true));
    options.AddPolicy("Customer.Delete", policy => policy.RequireAssertion(_ => true));

    options.AddPolicy("AdminDropdown.View", policy => policy.RequireAssertion(_ => true));
    options.AddPolicy("AdminDropdown.ViewById", policy => policy.RequireAssertion(_ => true));
    options.AddPolicy("AdminDropdown.ViewByName", policy => policy.RequireAssertion(_ => true));
    options.AddPolicy("AdminDropdown.InsertUpdate", policy => policy.RequireAssertion(_ => true));
    options.AddPolicy("AdminDropdown.Delete", policy => policy.RequireAssertion(_ => true));

    options.AddPolicy("Employee.View", policy => policy.RequireAssertion(_ => true));
    options.AddPolicy("Employee.ViewById", policy => policy.RequireAssertion(_ => true));
    options.AddPolicy("Employee.InsertUpdate", policy => policy.RequireAssertion(_ => true));
    options.AddPolicy("Employee.Delete", policy => policy.RequireAssertion(_ => true));

    options.AddPolicy("ProjectEmployee.View", policy => policy.RequireAssertion(_ => true));
    options.AddPolicy("ProjectEmployee.ViewById", policy => policy.RequireAssertion(_ => true));
    options.AddPolicy("ProjectEmployee.GetByProjectCode", policy => policy.RequireAssertion(_ => true));
    options.AddPolicy("ProjectEmployee.InsertUpdate", policy => policy.RequireAssertion(_ => true));
    options.AddPolicy("ProjectEmployee.Delete", policy => policy.RequireAssertion(_ => true));

    options.AddPolicy("ProjectMaster.View", policy => policy.RequireAssertion(_ => true));
    options.AddPolicy("ProjectMaster.ViewByCode", policy => policy.RequireAssertion(_ => true));
    options.AddPolicy("ProjectMaster.InsertUpdate", policy => policy.RequireAssertion(_ => true));
    options.AddPolicy("ProjectMaster.Delete", policy => policy.RequireAssertion(_ => true));

    options.AddPolicy("Vendor.View", policy => policy.RequireAssertion(_ => true));
    options.AddPolicy("Vendor.ViewById", policy => policy.RequireAssertion(_ => true));
    options.AddPolicy("Vendor.AddUpdate", policy => policy.RequireAssertion(_ => true));
    options.AddPolicy("Vendor.Delete", policy => policy.RequireAssertion(_ => true));

    options.AddPolicy("Timesheet.View", policy => policy.RequireAssertion(_ => true));

    options.AddPolicy("AdminTimesheetReport.View", policy => policy.RequireAssertion(_ => true));

    options.AddPolicy("Admin.ViewAllPermissions", policy => policy.RequireAssertion(_ => true));
    options.AddPolicy("Admin.ViewAllRolePermissions", policy => policy.RequireAssertion(_ => true));
    options.AddPolicy("Admin.ViewByIdRolePermissions", policy => policy.RequireAssertion(_ => true));
    options.AddPolicy("Admin.AddEditRolePermissions", policy => policy.RequireAssertion(_ => true));
    options.AddPolicy("Admin.DeleteRolePermissions", policy => policy.RequireAssertion(_ => true));

});

// ✅ CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy.WithOrigins("https://titentechnology.com", "https://localhost:44368")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});



var app = builder.Build();

// ✅ Middleware Pipeline
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Internal Portal API v1");
        c.RoutePrefix = "swagger";
    });

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowLocalhost");

app.UseAuthentication(); //  Must come before Authorization
app.UseAuthorization();

app.MapControllers();

app.Run();
