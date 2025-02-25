using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TommDAL.Models
{
    public partial class organizedmumContext : DbContext
    {
        public organizedmumContext()
        {
        }

        public organizedmumContext(DbContextOptions<organizedmumContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CategoryMst> CategoryMst { get; set; }
        public virtual DbSet<FoodCategories> FoodCategories { get; set; }
        public virtual DbSet<LevelMst> LevelMst { get; set; }
        public virtual DbSet<TaskMst> TaskMst { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserChristmasHistory> UserChristmasHistory { get; set; }

        public virtual DbSet<UserHolidays> UserHolidays { get; set; }
        public virtual DbSet<UserNotes> UserNotes { get; set; }
        public virtual DbSet<UserTasksHistory> UserTasksHistory { get; set; }
        public virtual DbSet<UserTasksMst> UserTasksMst { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySQL("server=192.168.0.211;port=3306;user=organizedmum;password=WToA7FhlcRzmTyYC;database=organizedmum");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<CategoryMst>(entity =>
            {
                entity.ToTable("CategoryMst", "organizedmum");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.ImageSource)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<FoodCategories>(entity =>
            {
                entity.ToTable("FoodCategories", "organizedmum");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Categories)
                    .IsRequired()
                    .HasColumnType("json");

                entity.Property(e => e.JetpackFeaturedMediaUrl)
                    .IsRequired()
                    .HasColumnName("Jetpack_featured_media_url")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Link)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<LevelMst>(entity =>
            {
                entity.ToTable("LevelMst", "organizedmum");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.DisplayText)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.LevelName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StartDate).HasColumnType("date");
            });

            modelBuilder.Entity<TaskMst>(entity =>
            {
                entity.ToTable("TaskMst", "organizedmum");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.DayText)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.IsFriday)
                    .HasColumnType("tinyint(1)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.IsMonday)
                    .HasColumnType("tinyint(1)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.IsSaturday)
                    .HasColumnType("tinyint(1)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.IsSunday)
                    .HasColumnType("tinyint(1)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.IsThursday)
                    .HasColumnType("tinyint(1)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.IsTuesday)
                    .HasColumnType("tinyint(1)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.IsWednesday)
                    .HasColumnType("tinyint(1)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.LevelId).HasColumnType("int(11)");

                entity.Property(e => e.TasksJson)
                    .IsRequired()
                    .HasColumnName("Tasks_Json")
                    .HasColumnType("json");

                entity.Property(e => e.WeekNumber).HasColumnType("int(11)");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User", "organizedmum");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Country)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Ran)
                    .IsRequired()
                    .HasColumnName("ran")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ResetToken)
                    .HasColumnName("reset_token")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Token)
                    .IsRequired()
                    .HasColumnName("token")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserChristmasHistory>(entity =>
            {
                entity.ToTable("UserChristmasHistory", "organizedmum");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.ChristmasJobs).HasColumnType("json");

                entity.Property(e => e.TasksJson)
                    .IsRequired()
                    .HasColumnName("Tasks_Json")
                    .HasColumnType("json");

                entity.Property(e => e.UserId).HasColumnType("bigint(20)");
            });


            modelBuilder.Entity<UserHolidays>(entity =>
            {
                entity.ToTable("UserHolidays", "organizedmum");

                entity.HasIndex(e => e.UserId)
                    .HasName("UserId_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.UserId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserHolidays)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("UserId");
            });

            modelBuilder.Entity<UserNotes>(entity =>
            {
                entity.ToTable("UserNotes", "organizedmum");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.IsActive).HasColumnType("tinyint(1)");

                entity.Property(e => e.Notes)
                    .IsRequired()
                    .HasColumnType("json");

                entity.Property(e => e.UserId).HasColumnType("bigint(20)");
            });

            modelBuilder.Entity<UserTasksHistory>(entity =>
            {
                entity.ToTable("UserTasksHistory", "organizedmum");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.TaskDate).HasColumnType("date");

                entity.Property(e => e.UserId).HasColumnType("bigint(20)");

                entity.Property(e => e.UserTasksUpdate)
                    .IsRequired()
                    .HasColumnName("UserTasks_Update")
                    .HasColumnType("json");
            });

            modelBuilder.Entity<UserTasksMst>(entity =>
            {
                entity.ToTable("UserTasksMst", "organizedmum");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.IsFriday).HasColumnType("tinyint(1)");

                entity.Property(e => e.IsMonday).HasColumnType("tinyint(1)");

                entity.Property(e => e.IsSaturday).HasColumnType("tinyint(1)");

                entity.Property(e => e.IsSunday).HasColumnType("tinyint(1)");

                entity.Property(e => e.IsThursday).HasColumnType("tinyint(1)");

                entity.Property(e => e.IsTuesday).HasColumnType("tinyint(1)");

                entity.Property(e => e.IsWednesday).HasColumnType("tinyint(1)");

                entity.Property(e => e.LevelId).HasColumnType("int(11)");

                entity.Property(e => e.TaskId).HasColumnType("int(11)");

                entity.Property(e => e.TasksJson)
                    .IsRequired()
                    .HasColumnName("Tasks_Json")
                    .HasColumnType("json");

                entity.Property(e => e.UserId).HasColumnType("bigint(20)");

                entity.Property(e => e.WeekNumber).HasColumnType("int(11)");
            });
        }
    }
}
