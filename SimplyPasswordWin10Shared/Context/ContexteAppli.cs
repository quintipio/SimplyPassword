using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using SimplyPasswordWin10Shared.Business;
using SimplyPasswordWin10Shared.Model;
using SimplyPasswordWin10Shared.Utils;

namespace SimplyPasswordWin10Shared.Context
{
    /// <summary>
    /// Classe contenant les informatiosn et méthodes nécéssaires dans toute l'appli
    /// </summary>
    public static class ContexteAppli
    {
        /// <summary>
        /// le dossier contenant tout les dossiers et mot de passe (objet sauvegarder lors du save)
        /// </summary>
        public static Dossier DossierMere { get; set; }
        
        /// <summary>
        /// liste des icones
        /// </summary>
        public static List<Image> ListeIcone { get; set; }

        /// <summary>
        /// l'id max des icones de l'appli
        /// </summary>
        public static int NbMaxIconeAppli { get; private set; }

        /// <summary>
        /// l'index de la couleur à utilisé pour le theme
        /// </summary>
        public static int IdCouleurTheme { get; set; }

        /// <summary>
        /// Indique si cortana peut accéder aux mots de passes
        /// </summary>
        public static bool IsCortanaActive { get; set; }

        /// <summary>
        /// Indique si c'est le fichier placé dans roaming ou fichier extérieur qui est ouvert
        /// </summary>
        public static bool IsFichierRoamingOuvert { get; set; }


        /// <summary>
        /// A lancer en premier dans le démarrage de l'application
        /// </summary>
        /// <param name="full">true si l'application est lancé en mode complet ou mode arrière plan</param>
        /// <param name="fichier">le fichier à ouvrir si différent de celui présent dans roaming</param>
        /// <returns></returns>
        public static async Task Initialize(bool full,StorageFile fichier)
        {
            if (full)
            {
                ChargerIcones();
                await IconeBusiness.Load();
                IconeBusiness.AddIconToIconAppli();
                DossierMere = new Dossier { Titre = ResourceLoader.GetForCurrentView().GetString("phraseRacine") };
                ImageUnlockBusiness.Init();
            }
            else
            {
                DossierMere = new Dossier { Titre = "Root"};
            }

            if (fichier != null)
            {
                if (StringUtils.GetExtension(fichier.Name) != ContexteStatic.Extension)
                {
                    fichier = null;
                }
            }
            IsFichierRoamingOuvert = fichier == null;
            PasswordBusiness.Init(fichier);
            ParamBusiness.Init();
            await ParamBusiness.Load(full);
            CortanaBusiness.Init();
        }

        /// <summary>
        /// Charge les icones fournis par l'application
        /// </summary>
        private static void ChargerIcones()
        {
            //chargement des icones de l'appli
            ListeIcone = new List<Image>
            {
                new Image {Tag = 0, Height = 40, Width = 40},
                new Image
                {
                    Source = new BitmapImage(new Uri(@"ms-appx:/rsc/icon/icon_globe40.png")),
                    Tag = 1,
                    Height = 40,
                    Width = 40
                },
                new Image
                {
                    Source = new BitmapImage(new Uri(@"ms-appx:/rsc/icon/icon_archive40.png")),
                    Tag = 2,
                    Height = 40,
                    Width = 40
                },
                new Image
                {
                    Source = new BitmapImage(new Uri(@"ms-appx:/rsc/icon/icon_cd40.png")),
                    Tag = 3,
                    Height = 40,
                    Width = 40
                },
                new Image
                {
                    Source = new BitmapImage(new Uri(@"ms-appx:/rsc/icon/icon_db40.png")),
                    Tag = 4,
                    Height = 40,
                    Width = 40
                },
                new Image
                {
                    Source = new BitmapImage(new Uri(@"ms-appx:/rsc/icon/icon_dollar40.png")),
                    Tag = 5,
                    Height = 40,
                    Width = 40
                },
                new Image
                {
                    Source = new BitmapImage(new Uri(@"ms-appx:/rsc/icon/icon_engrenage40.png")),
                    Tag = 6,
                    Height = 40,
                    Width = 40
                },
                new Image
                {
                    Source = new BitmapImage(new Uri(@"ms-appx:/rsc/icon/icon_euro40.png")),
                    Tag = 7,
                    Height = 40,
                    Width = 40
                },
                new Image
                {
                    Source = new BitmapImage(new Uri(@"ms-appx:/rsc/icon/icon_folder40.png")),
                    Tag = 8,
                    Height = 40,
                    Width = 40
                },
                new Image
                {
                    Source = new BitmapImage(new Uri(@"ms-appx:/rsc/icon/icon_house40.png")),
                    Tag = 9,
                    Height = 40,
                    Width = 40
                },
                new Image
                {
                    Source = new BitmapImage(new Uri(@"ms-appx:/rsc/icon/icon_mail40.png")),
                    Tag = 10,
                    Height = 40,
                    Width = 40
                },
                new Image
                {
                    Source = new BitmapImage(new Uri(@"ms-appx:/rsc/icon/icon_pc40.png")),
                    Tag = 11,
                    Height = 40,
                    Width = 40
                },
                new Image
                {
                    Source = new BitmapImage(new Uri(@"ms-appx:/rsc/icon/icon_people40.png")),
                    Tag = 12,
                    Height = 40,
                    Width = 40
                },
                new Image
                {
                    Source = new BitmapImage(new Uri(@"ms-appx:/rsc/icon/icon_phone40.png")),
                    Tag = 13,
                    Height = 40,
                    Width = 40
                },
                new Image
                {
                    Source = new BitmapImage(new Uri(@"ms-appx:/rsc/icon/icon_server40.png")),
                    Tag = 14,
                    Height = 40,
                    Width = 40
                },
                new Image
                {
                    Source = new BitmapImage(new Uri(@"ms-appx:/rsc/icon/icon_shell40.png")),
                    Tag = 15,
                    Height = 40,
                    Width = 40
                },
                new Image
                {
                    Source = new BitmapImage(new Uri(@"ms-appx:/rsc/icon/icon_software40.png")),
                    Tag = 16,
                    Height = 40,
                    Width = 40
                },
                new Image
                {
                    Source = new BitmapImage(new Uri(@"ms-appx:/rsc/icon/icon_wifi40.png")),
                    Tag = 17,
                    Height = 40,
                    Width = 40
                }
            };

            NbMaxIconeAppli = ListeIcone.Count - 1;
        }

        /// <summary>
        /// Efface le fichier de mot de passe
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> ReinitAppli()
        {
            //efface le fichier des mots de passes
            if (await PasswordBusiness.Delete() && await ParamBusiness.Delete() && await CortanaBusiness.DeletePassword())
            {
                await IconeBusiness.DeletelAll();
                await ImageUnlockBusiness.DeleteFile();
                await CortanaBusiness.UpdateCortana();
                await Initialize(true,null);
                return true;
            }
            return false;
        }

        

        /// <summary>
        /// Permet de renvoyer les couleurs du thème
        /// </summary>
        /// <returns>la ouleur à afficher</returns>
        public static SolidColorBrush GetColorTheme()
        {
            //var colorB = ContexteStatic.ListeCouleur[IdCouleurTheme] - 4290109673;
            //var colorC = colorB - 4844020;

           var  hex = string.Format("{0:X}", ContexteStatic.ListeCouleur[IdCouleurTheme]);
            var cColor = Color.FromArgb(byte.Parse(hex.Substring(0, 2), NumberStyles.AllowHexSpecifier),
                byte.Parse(hex.Substring(2, 2), NumberStyles.AllowHexSpecifier),
                byte.Parse(hex.Substring(4, 2), NumberStyles.AllowHexSpecifier),
                byte.Parse(hex.Substring(6, 2), NumberStyles.AllowHexSpecifier));

            return new SolidColorBrush(cColor);
        }
        
    }
}
