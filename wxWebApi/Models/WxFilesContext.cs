using Microsoft.EntityFrameworkCore;

namespace wxWebApi.Models
{
    public class WxFilesContext : DbContext
    {
        public DbSet<WxFiles> WxFiles { get; set; }

        public WxFilesContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=D_DOCMANAGEMENT;Trusted_Connection=True;TrustServerCertificate=true");
        }

    }
}
