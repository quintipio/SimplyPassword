using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace SimplyPasswordWin10.Converter
{
    /// <summary>
    /// Convertisseur d'un boolean en indication de Visible
    /// </summary>
    public class BoolPasswordRevealConverter : IValueConverter
    {
        /// <summary>
        /// Boolean to Visible
        /// </summary>
        /// <param name="value">la donnée</param>
        /// <param name="targetType">targettype</param>
        /// <param name="parameter">parameter</param>
        /// <param name="language">Culture</param>
        /// <returns>la conversion</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((bool)value) ? PasswordRevealMode.Visible : PasswordRevealMode.Peek;
        }

        /// <summary>
        /// Visible to boolean
        /// </summary>
        /// <param name="value">la donnée</param>
        /// <param name="targetType">targettype</param>
        /// <param name="parameter">parameter</param>
        /// <param name="language">Culture</param>
        /// <returns>la conversion</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return ((PasswordRevealMode)value == PasswordRevealMode.Visible);
        }
    }
}
