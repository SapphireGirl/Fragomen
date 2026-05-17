namespace Fragomen.UserAPI.Models
{
    public class CaseParty
    {
        public int CasePartyId { get; set; }
        public int CaseId { get; set; }
        public int PartyId { get; set; }
        public string Role { get; set; } = null!;
        public bool IsPrimaryContact { get; set; }
        public bool IsClient { get; set; }
        public bool IsAdverse { get; set; }
        public string? BillingResponsibility { get; set; }
        public DateTime? RepresentationStartDate { get; set; } // SQL DATE -> DateTime
        public DateTime? RepresentationEndDate { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        // Navigation
        public Party? Party { get; set; }
        public Case? Case { get; set; }

    }
}
