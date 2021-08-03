using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WpfApp1.BaseOper
{/// <summary>
 /// 提供转换：包括：Bitmap和BitmapImage相互转换。
 ///RenderTargetBitmap –> BitmapImage
 ///ImageSource –> Bitmap
 ///BitmapImage和byte[]相互转换。
 ///byte[] –> Bitmap
 /// </summary>
    class ImageTranslater
    {/// <summary>
     ///   Bitmap --> BitmapImage
     /// </summary>
     /// <param name="bitmap"></param>
     /// <returns></returns>

        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            byte[] bytes;
            bytes=ms.GetBuffer();
            BitmapImage bi = null;
            try
            {
                bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = new MemoryStream(bytes);
                bi.EndInit();

            }
            catch 
            {
                Console.WriteLine("转换错误");
                bi = null;
            }
            return bi;

        }


        // BitmapImage --> Bitmap
        //public static Bitmap BitmapImageToBitmap(BitmapImage bitmapImage)
        //{
        //    // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

        //    using (MemoryStream outStream = new MemoryStream())
        //    {
        //        BitmapEncoder enc = new BmpBitmapEncoder();
        //        enc.Frames.Add(BitmapFrame.Create(bitmapImage));
        //        enc.Save(outStream);
        //        Bitmap bitmap = new Bitmap(outStream);

        //        return new Bitmap(bitmap);
        //    }
        //}

        ///// <summary>
        /////  byte数组转为bitmap
        ///// </summary>
        ///// <param name="byteArray"></param>
        ///// <returns></returns>
        //public static BitmapImage ByteArrayToBitmap(byte[] byteArray)
        //{
        //    BitmapImage bmp = null;
        //    try
        //    {
        //        bmp = new BitmapImage();
        //        bmp.BeginInit();
        //        bmp.StreamSource = new MemoryStream(byteArray);
        //        bmp.EndInit();
        //    }
        //    catch
        //    {
        //        bmp = null;
        //    }
        //    return bmp;
        //}
        ///// <summary>
        ///// bitmap 转为byte数组
        ///// </summary>
        ///// <param name="bmp"></param>
        ///// <returns></returns>
        //public byte[] BitmapImageToByteArray(BitmapImage bmp)
        //{
        //    byte[] byteArray=null;
        //    try
        //    {
        //        Stream st = bmp.StreamSource;
        //        if (st != null && st.Length > 0)
        //        {
        //            st.Position = 0;
        //            using(BinaryReader br=new BinaryReader(st))
        //            {
        //                byteArray = br.ReadBytes((int)st.Length);
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        byteArray = null;
        //        Console.WriteLine("转换失败");
        //    }
        //    return byteArray;
        //}
        /// <summary>
        /// bitmapimage保存到指定路径的文件下
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="filePath"></param>
        public static void SaveBitmapImageIntoFile(BitmapImage bmp,string filePath)
        {
            BitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));
            using(var fileStream=new System.IO.FileStream(filePath, System.IO.FileMode.Create))
            {
                encoder.Save(fileStream);
            }

        }
        



    }
}
