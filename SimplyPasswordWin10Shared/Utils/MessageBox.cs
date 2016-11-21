using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;

namespace SimplyPasswordWin10Shared.Utils
{
    /// <summary>
    /// permet d'afficher simplement une messageBox
    /// </summary>
    public static class MessageBox
    {
        /// <summary>
        /// montre une messageBox personalisé
        /// </summary>
        /// <param name="messageBoxText">le texte à afficher</param>
        /// <param name="caption">le titre de la messageBox</param>
        /// <param name="button">les boutons à afficher</param>
        /// <returns></returns>
        public static async Task<MessageBoxResult> ShowAsync(string messageBoxText,
                                                             string caption,
                                                             MessageBoxButton button) {
 
            var md = new MessageDialog(messageBoxText, caption);
            var result = MessageBoxResult.None;
            if (button.HasFlag(MessageBoxButton.Ok)) {
                md.Commands.Add(new UICommand(ResourceLoader.GetForCurrentView().GetString("phraseOK"),
                    cmd => result = MessageBoxResult.Ok));
            }
            if (button.HasFlag(MessageBoxButton.Yes)) {
                md.Commands.Add(new UICommand(ResourceLoader.GetForCurrentView().GetString("phraseOui"),
                    cmd => result = MessageBoxResult.Yes));
            }
            if (button.HasFlag(MessageBoxButton.No)) {
                md.Commands.Add(new UICommand(ResourceLoader.GetForCurrentView().GetString("phraseNon"),
                    cmd => result = MessageBoxResult.No));
            }
            if (button.HasFlag(MessageBoxButton.Cancel)) {
                md.Commands.Add(new UICommand(ResourceLoader.GetForCurrentView().GetString("phraseAnnuler"),
                    cmd => result = MessageBoxResult.Cancel));
                md.CancelCommandIndex = (uint)md.Commands.Count - 1;
            }
            await md.ShowAsync();
            return result;
        }
 
        public static async Task<MessageBoxResult> ShowAsync(string messageBoxText) {
            return await ShowAsync(messageBoxText, "", MessageBoxButton.Ok);
        }
    }
 
    /// <summary>
    /// Les différents boutons possible à afficher
    /// </summary>
    [Flags]
    public enum MessageBoxButton {
        
        Ok = 1,
        Cancel = 2,
        OkCancel = Ok | Cancel,
        Yes = 4,
        No = 8,
        YesNo = Yes | No,
    }
 
   /// <summary>
   /// Définit les réponses de l'utilisateur
   /// </summary>
    public enum MessageBoxResult {
        None = 0,
        Ok = 1,
        Cancel = 2,
        Yes = 6,
        No = 7,
    }
}
