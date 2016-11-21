
using System.Collections.ObjectModel;
using SimplyPasswordWin10Shared.Model;

namespace SimplyPasswordWin10.ViewModel
{
    /// <summary>
    /// Prorités de la page d'affichage d'un mot de passe
    /// </summary>
    public partial class AfficherPasswordViewModel
    {
        private MotDePasse _password;

        public MotDePasse Password
        {
            get { return _password; }

            private set
            {
                _password = value;
                OnPropertyChanged();
            }
        }
        
        private bool _passwordReveal;

        public bool PasswordReveal
        {
            get { return _passwordReveal; }

            set
            {
                _passwordReveal = value;
                GenererMotDePasseAffiche(value);
                OnPropertyChanged();
            }
        }

        private string _passwordToAffich;

        public string PasswordToAffich
        {
            get { return _passwordToAffich; }

            private set
            {
                _passwordToAffich = value;
                OnPropertyChanged();
            }
        }

        private bool _isCommentVisible;

        public bool IsCommentVisible
        {
            get { return _isCommentVisible; }

            private set
            {
                _isCommentVisible = value;
                OnPropertyChanged();
            }
        }

        private bool _isSiteVisible;

        public bool IsSiteVisible
        {
            get { return _isSiteVisible; }

            private set
            {
                _isSiteVisible = value;
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
