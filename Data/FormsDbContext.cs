using System;
using Microsoft.EntityFrameworkCore;
using OrdersApp.Models;

namespace OrdersApp.Data;

public class FormsDbContext : DbContext
{
    public FormsDbContext(DbContextOptions<FormsDbContext> options)
        : base(options)
    {
    }

    public DbSet<FormEntry> Entries { get; set; } = null!;
}
