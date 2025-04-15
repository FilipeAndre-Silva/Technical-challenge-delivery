using AgendaApp.Domain.Entities;
using AgendaApp.Infrastructure.Mapping;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AgendaApp.Infrastructure.Context;

public class AgendaAppDbContext : IdentityDbContext<IdentityUserCustom, IdentityRole<Guid>, Guid>
{
    public AgendaAppDbContext(DbContextOptions<AgendaAppDbContext> options)
    : base(options) { }

    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<RequestLog> RequestLogs => Set<RequestLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ContactMap());

        base.OnModelCreating(modelBuilder);
    }
}