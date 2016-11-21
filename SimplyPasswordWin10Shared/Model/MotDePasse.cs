using System;
using System.Xml.Serialization;
using Windows.UI.Xaml.Media.Imaging;

namespace SimplyPasswordWin10Shared.Model
{
    /// <summary>
    /// les données d'un compte de l'utilisateur
    /// </summary>
    [XmlRoot("MotDePasse")]
    public class MotDePasse
    {
        /// <summary>
        /// le titre du compte
        /// </summary>
        [XmlElement("titreMdp")]
        public string Titre { get; set; }
        /// <summary>
        /// l'identifiant du compte
        /// </summary>
        [XmlElement("login")]
        public string Login { get; set; }
        /// <summary>
        /// le mot de passe du compte
        /// </summary>
        [XmlElement("motDePasse")]
        public string MotDePasseObjet { get; set; }
        /// <summary>
        /// un éventuel commentaire
        /// </summary>
        [XmlElement("commentaire")]
        public string Commentaire { get; set; }
        /// <summary>
        /// le dossier dans lequel se situe le mot de passe
        /// </summary>
        [XmlIgnore]
        public Dossier DossierPossesseur { get; set; }
        
        /// <summary>
        /// pour une future évol, l'id d'une icone
        /// </summary>
        [XmlElement("icone")]
        public int IdIcone { get; set; }

        /// <summary>
        /// pour une future évol, l'id d'une icone
        /// </summary>
        [XmlElement("web")]
        public string SiteWeb { get; set; }

        /// <summary>
        /// image (juste pour l'affichage dans la liste)
        /// </summary>
        [XmlIgnore]
        public BitmapImage Icone { get; set; }

        /// <summary>
        /// Indique dans le système de partage si le mot de passe est sélectionné
        /// </summary>
        [XmlIgnore]
        public bool Selected { get; set; }
        
        /// <summary>
        /// constructeur vide
        /// </summary>
        public MotDePasse()
        {

        }

        /// <summary>
        /// constructeur avec des données prête à être fournis
        /// </summary>
        /// <param name="titre">le titre du compte</param>
        /// <param name="login">l'identifiant</param>
        /// <param name="motDePasse">le mot de passe</param>
        /// <param name="dossierPossesseur">le dossier dans lequel se trouve le mot de passe</param>
        /// <param name="commentaire">un éventule commentaire</param>
        /// <param name="idIcone">l'id de l'icone</param>
        /// <param name="siteWeb">l'id de l'icone l'éventuel adresse web du mot de passe</param>
        public MotDePasse(string titre, string login, string motDePasse,Dossier dossierPossesseur, string commentaire,int idIcone, string siteWeb)
        {
            Titre = titre;
            Login = login;
            MotDePasseObjet = motDePasse;
            DossierPossesseur = dossierPossesseur;
            Commentaire = commentaire;
            IdIcone = idIcone;
            SiteWeb = siteWeb;
        }

        /// <summary>
        /// affiche le titre du compte
        /// </summary>
        /// <returns>le titre du compte</returns>
        public override string ToString()
        {
            return Titre;
        }
    }
}
