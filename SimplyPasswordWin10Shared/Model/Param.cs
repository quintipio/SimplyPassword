using System.Xml.Serialization;
using Windows.UI.Xaml.Media.Imaging;

namespace SimplyPasswordWin10Shared.Model
{
    [XmlRoot("Param")]
    public class Param
    {
        /// <summary>
        /// le code en deux lettres de la langue utilisé
        /// </summary>
        [XmlElement("langue")]
        public int IdLangue { get; set; }

        /// <summary>
        /// l'index de la liste de colorStatic de la couleur
        /// </summary>
        [XmlElement("color")]
        public int EmplacementListeCouleur { get; set; }

        [XmlElement("cortana")]
        public bool IsCortanaActive { get; set; }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="codeLangue">le code en deux lettres de la langue utilisé</param>
        /// <param name="emplacementListeCouleur">l'index de la liste de colorStatic de la couleur</param>
        /// <param name="cortanaActive">true si cortana peut accéder aux mots de passes</param>
        public Param(int codeLangue, int emplacementListeCouleur,bool cortanaActive)
        {
            IdLangue = codeLangue;
            EmplacementListeCouleur = emplacementListeCouleur;
            IsCortanaActive = cortanaActive;
        }

        /// <summary>
        /// Constructeur vide
        /// </summary>
        public Param()
        {
            
        }
    }
}
