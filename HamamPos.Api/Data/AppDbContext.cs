// EF Core DbContext: tabloları ve ilişkileri tanımlar.
// - Tickets -> TicketItems: bire-çoğ ilişki
// - Users.Username benzersiz index
// - Products.Category index (raporlarda hız)

using HamamPos.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace HamamPos.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<ServiceUnit> ServiceUnits => Set<ServiceUnit>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<TicketItem> TicketItems => Set<TicketItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<DaySession> DaySessions => Set<DaySession>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<User>().HasIndex(x => x.Username).IsUnique();
        b.Entity<Ticket>().HasMany(t => t.Items).WithOne().HasForeignKey(i => i.TicketId);
        b.Entity<Product>().HasIndex(p => p.Category);
    }
}
