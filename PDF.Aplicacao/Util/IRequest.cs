namespace PDF.Aplicacao.Util
{
    public interface IRequest<TResponse> { }

    public interface IContratoBaseHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Executar(TRequest request, CancellationToken cancellationToken = default);
    }
}
