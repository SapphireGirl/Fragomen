using Dapper;
using Fragomen.UserAPI.Interfaces;
using Fragomen.UserAPI.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Fragomen.UserAPI.Repositories
{
    public class PartyRepository : IPartyRepository
    {
        private readonly IDbConnection _connection;
        private readonly ILogger<UserRepository> _logger;

        public PartyRepository(IDbConnection connection, ILogger<UserRepository> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        public async Task<Party> GetPartyWithCasesAsync(int partyId)
        {
            try
            {
                // get the connection string from the configuration
                var connString = _connection.ConnectionString;

                using (var conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using var multi = conn.QueryMultiple("dbo.GetPartyWithCases",
                        new { partyId = partyId }, commandType: CommandType.StoredProcedure);

                    var party = multi.ReadSingleOrDefault<Party>();
                    if (party != null)
                    {
                        var caseRows = multi.Read<(Case caseObj, CaseParty cp)>() // alternative mapping shown below
                            .Select(t => {
                                t.cp.Case = t.caseObj;
                                return t.cp;
                            })
                            .ToList();

                        party.CaseParties = caseRows;
                    }

                    return party ?? new Party();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving party with cases: {ex.Message} partyId: {partyId}");
                throw new Exception($"Error retrieving party with cases: {ex.Message} partyId: {partyId}");
            }
        }
    }
}
