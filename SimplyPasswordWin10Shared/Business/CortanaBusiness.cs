using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Storage;
using SimplyPasswordWin10Shared.Com;
using SimplyPasswordWin10Shared.Context;
using SimplyPasswordWin10Shared.Enum;
using SimplyPasswordWin10Shared.Model;
using SimplyPasswordWin10Shared.Strings;
using SimplyPasswordWin10Shared.Utils;

namespace SimplyPasswordWin10Shared.Business
{
    /// <summary>
    /// Classe de gestion métier pour Cortana
    /// </summary>
    public static class CortanaBusiness
    {
        /// <summary>
        /// _fichier contenant le mot de passe de l'utilisateur
        /// </summary>
        private static ComFile _fichier;

        //les mots de passe pour le chiffrement du mot de passe utilisateur
        private static readonly string PrefixeSurchiffrement = "Suite de caractères";
        private static readonly string SuffixeSurchiffrement = "Suite de caractères";
        private static readonly string MotDePasseASurchiffrement = "Suite de caractères";
        private static readonly string MotDePasseBSurchiffrement = "Suite de caractères";
        private static readonly string SaltASurchiffrement = "Suite de caractères";
        private static readonly string SaltBSurchiffrement = "Suite de caractères";


        /// <summary>
        /// Constructeur
        /// </summary>
        public static void Init()
        {
            _fichier = new ComFile(ContexteStatic.NomFichierMotDePasseUser, PlaceEcritureEnum.LocalState);
        }


        #region Installation 
        /// <summary>
        /// Installe les commandes vocales de cortana ou met à jour le la liste des sites pour la recherche de mots de passe
        /// </summary>
        /// <returns>true si ça à bien focntionné</returns>
        public static async Task<bool> UpdateCortana()
        {
            if (ContexteAppli.IsFichierRoamingOuvert)
            {
                try
                {
                    //récupère tout les site dispo
                    if (ContexteAppli.DossierMere != null)
                    {
                        var liste = ChercherIdentifiants(ContexteAppli.DossierMere);

                        //installe les fichiers
                        var vcdfile = await Package.Current.InstalledLocation.GetFileAsync(@"VoiceCommands.xml");
                        await VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(vcdfile);

                        //installe les titres d'identifiants au sein de cortana
                        VoiceCommandDefinition command;

                        

                        if (VoiceCommandDefinitionManager.InstalledCommandDefinitions.TryGetValue(
                            "SimplyPassword_CommandSet_"+ListeLangues.GetLangueEnCours().Diminutif, out command))
                        {
                            await command.SetPhraseListAsync("site", liste);
                        }
                        
                        return true;
                    }
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }

        private static List<string> ChercherIdentifiants(Dossier dossier)
        {
            var retour = new List<string>();

            //recherche du mot de passe
            foreach (var m in dossier.ListeMotDePasse)
            {
                retour.Add(m.Titre);
            }

            //recherche dans les sous dossiers
            foreach (var d in dossier.SousDossier)
            {
                retour.AddRange(ChercherIdentifiants(d));
            }
            return retour;
        }
        #endregion

        #region gestion du mot de passe utilisateur
        /// <summary>
        /// Surchiffre le mot de passe utilisateur
        /// </summary>
        /// <param name="chaine">le mot de passe de l'utilisateur</param>
        /// <returns>le mot de passe surchiffré</returns>
        private static string ChiffrerMotDePasse(string chaine)
        {
            var intermed = CryptUtils.AesEncryptStringToString(chaine, MotDePasseASurchiffrement, SaltASurchiffrement);
            intermed = SuffixeSurchiffrement + intermed + PrefixeSurchiffrement;
            return CryptUtils.AesEncryptStringToString(intermed, MotDePasseBSurchiffrement, SaltBSurchiffrement);
        }

        /// <summary>
        /// Déchiffre un mot de passe surchiffré
        /// </summary>
        /// <param name="chaine">la chaine à déchiffrer</param>
        /// <returns>la chaine en clair</returns>
        public static string DechiffrementStringSurchiffre(string chaine)
        {
            var intermed = CryptUtils.AesDecryptStringToString(chaine, MotDePasseBSurchiffrement, SaltBSurchiffrement);
            intermed = intermed.Replace(SuffixeSurchiffrement, "");
            intermed = intermed.Replace(PrefixeSurchiffrement, "");
            return CryptUtils.AesDecryptStringToString(intermed, MotDePasseASurchiffrement, SaltASurchiffrement);
        }

        /// <summary>
        ///  Vérifie si le mot de passe enregistré correspond à celui entré et si il ne correspond pas, il refait le mot de passe
        /// </summary>
        /// <returns></returns>
        public static async Task CheckPassword()
        {
            var mdpEnregistre = await GetPasswordUser();
            if (PasswordBusiness.Password != mdpEnregistre)
            {
                await SavePassword();
            }
        }

        /// <summary>
        /// Sauvegarde le mot de passe de l'utilisateur dans 
        /// </summary>
        /// <returns>la task</returns>
        public static async Task SavePassword()
        {
            var chaine = ChiffrerMotDePasse(PasswordBusiness.Password);
            await _fichier.Ecrire(chaine, CreationCollisionOption.ReplaceExisting, true);

        }

        /// <summary>
        /// Retourne le mot de passe de l'utilisateur
        /// </summary>
        /// <returns>le mot de passe</returns>
        public static async Task<string> GetPasswordUser()
        {
            if (await _fichier.FileExist())
            {
                //récupération du mot de passe de l'utilisateur
                var mdpHash = await _fichier.LireString();
                return DechiffrementStringSurchiffre(mdpHash);
            }
            return null;
        }

        /// <summary>
        /// Efface le fichier de mot de passe
        /// </summary>
        /// <returns>la talk</returns>
        public static async Task<bool> DeletePassword()
        {
            if (await _fichier.FileExist())
            {
                await _fichier.DeleteFile();
            }
            return true;
        }
        #endregion

        

        /// <summary>
        /// Recherche une liste de mot de passe dans un dossier et ses sous dossiers à partir d'une chaine de caractère
        /// </summary>
        /// <param name="identifiant">l'identifiant recherché</param>
        /// <param name="dossier">le dossier à explorer</param>
        /// <returns>une liste de tuile à afficher par cortana</returns>
        public static List<VoiceCommandContentTile> GetMotDePasseTile(string identifiant,Dossier dossier)
        {
            var listeMdp = GetMotDePasse(identifiant, dossier);

            var destinationsContentTiles = new List<VoiceCommandContentTile>();

            foreach (var motDePass in listeMdp)
            {
                var destinationTile = new VoiceCommandContentTile
                {
                    ContentTileType = VoiceCommandContentTileType.TitleWithText,
                    Title = motDePass.Titre,
                    TextLine1 = motDePass.Login,
                    TextLine2 = motDePass.MotDePasseObjet
                };
                destinationsContentTiles.Add(destinationTile);
            }

            return destinationsContentTiles;
        }


        /// <summary>
        /// Recherche une liste de mot de passe dans un dossier et ses sous dossiers à partir d'une chaine de caractère
        /// </summary>
        /// <param name="identifiant">l'identifiant recherché</param>
        /// <param name="dossier">le dossier à explorer</param>
        /// <returns>une liste de tuile à afficher par cortana</returns>
        public static List<MotDePasse> GetMotDePasse(string identifiant, Dossier dossier)
        {
            return ChercherMotDePasse(dossier, identifiant);
        }


        /// <summary>
        /// Recherche un mot de passe dans les dossiers et sous dossier
        /// </summary>
        /// <param name="dossier">le dossier à explorer</param>
        /// <param name="texteRechercher">l'identifiant à rechercher</param>
        /// <returns>la liste des mots de passes trouvés</returns>
        private static List<MotDePasse> ChercherMotDePasse(Dossier dossier, string texteRechercher)
        {
            var retour = new List<MotDePasse>();

            //recherche du mot de passe
            foreach (var m in dossier.ListeMotDePasse)
            {
                if (m.Titre.ToLower().Contains(texteRechercher.ToLower()))
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
    }
}
