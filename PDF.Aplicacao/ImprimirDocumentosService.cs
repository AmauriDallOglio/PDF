using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace PDF.Aplicacao
{
    public class ImprimirDocumentosService
    {
        private readonly IConfiguration _configuration;

        public ImprimirDocumentosService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> ImprimirDocumentosAsync(CancellationToken cancellationToken = default)
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

            var downloadsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Downloads");

            Directory.CreateDirectory(downloadsPath);

            var nomeArquivo = "DocumentosImportados_PDF_AMA2026.pdf";
            var arquivoDestino = Path.Combine(downloadsPath, nomeArquivo);

            var pdf = GerarPdf(documentos);
            await File.WriteAllBytesAsync(arquivoDestino, pdf, cancellationToken);

            return arquivoDestino;
        }

        private static byte[] GerarPdf(List<DocumentoImportadoDto> documentos)
        {
            var linhas = new List<string>();
            linhas.Add("Documentos importados no banco RAG");
            linhas.Add("Chave PDF_AMA2026");
            linhas.Add(string.Empty);

            foreach (var documento in documentos)
            {
                linhas.Add($"[{documento.Id}] {documento.Titulo}");
                linhas.Add($"Tipo: {documento.TipoArquivo} | Tamanho: {documento.TamanhoArquivo ?? 0} bytes | Importado: {documento.DataImportacao:yyyy-MM-dd HH:mm:ss}");
                linhas.Add($"Texto: {documento.Texto?.Substring(0, Math.Min(120, documento.Texto.Length))}");
                linhas.Add(string.Empty);
            }

            if (documentos.Count == 0)
            {
                linhas.Add("Nenhum documento encontrado na base do RAG.");
            }

            var conteudo = string.Join("\n", linhas);
            var stream = string.Empty;

            var text = conteudo
                .Replace("\\", "\\\\")
                .Replace("(", "\\(")
                .Replace(")", "\\)");

            var lines = text.Split('\n');
            var streamBuilder = new StringBuilder();
            streamBuilder.AppendLine("BT");
            streamBuilder.AppendLine("/F1 11 Tf");
            streamBuilder.AppendLine("50 800 Td");

            for (var i = 0; i < lines.Length; i++)
            {
                var escaped = lines[i].Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");
                if (i == 0)
                {
                    streamBuilder.AppendLine($"({escaped}) Tj");
                }
                else
                {
                    streamBuilder.AppendLine("0 -16 Td");
                    streamBuilder.AppendLine($"({escaped}) Tj");
                }
            }

            streamBuilder.AppendLine("ET");
            stream = streamBuilder.ToString();

            var objects = new List<string>
            {
                "<< /Type /Catalog /Pages 2 0 R >>",
                "<< /Type /Pages /Kids [3 0 R] /Count 1 >>",
                "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Resources << /Font << /F1 4 0 R >> >> /Contents 5 0 R >>",
                "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>",
                $"<< /Length {Encoding.ASCII.GetByteCount(stream)} >>\nstream\n{stream}endstream"
            };

            using var pdf = new MemoryStream();
            var writer = new StreamWriter(pdf, Encoding.ASCII, leaveOpen: true);
            writer.Write("%PDF-1.4\n");
            writer.Flush();

            var offsets = new List<int> { 0 };

            for (var i = 0; i < objects.Count; i++)
            {
                offsets.Add((int)pdf.Position);
                writer.Write($"{i + 1} 0 obj\n{objects[i]}\nendobj\n");
                writer.Flush();
            }

            var xrefPosition = (int)pdf.Position;
            writer.Write($"xref\n0 {objects.Count + 1}\n0000000000 65535 f \n");

            for (var i = 1; i < offsets.Count; i++)
            {
                writer.Write($"{offsets[i]:D10} 00000 n \n");
            }

            writer.Write($"trailer\n<< /Size {objects.Count + 1} /Root 1 0 R >>\nstartxref\n{xrefPosition}\n%%EOF");
            writer.Flush();

            return pdf.ToArray();
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
