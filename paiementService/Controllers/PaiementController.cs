using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using paiementService.Context;
using paiementService.Models;
using paiementService.Models.Commands;

namespace paiementService.Controllers
{
    [Route("api/paiement")]
    [ApiController]
    public class PaiementController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public PaiementController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<PaiementController>> PostPaiement(PaiementCommand param)
        {
            if (_context.Paiements == null)
            {
                return Problem("L'entité 'DatabaseContext.Paiements' est null.");
            }

            if (_context.Paiements.Where(paiement => paiement.FilmId == param.FilmId && paiement.UtilisateurId == param.UtilisateurId).Any())
            {
                Paiement paiement = _context.Paiements.Where(paiement => paiement.FilmId == param.FilmId && paiement.UtilisateurId == param.UtilisateurId).FirstOrDefault();

                paiement.Montant += param.Montant;

                if (paiement.Montant > param.TotalPrix)
                {
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

                return Ok("PostPaiement : le nouveau paiement a été créer");
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

        private async Task SendLogAsync(string message)
        {
            using (var httpClient = new HttpClient())
            {
                string apiUrl = "https://localhost:7211/api/Log";

                var requestData = new
                {
                    ServiceName = "Paiement",
                    Log = message,
                };

                var jsonContent = JsonConvert.SerializeObject(requestData);

                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                try
                {
                    var response = await httpClient.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(responseContent);
                    }
                    else
                    {
                        Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                }
            }
        }
    }
}
