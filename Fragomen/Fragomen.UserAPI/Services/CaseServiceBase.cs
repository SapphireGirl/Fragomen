using Fragomen.UserAPI.Interfaces;
using Fragomen.UserAPI.Models;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;


namespace Fragomen.UserAPI.Services
{
    public abstract class CaseServiceBase
    {
        protected readonly ICaseRepository _caseRepository;

        protected CaseServiceBase(ICaseRepository caseRepository)
        {
            _caseRepository = caseRepository ?? throw new ArgumentNullException(nameof(caseRepository));
        }

        // This method can be overridden in a test or subclass.
        public virtual async Task<Case?> GetCaseAsync(int caseId, CancellationToken cancellationToken = default)
        {
            return await _caseRepository.GetCaseByIdAsync(caseId);
        }

        // Virtual group logic for test or extension
        public virtual List<(string StateProvince, List<Party> Parties, int Count)>GroupPartiesByStateProvince(Case caseDetails)
        {
            return caseDetails.CaseParties
                .Where(cp => cp.Party != null && !string.IsNullOrWhiteSpace(cp.Party.StateProvince))
                .GroupBy(cp => cp.Party!.StateProvince!)
                .Select(g => (
                    StateProvince: g.Key!,
                    Parties: g.Select(cp => cp.Party!).ToList(),
                    Count: g.Count()
                )).ToList();
        }
    }
}
