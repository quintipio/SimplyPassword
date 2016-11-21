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
    /// Vue pour le système de partage
    /// </summary>
    public sealed partial class PartageView : IView<PartageViewModel>
    {
        /// <summary>
        /// ViewModel
        /// </summary>
        public PartageViewModel ViewModel { get; set; }
        
        /// <summary>
        /// Constructeur
        /// </summary>
        public PartageView()
        {
            InitializeComponent();
            DataTransferManager.GetForCurrentView().DataRequested += MainPage_DataRequested;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel = new PartageViewModel();
            GridTitre.Background = ContexteAppli.GetColorTheme();
        }


        private void PartageView_OnLoaded(object sender, RoutedEventArgs e)
        {
                RadioSortieFichier.IsChecked = true;
                RadioChoixRecup.IsChecked = true;
        }

        private void ChoixMode_Checked(object sender, RoutedEventArgs e)
        {
            var rb = sender as RadioButton;

            if (rb != null)
            {
                var mode = rb.Tag.ToString();
                switch (mode)
                {
                    case "PartageMdp":
                        ViewModel.InOut = ModePartageInOutEnum.Partage;
                        GridMotDePasse.Visibility = Visibility.Visible;

                        TexteMdp.IsReadOnly = true;
                        CopieColleTextButton.Content = ResourceLoader.GetForCurrentView().GetString("textCopier");
                        AfficheEnvoiTextButton.Content = ResourceLoader.GetForCurrentView().GetString("textEnvoyer");
                        
                        GenereTextButton.Visibility = Visibility.Visible;
                        GenereAfficheFichierButton.Content = ResourceLoader.GetForCurrentView().GetString("textGenerer");
                        break;

                    case "Recup":
                        ViewModel.InOut = ModePartageInOutEnum.Recupération;
                        GridMotDePasse.Visibility = Visibility.Collapsed;

                        TexteMdp.IsReadOnly = false;
                        CopieColleTextButton.Content = ResourceLoader.GetForCurrentView().GetString("textColler");
                        AfficheEnvoiTextButton.Content = ResourceLoader.GetForCurrentView().GetString("textAfficher");
                        GenereTextButton.Visibility = Visibility.Collapsed;

                        GenereAfficheFichierButton.Content = ResourceLoader.GetForCurrentView().GetString("textAfficher");
                        break;
                }
            }
        }

        private  void ChoixCom_Checked(object sender, RoutedEventArgs e)
        {
            var rb = sender as RadioButton;

            if (rb != null)
            {
                var mode = rb.Tag.ToString();
                switch (mode)
                {
                    case "Texte":
                        ViewModel.Source = ModePartageSourceEnum.Texte;
                        GridFichier.Visibility = Visibility.Collapsed;
                        GridTexte.Visibility = Visibility.Visible;
                        break;
                    case "Fichier":
                        ViewModel.Source = ModePartageSourceEnum.Fichier;
                        GridFichier.Visibility = Visibility.Visible;
                        GridTexte.Visibility = Visibility.Collapsed;
                        break;

                }
            }
        }

        private async void OuvirFichier_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (ViewModel.InOut)
                {
                    //en cas d'import
                    case ModePartageInOutEnum.Recupération:

                        //choix du fichier à importer
                        var fileOpenPicker = new FileOpenPicker
                        {
                            CommitButtonText = ResourceLoader.GetForCurrentView().GetString("phraseOK"),
                            ViewMode = PickerViewMode.List,
                            SuggestedStartLocation = PickerLocationId.Downloads,
                            FileTypeFilter = {"."+ContexteStatic.ExtensionPartage},
                        };

                        //met en mémoire le fichier 
                        var fichier = await fileOpenPicker.PickSingleFileAsync();
                        if (fichier != null)
                        {
                            ViewModel.Fichier = fichier;
                            ViewModel.FichierChaine = fichier.Name;
                        }
                        break;

                    //en cas d'export
                    case ModePartageInOutEnum.Partage:
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
                            ViewModel.Fichier = fichierTmp;
                            ViewModel.FichierChaine = fichierTmp.Name;
                        }
                        break;
                }
            }
            catch 
            {
                await MessageBox.ShowAsync(ResourceLoader.GetForCurrentView("Errors").GetString("erreurSelectionFichier"));
            }
        }

        private async void OuvrirMdp_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.ListeMotDePasse == null ||
                (ViewModel.ListeMotDePasse != null && ViewModel.ListeMotDePasse.Count == 0))
            {
                ViewModel.ChargerMotsDePasse("");
            }
            await MdpContentDialog.ShowAsync();
        }


        #region dlg select mdp

        private void MdpContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            MdpContentDialog.Hide();
        }

        private void SearchMdp_KeyUp(object sender,KeyRoutedEventArgs e)
        {
            ViewModel.ChargerMotsDePasse(SearchBox.QueryText);
        }



        private void SelectUnSelectMdp_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var obj = sender as CheckBox;
            if (obj != null)
            {
                if (obj.IsChecked.HasValue && obj.IsChecked.Value)
                {
                    ViewModel.AjouterMotDePasse(((CheckBox)sender).Tag as MotDePasse);
                }
                else
                {
                    ViewModel.SupprimerMotDePasse(((CheckBox)sender).Tag as MotDePasse);
                }
            }
        }

        #endregion

        #region Action

        private async void GenereTextButton_OnClickTextButton_Click(object sender, RoutedEventArgs e)
        {
            var erreur = ViewModel.PartageTexte();
            if (!string.IsNullOrWhiteSpace(erreur))
            {
                await MessageBox.ShowAsync(erreur);
            }
        }

        private async void CopieColleTextButton_Click(object sender, RoutedEventArgs e)
        {
            switch (ViewModel.InOut)
            {
                case ModePartageInOutEnum.Partage:
                    if (!string.IsNullOrWhiteSpace(ViewModel.Texte))
                    {
                        try
                        {
                            var tmp = new DataPackage();
                            tmp.SetText(ViewModel.Texte);
                            Clipboard.SetContent(tmp);
                        }
                        catch 
                        {
                            //Ignored
                        }
                    }
                    break;

                case ModePartageInOutEnum.Recupération:
                    try
                    {
                        ViewModel.Texte = await Clipboard.GetContent().GetTextAsync();
                    }
                    catch
                    {
                        // ignored
                    }
                    break;

            }
        }


        private async void EnvoyerImporterTextButton_Click(object sender, RoutedEventArgs e)
        {
            switch (ViewModel.InOut)
            {
                case ModePartageInOutEnum.Partage:
                    DataTransferManager.ShowShareUI();
                    break;

                case ModePartageInOutEnum.Recupération:
                    var erreur = ViewModel.RecupTexte();
                    if (!string.IsNullOrEmpty(erreur))
                    {
                        await MessageBox.ShowAsync(erreur);
                    }

                    if (ViewModel.ListeMdpRecup != null && ViewModel.ListeMdpRecup.Count > 0)
                    {
                        ((Frame)Window.Current.Content).Navigate(typeof(RecupPasswordView), ViewModel.ListeMdpRecup);
                    }
                    break;
            }
        }



        private void MainPage_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            if (!string.IsNullOrEmpty(ViewModel.Texte))
            {
                args.Request.Data.SetText(ViewModel.Texte);
                args.Request.Data.Properties.Title = Windows.ApplicationModel.Package.Current.DisplayName;
            }
        }

        private async void GenereAfficheFichierButton_Click(object sender, RoutedEventArgs e)
        {
            switch (ViewModel.InOut)
            {
                case ModePartageInOutEnum.Partage:
                    var err = await ViewModel.PartageFichier();
                    if (!string.IsNullOrEmpty(err))
                    {
                        await MessageBox.ShowAsync(err);
                    }
                    break;

                case ModePartageInOutEnum.Recupération:
                    var errb = await ViewModel.RecupFichier();
                    if (!string.IsNullOrEmpty(errb))
                    {
                        await MessageBox.ShowAsync(errb);
                    }

                    if (ViewModel.ListeMdpRecup != null && ViewModel.ListeMdpRecup.Count > 0)
                    {
                        ((Frame)Window.Current.Content).Navigate(typeof(RecupPasswordView), ViewModel.ListeMdpRecup);
                    }
                    break;
            }
        }


        #endregion

    }
}
