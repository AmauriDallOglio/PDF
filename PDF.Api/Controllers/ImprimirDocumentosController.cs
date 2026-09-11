using Microsoft.AspNetCore.Mvc;
using PDF.Aplicacao.Rotas.ImprimirDocumentosRota;

namespace PDF.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImprimirDocumentosController : ControllerBase
    {
        private readonly ImprimirDocumentosHandler _handler;

        public ImprimirDocumentosController(ImprimirDocumentosHandler handler)
        {
            _handler = handler;
        }

        [HttpGet("ImprimirDocumentos")]
        public async Task<IActionResult> ImprimirDocumentos([FromQuery] ImprimirDocumentosRequest request, CancellationToken cancellationToken)
        {
            var resultado = await _handler.Executar(request, cancellationToken);
            return Ok(resultado);
        }
    }
}
