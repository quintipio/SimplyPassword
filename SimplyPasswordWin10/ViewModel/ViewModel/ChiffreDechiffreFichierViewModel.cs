using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using SimplyPasswordWin10.Abstract;
using SimplyPasswordWin10Shared.Context;
using SimplyPasswordWin10Shared.Enum;
using SimplyPasswordWin10Shared.Utils;

namespace SimplyPasswordWin10.ViewModel
{
    /// <summary>
    /// ViewModel pour la page de chiffrement déchiffrement de fichiers
    /// </summary>
    public partial class ChiffreDechiffreFichierViewModel : AbstractViewModel
    {
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="mode">le mode d'utilisation de la page</param>
        public ChiffreDechiffreFichierViewModel(ChiffrerDechiffrerEnum mode)
        {
            Mode = mode;
        }

        /// <summary>
        /// créer un mot de passe
        /// </summary>
        /// <returns>le mot de passe</returns>
        public void GenererPassword(int longueur, bool lettre, bool chiffres, bool spec)
        {
            Password = CryptUtils.GeneratePassword(longueur, lettre, chiffres, spec);
        }

        /// <summary>
        /// calcul la force d'un mot de passe
        /// </summary>
        /// <param name="pass">le mot de passe</param>
        public void CalculerForceMotdePasse(string pass)
        {
            Force = CryptUtils.CalculForceMotDePasse(pass);
        }

        /// <summary>
        /// Vérifie les informations et retourne les éventuelles erreurs à afficher
        /// </summary>
        /// <returns></returns>
        private async Task<string> Validate()
        {
            var retour = "";

            if (Password == null || (Password != null && Password.Length < 8))
            {
                retour += ResourceLoader.GetForCurrentView("Errors").GetString("erreurNb8Caracteres") + "\r\n";
            }

            if (FileInput == null)
            {
                retour += ResourceLoader.GetForCurrentView("Errors").GetString("erreurAucunFichierEntre") + "\r\n";
            }
            else
            {
                var toto = (await FileInput.GetBasicPropertiesAsync()).Size;
                if ((await FileInput.GetBasicPropertiesAsync()).Size > ContexteStatic.MaxSizeFichierChiffrer)
                {
                    retour += ResourceLoader.GetForCurrentView("Errors").GetString("erreurFichierEntreGros") + "\r\n";
                }
            }

            if (FileOutput == null)
            {
                retour += ResourceLoader.GetForCurrentView("Errors").GetString("erreurAucunFichierSorti") + "\r\n";
            }

           
            return retour;
        }

        /// <summary>
        /// Lance le chiffrement ou le déchiffrement du fichier
        /// </summary>
        /// <returns>les erreurs</returns>
        public async Task<string> Valider()
        {
            var retour = await Validate();

            if (string.IsNullOrWhiteSpace(retour))
            {
                try
                {
                    var buffer = await FileIO.ReadBufferAsync(FileInput);
                    switch (Mode)
                    {
                        case ChiffrerDechiffrerEnum.Chiffrer:
                            var cypherByte = CryptUtils.AesEncryptByteArrayToByteArray(buffer.ToArray(), Password,Password);
                            await FileIO.WriteBytesAsync(FileOutput, cypherByte);
                            break;

                        case ChiffrerDechiffrerEnum.Dechiffrer:
                            var cypherByteB = CryptUtils.AesDecryptByteArrayToByteArray(buffer.ToArray(), Password,Password);
                            await FileIO.WriteBytesAsync(FileOutput, cypherByteB);
                            break;
                    }
                }
                catch
                {
                    retour += ResourceLoader.GetForCurrentView("Errors").GetString("erreurDechiffrement")+ "\r\n";
                }
            }

            return retour;
        }
    }
}
