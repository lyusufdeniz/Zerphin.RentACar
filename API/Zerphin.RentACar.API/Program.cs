using Zerphin.RentACar.API.Extensions;
using Zerphin.RentACar.API.Filters;
using Zerphin.RentACar.Application.Extensions;
using Zerphin.RentACar.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<FluentValidationFilter>();
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddHttpContextAccessor();

// CORS configuration
builder.Services.AddCorsConfiguration();
builder.Services.AddSwagger();
builder.Services.AddEntityFramework(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddOptions(builder.Configuration);
builder.Services.AddAuthorization();


var app = builder.Build();
app.UseCors("AllowAll");
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.AddAuthzMiddlewares(); 
app.UseAuthorization();
app.MapControllers();


app.Run();
