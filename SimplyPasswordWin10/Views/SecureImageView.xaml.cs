using System;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using SimplyPasswordWin10.Interface;
using SimplyPasswordWin10.ViewModel;
using SimplyPasswordWin10Shared.Context;
using SimplyPasswordWin10Shared.Enum;


namespace SimplyPasswordWin10.Views
{
    /// <summary>
    /// Vue pour changer l'image de déverrouillage de l'appli ou ou obtenir le mot de passe
    /// </summary>
    public sealed partial class SecureImageView : IView<SecureImageViewModel>
    {
        /// <summary>
        /// Controleur
        /// </summary>
        public SecureImageViewModel ViewModel { get; set; }

        private readonly int _pointMargeErreurRayon = 30;

        public SecureImageView()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            GridTitre.Background = ContexteAppli.GetColorTheme();
            ViewModel = new SecureImageViewModel((OuvertureSecureImageEnum)e.Parameter);
            await ViewModel.LoadSecurity();
            if (ViewModel.Mode == OuvertureSecureImageEnum.MODE_CHANGEMENT_IMAGE)
            {
                GridChargement.Visibility = Visibility.Visible;
                GridDeblocage.Visibility = Visibility.Collapsed;
                AfficherPoints();
            }
            if (ViewModel.Mode == OuvertureSecureImageEnum.MODE_DEBLOCAGE)
            {
                GridDeblocage.Visibility = Visibility.Visible;
                GridChargement.Visibility = Visibility.Collapsed;
            }

        }
        

        /// <summary>
        /// Affiche les points sur l'image
        /// </summary>
        private void AfficherPoints()
        {
            if (ViewModel.ListePoint != null)
            {
                foreach (var point in ViewModel.ListePoint)
                {
                    AddEllipse(point);
                }
            }
        }

        /// <summary>
        /// Ajoute une ellipse sur l'image ou se situe le point
        /// </summary>
        /// <param name="point">le point à ajouter sur l'image</param>
        private void AddEllipse(Point point)
        {
            var ellipse = new Ellipse
            {
                Width = _pointMargeErreurRayon*2,
                Height = _pointMargeErreurRayon*2,
                StrokeThickness = 5,
                Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0)),
                Margin = new Thickness(point.X - (_pointMargeErreurRayon), point.Y - (_pointMargeErreurRayon), 0, 0),
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Tag = "Ellipse"
            };
            GridImage.Children.Add(ellipse);
        }

        /// <summary>
        /// Efface les points de l'image
        /// </summary>
        private void EffacerPoint()
        {
            ViewModel.EffacerPoint();
            var el = new Ellipse();
            var liste = GridImage.Children.Where(x => x.GetType() == el.GetType()).ToList();
            foreach (var element in liste)
            {
                GridImage.Children.Remove(element);
            }
        }

        private async void LoadFile_Click(object sender, RoutedEventArgs e)
        {
            var fileOpenPicker = new FileOpenPicker
            {
                CommitButtonText = ResourceLoader.GetForCurrentView().GetString("phraseOK"),
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.Downloads,
                FileTypeFilter = {".jpeg", ".jpg", ".png", ".bmp"},
            };

            var fichier = await fileOpenPicker.PickSingleFileAsync();
            if (fichier != null)
            {
                await ViewModel.SaveImage(fichier);
                EffacerPoint();
            }
        }


        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //ajout si moins de neuf points
            if (ViewModel.ListePoint != null && ViewModel.ListePoint.Count < 9)
            {
                var point = e.GetPosition(ImageLock);
                AddEllipse(point);
                ViewModel.AddPoint(point);
            }
        }

        private void Valid_EtapeDeux_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.VerifEtapeDeux();
        }

        private void EffacePoint_Click(object sender, RoutedEventArgs e)
        {
           EffacerPoint();
            ViewModel.ErreurEtapeDeux = "";
            ViewModel.ErreurVerif = "";
        }

        /// <summary>
        /// Quitte la page et revient en arrière
        /// </summary>
        private void Quit()
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private void Quit_click(object sender, RoutedEventArgs e)
        {
            Quit();
        }

        private async void DeleteAndQuit_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.Delete();
            EffacerPoint();
            Quit();
        }

        private async void SaveAndQuit_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.Save();
            Quit();
        }

        private void AnnuleVerifMotDePasse_Click(object sender, RoutedEventArgs e)
        {
            ((Frame)Window.Current.Content).Navigate(typeof(StartPageView), ModeOuvertureEnum.FichierDejaExistant);
        }

        private async void ValidVerifMotDePasse_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.CheckCorrespondancePoint(_pointMargeErreurRayon);
        }
        
    }
}
