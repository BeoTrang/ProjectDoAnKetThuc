using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CungCapAPI.Models;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<Test> Tests { get; set; }

    public virtual DbSet<VaiTro> VaiTros { get; set; }

    //Cái này dùng để map với Store Procedure
    public DbSet<ThongTinNguoiDung> ThongTinNguoiDung { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=SqlServer");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.NguoiDungId).HasName("PK__NguoiDun__C4BBA4BDAE96871F");

            entity.ToTable("NguoiDung");

            entity.HasIndex(e => e.TaiKhoan, "UQ__NguoiDun__D5B8C7F003E9E5CB").IsUnique();

            entity.Property(e => e.CapNhatVao).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.KhoaKetThuc).HasColumnType("datetime");
            entity.Property(e => e.KichHoat).HasDefaultValue(true);
            entity.Property(e => e.LanCuoiDangNhap).HasColumnType("datetime");
            entity.Property(e => e.MatKhauMaHoa).HasMaxLength(256);
            entity.Property(e => e.MatKhauMuoi).HasMaxLength(50);
            entity.Property(e => e.TaiKhoan).HasMaxLength(100);
            entity.Property(e => e.TaoVao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TenNguoiDung).HasMaxLength(100);

            entity.HasMany(d => d.VaiTros).WithMany(p => p.NguoiDungs)
                .UsingEntity<Dictionary<string, object>>(
                    "VaiTroNguoiDung",
                    r => r.HasOne<VaiTro>().WithMany()
                        .HasForeignKey("VaiTroId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_VaiTroNguoiDung_VaiTro"),
                    l => l.HasOne<NguoiDung>().WithMany()
                        .HasForeignKey("NguoiDungId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_VaiTroNguoiDung_NguoiDung"),
                    j =>
                    {
                        j.HasKey("NguoiDungId", "VaiTroId").HasName("PK__VaiTroNg__B0CCFCAC3A3DB080");
                        j.ToTable("VaiTroNguoiDung");
                    });
        });

        modelBuilder.Entity<Test>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Test");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.Ten).HasMaxLength(50);
        });

        modelBuilder.Entity<VaiTro>(entity =>
        {
            entity.HasKey(e => e.VaiTroId).HasName("PK__VaiTro__47758116D5B8FAAF");

            entity.ToTable("VaiTro");

            entity.HasIndex(e => e.VaiTro1, "UQ__VaiTro__4A1D9824BAE8F336").IsUnique();

            entity.Property(e => e.MieuTa).HasMaxLength(256);
            entity.Property(e => e.VaiTro1)
                .HasMaxLength(50)
                .HasColumnName("VaiTro");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
