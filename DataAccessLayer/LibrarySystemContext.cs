using LibrarySystemAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystemAPI.DataAccessLayer
{
    public class LibrarySystemContext : DbContext
    {
        public LibrarySystemContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User_Data> User_Data { get; set; }
        public DbSet<Book_Data> Book_Data { get; set; }
        public DbSet<Book_Stock> Book_Stock {  get; set; }
        public DbSet<Issued_Books> Issued_Books { get; set;}
    }
}
