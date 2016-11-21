using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using SimplyPasswordWin10.Abstract;
using SimplyPasswordWin10Shared.Business;
using SimplyPasswordWin10Shared.Context;
using SimplyPasswordWin10Shared.Model;

namespace SimplyPasswordWin10.ViewModel
{
    /// <summary>
    /// Viewmodel de la vue ouverte avec cortana
    /// </summary>
    public partial class ResultCortanaViewModel : AbstractViewModel
    {
        public ResultCortanaViewModel(string recherche)
        {
            NomPage = ContexteStatic.NomAppli;
            _recherche = recherche;
        }

        /// <summary>
        /// Permet de vérifier si l'application peut s'autoLogger, s'autologue si possible, et lance la recherche
        /// </summary>
        /// <returns>true si à tout réussi à faire automatiquement</returns>
        public async Task<bool> LanceRechercheSansMotDePasse()
        {
            if (ContexteAppli.IsCortanaActive)
            {
                var mdp = await CortanaBusiness.GetPasswordUser();
                if (!string.IsNullOrWhiteSpace(mdp))
                {
                    if (await PasswordBusiness.Load(mdp,true))
                    {
                        await LancerRecherche();
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Vérifie les données
        /// </summary>
        /// <returns>les erreurs évntuelles</returns>
        private string Validate()
        {
            var retour = "";
            
            if (string.IsNullOrWhiteSpace(MotDePasse) || (string.IsNullOrWhiteSpace(MotDePasse) && MotDePasse.Length < 8 ))
            {
                retour += ResourceLoader.GetForCurrentView("Errors").GetString("erreurNb8Caracteres") + "\r\n";
            }
            return retour;
        }

        public async Task<string> Valider()
        {
            var retour = Validate();

            if (string.IsNullOrWhiteSpace(retour))
            {
                if (!await PasswordBusiness.Load(MotDePasse,true))
                {
                    retour += ResourceLoader.GetForCurrentView("Errors").GetString("erreurOuvertureFichier");
                }
                else
                {
                    await LancerRecherche();
                }
            }
            return retour;
        }

        /// <summary>
        /// Lance la recherche du mot de passe et met en mémoire les mots de passe à retenir
        /// </summary>
        private async Task LancerRecherche()
        {
            var res = CortanaBusiness.GetMotDePasse(_recherche,ContexteAppli.DossierMere);
            ListeMotDePasse = new ObservableCollection<MotDePasse>(res.OrderBy(o => o.Titre).ToList());

            //si je suis arrivé ici alors que Cortana est activé et que le nombre de résultat est inférieur à 10, c'est qu'il y a eu problème sur le déchiffrement du mot de passe
            if (ContexteAppli.IsCortanaActive && res.Count < 10)
            {
               await CortanaBusiness.CheckPassword();
            }
        }
    }
}
