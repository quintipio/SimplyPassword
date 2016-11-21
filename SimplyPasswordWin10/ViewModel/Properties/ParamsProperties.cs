
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media;
using SimplyPasswordWin10Shared.Strings;

namespace SimplyPasswordWin10.ViewModel
{
    /// <summary>
    /// Prorités de la page des paramètres
    /// </summary>
    public partial class ParamsViewModel
    {
        private string _oldMdp;

        public string OldMdp
        {
            get { return _oldMdp; }

            set
            {
                _oldMdp = value;
                OnPropertyChanged();
            }
        }

        private string _newMdp;

        public string NewMdp
        {
            get { return _newMdp; }

            set
            {
                _newMdp = value;
                ForceMdp = CalculerForceMotdePasse(value);
                OnPropertyChanged();
            }
        }

        private string _confirmation;

        public string Confirmation
        {
            get { return _confirmation; }

            set
            {
                _confirmation = value;
                OnPropertyChanged();
            }
        }

        private double _forceMdp;

        public double ForceMdp
        {
            get { return _forceMdp; }

            set
            {
                _forceMdp = value;
                OnPropertyChanged();
            }
        }

        private bool? _cortanaActive;

        public bool? CortanaActive
        {
            get { return _cortanaActive; }

            set
            {
                _cortanaActive = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<ListeLangues.LanguesStruct> _listeLangue;

        public ObservableCollection<ListeLangues.LanguesStruct> ListeLangue
        {
            get { return _listeLangue; }

            set
            {
                _listeLangue = value;
                OnPropertyChanged();
            }
        }

        private ListeLangues.LanguesStruct _selectedLangue;

        public ListeLangues.LanguesStruct SelectedLangue
        {
            get { return _selectedLangue; }

            set
            {
                _selectedLangue = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<SolidColorBrush> _listeCouleur;

        public ObservableCollection<SolidColorBrush> ListeCouleur
        {
            get { return _listeCouleur; }

            set
            {
                _listeCouleur = value;
                OnPropertyChanged();
            }
        }
    }
}
