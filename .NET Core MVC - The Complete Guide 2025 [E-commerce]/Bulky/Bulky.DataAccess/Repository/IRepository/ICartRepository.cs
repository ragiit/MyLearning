using Bulky.Models.Models;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface ICartRepository : IRepository<Cart>
    {
        void Update(Cart Cart);
    }
}