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

namespace SimplyPasswordWin10.Views
{
    /// <summary>
    /// Vue de la page de résultat de cortana
    /// </summary>
    public sealed partial class ResultCortanaView : IView<ResultCortanaViewModel>
    {

        /// <summary>
        /// ViewModel
        /// </summary>
        public ResultCortanaViewModel ViewModel { get; set; }

        private readonly DispatcherTimer _timerCapsLock;


        public ResultCortanaView()
        {
            InitializeComponent();
            _timerCapsLock = new DispatcherTimer();
            _timerCapsLock.Tick += TimerCapsLockOnTick;
            _timerCapsLock.Start();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            GridTitre.Background = ContexteAppli.GetColorTheme();
            TextTitre.Text = ResourceLoader.GetForCurrentView().GetString("phraseAcceuilExistant");
            PasswordBoxMdp.Focus(FocusState.Keyboard);
            ViewModel = new ResultCortanaViewModel(e.Parameter as string);

            if (await ViewModel.LanceRechercheSansMotDePasse())
            {
                GridMdp.Visibility = Visibility.Collapsed;
                GridResult.Visibility = Visibility.Visible;
            }
            else
            {
                GridMdp.Visibility = Visibility.Visible;
                GridResult.Visibility = Visibility.Collapsed;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Frame.BackStack.Clear();
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
        /// Lance l'ouverture du fichier ou sa création
        /// </summary>
        private async void ValiderMdp()
        {
            RingWait.IsActive = true;
            var erreur = await ViewModel.Valider();

            if (string.IsNullOrWhiteSpace(erreur))
            {
                TextError.Text = "";
                RingWait.IsActive = true;
                GridMdp.Visibility = Visibility.Collapsed;
                GridResult.Visibility = Visibility.Visible;
                _timerCapsLock.Stop();
            }
            else
            {
                TextError.Text = erreur;
            }
            RingWait.IsActive = false;
        }
        
        private void validMdp_Click(object sender, RoutedEventArgs e)
        {
            ValiderMdp();
        }

        private void passwordBoxMdp_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                ValiderMdp();
            }
        }

        private async void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            if (GridMdp.Visibility == Visibility.Visible)
            {
                if (await PasswordBusiness.DoesFileCypherExist())
                {

                    ((Frame)Window.Current.Content).Navigate(typeof(StartPageView), ModeOuvertureEnum.FichierDejaExistant);
                }
                else
                {

                    ((Frame)Window.Current.Content).Navigate(typeof(StartPageView), ModeOuvertureEnum.FichierACreer);
                }
            }
            else
            {
                ((Frame)Window.Current.Content).Navigate(typeof(MainPageView));
            }
        }
    }
}
