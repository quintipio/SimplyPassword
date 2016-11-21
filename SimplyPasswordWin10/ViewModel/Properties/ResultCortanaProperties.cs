using System.Collections.ObjectModel;
using SimplyPasswordWin10Shared.Model;

namespace SimplyPasswordWin10.ViewModel
{
    /// <summary>
    /// page ds propriétés de la vue pour cortana
    /// </summary>
    public partial class ResultCortanaViewModel
    {
        private string _nomPage;

        /// <summary>
        /// le nom de la page
        /// </summary>
        public string NomPage
        {
            get { return _nomPage; }

            set
            {
                _nomPage = value;
                OnPropertyChanged();
            }
        }

        private string _motDePasse;

        /// <summary>
        /// le mot de passe de déchiffrement du fichier entré apr l'utilisateur
        /// </summary>
        public string MotDePasse
        {
            get { return _motDePasse; }

            set
            {
                _motDePasse = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<MotDePasse> _listeMotDePasse;
        /// <summary>
        /// La liste des mots de passes à afficher
        /// </summary>
        public ObservableCollection<MotDePasse> ListeMotDePasse
        {
            get { return _listeMotDePasse; }

            set
            {
                _listeMotDePasse = value;
                OnPropertyChanged();
            }
        }

        private string _recherche;
    }
}
