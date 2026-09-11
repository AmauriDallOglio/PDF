using PDF.Aplicacao.Util;

namespace PDF.Api.Configuracao.Middleware
{
    public class ErroMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErroMiddleware> _logger;

        public ErroMiddleware(RequestDelegate next, ILogger<ErroMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await TratarExcecaoAsync(context, ex);
            }
        }

        private Task TratarExcecaoAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "Erro não tratado na aplicação");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var resultado = ResultadoOperacao.GerarErro("Ocorreu um erro interno no servidor.", StatusCodes.Status500InternalServerError);
            return context.Response.WriteAsJsonAsync(resultado);
        }
    }
}
