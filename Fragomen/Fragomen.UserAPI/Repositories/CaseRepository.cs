using Dapper;
using Fragomen.UserAPI.Interfaces;
using Fragomen.UserAPI.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Fragomen.UserAPI.Repositories
{
    public class CaseRepository : IGenericRepository<Case>, ICaseRepository
    {
        private readonly IDbConnection _connection;
        private readonly ILogger<CaseRepository> _logger;

        public CaseRepository(IDbConnection Connection, ILogger<CaseRepository> logger)
        {
            _connection = Connection;
            _logger = logger;
        }

        public async Task<Case> GetCase_PartiesByCaseIdAsync(int caseId, CancellationToken cancellationToken = default)
        {
            if (_connection == null) throw new ArgumentNullException(nameof(_connection));

            var caseDictionary = new Dictionary<int, Case>();
            const string StoredProcName = "dbo.GetCase_PartiesByCaseId"; 

            var parameters = new DynamicParameters();
            parameters.Add("@CaseId", caseId, DbType.Int32);

            try
            {
                await _connection.QueryAsync<Case, CaseParty, Party, Case>(
                    StoredProcName,
                    (c, cp, p) =>
                    {
                        if (!caseDictionary.TryGetValue(c.CaseId, out var caseObj))
                        {
                            caseObj = c;
                            caseObj.CaseParties = caseObj.CaseParties ?? new List<CaseParty>();
                            caseDictionary.Add(caseObj.CaseId, caseObj);
                        }

                        // If no CaseParty row, skip adding
                        if (cp == null || cp.CasePartyId == 0)
                            return caseObj;

                        // ensure links
                        cp.CaseId = cp.CaseId == 0 ? caseObj.CaseId : cp.CaseId;
                        if (p != null && p.PartyId != 0)
                        {
                            cp.Party = p; // CaseParty must have public Party { get; set; }
                            cp.PartyId = cp.PartyId == 0 ? p.PartyId : cp.PartyId;
                        }

                        // avoid duplicates
                        if (!caseObj.CaseParties.Any(x => x.CasePartyId == cp.CasePartyId))
                        {
                            caseObj.CaseParties.Add(cp);
                        }

                        _logger.LogInformation($"Mapped CaseId: {caseObj.CaseId} with CasePartyId: {cp.CasePartyId} and PartyId: {p?.PartyId}");
                        return caseObj;
                    },
                    param: parameters,
                    commandType: CommandType.StoredProcedure,
                    splitOn: "CasePartyId, PartyId"

                );
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while fetching case with parties for CaseId: {caseId}. Exception: {ex} {caseId}");
                throw;
            }

            return caseDictionary.Values.FirstOrDefault();
        }

        public async Task<Case> GetCaseStatus(int caseId, CancellationToken cancellationToken = default)
        {

            // Allowed: Intake, Active, Pending, Closed

            // Here is the business logic for mmoving a case to closed.  when a case is created the default in Intake.  
            // Then we move to active
            // Then we move to pending
            // A pending case refers to a legal matter that has been filed in court but has not yet been resolved,
            // meaning no judgment has been entered or the case has not been dismissed.
            // It indicates that the case is still active and awaiting a final decision.

            // Algorithm
            // Given a caseId, return its status.

            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@CaseId", caseId, DbType.Int32);
                var sproc = "dbo.GetStatusGivenCaseId";

                var mycase = await _connection.QuerySingleAsync<Case>(sproc, parameters, commandType: CommandType.StoredProcedure);

                return mycase;
            }
            catch(Exception ex)
            {
                _logger.LogError($"ERROR fetching a case status with caseId: {caseId} {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateCaseStatusAsync(int caseId, string newStatus, CancellationToken cancellationToken = default)
        {
            // Note:
            // Never do this, it is vulnerable to SQL Injection.
            // var sql = $"UPDATE CASES SET Status = '{newStatus}' WHERE CaseId = {caseId}";

            // This should be a sproc but for demo purposes we will just write the SQL here.
            // In production code, we would want to use a stored procedure for this.
            var sproc = "dbo.UpdateStatusGivenCaseId";

            try
            {
                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@Status", newStatus, DbType.String);

                using (var conn = new SqlConnection(_connection.ConnectionString))
                {
                    var RowsChanged = await conn.ExecuteAsync(sproc, new { Status = newStatus, CaseId = caseId });
                    return RowsChanged > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR updating a case status with caseId: {caseId} {ex.Message}");
                throw;
            }
            
        }

        public Task<Case> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Case>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Case> AddAsync(Case entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
