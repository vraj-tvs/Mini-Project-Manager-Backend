using Microsoft.EntityFrameworkCore;
using ProjectManagerAPI.Models;

namespace ProjectManagerAPI.Data
{
    public class TaskManagerContext : DbContext
    {
        public TaskManagerContext(DbContextOptions<TaskManagerContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Models.Task> Tasks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(options => options.CommandTimeout(30));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Username).IsUnique();
            });

            // Configure Project entity
            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithMany(p => p.Projects)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Task entity
            modelBuilder.Entity<Models.Task>(entity =>
            {
                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
