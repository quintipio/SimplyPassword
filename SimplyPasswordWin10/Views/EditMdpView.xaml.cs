using System.Globalization;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;
using SimplyPasswordWin10.Interface;
using SimplyPasswordWin10.ViewModel;
using SimplyPasswordWin10Shared.Context;
using SimplyPasswordWin10Shared.Enum;
using SimplyPasswordWin10Shared.Model;
using SimplyPasswordWin10Shared.Utils;

namespace SimplyPasswordWin10.Views
{
    /// <summary>
    /// Page pour l'édition et l'ajout des mots de passes
    /// </summary>
    public sealed partial class EditMdpView : IView<EditPasswordViewModel>
    {
        /// <summary>
        /// controleur
        /// </summary>
        public EditPasswordViewModel ViewModel { get; set; }
        
        /// <summary>
        /// contructeur
        /// </summary>
        public EditMdpView()
        {
            InitializeComponent();
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            GridTitre.Background = ContexteAppli.GetColorTheme();
            
                //si ont fournit un dossier c'est qu'on veut y ajouter un mot de passe
                var dossier = e.Parameter as Dossier;
                if (dossier != null)
                {
                    ViewModel = new EditPasswordViewModel(dossier);
                    TitrePage.Text = ResourceLoader.GetForCurrentView().GetString("phraseAjout");
                }

                //si c'est un mot de passe, c'est qu'ont souhaite le modifier
                var motDePasse = e.Parameter as MotDePasse;
                if (motDePasse != null)
                {
                    ViewModel = new EditPasswordViewModel(motDePasse);
                    TitrePage.Text = ResourceLoader.GetForCurrentView().GetString("phraseModification");
                }
        }
        

        #region image

        private void comboIcone_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = ComboIcone.SelectedItem as Image;
            if (item?.Tag is int)
            {
                ViewModel.SelectedIcone = (int)item.Tag;
            }
        }

        #endregion

        #region génération mot de passe
        private void longueurSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            TextLongGen.Text = LongueurSlider.Value.ToString(CultureInfo.InvariantCulture);
        }

        private void genererButton_Click(object sender, RoutedEventArgs e)
        {
            CheckLettres.IsChecked = true;
            CheckChiffres.IsChecked = true;
            CheckSpec.IsChecked = true;
            TextLongGen.Text = ContexteStatic.NbCaractereGenerePassword.ToString();
            LongueurSlider.Value = ContexteStatic.NbCaractereGenerePassword;
            StoryboardOuvertureGenerateur.Begin();
        }

        private void buttonGenereFin_Click(object sender, RoutedEventArgs e)
        {
            if (CheckLettres.IsChecked != null && (CheckChiffres.IsChecked != null && (CheckSpec.IsChecked != null && (((bool)CheckLettres.IsChecked || (bool)CheckChiffres.IsChecked || (bool)CheckSpec.IsChecked) && LongueurSlider.Value != 0))))
            {
                ViewModel.GenererPassword(int.Parse(LongueurSlider.Value.ToString(CultureInfo.InvariantCulture)), (bool)CheckLettres.IsChecked, (bool)CheckChiffres.IsChecked, (bool)CheckSpec.IsChecked);
            }
            StoryboardFermetureGenerateur.Begin();
        }
        #endregion

        #region validation
        private async void validerButton_Click(object sender, RoutedEventArgs e)
        {
            WaitRing.IsActive = true;
            ValiderButton.IsEnabled = false;
            var fail = false;
            try
            {
               var erreur = await ViewModel.Save();
                if (!string.IsNullOrWhiteSpace(erreur))
                {
                    await MessageBox.ShowAsync(erreur);
                }
                else
                {
                    if (ViewModel.Action.Equals(ActionMotDePasseEnum.Ajouter))
                    {
                        ((Frame)Window.Current.Content).Navigate(typeof(MainPageView));
                    }
                    else
                    {
                        if (Frame.CanGoBack)
                        {
                            Frame.GoBack();
                        }
                    }
                }
            }
            catch
            {
                fail = true;
            }

            if (fail) { await MessageBox.ShowAsync(ResourceLoader.GetForCurrentView("Errors").GetString("erreurInconnu")); }

            WaitRing.IsActive = false;
            ValiderButton.IsEnabled = true;
        }
        #endregion

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel.Action == ActionMotDePasseEnum.Modifier)
            {
                var index = 0;
                foreach (var item in ComboIcone.Items)
                {
                    var it = item as Image;
                    if ((int)it.Tag == ViewModel.PasswordOriginal.IdIcone)
                    {
                        ComboIcone.SelectedIndex = index;
                        break;
                    }
                    index++;
                }
            }
        }
    }
}
