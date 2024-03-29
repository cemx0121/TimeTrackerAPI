﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using TimeTrackerModelLib.Models;

namespace TimeTrackerModelLib.Data
{
    public partial class timetrackerdbContext : DbContext
    {
        public timetrackerdbContext()
        {
        }

        public timetrackerdbContext(DbContextOptions<timetrackerdbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<DailyWorkLocation> DailyWorkLocations { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserProfile> UserProfiles { get; set; }
        public virtual DbSet<Workbooth> Workbooths { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=time-tracker-ct.database.windows.net;Initial Catalog=time-tracker-db;Persist Security Info=True;User ID=cemturan;Password=Cemo2610");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DailyWorkLocation>(entity =>
            {
                entity.HasKey(e => e.WorkLocationId)
                    .HasName("PK__DailyWor__0D3898FF231E89C1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.DailyWorkLocations)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__DailyWork__UserI__01142BA1");

                entity.HasOne(d => d.Workbooth)
                    .WithMany(p => p.DailyWorkLocations)
                    .HasForeignKey(d => d.WorkboothId)
                    .HasConstraintName("FK__DailyWork__Workb__02084FDA");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK__Users__RoleId__06CD04F7");
            });

            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.HasKey(e => e.ProfileId)
                    .HasName("PK__UserProf__290C88E481893BC8");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserProfiles)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__UserProfi__UserI__7C4F7684");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}