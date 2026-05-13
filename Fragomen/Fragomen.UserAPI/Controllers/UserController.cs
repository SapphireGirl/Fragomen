using Fragomen.UserAPI.Interfaces;
using Fragomen.UserAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace Fragomen.UserAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UserController(ILogger<UserController> logger, IUserRepository userRepository, IConfiguration configuration)
        {
            _logger = logger;
            _userRepository = userRepository;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpGet("GetAllUsers")]
        public async Task<IEnumerable<User>> GetAllUsers()
        {
            try
            {
                var users = await _userRepository.GetAllUsers();
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError($"UserController: ERROR GetAllUsers failed {ex.Message}");
                return new List<User>();
            }
        }

        [AllowAnonymous]
        [HttpGet("Ping")]
        public IActionResult Ping()
        {
            return Ok("Ping reached controller");
        }

        [AllowAnonymous]
        [HttpGet("DebugDb")]
        public async Task<IActionResult> DebugDb()
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("UserAPIConnectionString"));
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = @"
            SELECT
                DB_NAME() AS DatabaseName,
                SUSER_SNAME() AS LoginName,
                USER_NAME() AS DbUserName,
                (SELECT COUNT(*) FROM dbo.[User]) AS UserCount;
        ";

                using var reader = await command.ExecuteReaderAsync();

                var results = new List<object>();

                while (await reader.ReadAsync())
                {
                    results.Add(new
                    {
                        DatabaseName = reader["DatabaseName"]?.ToString(),
                        LoginName = reader["LoginName"]?.ToString(),
                        DbUserName = reader["DbUserName"]?.ToString(),
                        UserCount = reader["UserCount"]?.ToString()
                    });
                }

                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DebugDb failed");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
