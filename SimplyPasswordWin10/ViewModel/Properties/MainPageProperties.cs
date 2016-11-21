using System.Collections.ObjectModel;
using SimplyPasswordWin10Shared.Model;

namespace SimplyPasswordWin10.ViewModel
{
    /// <summary>
    /// Prorités de la page principale
    /// </summary>
    public partial class MainPageViewModel
    {
        private string _titrePage;

        public string TitrePage
        {
            get { return _titrePage; }

            set
            {
                _titrePage = value;
                OnPropertyChanged();
            }
        }

        private Dossier _dossierSelected;

        public Dossier DossierSelected
        {
            get { return _dossierSelected; }

            set
            {
                _dossierSelected = value;
                OnPropertyChanged();
            }
        }

        private Dossier _dossierToEdit;

        public Dossier DossierToEdit
        {
            get { return _dossierToEdit; }

            set
            {
                _dossierToEdit = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Dossier> _listeDossier;

        public ObservableCollection<Dossier> ListeDossier
        {
            get { return _listeDossier; }

            set
            {
                _listeDossier = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<MotDePasse> _listeMotDePasse;

        public ObservableCollection<MotDePasse> ListeMotDePasse
        {
            get { return _listeMotDePasse; }

            set
            {
                _listeMotDePasse = value;
                OnPropertyChanged();
            }
        }

        private string _derniereSynchro;

        public string DerniereSynchro
        {
            get { return _derniereSynchro; }

            set
            {
                _derniereSynchro = value;
                OnPropertyChanged();
            }
        }

        private string _espaceOccupe;

        public string EspaceOccupe
        {
            get { return _espaceOccupe; }

            set
            {
                _espaceOccupe = value;
                OnPropertyChanged();
            }
        }

        private string _champsRecherche;

        public string ChampsRecherche
        {
            get { return _champsRecherche; }

            set
            {
                _champsRecherche = value;
                RechercheMotDePasse(value);
                OnPropertyChanged();
            }
        }

        private bool _isBackButtonVisible;

        public bool IsBackButtonVisible
        {
            get { return _isBackButtonVisible; }

            set
            {
                _isBackButtonVisible = value;
                OnPropertyChanged();
            }
        }

        private string _dossierToEditName;

        public string DossierToEditName
        {
            get { return _dossierToEditName; }

            set
            {
                _dossierToEditName = value;
                OnPropertyChanged();
            }
        }
    }
}
