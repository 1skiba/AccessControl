using Microsoft.EntityFrameworkCore;

namespace AccessControlApi.Data;

public class AccessControlDbContext : DbContext
{
    public AccessControlDbContext(
        DbContextOptions<AccessControlDbContext> options)
        : base(options)
    {
    }
}