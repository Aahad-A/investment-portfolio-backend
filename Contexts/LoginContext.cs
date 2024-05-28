namespace final_project_back_end_Aahad_A;
using Microsoft.EntityFrameworkCore;
public class LoginContext : DbContext
{
    public DbSet<Login> Logins {get;set;}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=LoginDb.db");
    }
}