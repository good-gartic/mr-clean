using Microsoft.EntityFrameworkCore;
using MrClean.Models;

namespace MrClean.Data;

#nullable disable
public class MrCleanDbContext : DbContext
{
    public MrCleanDbContext(DbContextOptions<MrCleanDbContext> options) : base(options)
    {
    }

    public virtual DbSet<MessageFilter> MessageFilters { get; set; }
}