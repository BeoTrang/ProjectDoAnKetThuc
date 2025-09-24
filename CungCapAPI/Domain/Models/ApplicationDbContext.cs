using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CungCapAPI.Domain.Models;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TbAlert> TbAlerts { get; set; }

    public virtual DbSet<TbCommand> TbCommands { get; set; }

    public virtual DbSet<TbDevice> TbDevices { get; set; }

    public virtual DbSet<TbDeviceType> TbDeviceTypes { get; set; }

    public virtual DbSet<TbNotification> TbNotifications { get; set; }

    public virtual DbSet<TbRole> TbRoles { get; set; }

    public virtual DbSet<TbUser> TbUsers { get; set; }

    public virtual DbSet<TbUserDeviceRole> TbUserDeviceRoles { get; set; }

    public virtual DbSet<TbUserLogin> TbUserLogins { get; set; }

    public virtual DbSet<TbUserTelegram> TbUserTelegrams { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=SqlServer");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TbAlert>(entity =>
        {
            entity.HasKey(e => e.AlertId).HasName("PK__tb_alert__4B8FB03A2B705EBA");

            entity.ToTable("tb_alert");

            entity.Property(e => e.AlertId).HasColumnName("alert_id");
            entity.Property(e => e.Condition).HasColumnName("condition");
            entity.Property(e => e.DeviceId).HasColumnName("device_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.ResolvedAt)
                .HasColumnType("datetime")
                .HasColumnName("resolved_at");
            entity.Property(e => e.ResolvedBy).HasColumnName("resolved_by");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("open")
                .HasColumnName("status");
            entity.Property(e => e.TriggeredAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("triggered_at");

            entity.HasOne(d => d.Device).WithMany(p => p.TbAlerts)
                .HasForeignKey(d => d.DeviceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_alert_device");

            entity.HasOne(d => d.ResolvedByNavigation).WithMany(p => p.TbAlerts)
                .HasForeignKey(d => d.ResolvedBy)
                .HasConstraintName("FK_alert_resolved_by");
        });

        modelBuilder.Entity<TbCommand>(entity =>
        {
            entity.HasKey(e => e.CommandId).HasName("PK__tb_comma__F536D5DA6C472472");

            entity.ToTable("tb_command");

            entity.Property(e => e.CommandId).HasColumnName("command_id");
            entity.Property(e => e.Command)
                .HasMaxLength(200)
                .HasColumnName("command");
            entity.Property(e => e.DeviceId).HasColumnName("device_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("pending")
                .HasColumnName("status");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("timestamp");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Device).WithMany(p => p.TbCommands)
                .HasForeignKey(d => d.DeviceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_command_device");

            entity.HasOne(d => d.User).WithMany(p => p.TbCommands)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_command_user");
        });

        modelBuilder.Entity<TbDevice>(entity =>
        {
            entity.HasKey(e => e.DeviceId).HasName("PK__tb_devic__3B085D8B61379083");

            entity.ToTable("tb_device");

            entity.Property(e => e.DeviceId).HasColumnName("device_id");
            entity.Property(e => e.FirmwareVersion)
                .HasMaxLength(50)
                .HasColumnName("firmware_version");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.RegisteredAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("registered_at");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("active")
                .HasColumnName("status");
            entity.Property(e => e.TypeId)
                .HasMaxLength(50)
                .HasColumnName("type_id");

            entity.HasOne(d => d.Type).WithMany(p => p.TbDevices)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_device_type");
        });

        modelBuilder.Entity<TbDeviceType>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PK__tb_devic__2C0005989CB49277");

            entity.ToTable("tb_device_type");

            entity.Property(e => e.TypeId)
                .HasMaxLength(50)
                .HasColumnName("type_id");
            entity.Property(e => e.NameType)
                .HasMaxLength(100)
                .HasColumnName("name_type");
        });

        modelBuilder.Entity<TbNotification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__tb_notif__E059842FF8A42661");

            entity.ToTable("tb_notification");

            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeviceId).HasColumnName("device_id");
            entity.Property(e => e.Message)
                .HasMaxLength(500)
                .HasColumnName("message");
            entity.Property(e => e.NotificationType)
                .HasMaxLength(20)
                .HasColumnName("notification_type");
            entity.Property(e => e.ReadAt)
                .HasColumnType("datetime")
                .HasColumnName("read_at");
            entity.Property(e => e.ReceiverId).HasColumnName("receiver_id");
            entity.Property(e => e.SenderId).HasColumnName("sender_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("unread")
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");

            entity.HasOne(d => d.Device).WithMany(p => p.TbNotifications)
                .HasForeignKey(d => d.DeviceId)
                .HasConstraintName("FK_notification_device");

            entity.HasOne(d => d.Receiver).WithMany(p => p.TbNotificationReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .HasConstraintName("FK_notification_receiver");

            entity.HasOne(d => d.Sender).WithMany(p => p.TbNotificationSenders)
                .HasForeignKey(d => d.SenderId)
                .HasConstraintName("FK_notification_sender");
        });

        modelBuilder.Entity<TbRole>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__tb_role__760965CC36BE651D");

            entity.ToTable("tb_role");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleDescribe)
                .HasMaxLength(200)
                .HasColumnName("role_describe");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .HasColumnName("role_name");
            entity.Property(e => e.RoleScope)
                .HasMaxLength(50)
                .HasColumnName("role_scope");
        });

        modelBuilder.Entity<TbUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__tb_user__B9BE370FE557A69E");

            entity.ToTable("tb_user");

            entity.HasIndex(e => e.Email, "UQ__tb_user__AB6E616472195634").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.GlobalRoleId).HasColumnName("global_role_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phone_number");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("active")
                .HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("update_at");

            entity.HasOne(d => d.GlobalRole).WithMany(p => p.TbUsers)
                .HasForeignKey(d => d.GlobalRoleId)
                .HasConstraintName("FK_user_role");
        });

        modelBuilder.Entity<TbUserDeviceRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.DeviceId, e.RoleId }).HasName("PK__tb_user___D778BBB2B8490355");

            entity.ToTable("tb_user_device_role");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.DeviceId).HasColumnName("device_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Device).WithMany(p => p.TbUserDeviceRoles)
                .HasForeignKey(d => d.DeviceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_udr_device");

            entity.HasOne(d => d.Role).WithMany(p => p.TbUserDeviceRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_udr_role");

            entity.HasOne(d => d.User).WithMany(p => p.TbUserDeviceRoles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_udr_user");
        });

        modelBuilder.Entity<TbUserLogin>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__tb_user___B9BE370FDEE56650");

            entity.ToTable("tb_user_login");

            entity.HasIndex(e => e.AccountLogin, "UQ__tb_user___0313548553CB0E86").IsUnique();

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.AccountLogin)
                .HasMaxLength(50)
                .HasColumnName("account_login");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.LastLoginAt)
                .HasColumnType("datetime")
                .HasColumnName("last_login_at");
            entity.Property(e => e.PasswordLogin).HasColumnName("password_login");
            entity.Property(e => e.SaltLogin).HasColumnName("salt_login");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("update_at");

            entity.HasOne(d => d.User).WithOne(p => p.TbUserLogin)
                .HasForeignKey<TbUserLogin>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_user_login_user");
        });

        modelBuilder.Entity<TbUserTelegram>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__tb_user___B9BE370F79F4914C");

            entity.ToTable("tb_user_telegram");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.TeleBotId)
                .HasMaxLength(100)
                .HasColumnName("tele_bot_id");
            entity.Property(e => e.TeleChatId)
                .HasMaxLength(100)
                .HasColumnName("tele_chat_id");

            entity.HasOne(d => d.User).WithOne(p => p.TbUserTelegram)
                .HasForeignKey<TbUserTelegram>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_user_telegram_user");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
