using Fragomen.UserAPI.Interfaces;
using Fragomen.UserAPI.Models;
using System.Data;
using Dapper;

namespace Fragomen.UserAPI.Services
{
    public class UserPermissionService : IUserPermissionService
    {
        private readonly IDbConnection _dbConnection;

        public UserPermissionService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<bool> HasOverridePermissionAsync(Users user, int caseId)
        {
            if (user == null) return false;

            var result = await _dbConnection.QueryFirstOrDefaultAsync<bool>(
                "dbo.HasOverridePermission",
                new { UserId = user.Id },
                commandType: CommandType.StoredProcedure);

            return result;
        }
    }
}
