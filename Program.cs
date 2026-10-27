var builder = WebApplication.CreateBuilder(args);

// 1. Agregar soporte para controladores y para el generador de Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 2. Configurar el pipeline para que habilite Swagger en entorno de desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiBancoDemo v1");
        
        // OPCIONAL: Si deja esta línea activa, Swagger abrirá automáticamente 
        // en la raíz (http://localhost:5067/) sin necesidad de escribir /swagger
        c.RoutePrefix = string.Empty; 
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

// 3. Mapear los controladores para que reconozca CuentasController
app.MapControllers();

app.Run();