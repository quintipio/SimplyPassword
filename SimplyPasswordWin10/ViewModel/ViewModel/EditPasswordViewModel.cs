using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using SimplyPasswordWin10.Abstract;
using SimplyPasswordWin10Shared.Business;
using SimplyPasswordWin10Shared.Context;
using SimplyPasswordWin10Shared.Enum;
using SimplyPasswordWin10Shared.Model;
using SimplyPasswordWin10Shared.Utils;

namespace SimplyPasswordWin10.ViewModel
{
    /// <summary>
    /// controleur de l'édition ou de l'ajout des identifiants
    /// </summary>
    public partial class EditPasswordViewModel : AbstractViewModel
    {

        /// <summary>
        /// constructeur pour la modification d'un mot de passe
        /// </summary>
        /// <param name="password">le mot de passe à modifier</param>
        public EditPasswordViewModel(MotDePasse password)
        {
            ListeIcone =new ObservableCollection<Image>(ContexteAppli.ListeIcone);

            DossierPossesseur = password.DossierPossesseur;
            Titre = password.Titre;
            Login = password.Login;
            Password = password.MotDePasseObjet;
            Site = password.SiteWeb;
            Commentaire = password.Commentaire;
            SelectedIcone = password.IdIcone;
            PasswordOriginal = password;
            Action = ActionMotDePasseEnum.Modifier;
        }

        /// <summary>
        /// constructeur pour l'ajout d'un mot de passe
        /// </summary>
        /// <param name="dossierPossesseur">le dossier dans lequel sera créer le mot de passe</param>
        public EditPasswordViewModel(Dossier dossierPossesseur)
        {
            PasswordOriginal = null;
            DossierPossesseur = dossierPossesseur;
            ListeIcone = new ObservableCollection<Image>(ContexteAppli.ListeIcone);
            Action = ActionMotDePasseEnum.Ajouter;
        }

        /// <summary>
        /// créer un mot de passe
        /// </summary>
        /// <returns>le mot de passe</returns>
        public void GenererPassword(int longueur,bool lettre,bool chiffres,bool spec)
        {
             Password = CryptUtils.GeneratePassword(longueur, lettre, chiffres, spec);
        }

        /// <summary>
        /// calcul la force d'un mot de passe
        /// </summary>
        /// <param name="pass">le mot de passe</param>
        public void CalculerForceMotdePasse(string pass)
        {
            Force = CryptUtils.CalculForceMotDePasse(pass);
        }

        /// <summary>
        /// vérifie les informations à enregistré
        /// </summary>
        /// <returns>les erreurs</returns>
        private string Validate()
        {
            var retour = "";
                //vérifie que le mot de passe se trouve dans un dossier
                if(DossierPossesseur == null)
                {
                    retour += ResourceLoader.GetForCurrentView("Errors").GetString("erreurAucunDossierSelect") + "\r\n";
                }

                //champs titre non vide
                if (string.IsNullOrWhiteSpace(Titre))
                {
                    retour += ResourceLoader.GetForCurrentView("Errors").GetString("erreurChampsTitreVide") + "\r\n";
                }

                //titre pas trop long
                if(Titre.Length > 50)
                {
                    retour += ResourceLoader.GetForCurrentView("Errors").GetString("erreurTitreLong") + "\r\n";
                }

                //identifiants non vide
                if(string.IsNullOrWhiteSpace(Login))
                {
                    retour += ResourceLoader.GetForCurrentView("Errors").GetString("erreurIdentifiantVide") + "\r\n";
                }

                //mot de passe non vide
                if(string.IsNullOrWhiteSpace(Password))
                {
                    retour += ResourceLoader.GetForCurrentView("Errors").GetString("erreurMDPVide") + "\r\n";
                }
            
            return retour;
        }

        /// <summary>
        /// Sauvegarde dans le fichier les modifications apportés  un mot de passe, ou l'ajout de ce dernier
        /// </summary>
        /// <returns>les erreurs éventuellements générés</returns>
        public async Task<string> Save()
        {
            var retour = Validate();
            //si aucune erreur
            if(string.IsNullOrWhiteSpace(retour))
            {
                //en cas d'ajout du mot de passe
                if (Action.Equals(ActionMotDePasseEnum.Ajouter))
                {
                    var mdp = new MotDePasse
                    {
                        Titre = Titre,
                        Login = Login,
                        MotDePasseObjet = Password,
                        SiteWeb = Site,
                        Commentaire = Commentaire,
                        DossierPossesseur = DossierPossesseur,
                        IdIcone = SelectedIcone
                    };

                    var res = ContexteAppli.ListeIcone.Where(x => (int)x.Tag == SelectedIcone).ToList();
                    if (res.Count > 0)
                    {
                        mdp.Icone = (BitmapImage)res[0].Source;
                    }
                    else
                    {
                        mdp.Icone = null;
                    }
                    DossierPossesseur.ListeMotDePasse.Add(mdp);
                }
                //en cas de modification placement des nouvelles informations
                if (Action.Equals(ActionMotDePasseEnum.Modifier))
                {
                    PasswordOriginal.Titre = Titre;
                    PasswordOriginal.Login = Login;
                    PasswordOriginal.MotDePasseObjet = Password;
                    PasswordOriginal.IdIcone = SelectedIcone;
                    PasswordOriginal.Commentaire = Commentaire;
                    PasswordOriginal.SiteWeb = Site;
                    var res = ContexteAppli.ListeIcone.Where(x => (int)x.Tag == PasswordOriginal.IdIcone).ToList();
                    if (res.Count > 0)
                    {
                        PasswordOriginal.Icone = (BitmapImage)res[0].Source;
                    }
                    else
                    {
                        PasswordOriginal.Icone = null;
                    }
                }
                //sauvegarde
                await PasswordBusiness.Save();


                //mise à jour de la liste de cortana
                await CortanaBusiness.UpdateCortana();
            }
            return retour;
        }
    }
}
