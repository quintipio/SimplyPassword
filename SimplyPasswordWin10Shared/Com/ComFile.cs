using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using SimplyPasswordWin10Shared.Enum;

namespace SimplyPasswordWin10Shared.Com
{
    /// <summary>
    /// Classe permettant de gérer un fichier
    /// </summary>
    public class ComFile
    {
        private readonly string _path;
        private readonly PlaceEcritureEnum _placeEcriture;
        private StorageFile _fichier;

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="path">le chemin du fichier</param>
        /// <param name="placeEcriture">son répertoire</param>
        public ComFile(string path, PlaceEcritureEnum placeEcriture)
        {
            _path = path;
            _placeEcriture = placeEcriture;
            _fichier = null;
        }

        public ComFile(StorageFile fichier)
        {
            _path = fichier.Path;
            _placeEcriture = PlaceEcritureEnum.Personnel;
            _fichier = fichier;
        }

        /// <summary>
        /// ecrit un byte array dans un fichier
        /// </summary>
        /// <param name="data">les donénes à écrire</param>
        /// <param name="mode">le mode d'ouverture du fichier</param>
        /// <returns>true si ok</returns>
        public async Task<bool> Ecrire(byte[] data, CreationCollisionOption mode)
        {
            var retour = false;
            StorageFile file = null;
            switch(_placeEcriture)
            {
                case PlaceEcritureEnum.LocalState:
                    file = await ApplicationData.Current.LocalFolder.CreateFileAsync(_path, mode);
                    break;

                case PlaceEcritureEnum.Roaming:
                    file = await ApplicationData.Current.RoamingFolder.CreateFileAsync(_path, mode);
                    break;

                case PlaceEcritureEnum.Personnel:
                    file = _fichier;
                    break;
            }
            if(file != null)
            {
                await FileIO.WriteBytesAsync(file, data);
                retour = true;
            }
            return retour;
        }

        /// <summary>
        /// ecrit unne chaine de caractère dans un fichier
        /// </summary>
        /// <param name="data">la chaine de caractère à écrire</param>
        /// <param name="mode">le mode d'ouverture du fichier</param>
        /// <param name="ecraser">ecrase le fichier ou rajoute à la suite</param>
        /// <returns>true si ok</returns>
        public async Task<bool> Ecrire(string data, CreationCollisionOption mode, bool ecraser)
        {
            var retour = false;
            StorageFile file = null;
            switch (_placeEcriture)
            {
                case PlaceEcritureEnum.LocalState:
                    file = await ApplicationData.Current.LocalFolder.CreateFileAsync(_path, mode);
                    break;

                case PlaceEcritureEnum.Roaming:
                    file = await ApplicationData.Current.RoamingFolder.CreateFileAsync(_path, mode);
                    break;

                case PlaceEcritureEnum.Personnel:
                    file = _fichier;
                    break;
            }

            if (file != null)
            {
                if (ecraser)
                {
                    await FileIO.WriteTextAsync(file, data);
                }
                else
                {
                    await FileIO.AppendTextAsync(file, data);
                }
                retour = true;
            }
            return retour;
        }

        /// <summary>
        /// écrit une liste de string dans un fichier
        /// </summary>
        /// <param name="data">les données à écrire</param>
        /// <param name="mode">le mode d'ouverture du fichier</param>
        /// <param name="ecraser">écrase le fichier ou rajotue à la suite</param>
        /// <returns>true si ok</returns>
        public async Task<bool> Ecrire(IEnumerable<string> data, CreationCollisionOption mode,bool ecraser)
        {
            var retour = false;
            StorageFile file = null;
            switch (_placeEcriture)
            {
                case PlaceEcritureEnum.LocalState:
                    file = await ApplicationData.Current.LocalFolder.CreateFileAsync(_path, mode);
                    break;

                case PlaceEcritureEnum.Roaming:
                    file = await ApplicationData.Current.RoamingFolder.CreateFileAsync(_path, mode);
                    break;

                case PlaceEcritureEnum.Personnel:
                    file = _fichier;
                    break;
            }

            if (file != null)
            {
                if (ecraser)
                {
                    await FileIO.WriteLinesAsync(file, data);
                }
                else
                {
                    await FileIO.AppendLinesAsync(file, data);
                }
                retour = true;
            }
            return retour;
        }

        /// <summary>
        /// écrit un buffer dans un fichier
        /// </summary>
        /// <param name="data">les données àécrire</param>
        /// <param name="mode">le mode d'ouverture du fichier</param>
        /// <returns></returns>
        public async Task<bool> Ecrire(IBuffer data, CreationCollisionOption mode)
        {
            var retour = false;
            StorageFile file = null;
            switch (_placeEcriture)
            {
                case PlaceEcritureEnum.LocalState:
                    file = await ApplicationData.Current.LocalFolder.CreateFileAsync(_path, mode);
                    break;

                case PlaceEcritureEnum.Roaming:
                    file = await ApplicationData.Current.RoamingFolder.CreateFileAsync(_path, mode);
                    break;

                case PlaceEcritureEnum.Personnel:
                    file = _fichier;
                    break;
            }

            if (file != null)
            {
                await FileIO.WriteBufferAsync(file, data);
                retour = true;
            }
            return retour;
        }

        /// <summary>
        /// lit un fichier en format binaire
        /// </summary>
        /// <returns>le contenu du fichier en byte array</returns>
        public async Task<byte[]> LireByteArray()
        {
            StorageFile file = null;
            switch (_placeEcriture)
            {
                case PlaceEcritureEnum.LocalState:
                    file = await ApplicationData.Current.LocalFolder.GetFileAsync(_path);
                    break;

                case PlaceEcritureEnum.Roaming:
                    file = await ApplicationData.Current.RoamingFolder.GetFileAsync(_path);
                    break;

                case PlaceEcritureEnum.Personnel:
                    file = _fichier;
                    break;
            }

            if (file != null)
            {
                var buffer = await FileIO.ReadBufferAsync(file);
                return buffer.ToArray();
            }
            return null;
        }

        /// <summary>
        /// lit un fichier et le palce dans une chaine de caractère
        /// </summary>
        /// <returns>le contenu du fichier</returns>
        public async Task<string> LireString()
        {
            StorageFile file = null;
            switch (_placeEcriture)
            {
                case PlaceEcritureEnum.LocalState:
                    file = await ApplicationData.Current.LocalFolder.GetFileAsync(_path);
                    break;

                case PlaceEcritureEnum.Roaming:
                    file = await ApplicationData.Current.RoamingFolder.GetFileAsync(_path);
                    break;

                case PlaceEcritureEnum.Personnel:
                    file = _fichier;
                    break;
            }
           
            return (file != null)? await FileIO.ReadTextAsync(file):null;


        }

        /// <summary>
        /// lit un fichier et retourne ses lignes dans une liste de chaine de caractères
        /// </summary>
        /// <returns>le contenu du fichier</returns>
        public async Task<IList<string>> LireListString()
        {
            StorageFile file = null;
            switch (_placeEcriture)
            {
                case PlaceEcritureEnum.LocalState:
                    file = await ApplicationData.Current.LocalFolder.GetFileAsync(_path);
                    break;

                case PlaceEcritureEnum.Roaming:
                    file = await ApplicationData.Current.RoamingFolder.GetFileAsync(_path);
                    break;

                case PlaceEcritureEnum.Personnel:
                    file = _fichier;
                    break;
            }
             return (file != null)? await FileIO.ReadLinesAsync(file):null;
        }

        /// <summary>
        /// retourne le chemin du fichier
        /// </summary>
        /// <returns></returns>
        public string GetPath()
        {
            return _path;
        }

        /// <summary>
        /// permet de vérifier l'existence d'un fichier
        /// </summary>
        /// <returns>true si existe</returns>
        public async Task<bool> FileExist()
        {
            IStorageItem item = null;

            switch (_placeEcriture)
            {
                case PlaceEcritureEnum.LocalState:
                    item = await ApplicationData.Current.LocalFolder.TryGetItemAsync(_path);
                    break;

                case PlaceEcritureEnum.Roaming:
                    item = await ApplicationData.Current.RoamingFolder.TryGetItemAsync(_path);
                    break;
                case PlaceEcritureEnum.Personnel:
                    try
                    {
                        return await FileIO.ReadBufferAsync(_fichier) != null;

                    }
                    catch
                    {
                        return false;
                    }
            }
            return item != null;
        }

        /// <summary>
        /// retourne la dernière date de modification d'un fichier
        /// </summary>
        /// <returns>la date de dernière modification</returns>
        public async Task<DateTimeOffset> GetDateModified()
        {
            StorageFile file = null;
            switch (_placeEcriture)
            {
                case PlaceEcritureEnum.LocalState:
                    file = await ApplicationData.Current.LocalFolder.GetFileAsync(_path);
                    break;

                case PlaceEcritureEnum.Roaming:
                    file = await ApplicationData.Current.RoamingFolder.GetFileAsync(_path);
                    break;

                case PlaceEcritureEnum.Personnel:
                    file = _fichier;
                    break;
            }
            if(await FileExist())
            {
                if (file != null) return (await file.GetBasicPropertiesAsync()).DateModified;
            }
            return new DateTimeOffset();
        }

        /// <summary>
        /// recherche dans un fichier (de type csv par exemple) la valeur d'une variable
        /// </summary>
        /// <param name="chaineDebut">le chaine de caractère écrite avant le résultat souhaité</param>
        /// <param name="caractereFin">le caractère de fin de recherche</param>
        /// <returns>le résultat sinon null</returns>
        public async Task<string> FindElementInFile(string chaineDebut,char caractereFin)
        {
            StorageFile file = null;
            switch (_placeEcriture)
            {
                case PlaceEcritureEnum.LocalState:
                    file = await ApplicationData.Current.LocalFolder.GetFileAsync(_path);
                    break;

                case PlaceEcritureEnum.Roaming:
                    file = await ApplicationData.Current.RoamingFolder.GetFileAsync(_path);
                    break;

                case PlaceEcritureEnum.Personnel:
                    file = _fichier;
                    break;
            }

            if(file != null)
            {
                var data = await FileIO.ReadLinesAsync(file, UnicodeEncoding.Utf8);

                foreach (var mot in data)
                {
                    if (mot.Contains(chaineDebut))
                    {
                        var fin = mot.LastIndexOf(caractereFin);
                        var debut = mot.IndexOf(chaineDebut, StringComparison.Ordinal) + chaineDebut.Length;
                        return mot.Substring(debut, fin - debut);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Efface le fichier
        /// </summary>
        /// <returns>true si effacer</returns>
        public async Task<bool> DeleteFile()
        {
            if (await FileExist())
            {
                StorageFile file = null;
                switch (_placeEcriture)
                {
                    case PlaceEcritureEnum.LocalState:
                        file = await ApplicationData.Current.LocalFolder.GetFileAsync(_path);
                        break;

                    case PlaceEcritureEnum.Roaming:
                        file = await ApplicationData.Current.RoamingFolder.GetFileAsync(_path);
                        break;

                    case PlaceEcritureEnum.Personnel:
                        file = _fichier;
                        break;
                }
                if (file != null)
                {
                    await file.DeleteAsync();
                }
                return !await FileExist();
            }
            return true;
        }

        /// <summary>
        /// Retourne la taille du fichier
        /// </summary>
        /// <returns>la taille</returns>
        public async Task<ulong> GetSizeFile()
        {
            StorageFile file = null;
            if (await FileExist())
            {
                switch (_placeEcriture)
                {
                    case PlaceEcritureEnum.LocalState:
                        file = await ApplicationData.Current.LocalFolder.GetFileAsync(_path);
                        break;

                    case PlaceEcritureEnum.Roaming:
                        file = await ApplicationData.Current.RoamingFolder.GetFileAsync(_path);
                        break;

                    case PlaceEcritureEnum.Personnel:
                        file = _fichier;
                        break;
                }

                if (file != null)
                {
                    return (await file.GetBasicPropertiesAsync()).Size;
                }
            }

            return 0;
        }

        /// <summary>
        /// Retourne le nom du fichier
        /// </summary>
        /// <returns>le nom du fichier</returns>
        public async Task<string> GetNameFile()
        {
            StorageFile file = null;
            if (await FileExist())
            {
                switch (_placeEcriture)
                {
                    case PlaceEcritureEnum.LocalState:
                        file = await ApplicationData.Current.LocalFolder.GetFileAsync(_path);
                        break;

                    case PlaceEcritureEnum.Roaming:
                        file = await ApplicationData.Current.RoamingFolder.GetFileAsync(_path);
                        break;

                    case PlaceEcritureEnum.Personnel:
                        file = _fichier;
                        break;
                }
            }
            return file?.Name ?? "";
        }

    }
}
