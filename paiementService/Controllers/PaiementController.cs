using Microsoft.AspNetCore.Mvc;
using paiementService.Context;
using paiementService.Models;
using paiementService.Models.Commands;
using paiementService.Models.Queries;
using Steeltoe.Common.Discovery;
using Steeltoe.Discovery;
using System.Text;
using System.Text.Json;

namespace paiementService.Controllers
{
    [Route("api/paiement")]
    [ApiController]
    public class PaiementController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private DiscoveryHttpClientHandler _handler;

        public PaiementController(DatabaseContext context, IDiscoveryClient client)
        {
            _context = context;
            _handler = new DiscoveryHttpClientHandler(client);
        }

        [HttpPost]
        public async Task<ActionResult<PaiementController>> PostPaiement(PaiementCommand param)
        {
            if (_context.Paiements == null)
            {
                return Problem("L'entité 'DatabaseContext.Paiements' est null.");
            }

            // Make a request to the UtilisateurController to get a user with a specific ID
            var utilisateurId = param.UtilisateurId; // Assuming you have the user ID from the PaiementCommand parameter
            string utilisateurApiUrl = "http://MicroflixUtilisateurApi/api/utilisateurs/" + utilisateurId;

            // Use HttpClient to make the request

            var client = new HttpClient(_handler, false);
            var response = await client.GetAsync(utilisateurApiUrl);

            if (response.IsSuccessStatusCode)
            {
                var userJson = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<User>(userJson);

                // Now, you have the 'user' object of type User
                // You can use 'user' as needed in your code

                if (user.solde > param.Montant)
                {

                    if (_context.Paiements.Where(paiement => paiement.FilmId == param.FilmId && paiement.UtilisateurId == param.UtilisateurId).Any())
                    {
                        Paiement paiement = _context.Paiements.Where(paiement => paiement.FilmId == param.FilmId && paiement.UtilisateurId == param.UtilisateurId).FirstOrDefault();

                        paiement.Montant += param.Montant;

                        if (paiement.Montant > param.TotalPrix)
                        {
                            await SendLogAsync("Le paiement envoyé pour " + paiement.Montant.ToString() + " avec l'utilisateur : " + paiement.UtilisateurId + " est trop important ");

                            return BadRequest("Erreur, le montant payé est trop important");
                        }
                        else if (paiement.Montant == param.TotalPrix)
                        {
                            paiement.IsPayed = true;

                            _context.Update(paiement);
                            await _context.SaveChangesAsync();

                            await SendLogAsync("Le paiement a été enregistré pour " + paiement.Montant.ToString() + " avec l'utilisateur : " + paiement.UtilisateurId);

                            return Ok("PostPaiement, le paiement est terminé");
                        }
                        else
                        {
                            _context.Update(paiement);
                            await _context.SaveChangesAsync();

                            await SendLogAsync("Le paiement n'a pas été enregistré pour " + paiement.Montant.ToString() + " avec l'utilisateur : " + paiement.UtilisateurId);

                            return Ok("PostPaiement, le paiement est actualisé");
                        }
                    }
                    else
                    {
                        Paiement paiement = new Paiement();

                        paiement.UtilisateurId = param.UtilisateurId;
                        paiement.FilmId = param.FilmId;
                        paiement.Montant = param.Montant;

                        _context.Paiements.Add(paiement);
                        await _context.SaveChangesAsync();

                        await SendLogAsync("Le paiement du film " + param.FilmId + " d'un montant de : " + param.Montant + " pour l'utilisateur : " + param.UtilisateurId + " a été enregistré");
                        await PostDebitHistoryAsync(param.UtilisateurId, param.FilmId, param.Montant);

                        return Ok("PostPaiement : le nouveau paiement a été créer");
                    }

                }
                {
                    await SendLogAsync("L'utilisateur n'a pas assez de solde, Montant de la transaction :  " + param.Montant + " Montant du solde : " + user.solde);

                    return Problem("L'utilisateur n'a pas assez de solde, Montant de la transaction :  " + param.Montant + " Montant du solde : " + user.solde);
                }
            }
            else
            {
                // Handle the case where the request to the utilisateur_service fails
                // You might want to return an error or take appropriate action
                return Problem($"Error fetching user with ID {utilisateurId} from utilisateur_service. Status code: {response.StatusCode}");
            }
        }

        [HttpGet("{utilisateurId}/{filmId}")]
        public async Task<ActionResult<bool>> GetPaiement(int utilisateurId, int filmId)
        {
            Paiement paiement = _context.Paiements.Where(paiement => paiement.UtilisateurId == utilisateurId && paiement.FilmId == filmId).FirstOrDefault();

            if (paiement != null)
            {
                await SendLogAsync("Requête pour l'utilisateur : " + utilisateurId + " d'un paiement total de " + paiement.Montant + " ,film payé " + paiement.IsPayed);

                return paiement.IsPayed;
            }
            else
            {
                await SendLogAsync("Requête pour l'utilisateur : " + utilisateurId + " ,le film " + filmId + " n'est pas présent");

                return Ok("Erreur, le paiement n'est pas présent en base de données");
            }
        }

        private async Task SendLogAsync(string log)
        {
            var message = JsonSerializer.Serialize(new Logs { ServiceName = "PaiementService", Log = log });
            var client = new HttpClient(_handler, false);
            await client.PostAsync("http://MicroflixLogApi/api/Log", new StringContent(message, Encoding.UTF8, "application/json"));
        }

        public async Task PostDebitHistoryAsync(int userId, int movieId, decimal amount)
        {
            var message = JsonSerializer.Serialize(new DebitHistory
            {
                UserId = userId,
                MovieId = movieId,
                Amount = amount
            });
            var client = new HttpClient(_handler, false);
            await client.PostAsync("http://MicroflixHistoryApi/api/DebitHistory",
                new StringContent(message, Encoding.UTF8, "application/json"));
        }
    }
}
