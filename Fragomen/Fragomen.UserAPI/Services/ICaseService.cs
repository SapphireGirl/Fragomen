using Fragomen.UserAPI.Models;

namespace Fragomen.UserAPI.Services
{
    public interface ICaseService
    {
        Task<Case> GetCaseDetailsAsync(int caseId, CancellationToken cancellationToken = default);
        Task<bool> ValidateStatusChangeAsync(int caseId, string newStatus, CancellationToken cancellationToken = default);
        Task<bool> UpdateCaseStatusAsync(int caseId, string newStatus, CancellationToken cancellationToken = default);
        Task<List<(string StateProvince, List<Party> Parties, int Count)>> GetGroupByForCaseDetails(int CaseId, CancellationToken cancellationToken = default);

        List<(string StateProvince, List<Party> Parties, int Count)> GroupPartiesByStateProvince(Case caseDetails);
    }
}
