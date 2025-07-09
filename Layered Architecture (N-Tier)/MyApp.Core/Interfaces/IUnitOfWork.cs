namespace MyApp.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAuthorRepository Authors { get; }

        Task<int> CompleteAsync();
    }
}