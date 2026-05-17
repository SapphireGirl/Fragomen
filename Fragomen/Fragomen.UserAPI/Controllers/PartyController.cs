using Fragomen.UserAPI.Interfaces;
using Fragomen.UserAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Fragomen.UserAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PartyController : Controller
    {
        private readonly ILogger<PartyController> _logger;
        private readonly IPartyRepository _PartyRepository;
        private readonly IConfiguration _configuration;
        public PartyController(ILogger<PartyController> logger,
                               IPartyRepository PartyRepository,
                               IConfiguration configuration)
        {
            _logger = logger;
            _PartyRepository = PartyRepository  ;
            _configuration = configuration;
        }
        [AllowAnonymous]
        [HttpGet("GetPartysByPartyId")]
        public async Task<Party> GetPartyByPartyIdAsync(int PartyId, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _PartyRepository.GetPartyWithCasesAsync(PartyId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetPartyByPartyIdAsync for PartyId {PartyId}: {ex.Message}");
                return new Party();
            }
        }
    }
}
