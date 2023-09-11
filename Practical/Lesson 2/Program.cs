using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.ComponentModel.DataAnnotations;
class Program
{
    public static void Main()
    {

    }
}

class myContext : DbContext
{
    public DbSet<MyTable> MyTables { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(@"Host=localhost:5434;User ID=postgres;Password=;Database=Lesson2db;Maximum Pool Size=200");
    }
}

class MyTable
{
    [Key]
    public int ID { get; set; }
    public string Name { get; set; }
}