using System.Collections.ObjectModel;
using Windows.Storage;
using SimplyPasswordWin10Shared.Enum;
using SimplyPasswordWin10Shared.Model;

namespace SimplyPasswordWin10.ViewModel
{
    /// <summary>
    /// propriétés de la page d'import export
    /// </summary>
    public partial class ImportExportViewModel
    {

        /// <summary>
        /// Structure des différents formats existant
        /// </summary>
        public struct ExportFormat
        {
            public int Id { get; set; }
            public string Nom { get; set; }
            public string Format { get; set; }

            public ExportFormat(int id, string nom, string format)
                : this()
            {
                Id = id;
                Nom = nom;
                Format = format;
            }

            public override string ToString()
            {
                return Nom;
            }

            public override bool Equals(object obj)
            {
                if (obj is ExportFormat)
                {
                    return ((ExportFormat)obj).Id.Equals(Id);
                }
                return false;
            }

            public override int GetHashCode()
            {
                return Id.GetHashCode() * Nom.GetHashCode() * Format.GetHashCode();
            }
        }

        private Dossier _selectedDossier;

        public Dossier SelectedDossier
        {
            get { return _selectedDossier; }

            set
            {
                _selectedDossier = value;
                OnPropertyChanged();
            }
        }

        private Dossier _navigationDossier;

        public Dossier NavigationDossier
        {
            get { return _navigationDossier; }

            set
            {
                _navigationDossier = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<ExportFormat> _listeFormat;

        public ObservableCollection<ExportFormat> ListeFormat
        {
            get { return _listeFormat; }

            set
            {
                _listeFormat = value;
                OnPropertyChanged();
            }
        }

        private ExportFormat _formatChoisi;

        public ExportFormat FormatChoisi
        {
            get { return _formatChoisi; }

            set
            {
                _formatChoisi = value;
                OnPropertyChanged();
            }
        }

        private bool _ecraserDossier;

        public bool EcraserDossier
        {
            get { return _ecraserDossier; }

            set
            {
                _ecraserDossier = value;
                OnPropertyChanged();
            }
        }

        private StorageFile _fichier;

        public StorageFile Fichier
        {
            get { return _fichier; }

            set
            {
                _fichier = value;
                OnPropertyChanged();
            }
        }

        private ImportExportEnum _mode;

        public ImportExportEnum Mode
        {
            get { return _mode; }

            set
            {
                _mode = value;
                OnPropertyChanged();
            }
        }

        private string _mdpCypher;

        public string MdpCypher
        {
            get { return _mdpCypher; }

            set
            {
                _mdpCypher = value;
                OnPropertyChanged();
            }
        }


        #region Dlg Dossier

        private bool _isParentVisible;

        /// <summary>
        /// indique si le bouton dossier parent est visible ou non
        /// </summary>
        public bool IsParentVisible
        {
            get { return _isParentVisible; }

            set
            {
                _isParentVisible = value;
                OnPropertyChanged();
            }
        }

        private Dossier _dossierEncours;

        /// <summary>
        /// le dossier en cours pour la dlg des dossiers
        /// </summary>
        public Dossier DossierEncours
        {
            get { return _dossierEncours; }

            set
            {
                _dossierEncours = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Dossier> _listeDossierAffiche;

        /// <summary>
        /// La liste des dossiers à afficher dans la dlg de choix du dossier
        /// </summary>
        public ObservableCollection<Dossier> ListeDossierAffiche
        {
            get { return _listeDossierAffiche; }

            set
            {
                _listeDossierAffiche = value;
                OnPropertyChanged();
            }
        }
        #endregion
    }
}
