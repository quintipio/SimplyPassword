using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace SimplyPasswordWin10Shared.Utils
{
    public static class ObjectUtils
    {
        /// <summary>
        /// Converti un tableau d'object en une liste
        /// </summary>
        /// <typeparam name="TO">le type d'objet de tableau et de liste de sortie</typeparam>
        /// <param name="tab">le tableau d'objet</param>
        /// <returns>la liste</returns>
        public static List<TO> ArrayToList<TO>(IEnumerable<TO> tab)
        {
            return tab.ToList();
        }

        /// <summary>
        /// Converti un fichier en byte[]
        /// </summary>
        /// <param name="file">le fichier à convertir</param>
        /// <returns>le byte[]</returns>
        public static async Task<byte[]> ConvertFileToBytes(StorageFile file)
        {
            using (var inputStream = await file.OpenSequentialReadAsync())
            {
                var readStream = inputStream.AsStreamForRead();

                var byteArray = new byte[readStream.Length];
                await readStream.ReadAsync(byteArray, 0, byteArray.Length);
                return byteArray;
            }
        }

        /// <param name="tab">le tableau de bytes</param>
        /// <returns>l'image</returns>
        public static async Task<BitmapImage> ConvertBytesToBitmap(byte[] tab)
        {
            using (var stream = new InMemoryRandomAccessStream())
            {
               using (var writer = new DataWriter(stream.GetOutputStreamAt(0)))
               {
                  writer.WriteBytes(tab);
                  await writer.StoreAsync();
               }
                var image = new BitmapImage();
                await image.SetSourceAsync(stream);
                return image;
             }
        }
    }
}
