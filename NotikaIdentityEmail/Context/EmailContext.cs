using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NotikaIdentityEmail.Entities;

namespace NotikaIdentityEmail.Context;

public class EmailContext : IdentityDbContext<AppUser>
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("YoureServerName;initial Catalog=NotikaEmailDb; integrated security=true;TrustServerCertificate=True;");
    }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Natification> Natifications { get; set; }
    public DbSet<Comment> Comments { get; set; }

}
