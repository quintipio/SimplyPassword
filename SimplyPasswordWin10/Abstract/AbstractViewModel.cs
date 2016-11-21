using System.ComponentModel;
using System.Runtime.CompilerServices;
using SimplyPasswordWin10Shared.Context;
using SimplyPasswordWin10Shared.Model;

namespace SimplyPasswordWin10.Abstract
{
    /// <summary>
    /// Classe abstraite des ViewModel
    /// </summary>
    public abstract class AbstractViewModel: INotifyPropertyChanged
    {

        private static Dossier _selectedDossierAbstract;

        public static Dossier SelectedDossierAbstract
        {
            get { return _selectedDossierAbstract ?? ContexteAppli.DossierMere; }

            set
            {
                _selectedDossierAbstract = value;
            }
        }



        /// <summary>
        /// pour le InotifyPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
