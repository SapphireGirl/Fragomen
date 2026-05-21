using Fragomen.UserAPI.Enums;

namespace Fragomen.UserAPI.Interfaces
{
    public interface ICaseStatusTransitionPolicy
    {
        bool CanTransition(
            CaseStatus currentStatus,
            CaseStatus newStatus,
            bool hasOverridePermission);
    }
}
