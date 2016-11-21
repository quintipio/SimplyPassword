using SimplyPasswordWin10Shared.Enum;

namespace SimplyPasswordWin10.ViewModel
 {
    /// <summary>
    /// Les propriétés de la page de démarrage
    /// </summary>
    public partial class StartPageViewModel
    {

        private ModeOuvertureEnum _modeSelect;

        public ModeOuvertureEnum ModeSelect
        {
            get { return _modeSelect; }

            set
            {
                _modeSelect = value;
                OnPropertyChanged();
            }
        }

        private string _motDePasseA;

        public string MotDePasseA
        {
            get { return _motDePasseA; }

            set
            {
                _motDePasseA = value;
                ForceMdp = CalculForceMdp(value);
                OnPropertyChanged();
            }
        }

        private string _motDePasseB;

        public string MotDePasseB
        {
            get { return _motDePasseB; }

            set
            {
                _motDePasseB = value;
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

        private string _nomPage;

        public string NomPage
        {
            get { return _nomPage; }

            set
            {
                _nomPage = value;
                OnPropertyChanged();
            }
        }
    }
}
