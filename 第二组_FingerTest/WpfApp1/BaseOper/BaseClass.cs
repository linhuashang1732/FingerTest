﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.BaseOper
{
    class BaseClass
    {
        /// <summary>  
        /// 将源图像灰度化，并转化为8位灰度图像。  
        /// </summary>  
        /// <param name="original"> 源图像。 </param>  
        /// <returns> 8位灰度图像。 </returns>  
        public static Byte[] BmpToByte(Bitmap original)
        {
            if (original != null)
            {
                // 将源图像内存区域锁定  
                Rectangle rect = new Rectangle(0, 0, original.Width, original.Height);
                BitmapData bmpData = original.LockBits(rect, ImageLockMode.ReadOnly,
                    PixelFormat.Format8bppIndexed);

               int  width = bmpData.Width;
              int   height = bmpData.Height;
                int stride = bmpData.Stride;  // 扫描线的宽度  
                int offset = stride - width ;  // 显示宽度与扫描线宽度的间隙  
                IntPtr ptr = bmpData.Scan0;   // 获取bmpData的内存起始位置  ,即第一个像素的位置
                int scanBytes = stride * height;  // 用stride宽度，表示这是内存区域的大小  

                // 分别设置两个位置指针，指向源数组和目标数组  
                int posScan = 0, posDst = 0;
                byte[] rgbValues = new byte[scanBytes];  // 为目标数组分配内存  
                Marshal.Copy(ptr, rgbValues, 0, scanBytes);  // 将图像数据拷贝到rgbValues中  
                                                             // 分配灰度数组  
                byte[] grayValues = new byte[width * height]; // 不含未用空间。  
                                                              // 计算灰度数组  
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        double temp = rgbValues[posScan++];
                        grayValues[posDst++] = (byte)temp;
                    }
                    // 跳过图像数据每行未用空间的字节，length = stride - width * bytePerPixel  
                    posScan += offset;
                }


                // 内存解锁  
                Marshal.Copy(rgbValues, 0, ptr, scanBytes);
                original.UnlockBits(bmpData);  // 解锁内存区域  

                // 构建8位灰度位图  
                //Bitmap retBitmap = BuiltGrayBitmap(grayValues, width, height);
                //return retBitmap;
                return grayValues;
            }
            else
            {
              
                return null;
            }
        }

        /// <summary>  
        /// 用灰度数组新建一个8位灰度图像。  
        /// http://www.cnblogs.com/spadeq/archive/2009/03/17/1414428.html  
        /// </summary>  
        /// <param name="rawValues"> 灰度数组(length = width * height)。 </param>  
        /// <param name="width"> 图像宽度。 </param>  
        /// <param name="height"> 图像高度。 </param>  
        /// <returns> 新建的8位灰度位图。 </returns>  
        public static Bitmap BuiltGrayBitmap(byte[] rawValues, Bitmap original)
        {
            int width = original.Width;
            int height = original.Height;

            // 新建一个8位灰度位图，并锁定内存区域操作  
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, width, height),
                 ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            // 计算图像参数  
            int offset = bmpData.Stride - bmpData.Width;        // 计算每行未用空间字节数  
            IntPtr ptr = bmpData.Scan0;                         // 获取首地址  
            int scanBytes = bmpData.Stride * bmpData.Height;    // 图像字节数 = 扫描字节数 * 高度  
            byte[] grayValues = new byte[scanBytes];            // 为图像数据分配内存  

            // 为图像数据赋值  
            int posSrc = 0, posScan = 0;                        // rawValues和grayValues的索引  
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    grayValues[posScan++] = rawValues[posSrc++];
                }
                // 跳过图像数据每行未用空间的字节，length = stride - width * bytePerPixel  
                posScan += offset;
            }

            // 内存解锁  
            Marshal.Copy(grayValues, 0, ptr, scanBytes);
            bitmap.UnlockBits(bmpData);  // 解锁内存区域  

            //// 修改生成位图的索引表，从伪彩修改为灰度  
             ColorPalette palette= original.Palette;
            //// 获取一个Format8bppIndexed格式图像的Palette对象  
            //using (Bitmap bmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed))
            //{
            //    palette = bmp.Palette;
            //}
            //for (int i = 0; i < 256; i++)
            //{
            //    palette.Entries[i] = Color.FromArgb(i, i, i);
            //}
            //// 修改生成位图的索引表  
            bitmap.Palette =palette ;

            return bitmap;
        }
    }
}
