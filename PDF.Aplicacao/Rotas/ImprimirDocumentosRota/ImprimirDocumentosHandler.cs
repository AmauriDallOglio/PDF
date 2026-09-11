using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PDF.Aplicacao.Util;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PDF.Aplicacao.Rotas.ImprimirDocumentosRota
{
    public class ImprimirDocumentosHandler : IContratoBaseHandler<ImprimirDocumentosRequest, ResultadoOperacao>
    {
        private readonly IConfiguration _configuration;

        public ImprimirDocumentosHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ResultadoOperacao> Executar(ImprimirDocumentosRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("ConexaoServidorRag")
                    ?? "Server=DESKTOP-783G0M0;Database=RAG;User Id=sa;Password=SenhaForte123!;TrustServerCertificate=True;Encrypt=True;";

                var documentos = new List<DocumentoImportadoDto>();

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
                    documentos.Add(new DocumentoImportadoDto
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

                var downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                Directory.CreateDirectory(downloadsPath);

                var nomeArquivo = "DocumentosImportados_PDF_AMA2026.pdf";
                var arquivoDestino = Path.Combine(downloadsPath, nomeArquivo);

                GerarPdfComQuest(documentos, arquivoDestino);

                var response = ImprimirDocumentosResponse.Criar(arquivoDestino);
                return ResultadoOperacao.GerarSucesso(response, "Arquivo PDF gerado com sucesso.");
            }
            catch (Exception ex)
            {
                return ResultadoOperacao.GerarErro($"Erro interno: {ex.Message}", 500);
            }
        }

        private static void GerarPdfComQuest(List<DocumentoImportadoDto> documentos, string arquivoDestino)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Content().Column(col =>
                    {
                        col.Item().Text("Documentos importados no banco RAG")
                            .FontSize(14)
                            .Bold();

                        col.Item().Text($"Criado em { DateTime.Now} " )
                            .FontSize(10);

                        if (documentos.Count == 0)
                        {
                            col.Item().Text("Nenhum documento encontrado no banco RAG.");
                        }
                        else
                        {
                            foreach (var documento in documentos)
                            {
                                col.Item().Text($"[{documento.Id}] {documento.Titulo}")
                                    .FontSize(11)
                                    .Bold();

                                col.Item().Text($"Tipo: {documento.TipoArquivo} | Tamanho: {documento.TamanhoArquivo ?? 0} bytes | Importado: {documento.DataImportacao:yyyy-MM-dd HH:mm:ss}");

                                var texto = documento.Texto ?? string.Empty;
                                if (texto.Length > 180)
                                    texto = texto.Substring(0, 180);

                                col.Item().Text($"Texto: {texto}");
                                col.Item().Text("");
                            }
                        }
                    });
                });
            })
            .GeneratePdf(arquivoDestino);
        }
    }

    public class DocumentoImportadoDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Texto { get; set; } = string.Empty;
        public string TipoArquivo { get; set; } = string.Empty;
        public long? TamanhoArquivo { get; set; }
        public DateTime DataImportacao { get; set; } = DateTime.Now;
        public DateTime? DataAtualizacao { get; set; }
    }
}
