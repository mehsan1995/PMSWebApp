using DAL.Interfaces;
using DAL.Models;
using DAL.TenantProvider;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DAL
{
   
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITenantProvider _tenantProvider;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor, ITenantProvider tenantProvider)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            _tenantProvider = tenantProvider;
        }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Departments> Departments { get; set; }
        public DbSet<Tenants> Tenants { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<DepartmentType> DepartmentTypes { get; set; }
        public DbSet<DepartmentUsers> DepartmentUsers { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<Projects> Projects { get; set; }
        public DbSet<ProjectInformations> ProjectInformations { get; set; }
        public DbSet<Documents> Documents { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Permission>().ToTable("Permissions");

            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            modelBuilder.Entity<UserPermission>()
                .HasKey(up => new { up.UserId, up.PermissionId });

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany()
                .HasForeignKey(rp => rp.RoleId);

            modelBuilder.Entity<UserPermission>()
                .HasOne(up => up.User)
                .WithMany()
                .HasForeignKey(up => up.UserId);

            base.OnModelCreating(modelBuilder);

            var tenantId = _tenantProvider.GetTenantId();

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");

                    // e => EF.Property<bool>(e, "IsDeleted") == false
                    var propertyMethodBool = typeof(EF).GetMethod("Property")!
                        .MakeGenericMethod(typeof(bool));

                    var isDeletedProperty = Expression.Call(
                        propertyMethodBool,
                        parameter,
                        Expression.Constant("IsDeleted"));

                    var isDeletedFalse = Expression.Equal(isDeletedProperty, Expression.Constant(false));

                    Expression finalFilter = isDeletedFalse;

                    var tenantProperty = entityType.FindProperty("TenantId");

                    if (tenantProperty != null && tenantId != 0)
                    {
                        // Use correct type for TenantId property
                        var propertyMethodTenantId = typeof(EF).GetMethod("Property")!
                            .MakeGenericMethod(typeof(int)); // change if TenantId is int

                        var tenantIdProperty = Expression.Call(
                            propertyMethodTenantId,
                            parameter,
                            Expression.Constant("TenantId"));

                        var tenantIdConstant = Expression.Constant(tenantId);

                        var tenantIdMatch = Expression.Equal(tenantIdProperty, tenantIdConstant);

                        // Combine: (IsDeleted == false) && (TenantId == tenantId)
                        finalFilter = Expression.AndAlso(isDeletedFalse, tenantIdMatch);
                    }
                    else if (tenantId == 0)
                    {
                        // tenantId == 0 means no tenant filtering, so just filter IsDeleted == false
                        finalFilter = isDeletedFalse;
                    }

                    var lambda = Expression.Lambda(
                        typeof(Func<,>).MakeGenericType(entityType.ClrType, typeof(bool)),
                        finalFilter,
                        parameter);

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }




            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var currentUser = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";

            // You can store TenantId in claims, session, or cookies.  
            // Example: Reading it from claims.
            var tenantIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("TenantId")?.Value;
            int? tenantId = string.IsNullOrEmpty(tenantIdClaim) ? null : int.Parse(tenantIdClaim);

            foreach (var entry in ChangeTracker.Entries())
            {
                // Handle IAuditable
                if (entry.Entity is IAuditable auditable)
                {
                    if (entry.State == EntityState.Added)
                    {
                        auditable.CreatedAt = DateTime.UtcNow;
                        auditable.CreatedBy = currentUser;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        auditable.ModifiedAt = DateTime.UtcNow;
                        auditable.ModifiedBy = currentUser;
                    }
                }

                // Handle ITenant
                if (entry.Entity is ITenant tenantEntity && tenantId.HasValue)
                {
                    if (entry.State == EntityState.Added)
                    {
                        tenantEntity.TenantId = tenantId.Value;
                    }
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        //public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        //{
        //    var currentUser = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";

        //    foreach (var entry in ChangeTracker.Entries<IAuditable>())
        //    {
        //        if (entry.State == EntityState.Added)
        //        {
        //            entry.Entity.CreatedAt = DateTime.UtcNow;
        //            entry.Entity.CreatedBy = currentUser;
        //        }
        //        else if (entry.State == EntityState.Modified)
        //        {
        //            entry.Entity.ModifiedAt = DateTime.UtcNow;
        //            entry.Entity.ModifiedBy = currentUser;
        //        }
        //    }

        //    return await base.SaveChangesAsync(cancellationToken);
        //}

    }

}
