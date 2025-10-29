using System;
using System.Collections.Generic;
using FishFarm.BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FishFarm.DataAccessLayer;

public partial class FishFarmDbV2Context : DbContext
{
    public FishFarmDbV2Context()
    {
    }

    public FishFarmDbV2Context(DbContextOptions<FishFarmDbV2Context> options)
        : base(options)
    {
    }

    //Class moi
    public virtual DbSet<Device> Device { get; set; }
    public virtual DbSet<RefreshToken> RefreshToken { get; set; }
    public virtual DbSet<FishBreed> FishBreeds { get; set; }

    public virtual DbSet<FishHealthStatus> FishHealthStatuses { get; set; }

    public virtual DbSet<IoTdatum> IoTdata { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<PermissionGroup> PermissionGroups { get; set; }

    public virtual DbSet<RoleGroup> RoleGroups { get; set; }

    public virtual DbSet<Sensor> Sensors { get; set; }

    public virtual DbSet<SensorType> SensorTypes { get; set; }

    public virtual DbSet<ShopOwner> ShopOwners { get; set; }

    public virtual DbSet<ShopStaff> ShopStaffs { get; set; }

    public virtual DbSet<SpendingRecord> SpendingRecords { get; set; }

    public virtual DbSet<SpendingType> SpendingTypes { get; set; }

    public virtual DbSet<Tank> Tanks { get; set; }

    public virtual DbSet<TankCluster> TankClusters { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserProfile> UserProfiles { get; set; }

    private string GetConnectionString()
    {
        IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();
        return configuration["ConnectionStrings:MyDbConnection"]; 
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(GetConnectionString());
        }
    }

    //ep kieu chu hoa thuong
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<FishBreed>(entity =>
        {
            entity.HasKey(e => e.BreedId).HasName("PK__FishBree__9C0214357AA6B114");

            entity.ToTable("FishBreed");

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

        modelBuilder.Entity<FishHealthStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__FishHeal__3683B531FF918ABC");

            entity.ToTable("FishHealthStatus");

            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.CurrentStatus).HasColumnName("current_status");
            entity.Property(e => e.Prediction).HasColumnName("prediction");
            entity.Property(e => e.TankId).HasColumnName("tank_id");

            entity.HasOne(d => d.Tank).WithMany(p => p.FishHealthStatuses)
                .HasForeignKey(d => d.TankId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FishHealt__tank___5EBF139D");
        });

        modelBuilder.Entity<IoTdatum>(entity =>
        {
            entity.HasKey(e => e.DataId).HasName("PK__IoTData__F5A76B3BFCC0A361");

            entity.ToTable("IoTData");

            entity.Property(e => e.DataId).HasColumnName("data_id");
            entity.Property(e => e.SensorId).HasColumnName("sensor_id");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("timestamp");
            entity.Property(e => e.Value).HasColumnName("value");

            entity.HasOne(d => d.Sensor).WithMany(p => p.IoTdata)
                .HasForeignKey(d => d.SensorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__IoTData__sensor___5BE2A6F2");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__E059842F331018C4");

            entity.ToTable("Notification");

            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__user___693CA210");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("PK__Permissi__E5331AFAEF5CE317");

            entity.ToTable("Permission");

            entity.Property(e => e.PermissionId).HasColumnName("permission_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<PermissionGroup>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PK__Permissi__D57795A0957C8799");

            entity.ToTable("PermissionGroup");

            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<RoleGroup>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__RoleGrou__760965CCC2BBB2CB");

            entity.ToTable("RoleGroup");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Sensor>(entity =>
        {
            entity.HasKey(e => e.SensorId).HasName("PK__Sensor__1A8E90604F446CA7");

            entity.ToTable("Sensor");

            entity.Property(e => e.SensorId).HasColumnName("sensor_id");
            entity.Property(e => e.AssignedToCluster).HasColumnName("assigned_to_cluster");
            entity.Property(e => e.AssignedToTank).HasColumnName("assigned_to_tank");
            entity.Property(e => e.SensorTypeId).HasColumnName("sensor_type_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");

            entity.HasOne(d => d.AssignedToClusterNavigation).WithMany(p => p.Sensors)
                .HasForeignKey(d => d.AssignedToCluster)
                .HasConstraintName("FK__Sensor__assigned__5812160E");

            entity.HasOne(d => d.AssignedToTankNavigation).WithMany(p => p.Sensors)
                .HasForeignKey(d => d.AssignedToTank)
                .HasConstraintName("FK__Sensor__assigned__571DF1D5");

            entity.HasOne(d => d.SensorType).WithMany(p => p.Sensors)
                .HasForeignKey(d => d.SensorTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Sensor__sensor_t__5629CD9C");
        });

        modelBuilder.Entity<SensorType>(entity =>
        {
            entity.HasKey(e => e.SensorTypeId).HasName("PK__SensorTy__4797377A9915E540");

            entity.ToTable("SensorType");

            entity.Property(e => e.SensorTypeId).HasColumnName("sensor_type_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
        });

        modelBuilder.Entity<ShopOwner>(entity =>
        {
            entity.HasKey(e => e.OwnerId).HasName("PK__ShopOwne__3C4FBEE4ABAC2443");

            entity.ToTable("ShopOwner");

            entity.Property(e => e.OwnerId).HasColumnName("owner_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.ShopOwners)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ShopOwner__user___3F466844");
        });

        modelBuilder.Entity<ShopStaff>(entity =>
        {
            entity.HasKey(e => e.StaffId).HasName("PK__ShopStaf__1963DD9C116A4338");

            entity.ToTable("ShopStaff");

            entity.Property(e => e.StaffId).HasColumnName("staff_id");
            entity.Property(e => e.Permissions).HasColumnName("permissions");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.ShopStaffs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ShopStaff__user___4222D4EF");
        });

        modelBuilder.Entity<SpendingRecord>(entity =>
        {
            entity.HasKey(e => e.RecordId).HasName("PK__Spending__BFCFB4DD2E983C84");

            entity.ToTable("SpendingRecord");

            entity.Property(e => e.RecordId).HasColumnName("record_id");
            entity.Property(e => e.AccountNo)
                .HasMaxLength(100)
                .HasColumnName("account_no");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.BankNo)
                .HasMaxLength(100)
                .HasColumnName("bank_no");
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

            entity.HasOne(d => d.SpendingType).WithMany(p => p.SpendingRecords)
                .HasForeignKey(d => d.SpendingTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SpendingR__spend__66603565");

            entity.HasOne(d => d.User).WithMany(p => p.SpendingRecords)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SpendingR__user___656C112C");
        });

        modelBuilder.Entity<SpendingType>(entity =>
        {
            entity.HasKey(e => e.SpendingTypeId).HasName("PK__Spending__D1CE297F3EEB1E7B");

            entity.ToTable("SpendingType");

            entity.Property(e => e.SpendingTypeId).HasColumnName("spending_type_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Tank>(entity =>
        {
            entity.HasKey(e => e.TankId).HasName("PK__Tank__2A95910E4DFE4464");

            entity.ToTable("Tank");

            entity.Property(e => e.TankId).HasColumnName("tank_id");
            entity.Property(e => e.BreedId).HasColumnName("breed_id");
            entity.Property(e => e.ClusterId).HasColumnName("cluster_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Breed).WithMany(p => p.Tanks)
                .HasForeignKey(d => d.BreedId)
                .HasConstraintName("FK__Tank__breed_id__52593CB8");

            entity.HasOne(d => d.Cluster).WithMany(p => p.Tanks)
                .HasForeignKey(d => d.ClusterId)
                .HasConstraintName("FK__Tank__cluster_id__5165187F");
        });

        modelBuilder.Entity<TankCluster>(entity =>
        {
            entity.HasKey(e => e.ClusterId).HasName("PK__TankClus__29FEE76C032E1AAB");

            entity.ToTable("TankCluster");

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
            entity.HasKey(e => e.UserId).HasName("PK__Users__B9BE370F3A6E2E2E");

            entity.HasIndex(e => e.Email, "UQ__Users__AB6E61641FB9B525").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__Users__F3DBC572CDF3E26C").IsUnique();

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

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.ProfileId).HasName("PK__UserProf__AEBB701F5E13EB90");

            entity.Property(e => e.ProfileId).HasColumnName("profile_id");
            entity.Property(e => e.ContactAddress)
                .HasMaxLength(255)
                .HasColumnName("contact_address");
            entity.Property(e => e.CurrentPhoneNumber)
                .HasMaxLength(50)
                .HasColumnName("current_phone_number");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.PermanentAddress)
                .HasMaxLength(255)
                .HasColumnName("permanent_address");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserProfiles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserProfi__user___3C69FB99");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
