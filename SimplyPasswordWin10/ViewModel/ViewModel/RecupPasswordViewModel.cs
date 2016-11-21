using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using SimplyPasswordWin10.Abstract;
using SimplyPasswordWin10Shared.Business;
using SimplyPasswordWin10Shared.Context;
using SimplyPasswordWin10Shared.Model;
using SimplyPasswordWin10Shared.Utils;

namespace SimplyPasswordWin10.ViewModel
{
    /// <summary>
    /// Controleur pour la page de récupération pour un mot de passe partagé
    /// </summary>
    public partial class RecupPasswordViewModel : AbstractViewModel
    {
        /// <summary>
        /// Constructeur
        /// </summary>
        public RecupPasswordViewModel()
        {
        }

        /// <summary>
        /// Initialise le controleur
        /// </summary>
        /// <param name="listeMotDePasse">la liste des mots de passe à importer</param>
        public void Init(IEnumerable<MotDePasse> listeMotDePasse)
        {

            ListeMotDePasses = new ObservableCollection<MotDePasse>(listeMotDePasse);
            IsLog = !string.IsNullOrEmpty(PasswordBusiness.Password);
        }

        /// <summary>
        /// Initialise le controleur
        /// </summary>
        /// <param name="fichier">le fichier à ouvrir contenant la liste des mots de passe</param>
        /// <returns>la liste des mots de passes sinon null</returns>
        public async Task Init(StorageFile fichier)
        {
            try
            {
                //lecture
                var buffer = (await FileIO.ReadBufferAsync(fichier)).ToArray();

                //déchiffrement
                var data = CryptUtils.AesDecryptByteArrayToString(buffer, PartageViewModel.CryptKey, PartageViewModel.CryptSalt);

                //deserialization
                var xsb = new XmlSerializer(typeof(ObservableCollection<MotDePasse>));
                var rd = new StringReader(data);
                ListeMotDePasses = xsb.Deserialize(rd) as ObservableCollection<MotDePasse>;
                IsLog = !string.IsNullOrEmpty(PasswordBusiness.Password);
            }
            catch
            {
                //Ignored
            }
        }

        

        /// <summary>
        /// Controle des données avant l'import
        /// </summary>
        /// <returns></returns>
        private string Validate()
        {
            var erreur = "";
            if (ListeMotDePasses.Count == 0 || ListeMotDePasses.Count(x => x.Selected) == 0)
            {
                erreur += ResourceLoader.GetForCurrentView("Errors").GetString("erreurAucunMdpRecup") + "\r\n";
            }
            return erreur;
        }

        /// <summary>
        /// Lance l'opération de récupération des mots de passe
        /// </summary>
        /// <returns>les erreurs à afficher.</returns>
        public async Task<string> Recuperer()
        {
            ValidSave = false;
            if (IsLog)
            {
                var erreur = Validate();
                if (string.IsNullOrWhiteSpace(erreur))
                {
                    foreach (var mdp in ListeMotDePasses.Where(mdp => mdp.Selected))
                    {
                        if (mdp.IdIcone != 0 && mdp.IdIcone < ContexteAppli.ListeIcone.Count)
                        {
                            mdp.Icone = (BitmapImage)ContexteAppli.ListeIcone[mdp.IdIcone].Source;
                        }
                        else
                        {
                            mdp.Icone = null;
                        }

                        if (mdp.DossierPossesseur == null)
                        {
                            mdp.DossierPossesseur = ContexteAppli.DossierMere;
                        }
                        mdp.DossierPossesseur.ListeMotDePasse.Add(mdp);
                    }
                    
                    await PasswordBusiness.Save();
                    ValidSave = true;
                    return ResourceLoader.GetForCurrentView("Errors").GetString("infoMdpAjoute");
                }

                return erreur;
            }
            return ResourceLoader.GetForCurrentView("Errors").GetString("erreurFichierPasOuvert");
        }


        public void NaviguerDossier(Dossier dossier)
        {
            IsParentVisible = dossier.DossierParent != null;
            DossierEncours = dossier;
            ListeDossierAffiche = (dossier.SousDossier != null)? new ObservableCollection<Dossier>(dossier.SousDossier): new ObservableCollection<Dossier>();
        }

        /// <summary>
        /// Tente d'ouvrir le fichier de mot de passe retourne null si ok sinon le message d'erreur
        /// </summary>
        /// <returns>le message d'erreur sinon null</returns>
        public async Task<string> OuvrirFichierPassword()
        {
            if (string.IsNullOrWhiteSpace(Password))
            {
                Password = "";
                return ResourceLoader.GetForCurrentView("Errors").GetString("erreurChampsNonRemplis") + "\r\n";
            }
            

            if (!await PasswordBusiness.Load(Password, true))
            {
                Password = "";
                return ResourceLoader.GetForCurrentView("Errors").GetString("erreurOuvertureFichier");
            }
            IsLog = true;
            return null;
        }
    }
}
