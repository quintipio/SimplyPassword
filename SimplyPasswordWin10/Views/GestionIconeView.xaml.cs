
using System;
using Windows.ApplicationModel.Resources;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SimplyPasswordWin10.Interface;
using SimplyPasswordWin10.ViewModel;
using SimplyPasswordWin10Shared.Context;
using SimplyPasswordWin10Shared.Utils;

namespace SimplyPasswordWin10.Views
{
    /// <summary>
    /// Code behind de la vue de gestion des icones
    /// </summary>
    public sealed partial class GestionIconeView : IView<GestionIconeViewModel>
    {
        /// <summary>
        /// Controleur
        /// </summary>
        public GestionIconeViewModel ViewModel { get; set; }



        public GestionIconeView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            GridTitre.Background = ContexteAppli.GetColorTheme();
            ViewModel = new GestionIconeViewModel();
        }

        private async void AjouterIcone_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var fileOpenPicker = new FileOpenPicker
                {
                    CommitButtonText = ResourceLoader.GetForCurrentView().GetString("phraseOK"),
                    ViewMode = PickerViewMode.List,
                    SuggestedStartLocation = PickerLocationId.Downloads,
                    FileTypeFilter = { ".jpg", ".jpeg", ".png", ".ico" }
                };

                //met en mémoire le fichier 
                var fichier = await fileOpenPicker.PickSingleFileAsync();
                if (fichier != null)
                {
                    var erreur = await ViewModel.AjouterIcone(fichier);
                    if (!string.IsNullOrWhiteSpace(erreur))
                    {
                        await MessageBox.ShowAsync(erreur);
                    }
                }
            }
            catch 
            {
                await MessageBox.ShowAsync(ResourceLoader.GetForCurrentView("Errors").GetString("erreurInconnu")); 
            }
        }

        private async void SupprimerIcone_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await ViewModel.SupprimerIcone((int)((Button)sender).Tag);
            }
            catch 
            {
                await MessageBox.ShowAsync(ResourceLoader.GetForCurrentView("Errors").GetString("erreurInconnu"));
            }
}
    }
}
