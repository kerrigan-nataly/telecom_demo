using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using telecomdemo2.Models;

namespace telecomdemo2.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Component> Components { get; set; }

    public virtual DbSet<ComponentStorage> ComponentStorages { get; set; }

    public virtual DbSet<ComponentStorageInsertsDelete> ComponentStorageInsertsDeletes { get; set; }

    public virtual DbSet<ComponentStorageUpdate> ComponentStorageUpdates { get; set; }

    public virtual DbSet<ComponentType> ComponentTypes { get; set; }

    public virtual DbSet<Manufacturer> Manufacturers { get; set; }

    public virtual DbSet<Node> Nodes { get; set; }

    public virtual DbSet<NodeType> NodeTypes { get; set; }

    public virtual DbSet<NodeTypeComponent> NodeTypeComponents { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderComponent> OrderComponents { get; set; }

    public virtual DbSet<OrderInsertsDelete> OrderInsertsDeletes { get; set; }

    public virtual DbSet<OrderNode> OrderNodes { get; set; }

    public virtual DbSet<OrderUpdate> OrderUpdates { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Storage> Storages { get; set; }

    public virtual DbSet<StorageType> StorageTypes { get; set; }

    public virtual DbSet<TemperatureMode> TemperatureModes { get; set; }

    public virtual DbSet<TestingResult> TestingResults { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=telecom;Username=postgres;Password=3121");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Component>(entity =>
        {
            entity.HasKey(e => e.IdComponent).HasName("component_pkey");

            entity.ToTable("component");

            entity.Property(e => e.IdComponent)
                .HasMaxLength(10)
                .HasColumnName("id_component");
            entity.Property(e => e.Characteristic)
                .HasColumnType("jsonb")
                .HasColumnName("characteristic");
            entity.Property(e => e.ComponentTypeId).HasColumnName("component_type_id");
            entity.Property(e => e.ExpirationDate).HasColumnName("expiration_date");
            entity.Property(e => e.ManufacturerId).HasColumnName("manufacturer_id");
            entity.Property(e => e.MaxStorage).HasColumnName("max_storage");
            entity.Property(e => e.MinStorage).HasColumnName("min_storage");
            entity.Property(e => e.NameComponent)
                .HasMaxLength(50)
                .HasColumnName("name_component");
            entity.Property(e => e.PathToScheme)
                .HasMaxLength(200)
                .HasColumnName("path_to_scheme");
            entity.Property(e => e.TemperatureModeId).HasColumnName("temperature_mode_id");
            entity.Property(e => e.Unit)
                .HasMaxLength(10)
                .HasColumnName("unit");

            entity.HasOne(d => d.ComponentType).WithMany(p => p.Components)
                .HasForeignKey(d => d.ComponentTypeId)
                .HasConstraintName("component_component_type_id_fkey");

            entity.HasOne(d => d.Manufacturer).WithMany(p => p.Components)
                .HasForeignKey(d => d.ManufacturerId)
                .HasConstraintName("component_manufacturer_id_fkey");

            entity.HasOne(d => d.TemperatureMode).WithMany(p => p.Components)
                .HasForeignKey(d => d.TemperatureModeId)
                .HasConstraintName("component_temperature_mode_id_fkey");
        });

        modelBuilder.Entity<ComponentStorage>(entity =>
        {
            entity.HasKey(e => e.IdComponentStorage).HasName("component_storage_pkey");

            entity.ToTable("component_storage");

            entity.Property(e => e.IdComponentStorage).HasColumnName("id_component_storage");
            entity.Property(e => e.ComponentCount).HasColumnName("component_count");
            entity.Property(e => e.ComponentId)
                .HasMaxLength(10)
                .HasColumnName("component_id");
            entity.Property(e => e.StorageId).HasColumnName("storage_id");

            entity.HasOne(d => d.Component).WithMany(p => p.ComponentStorages)
                .HasForeignKey(d => d.ComponentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("component_storage_component_id_fkey");

            entity.HasOne(d => d.Storage).WithMany(p => p.ComponentStorages)
                .HasForeignKey(d => d.StorageId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("component_storage_storage_id_fkey");
        });

        modelBuilder.Entity<ComponentStorageInsertsDelete>(entity =>
        {
            entity.HasKey(e => e.IdHistory).HasName("component_storage_inserts_deletes_pkey");

            entity.ToTable("component_storage_inserts_deletes");

            entity.Property(e => e.IdHistory).HasColumnName("id_history");
            entity.Property(e => e.ChangeDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("change_date");
            entity.Property(e => e.ComponentCount).HasColumnName("component_count");
            entity.Property(e => e.ComponentId)
                .HasMaxLength(10)
                .HasColumnName("component_id");
            entity.Property(e => e.OperationType)
                .HasMaxLength(10)
                .HasColumnName("operation_type");
            entity.Property(e => e.StorageId).HasColumnName("storage_id");

            entity.HasOne(d => d.Component).WithMany(p => p.ComponentStorageInsertsDeletes)
                .HasForeignKey(d => d.ComponentId)
                .HasConstraintName("component_storage_inserts_deletes_component_id_fkey");

            entity.HasOne(d => d.Storage).WithMany(p => p.ComponentStorageInsertsDeletes)
                .HasForeignKey(d => d.StorageId)
                .HasConstraintName("component_storage_inserts_deletes_storage_id_fkey");
        });

        modelBuilder.Entity<ComponentStorageUpdate>(entity =>
        {
            entity.HasKey(e => e.IdHistory).HasName("component_storage_updates_pkey");

            entity.ToTable("component_storage_updates");

            entity.Property(e => e.IdHistory).HasColumnName("id_history");
            entity.Property(e => e.ChangeDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("change_date");
            entity.Property(e => e.ComponentId)
                .HasMaxLength(10)
                .HasColumnName("component_id");
            entity.Property(e => e.NewCount).HasColumnName("new_count");
            entity.Property(e => e.NewStorageId).HasColumnName("new_storage_id");
            entity.Property(e => e.OldCount).HasColumnName("old_count");
            entity.Property(e => e.OldStorageId).HasColumnName("old_storage_id");

            entity.HasOne(d => d.Component).WithMany(p => p.ComponentStorageUpdates)
                .HasForeignKey(d => d.ComponentId)
                .HasConstraintName("component_storage_updates_component_id_fkey");

            entity.HasOne(d => d.NewStorage).WithMany(p => p.ComponentStorageUpdateNewStorages)
                .HasForeignKey(d => d.NewStorageId)
                .HasConstraintName("component_storage_updates_new_storage_id_fkey");

            entity.HasOne(d => d.OldStorage).WithMany(p => p.ComponentStorageUpdateOldStorages)
                .HasForeignKey(d => d.OldStorageId)
                .HasConstraintName("component_storage_updates_old_storage_id_fkey");
        });

        modelBuilder.Entity<ComponentType>(entity =>
        {
            entity.HasKey(e => e.IdComponentType).HasName("component_type_pkey");

            entity.ToTable("component_type");

            entity.Property(e => e.IdComponentType).HasColumnName("id_component_type");
            entity.Property(e => e.NameComponentType)
                .HasMaxLength(50)
                .HasColumnName("name_component_type");
        });

        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.HasKey(e => e.IdManufacturer).HasName("manufacturer_pkey");

            entity.ToTable("manufacturer");

            entity.Property(e => e.IdManufacturer).HasColumnName("id_manufacturer");
            entity.Property(e => e.AvgTime).HasColumnName("avg_time");
            entity.Property(e => e.Contact)
                .HasMaxLength(50)
                .HasColumnName("contact");
            entity.Property(e => e.NameManufacturer)
                .HasMaxLength(50)
                .HasColumnName("name_manufacturer");
            entity.Property(e => e.Rating).HasColumnName("rating");
        });

        modelBuilder.Entity<Node>(entity =>
        {
            entity.HasKey(e => e.IdNode).HasName("node_pkey");

            entity.ToTable("node");

            entity.Property(e => e.IdNode).HasColumnName("id_node");
            entity.Property(e => e.NameNode)
                .HasMaxLength(50)
                .HasColumnName("name_node");
            entity.Property(e => e.NodeTypeId).HasColumnName("node_type_id");
            entity.Property(e => e.TestingResultId).HasColumnName("testing_result_id");

            entity.HasOne(d => d.NodeType).WithMany(p => p.Nodes)
                .HasForeignKey(d => d.NodeTypeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("node_node_type_id_fkey");

            entity.HasOne(d => d.TestingResult).WithMany(p => p.Nodes)
                .HasForeignKey(d => d.TestingResultId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("node_testing_result_id_fkey");
        });

        modelBuilder.Entity<NodeType>(entity =>
        {
            entity.HasKey(e => e.IdNodeType).HasName("node_type_pkey");

            entity.ToTable("node_type");

            entity.Property(e => e.IdNodeType).HasColumnName("id_node_type");
            entity.Property(e => e.NameNodeType)
                .HasMaxLength(50)
                .HasColumnName("name_node_type");
        });

        modelBuilder.Entity<NodeTypeComponent>(entity =>
        {
            entity.HasKey(e => e.IdNodeTypeComponent).HasName("node_type_component_pkey");

            entity.ToTable("node_type_component");

            entity.Property(e => e.IdNodeTypeComponent).HasColumnName("id_node_type_component");
            entity.Property(e => e.ComponentCount).HasColumnName("component_count");
            entity.Property(e => e.ComponentId)
                .HasMaxLength(10)
                .HasColumnName("component_id");
            entity.Property(e => e.NodeTypeId).HasColumnName("node_type_id");

            entity.HasOne(d => d.Component).WithMany(p => p.NodeTypeComponents)
                .HasForeignKey(d => d.ComponentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("node_type_component_component_id_fkey");

            entity.HasOne(d => d.NodeType).WithMany(p => p.NodeTypeComponents)
                .HasForeignKey(d => d.NodeTypeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("node_type_component_node_type_id_fkey");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.IdOrder).HasName("ORDER_pkey");

            entity.ToTable("ORDER");

            entity.Property(e => e.IdOrder).HasColumnName("id_order");
            entity.Property(e => e.DeadlineDate).HasColumnName("deadline_date");
            entity.Property(e => e.NameOrder)
                .HasMaxLength(50)
                .HasColumnName("name_order");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.StatusId).HasColumnName("status_id");

            entity.HasOne(d => d.Status).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("ORDER_status_id_fkey");
        });

        modelBuilder.Entity<OrderComponent>(entity =>
        {
            entity.HasKey(e => e.IdOrderComponent).HasName("order_component_pkey");

            entity.ToTable("order_component");

            entity.Property(e => e.IdOrderComponent).HasColumnName("id_order_component");
            entity.Property(e => e.ComponentCount).HasColumnName("component_count");
            entity.Property(e => e.ComponentId)
                .HasMaxLength(10)
                .HasColumnName("component_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");

            entity.HasOne(d => d.Component).WithMany(p => p.OrderComponents)
                .HasForeignKey(d => d.ComponentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("order_component_component_id_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderComponents)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("order_component_order_id_fkey");
        });

        modelBuilder.Entity<OrderInsertsDelete>(entity =>
        {
            entity.HasKey(e => e.IdHistory).HasName("order_inserts_deletes_pkey");

            entity.ToTable("order_inserts_deletes");

            entity.Property(e => e.IdHistory).HasColumnName("id_history");
            entity.Property(e => e.ChangeDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("change_date");
            entity.Property(e => e.OperationType)
                .HasMaxLength(10)
                .HasColumnName("operation_type");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.OrderStatus).HasColumnName("order_status");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderInsertsDeletes)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("order_inserts_deletes_order_id_fkey");

            entity.HasOne(d => d.OrderStatusNavigation).WithMany(p => p.OrderInsertsDeletes)
                .HasForeignKey(d => d.OrderStatus)
                .HasConstraintName("order_inserts_deletes_order_status_fkey");
        });

        modelBuilder.Entity<OrderNode>(entity =>
        {
            entity.HasKey(e => e.IdOrderNode).HasName("order_node_pkey");

            entity.ToTable("order_node");

            entity.Property(e => e.IdOrderNode).HasColumnName("id_order_node");
            entity.Property(e => e.NodeCount).HasColumnName("node_count");
            entity.Property(e => e.NodeId).HasColumnName("node_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");

            entity.HasOne(d => d.Node).WithMany(p => p.OrderNodes)
                .HasForeignKey(d => d.NodeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("order_node_node_id_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderNodes)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("order_node_order_id_fkey");
        });

        modelBuilder.Entity<OrderUpdate>(entity =>
        {
            entity.HasKey(e => e.IdHistory).HasName("order_updates_pkey");

            entity.ToTable("order_updates");

            entity.Property(e => e.IdHistory).HasColumnName("id_history");
            entity.Property(e => e.ChangeDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("change_date");
            entity.Property(e => e.NewStatus).HasColumnName("new_status");
            entity.Property(e => e.OldStatus).HasColumnName("old_status");
            entity.Property(e => e.OrderId).HasColumnName("order_id");

            entity.HasOne(d => d.NewStatusNavigation).WithMany(p => p.OrderUpdateNewStatusNavigations)
                .HasForeignKey(d => d.NewStatus)
                .HasConstraintName("order_updates_new_status_fkey");

            entity.HasOne(d => d.OldStatusNavigation).WithMany(p => p.OrderUpdateOldStatusNavigations)
                .HasForeignKey(d => d.OldStatus)
                .HasConstraintName("order_updates_old_status_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderUpdates)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("order_updates_order_id_fkey");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.IdStatus).HasName("status_pkey");

            entity.ToTable("status");

            entity.Property(e => e.IdStatus).HasColumnName("id_status");
            entity.Property(e => e.NameStatus)
                .HasMaxLength(50)
                .HasColumnName("name_status");
        });

        modelBuilder.Entity<Storage>(entity =>
        {
            entity.HasKey(e => e.IdStorage).HasName("STORAGE_pkey");

            entity.ToTable("STORAGE");

            entity.Property(e => e.IdStorage).HasColumnName("id_storage");
            entity.Property(e => e.NameStorage)
                .HasMaxLength(50)
                .HasColumnName("name_storage");
            entity.Property(e => e.StorageTypeId).HasColumnName("storage_type_id");
            entity.Property(e => e.TemperatureModeId).HasColumnName("temperature_mode_id");

            entity.HasOne(d => d.StorageType).WithMany(p => p.Storages)
                .HasForeignKey(d => d.StorageTypeId)
                .HasConstraintName("STORAGE_storage_type_id_fkey");

            entity.HasOne(d => d.TemperatureMode).WithMany(p => p.Storages)
                .HasForeignKey(d => d.TemperatureModeId)
                .HasConstraintName("STORAGE_temperature_mode_id_fkey");
        });

        modelBuilder.Entity<StorageType>(entity =>
        {
            entity.HasKey(e => e.IdStorageType).HasName("storage_type_pkey");

            entity.ToTable("storage_type");

            entity.Property(e => e.IdStorageType).HasColumnName("id_storage_type");
            entity.Property(e => e.NameStorageType)
                .HasMaxLength(50)
                .HasColumnName("name_storage_type");
        });

        modelBuilder.Entity<TemperatureMode>(entity =>
        {
            entity.HasKey(e => e.IdTemperatureMode).HasName("temperature_mode_pkey");

            entity.ToTable("temperature_mode");

            entity.Property(e => e.IdTemperatureMode).HasColumnName("id_temperature_mode");
            entity.Property(e => e.MaxHumidity).HasColumnName("max_humidity");
            entity.Property(e => e.MaxTemperature).HasColumnName("max_temperature");
            entity.Property(e => e.MinHumidity).HasColumnName("min_humidity");
            entity.Property(e => e.MinTemperature).HasColumnName("min_temperature");
            entity.Property(e => e.NameTemperatureMode)
                .HasMaxLength(50)
                .HasColumnName("name_temperature_mode");
        });

        modelBuilder.Entity<TestingResult>(entity =>
        {
            entity.HasKey(e => e.IdTestingResult).HasName("testing_result_pkey");

            entity.ToTable("testing_result");

            entity.Property(e => e.IdTestingResult).HasColumnName("id_testing_result");
            entity.Property(e => e.NameTestingResult)
                .HasMaxLength(50)
                .HasColumnName("name_testing_result");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("USER_pkey");

            entity.ToTable("USER");

            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.Fullname)
                .HasMaxLength(150)
                .HasColumnName("fullname");
            entity.Property(e => e.Login)
                .HasMaxLength(50)
                .HasColumnName("login");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.UserRoleId).HasColumnName("user_role_id");

            entity.HasOne(d => d.UserRole).WithMany(p => p.Users)
                .HasForeignKey(d => d.UserRoleId)
                .HasConstraintName("USER_user_role_id_fkey");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.IdUserRole).HasName("user_role_pkey");

            entity.ToTable("user_role");

            entity.Property(e => e.IdUserRole).HasColumnName("id_user_role");
            entity.Property(e => e.NameUserRole)
                .HasMaxLength(50)
                .HasColumnName("name_user_role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
