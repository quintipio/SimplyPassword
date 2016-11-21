using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI;
using Windows.UI.Xaml.Media;
using SimplyPasswordWin10.Abstract;
using SimplyPasswordWin10Shared.Business;
using SimplyPasswordWin10Shared.Context;
using SimplyPasswordWin10Shared.Strings;
using SimplyPasswordWin10Shared.Utils;

namespace SimplyPasswordWin10.ViewModel
{
    /// <summary>
    /// ViewModel pour les paramètres (actuellement changement de passe du fichier utilisateur)
    /// </summary>
    public partial class ParamsViewModel : AbstractViewModel
    {
        //indique si la page est en cours de chargement
        private readonly bool _isPageStarting;

        /// <summary>
        /// Constructeur
        /// </summary>
        public ParamsViewModel()
        {
            _isPageStarting = true;
            ListeLangue = new ObservableCollection<ListeLangues.LanguesStruct>(ListeLangues.GetListesLangues());
            SelectedLangue = ListeLangues.GetLangueEnCours();
            CortanaActive = ContexteAppli.IsCortanaActive;

            ListeCouleur = new ObservableCollection<SolidColorBrush>();
            foreach (var couleur in ContexteStatic.ListeCouleur)
                ListeCouleur.Add(GetColor(couleur));
            _isPageStarting = false;
        }

        #region nouveau mot de passe
        /// <summary>
        /// créer un mot de passe aléatoire
        /// </summary>
        /// <returns>le mot de passe</returns>
        public void GenererPassword(int longueur,bool lettre,bool chiffres, bool spec)
        {
            NewMdp = CryptUtils.GeneratePassword(longueur,lettre,chiffres,spec);
        }

        /// <summary>
        /// calcul la force d'un mot de passe et retourne en 0 et 100 la résistance du mot de passe
        /// </summary>
        /// <param name="pass">le mot de passe</param>
        /// <returns>sa résistance en %</returns>
        public int CalculerForceMotdePasse(string pass)
        {
            return CryptUtils.CalculForceMotDePasse(pass);
        }

        /// <summary>
        /// Méthode de controle des informations
        /// </summary>
        /// <returns>la liste des message d'erreurs</returns>
        private string Validate()
        {
            string retour = "";
            //champ vide
            if(string.IsNullOrWhiteSpace(OldMdp) || string.IsNullOrWhiteSpace(NewMdp) || string.IsNullOrWhiteSpace(Confirmation))
            {
                retour += ResourceLoader.GetForCurrentView("Errors").GetString("erreurChampsNonRemplis")+ "\r\n";
            }

            if (NewMdp != null && OldMdp != null)
            {
                //mot de passe et confirmation sont pareil
                if (!Equals(NewMdp, Confirmation))
                {
                    retour += ResourceLoader.GetForCurrentView("Errors").GetString("erreurMDPDifferents") + "\r\n";
                }

                //taille du mot de passe
                if (NewMdp.Length < 8 || OldMdp.Length < 8)
                {
                    retour += ResourceLoader.GetForCurrentView("Errors").GetString("erreurNb8Caracteres") + "\r\n";
                }

                //vérifie l'ancien mot de passe
                if (!Equals(OldMdp, PasswordBusiness.Password))
                {
                    retour += ResourceLoader.GetForCurrentView("Errors").GetString("erreurAncienMDPIncorrect") + "\r\n";
                }
            }
            
            return retour;
        }

        /// <summary>
        /// change le mot de passe et sauvegarde le fichier
        /// </summary>
        /// <returns>les erreurs du validate sinon une ssting vide</returns>
        public async Task<string> ChangerMdp()
        {
            var retour = Validate();

            if(string.IsNullOrWhiteSpace(retour))
            {
                PasswordBusiness.Password = NewMdp;
                await PasswordBusiness.Save();
                //si cortana est activé, changer le fichier
                if (CortanaActive!= null && CortanaActive.Value)
                {
                    await CortanaBusiness.SavePassword();
                }
            }

            return retour;
        }
        #endregion

        #region couleur
        /// <summary>
        ///     Retourne le solidColorBrush à appliquer à un rectangle à partir de son code couleur
        /// </summary>
        /// <param name="color">la couleur</param>
        /// <returns></returns>
        private SolidColorBrush GetColor(uint color)
        {
            var hex = string.Format("{0:X}", color);
            return new SolidColorBrush(Color.FromArgb(byte.Parse(hex.Substring(0, 2), NumberStyles.AllowHexSpecifier),
                byte.Parse(hex.Substring(2, 2), NumberStyles.AllowHexSpecifier),
                byte.Parse(hex.Substring(4, 2), NumberStyles.AllowHexSpecifier),
                byte.Parse(hex.Substring(6, 2), NumberStyles.AllowHexSpecifier)));
        }

        /// <summary>
        /// Change la couleur de l'application
        /// </summary>
        /// <param name="solidColor">la nouvelle couleur</param>
        /// <returns>la task</returns>
        public async Task ChangeCouleur(SolidColorBrush solidColor)
        {
            var color =
                (uint)
                    ((solidColor.Color.A << 24) | (solidColor.Color.R << 16) | (solidColor.Color.G << 8) |
                     (solidColor.Color.B << 0));
            ContexteAppli.IdCouleurTheme = ContexteStatic.ListeCouleur.IndexOf(color);
            await ParamBusiness.Save();
        }
        #endregion

        #region langue
        /// <summary>
        /// Change la langue de l'application
        /// </summary>
        public async Task ChangeLangueApplication()
        {
            ListeLangues.ChangeLangueAppli(SelectedLangue);
            await ParamBusiness.Save();
        }
        #endregion

        #region Cortana

        /// <summary>
        /// Modifie l'activité de cortana (si activé, enregistre le mot de passe en le chiffrant, si déasctivé, supprime le mot de passe)
        /// </summary>
        public async Task ChangeCortana(bool value)
        {
            if (!_isPageStarting)
            {
                //changement du paramètre
                ContexteAppli.IsCortanaActive = value;
                await ParamBusiness.Save();

                if (ContexteAppli.IsCortanaActive)
                {
                    await CortanaBusiness.SavePassword();
                }
                else
                {
                    await CortanaBusiness.DeletePassword();
                }
            }
        }
        
        #endregion
    }
}
