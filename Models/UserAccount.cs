namespace Contoso_BookStore_AccountService.Models
{
    public class UserAccount
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordSalt { get; set; }
        public string PasswordHash { get; set; }
        public DateTime DateAdded { get; set; }
    }
}