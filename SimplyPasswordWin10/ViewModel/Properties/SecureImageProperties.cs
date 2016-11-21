using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml.Media.Imaging;
using SimplyPasswordWin10Shared.Enum;

namespace SimplyPasswordWin10.ViewModel
{
    /// <summary>
    /// Propriétés pour la vue de la gestion de l'image de déblocage de l'appli
    /// </summary>
    public partial class SecureImageViewModel
    {

        private readonly double _maxSizeX = 640.0;
        private readonly double _maxSizeY = 480.0;

        private OuvertureSecureImageEnum _mode;

        /// <summary>
        /// Le mode d'utilisation de la vue
        /// </summary>
        public OuvertureSecureImageEnum Mode
        {
            get { return _mode; }

            set
            {
                _mode = value;
                OnPropertyChanged();
            }
        }

        private BitmapImage _imageLock;

        /// <summary>
        /// L'image de dévérrouillage à afficher
        /// </summary>
        public BitmapImage ImageLock
        {
            get { return _imageLock; }

            set
            {
                _imageLock = value;
                OnPropertyChanged();
            }
        }

        private byte[] _imageToSave;

        private bool _etapeUnOk;

        /// <summary>
        /// Indique si l'étape un est valide (chargement de l'image)
        /// </summary>
        public bool EtapeUnOk
        {
            get { return _etapeUnOk; }

            set
            {
                _etapeUnOk = value;
                OnPropertyChanged();
            }
        }

        private string _erreurEtapeUn;

        /// <summary>
        /// Les erreurs à afficher liées à l'étape 1
        /// </summary>
        public string ErreurEtapeUn
        {
            get { return _erreurEtapeUn; }

            set
            {
                _erreurEtapeUn = value;
                OnPropertyChanged();
            }
        }

        private bool _etapeDeuxOk;

        /// <summary>
        /// Indique si l'étape deux est valide (mise en place des points)
        /// </summary>
        public bool EtapeDeuxOk
        {
            get { return _etapeDeuxOk; }

            set
            {
                _etapeDeuxOk = value;
                OnPropertyChanged();
            }
        }

        private string _erreurEtapeDeux;

        /// <summary>
        /// Les erreurs à afficher liées à l'étape 1
        /// </summary>
        public string ErreurEtapeDeux
        {
            get { return _erreurEtapeDeux; }

            set
            {
                _erreurEtapeDeux = value;
                OnPropertyChanged();
            }
        }

        private bool _nbPointsOk;

        public bool NbPointsOk
        {
            get { return _nbPointsOk; }

            set
            {
                _nbPointsOk = value;
                OnPropertyChanged();
            }
        }

        private List<Point> _listePoint;

        /// <summary>
        /// La liste des points sur l'image
        /// </summary>
        public List<Point> ListePoint
        {
            get { return _listePoint; }

            set
            {
                _listePoint = value;
                OnPropertyChanged();
            }
        }

        private List<Point> _listePointToVerif;

        /// <summary>
        /// la liste des points entrés lors de la vérification de la clé
        /// </summary>
        public List<Point> ListePointToVerif
        {
            get { return _listePointToVerif; }

            set
            {
                _listePointToVerif = value;
                OnPropertyChanged();
            }
        }

        private string _erreurVerif;

        /// <summary>
        /// message d'erreur lors de la vérification du mot de passe
        /// </summary>
        public string ErreurVerif
        {
            get { return _erreurVerif; }

            set
            {
                _erreurVerif = value;
                OnPropertyChanged();
            }
        }
    }
}
