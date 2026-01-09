using CvBuilderBack.Helpers;
using CvBuilderBack.Models;
using Microsoft.EntityFrameworkCore;

namespace CvBuilderBack.Contexts;

public class DataContextEntityFramework(IConfiguration configuration) : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("defaultConnection"),
                options => options.EnableRetryOnFailure());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<User>().ToTable("Users", Constants.AuthentificationSchema).HasKey(user => user.Id);
        modelBuilder.Entity<Cv>().ToTable("Cvs", Constants.PublicSchema).HasKey(cv => cv.Id);
        modelBuilder.Entity<UserCvJoin>().ToTable("UserCvJoin", Constants.PublicSchema).HasKey(userCvJoin => userCvJoin.Id);
    }
}