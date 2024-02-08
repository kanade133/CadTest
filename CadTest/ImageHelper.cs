using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CadTest
{
    internal class ImageHelper
    {
        private static readonly string folderPathExternal = Path.GetDirectoryName(typeof(ImageHelper).Assembly.Location);
        private static readonly string folderPathInternal = "pack://application:,,,/CadTest;component/";

        /// <summary>
        /// Get ImageSource From External Folder
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public static ImageSource GetImageSourceExternal(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
            {
                return null;
            }
            try
            {
                return new BitmapImage(new Uri(folderPathExternal + "\\" + relativePath));
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// Get ImageSource From Internal Dll
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public static ImageSource GetImageSourceInternal(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
            {
                return null;
            }
            try
            {
                return new BitmapImage(new Uri(folderPathInternal + "\\" + relativePath));
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// Get Icon From Internal Dll
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public static Icon GetIconInternal(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
            {
                return null;
            }
            try
            {
                var bitmap = BitmapImage2Bitmap(new BitmapImage(new Uri(folderPathInternal + "\\" + relativePath)));
                Icon icon = Icon.FromHandle(bitmap.GetHicon());
                return icon;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                return new Bitmap(outStream);
            }
        }
    }
}
