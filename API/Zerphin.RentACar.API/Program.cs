using Zerphin.RentACar.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwagger();
builder.Services.AddEntityFramework(builder.Configuration);

var app = builder.Build();

app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
