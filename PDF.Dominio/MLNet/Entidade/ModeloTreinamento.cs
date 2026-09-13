namespace PDF.Dominio.MLNet.Entidade
{
    public class ModeloTreinamento
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Versao { get; set; } = string.Empty;
        public DateTime DataTreinamento { get; set; }
        public byte[] Modelo { get; set; } = Array.Empty<byte>();
    }
}
