using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using SimplyPasswordWin10Shared.Enum;
using SimplyPasswordWin10Shared.Model;

namespace SimplyPasswordWin10.ViewModel
{
    /// <summary>
    /// Prorités de la page d'édition d'un mot de passe
    /// </summary>
    public partial class EditPasswordViewModel
    {
        private ActionMotDePasseEnum _action;

        public ActionMotDePasseEnum Action
        {
            get { return _action; }

            private set
            {
                _action = value;
                OnPropertyChanged();
            }
        }

        private Dossier _dossierPossesseur;

        public Dossier DossierPossesseur
        {
            get { return _dossierPossesseur; }

            set
            {
                _dossierPossesseur = value;
                OnPropertyChanged();
            }
        }

        private MotDePasse _passwordOriginal;

        public MotDePasse PasswordOriginal
        {
            get { return _passwordOriginal; }

            set
            {
                _passwordOriginal = value;
                OnPropertyChanged();
            }
        }

        private string _titre;

        public string Titre
        {
            get { return _titre; }

            set
            {
                _titre = value;
                OnPropertyChanged();
            }
        }

        private string _login;

        public string Login
        {
            get { return _login; }

            set
            {
                _login = value;
                OnPropertyChanged();
            }
        }

        private string _password;

        public string Password
        {
            get { return _password; }

            set
            {
                _password = value;
                CalculerForceMotdePasse(value);
                OnPropertyChanged();
            }
        }

        private double _force;

        public double Force
        {
            get { return _force; }

            private set
            {
                _force = value;
                OnPropertyChanged();
            }
        }

        private string _site;

        public string Site
        {
            get { return _site; }

            set
            {
                _site = value;
                OnPropertyChanged();
            }
        }

        private string _commentaire;

        public string Commentaire
        {
            get { return _commentaire; }

            set
            {
                _commentaire = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Image> _listeIcone;

        public ObservableCollection<Image> ListeIcone
        {
            get { return _listeIcone; }

            private set
            {
                _listeIcone = value;
                OnPropertyChanged();
            }
        }

        private int _selectedIcone;

        public int SelectedIcone
        {
            get { return _selectedIcone; }

            set
            {
                _selectedIcone = value;
                OnPropertyChanged();
            }
        }

        private bool _isMdpVisible;

        public bool IsMdpVisible
        {
            get { return _isMdpVisible; }

            set
            {
                _isMdpVisible = value;
                OnPropertyChanged();
            }
        }
    }
}
