using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;
using SimplyPasswordWin10Shared.Com;
using SimplyPasswordWin10Shared.Context;
using SimplyPasswordWin10Shared.Enum;
using SimplyPasswordWin10Shared.Model;
using SimplyPasswordWin10Shared.Strings;

namespace SimplyPasswordWin10Shared.Business
{
    /// <summary>
    /// Classe pour gérer le fichier de paramètre
    /// </summary>
    public static class ParamBusiness
    {

        /// <summary>
        /// Fichier de sauvegarde des paramètres
        /// </summary>
        private static ComFile _fichier;


        /// <summary>
        /// Initialise le fichier
        /// </summary>
        public static void Init()
        {
            _fichier = new ComFile(ContexteStatic.NomFichierParam, PlaceEcritureEnum.LocalState);
        }

        /// <summary>
        /// va récupérer toute les informations des paramètres de l'appli à sauvegarder, puis sauvegarde dans un fichier
        /// </summary>
        public static async Task Save()
        {
            //serialization
            var param = new Param(ListeLangues.GetLangueEnCours().Id, ContexteAppli.IdCouleurTheme,ContexteAppli.IsCortanaActive);
            var xs = new XmlSerializer(typeof(Param));
            var wr = new StringWriter();
            xs.Serialize(wr, param);
            //ecriture
            await _fichier.Ecrire(wr.ToString(), CreationCollisionOption.ReplaceExisting, true);
        }

        /// <summary>
        /// charge le fichier de paramètre si il existe
        /// </summary>
        public static async Task Load(bool fullLoad)
        {
            //lecture
            if (_fichier != null && await _fichier.FileExist())
            {
                var inFile = await _fichier.LireString();

                var xsb = new XmlSerializer(typeof (Param));
                var rd = new StringReader(inFile);
                var param = xsb.Deserialize(rd) as Param;
                if (param != null)
                {
                    if (fullLoad)
                    {
                        ContexteAppli.IdCouleurTheme = param.EmplacementListeCouleur < ContexteStatic.ListeCouleur.Count
                            ? param.EmplacementListeCouleur
                            : ContexteStatic.ListeCouleur.IndexOf(0xFF00613F);
                        ListeLangues.ChangeLangueAppli(param.IdLangue);
                    }
                    ContexteAppli.IsCortanaActive = param.IsCortanaActive;
                }
                else
                {
                    ContexteAppli.IdCouleurTheme = ContexteStatic.ListeCouleur.IndexOf(0xFF00613F);
                    ContexteAppli.IsCortanaActive = false;
                }
            }
            else
            {
                ContexteAppli.IdCouleurTheme = ContexteStatic.ListeCouleur.IndexOf(0xFF00613F);
                ContexteAppli.IsCortanaActive = false;
            }
        }
        

        /// <summary>
        /// efface le fichier de paramètre
        /// </summary>
        /// <returns>true si ok</returns>
        public static async Task<bool> Delete()
        {
            return await _fichier.DeleteFile();
        }
        
    }
}
