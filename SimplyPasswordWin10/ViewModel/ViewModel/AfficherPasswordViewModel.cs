using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using SimplyPasswordWin10.Abstract;
using SimplyPasswordWin10Shared.Business;
using SimplyPasswordWin10Shared.Model;

namespace SimplyPasswordWin10.ViewModel
{
    /// <summary>
    /// Controleur de l'affichage d'un mot de passe
    /// </summary>
    public partial class AfficherPasswordViewModel : AbstractViewModel
    {

        /// <summary>
        /// constructeur
        /// </summary>
        /// <param name="password">le mot de passe à afficher</param>
        public AfficherPasswordViewModel(MotDePasse password)
        {
            Password = password;
            PasswordReveal = false;
            IsCommentAffich();
            IsSiteAffich();
        }

        /// <summary>
        /// Permet d'afficher ou non le commentaire
        /// </summary>
        private void IsCommentAffich()
        {
            IsCommentVisible = !string.IsNullOrWhiteSpace(Password.Commentaire);
        }

        /// <summary>
        /// permet d'afficher ou non le site
        /// </summary>
        private void IsSiteAffich()
        {
            IsSiteVisible = !string.IsNullOrWhiteSpace(Password.SiteWeb);
            if (IsSiteVisible)
            {
                var lien = Password.SiteWeb;
                if (Password.SiteWeb.Contains("http://www.")) lien = lien.Replace("http://www.", "");
                if (Password.SiteWeb.Contains("https://www.")) lien = lien.Replace("https://www.", "");
                if (Password.SiteWeb.Contains("www.")) lien = lien.Replace("www.", "");
            }
        }

        /// <summary>
        /// Génère le mot de passe a afficher en focntion du droit de visibilité
        /// </summary>
        private void GenererMotDePasseAffiche(bool isToAffich)
        {
            if (!isToAffich)
            {
                PasswordToAffich = "";
                for (var i = 0; i < Password.MotDePasseObjet.Length; i++)
                {
                    PasswordToAffich += "*";
                }
            }
            else
            {
                PasswordToAffich = Password.MotDePasseObjet;
            }
        }

        /// <summary>
        /// supprime un mot de passe des données de l'utilisateur et sauvegarde le fichier
        /// </summary>
        public async Task SupprimerMotDepasse()
        {
            Password.DossierPossesseur.ListeMotDePasse.Remove(Password);
            await PasswordBusiness.Save();

            //mise à jour de la liste de cortana
            await CortanaBusiness.UpdateCortana();
        }

        /// <summary>
        /// copie une string dans le presse papier
        /// </summary>
        /// <param name="data">la donnée à copier</param>
        public void DataIntoClipBoard(string data)
        {
            var tmp = new DataPackage();
            tmp.SetText(data);
            Clipboard.SetContent(tmp);
        }

        /// <summary>
        /// Permet de naviguer dans la dlg des dossiers
        /// </summary>
        /// <param name="dossier"></param>
        public void NaviguerDossier(Dossier dossier)
        {
            IsParentVisible = dossier.DossierParent != null;
            DossierEncours = dossier;
            ListeDossierAffiche = (dossier.SousDossier != null) ? new ObservableCollection<Dossier>(dossier.SousDossier) : new ObservableCollection<Dossier>();
        }

        /// <summary>
        /// Change l'emplacement du mot de passe avec le dossier en cours de navigation
        /// </summary>
        public async Task ChangerEmplacementMotDePasse()
        {
            Password.DossierPossesseur.ListeMotDePasse.Remove(Password);
            DossierEncours.ListeMotDePasse.Add(Password);
            Password.DossierPossesseur = DossierEncours;
            await PasswordBusiness.Save();
            SelectedDossierAbstract = DossierEncours;
        }
    }
}
