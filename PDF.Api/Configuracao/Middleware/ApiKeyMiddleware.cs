using Microsoft.Extensions.Options;
using PDF.Aplicacao.Dto;

namespace PDF.Api.Configuracao.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IOptionsMonitor<AppSettingsDto> _appSettings;

        public ApiKeyMiddleware(RequestDelegate next, IOptionsMonitor<AppSettingsDto> appSettings)
        {
            _next = next;
            _appSettings = appSettings;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string apiKeyConfigurada = _appSettings.CurrentValue.Seguranca.ApiKey;
            var apiKeyInformada = context.Request.Headers["X-Api-Key"].ToString();

            if (string.IsNullOrWhiteSpace(apiKeyConfigurada) || string.IsNullOrWhiteSpace(apiKeyInformada))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { sucesso = false, mensagem = "API Key ausente." });
                return;
            }

            if (!string.Equals(apiKeyInformada, apiKeyConfigurada, StringComparison.Ordinal))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { sucesso = false, mensagem = "API Key inválida." });
                return;
            }

            await _next(context);
        }
    }
}
