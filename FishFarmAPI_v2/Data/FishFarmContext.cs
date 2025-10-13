using System;
using System.Collections.Generic;
using FishFarmAPI_v2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; 



namespace FishFarmAPI_v2.Data;

public partial class FishFarmContext : DbContext
{
    public FishFarmContext()
    {
    }

    public FishFarmContext(DbContextOptions<FishFarmContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Fishbreed> Fishbreeds { get; set; }

    public virtual DbSet<Fishhealthstatus> Fishhealthstatuses { get; set; }

    public virtual DbSet<Iotdatum> Iotdata { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Permissiongroup> Permissiongroups { get; set; }

    public virtual DbSet<Rolegroup> Rolegroups { get; set; }

    public virtual DbSet<Sensor> Sensors { get; set; }

    public virtual DbSet<Sensortype> Sensortypes { get; set; }

    public virtual DbSet<Shopowner> Shopowners { get; set; }

    public virtual DbSet<Shopstaff> Shopstaffs { get; set; }

    public virtual DbSet<Spendingrecord> Spendingrecords { get; set; }

    public virtual DbSet<Spendingtype> Spendingtypes { get; set; }

    public virtual DbSet<Tank> Tanks { get; set; }

    public virtual DbSet<Tankcluster> Tankclusters { get; set; }

    public virtual DbSet<User> Users { get; set; }

    private string GetConnectionString()
    {
        IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();
        return configuration["ConnectionStrings:MyDbConnection"]
             ?? throw new InvalidOperationException("Connection string 'MyDbConnection' not found."); ; 
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(GetConnectionString());
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Fishbreed>(entity =>
        {
            entity.HasKey(e => e.BreedId).HasName("PK__FISHBREE__9C021435B750D9E1");

            entity.ToTable("FISHBREED");

            entity.Property(e => e.BreedId).HasColumnName("breed_id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("image_url");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
        });

        modelBuilder.Entity<Fishhealthstatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__FISHHEAL__3683B5316A0D2F75");

            entity.ToTable("FISHHEALTHSTATUS");

            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.CurrentStatus).HasColumnName("current_status");
            entity.Property(e => e.Prediction).HasColumnName("prediction");
            entity.Property(e => e.TankId).HasColumnName("tank_id");

            entity.HasOne(d => d.Tank).WithMany(p => p.Fishhealthstatuses)
                .HasForeignKey(d => d.TankId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FISHHEALT__tank___5BE2A6F2");
        });

        modelBuilder.Entity<Iotdatum>(entity =>
        {
            entity.HasKey(e => e.DataId).HasName("PK__IOTDATA__F5A76B3BCFDB4EE2");

            entity.ToTable("IOTDATA");

            entity.Property(e => e.DataId).HasColumnName("data_id");
            entity.Property(e => e.SensorId).HasColumnName("sensor_id");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("timestamp");
            entity.Property(e => e.Value).HasColumnName("value");

            entity.HasOne(d => d.Sensor).WithMany(p => p.Iotdata)
                .HasForeignKey(d => d.SensorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__IOTDATA__sensor___59063A47");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__NOTIFICA__E059842FF43E0DDA");

            entity.ToTable("NOTIFICATION");

            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__NOTIFICAT__user___66603565");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("PK__PERMISSI__E5331AFA1301EC0C");

            entity.ToTable("PERMISSION");

            entity.Property(e => e.PermissionId).HasColumnName("permission_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Permissiongroup>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PK__PERMISSI__D57795A0F6AF18C0");

            entity.ToTable("PERMISSIONGROUP");

            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Rolegroup>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__ROLEGROU__760965CC336333A4");

            entity.ToTable("ROLEGROUP");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Sensor>(entity =>
        {
            entity.HasKey(e => e.SensorId).HasName("PK__SENSOR__1A8E90607A1316E6");

            entity.ToTable("SENSOR");

            entity.Property(e => e.SensorId).HasColumnName("sensor_id");
            entity.Property(e => e.AssignedToCluster).HasColumnName("assigned_to_cluster");
            entity.Property(e => e.AssignedToTank).HasColumnName("assigned_to_tank");
            entity.Property(e => e.SensorTypeId).HasColumnName("sensor_type_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");

            entity.HasOne(d => d.AssignedToClusterNavigation).WithMany(p => p.Sensors)
                .HasForeignKey(d => d.AssignedToCluster)
                .HasConstraintName("FK__SENSOR__assigned__5535A963");

            entity.HasOne(d => d.AssignedToTankNavigation).WithMany(p => p.Sensors)
                .HasForeignKey(d => d.AssignedToTank)
                .HasConstraintName("FK__SENSOR__assigned__5441852A");

            entity.HasOne(d => d.SensorType).WithMany(p => p.Sensors)
                .HasForeignKey(d => d.SensorTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SENSOR__sensor_t__534D60F1");
        });

        modelBuilder.Entity<Sensortype>(entity =>
        {
            entity.HasKey(e => e.SensorTypeId).HasName("PK__SENSORTY__4797377A5269DE3D");

            entity.ToTable("SENSORTYPE");

            entity.Property(e => e.SensorTypeId).HasColumnName("sensor_type_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
        });

        modelBuilder.Entity<Shopowner>(entity =>
        {
            entity.HasKey(e => e.OwnerId).HasName("PK__SHOPOWNE__3C4FBEE4E5F8EC3B");

            entity.ToTable("SHOPOWNER");

            entity.Property(e => e.OwnerId).HasColumnName("owner_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Shopowners)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SHOPOWNER__user___3C69FB99");
        });

        modelBuilder.Entity<Shopstaff>(entity =>
        {
            entity.HasKey(e => e.StaffId).HasName("PK__SHOPSTAF__1963DD9C1FE6F61E");

            entity.ToTable("SHOPSTAFF");

            entity.Property(e => e.StaffId).HasColumnName("staff_id");
            entity.Property(e => e.Permissions).HasColumnName("permissions");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Shopstaffs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SHOPSTAFF__user___3F466844");
        });

        modelBuilder.Entity<Spendingrecord>(entity =>
        {
            entity.HasKey(e => e.RecordId).HasName("PK__SPENDING__BFCFB4DDE9D552DC");

            entity.ToTable("SPENDINGRECORD");

            entity.Property(e => e.RecordId).HasColumnName("record_id");
            entity.Property(e => e.AccountNo)
                .HasMaxLength(100)
                .HasColumnName("accountNo");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.BankNo)
                .HasMaxLength(100)
                .HasColumnName("bankNo");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.SpendingTypeId).HasColumnName("spending_type_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("timestamp");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.SpendingType).WithMany(p => p.Spendingrecords)
                .HasForeignKey(d => d.SpendingTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SPENDINGR__spend__6383C8BA");

            entity.HasOne(d => d.User).WithMany(p => p.Spendingrecords)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SPENDINGR__user___628FA481");
        });

        modelBuilder.Entity<Spendingtype>(entity =>
        {
            entity.HasKey(e => e.SpendingTypeId).HasName("PK__SPENDING__D1CE297FBE0B481D");

            entity.ToTable("SPENDINGTYPE");

            entity.Property(e => e.SpendingTypeId).HasColumnName("spending_type_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Tank>(entity =>
        {
            entity.HasKey(e => e.TankId).HasName("PK__TANK__2A95910E88F842FF");

            entity.ToTable("TANK");

            entity.Property(e => e.TankId).HasColumnName("tank_id");
            entity.Property(e => e.BreedId).HasColumnName("breed_id");
            entity.Property(e => e.ClusterId).HasColumnName("cluster_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Breed).WithMany(p => p.Tanks)
                .HasForeignKey(d => d.BreedId)
                .HasConstraintName("FK__TANK__breed_id__4F7CD00D");

            entity.HasOne(d => d.Cluster).WithMany(p => p.Tanks)
                .HasForeignKey(d => d.ClusterId)
                .HasConstraintName("FK__TANK__cluster_id__4E88ABD4");
        });

        modelBuilder.Entity<Tankcluster>(entity =>
        {
            entity.HasKey(e => e.ClusterId).HasName("PK__TANKCLUS__29FEE76C693C21A2");

            entity.ToTable("TANKCLUSTER");

            entity.Property(e => e.ClusterId).HasColumnName("cluster_id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__USER__B9BE370F5491156F");

            entity.ToTable("USER");

            entity.HasIndex(e => e.Email, "UQ__USER__AB6E616431710A3E").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__USER__F3DBC5727B08AAF8").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .HasColumnName("phone");
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasColumnName("role");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
