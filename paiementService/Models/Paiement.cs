namespace paiementService.Models
{
    public class Paiement
    {
        public int Id { get; set; }
        public int UtilisateurId { get; set; }
        public int FilmId { get; set; }
        public int Montant { get; set; }
        public bool IsPayed { get; set; }
    }
}
