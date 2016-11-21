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
using SimplyPasswordWin10.Abstract;
using SimplyPasswordWin10Shared.Context;
using SimplyPasswordWin10Shared.Enum;
using SimplyPasswordWin10Shared.Model;
using SimplyPasswordWin10Shared.Utils;

namespace SimplyPasswordWin10.ViewModel
{
    /// <summary>
    /// Controleur de la page de partage de mots de passe
    /// </summary>
    public partial class PartageViewModel : AbstractViewModel
    {

        /// <summary>
        /// Constructeur
        /// </summary>
        public PartageViewModel()
        {
            ListeMotDePasseSelected = new ObservableCollection<MotDePasse>();
            ListeMotDePasseSelectedChaine = "";
        }


        #region Dlg des mots de passe
        /// <summary>
        /// Charge la liste des mots de passe à afficher
        /// </summary>
        /// <param name="recherche">un identifiant à rechercher</param>
        public void ChargerMotsDePasse(string recherche)
        {
            if (ContexteAppli.DossierMere != null)
            {
                //chargement de la liste des mots de passe
                var liste =
                    new ObservableCollection<MotDePasse>(ChercherMotDePasse(ContexteAppli.DossierMere, recherche));
                ListeMotDePasse = new ObservableCollection<MotDePasse>(liste.OrderBy(x => x.Titre));

                foreach (var mdp in ListeMotDePasse)
                {
                    if (ListeMotDePasseSelected.Contains(mdp))
                    {
                        mdp.Selected = true;
                    }
                    else
                    {
                        mdp.Selected = false;
                    }
                }
            }
        }

        /// <summary>
        /// Ajoute un mot de passe à la liste des mots de passe sélectionné et l'ajoute à la chaine de caractères à afficher
        /// </summary>
        /// <param name="mdp">le mot de passe à ajouter</param>
        public void AjouterMotDePasse(MotDePasse mdp)
        {
            if (mdp != null)
            {
                ListeMotDePasseSelected.Add(mdp);
                GenererChaineMotDePasse();
            }
        }

        /// <summary>
        /// Supprime un mot de passe de la liste des mots de passe sélectionné et le supprime de la chaine de caractères à afficher
        /// </summary>
        /// <param name="mdp">le mot de passe à supprimer</param>
        public void SupprimerMotDePasse(MotDePasse mdp)
        {
            if (mdp != null)
            {
                ListeMotDePasseSelected.Remove(mdp);
                GenererChaineMotDePasse();
            }
        }

        /// <summary>
        /// Genere une liste en string des mots de passe sélectionné
        /// </summary>
        private void GenererChaineMotDePasse()
        {
            ListeMotDePasseSelectedChaine = "";
            if (ListeMotDePasseSelected.Count == 0)
            {
                ListeMotDePasseSelectedChaine = "";
            }
            else if (ListeMotDePasseSelected.Count == 1)
            {
                ListeMotDePasseSelectedChaine = ListeMotDePasseSelected[0].Titre;
            }
            else
            {
                var i = 0;
                foreach (var mdp in ListeMotDePasseSelected)
                {
                    ListeMotDePasseSelectedChaine += mdp.Titre;
                    i++;
                    if (i < ListeMotDePasseSelected.Count)
                    {
                        ListeMotDePasseSelectedChaine += ", ";
                    }
                }
            }
        }


        /// <summary>
        /// Recherche un mot de passe dans les dossiers et sous dossier
        /// </summary>
        /// <param name="dossier">le dossier à explorer</param>
        /// <param name="texteRechercher">l'identifiant à rechercher</param>
        /// <returns>la liste des mots de passes trouvés</returns>
        private IEnumerable<MotDePasse> ChercherMotDePasse(Dossier dossier, string texteRechercher)
        {
            //recherche du mot de passe
            var retour = dossier.ListeMotDePasse.Where(m => m.Titre.ToLower().Contains(texteRechercher.ToLower()) || string.IsNullOrEmpty(texteRechercher)).ToList();
            
            //recherche dans les sous dossiers
            foreach (var d in dossier.SousDossier)
            {
                retour.AddRange(ChercherMotDePasse(d, texteRechercher));
            }
            return retour;
        }
        #endregion


        #region partage/récupération

        /// <summary>
        /// Controle des erreurs éventuelles
        /// </summary>
        /// <returns>les erreurs</returns>
        private string Validate()
        {
            var erreur = "";
            if (Source == ModePartageSourceEnum.Fichier && Fichier == null)
            {
                erreur += ResourceLoader.GetForCurrentView("Errors").GetString("erreurAucunFichier") + "\r\n";
            }

            if (InOut == ModePartageInOutEnum.Recupération && Source == ModePartageSourceEnum.Texte && string.IsNullOrWhiteSpace(Texte))
            {
                erreur += ResourceLoader.GetForCurrentView("Errors").GetString("erreurAucunTexteRecup") + "\r\n";
            }

            if (InOut == ModePartageInOutEnum.Partage && ListeMotDePasseSelected.Count == 0)
            {
                erreur += ResourceLoader.GetForCurrentView("Errors").GetString("erreurAucunMdpRecup") + "\r\n";
            }

                return erreur;
        }

        /// <summary>
        /// Partage les mots de passe dans un fichier
        /// </summary>
        /// <returns>les erreurs si il y en a</returns>
        public async Task<string> PartageFichier()
        {
            var erreur = Validate();

            if (string.IsNullOrWhiteSpace(erreur))
            {
                //serialization
                var xs = new XmlSerializer(typeof(ObservableCollection<MotDePasse>));
                var wr = new StringWriter();
                xs.Serialize(wr, ListeMotDePasseSelected);
                //chiffrement
                var dataToSave = CryptUtils.AesEncryptStringToByteArray(wr.ToString(), CryptKey, CryptSalt);

                //écriture
                await FileIO.WriteBytesAsync(Fichier, dataToSave);
                erreur = ResourceLoader.GetForCurrentView("Errors").GetString("infoFichierGenere");
            }

            return erreur;
        }

        /// <summary>
        /// Partage les mots de passes dans du texte
        /// </summary>
        /// <returns>les erreurs si il y en a</returns>
        public string PartageTexte()
        {
            var erreur = Validate();

            if (string.IsNullOrWhiteSpace(erreur))
            {
                //serialization
                var xs = new XmlSerializer(typeof(ObservableCollection<MotDePasse>));
                var wr = new StringWriter();
                xs.Serialize(wr, ListeMotDePasseSelected);

                //chiffrement
                Texte = CryptUtils.AesEncryptStringToString(wr.ToString(), CryptKey, CryptSalt);
            }

            return erreur;
        }

        /// <summary>
        /// Récupère les mots de passe d'un fichier
        /// </summary>
        /// <returns>les erreurs si il y en a</returns>
        public async Task<string> RecupFichier()
        {
            var erreur = Validate();

            if (string.IsNullOrWhiteSpace(erreur))
            {
                try
                {
                    //lecture
                    var buffer = (await FileIO.ReadBufferAsync(Fichier)).ToArray();

                    //déchiffrement
                    var data = CryptUtils.AesDecryptByteArrayToString(buffer, CryptKey, CryptSalt);

                    //deserialization
                    var xsb = new XmlSerializer(typeof(ObservableCollection<MotDePasse>));
                    var rd = new StringReader(data);
                    ListeMdpRecup = xsb.Deserialize(rd) as ObservableCollection<MotDePasse>;
                }
                catch
                {
                    erreur += ResourceLoader.GetForCurrentView("Errors").GetString("erreurDechiffrementb");
                }
            }
            return erreur;
        }

        /// <summary>
        /// Récupère les mots de passe de texte
        /// </summary>
        /// <returns>les erreurs si il y en a</returns>
        public string RecupTexte()
        {
            var erreur = Validate();

            if (string.IsNullOrWhiteSpace(erreur))
            {
                try
                {
                    //déchiffrement
                    var data = CryptUtils.AesDecryptStringToString(Texte, CryptKey, CryptSalt);

                    //deserialization
                    var xsb = new XmlSerializer(typeof (ObservableCollection<MotDePasse>));
                    var rd = new StringReader(data);
                    ListeMdpRecup = xsb.Deserialize(rd) as ObservableCollection<MotDePasse>;
                }
                catch
                {
                    erreur += ResourceLoader.GetForCurrentView("Errors").GetString("erreurDechiffrementb");
                }
                
            }

            return erreur;
        }

        #endregion
    }
}
