using BackendAPI.Data;
using BackendAPI.Mapping;
using Microsoft.EntityFrameworkCore;
using BackendAPI.Middleware;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BackendAPI.Services;
// Services
using BackendAPI.Services.Product;
using BackendAPI.Services.Production;
using BackendAPI.Services.Bom;
using BackendAPI.Services.RawMaterial;

// Repositories
using BackendAPI.Repositories.ProductRepository;
using BackendAPI.Repositories.BomRepository;
using BackendAPI.Repositories.ProductionRepository;
using BackendAPI.Repositories.RawMaterial;



var builder = WebApplication.CreateBuilder(args);



// Add SQL Server Connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// jwt
builder.Services.AddScoped<JwtService>();
//  DI
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductionRepository,ProductionRepository>();
builder.Services.AddScoped<IBomRepository, BomRepository>();
builder.Services.AddScoped<IRawMaterialRepository, RawMaterialRepository>();

// Services
builder.Services.AddScoped<IBomService, BomService>();
builder.Services.AddScoped<IProductionService,ProductionService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IRawMaterialService, RawMaterialService>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Automapper
builder.Services.AddAutoMapper(typeof(ProductProfile));

// TODO: jwt validation-need to revisit
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
    });

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



app.UseAuthentication();


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    
    
    DbInitializer.Initialize(context);
}

app.Run();
