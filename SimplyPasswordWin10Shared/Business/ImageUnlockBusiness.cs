using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.Storage;
using SimplyPasswordWin10Shared.Com;
using SimplyPasswordWin10Shared.Context;
using SimplyPasswordWin10Shared.Enum;
using SimplyPasswordWin10Shared.Model;
using SimplyPasswordWin10Shared.Utils;

namespace SimplyPasswordWin10Shared.Business
{
    /// <summary>
    /// Classe de gestion du fichier lié au déblocage par image
    /// </summary>
    public static class ImageUnlockBusiness
    {
        /// <summary>
        /// le fichier contenant les informations
        /// </summary>
        private static ComFile Fichier { get; set; }

        private static readonly string password = "Suite de caractères";

        private static readonly string salt = "Suite de caractères";

        /// <summary>
        /// Initialise le fichier de récupération
        /// </summary>
        public static void Init()
        {
            Fichier = new ComFile(ContexteStatic.NomFichierImageUnlock, PlaceEcritureEnum.LocalState);
        }

        /// <summary>
        /// Sauvegarde le fichier de récupération de mot de passe
        /// </summary>
        /// <param name="image">l'image et sauvegarder</param>
        /// <param name="listePoint">la liste des points et sauvegarder</param>
        public static async Task Save(byte[] image, List<Point> listePoint)
        {
            var element = new ImageUnlock
            {
                Image = image,
                ListePoint = listePoint
            };
            var xs = new XmlSerializer(typeof(ImageUnlock));
            var wr = new StringWriter();
            xs.Serialize(wr, element);
            var dataToSave = CryptUtils.AesEncryptStringToByteArray(wr.ToString(), password, salt);
            await Fichier.Ecrire(dataToSave, CreationCollisionOption.ReplaceExisting);
        }

        /// <summary>
        /// Efface le fichier
        /// </summary>
        public static async Task DeleteFile()
        {
            if (await Fichier.FileExist())
            {
                await Fichier.DeleteFile();
            }
        }

        /// <summary>
        /// Recharge les éléments pour la récupération de mot de passe
        /// </summary>
        /// <returns>les éléments</returns>
        public static async Task<ImageUnlock> Load()
        {
            try
            {
                var inFile = await Fichier.LireByteArray();
                var xmlIn = CryptUtils.AesDecryptByteArrayToString(inFile, password, salt);
                var xsb = new XmlSerializer(typeof(ImageUnlock));
                var rd = new StringReader(xmlIn);
                return xsb.Deserialize(rd) as ImageUnlock;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Vérifie si le fichier existe
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> FileExist()
        {
            return await Fichier.FileExist();
        }
    }
}
