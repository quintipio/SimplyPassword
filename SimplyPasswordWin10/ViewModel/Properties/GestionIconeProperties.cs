
using System.Collections.Generic;
using Windows.UI.Xaml.Media.Imaging;

namespace SimplyPasswordWin10.ViewModel
{
    /// <summary>
    /// Les propriétés de la vue de gestion des icones
    /// </summary>
    public partial class GestionIconeViewModel
    {


        private Dictionary<int,BitmapImage> _listeIconePerso;

        /// <summary>
        /// La liste des icones de l'utilisateur
        /// </summary>
        public Dictionary<int, BitmapImage> ListeIconePerso
        {
            get { return _listeIconePerso; }

            set
            {
                _listeIconePerso = value;
                OnPropertyChanged();
            }
        }

    }
}
