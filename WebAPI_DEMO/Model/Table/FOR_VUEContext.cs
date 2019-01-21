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
                    .HasMaxLength(30)
                    .ValueGeneratedNever();

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.PassWord)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.SendmailDate)
                    .HasColumnName("sendmail_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.SigninDate)
                    .HasColumnName("signin_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.SignupDate)
                    .HasColumnName("signup_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.SignupFinish)
                    .HasColumnName("signup_finish")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.VerificationCode)
                    .HasColumnName("verification_code")
                    .HasMaxLength(4)
                    .IsUnicode(false);
            });
        }
    }
}
