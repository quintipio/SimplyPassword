using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using SimplyPasswordWin10.Abstract;
using SimplyPasswordWin10Shared.Business;
using SimplyPasswordWin10Shared.Enum;
using SimplyPasswordWin10Shared.Utils;

namespace SimplyPasswordWin10.ViewModel
{
    /// <summary>
    /// Controleur pour la vue de déverrouillage de l'appli par image
    /// </summary>
    public partial class SecureImageViewModel : AbstractViewModel
    {
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="mode">le mode d'ouverture de la page</param>
        public SecureImageViewModel(OuvertureSecureImageEnum mode)
        {
            Mode = mode;
            ListePoint = new List<Point>();
            ListePointToVerif = new List<Point>();
            ImageUnlockBusiness.Init();
            CortanaBusiness.Init();
        }

        /// <summary>
        /// Méthode chargeant des paramètres les données déjà existantes
        /// </summary>
        public async Task<bool> LoadSecurity()
        {
            if (await ImageUnlockBusiness.FileExist())
            {
               var element = await ImageUnlockBusiness.Load();
                if (element != null)
                {
                    _imageToSave = element.Image;
                    ImageLock = ResizedImage(await ObjectUtils.ConvertBytesToBitmap(_imageToSave),(int)_maxSizeX, (int)_maxSizeY);
                    ListePoint = new List<Point>(element.ListePoint);
                    EtapeUnOk = true;
                    VerifPointImage();
                    VerifEtapeDeux();
                    return true;
                }
            }
            EtapeUnOk = false;
            VerifPointImage();
            EtapeDeuxOk = false;
            return false;
        }

        #region chargement fichier
        /// <summary>
        /// Vérifie la validité d'une image chargé par un utilisateur
        /// </summary>
        /// <param name="image">l'image à vérifier</param>
        /// <returns>les erreurs</returns>
        private async Task<string> ValidateImage(StorageFile image)
        {
            string retour = "";
            if (image == null)
            {
                retour += ResourceLoader.GetForCurrentView("Errors").GetString("erreurAucuneImage") + "\r\n";
            }
            else
            {
                if ((await image.Properties.GetImagePropertiesAsync()).Width < _maxSizeX || (await image.Properties.GetImagePropertiesAsync()).Height < _maxSizeY)
                {
                    retour += ResourceLoader.GetForCurrentView("Errors").GetString("erreurSizeImage") +" "+ _maxSizeX+"x"+_maxSizeY+"\r\n";
                }

                if ((await image.GetBasicPropertiesAsync()).Size > 2048000)
                {
                    retour += ResourceLoader.GetForCurrentView("Errors").GetString("erreurPoidImage") + " 2Mb\r\n";
                }
            }

            
            return retour;
        }
        

        /// <summary>
        /// Met en mémoire le fichier à afficher
        /// </summary>
        /// <param name="fichier">le fichier à charger</param>
        /// <returns></returns>
        public async Task SaveImage(StorageFile fichier)
        {
            var erreur = await ValidateImage(fichier);
            if (string.IsNullOrWhiteSpace(erreur))
            {
                //si ok conversion du fichier en image
                var bitmapImage = new BitmapImage();
                var stream = (FileRandomAccessStream)await fichier.OpenAsync(FileAccessMode.Read);
                bitmapImage.SetSource(stream);
                ImageLock = ResizedImage(bitmapImage, (int)_maxSizeX,(int)_maxSizeY);

                //puis mise en mémoire en binaire pour la sauvegarde
                _imageToSave = await ObjectUtils.ConvertFileToBytes(fichier);

                ErreurEtapeUn = "";
                EtapeUnOk = true;
                EffacerPoint();
            }
            ErreurEtapeUn = erreur;
        }


        public static BitmapImage ResizedImage(BitmapImage sourceImage, int maxWidth, int maxHeight)
        {
            var origHeight = sourceImage.PixelHeight;
            var origWidth = sourceImage.PixelWidth;
            var ratioX = maxWidth / (float)origWidth;
            var ratioY = maxHeight / (float)origHeight;
            var ratio = Math.Min(ratioX, ratioY);
            var newHeight = (int)(origHeight * ratio);
            var newWidth = (int)(origWidth * ratio);

            sourceImage.DecodePixelWidth = newWidth;
            sourceImage.DecodePixelHeight = newHeight;

            return sourceImage;
        }

        

        #endregion

        #region pointage Image

        /// <summary>
        /// Vérifie si l'étape deux peut être validé
        /// </summary>
        private void VerifPointImage()
        {
            NbPointsOk = ListePoint != null && ListePoint.Count >= 3 && ListePoint.Count <= 9;
        }

        /// <summary>
        /// Ajoute un point à la liste des points sur l'image
        /// </summary>
        /// <param name="point">le point à ajouter</param>
        public void AddPoint(Point point)
        {
            if (Mode == OuvertureSecureImageEnum.MODE_CHANGEMENT_IMAGE)
            {
                if (ListePoint == null)
                {
                    ListePoint = new List<Point>();
                }
                ListePoint.Add(point);
                VerifPointImage();
            }
            else
            {
                if (ListePointToVerif == null)
                {
                    ListePointToVerif = new List<Point>();
                }
                ListePointToVerif.Add(point);
            }
        }

        /// <summary>
        /// Efface les points enregistré
        /// </summary>
        public void EffacerPoint()
        {
            if (Mode == OuvertureSecureImageEnum.MODE_CHANGEMENT_IMAGE)
            {
                ListePoint?.Clear();
                VerifPointImage();
                EtapeDeuxOk = false;
            }
            else
            {
                ListePointToVerif?.Clear();
            }
        }

        /// <summary>
        /// Vérifie les coordonnées pour l'étape deux
        /// </summary>
        /// <returns></returns>
        private string ValidatePoint()
        {
            var retour = "";

            if (ListePoint == null)
            {
                retour += ResourceLoader.GetForCurrentView("Errors").GetString("erreurAucunPoint") +"\r\n";
            }
            else
            {
                if (ListePoint.Count < 3)
                {
                    retour += ResourceLoader.GetForCurrentView("Errors").GetString("erreurMinPoint") + "\r\n";
                }
                if (ListePoint.Count > 9)
                {
                    retour += ResourceLoader.GetForCurrentView("Errors").GetString("erreurMaxPoint") + "\r\n";
                }
                
            }
            return retour;
        }

        /// <summary>
        /// Vérifie la validité de l'étape deux
        /// </summary>
        public void VerifEtapeDeux()
        {
            var retour = ValidatePoint();
            if (string.IsNullOrWhiteSpace(retour))
            {
                ErreurEtapeDeux = "";
                EtapeDeuxOk = true;
            }
            else
            {
                ErreurEtapeDeux = retour;
            }
        }
        #endregion

        #region Validation
        /// <summary>
        /// Sauvegarde les élements
        /// </summary>
        public async Task Save()
        {
            if (EtapeDeuxOk && EtapeUnOk)
            {
                await ImageUnlockBusiness.Save(_imageToSave, ListePoint);
            }
        }

        /// <summary>
        /// Efface le fichier
        /// </summary>
        /// <returns></returns>
        public async Task Delete()
        {
            await ImageUnlockBusiness.DeleteFile();
            ImageLock = null;
            _imageToSave = null;
            EtapeUnOk = false;
            EffacerPoint();
            VerifPointImage();
            EtapeDeuxOk = false;
        }
        #endregion

        #region Vérification
        /// <summary>
        /// Vérifie que les points entrés par l'utilisateur correspondent à ceux attendus
        /// </summary>
        /// <param name="round">l'écart autorisé entre le point original et le point à vérifier</param>
        public async Task CheckCorrespondancePoint(int round)
        {
            bool ok = true;
            if (ListePoint.Count == ListePointToVerif.Count)
            {
                for (int i = 0; i < ListePointToVerif.Count; i++)
                {
                    var xmin = ListePoint[i].X - round;
                    var xmax = ListePoint[i].X + round;
                    var ymin = ListePoint[i].Y - round;
                    var ymax = ListePoint[i].Y + round;
                    if (ListePointToVerif[i].X > xmax || ListePointToVerif[i].X < xmin
                        || ListePointToVerif[i].Y > ymax || ListePointToVerif[i].Y < ymin)
                    {
                        ok = false;
                        break;
                    }
                }
            }
            else
            {
                ok = false;
            }
            if (ok)
            {
                await GetPassword();
            }
            else
            {
                ErreurVerif = ResourceLoader.GetForCurrentView("Errors").GetString("erreurMauvaisPoint");
            }
        }

        /// <summary>
        /// Vérifie si le mot de l'utilisateur est déchiffrable
        /// </summary>
        /// <returns>true si ok</returns>
        public async Task GetPassword()
        {
            try
            {
                var mdp = await CortanaBusiness.GetPasswordUser();
                if (mdp != null)
                {
                    ErreurVerif = ResourceLoader.GetForCurrentView("Errors").GetString("erreurAffichMdp")+ " " + mdp;
                }
                else
                {
                    ErreurVerif = ResourceLoader.GetForCurrentView("Errors").GetString("erreurRecupPass");
                }
            }
            catch (Exception)
            {
                ErreurVerif = ResourceLoader.GetForCurrentView("Errors").GetString("erreurRecupPass");
            }
        }
        
        #endregion
    }
}
