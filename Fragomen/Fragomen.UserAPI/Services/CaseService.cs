using Fragomen.UserAPI.Enums;
using Fragomen.UserAPI.Interfaces;
using Fragomen.UserAPI.Models;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Fragomen.UserAPI.Services
{
    public class CaseService : CaseServiceBase, ICaseService
    {
        private readonly ILogger<CaseService> _logger;
        private readonly ICaseStatusTransitionPolicy _caseStatusTransitionPolicy;
        private readonly IUserPermissionService _userPermissionService;

        public CaseService(ICaseRepository caseRepository, 
                           ILogger<CaseService> logger,
                           ICaseStatusTransitionPolicy caseStatusTransitionPolicy,
                           IUserPermissionService userPermissionService) : base(caseRepository)
        {
            _logger = logger;
            _caseStatusTransitionPolicy = caseStatusTransitionPolicy;
            _userPermissionService = userPermissionService;
        }

        public async Task<Case> GetCaseDetailsAsync(int caseId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving case with ID {CaseId}", caseId);

            var caseDetails = await _caseRepository.GetCase_PartiesByCaseIdAsync(caseId, cancellationToken);

            if (caseDetails == null)
            {
                _logger.LogWarning("No case found for ID {CaseId}", caseId);
            }
            

            // Business logic example: mark case as high-value if it has a settlement amount > $1M
            if (caseDetails?.SettlementAmount > 1_000_000)
            {
                _logger.LogInformation("High-value case detected (SettlementAmount: {Amount})", caseDetails.SettlementAmount);
            }

            return caseDetails;
        }

        public override List<(string StateProvince, List<Party> Parties, int Count)> GroupPartiesByStateProvince(Case caseDetails)
        {
            if (caseDetails == null || caseDetails.CaseParties == null)
                return new List<(string, List<Party>, int)>();

            var partiesByStateProvince = caseDetails.CaseParties
                    .Select(cp => cp.Party)
                    .Where(p => p is not null && !string.IsNullOrWhiteSpace(p.StateProvince))
                    .GroupBy(p => p!.StateProvince!)
                    .Select(g => (
                        StateProvince: g.Key!,                     // string (never null after filter)
                        Parties: g.Select(p => p!).ToList(),       // List<Party> (never null)
                        Count: g.Count()
                    ))
                    .ToList();

            return partiesByStateProvince;
        }

        public async Task<List<(string StateProvince, List<Party> Parties, int Count)>> GetGroupByForCaseDetails(int caseId, CancellationToken cancellationToken = default)
        {
            var groupByDetails = new List<(string StateProvince, List<Party> Parties, int Count)>();
            try
            {
                var cssDetails = await _caseRepository.GetCase_PartiesByCaseIdAsync(caseId, cancellationToken);
                if (cssDetails != null)
                {
                    groupByDetails = GroupPartiesByStateProvince(cssDetails);
                    return groupByDetails;
                }

                return groupByDetails;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> ValidateStatusChangeAsync(
            int caseId, string newStatus, Users currentUser, CancellationToken cancellationToken = default)
        {
            var myCase = await _caseRepository.GetCaseStatus(caseId, cancellationToken);

            if (!Enum.TryParse<CaseStatus>(myCase.Status, true, out var currentStatus))
            {
                _logger.LogWarning("Invalid current status {CurrentStatus} for case ID {CaseId}", myCase.Status, caseId);
                return false;
            }
            if (!Enum.TryParse<CaseStatus>(newStatus, true, out var newCaseStatus))
            {
                _logger.LogWarning("Invalid new status {NewStatus} for case ID {CaseId}", newStatus, caseId);
                return false;
            }

            var hasOverride = await _userPermissionService.HasOverridePermissionAsync(currentUser, caseId);

            var allowed = _caseStatusTransitionPolicy.CanTransition(currentStatus, newCaseStatus, hasOverride);
            if (!allowed)
                _logger.LogWarning("Invalid status transition: {CurrentStatus} → {NewStatus} [Override? {HasOverride}] on Case {CaseId}",
                    currentStatus, newCaseStatus, hasOverride, caseId);

            return allowed;
        }
        public async Task<bool> UpdateCaseStatusAsync(int caseId, string newStatus, CancellationToken cancellationToken = default)
        {
            var isValid = await ValidateStatusChangeAsync(caseId, newStatus, null, cancellationToken);

            if (!isValid)
            {
                _logger.LogWarning("Status change validation failed for case ID {CaseId} to new status {NewStatus}", caseId, newStatus);
                return false;
            }

            await _caseRepository.UpdateCaseStatusAsync(caseId, newStatus, cancellationToken);
            return await Task.FromResult(true);

        }

        public override async Task<Case?> GetCaseAsync(int caseId, CancellationToken cancellationToken = default)
        {
            try
            {
                var caseDetails = await _caseRepository.GetCase_PartiesByCaseIdAsync(caseId, cancellationToken);
                if (caseDetails == null)
                {
                    _logger.LogWarning("No case found for ID {CaseId}", caseId);
                    return null;
                }
                return caseDetails;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving case details for case ID {CaseId}", caseId);
                throw;
            }

        }
    }
}
