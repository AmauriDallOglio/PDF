using Microsoft.AspNetCore.Mvc;
using PDF.Aplicacao.Rotas.ImprimirDocumentosRota;

namespace PDF.Api.Configuracao
{
    public static class InjecaoDependenciaConfiguracao
    {
        public static void RegistrarServicos(WebApplicationBuilder builder)
        {
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            builder.Services.AddScoped<ImprimirDocumentosHandler>();
        }
    }
}
