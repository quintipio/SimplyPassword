using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SimplyPasswordWin10.Abstract;
using SimplyPasswordWin10.Interface;
using SimplyPasswordWin10.ViewModel;
using SimplyPasswordWin10Shared.Context;
using SimplyPasswordWin10Shared.Model;
using SimplyPasswordWin10Shared.Utils;

namespace SimplyPasswordWin10.Views
{
    /// <summary>
    /// Page principale de l'appli pour afficher les dossier et les mots de passes
    /// </summary>
    public sealed partial class MainPageView : IView<MainPageViewModel>
    {
        /// <summary>
        /// ViewModel
        /// </summary>
        public MainPageViewModel ViewModel { get; set; }

        /// <summary>
        /// Couleur de fond des boutons de mot de passes
        /// </summary>
        public SolidColorBrush BgColor { get; set; }
        
        /// <summary>
        /// pour la fenetre de gestion de dossier, indique si il s'agit d'un nouveau dossier ou non
        /// </summary>
        private bool _isNewDossier;

        /// <summary>
        /// constructeur
        /// </summary>
        public MainPageView()
        {
            InitializeComponent();
        }
        
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel = new MainPageViewModel();
            ViewModel.OuvrirDossier(AbstractViewModel.SelectedDossierAbstract);

            DerniereSynchroText.Visibility = (ContexteAppli.IsFichierRoamingOuvert)
                ? Visibility.Visible
                : Visibility.Collapsed;


            var couleur = ContexteAppli.GetColorTheme();
            BgColor = couleur;
            DossierSplitview.Background = couleur;
            DossierSplitview.PaneBackground = couleur;
            GridTitre.Background = couleur;

            await ViewModel.MajDerniereSynchro();

        }
        


        #region évènements commun
        
        private void ParamsRadioButton_Click(object sender, RoutedEventArgs e)
        {
            ((Frame)Window.Current.Content).Navigate(typeof(ParamsView));
        }

        private void OutilsRadioButton_Click(object sender, RoutedEventArgs e)
        {
            ((Frame)Window.Current.Content).Navigate(typeof(OutilsView));
        }

        private async void RateButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?PFN=" + Package.Current.Id.FamilyName));
        }

        private async void BugsButton_Click(object sender, RoutedEventArgs e)
        {
            var mailto = new Uri("mailto:?to=" + ContexteStatic.Support + "&subject=Bug or suggestion for " + ContexteStatic.NomAppli);
            await Launcher.LaunchUriAsync(mailto);
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            ((Frame)Window.Current.Content).Navigate(typeof(AboutPageView));
        }
        
        private void QuitterRadioButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        #endregion





        #region évènements dossiers

        private void dossierParentButton_Click(object sender, RoutedEventArgs e)
        {
           ViewModel.OuvriDossierParent();
        }

        private void ajouterDossierButton_Click(object sender, RoutedEventArgs e)
        {
            _isNewDossier = true;
            TextBoxEditionDossier.Text = "";
            TextEditionDossier.Text = ResourceLoader.GetForCurrentView().GetString("phraseNouveauDossier");
            ListeMdpGrid.Visibility = Visibility.Collapsed;
            EditMdpGrid.Visibility = Visibility.Visible;
            TextBoxEditionDossier.Focus(FocusState.Keyboard);
        }

        private void editDossierButton_Click(object sender, TappedRoutedEventArgs e)
        {
            _isNewDossier = false;

            ViewModel.DossierToEdit = (Dossier)(((AppBarButton)sender).Tag);
            ViewModel.DossierToEditName = ViewModel.DossierToEdit.Titre;

            TextEditionDossier.Text = ResourceLoader.GetForCurrentView().GetString("phraseModifierDossier");
            ListeMdpGrid.Visibility = Visibility.Collapsed;
            EditMdpGrid.Visibility = Visibility.Visible;
            TextBoxEditionDossier.Focus(FocusState.Keyboard);
        }

        private async void supButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var res = await MessageBox.ShowAsync(ResourceLoader.GetForCurrentView().GetString("phraseAvertissementSupDossier"), ResourceLoader.GetForCurrentView().GetString("phraseAttention"), MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                {
                    await ViewModel.SuppprimerDossier((Dossier)((AppBarButton)sender).Tag);
                    ViewModel.OuvrirDossier(ViewModel.DossierSelected);
                }
            }
            catch
            {
                await MessageBox.ShowAsync(ResourceLoader.GetForCurrentView("Errors").GetString("erreurInconnu"));
            }
           
        }


        private async void buttonValiderEditionDossier_Click(object sender, RoutedEventArgs e)
        {
            await ValiderDossier();
        }

        private async Task ValiderDossier()
        {
            try
            { 
                if (_isNewDossier)
                {
                    var retour = await ViewModel.AjouterDossier();

                    if (string.IsNullOrWhiteSpace(retour))
                    {
                        EditMdpGrid.Visibility = Visibility.Collapsed;
                        ListeMdpGrid.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        await MessageBox.ShowAsync(retour);
                    }
                }
                else
                {
                    var retour = await ViewModel.ModifierDossier();

                    if (string.IsNullOrWhiteSpace(retour))
                    {
                        EditMdpGrid.Visibility = Visibility.Collapsed;
                        ListeMdpGrid.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        await MessageBox.ShowAsync(retour);
                    }
                }
            }
            catch
            {
                await MessageBox.ShowAsync(ResourceLoader.GetForCurrentView("Errors").GetString("erreurInconnu"));
            }
        }
        private void buttonAnnulerEditionDossier_Click(object sender, RoutedEventArgs e)
        {
            EditMdpGrid.Visibility = Visibility.Collapsed;
            ListeMdpGrid.Visibility = Visibility.Visible;
            ViewModel.DossierToEditName = "";
        }


        private void Ouvrirdossier_Click(object sender, TappedRoutedEventArgs e)
        {
            Dossier dossier = null;
            if (sender is AppBarButton)
            {
                dossier = ((AppBarButton) sender).Tag as Dossier;
            }
            else if (sender is Grid)
            {
                dossier = ((Grid)sender).Tag as Dossier;
            }
            else if (sender is TextBlock)
            {
                dossier = ((TextBlock)sender).Tag as Dossier;
            }
            if (dossier != null)
            {
                ViewModel.OuvrirDossier(dossier);
            }
        }
        
        private async void TextBoxEditionDossier_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                await ValiderDossier();
            }
        }
        #endregion




        #region évènements mots de passes
        private void ajouterMDPButton_Click(object sender, RoutedEventArgs e)
        {
            ((Frame)Window.Current.Content).Navigate(typeof(EditMdpView), ViewModel.DossierSelected);
        }

        private void motDePasse_Click(object sender, TappedRoutedEventArgs e)
        {
            if ((sender as TextBlock)?.Tag is MotDePasse)
            {
                ((Frame)Window.Current.Content).Navigate(typeof(AffichMdpView), ((TextBlock)sender).Tag);
            }

            if ((sender as Image)?.Tag is MotDePasse)
            {
                ((Frame)Window.Current.Content).Navigate(typeof(AffichMdpView), ((Image)sender).Tag);
            }
        }
        #endregion



        #region évènement Affichage
        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            DossierSplitview.IsPaneOpen = !DossierSplitview.IsPaneOpen;
            CheckSecButtonVisible();
        }

        private void CheckSecButtonVisible()
        {
            if (!DossierSplitview.IsPaneOpen || (DossierSplitview.IsPaneOpen && Width < 750))
            {
                RateButton.Visibility = Visibility.Collapsed;
                BugsButton.Visibility = Visibility.Collapsed;
                AboutButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                RateButton.Visibility = Visibility.Visible;
                BugsButton.Visibility = Visibility.Visible;
                AboutButton.Visibility = Visibility.Visible;
            }
        }
        #endregion

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CheckSecButtonVisible();
        }
        
    }
}
