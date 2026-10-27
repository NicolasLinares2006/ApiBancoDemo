using System.Text;
using ApiBancoDemo.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// 1. Controladores + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 2. Swagger con soporte para Bearer Token
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ApiBancoDemo v1",
        Version = "v1",
        Description = "API de capacitación JMeter con JWT, imágenes y transferencias en RAM"
    });

    // Definición del esquema de seguridad (Bearer)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese SOLO el token JWT (sin la palabra 'Bearer')."
    });

    // ✅ Nuevo formato con delegado (Microsoft.OpenApi 2.x)
    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

// 3. DI: almacén en RAM + servicios
builder.Services.AddSingleton<AlmacenEnMemoria>();
builder.Services.AddSingleton<TokenService>();
builder.Services.AddSingleton<ImageStorageService>();

// 4. Autenticación JWT
var tokenService = new TokenService(builder.Configuration);
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = tokenService.GetValidationParameters();

        options.Events = new JwtBearerEvents
        {
            OnChallenge = ctx =>
            {
                ctx.HandleResponse();
                ctx.Response.StatusCode = 401;
                ctx.Response.ContentType = "application/json";
                return ctx.Response.WriteAsync("{\"mensaje\":\"Token ausente o inválido.\"}");
            },
            OnForbidden = ctx =>
            {
                ctx.Response.StatusCode = 403;
                ctx.Response.ContentType = "application/json";
                return ctx.Response.WriteAsync("{\"mensaje\":\"No tiene permisos para este recurso.\"}");
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// 5. Swagger en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiBancoDemo v1");
        c.RoutePrefix = string.Empty;
    });
}

// 6. Pipeline
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();