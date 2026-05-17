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
    public class CaseController : Controller
    {
        private readonly ILogger<CaseController> _logger;
        private readonly ICaseRepository _caseRepository;
        private readonly IConfiguration _configuration;
        public CaseController(ILogger<CaseController> logger,
               ICaseRepository caseRepository, 
               IConfiguration configuration)
        {
            _logger = logger;
            _caseRepository = caseRepository;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpGet("GetCase_PartiesByCaseId")]
        public async Task<Case> GetCase_PartiesByCaseIdAsync(int caseId, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _caseRepository.GetCase_PartiesByCaseIdAsync(caseId, cancellationToken);
                _logger.LogInformation($"{result}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetCase_PartiesByCaseIdAsync for CaseId {caseId}: {ex.Message}");
                return new Case(); // Return empty case on error, or consider returning an appropriate error response
            }
        }
    }
}
