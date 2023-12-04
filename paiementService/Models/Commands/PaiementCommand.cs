namespace paiementService.Models.Commands
{
    public class PaiementCommand
    {
        public int FilmId { get; set; }
        public int UtilisateurId { get; set; }
        public int Montant { get; set; }
        public int TotalPrix { get; set; }
    }
}
