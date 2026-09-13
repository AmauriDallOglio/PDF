using PDF.Dominio.RAG.Entidade;

namespace PDF.Dominio.RAG.InterfaceRepositorio
{
    public interface IDocumentoImportadoRepositorio
    {
        Task<List<DocumentoImportado>> ObterDocumentosAsync(CancellationToken cancellationToken = default);
    }
}
