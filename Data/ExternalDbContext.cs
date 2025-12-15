using ClassHub.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassHub.Data
{
    public class ExternalDbContext : DbContext
    {
        public ExternalDbContext(DbContextOptions<ExternalDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupUser> GroupUsers { get; set; }
        public DbSet<ChatRoom> ChatRooms { get; set; }
        public DbSet<ChatRoomUser> ChatRoomUsers { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<OrganisationInvite> OrganisationInvites { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // UserRole composite key
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId, ur.OrganisationId });

            // GroupUser composite key
            modelBuilder.Entity<GroupUser>()
                .HasKey(gu => new { gu.GroupId, gu.UserId });

            // ChatRoomUser composite key
            modelBuilder.Entity<ChatRoomUser>()
                .HasKey(cru => new { cru.ChatRoomId, cru.UserId });

            // kapcsolatok példák:

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Organisation)
                .WithMany(o => o.UserRoles)
                .HasForeignKey(ur => ur.OrganisationId);

            modelBuilder.Entity<GroupUser>()
                .HasOne(gu => gu.Group)
                .WithMany(g => g.GroupUsers)
                .HasForeignKey(gu => gu.GroupId);

            modelBuilder.Entity<GroupUser>()
                .HasOne(gu => gu.User)
                .WithMany(u => u.GroupUsers)
                .HasForeignKey(gu => gu.UserId);

            modelBuilder.Entity<ChatRoomUser>()
                .HasOne(cru => cru.ChatRoom)
                .WithMany(cr => cr.ChatRoomUsers)
                .HasForeignKey(cru => cru.ChatRoomId);

            modelBuilder.Entity<ChatRoomUser>()
                .HasOne(cru => cru.User)
                .WithMany(u => u.ChatRoomUsers)
                .HasForeignKey(cru => cru.UserId);

            // RefreshToken – már DataAnnotation-nel is be van lőve, itt elég ennyi:
            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Owner" },
                new Role { Id = 2, Name = "Admin" },
                new Role { Id = 3, Name = "Member" }
);


        }

        
    }
}
