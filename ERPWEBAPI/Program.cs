//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.OpenApi.Models;
//using System.Text;
//using ElevateERP.API.Data;

//var builder = WebApplication.CreateBuilder(args);

//// ??? 1. DATABASE (PostgreSQL) ???????????????????????????????????????????????
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
//    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseNpgsql(connectionString));

//// ??? 2. JWT AUTHENTICATION ??????????????????????????????????????????????????
////var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured");
////var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "ElevateERP";
////var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "ElevateERPFrontend";

////builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
////    .AddJwtBearer(options =>
////    {
////        options.TokenValidationParameters = new TokenValidationParameters
////        {
////            ValidateIssuer = true,
////            ValidateAudience = true,
////            ValidateLifetime = true,
////            ValidateIssuerSigningKey = true,
////            ValidIssuer = jwtIssuer,
////            ValidAudience = jwtAudience,
////            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
////        };
////    });

////builder.Services.AddAuthorization();

//// ??? 3. CORS (allow React frontend) ????????????????????????????????????????
//// CORS
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("ReactPolicy", policy =>
//    {
//        policy
//            .WithOrigins("http://localhost:8080")
//            .AllowAnyHeader()
//            .AllowAnyMethod()
//            .AllowCredentials();
//    });
//});

//// ??? 4. CONTROLLERS ?????????????????????????????????????????????????????????
//builder.Services.AddControllers();

//// ??? 5. SWAGGER / OPENAPI ???????????????????????????????????????????????????
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
////builder.Services.AddSwaggerGen(c =>
////{
////    c.SwaggerDoc("v1", new OpenApiInfo
////    {
////        Title = "ElevateERP API",
////        Version = "v1",
<<<<<<< HEAD
////        Description = "Backend API for ElevateERP � HR & Staff Management System"
=======
////        Description = "Backend API for ElevateERP — HR & Staff Management System"
>>>>>>> d728f8f40026a18e12e14a4b19af1e31ae724a1d
////    });

////    // Add JWT bearer auth to Swagger UI
////    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
////    {
////        Name = "Authorization",
////        Type = SecuritySchemeType.Http,
////        Scheme = "Bearer",
////        BearerFormat = "JWT",
////        In = ParameterLocation.Header,
////        Description = "Enter your JWT token. Example: Bearer eyJhbGci..."
////    });
////    c.AddSecurityRequirement(new OpenApiSecurityRequirement
////    {
////        {
////            new OpenApiSecurityScheme
////            {
////                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
////            },
////            Array.Empty<string>()
////        }
////    });
////});

//// ??? BUILD ??????????????????????????????????????????????????????????????????
//var app = builder.Build();

//// ??? 6. AUTO MIGRATE ON STARTUP ?????????????????????????????????????????????
//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//    db.Database.Migrate();
//}

//// ??? 7. MIDDLEWARE PIPELINE ?????????????????????????????????????????????????
//// Swagger
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();

//    app.UseSwaggerUI();
//}


//app.UseHttpsRedirection();

//app.UseRouting();

//app.UseCors("ReactPolicy");

////app.UseAuthentication();

//app.UseAuthorization();

//app.MapControllers();


//// Health
//app.MapGet("/health", () =>
//{
//    return Results.Ok(new
//    {
//        status = "healthy",
//        time = DateTime.UtcNow
//    });
//});

//app.Run();
using Microsoft.EntityFrameworkCore;
using ElevateERP.API.Data;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// DATABASE
var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string not found");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// CORS
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("ReactPolicy", policy =>
//    {
//        policy.WithOrigins(
//                "http://localhost:8080",
//                "http://localhost:8082"
//              )
//              .AllowAnyHeader()
//              .AllowAnyMethod();
//    });
//});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVercel", policy =>
    {
        policy.WithOrigins("https://digambar-medical-crm.vercel.app")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Auto Migration
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Swagger
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Add this for detailed error messages during development
app.UseDeveloperExceptionPage();
<<<<<<< HEAD

=======
// app.UseCors("AllowVercel");
>>>>>>> d728f8f40026a18e12e14a4b19af1e31ae724a1d
// OR if you want JSON error response:
app.UseExceptionHandler(appBuilder =>
{
    appBuilder.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        var error = context.Features.Get<IExceptionHandlerFeature>();
        if (error != null)
        {
            await context.Response.WriteAsync(
                System.Text.Json.JsonSerializer.Serialize(new
                {
                    message = error.Error.Message,
                    detail = error.Error.InnerException?.Message
                })
            );
        }
    });
});
app.MapGet("/", () => "Digambar Medical CRM API is running!");
app.UseHttpsRedirection();

app.UseRouting();

<<<<<<< HEAD
app.UseCors("AllowVercel");
// app.UseCors("ReactPolicy");
=======
// app.UseCors("ReactPolicy");
app.UseCors("AllowVercel");
>>>>>>> d728f8f40026a18e12e14a4b19af1e31ae724a1d

app.MapControllers();

// Health
app.MapGet("/health", () =>
{
    return Results.Ok(new
    {
        status = "healthy",
        time = DateTime.UtcNow
    });
});

app.Run();
