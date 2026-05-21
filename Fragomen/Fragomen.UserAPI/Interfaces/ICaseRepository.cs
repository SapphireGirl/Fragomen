using Fragomen.UserAPI.Models;
using System.Data;

namespace Fragomen.UserAPI.Interfaces
{
    public interface ICaseRepository
    {
        Task<Case> GetCase_PartiesByCaseIdAsync(int caseId, CancellationToken cancellationToken = default);

        Task<Case> GetCaseStatus(int caseId, CancellationToken cancellationToken = default);

        Task<bool> UpdateCaseStatusAsync(int caseId, string newStatus, CancellationToken cancellationToken = default);
        Task<Case> GetCaseByIdAsync(int caseId, CancellationToken cancellationToken = default);
    }
}
    