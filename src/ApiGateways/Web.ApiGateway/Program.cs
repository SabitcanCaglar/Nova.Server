using Base.Api;
using Base.Application;
using Base.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddBaseApplicationServices();
builder.Services.AddBaseInfrastructureServices(builder.Configuration);
builder.Services.AddBaseApiServices(builder.Configuration);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();