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

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese SOLO el token JWT (sin la palabra 'Bearer')."
    });

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

// 5. Swagger habilitado en TODOS los entornos (necesario para Render)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiBancoDemo v1");
    c.RoutePrefix = string.Empty; // UI en la raíz: https://tu-servicio.onrender.com/
});

// 6. Redirección raíz -> /swagger si en el futuro cambias RoutePrefix
// (comentado porque ahora RoutePrefix = string.Empty)
// app.MapGet("/", () => Results.Redirect("/swagger"));

// 7. Pipeline
// En Render el TLS lo termina el balanceador, por lo que UseHttpsRedirection
// solo genera warnings ("Failed to determine the https port"). Lo limitamos a local.
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();