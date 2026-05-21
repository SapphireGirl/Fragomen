using Fragomen.UserAPI.Interfaces;
using Fragomen.UserAPI.Models;
using Dapper;
using System.Data;

namespace Fragomen.UserAPI.Repositories
{
    public class UsersRepository : IUserRepository
    {
        private readonly IDbConnection _connection;
        private readonly ILogger<UsersRepository> _logger;   

        public UsersRepository(IDbConnection connection, ILogger<UsersRepository> logger)
        {
            _connection = connection;
            _logger = logger;
        }
        public async Task<IEnumerable<Users>> GetAllUsers()
        {
            try
            {
                // use Dapper to execute the stored procedure and return the results as a list of User objects

                var allUsers = await _connection.QueryAsync<Users>("dbo.GetAllUsers", commandType: CommandType.StoredProcedure);
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
