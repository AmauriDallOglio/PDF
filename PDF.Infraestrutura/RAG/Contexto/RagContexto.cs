using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PDF.Dominio.RAG.Entidade;
using PDF.Dominio.RAG.InterfaceRepositorio;

namespace PDF.Infraestrutura.RAG.Contexto
{
    public class RagContexto : IDocumentoImportadoRepositorio
    {
        private readonly IConfiguration _configuration;

        public RagContexto(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<DocumentoImportado>> ObterDocumentosAsync(CancellationToken cancellationToken = default)
        {
            var connectionString = _configuration.GetConnectionString("ConexaoServidorRag")
                ?? "Server=DESKTOP-783G0M0;Database=RAG;User Id=sa;Password=SenhaForte123!;TrustServerCertificate=True;Encrypt=True;";

            var documentos = new List<DocumentoImportado>();

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            const string sql = @"
                SELECT Id, Titulo, Texto, TipoArquivo, TamanhoArquivo, DataImportacao, DataAtualizacao
                FROM Documento
                ORDER BY DataImportacao DESC;";

            await using var command = new SqlCommand(sql, connection);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                documentos.Add(new DocumentoImportado
                {
                    Id = reader.GetInt32(0),
                    Titulo = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    Texto = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    TipoArquivo = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    TamanhoArquivo = reader.IsDBNull(4) ? null : reader.GetInt64(4),
                    DataImportacao = reader.IsDBNull(5) ? DateTime.Now : reader.GetDateTime(5),
                    DataAtualizacao = reader.IsDBNull(6) ? null : reader.GetDateTime(6)
                });
            }

            return documentos;
        }
    }
}
