using System.Collections.Generic;

namespace SimplyPasswordWin10Shared.Context
{
    /// <summary>
    /// Classe contenant des informations fixe et ne devant pas être modifié nécéssaire dans toute l'appli
    /// </summary>
    public static class ContexteStatic
    {
        /// <summary>
        /// le nom de l'application
        /// </summary>
        public const string NomAppli = "Simply password";

        /// <summary>
        /// adresse de support
        /// </summary>
        public const string Support = "xxxx@xxx.fr";

        /// <summary>
        /// version de l'application
        /// </summary>
        public const string Version = "2.2.1";

        /// <summary>
        ///  Le nom du dossier local contenant les icones
        /// </summary>
        public const string NomDossierIcone = "icons";

        /// <summary>
        /// Nom du fichier des paramètres
        /// </summary>
        public const string NomFichierParam = "param";

        /// <summary>
        /// Nom du fichier pour la récupération du mot de passe
        /// </summary>
        public const string NomFichierImageUnlock = "UnlockSecure";

        /// <summary>
        /// Le nom du fichier contenant le mot de passe de l'utilisateur pour Cortana
        /// </summary>
        public const string NomFichierMotDePasseUser = "hash";

        /// <summary>
        /// le nom par défaut du fichier chiffré
        /// </summary>
        public const string NomFichierPassword = "MyPassword";

        /// <summary>
        /// nom du développeur
        /// </summary>
        public const string Developpeur = "XXXXX";

        /// <summary>
        /// l'extension par défaut des fichiers chiffré de l'appli
        /// </summary>
        public const string Extension = "kwi";

        /// <summary>
        /// l'extension par défaut des fichiers de partage
        /// </summary>
        public const string ExtensionPartage = "kwip";

        /// <summary>
        /// temps de mise en mémoire des données dans le presse papier en milliseconde
        /// </summary>
        public const int WaitBeforeDelPassword = 20000;

        /// <summary>
        /// nombre de caractère lors de la génération aléatoire d'un mot de passe
        /// </summary>
        public const int NbCaractereGenerePassword = 8;

        /// <summary>
        /// liste des couleurs les plus sombres applicables pour le theme de la version windows 8
        /// </summary>
        public static readonly List<uint> ListeCouleur = new List<uint> { 0xFF610000, 0xFF613E00, 0xFF616100, 0xFF0D6100, 0xFF00613F, 0xFF00615D, 0xFF001661, 0xFF4F0061, 0xFF610039 };
        
        /// <summary>
        /// pour windows phone 10, il est à 0, donc solution de remplacement
        /// </summary>
        public const ulong RoaminsStorageQuotaBis = 100;

        /// <summary>
        /// Le poid max en octets des fichiers pouvant être chiffrer/déchiffrer
        /// </summary>
        public const int MaxSizeFichierChiffrer = 10000000;
    }
}
