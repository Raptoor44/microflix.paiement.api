using Microsoft.EntityFrameworkCore;
using paiementService.Models;

namespace paiementService.Context
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Paiement> Paiements { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("server=wp-mysql;port=3306;database=paiement;user=paiement-service;password=paiement-service");
        }
    }
}
