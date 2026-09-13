using PDF.Dominio.InterfaceRepositorio.Configuracao;
using PDF.Dominio.RAG.Entidade;

namespace PDF.Dominio.RAG.InterfaceRepositorio
{
    public interface IDocumentoImportadoCommandRepositorio : IGenericoCommandRepositorio<DocumentoImportado>
    {
        Task<List<DocumentoImportado>> ObterPaginadoAsync(int page, int pageSize, CancellationToken cancellationToken);
        Task<List<DocumentoImportado>> ObterTodosComEstruturaAsync(CancellationToken cancellationToken);
        Task<DocumentoImportado?> ObterPorIdComEstruturaAsync(int id, CancellationToken cancellationToken);
    }
}
