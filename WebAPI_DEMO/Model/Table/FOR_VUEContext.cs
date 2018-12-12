using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WebAPI_DEMO.Model.Table
{
    public partial class FOR_VUEContext : DbContext
    {
        public FOR_VUEContext()
        {
        }

        public FOR_VUEContext(DbContextOptions<FOR_VUEContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AccountData> AccountData { get; set; }

     

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountData>(entity =>
            {
                entity.HasKey(e => e.Account);

                entity.Property(e => e.Account)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Data)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.PassWord)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });
        }
    }
}
