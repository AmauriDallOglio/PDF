namespace PDF.Aplicacao.Dto
{
    public class AppSettingsDto
    {
        public LoggingDto Logging { get; set; } = new LoggingDto();
        public ConnectionStringsDto ConnectionStrings { get; set; } = new ConnectionStringsDto();
        public RateLimitDto RateLimit { get; set; } = new RateLimitDto();
        public SegurancaDto Seguranca { get; set; } = new SegurancaDto();
    }

    public class RateLimitDto
    {
        public bool Habilitado { get; set; } = true;
        public int RequisicoesPermitidas { get; set; } = 100;
        public int JanelaEmSegundos { get; set; } = 60;
        public int TamanhoFila { get; set; } = 0;
    }

    public class ConnectionStringsDto
    {
        public string ConexaoServidor { get; set; } = string.Empty;
        public string ConexaoServidorQuery { get; set; } = string.Empty;
        public string ConexaoServidorOllama { get; set; } = string.Empty;
        public string ConexaoServidorRag { get; set; } = string.Empty;
    }

    public class LoggingDto
    {
        public LogLevelDto LogLevel { get; set; } = new LogLevelDto();
    }

    public class LogLevelDto
    {
        public string Default { get; set; } = string.Empty;
        [System.Text.Json.Serialization.JsonPropertyName("Microsoft.AspNetCore")]
        public string MicrosoftAspNetCore { get; set; } = string.Empty;
    }

    public class SegurancaDto
    {
        public string ApiKey { get; set; } = string.Empty;
    }
}
