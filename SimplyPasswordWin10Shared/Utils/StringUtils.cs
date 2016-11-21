using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SimplyPasswordWin10Shared.Utils
{
    /// <summary>
    /// Outils pour gérer les chaines de caractères
    /// </summary>
    public static class StringUtils
    {
        /// <summary>
        /// Sépare une chaine de caractère en liste de string à partir d'une chaine de caractère
        /// </summary>
        /// <param name="chaine">La chaine à couper</param>
        /// <param name="separateur">la chaine séparatrice</param>
        /// <returns>la liste de String</returns>
        public static List<string> Split(string chaine, string separateur)
        {
            return ObjectUtils.ArrayToList(Regex.Split(chaine, separateur));
        }

        /// <summary>
        /// Permet de donner l'extension d'un fichier
        /// </summary>
        /// <param name="fichier">le chemin ou le nom complet du fichier</param>
        /// <returns>l'extension</returns>
        public static string GetExtension(string fichier)
        {
            return fichier.Substring(fichier.LastIndexOf('.') + 1, fichier.Length - (fichier.LastIndexOf('.') + 1));
        }

        /// <summary>
        /// Permet de donner le nom d'un fichier sans son extension
        /// </summary>
        /// <param name="fichier">le nom complet du fichier</param>
        /// <returns>le nom</returns>
        public static string GetNomFichier(string fichier)
        {
            return fichier.Substring(0, (fichier.LastIndexOf('.') + 1));
        }
    }
}
