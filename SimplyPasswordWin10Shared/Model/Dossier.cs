using System.Collections.Generic;
using System.Xml.Serialization;

namespace SimplyPasswordWin10Shared.Model
{
    /// <summary>
    /// Entité de dossier contenant des dossiers et des mots de passe
    /// </summary>
    [XmlRoot("Dossier")]
    public class Dossier
    {
        /// <summary>
        /// le nom du dossier
        /// </summary>
        [XmlElement("titre")]
        public string Titre { get; set; }
        /// <summary>
        /// le dossier parent possesseur (null pour le dossier racine)
        /// </summary>
        [XmlIgnore]
        public Dossier DossierParent { get; set; }
        /// <summary>
        /// les sous dossiers
        /// </summary>
        [XmlArray("sousDossier")]
        [XmlArrayItem("dossier")]
        public List<Dossier> SousDossier { get; set; }
        /// <summary>
        /// les mots de passes
        /// </summary>
        [XmlArray("listeMdp")]
        [XmlArrayItem("identifiant")]
        public List<MotDePasse> ListeMotDePasse { get; set; }
        /// <summary>
        /// pour une future evol, l'id d'une icone 
        /// </summary>
        [XmlElement("icone")]
        public int IdIcone { get; set; }

        /// <summary>
        /// constructeur vide
        /// </summary>
        public Dossier()
        {
            SousDossier = new List<Dossier>();
            ListeMotDePasse = new List<MotDePasse>();
        }

        /// <summary>
        /// constructeur pour un dossier avec les informations
        /// </summary>
        /// <param name="titre"> le nom du dossier</param>
        /// <param name="dossierParent">le dossier possesseur</param>
        public Dossier(string titre, Dossier dossierParent)
        {
            Titre = titre;
            DossierParent = dossierParent;
            SousDossier = new List<Dossier>();
            ListeMotDePasse = new List<MotDePasse>();
        }

        /// <summary>
        /// affiche le titre du dossier
        /// </summary>
        /// <returns>le titre du dossier</returns>
        public override string ToString()
        {
            return Titre;
        }
    }
}
