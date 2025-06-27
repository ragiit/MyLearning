namespace NDP.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IApplicationUserRepository UserRepository { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}