using System.Collections.ObjectModel;
using SimplyPasswordWin10Shared.Model;

namespace SimplyPasswordWin10.ViewModel
{
    /// <summary>
    /// Les propriétés de la page de récupération de mots de passe par le partage
    /// </summary>
    public partial class RecupPasswordViewModel
    {
        public bool ValidSave { get; set; }

        private bool _isLog;

        /// <summary>
        /// Indique si l'utilisateur a déchiffrer son fichier
        /// </summary>
        public bool IsLog
        {
            get { return _isLog; }

            set
            {
                _isLog = value;
                OnPropertyChanged();
            }
        }


        private ObservableCollection<MotDePasse> _listeMotDePasses;

        /// <summary>
        /// la liste des mots de passes déchiffré
        /// </summary>
        public ObservableCollection<MotDePasse> ListeMotDePasses
        {
            get { return _listeMotDePasses; }

            set
            {
                _listeMotDePasses = value;
                OnPropertyChanged();
            }
        }

        private MotDePasse _selectedMotDePasse;

        /// <summary>
        /// le mot de passe sélectionné pour un changement de dossier
        /// </summary>
        public MotDePasse SelectedMotDePasse
        {
            get { return _selectedMotDePasse; }

            set
            {
                _selectedMotDePasse = value;
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

        #region DLG password

        private string _password;

        public string Password
        {
            get { return _password; }

            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        #endregion

    }
}
