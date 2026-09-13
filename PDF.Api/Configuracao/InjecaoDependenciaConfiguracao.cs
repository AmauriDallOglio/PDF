using Microsoft.AspNetCore.Mvc;
using PDF.Aplicacao.Rotas.ImprimirDocumentosRota;
using PDF.Aplicacao.Rotas.ImprimirModelosRota;
using PDF.Dominio.RAG.InterfaceRepositorio;
using PDF.Dominio.MLNet.InterfaceRepositorio;
using PDF.Infraestrutura.RAG.Contexto;
using PDF.Infraestrutura.MLNet.Contexto;

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

            builder.Services.AddScoped<IDocumentoImportadoRepositorio, RagContexto>();
            builder.Services.AddScoped<IModeloTreinamentoRepositorio, MlNetContexto>();
            builder.Services.AddScoped<ImprimirDocumentosHandler>();
            builder.Services.AddScoped<ImprimirModelosHandler>();
        }
    }
}
