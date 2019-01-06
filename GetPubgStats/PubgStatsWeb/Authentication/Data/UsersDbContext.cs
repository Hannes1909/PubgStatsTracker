using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PubgStatsWeb.Authentication.Data
{
    public class UsersDbContext : DbContext
    {
        public UsersDbContext(DbContextOptions options) : base(options)
        {
            this.Database.EnsureCreated();
        }

        public DbSet<IdentityUserLogin<string>> UserLogins { get; set; }
        public DbSet<IdentityUserClaim<string>> UserClaims { get; set; }
        public DbSet<IdentityUser> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityUserLogin<string>>().HasKey(_user => _user.UserId);

            builder.Entity<IdentityUserLogin<string>>().Property(_user => _user.UserId).HasMaxLength(127);

            builder.Entity<IdentityUser>().HasKey(_user => _user.Id);
            builder.Entity<IdentityUser>().Property(_user => _user.Id).HasMaxLength(127);

            builder.Entity<IdentityUserClaim<string>>().HasKey(_user => _user.Id);
            builder.Entity<IdentityUserClaim<string>>().Property(_user => _user.Id).HasMaxLength(127);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
        }
    }
}
