using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SimplyPasswordWin10Shared.Context;
using SimplyPasswordWin10Shared.Enum;

namespace SimplyPasswordWin10.Views
{
    /// <summary>
    /// Vue pour la sélection d'un outil
    /// </summary>
    public sealed partial class OutilsView
    {

        public OutilsView()
        {
            InitializeComponent();
            GridTitre.Background = ContexteAppli.GetColorTheme();
        }
        

        private void OpenExport_Click(object sender, RoutedEventArgs e)
        {
            ((Frame)Window.Current.Content).Navigate(typeof(ImportExportView), ImportExportEnum.Export);
        }

        private void OpenImport_Click(object sender, RoutedEventArgs e)
        {
            ((Frame)Window.Current.Content).Navigate(typeof(ImportExportView), ImportExportEnum.Import);
        }

        private void OpenDechiffrer_Click(object sender, RoutedEventArgs e)
        {
            ((Frame)Window.Current.Content).Navigate(typeof(ChiffreDechiffreFichierView), ChiffrerDechiffrerEnum.Dechiffrer);
        }

        private void OpenChiffrer_Click(object sender, RoutedEventArgs e)
        {
            ((Frame)Window.Current.Content).Navigate(typeof(ChiffreDechiffreFichierView), ChiffrerDechiffrerEnum.Chiffrer);
        }

        private void OpenPartage_Click(object sender, RoutedEventArgs e)
        {
            ((Frame)Window.Current.Content).Navigate(typeof(PartageView), null);
        }

        private void OpenMesIcones_Click(object sender, RoutedEventArgs e)
        {
            ((Frame)Window.Current.Content).Navigate(typeof(GestionIconeView), null);
        }
    }
}
