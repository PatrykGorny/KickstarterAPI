using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infractructure.EF;

public class AppDbContext : IdentityDbContext<UserEntity>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<KickstarterEntity> Kickstarters { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        var adminId = "0bb08caa-d013-4715-a64f-a7e77ee77b01";
        var createdAt = new DateTime(2025, 04, 08);
        var hash = "AQAAAAIAAYagAAAAEOrArrSG1swr5b94IyFxxXI9wv/pMOWdiSK3LvAtL3VoMmk6sTFHTvhuRqAesmP/Ag==";
        var adminUser = new UserEntity()
        {
            Id = adminId,
            Email = "admin@wsei.edu.pl",
            NormalizedEmail = "admin@wsei.edu.pl".ToUpper(),
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            ConcurrencyStamp = adminId,
            SecurityStamp = adminId,
            PasswordHash = hash
        };
        
        builder.Entity<UserEntity>().HasData(adminUser);
        builder.Entity<UserEntity>().OwnsOne(u => u.Details)
            .HasData(new
            {
                UserEntityId = adminId,
                CreatedAt = createdAt
            });
    }
}