using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

public class Program {
    private static string DBName { get; set; }

    public static void Main(string[] args) {
        DBName = "InMemDB_" + System.Guid.NewGuid().ToString();
        InitData();

        System.Console.WriteLine("Data was initialized in different dbcontext, which means the following would fail if there wasn't lazy loading support (also, it's done in a way that avoids select n+1 problem).");
        ListData();

        System.Console.WriteLine("Deleted one of the teams, listing current data; as shown foreign keys to the deleted team are set to null");
        DeleteTeam("TEST_2");
        ListData();
    }

    private static void InitData() {
        var dbContext = GetContext();
        
        var team = dbContext.Add<Team>(new Team { Name = "TEST" }).Entity;
        var secondTeam = dbContext.Add<Team>(new Team { Name = "TEST_2" }).Entity;
        dbContext.Add(new TeamMember { FirstName = "First", LastName = "Member", Team = team, Title = JobTitle.ProjectOwner });
        dbContext.Add(new TeamMember { FirstName = "Another", LastName = "Member", Team = team, Title = JobTitle.Developer });
        dbContext.Add(new TeamMember { FirstName = "Third", LastName = "Member", Team = team, Title = JobTitle.ScrumMaster });
        dbContext.Add(new TeamMember { FirstName = "Lone", LastName = "Dev", Team = secondTeam, Title = JobTitle.Developer });
        dbContext.SaveChanges();
    }

    // N + 1 problem, lazy loading
    private static void ListData() {
        var ctx = GetContext();
        foreach (var team in ctx.Teams.Include(t => t.TeamMembers).ToList()) {
            System.Console.WriteLine(team.ToString());
            foreach (var member in team.TeamMembers)
                System.Console.WriteLine("\t" + member.ToString());
            System.Console.WriteLine();
        }

        var teamlessMembers = ctx.TeamMembers.Where(m => m.Team == null).ToList();
        if (teamlessMembers.Count != 0) {
            System.Console.WriteLine("Teamless members: ");
            foreach (var member in teamlessMembers)
                System.Console.WriteLine("\t" + member.ToString());
        }
    }

    private static void DeleteTeam(string name) {
        var dbContext = GetContext();
        dbContext.Remove(dbContext.Teams.Where(t => t.Name == name).First());
        dbContext.SaveChanges();
    }

    private static ReservationsContext GetContext()
        => ReservationsContext.CreateDbInRuntimeMemory(DBName);
    
}
