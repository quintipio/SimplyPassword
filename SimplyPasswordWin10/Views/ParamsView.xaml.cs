using System.Globalization;
using Windows.ApplicationModel.Resources;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using SimplyPasswordWin10.Interface;
using SimplyPasswordWin10.ViewModel;
using SimplyPasswordWin10Shared.Context;
using SimplyPasswordWin10Shared.Enum;
using SimplyPasswordWin10Shared.Strings;
using SimplyPasswordWin10Shared.Utils;

namespace SimplyPasswordWin10.Views
{
    /// <summary>
    /// Classe de la vue des paramètres
    /// </summary>
    public sealed partial class ParamsView : IView<ParamsViewModel>
    {

        /// <summary>
        /// ViewModel
        /// </summary>
        public ParamsViewModel ViewModel { get; set; }

        /// <summary>
        /// permet d'indiquer quand la pages est finie de charger pour éviter de déclencher trop souvent les évènements _Changed
        /// </summary>
        private bool _canChangeLangue;

        private readonly DispatcherTimer _timerCapsLock;


        /// <summary>
        /// constructeur
        /// </summary>
        public ParamsView()
        {
            InitializeComponent();
            _canChangeLangue = false;
            _timerCapsLock = new DispatcherTimer();
            _timerCapsLock.Tick += TimerCapsLockOnTick;
            _timerCapsLock.Start();
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            StackPanelSecurite.Visibility = (ContexteAppli.IsFichierRoamingOuvert)
                ? Visibility.Visible
                : Visibility.Collapsed;
            ViewModel = new ParamsViewModel();
            GridTitre.Background  = ContexteAppli.GetColorTheme();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _timerCapsLock.Stop();
            base.OnNavigatedFrom(e);
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



        #region mot de passe


        private void TextBoxNouveauMdp_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            TextBoxNouveauMdp.PasswordRevealMode = PasswordRevealMode.Peek;
        }

        private void buttonGenerer_Click(object sender, RoutedEventArgs e)
        {
            TextBoxConfirmation.Password = "";
            TextBoxNouveauMdp.Password = "";
            CheckLettres.IsChecked = true;
            CheckChiffres.IsChecked = true;
            CheckSpec.IsChecked = true;
            TextLongGen.Text = ContexteStatic.NbCaractereGenerePassword.ToString();
            LongueurSlider.Value = ContexteStatic.NbCaractereGenerePassword;
            StoryboardOuvertueGenerateur.Begin();
        }

        private void sliderTailleMdp_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            TextLongGen.Text = LongueurSlider.Value.ToString(CultureInfo.InvariantCulture);
        }


        private void buttonGenererAction_Click(object sender, RoutedEventArgs e)
        {
            if (CheckLettres.IsChecked != null && (CheckChiffres.IsChecked != null && (CheckSpec.IsChecked != null && (((bool)CheckLettres.IsChecked || (bool)CheckChiffres.IsChecked || (bool)CheckSpec.IsChecked) && LongueurSlider.Value != 0))))
            {
               ViewModel.GenererPassword(int.Parse(LongueurSlider.Value.ToString(CultureInfo.InvariantCulture)), (bool)CheckLettres.IsChecked, (bool)CheckChiffres.IsChecked, (bool)CheckSpec.IsChecked);
                TextBoxNouveauMdp.PasswordRevealMode = PasswordRevealMode.Visible;
            }
            StoryboardFermetureGenerateur.Begin();
        }

        private async void buttonValider_Click(object sender, RoutedEventArgs e)
        {
            var fail = false;
            try
            {
                var erreur = await ViewModel.ChangerMdp();

                if (string.IsNullOrWhiteSpace(erreur))
                {
                    await MessageBox.ShowAsync(ResourceLoader.GetForCurrentView().GetString("phraseChangementEffectue"));
                }
                else
                {
                    TextError.Text = erreur;
                }
            }
            catch
            {
                fail = true;
            }

            if (fail) { await MessageBox.ShowAsync(ResourceLoader.GetForCurrentView("Errors").GetString("erreurModificationMDP")); }

        }

        #endregion


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _canChangeLangue = true;
            SelectLangueCombo();
        }

        #region langue

        private void SelectLangueCombo()
        {
            ComboListeLangue.SelectedIndex = ViewModel.SelectedLangue.Id - 1;
        }

        //change la langue de l'appli à partir de la comboBox
        private async void comboListeLangue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_canChangeLangue && ComboListeLangue.SelectedItem is ListeLangues.LanguesStruct)
            {
                _canChangeLangue = false;
                ViewModel.SelectedLangue = (ListeLangues.LanguesStruct) ComboListeLangue.SelectedItem;
                await ViewModel.ChangeLangueApplication();
                _canChangeLangue = true;
            }
        }

        #endregion



        #region couleur

        //change la couleur du thème
        private async void Rectangle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await ViewModel.ChangeCouleur(((SolidColorBrush)((Rectangle)sender).Tag));
            GridTitre.Background = ContexteAppli.GetColorTheme();
        }

        #endregion

        private async void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (_canChangeLangue)
            {
                await ViewModel.ChangeCortana(CheckboxCortana.IsChecked ?? false);
            }
        }

        private async void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_canChangeLangue)
            {
                await ViewModel.ChangeCortana(CheckboxCortana.IsChecked ?? false);
            }
        }

        private void RecupMdp_Click(object sender, RoutedEventArgs e)
        {
            if (ContexteAppli.IsCortanaActive)
            {
                ((Frame)Window.Current.Content).Navigate(typeof(SecureImageView), OuvertureSecureImageEnum.MODE_CHANGEMENT_IMAGE);
            }
        }
    }
}
