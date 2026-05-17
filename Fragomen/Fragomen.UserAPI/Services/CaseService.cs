namespace Fragomen.UserAPI.Services
{
    using Fragomen.UserAPI.Interfaces;
    using Fragomen.UserAPI.Models;
    using Microsoft.Extensions.Logging;

    public class CaseService : ICaseService
    {
        private readonly ICaseRepository _caseRepository;
        private readonly ILogger<CaseService> _logger;

        public CaseService(ICaseRepository caseRepository, ILogger<CaseService> logger)
        {
            _caseRepository = caseRepository;
            _logger = logger;
        }

        public async Task<Case> GetCaseDetailsAsync(int caseId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving case with ID {CaseId}", caseId);

            var caseDetails = await _caseRepository.GetCase_PartiesByCaseIdAsync(caseId, cancellationToken);

            if (caseDetails == null)
            {
                _logger.LogWarning("No case found for ID {CaseId}", caseId);
            }
            else
            {
                _logger.LogInformation("Retrieved case: {CaseNumber}, with {PartyCount} parties", caseDetails.CaseNumber, caseDetails.CaseParties?.Count ?? 0);
            }

            // Business logic example: mark case as high-value if it has a settlement amount > $1M
            if (caseDetails?.SettlementAmount > 1_000_000)
            {
                _logger.LogInformation("High-value case detected (SettlementAmount: {Amount})", caseDetails.SettlementAmount);
            }

            return caseDetails;
        }

        public async Task<bool> ValidateStatusChangeAsync(int caseId, string newStatus, CancellationToken cancellationToken = default)
        {
            try
            {
                // validation rules: intake -> active, active -> pending/closed, pending -> active/closed, closed -> no changes
                var myCase = await _caseRepository.GetCaseStatus(caseId, cancellationToken);

                switch (myCase.Status)
                {
                    case "intake":
                        // intake can only move to active
                        if (!newStatus.Equals("active", StringComparison.OrdinalIgnoreCase))
                        {
                            _logger.LogWarning("Invalid status change from {CurrentStatus} to {NewStatus} for case ID {CaseId}", myCase.Status, newStatus, caseId);
                            return false;
                        }
                        return newStatus.Equals("active", StringComparison.OrdinalIgnoreCase);

                    case "active":
                        // active can only move to pending or closed
                        if (!newStatus.Equals("pending", StringComparison.OrdinalIgnoreCase) || !newStatus.Equals("closed", StringComparison.OrdinalIgnoreCase))
                        {
                            _logger.LogWarning("Invalid status change from {CurrentStatus} to {NewStatus} for case ID {CaseId}", myCase.Status, newStatus, caseId);
                            return false;
                        }
                        return true; // both pending and closed are valid

                    case "pending":
                        // pending can only move to active or closed
                        if (!newStatus.Equals("active", StringComparison.OrdinalIgnoreCase) && !newStatus.Equals("closed", StringComparison.OrdinalIgnoreCase))
                        {
                            _logger.LogWarning("Invalid status change from {CurrentStatus} to {NewStatus} for case ID {CaseId}", myCase.Status, newStatus, caseId);
                            return false;
                        }
                        return true; // both active and closed are valid

                    case "closed":
                        return false; // no changes allowed from closed

                    default:
                        _logger.LogWarning("Unknown current status {CurrentStatus} for case ID {CaseId}", myCase.Status, caseId);
                        return false;
                }




            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating status change for case ID {CaseId} to new status {NewStatus}", caseId, newStatus);
                throw;
            }
        }

        public async Task<bool> UpdateCaseStatusAsync(int caseId, string newStatus, CancellationToken cancellationToken = default)
        {
            var isValid = await ValidateStatusChangeAsync(caseId, newStatus, cancellationToken);

            if (!isValid)
            {
                _logger.LogWarning("Status change validation failed for case ID {CaseId} to new status {NewStatus}", caseId, newStatus);
                return false;
            }

            await _caseRepository.UpdateCaseStatusAsync(caseId, newStatus, cancellationToken);
            return await Task.FromResult(true);

        }
    }
}
