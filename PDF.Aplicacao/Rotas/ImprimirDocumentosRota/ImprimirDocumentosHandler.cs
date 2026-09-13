using PDF.Aplicacao.Util;
using PDF.Dominio.RAG.Entidade;
using PDF.Dominio.RAG.InterfaceRepositorio;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PDF.Aplicacao.Rotas.ImprimirDocumentosRota
{
    public class ImprimirDocumentosHandler : IContratoBaseHandler<ImprimirDocumentosRequest, ResultadoOperacao>
    {
        private readonly IDocumentoImportadoRepositorio _documentoImportadoRepositorio;

        public ImprimirDocumentosHandler(IDocumentoImportadoRepositorio documentoImportadoRepositorio)
        {
            _documentoImportadoRepositorio = documentoImportadoRepositorio;
        }

        public async Task<ResultadoOperacao> Executar(ImprimirDocumentosRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var documentos = await _documentoImportadoRepositorio.ObterDocumentosAsync(cancellationToken);

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

        private static void GerarPdfComQuest(List<DocumentoImportado> documentos, string arquivoDestino)
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

                        col.Item().Text($"Criado em { DateTime.Now} ")
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

                                col.Item().Text($"TipoArquivo: {documento.TipoArquivo}");
                                col.Item().Text($"TamanhoArquivo: {documento.TamanhoArquivo ?? 0} bytes");
                                col.Item().Text($"DataImportacao: {documento.DataImportacao:yyyy-MM-dd HH:mm:ss}");
                                col.Item().Text($"DataAtualizacao: {documento.DataAtualizacao?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A"}");

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
}
