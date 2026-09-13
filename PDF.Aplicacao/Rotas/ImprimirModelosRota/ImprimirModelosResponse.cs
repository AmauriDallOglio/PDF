namespace PDF.Aplicacao.Rotas.ImprimirModelosRota
{
    public class ImprimirModelosResponse
    {
        public string Mensagem { get; set; } = string.Empty;
        public string CaminhoArquivo { get; set; } = string.Empty;
        public string NomeArquivo { get; set; } = string.Empty;

        public static ImprimirModelosResponse Criar(string caminhoArquivo)
        {
            return new ImprimirModelosResponse
            {
                Mensagem = "Arquivo PDF gerado com sucesso.",
                CaminhoArquivo = caminhoArquivo,
                NomeArquivo = System.IO.Path.GetFileName(caminhoArquivo)
            };
        }
    }
}
