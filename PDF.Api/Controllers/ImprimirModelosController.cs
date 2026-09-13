using Microsoft.AspNetCore.Mvc;
using PDF.Aplicacao.Rotas.ImprimirModelosRota;

namespace PDF.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImprimirModelosController : ControllerBase
    {
        private readonly ImprimirModelosHandler _handler;

        public ImprimirModelosController(ImprimirModelosHandler handler)
        {
            _handler = handler;
        }

        [HttpGet("ImprimirModelos")]
        public async Task<IActionResult> ImprimirModelos([FromQuery] ImprimirModelosRequest request, CancellationToken cancellationToken)
        {
            var resultado = await _handler.Executar(request, cancellationToken);
            return Ok(resultado);
        }
    }
}
