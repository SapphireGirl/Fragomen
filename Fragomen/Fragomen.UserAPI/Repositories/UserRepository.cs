using Fragomen.UserAPI.Interfaces;
using Fragomen.UserAPI.Models;
using Dapper;
using System.Data;

namespace Fragomen.UserAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _connection;
        private readonly ILogger<UserRepository> _logger;   

        public UserRepository(IDbConnection connection, ILogger<UserRepository> logger)
        {
            _connection = connection;
            _logger = logger;
        }
        public async Task<IEnumerable<User>> GetAllUsers()
        {
            try
            {
                // use Dapper to execute the stored procedure and return the results as a list of User objects

                var allUsers = await _connection.QueryAsync<User>("dbo.GetAllUsers", commandType: CommandType.StoredProcedure);
                return allUsers;
            }
            catch (Exception ex)
            { 
                _logger.LogError($"Error retrieving users: {ex.Message}");
                throw new Exception($"Error retrieving users: {ex.Message}");
            }
        }
    }
}
