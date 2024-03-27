using Microsoft.EntityFrameworkCore;

namespace wxWebApi.Models
{
    public class WxFoldersContext : DbContext
    {
        public DbSet<WxFolders> WxFolders { get; set; }

        public WxFoldersContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=D_DOCMANAGEMENT;Trusted_Connection=True;TrustServerCertificate=true");
        }

    }
}
