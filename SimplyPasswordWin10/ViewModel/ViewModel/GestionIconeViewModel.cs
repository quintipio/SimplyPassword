using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using SimplyPasswordWin10.Abstract;
using SimplyPasswordWin10Shared.Business;

namespace SimplyPasswordWin10.ViewModel
{
    /// <summary>
    /// Le controleur de la gestion des icones
    /// </summary>
    public partial class GestionIconeViewModel : AbstractViewModel
    {
        /// <summary>
        /// Constructeur
        /// </summary>
        public GestionIconeViewModel()
        {
            LoadIcons();
        }


        /// <summary>
        /// Vérifie la validité d'une image chargé par un utilisateur
        /// </summary>
        /// <param name="image">l'image à vérifier</param>
        /// <returns>les erreurs</returns>
        private async Task<string> ValidateImage(StorageFile image)
        {
            var retour = "";
            if (image == null)
            {
                retour += ResourceLoader.GetForCurrentView("Errors").GetString("erreurAucuneImage") + "\r\n";
            }
            else
            {
                if ((await image.Properties.GetImagePropertiesAsync()).Width < IconeBusiness.MaxSizeX ||
                    (await image.Properties.GetImagePropertiesAsync()).Height < IconeBusiness.MaxSizeY)
                {
                    retour += ResourceLoader.GetForCurrentView("Errors").GetString("erreurSizeImage") + " " + IconeBusiness.MaxSizeX +
                              "x" + IconeBusiness.MaxSizeY + "\r\n";
                }

                if ((await image.GetBasicPropertiesAsync()).Size > 2048000)
                {
                    retour += ResourceLoader.GetForCurrentView("Errors").GetString("erreurPoidImage") + " 2Mb\r\n";
                }
            }

            return retour;
        }

        /// <summary>
        /// Ajoute une icone dans le dossier local
        /// </summary>
        /// <param name="fichier">le fichier de l'image à ajouter</param>
        public async Task<string> AjouterIcone(StorageFile fichier)
        {
            var erreur = await ValidateImage(fichier);
            if (string.IsNullOrWhiteSpace(erreur))
            {
                //Ajout à la liste des icone et sauvegarde du fichier
                await IconeBusiness.AddImage(fichier);
                LoadIcons();
            }
            return erreur;
        }

        /// <summary>
        /// Supprime une icone
        /// </summary>
        /// <param name="index">l'id de l'icone à supprimer</param>
        /// <returns></returns>
        public async Task SupprimerIcone(int index)
        {
            await IconeBusiness.RemoveImage(index);
            LoadIcons();
        }

        /// <summary>
        /// recharge les icones de l'utilisateur
        /// </summary>
        private void LoadIcons()
        {
            ListeIconePerso =(IconeBusiness.ListeIcone != null)? new Dictionary<int,BitmapImage>(IconeBusiness.ListeIcone):new Dictionary<int, BitmapImage>();
        }
        
    }
}
