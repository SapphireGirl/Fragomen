using System.ComponentModel.DataAnnotations;

namespace Fragomen.UserAPI.Models
{
    public class Case
    {
        [Required]
        public int CaseId { get; set; }
        [Required]
        public string CaseNumber { get; set; } = null!;
        [Required]
        public string CaseTitle { get; set; }
        [Required]
        public string Status { get; set; } // Allowed: Intake, Active, Pending, Closed
        public string? MatterType { get; set; }
        public string? PracticeArea { get; set; }
        public string? Court { get; set; }
        public string? Jurisdiction { get; set; }
        public string? Outcome { get; set; }
        public decimal? SettlementAmount { get; set; }
        public DateTime CaseCreatedAt { get; set; }
        public DateTime? CaseModifiedAt { get; set; }
        public bool IsSealed { get; set; }

        // Navigation
        public List<CaseParty> CaseParties { get; set; } = new();
    }
}
