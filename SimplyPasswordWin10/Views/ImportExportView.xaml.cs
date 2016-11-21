using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using SimplyPasswordWin10.Abstract;
using SimplyPasswordWin10.Interface;
using SimplyPasswordWin10.ViewModel;
using SimplyPasswordWin10Shared.Business;
using SimplyPasswordWin10Shared.Context;
using SimplyPasswordWin10Shared.Enum;
using SimplyPasswordWin10Shared.Model;
using SimplyPasswordWin10Shared.Utils;

namespace SimplyPasswordWin10.Views
{
    /// <summary>
    /// Vue pour l'import ou l'export
    /// </summary>
    public sealed partial class ImportExportView : IView<ImportExportViewModel>
    {

        /// <summary>
        /// controleur
        /// </summary>
        public ImportExportViewModel ViewModel { get; set; }


        private readonly DispatcherTimer _timerCapsLock;

        /// <summary>
        /// constructeur
        /// </summary>
        public ImportExportView()
        {
            InitializeComponent();
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


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel = new ImportExportViewModel();
            GridTitre.Background = ContexteAppli.GetColorTheme();

            if (e.Parameter is ImportExportEnum)
            {
                ViewModel.Mode = ((ImportExportEnum)e.Parameter);
                ViewModel.SelectedDossier = AbstractViewModel.SelectedDossierAbstract;

                switch (ViewModel.Mode)
                {
                    case ImportExportEnum.Import:
                        TitrePage.Text = ResourceLoader.GetForCurrentView().GetString("phraseImporter");
                        DossierTextBox.Text = ResourceLoader.GetForCurrentView().GetString("phraseExpImport");
                        FormatComboBox.IsEnabled = false;
                        EraseCheckBox.Visibility = Visibility.Visible;
                        break;

                    case ImportExportEnum.Export:
                        TitrePage.Text = ResourceLoader.GetForCurrentView().GetString("phraseExporter");
                        DossierTextBox.Text = ResourceLoader.GetForCurrentView().GetString("phraseExpExport");
                        break;
                }
                AfficherResultatOutil(false);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _timerCapsLock.Stop();
            base.OnNavigatedFrom(e);
        }


        public void AfficherResultatOutil(bool isFile)
        {
            var toAffiche = "";
            const string fleche = " --> ";
            const string inconnu = "???";
            if (ViewModel.Mode.Equals(ImportExportEnum.Export))
            {
                toAffiche = ViewModel.SelectedDossier.Titre + fleche;
                if (isFile)
                {
                    toAffiche += ViewModel.Fichier.Name;
                }
                else
                {
                    toAffiche += inconnu;
                }
            }

            if (ViewModel.Mode.Equals(ImportExportEnum.Import))
            {
                if (isFile)
                {
                    toAffiche += ViewModel.Fichier.Name;
                }
                else
                {
                    toAffiche += inconnu;
                }
                toAffiche += fleche + ViewModel.SelectedDossier.Titre;
            }
            EmplacementFichierTextBlock.Text = toAffiche;
        }

        

        #region évènements

        private void formatComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(FormatComboBox.SelectedItem is ImportExportViewModel.ExportFormat)
            {
                if(((ImportExportViewModel.ExportFormat)FormatComboBox.SelectedItem).Id.Equals(3))
                {
                    MdpTextBlock.Visibility = Visibility.Visible;
                    MdpTextbox.Visibility = Visibility.Visible;
                }
                else
                {
                    MdpTextBlock.Visibility = Visibility.Collapsed;
                    MdpTextbox.Visibility = Visibility.Collapsed;
                    MdpTextbox.Password = "";
                }
            }
        }

        private async void parcourirButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (ViewModel.Mode)
                {
                    //en cas d'import
                    case ImportExportEnum.Import:

                        //choix du fichier à importer
                        var fileOpenPicker = new FileOpenPicker
                        {
                            CommitButtonText = ResourceLoader.GetForCurrentView().GetString("phraseOK"),
                            ViewMode = PickerViewMode.List,
                            SuggestedStartLocation = PickerLocationId.Downloads,
                            FileTypeFilter = {".kwi", ".xml", ".csv"},
                        };

                        //met en mémoire le fichier 
                        var fichier = await fileOpenPicker.PickSingleFileAsync();
                        if (fichier != null)
                        {
                            ViewModel.Fichier = fichier;
                            var extension = StringUtils.GetExtension(ViewModel.Fichier.Name);
                            foreach (var format in ViewModel.ListeFormat.Where(format => format.Format.Equals(extension))
                                )
                            {
                                FormatComboBox.SelectedItem = format;
                            }
                            AfficherResultatOutil(true);
                        }
                        break;

                    //en cas d'export
                    case ImportExportEnum.Export:
                        if (FormatComboBox.SelectedItem != null)
                        {
                            var extensionChoisi = "." +
                                                  ((ImportExportViewModel.ExportFormat) FormatComboBox.SelectedItem)
                                                      .Format;
                            var listeExtension = new List<string> {extensionChoisi};

                            //recherche du fichier à exporter
                            var fileSavePicker = new FileSavePicker
                            {
                                CommitButtonText = ResourceLoader.GetForCurrentView().GetString("phraseOK"),
                                SuggestedFileName = "myPassword",
                                SuggestedStartLocation = PickerLocationId.Downloads,
                                DefaultFileExtension = extensionChoisi,

                            };

                            //mise en mémoire du fichier
                            fileSavePicker.FileTypeChoices.Add("myPassword", listeExtension);
                            var fichierTmp = await fileSavePicker.PickSaveFileAsync();
                            if (fichierTmp != null)
                            {
                                ViewModel.Fichier = fichierTmp;
                                TextBlockErreur.Text = "";
                                AfficherResultatOutil(true);
                            }
                        }
                        else
                        {
                            TextBlockErreur.Text = ResourceLoader.GetForCurrentView().GetString("phraseDemandeFormat");
                        }
                        break;
                }
            }
            catch
            {
                TextBlockErreur.Text = ResourceLoader.GetForCurrentView("Errors").GetString("erreurSelectionFichier");
            }
        }

        private async void ChangeDossierButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NaviguerDossier(ContexteAppli.DossierMere);
            await SelectDossierContentDialog.ShowAsync();
        }

        private async void validerButton_Click(object sender, RoutedEventArgs e)
        {
            WaitRing.IsActive = true;
            ValiderButton.IsEnabled = false;
            ParcourirButton.IsEnabled = false;
            ChangerDossierButton.IsEnabled = false;
            EraseCheckBox.IsEnabled = false;
            MdpTextbox.IsEnabled = false;
            var erreur = "";
            try
            {
                //vérification du format
                if (FormatComboBox.SelectedItem != null)
                {
                    ViewModel.FormatChoisi = ((ImportExportViewModel.ExportFormat)FormatComboBox.SelectedItem);
                }
                else
                {
                    erreur = ResourceLoader.GetForCurrentView("Errors").GetString("erreurAucunFormat");
                }

                ViewModel.MdpCypher = MdpTextbox.Password;
                //ViewModel.SelectedDossier = ContexteAppli.PageViewMere.GetDossierSelected();
                
                switch (ViewModel.Mode)
                {
                    case ImportExportEnum.Import:
                        switch (ViewModel.FormatChoisi.Id)
                        {
                            case 1:
                                erreur = await ViewModel.ImporterCsv();
                                break;

                            case 2:
                                erreur = await ViewModel.ImporterXml();
                                break;

                            case 3:
                                erreur = await ViewModel.ImporterKwi();
                                break;
                        }
                        break;

                    case ImportExportEnum.Export:
                        switch (ViewModel.FormatChoisi.Id)
                        {
                            case 1:
                                erreur = await ViewModel.ExporterCsv();
                                break;

                            case 2:
                                erreur = await ViewModel.ExporterXml();
                                break;

                            case 3:
                                erreur = await ViewModel.ExporterKwi();
                                break;
                        }
                        break;
                }

                if (!string.IsNullOrWhiteSpace(erreur))
                {
                    TextBlockErreur.Text = erreur;
                }
                else
                {
                    await PasswordBusiness.Save();

                    //mise à jour de la liste de cortana
                    await CortanaBusiness.UpdateCortana();

                    switch (ViewModel.Mode)
                    {
                        case ImportExportEnum.Import:
                            await MessageBox.ShowAsync(ResourceLoader.GetForCurrentView().GetString("textImportOK"), "", MessageBoxButton.Ok);
                            break;
                        case ImportExportEnum.Export:
                            await MessageBox.ShowAsync(ResourceLoader.GetForCurrentView().GetString("textExportOK"), "", MessageBoxButton.Ok);
                            break;
                    }
                    ((Frame)Window.Current.Content).Navigate(typeof(MainPageView));
                }

            }
            catch
            {
                TextBlockErreur.Text = ResourceLoader.GetForCurrentView("Errors").GetString("erreurInconnu");
            }
            WaitRing.IsActive = false;
            ValiderButton.IsEnabled = true;
            ParcourirButton.IsEnabled = true;
            ChangerDossierButton.IsEnabled = true;
            EraseCheckBox.IsEnabled = true;
            MdpTextbox.IsEnabled = true;
        }
        #endregion

        private void EraseCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.EcraserDossier = true;
        }

        private void EraseCheckBox_UnChecked(object sender, RoutedEventArgs e)
        {
            ViewModel.EcraserDossier = false;
        }


        #region Dlg Dossier
        private void ChoixDossier_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            ViewModel.ChangerEmplacementDossier();
            SelectDossierContentDialog.Hide();
            AfficherResultatOutil(ViewModel.Fichier != null);
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
