using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.UI.Xaml.Media.Imaging;

namespace SimplyPasswordWin10Shared.Model
{
    [XmlRoot("ImageUnlock")]
    public class ImageUnlock
    {
        /// <summary>
        /// l'imag eà sauvegarder
        /// </summary>
        [XmlElement("image")]
        public byte[] Image { get; set; }

        /// <summary>
        /// la liste des points
        /// </summary>
        [XmlElement("listePoint")]
        public List<Point> ListePoint { get; set; }
    }
}
