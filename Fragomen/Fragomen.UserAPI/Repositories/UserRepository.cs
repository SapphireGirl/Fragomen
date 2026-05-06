using Fragomen.UserAPI.Interfaces;
using Dapper;
using System.Data;

namespace Fragomen.UserAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _connection;

        public UserRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        public async Task<IEnumerable<Models.User>> GetAllUsers()
        {
            try
            {
                // use Dapper to execute the stored procedure and return the results as a list of User objects

                var allUsers = await _connection.QueryAsync<Models.User>("dbo.GetAllUsers", commandType: CommandType.StoredProcedure);
                return allUsers;
            }
            catch (Exception ex)
            { 
                throw new Exception($"Error retrieving users: {ex.Message}", ex);
            }
        }
    }
}
