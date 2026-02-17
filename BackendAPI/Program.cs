using BackendAPI.Data;
using Microsoft.EntityFrameworkCore;
using BackendAPI.Middleware;

// Services
using BackendAPI.Services.Production;
using BackendAPI.Services.Bom;

// Repositories
using BackendAPI.Repositories.BomRepository;
using BackendAPI.Repositories.ProductionRepository;



var builder = WebApplication.CreateBuilder(args);


// Add SQL Server Connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//  DI
builder.Services.AddScoped<IProductionRepository,ProductionRepository>();
builder.Services.AddScoped<IBomRepository, BomRepository>();

// Services
builder.Services.AddScoped<IBomService, BomService>();
builder.Services.AddScoped<IProductionService,ProductionService>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") 
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddControllers();


var app = builder.Build();
// Middleware
app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", async context =>
{
    context.Response.Redirect("/swagger/index.html");
    await Task.CompletedTask;
});


// before auth
app.UseCors("AllowFrontend");

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
