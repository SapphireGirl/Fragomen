using Fragomen.UserAPI.Models;

namespace Fragomen.UserAPI.Interfaces
{
    public interface IUserPermissionService
    {
        Task<bool> HasOverridePermissionAsync(Users currentUser, int caseId);
    }
}
