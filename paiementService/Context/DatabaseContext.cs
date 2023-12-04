using Microsoft.EntityFrameworkCore;
using paiementService.Models;

namespace paiementService.Context
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Paiement> Paiements { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("server=localhost;port=10000;database=paiement;user=paiement-service;password=paiement-service");
        }
    }
}
