namespace Fragomen.UserAPI.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<Models.User>> GetAllUsers();
    }
}
