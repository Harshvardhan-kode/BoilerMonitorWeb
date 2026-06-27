using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // <-- CRITICAL: Required for IdentityDbContext
using BoilerMonitorWeb.Application.Domain.Entities;

namespace BoilerMonitorWeb.Infrastructure.Data
{
    // Changed inheritance from DbContext to IdentityDbContext<IdentityUser>
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Boiler> Boilers { get; set; }
        public DbSet<Telemetry> BoilerLogs { get; set; }
        public DbSet<AlarmDefinition> AlarmDefinitions { get; set; }
        public DbSet<AlarmLog> AlarmLogs { get; set; }

        // Stage 9 Compliance Engine Tables
        public DbSet<BatchParameterSet> BatchParameterSets { get; set; }
        public DbSet<BatchValidationResult> BatchValidationResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // CRITICAL: Always call base.OnModelCreating when using IdentityDbContext 
            // so it can configure the internal Identity tables (AspNetUsers, AspNetRoles, etc.)
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Telemetry>(entity => {
                entity.HasNoKey();
                entity.ToTable("BoilerLogs", "dbo");

                // Clear decimal validation warnings for Telemetry metrics
                entity.Property(e => e.SteamPressure_Bar).HasPrecision(18, 2);
                entity.Property(e => e.SteamTemp_C).HasPrecision(18, 2);
                entity.Property(e => e.WaterLevel_mm).HasPrecision(18, 2);
                entity.Property(e => e.FeedwaterTemp_C).HasPrecision(18, 2);
                entity.Property(e => e.FlueGasTemp_C).HasPrecision(18, 2);
                entity.Property(e => e.O2_Percentage).HasPrecision(18, 2);
                entity.Property(e => e.SteamFlow_KGHR).HasPrecision(18, 2);
            });

            modelBuilder.Entity<AlarmDefinition>(entity => {
                entity.HasKey(e => e.AlarmDefID);
                entity.ToTable("AlarmDefinitions", "dbo");

                // Clear decimal validation warnings
                entity.Property(e => e.MinValue).HasPrecision(18, 2);
                entity.Property(e => e.MaxValue).HasPrecision(18, 2);
            });

            modelBuilder.Entity<AlarmLog>(entity => {
                entity.HasKey(e => e.AlarmID);
                entity.ToTable("AlarmLogs", "dbo");

                // Clear decimal validation warning
                entity.Property(e => e.CurrentValue).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Boiler>(entity => {
                entity.HasKey(e => e.BoilerID);
                entity.ToTable("Boilers", "dbo");
                entity.Property(e => e.Capacity_KGHR).HasColumnName("CapacityKG").HasPrecision(18, 2);
            });

            modelBuilder.IdentityComplianceMapping();
        }
    }

    // Helper extension to keep your model builder readable
    public static class ModelBuilderExtensions
    {
        public static void IdentityComplianceMapping(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BatchParameterSet>(entity => {
                entity.HasKey(e => e.SetID);
                entity.ToTable("BatchParameterSets", "dbo");
            });

            modelBuilder.Entity<BatchValidationResult>(entity => {
                entity.HasKey(e => e.ValidationID);
                entity.ToTable("BatchValidationResults", "dbo");
            });
        }
    }
}