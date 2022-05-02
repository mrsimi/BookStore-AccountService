using Contoso_BookStore_AccountService.Models;
using Microsoft.EntityFrameworkCore;

namespace Contoso_BookStore_AccountService.Data 
{
    public class ContosoBookStoreDbContext : DbContext
    {
        public ContosoBookStoreDbContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<UserAccount> UserAccounts {get; set;}
    }
}