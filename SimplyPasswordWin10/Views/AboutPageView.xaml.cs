using SimplyPasswordWin10Shared.Context;

namespace SimplyPasswordWin10.Views
{
    /// <summary>
    /// Page a propos de...
    /// </summary>
    public sealed partial class AboutPageView
    {

        public AboutPageView()
        {
            InitializeComponent();

            GridTitre.Background = ContexteAppli.GetColorTheme();

            NomAppli.Text = ContexteStatic.NomAppli;
            Version.Text = ContexteStatic.Version;
            Dev.Text = ContexteStatic.Developpeur;
        }
    }
}
