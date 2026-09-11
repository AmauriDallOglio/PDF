using Microsoft.Extensions.Options;
using PDF.Aplicacao.Dto;

namespace PDF.Api.Configuracao
{
    public static class AppSettingsConfiguracao
    {
        public static void Carregar(this IServiceCollection services, IConfigurationRoot configuration)
        {
            services.Configure<AppSettingsDto>(configuration);
            services.RegistrarRateLimit(configuration.Get<AppSettingsDto>() ?? new AppSettingsDto());
        }

        public static void AtivarAppSettinngsConfiguracao(this WebApplication app)
        {
            var monitor = app.Services.GetRequiredService<IOptionsMonitor<AppSettingsDto>>();

            monitor.OnChange(settings =>
            {
                Console.WriteLine($"[CONFIG] AppSettingsDto alterado em {DateTime.Now}");
                Console.WriteLine($"Nova API Key: {settings.Seguranca.ApiKey}");
                Console.WriteLine($"RateLimit habilitado: {settings.RateLimit.Habilitado}");
            });

            if (monitor.CurrentValue.RateLimit.Habilitado)
                app.UseRateLimiter();
        }
    }
}
