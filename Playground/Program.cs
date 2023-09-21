using System.Data;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using System.Linq;
using System.Linq.Expressions;
using Models;

var r = new Random();
using (var myData = new ProductsDb())
{
  var sw = System.Diagnostics.Stopwatch.StartNew();
  var now = DateTime.Now.ToUniversalTime();

  // write queries here
  //Console.WriteLine(myData.Users.Count());

  // using "group" is a reservable (not useable)
  // var myQuery1 = (
  //   from g in myData.Groups //.OrderBy(g => g.Id).Skip(0).Take(100)
  //   select new { g.Name, g.Description }
  // ).ToList();

  // foreach (var row in myQuery1)
  // {
  //   Console.WriteLine($"Group: {row.Name} {row.Description}");
  // }

  var myQuery2 = (
    from u in myData.Users
    let name = u.Name
    let surname = u.Surname
    let birthday = u.Birthday
    let fullname = name + " " + surname
    where now - birthday > TimeSpan.FromDays(3)
    orderby birthday descending
    select new { FullName = fullname, Surname = surname, Birthday = birthday, u.UsersInGroups }
  ).ToList();

  foreach (var row in myQuery2)
  {
    Console.WriteLine($"Group: {row.FullName} {row.Surname} {row.Birthday} {row.UsersInGroups.Count()}");
  }

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
      modelBuilder.Entity<UserInGroup>() // 1:M
        .HasOne(ug => ug.User) // connected to one
        .WithMany(u => u.UserInGroups) // connected to group
        .HasForeignKey(ug => ug.UserId);

      modelBuilder.Entity<UserInGroup>()
        .HasOne(ug => ug.Group)
        .WithMany(g => g.UserInGroups)
        .HasForeignKey(ug => ug.GroupId);

      // a user cannot be in more then one group
      // it's a 1-N relation from user to group
      // modelBuilder.Entity<UserInGroup>()
      //     .HasIndex(u => u.UserId)
      //     .IsUnique();

      //modelBuilder.Entity<UserInGroup>()
      //     .HasIndex(u => u.GroupId)
      //     .IsUnique();
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

  // Find user id's in specific group
  // group
  // UsersInGroup uig where GroupId = group.Id
  // all Users where user.Id = uig.UserId

}

