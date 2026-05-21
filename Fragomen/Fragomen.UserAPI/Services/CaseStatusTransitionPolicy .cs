using Fragomen.UserAPI.Enums;
using Fragomen.UserAPI.Interfaces;

namespace Fragomen.UserAPI.Services
{
    public class CaseStatusTransitionPolicy : ICaseStatusTransitionPolicy
    {
        // Define allowable transitions
        private static readonly Dictionary<CaseStatus, CaseStatus[]> AllowedTransitions = new()
        {
            [CaseStatus.Intake] = new[] { CaseStatus.Active },
            [CaseStatus.Active] = new[] { CaseStatus.Pending, CaseStatus.Closed },
            [CaseStatus.Pending] = new[] { CaseStatus.Active, CaseStatus.Closed },

            [CaseStatus.Closed] = Array.Empty<CaseStatus>() // No transitions allowed from Closed

        };

        public bool CanTransition(CaseStatus current, CaseStatus next, bool hasOverride)
        {
            if (current == CaseStatus.Closed)
                return hasOverride; // Only allow transition from Closed if override is provided
            return AllowedTransitions.TryGetValue(current, out var targets) &&
                targets.Contains(next);
        }
    }
}
