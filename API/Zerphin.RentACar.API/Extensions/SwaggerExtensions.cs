namespace Zerphin.RentACar.API.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "Zerphin Rent A Car API",
                Version = "v1",
                Description = "Araç kiralama sistemi API'si"
            });
        });

        return services;
    }

    public static WebApplication UseSwaggerUI(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Zerphin Rent A Car API v1");
                c.RoutePrefix = string.Empty;
            });
        }

        return app;
    }
}
