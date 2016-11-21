using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;
using SimplyPasswordWin10.Interface;
using SimplyPasswordWin10.ViewModel;
using SimplyPasswordWin10Shared.Context;
using SimplyPasswordWin10Shared.Enum;
using SimplyPasswordWin10Shared.Utils;

namespace SimplyPasswordWin10.Views
{
    /// <summary>
    /// Vue pour le chiffrement / déchiffrement d'un fichier
    /// </summary>
    public sealed partial class ChiffreDechiffreFichierView : IView<ChiffreDechiffreFichierViewModel>
    {
        public ChiffreDechiffreFichierViewModel ViewModel { get; set; }

        private readonly DispatcherTimer _timerCapsLock;

        public ChiffreDechiffreFichierView()
        {
            InitializeComponent();
            GridTitre.Background = ContexteAppli.GetColorTheme();
            _timerCapsLock = new DispatcherTimer();
            _timerCapsLock.Tick += TimerCapsLockOnTick;
            _timerCapsLock.Start();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel = new ChiffreDechiffreFichierViewModel((ChiffrerDechiffrerEnum)e.Parameter);
            OutputParcourir.IsEnabled = false;

            if (ViewModel.Mode == ChiffrerDechiffrerEnum.Chiffrer)
            {
                TitrePage.Text = ResourceLoader.GetForCurrentView().GetString("textChiffreFichier");
                InputFileTextBlock.Text = ResourceLoader.GetForCurrentView().GetString("textFichierAChiffrer");
                OutputFileTextBlock.Text = ResourceLoader.GetForCurrentView().GetString("textFichierChiffrer");
                ValiderButton.Content = ResourceLoader.GetForCurrentView().GetString("textChiffrer");
            }
            else
            {
                TitrePage.Text = ResourceLoader.GetForCurrentView().GetString("textDechiffreFichier");
                InputFileTextBlock.Text = ResourceLoader.GetForCurrentView().GetString("textFichierADechiffrer");
                OutputFileTextBlock.Text = ResourceLoader.GetForCurrentView().GetString("textFichierDechiffrer");
                ValiderButton.Content = ResourceLoader.GetForCurrentView().GetString("textDechiffrer");
            }

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

        #region générateur
        private void OpenGenerer_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Password = "";
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
                MdpPasswordBox.PasswordRevealMode = PasswordRevealMode.Visible;
            }
            StoryboardFermetureGenerateur.Begin();
        }

        /// <summary>
        /// Séelctionne un fichier
        /// </summary>
        /// <param name="mode">1 pour une ouverture en mode entrée, 2 pour une ouverture en mode sortie</param>
        /// <returns>la task</returns>
        private async Task SelectFile(int mode)
        {
            try
            {
                switch (mode)
                {
                    //en cas d'input
                    case 1:

                        //choix du fichier à importer
                        var fileOpenPicker = new FileOpenPicker
                        {
                            CommitButtonText = ResourceLoader.GetForCurrentView().GetString("phraseOK"),
                            ViewMode = PickerViewMode.List,
                            SuggestedStartLocation = PickerLocationId.Downloads,
                            FileTypeFilter = { ".doc", ".docx", ".pdf", ".xls", ".xlsx", ".odt", ".mp3", ".txt", ".ppt", ".ods", ".jpg", ".png", ".gif", ".bmp", ".avi", ".mpg", ".mkv", ".mp4", ".wav", ".wma", ".ogg", ".csv", ".xml", ".kwi", ".epub", "." }
                        };

                        //met en mémoire le fichier 
                        var fichier = await fileOpenPicker.PickSingleFileAsync();
                        if (fichier != null)
                        {
                            ViewModel.FileInput = fichier;
                            InputFileText.Text = fichier.Name;
                            OutputParcourir.IsEnabled = true;
                        }
                        break;

                    //en cas d'export
                    case 2:
                        if (ViewModel.Extension != null && ViewModel.NomFichier != null)
                        {
                            var extensionChoisi = "." + ViewModel.Extension;
                            var listeExtension = new List<string> { extensionChoisi };

                            //recherche du fichier à exporter
                            var fileSavePicker = new FileSavePicker
                            {
                                CommitButtonText = ResourceLoader.GetForCurrentView().GetString("phraseOK"),
                                SuggestedFileName = ViewModel.NomFichier+"1",
                                SuggestedStartLocation = PickerLocationId.Downloads,
                                DefaultFileExtension = extensionChoisi,

                            };

                            //mise en mémoire du fichier
                            fileSavePicker.FileTypeChoices.Add(ViewModel.NomFichier, listeExtension);
                            var fichierTmp = await fileSavePicker.PickSaveFileAsync();
                            if (fichierTmp != null)
                            {
                                OutputFileText.Text = fichierTmp.Name;
                                ViewModel.FileOutput = fichierTmp;
                            }
                        }
                        else
                        {
                            await MessageBox.ShowAsync(ResourceLoader.GetForCurrentView().GetString("phraseDemandeInput"));
                        }
                        break;
                }
            }
            catch
            {
                await MessageBox.ShowAsync(ResourceLoader.GetForCurrentView("Errors").GetString("erreurSelectionFichier"));
            }
        }
        #endregion


        private async void LanceAction_Click(object sender, RoutedEventArgs e)
        {
            RingWait.IsActive = true;
            var retour = await ViewModel.Valider();
            if (string.IsNullOrWhiteSpace(retour))
            {
                await MessageBox.ShowAsync(ViewModel.Mode == ChiffrerDechiffrerEnum.Chiffrer?ResourceLoader.GetForCurrentView().GetString("textChiffrementOk") : ResourceLoader.GetForCurrentView().GetString("textDechiffrementOk"));
            }
            else
            {
                await MessageBox.ShowAsync(retour);
            }

            RingWait.IsActive = false;
        }

        private void MdpPasswordBox_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            MdpPasswordBox.PasswordRevealMode = PasswordRevealMode.Peek;
        }

        private async void OpenParcourirOutput_Click(object sender, RoutedEventArgs e)
        {
            await SelectFile(2);
        }

        private async void OpenParcourirInput_Click(object sender, RoutedEventArgs e)
        {
            await SelectFile(1);
        }
    }
}
