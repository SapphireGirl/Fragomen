namespace Fragomen.UserAPI.Models
{
    public class Party
    {
        public int PartyId { get; set; }
        public string PartyType { get; set; } = null!; // "Individual" | "Organization"
        public string? GivenName { get; set; }
        public string? FamilyName { get; set; }
        public string? OrganizationName { get; set; }

        // computed DB column; include getter for convenience (matches SQL persisted expression)
        public string? DisplayName =>
            string.IsNullOrWhiteSpace(OrganizationName)
                ? string.IsNullOrWhiteSpace(GivenName) && string.IsNullOrWhiteSpace(FamilyName)
                    ? null
                    : $"{GivenName} {FamilyName}".Trim()
                : OrganizationName;

        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? StateProvince { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public bool IsArchived { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        // Navigation
        public List<CaseParty> CaseParties { get; set; } = new();

    }
}
