namespace paiementService.Models.Queries
{
    public class User
    {
        public int id { get; set; }
        public string nom { get; set; }
        public string prenom { get; set; }
        public string nom_utilisateur { get; set; }
        public string mot_de_passe { get; set; }
        public double solde { get; set; } 
        public string roleAdmin { get; set; }
    }
}
