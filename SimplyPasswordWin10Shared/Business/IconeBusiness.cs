using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using SimplyPasswordWin10Shared.Context;
using SimplyPasswordWin10Shared.Model;
using SimplyPasswordWin10Shared.Utils;

namespace SimplyPasswordWin10Shared.Business
{
    /// <summary>
    /// Classe de gestion des icones
    /// </summary>
    public static class IconeBusiness
    {

        public const double MaxSizeX = 40.0;
        public const double MaxSizeY = 40.0;

        /// <summary>
        /// Les icones présentes
        /// </summary>
        public static Dictionary<int,BitmapImage> ListeIcone { get; set; }



        #region méthodes internes
        private static async Task CreateFolderIfNotExist()
        {
            if (!await IsFolderIconePresent())
            {
                await ApplicationData.Current.LocalFolder.CreateFolderAsync(ContexteStatic.NomDossierIcone);
            }
        }


        /// <summary>
        /// Vérifie si le dossier d'icone est présent
        /// </summary>
        /// <returns>true si il existe</returns>
        private static async Task<bool> IsFolderIconePresent()
        {
            var tot = await ApplicationData.Current.LocalFolder.GetFilesAsync();
            var tat = await ApplicationData.Current.LocalFolder.GetFoldersAsync();
            var localFolder = await ApplicationData.Current.LocalFolder.TryGetItemAsync(ContexteStatic.NomDossierIcone);
            return localFolder != null;
        }

        /// <summary>
        /// Retourne un nouvel id
        /// </summary>
        /// <returns>l'id</returns>
        private static async Task<int> GetNewId()
        {
            var listeId = new List<int>();
            if (await IsFolderIconePresent())
            {
                var listeFichier = await (await ApplicationData.Current.LocalFolder.GetFolderAsync(ContexteStatic.NomDossierIcone)).GetFilesAsync();

                if (listeFichier.Count == 0)
                {
                    return 1;
                }
                else
                {
                    foreach (var file in listeFichier)
                    {
                        int id;
                        if (int.TryParse(file.Name, out id))
                        {
                            listeId.Add(id);
                        }
                    }

                    return listeId.OrderByDescending(x => x).FirstOrDefault() + 1;
                }
            }
            return 1;
        }

        /// <summary>
        /// Redimmensionne une image
        /// </summary>
        /// <param name="sourceImage">l'image à modifier</param>
        /// <returns>l'image redimensionnée</returns>
        private static BitmapImage ResizedImage(BitmapImage sourceImage)
        {
            var newImg = sourceImage;
            var origHeight = sourceImage.PixelHeight;
            var origWidth = sourceImage.PixelWidth;
            var ratioX = MaxSizeX / (float)origWidth;
            var ratioY = MaxSizeY / (float)origHeight;
            var ratio = Math.Min(ratioX, ratioY);
            var newHeight = (int)(origHeight * ratio);
            var newWidth = (int)(origWidth * ratio);

            newImg.DecodePixelWidth = newWidth;
            newImg.DecodePixelHeight = newHeight;

            return newImg;
        }

        /// <summary>
        /// Supprime une icone d'un répertoire et de ses sous répertories
        /// </summary>
        /// <param name="dossier">le dossier où supprimer les mots de passes</param>
        /// <param name="idIcone">l'id de l'icone à supprimer</param>
        private static void RemoveIconeFromPassword(Dossier dossier, int idIcone)
        {
            foreach (var mdp in dossier.ListeMotDePasse)
            {
                if (mdp.IdIcone == idIcone)
                {
                    mdp.Icone = null;
                    mdp.IdIcone = 0;
                }
            }

            foreach (var dos in dossier.SousDossier)
            {
                RemoveIconeFromPassword(dos, idIcone);
            }
        }
        #endregion



        /// <summary>
        /// Ajoute une image et sauvegarde la liste
        /// </summary>
        /// <param name="fichier">le fichier à enregsitrer</param>
        /// <returns></returns>
        public static async Task AddImage(StorageFile fichier)
        {
            //enregistrement du fichier
            var imageToSave = await ObjectUtils.ConvertFileToBytes(fichier);
            var nomImageToSave = await GetNewId();
            
            await CreateFolderIfNotExist();

            var folder = await ApplicationData.Current.LocalFolder.GetFolderAsync(ContexteStatic.NomDossierIcone);
            var file = await folder.CreateFileAsync(nomImageToSave.ToString());
            await FileIO.WriteBytesAsync(file, imageToSave);

            //redimensionnement et ajout dans la liste
            if (ListeIcone == null)
            {
                ListeIcone = new Dictionary<int, BitmapImage>();
            }
            var bitmapImage = new BitmapImage();
            var stream = (FileRandomAccessStream)await fichier.OpenAsync(FileAccessMode.Read);
            bitmapImage.SetSource(stream);
            var imageResize = ResizedImage(bitmapImage);
            ListeIcone.Add(nomImageToSave, imageResize);
            ContexteAppli.ListeIcone.Add(new Image
            {
                Source = imageResize,
                Tag = nomImageToSave * -1,
                Height = bitmapImage.DecodePixelHeight,
                Width = bitmapImage.DecodePixelWidth
            });
        }

        /// <summary>
        /// Supprime une image de la liste et la sauvegarde
        /// </summary>
        /// <param name="index">le numero de l'image à supprimer</param>
        public static async Task RemoveImage(int index)
        {
            //suppression du fichier
            var foldericons = await ApplicationData.Current.LocalFolder.GetFolderAsync(ContexteStatic.NomDossierIcone);
            var file = await foldericons.GetFileAsync(index.ToString());
            await file.DeleteAsync();

            //supression de la liste
            ListeIcone.Remove(index);

            //supression des icones dans les mots de passes
            RemoveIconeFromPassword(ContexteAppli.DossierMere,index*-1);

            //supression des icones en mémoire
            var el = ContexteAppli.ListeIcone.First(x => (int) x.Tag == index*-1);
            if (el != null)
            {
                ContexteAppli.ListeIcone.Remove(el);
            }
        }


        /// <summary>
        /// Charge les icones enregistrées
        /// </summary>
        public static async Task Load()
        {
            if (await IsFolderIconePresent())
            {
                var foldericons = await ApplicationData.Current.LocalFolder.GetFolderAsync(ContexteStatic.NomDossierIcone);
                var fichiers = await foldericons.GetFilesAsync();

                ListeIcone = new Dictionary<int, BitmapImage>();

                foreach (var fichier in fichiers)
                {
                    //récupération seulement avec des noms corrects
                    int id;
                    if (int.TryParse(fichier.Name, out id))
                    {
                        var image = new BitmapImage();
                        var stream = (FileRandomAccessStream)await fichier.OpenAsync(FileAccessMode.Read);
                        image.SetSource(stream);
                        ListeIcone.Add(id, ResizedImage(image));
                    }
                }
            }
        }

        

        /// <summary>
        /// Ajoute les icones personalisées aux icones de l'appli
        /// </summary>
        public static void AddIconToIconAppli()
        {
            if (ListeIcone == null)
            {
                ListeIcone = new Dictionary<int, BitmapImage>();
            }

            foreach (var image in ListeIcone)
            {
                var img = new Image
                {
                    Source = image.Value,
                    Tag = image.Key *-1,
                    Height = image.Value.DecodePixelHeight,
                    Width = image.Value.DecodePixelWidth
                };
                ContexteAppli.ListeIcone.Add(img);
            }
        }

        /// <summary>
        /// Réinitialise toute les icones
        /// </summary>
        public static async Task DeletelAll()
        {
            if (await IsFolderIconePresent())
            {
                var localFolder = await ApplicationData.Current.LocalFolder.TryGetItemAsync(ContexteStatic.NomDossierIcone);
                await localFolder.DeleteAsync();
                ListeIcone = null;
            }
        }

        
    }
}
