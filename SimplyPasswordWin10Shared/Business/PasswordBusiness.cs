using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using SimplyPasswordWin10Shared.Com;
using SimplyPasswordWin10Shared.Context;
using SimplyPasswordWin10Shared.Enum;
using SimplyPasswordWin10Shared.Model;
using SimplyPasswordWin10Shared.Utils;

namespace SimplyPasswordWin10Shared.Business
{
    /// <summary>
    /// Classe pour gérer le fichier de mot de passe
    /// </summary>
    public static class PasswordBusiness
    {

        /// <summary>
        /// le fichier contenant les informations
        /// </summary>
        private static ComFile Fichier { get; set; }

        public static string Password { get; set; }


        /// <summary>
        /// Initialise le fichier des mots de passe
        /// </summary>
        public static void Init(StorageFile fichier)
        {
            if (fichier != null)
            {
                Fichier = new ComFile(fichier);
            }
            else
            {
                Fichier = new ComFile(ContexteStatic.NomFichierPassword + "." + ContexteStatic.Extension, PlaceEcritureEnum.Roaming);
            }
        }

        /// <summary>
        /// Serialize les mots de passe, chiffre en AES à partir du mot de passe dans ce contexte et écrit dans un fichier en roaming
        /// </summary>
        public static async Task Save()
        {
            //serialization
            var xs = new XmlSerializer(typeof(Dossier));
            var wr = new StringWriter();
            xs.Serialize(wr, ContexteAppli.DossierMere);
            //chiffrement
            var dataToSave = CryptUtils.AesEncryptStringToByteArray(wr.ToString(), Password, Password);
            //écriture
            await Fichier.Ecrire(dataToSave, CreationCollisionOption.ReplaceExisting);

        }

        /// <summary>
        /// lit le fichier , le déchiffre à partir du mot de passe fournit et le désérialize
        /// </summary>
        /// <param name="passwordTmp">le mot de passe pour tenter le déchiffrement</param>
        /// <param name="fullLoad">Indique si c'est un chargement complet ou légé</param>
        /// <returns>true si ok</returns>
        public static async Task<bool> Load(string passwordTmp,bool fullLoad)
        {
            try
            {
                //lecture
                var inFile = await Fichier.LireByteArray();

                //dechiffrement
                var xmlIn = CryptUtils.AesDecryptByteArrayToString(inFile, passwordTmp, passwordTmp);

                //deserialize
                var xsb = new XmlSerializer(typeof(Dossier));
                var rd = new StringReader(xmlIn);
                ContexteAppli.DossierMere = xsb.Deserialize(rd) as Dossier;
                if (fullLoad)
                {
                    RemiseEnPlaceParent(ContexteAppli.DossierMere);
                    RemiseEnPlaceIcone(ContexteAppli.DossierMere);
                }
                Password = passwordTmp;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Méthode récursive permettant de placer dans les objets correspondant les objets possesseur de leurs enfants
        /// les dossier parents de sous dossiers et les dossier possesseur des mots de passe)
        /// </summary>
        /// <param name="ori">le dossier à scanner</param>
        private static void RemiseEnPlaceParent(Dossier ori)
        {
            foreach (var m in ori.ListeMotDePasse)
            {
                m.DossierPossesseur = ori;
            }

            foreach (var d in ori.SousDossier)
            {
                d.DossierParent = ori;
                RemiseEnPlaceParent(d);
            }
        }

        /// <summary>
        /// Place les icones dans chaque mot de passes
        /// </summary>
        /// <param name="ori">le dossier à scanner (récursif)</param>
        private static void RemiseEnPlaceIcone(Dossier ori)
        {
            foreach (var m in ori.ListeMotDePasse)
            {
               var res =  ContexteAppli.ListeIcone.Where(x => (int) x.Tag == m.IdIcone).ToList();
                if (res.Count > 0)
                {
                    m.Icone = (BitmapImage)res[0].Source;
                }
                else
                {
                    m.Icone = null;
                }
            }

            foreach (var d in ori.SousDossier)
            {
                RemiseEnPlaceIcone(d);
            }
        }

        /// <summary>
        /// efface le fichier
        /// </summary>
        /// <returns>true si ok</returns>
        public static async Task<bool> Delete()
        {
            return await Fichier.DeleteFile();
        }

        /// <summary>
        /// Vérifie si le fichier chiffrer existe déjà ou non
        /// </summary>
        /// <returns>true si déj existant</returns>
        public static async Task<bool> DoesFileCypherExist()
        {
            return await Fichier.FileExist();
        }

        /// <summary>
        /// retourne la dernière date de modification du fichier chiffré
        /// </summary>
        /// <returns>la dernière date de modification </returns>
        public static async Task<DateTimeOffset> GetDerniereModifFile()
        {
            return await Fichier.GetDateModified();
        }

        /// <summary>
        /// Retourne en % l'espace occupé par le fichier dans le dossier de roaming
        /// </summary>
        /// <returns></returns>
        public static async Task<int> GetEspaceFichierOccupePourcent()
        {
            var espaceMax = (ApplicationData.Current.RoamingStorageQuota > 0)
                ? ApplicationData.Current.RoamingStorageQuota * 1000 : ContexteStatic.RoaminsStorageQuotaBis * 1000;
            var espaceOccupe = await Fichier.GetSizeFile();
            var ret = (100 * espaceOccupe) / espaceMax;
            return (espaceOccupe > 0 && ret == 0) ? 1 : (int)ret;
        }

        /// <summary>
        /// Retourne le nom du fichier utilisé
        /// </summary>
        /// <returns>le nom</returns>
        public static async Task<string> GetNameFile()
        {
            return await Fichier.GetNameFile();
        }
    }
}
