using System;
using Microsoft.EntityFrameworkCore;
using Recievables.Models;

namespace Recievables.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }


        public DbSet<Receivable> Receivables { get; set; }
    }
}
