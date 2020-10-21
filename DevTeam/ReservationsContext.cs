using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public class ReservationsContext : DbContext {
    public ReservationsContext(DbContextOptions<ReservationsContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelbuilder)
    {
        foreach (var relationship in modelbuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            relationship.DeleteBehavior = DeleteBehavior.SetNull;

        base.OnModelCreating(modelbuilder);
        modelbuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public static ReservationsContext CreateDbInRuntimeMemory(string dbname) {
        var dbOptions = new DbContextOptionsBuilder<ReservationsContext>()
            .UseInMemoryDatabase(databaseName: dbname)
            .UseLazyLoadingProxies()
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        return new ReservationsContext(dbOptions);
    }

    public DbSet<Team> Teams { get; set; }
    public DbSet<TeamMember> TeamMembers { get; set; }
}
