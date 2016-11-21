using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using SimplyPasswordWin10.Interface;
using SimplyPasswordWin10.ViewModel;
using SimplyPasswordWin10Shared.Context;
using SimplyPasswordWin10Shared.Model;
using SimplyPasswordWin10Shared.Utils;


namespace SimplyPasswordWin10.Views
{
    /// <summary>
    /// Vue pour récupérer les mots de passe d'un partage
    /// </summary>
    public sealed partial class RecupPasswordView : IView<RecupPasswordViewModel>
    {
        /// <summary>
        /// ViewModel
        /// </summary>
        public RecupPasswordViewModel ViewModel { get; set; }
        
        private readonly DispatcherTimer _timerCapsLock;

        /// <summary>
        /// Constructeur
        /// </summary>
        public RecupPasswordView()
        {
            InitializeComponent();
            ViewModel = new RecupPasswordViewModel();
            _timerCapsLock = new DispatcherTimer();
            _timerCapsLock.Tick += TimerCapsLockOnTick;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            GridTitre.Background = ContexteAppli.GetColorTheme();

            if (e.Parameter is StorageFile)
            {
                await ViewModel.Init((StorageFile) e.Parameter);
            }

            if (e.Parameter is ObservableCollection<MotDePasse>)
            {
                ViewModel.Init((ObservableCollection<MotDePasse>)e.Parameter);
            }
            ChangeNomButtonValid();
            //ViewModel.ListeMotDePasses = new ObservableCollection<MotDePasse>(ViewModel.ListeMotDePasses);

        }


        private void TimerCapsLockOnTick(object sender, object o)
        {
            if (CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.CapitalLock) == CoreVirtualKeyStates.Locked)
            {
                TextCapsLock.Visibility = Visibility.Visible;
            }
            else
            {
                TextCapsLock.Visibility = Visibility.Collapsed;
            }
        }

        #region Page principale

        private void ChangeNomButtonValid()
        {
            RecupereLogButton.Content = (ViewModel.IsLog) ? ResourceLoader.GetForCurrentView().GetString("textRecuperer") : ResourceLoader.GetForCurrentView().GetString("textOuvrirFchier");
        }

        private async void SelectFolder_Click(object sender, RoutedEventArgs e)
        {
            var mdp = ((AppBarButton)sender).Tag as MotDePasse;

            if (mdp != null)
            {
                ViewModel.SelectedMotDePasse = mdp;
                ViewModel.NaviguerDossier(mdp.DossierPossesseur ?? ContexteAppli.DossierMere);
                await SelectDossierContentDialog.ShowAsync();
            }
        }


        private async void RecupereLog_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ViewModel.IsLog)
                {
                    var erreur = await ViewModel.Recuperer();
                    if (!string.IsNullOrWhiteSpace(erreur))
                    {
                        await MessageBox.ShowAsync(erreur);
                    }

                    if (ViewModel.ValidSave)
                    {
                        ((Frame)Window.Current.Content).Navigate(typeof(MainPageView));
                    }
                }
                else
                {
                    _timerCapsLock.Start();
                    await OpenFileContentDialog.ShowAsync();
                }
            }
            catch (Exception)
            {
                await MessageBox.ShowAsync(ResourceLoader.GetForCurrentView("Errors").GetString("erreurInconnu"));
            }
        }

        #endregion


        #region Dlg Choix de dossier
        private void ChoixDossier_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            foreach (var mdp in ViewModel.ListeMotDePasses)
            {
                if (mdp.Equals(ViewModel.SelectedMotDePasse))
                {
                    mdp.DossierPossesseur = ViewModel.DossierEncours;
                    break;
                }
            }
            ViewModel.ListeMotDePasses = new ObservableCollection<MotDePasse>(ViewModel.ListeMotDePasses);
            SelectDossierContentDialog.Hide();
        }
        private void ChoixDossier_SecondaryButton(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
           SelectDossierContentDialog.Hide();
        }

        private void dossierParentButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NaviguerDossier(ViewModel.DossierEncours.DossierParent);
        }

        private void SelectDossierForMdp_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var dos = ((TextBlock)sender).Tag as Dossier;
            if (dos != null)
            {
                ViewModel.NaviguerDossier(dos);
            }
        }
        #endregion


        #region Dlg du mot de passe

        private async Task Valide()
        {
            var erreur = await ViewModel.OuvrirFichierPassword();
            if (!string.IsNullOrEmpty(erreur))
            {
                await MessageBox.ShowAsync(erreur);
            }
            else
            {
                ChangeNomButtonValid();
                OpenFileContentDialog.Hide();
                _timerCapsLock.Stop();
            }
        }

        private async void PasswordFileBox_OnKeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                await Valide();
            }

        }
        private async void ValidPassFile_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            await Valide();
        }

        private void AnnuleChoixPass_SecondaryButton(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            OpenFileContentDialog.Hide();
            _timerCapsLock.Stop();
        }

        #endregion

        private void RecupPasswordPage_Loaded(object o, RoutedEventArgs routedEventArgs)
        {
            if(ViewModel?.ListeMotDePasses != null)
            {
                ViewModel.ListeMotDePasses = new ObservableCollection<MotDePasse>(ViewModel.ListeMotDePasses);
            }
        }
    }
}
