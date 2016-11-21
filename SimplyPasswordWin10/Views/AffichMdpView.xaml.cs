using System;
using System.Collections.Generic;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
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
    /// Page d'affichage des données du mot de passe
    /// </summary>
    public sealed partial class AffichMdpView : IView<AfficherPasswordViewModel>
    {
        private DispatcherTimer _timerLogin;
        private DispatcherTimer _timerPassword;
        //en release, le clipboard clean ne fonctionne pas si l'application n'est pas en foreground. donc un timer s'active pour effacer le contenu si l'appel à planter et fonctionnera jusqu'à effacement du clipboard
        private DispatcherTimer _timerCleanClipBoard;

        private int _compteurSecondeTimerLogin;
        private int _compteurSecondeTimerPassword;

        private bool _isTimerLoginLaunch;
        private bool _isTimerPasswordLaunch;

        private string _texteMdpShare;

        /// <summary>
        /// Controleur
        /// </summary>
        public AfficherPasswordViewModel ViewModel { get; set; }

        /// <summary>
        /// Constructeur
        /// </summary>
        public AffichMdpView()
        {
            InitializeComponent();
            DataTransferManager.GetForCurrentView().DataRequested += MainPage_DataRequested;
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            GridTitre.Background = ContexteAppli.GetColorTheme();
            ViewModel = new AfficherPasswordViewModel(e.Parameter as MotDePasse);

            _timerLogin = new DispatcherTimer {Interval = new TimeSpan(0, 0, 1)};
            _timerPassword = new DispatcherTimer { Interval = new TimeSpan(0, 0, 1) };
            _timerCleanClipBoard = new DispatcherTimer {Interval = new TimeSpan(0,0,1)};
            _timerCleanClipBoard.Tick += timerClean_Tick;
            _timerLogin.Tick += timerLogin_Tick;
            _timerPassword.Tick += timerPassword_Tick;
            _compteurSecondeTimerLogin = 0;
            _compteurSecondeTimerPassword = 0;
            _isTimerLoginLaunch = false;
            _isTimerPasswordLaunch = false;
        }



        #region gestion de la copie dans le presse papier

        private void timerClean_Tick(object sender, object e)
        {
            try
            {
                Clipboard.Clear();
                _timerCleanClipBoard.Stop();
            }
            catch
            {
                // ignored
            }
        }

        private void timerPassword_Tick(object sender, object e)
        {
            _compteurSecondeTimerPassword++;
            ProgressBarCopyPassword.Value = 100 -
                                            ((100*_compteurSecondeTimerPassword)/
                                             (ContexteStatic.WaitBeforeDelPassword/1000));
            if (_compteurSecondeTimerPassword > ContexteStatic.WaitBeforeDelPassword/1000)
            {
                ProgressBarCopyPassword.Value = 0;
                _compteurSecondeTimerPassword = 0;
                _timerPassword.Stop();
                _isTimerPasswordLaunch = false;
                try
                {
                    Clipboard.Clear();
                }
                catch (Exception)
                {
                    _timerCleanClipBoard.Start();
                }
            }

        }

        private void timerLogin_Tick(object sender, object e)
        {
            _compteurSecondeTimerLogin++;
            ProgressBarCopyLogin.Value = 100 -
                                            ((100 * _compteurSecondeTimerLogin) /
                                             (ContexteStatic.WaitBeforeDelPassword / 1000));
            if (_compteurSecondeTimerLogin > ContexteStatic.WaitBeforeDelPassword / 1000)
            {
                ProgressBarCopyLogin.Value = 0;
                _compteurSecondeTimerLogin = 0;
                _timerLogin.Stop();
                _isTimerLoginLaunch = false;
                try
                {
                    Clipboard.Clear();
                }
                catch (Exception)
                {
                    _timerCleanClipBoard.Start();
                }

            }
        }
        
        #endregion





        #region évènements

        private async void SiteWebTextBlock_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(ViewModel.Password.SiteWeb))
                {
                    var adresse = ViewModel.Password.SiteWeb;
                    if (!adresse.Contains("www.") &&! adresse.StartsWith("http://"))
                    {
                        adresse = "wwww." + adresse;
                    }

                    if (!adresse.Contains("www") && !adresse.StartsWith("http://"))
                    {
                        adresse = "www" + adresse;
                    }

                    if (!adresse.Contains("http://"))
                    {
                        adresse = "http://" + adresse;
                    }

                    await Windows.System.Launcher.LaunchUriAsync(new Uri(adresse));
                }
            }
            catch
            {
                //Ignored
            }
           
        }
        
        private void CopyIdentifiantButton_Click(object sender, RoutedEventArgs e)
        {
                if (_isTimerPasswordLaunch)
                {
                    ProgressBarCopyPassword.Value = 0;
                    _compteurSecondeTimerPassword = 0;
                    _timerPassword.Stop();
                    _isTimerPasswordLaunch = false;
                    
                }
                _compteurSecondeTimerLogin = 0;
                ProgressBarCopyLogin.Value = 100;
                ViewModel.DataIntoClipBoard(ViewModel.Password.Login);
                _isTimerLoginLaunch = true;
                _timerLogin.Start();

        }

        private void copyPasswordButton_Click(object sender, RoutedEventArgs e)
        {
                if (_isTimerLoginLaunch)
                {
                    ProgressBarCopyLogin.Value = 0;
                    _compteurSecondeTimerLogin = 0;
                    _timerLogin.Stop();
                    _isTimerLoginLaunch = false;
                }
                _compteurSecondeTimerPassword = 0;
                ProgressBarCopyPassword.Value = 100;
                ViewModel.DataIntoClipBoard(ViewModel.Password.MotDePasseObjet);
                _isTimerPasswordLaunch = true;
                _timerPassword.Start();
        }

        private async void SupButton_Click(object sender, RoutedEventArgs e)
        {
            WaitRing.IsActive = true;
            SupprimerButton.IsEnabled = false;
            DeplacerButton.IsEnabled = false;
            ModifierButton.IsEnabled = false;
            ShareButton.IsEnabled = false;
            var result = await MessageBox.ShowAsync(ResourceLoader.GetForCurrentView().GetString("phraseEffacerMDP"), ResourceLoader.GetForCurrentView().GetString("phraseAttention"), MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                try
                { 
                    await ViewModel.SupprimerMotDepasse();
                    ((Frame)Window.Current.Content).Navigate(typeof(MainPageView));
                }
                catch
                {
                    await MessageBox.ShowAsync(ResourceLoader.GetForCurrentView("Errors").GetString("erreurInconnu"));
                }
            }
            SupprimerButton.IsEnabled = true;
            DeplacerButton.IsEnabled = true;
            ModifierButton.IsEnabled = true;
            ShareButton.IsEnabled = true;
            WaitRing.IsActive = false;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            ((Frame)Window.Current.Content).Navigate(typeof(EditMdpView),ViewModel.Password);
        }

        private async void ChangePathButton_OnClickButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NaviguerDossier(ContexteAppli.DossierMere);
            await SelectDossierContentDialog.ShowAsync();
        }


        private async void ShareFichier_Click(object sender, RoutedEventArgs e)
        {
            var viewModelShare = new PartageViewModel
            {
                InOut = ModePartageInOutEnum.Partage,
                Source = ModePartageSourceEnum.Fichier
            };
            viewModelShare.ListeMotDePasseSelected.Add(ViewModel.Password);

            var listeExtension = new List<string> {"." + ContexteStatic.ExtensionPartage};

            //recherche du fichier à exporter
            var fileSavePicker = new FileSavePicker
            {
                CommitButtonText = ResourceLoader.GetForCurrentView().GetString("phraseOK"),
                SuggestedFileName = "pass",
                SuggestedStartLocation = PickerLocationId.Downloads,
                DefaultFileExtension = "." + ContexteStatic.ExtensionPartage,

            };

            //mise en mémoire du fichier
            fileSavePicker.FileTypeChoices.Add("pass", listeExtension);
            var fichierTmp = await fileSavePicker.PickSaveFileAsync();
            if (fichierTmp != null)
            {
                viewModelShare.Fichier = fichierTmp;
                var erreur = await viewModelShare.PartageFichier();
                if (!string.IsNullOrWhiteSpace(erreur))
                {
                    await MessageBox.ShowAsync(erreur);
                }
            }
        }

        private void ShareTexte_Click(object sender, RoutedEventArgs e)
        {
            var viewModelShare = new PartageViewModel
            {
                InOut = ModePartageInOutEnum.Partage,
                Source = ModePartageSourceEnum.Texte
            };
            viewModelShare.ListeMotDePasseSelected.Add(ViewModel.Password);
            viewModelShare.PartageTexte();
            _texteMdpShare = viewModelShare.Texte;
            DataTransferManager.ShowShareUI();
        }

        private void MainPage_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            if (!string.IsNullOrEmpty(_texteMdpShare))
            {
                args.Request.Data.SetText(_texteMdpShare);
                args.Request.Data.Properties.Title = Windows.ApplicationModel.Package.Current.DisplayName;
            }
        }

        #endregion



        #region Dlg Dossier
        private async void ChoixDossier_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            try
            {
                await ViewModel.ChangerEmplacementMotDePasse();
                SelectDossierContentDialog.Hide();
            }
            catch (Exception)
            {
                await MessageBox.ShowAsync(ResourceLoader.GetForCurrentView("Errors").GetString("erreurInconnu"));
            }
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
    }
}
