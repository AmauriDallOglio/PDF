using PDF.Aplicacao.Dto;
using PDF.Aplicacao.Util;
using System.Net;
using System.Threading.RateLimiting;

namespace PDF.Api.Configuracao
{
    public static class RateLimitConfiguracao
    {
        public static void RegistrarRateLimit(this IServiceCollection services, AppSettingsDto appSettings)
        {
            RateLimitDto configuracao = appSettings.RateLimit;

            if (!configuracao.Habilitado)
                return;

            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                {
                    string chave = ObterChaveCliente(context);

                    return RateLimitPartition.GetFixedWindowLimiter(chave, _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = configuracao.RequisicoesPermitidas,
                        Window = TimeSpan.FromSeconds(configuracao.JanelaEmSegundos),
                        QueueLimit = configuracao.TamanhoFila,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        AutoReplenishment = true
                    });
                });

                options.OnRejected = async (context, cancellationToken) =>
                {
                    context.HttpContext.Response.ContentType = "application/json";

                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfter))
                        context.HttpContext.Response.Headers.RetryAfter = Math.Ceiling(retryAfter.TotalSeconds).ToString();

                    ResultadoOperacao resultado = ResultadoOperacao.GerarErro(
                        "Limite de requisições excedida. Tente novamente mais tarde.",
                        StatusCodes.Status429TooManyRequests);

                    await context.HttpContext.Response.WriteAsJsonAsync(resultado, cancellationToken);
                };
            });
        }

        private static string ObterChaveCliente(HttpContext context)
        {
            IPAddress? ip = context.Connection.RemoteIpAddress;
            return ip?.ToString() ?? "cliente-desconhecido";
        }
    }
}
