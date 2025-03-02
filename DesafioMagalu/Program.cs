using DesafioMagalu.Data;
using DesafioMagalu.Mapping;
using DesafioMagalu.Repositories;
using DesafioMagalu.Services;
using DesafioMagalu.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<string>(provider =>
    builder.Configuration.GetConnectionString("DefaultConnection")
);

builder.Services.AddSingleton<DatabaseService>();

builder.Services.AddScoped<IArquivoValidator, ArquivoValidator>();
builder.Services.AddScoped<IPedidoFiltroValidator, PedidoFiltroValidator>();

builder.Services.AddScoped<IMagaluRepository, MagaluRepository>();
builder.Services.AddScoped<IMagaluService, MagaluService>();
builder.Services.AddScoped<IZipService, ZipService>();
builder.Services.AddScoped<IDataMapper, DataMapper>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
