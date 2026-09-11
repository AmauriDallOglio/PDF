namespace PDF.Aplicacao.Rotas.ImprimirDocumentosRota
{
    public class ImprimirDocumentosResponse
    {
        public string Mensagem { get; set; } = string.Empty;
        public string CaminhoArquivo { get; set; } = string.Empty;
        public string NomeArquivo { get; set; } = string.Empty;

        public static ImprimirDocumentosResponse Criar(string caminhoArquivo)
        {
            return new ImprimirDocumentosResponse
            {
                Mensagem = "Arquivo PDF gerado com sucesso.",
                CaminhoArquivo = caminhoArquivo,
                NomeArquivo = System.IO.Path.GetFileName(caminhoArquivo)
            };
        }
    }
}
