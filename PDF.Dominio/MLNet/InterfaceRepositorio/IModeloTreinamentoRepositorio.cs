using PDF.Dominio.MLNet.Entidade;

namespace PDF.Dominio.MLNet.InterfaceRepositorio
{
    public interface IModeloTreinamentoRepositorio
    {
        Task<List<ModeloTreinamento>> ObterModelosAsync(CancellationToken cancellationToken = default);
        Task SalvarModeloAsync(ModeloTreinamento modelo, CancellationToken cancellationToken = default);
    }
}
