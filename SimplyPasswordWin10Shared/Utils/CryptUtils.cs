using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace SimplyPasswordWin10Shared.Utils
{
    public static class CryptUtils
    {
        private static readonly List<char> ListeLettreMinuscule = new List<char> { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k'
        , 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'};
        private static readonly List<char> ListeLettreMajuscule = new List<char> { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K'
        , 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'};
        private static readonly List<char> ListeChiffre = new List<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        private static readonly List<char> ListeCaractereSpeciaux = new List<char> { '²', '&', 'é', '"', '#', '\'', '{', '-', '|', 'è', '_'
        , '\\', 'ç', 'à', '@', ')', '(', '[', ']', '=', '+', '}', '£', '$', '¤', '%', 'ù', 'µ', '*', '?', ',', '.', ';'
        , '/', ':', '§', '!', '€', '>', '<'};

        

        #region utilitaire pour un mot de passe
        /*****METHODE UTILITAIRE DE MOT DE PASSE****/
        /// <summary>
        /// Calcul la force approximative d'un mot de passe
        /// </summary>
        /// <param name="motDePasse">le mot de passe à calculer</param>
        /// <returns>la puissance en %</returns>
        public static int CalculForceMotDePasse(string motDePasse)
        {
            var somme = 0;// motDePasse.Length * 5;//chaque caractère vaut 5 %
            var nbTypePresent = 0;
            var minusculePresent = false;
            var chiffrePresent = false;
            var majusculePresent = false;
            var speciauxPresent = false;
            //ont vérifie les différents type de caractères présent
            if (motDePasse == null)
            {
                motDePasse = "";
            }
            foreach (var t in motDePasse)
            {
                if (ListeLettreMinuscule.Contains(t))
                {
                    minusculePresent = true;
                    somme += 4;
                }

                if (ListeLettreMajuscule.Contains(t))
                {
                    majusculePresent = true;
                    somme += 4;
                }

                if (ListeChiffre.Contains(t))
                {
                    chiffrePresent = true;
                    somme += 2;
                }

                if (ListeCaractereSpeciaux.Contains(t))
                {
                    speciauxPresent = true;
                    somme += 7;
                }
            }

            if (speciauxPresent) { nbTypePresent++; }
            if (minusculePresent) { nbTypePresent++; }
            if (majusculePresent) { nbTypePresent++; }
            if (chiffrePresent) { nbTypePresent++; }

            //ont multiple la force par un certains nombre en fonction du nombre de caractères.
            switch (nbTypePresent)
            {
                case 1: somme = ((int)(somme * 0.75)); break;
                case 2: somme = ((int)(somme * 1.3)); break;
                case 3: somme = ((int)(somme * 1.7)); break;
                case 4: somme = ((somme * 2)); break;
            }

            if (somme > 100) somme = 100;
            return somme;
        }

        /// <summary>
        /// Genere un mot de passe aléatoire composer de caractères majuscules, minuscules, de chiffres et de caractères spéciaux
        /// </summary>
        /// <param name="longueur">longueur du mot de passe souhaité, si 0 sera de 12 caractères</param>
        /// <param name="lettre">autorise les lettres minuscules et majuscules dans le mot de passe</param>
        /// <param name="chiffre">autorise les chiffres dans le mot de passe</param>
        /// <param name="caracSpeciaux">autorise les caractères spéciaux dans le mot de passe</param>
        /// <returns>le mot de passe généré</returns>
        public static string GeneratePassword(int longueur, bool lettre, bool chiffre, bool caracSpeciaux)
        {
            var length = (longueur == 0) ? 12 : longueur;
            var password = "";
            var rnd = new Random();
            for (var i = 0; i < length; i++)
            {
                var caracBienCree = false;
                do
                {
                    var typeTab = rnd.Next(4);
                    switch (typeTab)
                    {
                        case 0:
                            if (lettre)
                            {
                                password += ListeLettreMinuscule[rnd.Next(ListeLettreMinuscule.Count)];
                                caracBienCree = true;
                            }
                            break;
                        case 1:
                            if (lettre)
                            {
                                password += ListeLettreMajuscule[rnd.Next(ListeLettreMajuscule.Count)];
                                caracBienCree = true;
                            }
                            break;
                        case 2:
                            if (chiffre)
                            {
                                password += ListeChiffre[rnd.Next(ListeChiffre.Count)];
                                caracBienCree = true;
                            }
                            break;
                        case 3:
                            if (caracSpeciaux)
                            {
                                password += ListeCaractereSpeciaux[rnd.Next(ListeCaractereSpeciaux.Count)];
                                caracBienCree = true;
                            }
                            break;
                    }
                } while (!caracBienCree);
            }
            return password;
        }
        #endregion

        #region AES
        /// <summary>
        /// Génère les clés de chiffrement AES nécéssaire au chiffrement ou au déchiffrement
        /// </summary>
        /// <param name="password">le mot de passe</param>
        /// <param name="salt">la complexification du mot de passe</param>
        /// <param name="iterationCount">le nombre de passage</param>
        /// <param name="keyMaterial">le buffer du mot de passe de sorti</param>
        /// <param name="iv">le buffer du vecteur de sorti</param>
        private static void GenerateKeyMaterial(string password, string salt, uint iterationCount, out IBuffer keyMaterial, out IBuffer iv)
        {
            var pwBuffer = CryptographicBuffer.ConvertStringToBinary(password, BinaryStringEncoding.Utf8);
            var saltBuffer = CryptographicBuffer.ConvertStringToBinary(salt, BinaryStringEncoding.Utf16LE);

            var keyDerivationProvider = KeyDerivationAlgorithmProvider.OpenAlgorithm("PBKDF2_SHA1");
            var pbkdf2Parms = KeyDerivationParameters.BuildForPbkdf2(saltBuffer, iterationCount);

            var keyOriginal = keyDerivationProvider.CreateKey(pwBuffer);
            keyMaterial = CryptographicEngine.DeriveKeyMaterial(keyOriginal, pbkdf2Parms, 32);
            var derivedPwKey = keyDerivationProvider.CreateKey(pwBuffer);
            iv = CryptographicEngine.DeriveKeyMaterial(derivedPwKey, pbkdf2Parms, 16);
        }

        /// <summary>
        /// Chiffre une chaine de caractère et en ressort une chaine de caractère
        /// </summary>
        /// <param name="dataToEncrypt">la chaine de caractère à chiffrer</param>
        /// <param name="password">le mot de passe</param>
        /// <param name="salt">la complexification du mot de passe</param>
        /// <returns>la chaine de caractère chiffré en string</returns>
        public static string AesEncryptStringToString(string dataToEncrypt, string password, string salt)
        {
            IBuffer aesKeyMaterial;
            IBuffer iv;
            const uint iterationCount = 10000;
            GenerateKeyMaterial(password, salt, iterationCount, out aesKeyMaterial, out iv);
            var plainText = CryptographicBuffer.ConvertStringToBinary(dataToEncrypt, BinaryStringEncoding.Utf8);

            var symProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm("AES_CBC_PKCS7");
            var symmKey = symProvider.CreateSymmetricKey(aesKeyMaterial);

            var resultBuffer = CryptographicEngine.Encrypt(symmKey, plainText, iv);
            return CryptographicBuffer.EncodeToBase64String(resultBuffer);
        }

        /// <summary>
        /// déchiffré une chaine de caractère et en ressort une chaine de caractèr
        /// </summary>
        /// <param name="dataToDecrypt">la chaine à déchiffrer</param>
        /// <param name="password">le mot de passe</param>
        /// <param name="salt">la complexification du mot de passe</param>
        /// <returns>la chaine de caractère déchiffré</returns>
        public static string AesDecryptStringToString(string dataToDecrypt, string password, string salt)
        {
            IBuffer aesKeyMaterial;
            IBuffer iv;
            const uint iterationCount = 10000;
            GenerateKeyMaterial(password, salt, iterationCount, out aesKeyMaterial, out iv);
            var ciphertext = CryptographicBuffer.DecodeFromBase64String(dataToDecrypt);

            var symProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm("AES_CBC_PKCS7");
            var symmKey = symProvider.CreateSymmetricKey(aesKeyMaterial);

            var resultBuffer = CryptographicEngine.Decrypt(symmKey, ciphertext, iv);
            var decryptedArray = resultBuffer.ToArray();
            return Encoding.UTF8.GetString(decryptedArray, 0, decryptedArray.Length);
        }

        /// <summary>
        /// chiffre un tableau de byte et en ressort un tableau de byte
        /// </summary>
        /// <param name="dataToEncrypt">la donnée à chiffrer</param>
        /// <param name="password">le mot de passe</param>
        /// <param name="salt">la complexification du mot de passe</param>
        /// <returns>le résultat chiffré</returns>
        public static byte[] AesEncryptByteArrayToByteArray(byte[] dataToEncrypt, string password, string salt)
        {
            IBuffer aesKeyMaterial;
            IBuffer iv;
            const uint iterationCount = 10000;
            GenerateKeyMaterial(password, salt, iterationCount, out aesKeyMaterial, out iv);
            var plainText = CryptographicBuffer.CreateFromByteArray(dataToEncrypt);

            var symProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm("AES_CBC_PKCS7");
            var symmKey = symProvider.CreateSymmetricKey(aesKeyMaterial);

            var encrypted = CryptographicEngine.Encrypt(symmKey, plainText, iv);
            byte[] retour;
            CryptographicBuffer.CopyToByteArray(encrypted, out retour);
            return retour;
        }

        /// <summary>
        /// déchiffre un tableau de byte et en ressort un tableau de byte
        /// </summary>
        /// <param name="dataToDecrypt">la donnée à chiffrer</param>
        /// <param name="password">le mot de passe</param>
        /// <param name="salt">la complexification du mot de passe</param>
        /// <returns>le résultat chiffré</returns>
        public static byte[] AesDecryptByteArrayToByteArray(byte[] dataToDecrypt, string password, string salt)
        {
            IBuffer aesKeyMaterial;
            IBuffer iv;
            const uint iterationCount = 10000;
            GenerateKeyMaterial(password, salt, iterationCount, out aesKeyMaterial, out iv);
            var ciphertext = CryptographicBuffer.CreateFromByteArray(dataToDecrypt);

            var symProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm("AES_CBC_PKCS7");
            var symmKey = symProvider.CreateSymmetricKey(aesKeyMaterial);

            var decrypted = CryptographicEngine.Decrypt(symmKey, ciphertext, iv);
            return decrypted.ToArray();
        }

        /// <summary>
        /// chiffre un string et en ressort un tableau de byte
        /// </summary>
        /// <param name="dataToEncrypt">la donnée à chiffrer</param>
        /// <param name="password">le mot de passe</param>
        /// <param name="salt">la complexification du mot de passe</param>
        /// <returns>le résultat chiffré</returns>
        public static byte[] AesEncryptStringToByteArray(string dataToEncrypt, string password, string salt)
        {

            IBuffer aesKeyMaterial;
            IBuffer iv;
            GenerateKeyMaterial(password, salt, 10000, out aesKeyMaterial, out iv);
            var plainText = CryptographicBuffer.ConvertStringToBinary(dataToEncrypt, BinaryStringEncoding.Utf8);

            var symProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm("AES_CBC_PKCS7");
            var symmKey = symProvider.CreateSymmetricKey(aesKeyMaterial);

            var resultBuffer = CryptographicEngine.Encrypt(symmKey, plainText, iv);
            return resultBuffer.ToArray();
        }

        /// <summary>
        /// déchiffre un tableau de byte et en ressort un string
        /// </summary>
        /// <param name="dataToDecrypt">la donnée à chiffrer</param>
        /// <param name="password">le mot de passe</param>
        /// <param name="salt">la complexification du mot de passe</param>
        /// <returns>le résultat chiffré</returns>
        public static String AesDecryptByteArrayToString(byte[] dataToDecrypt, string password, string salt)
        {
            IBuffer aesKeyMaterial;
            IBuffer iv;
            GenerateKeyMaterial(password, salt, 10000, out aesKeyMaterial, out iv);
            var ciphertext = CryptographicBuffer.CreateFromByteArray(dataToDecrypt);

            var symProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm("AES_CBC_PKCS7");
            var symmKey = symProvider.CreateSymmetricKey(aesKeyMaterial);

            var resultBuffer = CryptographicEngine.Decrypt(symmKey, ciphertext, iv);
            var decryptedArray = resultBuffer.ToArray();
            return Encoding.UTF8.GetString(decryptedArray, 0, decryptedArray.Length);
        }
        #endregion
        
    }
}
