using Windows.Storage;
using SimplyPasswordWin10Shared.Enum;
using SimplyPasswordWin10Shared.Utils;

namespace SimplyPasswordWin10.ViewModel
{
    /// <summary>
    /// propriétés pour la page de chiffrement déchiffrement de fichiers
    /// </summary>
    public partial class ChiffreDechiffreFichierViewModel
    {
        public string Extension { get; set; }

        public string NomFichier { get; set; }

        private ChiffrerDechiffrerEnum _mode;

        public ChiffrerDechiffrerEnum Mode
        {
            get { return _mode; }

            set
            {
                _mode = value;
                OnPropertyChanged();
            }
        }

        private StorageFile _fileInput;

        public StorageFile FileInput
        {
            get { return _fileInput; }

            set
            {
                _fileInput = value;
                Extension = value != null ? StringUtils.GetExtension(value.Name) : null;
                NomFichier = value != null ? StringUtils.GetNomFichier(value.Name) : null;
                OnPropertyChanged();
            }
        }

        private StorageFile _fileOutput;

        public StorageFile FileOutput
        {
            get { return _fileOutput; }

            set
            {
                _fileOutput = value;
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

            set
            {
                _force = value;
                OnPropertyChanged();
            }
        }
    }
}
