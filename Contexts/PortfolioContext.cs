namespace final_project_back_end_Aahad_A;
using Microsoft.EntityFrameworkCore;
public class PortfolioContext : DbContext
{
    public DbSet<Portfolio> Portfolios { get; set; }
    public DbSet<Investment> Investments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source = Databases/PortfolioDb.db");
    }

}
