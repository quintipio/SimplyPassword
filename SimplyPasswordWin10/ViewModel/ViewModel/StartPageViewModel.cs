using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SimplyPasswordWin10.Abstract;
using SimplyPasswordWin10.Views;
using SimplyPasswordWin10Shared.Business;
using SimplyPasswordWin10Shared.Context;
using SimplyPasswordWin10Shared.Enum;
using SimplyPasswordWin10Shared.Utils;

namespace SimplyPasswordWin10.ViewModel 
{
    /// <summary>
    /// ViewModel de la page de démarrage
    /// </summary>
    public partial class StartPageViewModel : AbstractViewModel
    {
        /// <summary>
        /// Constructeur
        /// </summary>
        public StartPageViewModel(ModeOuvertureEnum mode)
        {
            NomPage = ContexteStatic.NomAppli;
            ModeSelect = mode;
            ImageUnlockBusiness.Init();
        }

        /// <summary>
        /// Vérifie les données
        /// </summary>
        /// <returns>les erreurs évntuelles</returns>
        private string Validate()
        {
            var retour = "";
            try
            {

                if ((string.IsNullOrWhiteSpace(MotDePasseA) || string.IsNullOrWhiteSpace(MotDePasseB)) &&
                    ModeSelect == ModeOuvertureEnum.FichierACreer)
                {
                    retour += ResourceLoader.GetForCurrentView("Errors").GetString("erreurChampsNonRemplis") + "\r\n";
                }

                if (!string.IsNullOrWhiteSpace(MotDePasseA) && MotDePasseA.Length < 8 &&
                    ModeSelect == ModeOuvertureEnum.FichierACreer)
                {
                    retour += ResourceLoader.GetForCurrentView("Errors").GetString("erreurNb8Caracteres") + "\r\n";
                }

                if (!string.IsNullOrWhiteSpace(MotDePasseA) && !string.IsNullOrWhiteSpace(MotDePasseB) &&
                    !MotDePasseA.Equals(MotDePasseB) && ModeSelect == ModeOuvertureEnum.FichierACreer)
                {
                    retour += ResourceLoader.GetForCurrentView("Errors").GetString("erreurMDPNonIdentiques") + "\r\n";
                }
            }
            catch (Exception)
            {
                retour += ResourceLoader.GetForCurrentView("Errors").GetString("erreurInconnu") + "\r\n";
            }
            

            return retour;
        }

        /// <summary>
        /// Charge un fichier de mot de passe, ou le créer si aucun
        /// </summary>
        /// <returns>les erreurs à afficher</returns>
        public async Task<string> Valider()
        {
            var retour = Validate();

            if (string.IsNullOrWhiteSpace(retour))
            {
                //si le fichier est à créer
                if (ModeSelect.Equals(ModeOuvertureEnum.FichierACreer))
                {
                    PasswordBusiness.Password = MotDePasseA;
                    await PasswordBusiness.Save();
                }

                if (ModeSelect.Equals(ModeOuvertureEnum.FichierDejaExistant))
                {
                    if (!await PasswordBusiness.Load(MotDePasseA, true))
                    {
                        retour += ResourceLoader.GetForCurrentView("Errors").GetString("erreurOuvertureFichier");
                    }
                    else
                    {
                        if (ContexteAppli.IsCortanaActive)
                        {
                            await CortanaBusiness.CheckPassword();
                        }
                    }
                }
            }
            return retour;
        }

        /// <summary>
        /// Réinitialise l'application
        /// </summary>
        /// <returns>la task</returns>
        public async Task ReinitAppli()
        {
            if (ContexteAppli.IsFichierRoamingOuvert)
            {
                if (await ContexteAppli.ReinitAppli())
                {
                    ((Frame)Window.Current.Content).Navigate(typeof(StartPageView), ModeOuvertureEnum.FichierACreer);
                }
            }
            
        }

        /// <summary>
        /// Calcule sur 100 la force d'un mot de passe
        /// </summary>
        /// <param name="mdp">le mot de passe</param>
        /// <returns>sa force</returns>
        private double CalculForceMdp(string mdp)
        {
           return CryptUtils.CalculForceMotDePasse(mdp);
        }
    }
}
