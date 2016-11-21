using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SimplyPasswordWin10.Views;
using SimplyPasswordWin10Shared.Business;
using SimplyPasswordWin10Shared.Context;
using SimplyPasswordWin10Shared.Enum;
using SimplyPasswordWin10Shared.Utils;

namespace SimplyPasswordWin10
{
    /// <summary>
    /// Coeur de l'application
    /// </summary>
    partial class App
    {
        /// <summary>
        /// Constructeur
        /// </summary>
        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            InitializeComponent();
            Suspending += OnSuspending;
            ApplicationData.Current.DataChanged += Current_DataChanged;
        }
        

        public async void Current_DataChanged(ApplicationData sender, object args)
        {
                if (!string.IsNullOrWhiteSpace(PasswordBusiness.Password))
                {
                    await PasswordBusiness.Load(PasswordBusiness.Password,true);
                }
        }


        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            await LaunchApp(null,null);
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            // Each time a navigation event occurs, update the Back button's visibility
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                ((Frame)sender).CanGoBack ?
                AppViewBackButtonVisibility.Visible :
                AppViewBackButtonVisibility.Collapsed;
        }
        
        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;

            if (rootFrame != null && rootFrame.CanGoBack)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

        protected override async void OnActivated(IActivatedEventArgs e)
        {
            base.OnActivated(e);
            //Si activation par l'arrière plan
            if (e.Kind == ActivationKind.Protocol)
            {
                var commandArgs = e as ProtocolActivatedEventArgs;
                if (commandArgs != null)
                {
                    Windows.Foundation.WwwFormUrlDecoder decoder =
                        new Windows.Foundation.WwwFormUrlDecoder(commandArgs.Uri.Query);
                    var site = decoder.GetFirstValueByName("LaunchContext");
                    await LaunchApp(site,null);
                }
                else
                {
                    await LaunchApp(null,null);
                }
            }
        }


        protected override async void OnFileActivated(FileActivatedEventArgs args)
        {
            if (args.Kind == ActivationKind.File)
            {
                var file = (StorageFile)args.Files[0];
                await LaunchApp(null, file);
            }

        }



        /// <summary>
        /// Lance l'application
        /// </summary>
        /// <param name="recherche">la recherche commandé par cortana sinon null</param>
        /// <param name="fichier">le fichier à ouvrir dans la page de partage</param>
        /// <returns></returns>
        private async Task LaunchApp(string recherche,StorageFile fichier)
        {
            var rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;
                rootFrame.Navigated += OnNavigated;

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;

                // Register a handler for BackRequested events and set the
                // visibility of the Back button
                SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    rootFrame.CanGoBack ?
                    AppViewBackButtonVisibility.Visible :
                    AppViewBackButtonVisibility.Collapsed;
            }

            if (rootFrame.Content == null)
            {
                
                await ContexteAppli.Initialize(true,fichier);
                bool ouvertureReussi;

                if (fichier != null)
                {
                    if (StringUtils.GetExtension(fichier.Name) == ContexteStatic.Extension)
                    {
                        ouvertureReussi = rootFrame.Navigate(typeof(StartPageView), ModeOuvertureEnum.FichierDejaExistant);
                    }
                    else if (StringUtils.GetExtension(fichier.Name) == ContexteStatic.ExtensionPartage)
                    {
                        if (await PasswordBusiness.DoesFileCypherExist())
                        {
                            ouvertureReussi = rootFrame.Navigate(typeof (RecupPasswordView), fichier);
                        }
                        else
                        {
                            ouvertureReussi = rootFrame.Navigate(typeof (StartPageView), ModeOuvertureEnum.FichierACreer);
                        }
                    }
                    else
                    {
                        ouvertureReussi = false;
                    }
                }
                else
                {
                    if (await PasswordBusiness.DoesFileCypherExist())
                    {
                        if (!string.IsNullOrWhiteSpace(recherche))
                        {
                            ouvertureReussi = rootFrame.Navigate(typeof(ResultCortanaView), recherche);
                        }
                        else
                        {
                            ouvertureReussi = rootFrame.Navigate(typeof(StartPageView), ModeOuvertureEnum.FichierDejaExistant);
                        }
                    }
                    else
                    {
                        ouvertureReussi = rootFrame.Navigate(typeof(StartPageView), ModeOuvertureEnum.FichierACreer);
                    }
                }

                if (!ouvertureReussi)
                {
                    throw new Exception(ResourceLoader.GetForCurrentView("Errors").GetString("erreurDemarrage"));
                }
                Window.Current.Activate();
            }
        }
    }
}
