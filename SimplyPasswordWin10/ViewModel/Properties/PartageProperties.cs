
using System.Collections.ObjectModel;
using Windows.Storage;
using SimplyPasswordWin10Shared.Enum;
using SimplyPasswordWin10Shared.Model;

namespace SimplyPasswordWin10.ViewModel
{
    /// <summary>
    /// page des propriétés de la page de partage 
    /// </summary>
    public partial class PartageViewModel
    {

        /// <summary>
        /// Clé pour le chiffrement déchiffrement du mot de passe partagé
        /// </summary>
        public static readonly string CryptKey = "p55sbev7/2>tV8m7^]Fm4#4jJp%),a3fCpxE.E8?Fd{a=yE*g2R/yt(yG6~vu<fK,^5eP9?~EV8$Bm8kc3L8X9.d7)bT#VH9JAjJ44!t279fR53M?3>rLX8.TmX77Y52)mTT5H7Ac27^mK99R+U@F@3Ac{-45n*r@PkJ4Y3Mg5sw2pr8CC9)95s9]Q4.~g*g2,m4t2_*95AT%C[KK7U;uA>^PgLLdU>}/aij&Luyf&~,3;6TX$&e_Z45;2E^SzyH";

        /// <summary>
        /// Salt pour le chiffrement déchiffrement du mot de passe partagé
        /// </summary>
        public static readonly string CryptSalt = "835/Y7Pu42b+3cR!:63=dc9,W=2!9SseYW3/ek[E(MSND#%t2NVt-)fe{39$>XPcMR)D@Pyw_v6h36C)4Gn:Kw8>mH:r84@2LYRB7L]9{U4hsX4bH]D,6~=79Ni^65*cc(yaQWWEs@=*f#Vv&?4(4v3?n]+9TP9TxAF4$4v*28_4$yAHAa6>F[2-md3h87GrA@E$8[z2KAL&qBc@h3Xhuq53w:vXf4Edzg2~ca?/99&f6iH%J7y579XQzf/7Tv3R";

        /// <summary>
        /// une liste de mot de passe rempli lors de la récupération
        /// </summary>
        public ObservableCollection<MotDePasse> ListeMdpRecup { get; set; }

        private ModePartageInOutEnum _inOut;

        public ModePartageInOutEnum InOut
        {
            get { return _inOut; }

            set
            {
                _inOut = value;
                OnPropertyChanged();
            }
        }

        private ModePartageSourceEnum _source;

        public ModePartageSourceEnum Source
        {
            get { return _source; }

            set
            {
                _source = value;
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

        private string _texte;

        public string Texte
        {
            get { return _texte; }

            set
            {
                _texte = value;
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

        private ObservableCollection<MotDePasse> _listeMotDePasseSelected;

        public ObservableCollection<MotDePasse> ListeMotDePasseSelected
        {
            get { return _listeMotDePasseSelected; }

            set
            {
                _listeMotDePasseSelected = value;
                OnPropertyChanged();
            }
        }

        private string _listeMotDePasseSelectedChaine;

        public string ListeMotDePasseSelectedChaine
        {
            get { return _listeMotDePasseSelectedChaine; }

            set
            {
                _listeMotDePasseSelectedChaine = value;
                OnPropertyChanged();
            }
        }

        private string _fichierChaine;

        public string FichierChaine
        {
            get { return _fichierChaine; }

            set
            {
                _fichierChaine = value;
                OnPropertyChanged();
            }
        }



    }
}
