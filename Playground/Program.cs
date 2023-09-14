using System.Data;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Models;

var r = new Random();
using (var myData = new ProductsDb())
{
  var sw = System.Diagnostics.Stopwatch.StartNew();

  // write queries here
  Console.WriteLine(myData.Users.Count());

  // var users = myData.Users
  //   .Skip(5)
  //   .Take(100)
  //   .Include(u => u.UserInGroups)
  //   .ThenInclude(ug => ug.Group)
  //   .ToList();

  // foreach (var u in users)
  // {
  //   Console.WriteLine($"User {u.Name} {u.Surname} is part of groups {string.Join(", ", u.UserInGroups.Select(ug => $"'{ug.Group.Description}'"))}");
  // }

  // var groups = myData.Groups
  //   .Take(100)
  //   .Include(u => u.UserInGroups)
  //   .ThenInclude(ug => ug.User)
  //   .ToList();

  // foreach (var g in groups)
  // {
  //   Console.WriteLine($"Group {g.Name} {g.Description} contains users {string.Join(", ", g.UserInGroups.Select(ug => $"'{ug.User.Name + " " + ug.User.Surname}'"))}");
  // }

  // var u1 = new User(Guid.NewGuid(), "User", "1", DateTime.Now.ToUniversalTime());
  // var u2 = new User(Guid.NewGuid(), "User", "2", DateTime.Now.ToUniversalTime());
  // var u3 = new User(Guid.NewGuid(), "User", "3", DateTime.Now.ToUniversalTime());
  // var u4 = new User(Guid.NewGuid(), "User", "4", DateTime.Now.ToUniversalTime());
  // myData.Users.AddRange(u1, u2, u3, u4);

  // var g1 = new Group(Guid.NewGuid(), "Users", "1 and 2");
  // var g2 = new Group(Guid.NewGuid(), "Users", "2, 3 and 4");
  // myData.Groups.AddRange(g1, g2);

  // myData.UsersInGroups.Add(new UserInGroup(Guid.NewGuid(), u1.Id, g1.id));
  // myData.UsersInGroups.Add(new UserInGroup(Guid.NewGuid(), u2.Id, g1.id));

  // myData.UsersInGroups.Add(new UserInGroup(Guid.NewGuid(), u2.Id, g2.id));
  // myData.UsersInGroups.Add(new UserInGroup(Guid.NewGuid(), u3.Id, g2.id));
  // myData.UsersInGroups.Add(new UserInGroup(Guid.NewGuid(), u3.Id, g2.id));


  await myData.SaveChangesAsync();

  Console.WriteLine(sw.ElapsedMilliseconds);
}


namespace Models
{
  public class ProductsDb : DbContext
  {

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Group> Groups { get; set; } = null!;
    public DbSet<UserInGroup> UsersInGroups { get; set; } = null!;

    public ProductsDb()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
      optionsBuilder
        .UseNpgsql(@"Host=localhost:5432;Username=postgres;Password=;Database=academy-products-console;Maximum Pool Size=200")
        .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<UserInGroup>()
        .HasOne(ug => ug.User)
        .WithMany(u => u.UserInGroups)
        .HasForeignKey(ug => ug.UserId);

      modelBuilder.Entity<UserInGroup>()
        .HasOne(ug => ug.Group)
        .WithMany(u => u.UserInGroups)
        .HasForeignKey(g => g.GroupId);
    }
  }

  public record User(Guid Id, string Name, string Surname, DateTime Birthday)
  {
    public List<UserInGroup> UserInGroups { get; set; } = null!;
  };
  public record Group(Guid Id, string Name, string Description)
  {
    public List<UserInGroup> UserInGroups { get; set; } = null!;
  }
  public record UserInGroup(Guid Id, Guid UserId, Guid GroupId)
  {
    public User User { get; set; } = null!;
    public Group Group { get; set; } = null!;
  };

}

