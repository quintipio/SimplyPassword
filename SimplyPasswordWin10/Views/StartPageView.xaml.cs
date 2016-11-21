using System;
using Windows.ApplicationModel.Resources;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using SimplyPasswordWin10.Interface;
using SimplyPasswordWin10.ViewModel;
using SimplyPasswordWin10Shared.Business;
using SimplyPasswordWin10Shared.Context;
using SimplyPasswordWin10Shared.Enum;
using SimplyPasswordWin10Shared.Utils;

namespace SimplyPasswordWin10.Views
{
    /// <summary>
    /// Page de démarrage pour de l'appli
    /// </summary>
    public sealed partial class StartPageView : IView<StartPageViewModel>
    {
        /// <summary>
        /// ViewModel
        /// </summary>
        public StartPageViewModel ViewModel { get; set; }

        private bool _validAutorise;

        private readonly DispatcherTimer _timerCapsLock;

        /// <summary>
        /// constructeur
        /// </summary>
        public StartPageView()
        {
            InitializeComponent();
            _validAutorise = true;
            _timerCapsLock = new DispatcherTimer();
            _timerCapsLock.Tick += TimerCapsLockOnTick;
            _timerCapsLock.Start();
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

        /// <summary>
        /// Se lance lors de l'ouverture de la page
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel = new StartPageViewModel((ModeOuvertureEnum)e.Parameter);
            StackTitre.Background = ContexteAppli.GetColorTheme();
            ButtonRecupPass.Visibility = (ContexteAppli.IsFichierRoamingOuvert && ContexteAppli.IsCortanaActive && await ImageUnlockBusiness.FileExist() && ViewModel.ModeSelect.Equals(ModeOuvertureEnum.FichierDejaExistant))
                ? Visibility.Visible
                : Visibility.Collapsed;


            if (ViewModel.ModeSelect.Equals(ModeOuvertureEnum.FichierDejaExistant))
            {
                TextTitre.Text = ResourceLoader.GetForCurrentView().GetString("phraseAcceuilExistant");
                GridConfirm.Visibility = Visibility.Collapsed;
                ProgressBarForce.Visibility = Visibility.Collapsed;
                ButtonReinit.Visibility = ContexteAppli.IsFichierRoamingOuvert? Visibility.Visible : Visibility.Collapsed;
            }

            if (ViewModel.ModeSelect.Equals(ModeOuvertureEnum.FichierACreer))
            {
                TextTitre.Text = ResourceLoader.GetForCurrentView().GetString("phraseAcceuilNouveau");
                GridConfirm.Visibility = Visibility.Visible;
                ProgressBarForce.Visibility = Visibility.Visible;
                ButtonReinit.Visibility = Visibility.Collapsed;
            }
            PasswordBoxMdp.Focus(FocusState.Keyboard);
            Frame.BackStack.Clear();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Frame.BackStack.Clear();
        }

        /// <summary>
        /// Lance l'ouverture du fichier ou sa création
        /// </summary>
        private async void Valider()
        {
            if (_validAutorise)
            {
                RingWait.IsActive = true;
                var erreur = await ViewModel.Valider();

                if (string.IsNullOrWhiteSpace(erreur))
                {
                    TextError.Text = "";
                    _timerCapsLock.Stop();
                    ((Frame)Window.Current.Content).Navigate(typeof(MainPageView));
                }
                else
                {
                    TextError.Text = erreur;
                }
                RingWait.IsActive = false;
            }
        }
        

        #region évènements
        private void validMdp_Click(object sender, RoutedEventArgs e)
        {
            Valider();
        }

        private void passwordBoxMdp_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                Valider();
            }
            
        }

        private void passwordBoxConfirm_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                Valider();
            }
        }

        private void buttonQuitter_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        private async void buttonReinit_Click(object sender, RoutedEventArgs e)
        {
            _validAutorise = false;
            var res = await MessageBox.ShowAsync(ResourceLoader.GetForCurrentView().GetString("phraseAvertissementReinit"), ResourceLoader.GetForCurrentView().GetString("phraseAttention"), MessageBoxButton.YesNo);
            if (res == MessageBoxResult.Yes)
            {
                await ViewModel.ReinitAppli();
            }
            _validAutorise = true;
        }


        private async void buttonRecup_Click(object sender, RoutedEventArgs e)
        {
            if (ContexteAppli.IsCortanaActive && await ImageUnlockBusiness.FileExist())
            {
                ((Frame)Window.Current.Content).Navigate(typeof(SecureImageView), OuvertureSecureImageEnum.MODE_DEBLOCAGE);
            }
        }
        #endregion
    }
}
