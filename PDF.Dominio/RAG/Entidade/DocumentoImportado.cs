namespace PDF.Dominio.RAG.Entidade
{
    public class DocumentoImportado
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Texto { get; set; } = string.Empty;
        public string TipoArquivo { get; set; } = string.Empty;
        public long? TamanhoArquivo { get; set; }
        public DateTime DataImportacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
    }
}
