using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Model;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.ValueConverter
{
    public class PageNameToBitmapImage : IMultiValueConverter
    {
        static Dictionary<string, BitmapImage> bitmapCache = new Dictionary<string, BitmapImage>();
        static object m_lock = new object();
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ( values != null && values.Length == 2 )
            {
                string fileName = values[0] as string;
                int pageNo = (int) values[1];

                string key = fileName + "_P" + pageNo.ToString(); 

                BitmapImage bitmapImage;

                if ( bitmapCache.TryGetValue( key, out bitmapImage) == false )
                {
                    // Performance -  Do nothing while the system is running - 
                    if ( AppDataCenter.Singleton.IsRunning == true )
                    {
                        return null;
                    }

                    lock (m_lock)
                    {
                        using (var srcFile = System.IO.File.OpenRead(fileName))
                        {
                            var dec = TiffBitmapDecoder.Create(srcFile, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);

                            var page = dec.Frames[pageNo - 1];

                            var buff = BitmapFrameToStream(page);

                            bitmapImage = new BitmapImage();

                            bitmapImage.BeginInit();
                            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                            bitmapImage.DecodePixelHeight = 200;
                            bitmapImage.StreamSource = buff;
                            bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                            bitmapImage.EndInit();

                            bitmapCache[key] = bitmapImage;
                        }
                        
                    }
                }

                return bitmapImage;
            }
            else
            {
                return null;
            }
        }

        private static MemoryStream BitmapFrameToStream(BitmapFrame page)
        {
            var enc = new PngBitmapEncoder();

            enc.Frames.Add(page);

            var buff = new MemoryStream();

            enc.Save(buff);

            return buff;
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
