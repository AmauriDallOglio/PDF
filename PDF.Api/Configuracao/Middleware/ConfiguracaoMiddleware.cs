namespace PDF.Api.Configuracao.Middleware
{
    public static class ConfiguracaoMiddleware
    {
        public static IApplicationBuilder UseApiKeyMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiKeyMiddleware>();
        }

        public static IApplicationBuilder UseRegistroRequisicao(this IApplicationBuilder builder)
        {
            return builder;
        }

        public static IApplicationBuilder UseErroMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErroMiddleware>();
        }

        public static IApplicationBuilder UseTempoResposta(this IApplicationBuilder builder)
        {
            return builder;
        }

        public static IApplicationBuilder ConfigurarMiddlewaresApi(this IApplicationBuilder app)
        {
            app.UseRegistroRequisicao();
            app.UseApiKeyMiddleware();
            return app;
        }
    }
}
