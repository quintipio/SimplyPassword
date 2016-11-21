using System.Collections.Generic;
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
    /// Controleur de l'application en générale
    /// </summary>
    public partial  class MainPageViewModel : AbstractViewModel
    {
        /// <summary>
        /// Constructeur placant par défaut dans le dossier mère
        /// </summary>
        public MainPageViewModel()
        {
            DossierSelected = ContexteAppli.DossierMere;
            TitrePage = ContexteStatic.NomAppli;
            DossierToEdit = null;
        }


        /// <summary>
        /// met à jour la date de dernière modification du fichier
        /// </summary>
        public async Task MajDerniereSynchro()
        {
            var date = await PasswordBusiness.GetDerniereModifFile();
            DerniereSynchro = ResourceLoader.GetForCurrentView().GetString("textDerniereSynchro") + date.Day + "/" +
                date.Month + "/" +
                date.Year + " " +
                date.Hour + ":" +
                date.Minute + ":" +
                date.Second;

            EspaceOccupe = (ContexteAppli.IsFichierRoamingOuvert)?ResourceLoader.GetForCurrentView().GetString("textEspaceOccupe") +
                           await PasswordBusiness.GetEspaceFichierOccupePourcent() + " %" : await PasswordBusiness.GetNameFile();
        }

        /// <summary>
        /// Vérifie les informations avant l'ajout ou la modification d'un dossier
        /// </summary>
        /// <param name="nom">le nouveau nom du dossier</param>
        /// <returns>les erreurs</returns>
        private static string ValidateDossier(string nom)
        {
            var resourceLoader = ResourceLoader.GetForCurrentView("Errors");
            var retour = "";
            //vérifie si le nouveau nom n'est pas vide
            if (string.IsNullOrWhiteSpace(nom))
            {
                retour += resourceLoader.GetString("erreurNomDossierVide") + "\r\n";
            }

            return retour;
        }



        #region GestionDossier
        /// <summary>
        /// Ajoute le nouveau dossier au dossier en cours et sauvegarde le tout
        /// </summary>
        /// <returns>les erreurs à afficher</returns>
        public async Task<string> AjouterDossier()
        {
            var retour = ValidateDossier(DossierToEditName);

            if(string.IsNullOrWhiteSpace(retour))
            {
                var newDossier = new Dossier(DossierToEditName, DossierSelected);
                DossierSelected.SousDossier.Add(newDossier);
                await PasswordBusiness.Save();
                OuvrirDossier(DossierSelected);
            }
            return retour;
        }

        /// <summary>
        /// modifie le nom d'un dossier
        /// </summary>
        /// <returns>les erreurs</returns>
        public async Task<string> ModifierDossier()
        {
            var resourceLoader = ResourceLoader.GetForCurrentView("Errors");
            var retour = ValidateDossier(DossierToEditName);
            if(string.IsNullOrWhiteSpace(retour))
            {
                if (DossierToEdit != null)
                {
                    DossierToEdit.Titre = DossierToEditName;
                    await PasswordBusiness.Save();
                    OuvrirDossier(DossierSelected);
                }
                else
                {
                    retour += resourceLoader.GetString("erreurAucunDossier") + "\r\n";
                }
            }
            return retour;
        }

        /// <summary>
        /// supprime un dossier
        /// </summary>
        /// <param name="toDelete">le dossier à effacer</param>
        /// <returns>les erreurs à afficher</returns>
        public async Task<string> SuppprimerDossier(Dossier toDelete)
        {
            var resourceLoader = ResourceLoader.GetForCurrentView("Errors");
            if(toDelete != null)
            {
                if(toDelete.DossierParent != null)
                {
                   toDelete.DossierParent.SousDossier.Remove(toDelete);
                    await PasswordBusiness.Save();
                    OuvrirDossier(DossierSelected);
                }
            }
            else
            {
                return resourceLoader.GetString("erreurAucunDossierToSup") + "\r\n";
            }
            return "";
        }
        #endregion

        /// <summary>
        /// Ouvre un dossier en mettant en place les listes
        /// </summary>
        /// <param name="newDossier">le dosier à lire</param>
        public void OuvrirDossier(Dossier newDossier)
        {
            DossierSelected = newDossier;
            ListeDossier = new ObservableCollection<Dossier>(DossierSelected.SousDossier);
            IsBackButtonVisible = !ContexteAppli.DossierMere.Equals(DossierSelected);
            SelectedDossierAbstract = DossierSelected;
            RafraichirMotDePasse();
        }

        /// <summary>
        /// Recharge la liste des mots de passes à afficher
        /// </summary>
        public void RafraichirMotDePasse()
        {
            ListeMotDePasse = new ObservableCollection<MotDePasse>(DossierSelected.ListeMotDePasse.OrderBy(o => o.Titre).ToList());
        }

        /// <summary>
        /// Ouvre le dossier parent
        /// </summary>
        public void OuvriDossierParent()
        {
            if (DossierSelected.DossierParent != null)
            {
                OuvrirDossier(DossierSelected.DossierParent);
            }
        }


        #region recherche
        /// <summary>
        /// Lance la recherche de mot de passe
        /// </summary>
        /// <param name="chaine"></param>
        private void RechercheMotDePasse(string chaine)
        {
            if (!string.IsNullOrWhiteSpace(chaine))
            {
                var liste = ChercherMotDePasse(ContexteAppli.DossierMere, chaine);
                ListeMotDePasse = new ObservableCollection<MotDePasse>(liste.OrderBy(o => o.Titre).ToList());
            }
            else
            {
                RafraichirMotDePasse();
            }
        }

        /// <summary>
        /// méthode récursive recherchant tout les mots de passe compris dans un dossier et ses sous dossiers
        /// </summary>
        /// <param name="dossier">le dossier dans lequel s'effectue la recherche</param>
        /// <param name="texteRechercher">le texte à rechercher</param>
        /// <returns>la liste des mots de passe trouvé</returns>
        private static IEnumerable<MotDePasse> ChercherMotDePasse(Dossier dossier, string texteRechercher)
        {
            var retour = new List<MotDePasse>();

            //recherche du mot de passe
            foreach (var m in dossier.ListeMotDePasse)
            {
                if (m.Titre.ToLower().Contains(texteRechercher.ToLower()) ||
                        m.Login.ToLower().Contains(texteRechercher.ToLower()))
                {
                    retour.Add(m);
                }
            }

            //recherche dans les sous dossiers
            foreach (var d in dossier.SousDossier)
            {
                retour.AddRange(ChercherMotDePasse(d, texteRechercher));
            }
            return retour;
        }
        #endregion
    }
}
