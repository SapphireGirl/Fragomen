using Fragomen.UserAPI.Models;

namespace Fragomen.UserAPI.Interfaces
{
    public interface IPartyRepository
    {
        public Task<Party> GetPartyWithCasesAsync(int partyId);
    }
}
