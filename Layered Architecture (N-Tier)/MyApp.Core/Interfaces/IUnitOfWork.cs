namespace MyApp.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAuthorRepository Authors { get; }
        IBookRepository Books { get; }

        Task<int> CompleteAsync();
    }
}