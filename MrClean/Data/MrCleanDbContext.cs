using Microsoft.EntityFrameworkCore;

namespace MrClean.Data;

public class MrCleanDbContext : DbContext
{
    public MrCleanDbContext(DbContextOptions<MrCleanDbContext> options) : base(options)
    {
    }
}