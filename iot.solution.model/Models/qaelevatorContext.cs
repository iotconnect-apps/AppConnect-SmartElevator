﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace iot.solution.model.Models
{
    public partial class qaelevatorContext : DbContext
    {
        public qaelevatorContext()
        {
        }

        public qaelevatorContext(DbContextOptions<qaelevatorContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AdminRule> AdminRule { get; set; }
        public virtual DbSet<AdminUser> AdminUser { get; set; }
        public virtual DbSet<AttributeValue> AttributeValue { get; set; }
        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<CompanyConfig> CompanyConfig { get; set; }
        public virtual DbSet<Configuration> Configuration { get; set; }
        public virtual DbSet<DebugInfo> DebugInfo { get; set; }
        public virtual DbSet<Elevator> Elevator { get; set; }
        public virtual DbSet<ElevatorMaintenance> ElevatorMaintenance { get; set; }
        public virtual DbSet<Entity> Entity { get; set; }
        public virtual DbSet<HardwareKit> HardwareKit { get; set; }
        public virtual DbSet<KitType> KitType { get; set; }
        public virtual DbSet<KitTypeAttribute> KitTypeAttribute { get; set; }
        public virtual DbSet<KitTypeCommand> KitTypeCommand { get; set; }
        public virtual DbSet<Module> Module { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<RoleModulePermission> RoleModulePermission { get; set; }
        public virtual DbSet<TelemetrySummaryDaywise> TelemetrySummaryDaywise { get; set; }
        public virtual DbSet<TelemetrySummaryHourwise> TelemetrySummaryHourwise { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<MasterWidget> MasterWidget { get; set; }
        public virtual DbSet<UserDasboardWidget> UserDasboardWidget { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json").Build();
                string connString = component.helper.SolutionConfiguration.Configuration.ConnectionString;// configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdminRule>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__AdminRul__497F6CB426B05957");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.AttributeGuid)
                    .HasColumnName("attributeGuid")
                    .HasMaxLength(1000);

                entity.Property(e => e.CommandText)
                    .HasColumnName("commandText")
                    .HasMaxLength(500);

                entity.Property(e => e.CommandValue)
                    .HasColumnName("commandValue")
                    .HasMaxLength(100);

                entity.Property(e => e.ConditionText)
                    .HasColumnName("conditionText")
                    .HasMaxLength(1000);

                entity.Property(e => e.ConditionValue)
                    .HasColumnName("conditionValue")
                    .HasMaxLength(1000);

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(100);

                entity.Property(e => e.NotificationType).HasColumnName("notificationType");

                entity.Property(e => e.RuleType)
                    .HasColumnName("ruleType")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.SeverityLevelGuid).HasColumnName("severityLevelGuid");

                entity.Property(e => e.TemplateGuid).HasColumnName("templateGuid");

                entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updatedDate")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<AdminUser>(entity =>
            {
                entity.HasKey(e => e.Guid);

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.CompanyGuid).HasColumnName("companyGuid");

                entity.Property(e => e.ContactNo)
                    .HasColumnName("contactNo")
                    .HasMaxLength(25);

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(100);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnName("firstName")
                    .HasMaxLength(50);

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnName("lastName")
                    .HasMaxLength(50);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updatedDate")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<AttributeValue>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__Attribut__497F6CB4D64B11C0");

                entity.ToTable("AttributeValue", "IOTConnect");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.AggregateType).HasColumnName("aggregateType");

                entity.Property(e => e.AttributeValue1)
                    .HasColumnName("attributeValue")
                    .HasMaxLength(1000);

                entity.Property(e => e.CompanyGuid).HasColumnName("companyGuid");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.DeviceUpdatedDate)
                    .HasColumnName("deviceUpdatedDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.GatewayUpdatedDate)
                    .HasColumnName("gatewayUpdatedDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.LocalName)
                    .HasColumnName("localName")
                    .HasMaxLength(100);

                entity.Property(e => e.SdkUpdatedDate)
                    .HasColumnName("sdkUpdatedDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Tag)
                    .HasColumnName("tag")
                    .HasMaxLength(200);

                entity.Property(e => e.UniqueId)
                    .HasColumnName("uniqueId")
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__Company__497F6CB411118A97");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.Address)
                    .HasColumnName("address")
                    .HasMaxLength(500);

                entity.Property(e => e.AdminUserGuid).HasColumnName("adminUserGuid");

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasMaxLength(50);

                entity.Property(e => e.ContactNo)
                    .HasColumnName("contactNo")
                    .HasMaxLength(25);

                entity.Property(e => e.CountryGuid).HasColumnName("countryGuid");

                entity.Property(e => e.CpId)
                    .IsRequired()
                    .HasColumnName("cpId")
                    .HasMaxLength(200);

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.EntityGuid).HasColumnName("entityGuid");

                entity.Property(e => e.Image)
                    .HasColumnName("image")
                    .HasMaxLength(250);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(100);

                entity.Property(e => e.ParentGuid).HasColumnName("parentGuid");

                entity.Property(e => e.PostalCode)
                    .HasColumnName("postalCode")
                    .HasMaxLength(30);

                entity.Property(e => e.StateGuid).HasColumnName("stateGuid");

                entity.Property(e => e.TimezoneGuid).HasColumnName("timezoneGuid");

                entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updatedDate")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<CompanyConfig>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__CompanyC__497F6CB473711EEF");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.CompanyGuid).HasColumnName("companyGuid");

                entity.Property(e => e.ConfigurationGuid).HasColumnName("configurationGuid");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updatedDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Value).HasColumnName("value");
            });

            modelBuilder.Entity<Configuration>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__Configur__497F6CB468E58C36");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.ConfigKey)
                    .IsRequired()
                    .HasColumnName("configKey")
                    .HasMaxLength(100);

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updatedDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Value)
                    .HasColumnName("value")
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<DebugInfo>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Data)
                    .IsRequired()
                    .HasColumnName("data");

                entity.Property(e => e.Dt)
                    .HasColumnName("dt")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Elevator>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__Elevator__497F6CB4A9E84FFD");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.CompanyGuid).HasColumnName("companyGuid");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(1000);

                entity.Property(e => e.EntityGuid).HasColumnName("entityGuid");

                entity.Property(e => e.Image)
                    .HasColumnName("image")
                    .HasMaxLength(200);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.IsProvisioned).HasColumnName("isProvisioned");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(500);

                entity.Property(e => e.Note)
                    .HasColumnName("note")
                    .HasMaxLength(1000);

                entity.Property(e => e.Specification)
                    .HasColumnName("specification")
                    .HasMaxLength(1000);

                entity.Property(e => e.Tag)
                    .HasColumnName("tag")
                    .HasMaxLength(50);

                entity.Property(e => e.TemplateGuid).HasColumnName("templateGuid");

                entity.Property(e => e.TypeGuid).HasColumnName("typeGuid");

                entity.Property(e => e.UniqueId)
                    .IsRequired()
                    .HasColumnName("uniqueId")
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updatedDate")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<ElevatorMaintenance>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__Elevator__497F6CB4D51981B3");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.CompanyGuid).HasColumnName("companyGuid");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");
               
                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(1000);

                entity.Property(e => e.ElevatorGuid).HasColumnName("elevatorGuid");

                entity.Property(e => e.EntityGuid).HasColumnName("entityGuid");
              
                //entity.Property(e => e.Status)
                //    .HasColumnName("status")
                //    .HasMaxLength(100);
            });

            modelBuilder.Entity<Entity>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__Entity__497F6CB475C7652E");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasColumnName("address")
                    .HasMaxLength(500);

                entity.Property(e => e.Address2)
                    .HasColumnName("address2")
                    .HasMaxLength(500);

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasMaxLength(50);

                entity.Property(e => e.CompanyGuid).HasColumnName("companyGuid");

                entity.Property(e => e.CountryGuid).HasColumnName("countryGuid");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(1000);

                entity.Property(e => e.Image)
                    .HasColumnName("image")
                    .HasMaxLength(250);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.Latitude)
                    .HasColumnName("latitude")
                    .HasMaxLength(50);

                entity.Property(e => e.Longitude)
                    .HasColumnName("longitude")
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(500);

                entity.Property(e => e.ParentEntityGuid).HasColumnName("parentEntityGuid");

                entity.Property(e => e.StateGuid).HasColumnName("stateGuid");

                entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updatedDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Zipcode)
                    .HasColumnName("zipcode")
                    .HasMaxLength(10);
            });

            modelBuilder.Entity<HardwareKit>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__Hardware__497F6CB4048EB41D");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.CompanyGuid).HasColumnName("companyGuid");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.EntityGuid).HasColumnName("entityGuid");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.IsProvisioned)
                    .IsRequired()
                    .HasColumnName("isProvisioned")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.KitCode)
                    .IsRequired()
                    .HasColumnName("kitCode")
                    .HasMaxLength(50);

                entity.Property(e => e.KitTypeGuid).HasColumnName("kitTypeGuid");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(500);

                entity.Property(e => e.Note)
                    .IsRequired()
                    .HasColumnName("note")
                    .HasMaxLength(1000);

                entity.Property(e => e.TagGuid).HasColumnName("tagGuid");                    

                entity.Property(e => e.UniqueId)
                    .IsRequired()
                    .HasColumnName("uniqueId")
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updatedDate")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<KitType>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__KitType__497F6CB4F9806AB2");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .HasColumnName("code")
                    .HasMaxLength(50);

                entity.Property(e => e.CompanyGuid).HasColumnName("companyGuid");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(200);

                entity.Property(e => e.Tag)
                    .HasColumnName("tag")
                    .HasMaxLength(50);

                entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updatedDate")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<KitTypeAttribute>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__KitTypeA__497F6CB4AFF441C6");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasColumnName("code")
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(100);

                entity.Property(e => e.LocalName)
                    .IsRequired()
                    .HasColumnName("localName")
                    .HasMaxLength(100);

                entity.Property(e => e.ParentTemplateAttributeGuid).HasColumnName("parentTemplateAttributeGuid");

                entity.Property(e => e.Tag)
                    .HasColumnName("tag")
                    .HasMaxLength(50);

                entity.Property(e => e.TemplateGuid).HasColumnName("templateGuid");
            });

            modelBuilder.Entity<KitTypeCommand>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Command)
                    .HasColumnName("command")
                    .HasMaxLength(100)
                    .IsFixedLength();

                entity.Property(e => e.Guid).HasColumnName("guid");

                entity.Property(e => e.IsOtacommand).HasColumnName("isOTACommand");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(100);

                entity.Property(e => e.RequiredAck).HasColumnName("requiredAck");

                entity.Property(e => e.RequiredParam).HasColumnName("requiredParam");

                entity.Property(e => e.Tag)
                    .HasColumnName("tag")
                    .HasMaxLength(100)
                    .IsFixedLength();

                entity.Property(e => e.TemplateGuid).HasColumnName("templateGuid");
            });

            modelBuilder.Entity<Module>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__Module__497F6CB4A1228A79");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.ApplyTo).HasColumnName("applyTo");

                entity.Property(e => e.CategoryName)
                    .HasColumnName("categoryName")
                    .HasMaxLength(200);

                entity.Property(e => e.IsAdminModule).HasColumnName("isAdminModule");

                entity.Property(e => e.ModuleSequence).HasColumnName("moduleSequence");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(50);

                entity.Property(e => e.Permission).HasColumnName("permission");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__Role__497F6CB433C166E2");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.CompanyGuid).HasColumnName("companyGuid");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(500);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsAdminRole).HasColumnName("isAdminRole");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(100);

                entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updatedDate")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<RoleModulePermission>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__RoleModu__497F6CB4B4D3EDA0");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.CompanyGuid).HasColumnName("companyGuid");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.ModuleGuid).HasColumnName("moduleGuid");

                entity.Property(e => e.Permission).HasColumnName("permission");

                entity.Property(e => e.RoleGuid).HasColumnName("roleGuid");
            });

            modelBuilder.Entity<TelemetrySummaryDaywise>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__Telemetr__497F6CB4E1416FBB");

                entity.ToTable("TelemetrySummary_Daywise");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.Attribute)
                    .HasColumnName("attribute")
                    .HasMaxLength(1000);

                entity.Property(e => e.Avg).HasColumnName("avg");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("date");

                entity.Property(e => e.DeviceGuid).HasColumnName("deviceGuid");

                entity.Property(e => e.Latest).HasColumnName("latest");

                entity.Property(e => e.Max).HasColumnName("max");

                entity.Property(e => e.Min).HasColumnName("min");

                entity.Property(e => e.Sum).HasColumnName("sum");
            });

            modelBuilder.Entity<TelemetrySummaryHourwise>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__Telemetr__497F6CB4212900FD");

                entity.ToTable("TelemetrySummary_Hourwise");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.Attribute)
                    .HasColumnName("attribute")
                    .HasMaxLength(1000);

                entity.Property(e => e.Avg).HasColumnName("avg");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("datetime");

                entity.Property(e => e.DeviceGuid).HasColumnName("deviceGuid");

                entity.Property(e => e.Latest).HasColumnName("latest");

                entity.Property(e => e.Max).HasColumnName("max");

                entity.Property(e => e.Min).HasColumnName("min");

                entity.Property(e => e.Sum).HasColumnName("sum");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__User__497F6CB4FD41A318");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.CompanyGuid).HasColumnName("companyGuid");

                entity.Property(e => e.ContactNo)
                    .HasColumnName("contactNo")
                    .HasMaxLength(25);

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(100);

                entity.Property(e => e.EntityGuid).HasColumnName("entityGuid");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnName("firstName")
                    .HasMaxLength(50);

                entity.Property(e => e.ImageName)
                    .HasColumnName("imageName")
                    .HasMaxLength(100);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnName("lastName")
                    .HasMaxLength(50);

                entity.Property(e => e.RoleGuid).HasColumnName("roleGuid");

                entity.Property(e => e.TimeZoneGuid).HasColumnName("timeZoneGuid");

                entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updatedDate")
                    .HasColumnType("datetime");
                entity.Property(e => e.SubscriptionEndDate)
                  .HasColumnName("subscriptionEndDate")
                  .HasColumnType("datetime");
            });

            modelBuilder.Entity<MasterWidget>(entity =>
            {
                entity.HasKey(e => e.Guid);
                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();
                entity.Property(e => e.Widgets)
                    .HasColumnName("widgets");
                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");
                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");
                entity.Property(e => e.IsActive).HasColumnName("isActive");
                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");
                entity.Property(e => e.ModifiedDate)
                    .HasColumnName("modifiedDate")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<UserDasboardWidget>(entity =>
            {
                entity.HasKey(e => e.Guid);
                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .ValueGeneratedNever();

                entity.Property(e => e.DashboardName)
                    .IsRequired()
                    .HasColumnName("dashboardName")
                    .HasMaxLength(100);

                entity.Property(e => e.Widgets)
                    .HasColumnName("widgets");
                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");
                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");
                entity.Property(e => e.IsDefault).HasColumnName("isDefault");
                entity.Property(e => e.IsSystemDefault).HasColumnName("isSystemDefault");
                entity.Property(e => e.UserId).HasColumnName("userId");
                entity.Property(e => e.IsActive).HasColumnName("isActive");
                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");
                entity.Property(e => e.ModifiedDate)
                    .HasColumnName("modifiedDate")
                    .HasColumnType("datetime");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
