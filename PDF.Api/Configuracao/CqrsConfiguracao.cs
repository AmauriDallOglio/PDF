using PDF.Aplicacao.Rotas.ImprimirDocumentosRota;
using PDF.Aplicacao.Util;

namespace PDF.Api.Configuracao
{
    public static class CqrsConfiguracao
    {
        public static IServiceCollection RegistrarCqrs(this IServiceCollection services)
        {
            services.RegistrarHandler<ImprimirDocumentosRequest, ImprimirDocumentosHandler>();
            return services;
        }

        private static IServiceCollection RegistrarHandler<TRequest, THandler>(this IServiceCollection services)
            where TRequest : IRequest<ResultadoOperacao>
            where THandler : class, IContratoBaseHandler<TRequest, ResultadoOperacao>
        {
            services.AddScoped<THandler>();
            services.AddScoped<IContratoBaseHandler<TRequest, ResultadoOperacao>, THandler>();
            return services;
        }
    }
}
