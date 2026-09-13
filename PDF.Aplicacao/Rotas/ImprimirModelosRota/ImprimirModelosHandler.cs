using PDF.Aplicacao.Util;
using PDF.Dominio.MLNet.InterfaceRepositorio;
using PDF.Dominio.MLNet.Entidade;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PDF.Aplicacao.Rotas.ImprimirModelosRota
{
    public class ImprimirModelosHandler : IContratoBaseHandler<ImprimirModelosRequest, ResultadoOperacao>
    {
        private readonly IModeloTreinamentoRepositorio _modeloTreinamentoRepositorio;

        public ImprimirModelosHandler(IModeloTreinamentoRepositorio modeloTreinamentoRepositorio)
        {
            _modeloTreinamentoRepositorio = modeloTreinamentoRepositorio;
        }

        public async Task<ResultadoOperacao> Executar(ImprimirModelosRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var modelos = await _modeloTreinamentoRepositorio.ObterModelosAsync(cancellationToken);

                var downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                Directory.CreateDirectory(downloadsPath);

                var nomeArquivo = "ModelosTreinamento_PDF_AMA2026.pdf";
                var arquivoDestino = Path.Combine(downloadsPath, nomeArquivo);

                GerarPdfComQuest(modelos, arquivoDestino);

                var response = ImprimirModelosResponse.Criar(arquivoDestino);
                return ResultadoOperacao.GerarSucesso(response, "Arquivo PDF gerado com sucesso.");
            }
            catch (Exception ex)
            {
                return ResultadoOperacao.GerarErro($"Erro interno: {ex.Message}", 500);
            }
        }

        private static void GerarPdfComQuest(List<ModeloTreinamento> modelos, string arquivoDestino)
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
                        col.Item().Text("Modelos de treinamento no banco MLNet")
                            .FontSize(14)
                            .Bold();

                        col.Item().Text($"Criado em {DateTime.Now}")
                            .FontSize(10);

                        if (modelos.Count == 0)
                        {
                            col.Item().Text("Nenhum modelo encontrado no banco MLNet.");
                        }
                        else
                        {
                            foreach (var modelo in modelos)
                            {
                                col.Item().Text($"[{modelo.Id}] {modelo.Nome}")
                                    .FontSize(11)
                                    .Bold();

                                col.Item().Text($"Nome: {modelo.Nome}");
                                col.Item().Text($"Versao: {modelo.Versao}");
                                col.Item().Text($"DataTreinamento: {modelo.DataTreinamento:yyyy-MM-dd HH:mm:ss}");
                                col.Item().Text($"TamanhoModelo: {modelo.Modelo?.Length ?? 0} bytes");

                                var modeloBase64 = modelo.Modelo != null && modelo.Modelo.Length > 0
                                    ? Convert.ToBase64String(modelo.Modelo)
                                    : string.Empty;

                                if (modeloBase64.Length > 180)
                                    modeloBase64 = modeloBase64.Substring(0, 180);

                                col.Item().Text($"Modelo: {modeloBase64}");
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
