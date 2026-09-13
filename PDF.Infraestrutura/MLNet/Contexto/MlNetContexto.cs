using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PDF.Dominio.MLNet.Entidade;
using PDF.Dominio.MLNet.InterfaceRepositorio;

namespace PDF.Infraestrutura.MLNet.Contexto
{
    public class MlNetContexto : IModeloTreinamentoRepositorio
    {
        private readonly IConfiguration _configuration;

        public MlNetContexto(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<ModeloTreinamento>> ObterModelosAsync(CancellationToken cancellationToken = default)
        {
            var connectionString = _configuration.GetConnectionString("ConexaoServidorMlNet")
                ?? "Server=DESKTOP-783G0M0;Database=MLNet;User Id=sa;Password=SenhaForte123!;TrustServerCertificate=True;Encrypt=True;";

            var modelos = new List<ModeloTreinamento>();

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            const string sql = @"
                SELECT Id, NomeModelo, DadosModelo, DataTreinamento, Versao, Quantidade
                FROM ModeloML
                ORDER BY DataTreinamento DESC;";

            await using var command = new SqlCommand(sql, connection);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                modelos.Add(new ModeloTreinamento
                {
                    Id = reader.GetInt32(0),
                    Nome = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    Versao = reader.IsDBNull(4) ? string.Empty : reader.GetInt32(4).ToString(),
                    DataTreinamento = reader.IsDBNull(3) ? DateTime.Now : reader.GetDateTime(3),
                    Modelo = reader.IsDBNull(2) ? Array.Empty<byte>() : reader.GetFieldValue<byte[]>(2)
                });
            }

            return modelos;
        }

        public async Task SalvarModeloAsync(ModeloTreinamento modelo, CancellationToken cancellationToken = default)
        {
            var connectionString = _configuration.GetConnectionString("ConexaoServidorMlNet")
                ?? "Server=DESKTOP-783G0M0;Database=MLNet;User Id=sa;Password=SenhaForte123!;TrustServerCertificate=True;Encrypt=True;";

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            const string sql = @"
                INSERT INTO ModeloML (NomeModelo, DadosModelo, DataTreinamento, Versao, Quantidade)
                VALUES (@Nome, @Modelo, @DataTreinamento, @Versao, @Quantidade);";

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Nome", modelo.Nome);
            command.Parameters.AddWithValue("@Modelo", modelo.Modelo ?? Array.Empty<byte>());
            command.Parameters.AddWithValue("@DataTreinamento", modelo.DataTreinamento);
            command.Parameters.AddWithValue("@Versao", Convert.ToInt32(modelo.Versao));
            command.Parameters.AddWithValue("@Quantidade", 0);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}
