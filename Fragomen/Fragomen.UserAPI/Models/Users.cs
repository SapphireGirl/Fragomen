namespace Fragomen.UserAPI.Models
{
    public class Users
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string Status { get; set; } 
        public List<Roles> Roles { get; set; } = new();
    }
}
