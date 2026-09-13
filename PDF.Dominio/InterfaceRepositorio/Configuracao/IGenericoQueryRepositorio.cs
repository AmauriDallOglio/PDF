namespace PDF.Dominio.InterfaceRepositorio.Configuracao
{
    public interface IGenericoQueryRepositorio<T> where T : class
    {
        Task<List<T>> ObterTodosAsync(CancellationToken cancellationToken);
        Task<T?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);
    }
}
