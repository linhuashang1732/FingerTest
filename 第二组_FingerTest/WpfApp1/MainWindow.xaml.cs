using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.BaseOper;


namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// g_Handle:指纹器句柄
        /// g_biokeyHandle:biokey算法库句柄
        /// g_nWidth：图片宽度
        /// g_nHeight ：图片高度
        /// g_Connected：指纹器是否连接，true：连接；false：未连接
        /// g_bIsTimeToDie：读取指纹信息现程是否终止；true：终止，false:未终止
        /// g_IsRegister：是否采集
        /// </summary>
        IntPtr g_Handle = IntPtr.Zero;

        /// <summary>
        /// g_biokeyHandle:算法库句柄
        /// </summary>
        IntPtr g_biokeyHandle = IntPtr.Zero;
        int g_nWidth = 0;
        int g_nHeight = 0;
        bool g_Connected = false;
        /// <summary>
        /// byte[] g_FPBuffer：图像数据数组
        /// </summary>
        byte[] g_FPBuffer;
        int g_FPBufferSize = 0;
        bool g_bIsTimeToDie = false;
        byte[] temp0 = new byte[2048];
       byte[] temp1 =new byte[2048];
        byte[][] g_RegTmps = new byte[3][];
        byte[] g_RegTmp = new byte[2048];
        byte[] g_VerTmp = new byte[2048];
        string path;
       
        public MainWindow()
        {
            InitializeComponent();
            StateChange(true);
            btn_Op4.IsEnabled = btn_Op5.IsEnabled = btn_Op6.IsEnabled = btn_Op7.IsEnabled = false;
        }
        private void btn_OpClick(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            switch (btn.Content)
            {
                case "启动":

                    StateChange(true);
                    if (!g_Connected)
                    {
                        int ret = 0;
                        byte[] paramValue = new byte[64];

                        // Enable log
                        Array.Clear(paramValue, 0, paramValue.Length);
                        paramValue[0] = 1;
                        ZKFPCap.sensorSetParameterEx(g_Handle, 1100, paramValue, 4);//

                        ret = ZKFPCap.sensorInit();
                        if (ret != 0)
                        {
                            MessageBox.Show("Init Failed, ret=" + ret.ToString());
                            return;
                        }
                        g_Handle = ZKFPCap.sensorOpen(0);

                        Array.Clear(paramValue, 0, paramValue.Length);
                        ZKFPCap.sensorGetVersion(paramValue, paramValue.Length);//读取版本号，paramVlaue：保存版本号

                        ret = paramValue.Length;
                        Array.Clear(paramValue, 0, paramValue.Length);
                        ZKFPCap.sensorGetParameterEx(g_Handle, 1, paramValue, ref ret);
                        g_nWidth = BitConverter.ToInt32(paramValue, 0);

                        this.img1.Width = g_nWidth;
                        this.img2.Width = g_nWidth;


                        ret = paramValue.Length;
                        Array.Clear(paramValue, 0, paramValue.Length);
                        ZKFPCap.sensorGetParameterEx(g_Handle, 2, paramValue, ref ret);
                        g_nHeight = BitConverter.ToInt32(paramValue, 0);
                        this.img1.Height = g_nHeight;
                        this.img2.Height = g_nHeight;


                        ret = paramValue.Length;
                        Array.Clear(paramValue, 0, paramValue.Length);
                        ZKFPCap.sensorGetParameterEx(g_Handle, 106, paramValue, ref ret);
                        g_FPBufferSize = BitConverter.ToInt32(paramValue, 0);
                        
                        g_FPBuffer = new byte[g_FPBufferSize];
                        Array.Clear(g_FPBuffer, 0, g_FPBuffer.Length);

                        //绘制指纹
                        // Fingerprint Alg
                        short[] iSize = new short[24];
                        iSize[0] = (short)g_nWidth;
                        iSize[1] = (short)g_nHeight;
                        iSize[20] = (short)g_nWidth;
                        iSize[21] = (short)g_nHeight; 
                        
                        g_biokeyHandle = ZKFinger10.BIOKEY_INIT(0, iSize, null, null, 0);
                        //传入一个无符号双字节长度为 22 的数组，且 isize[0]和isize[20]为传入图像宽度，isize[1]和 isize[21]为传入图像高度
                        //[返回值]> 0 返回指纹算法库句柄    0 初始化失败
                        if (g_biokeyHandle == IntPtr.Zero)
                        {
                            MessageBox.Show("BIOKEY_INIT failed");
                            return;
                        }

                        // Set allow 360 angle of Press Finger
                        ZKFinger10.BIOKEY_SET_PARAMETER(g_biokeyHandle, 4, 180);

                        // Set Matching threshold
                        //设置比对速度和比对阀值:阈值越大越准确
                        ZKFinger10.BIOKEY_MATCHINGPARAM(g_biokeyHandle, 0, ZKFinger10.THRESHOLD_MIDDLE);

                        // Init RegTmps
                        for (int i = 0; i < 3; i++)
                        {
                            g_RegTmps[i] = new byte[2048];
                        }

                        Thread captureThread = new Thread(new ThreadStart(DoCapture));
                        captureThread.IsBackground = true;
                        captureThread.Start();
                        g_bIsTimeToDie = false;
                        
                        g_Connected = true;
                        //btn_Op5.IsEnabled = true;
                        //btn_Op6.IsEnabled = true;
                        MessageBox.Show("请放手指");
                        btn_Op4.IsEnabled = btn_Op5.IsEnabled = btn_Op6.IsEnabled = btn_Op7.IsEnabled = true;
                        btn_Op3.IsEnabled = false;
                    }    
                    img2.Source = null;
                    break;
                case "保存为图片":

                    StateChange(true);
                    // g_bIsTimeToDie = true;
                    CommonSaveFileDialog save = new CommonSaveFileDialog();
                    save.Filters.Add(new CommonFileDialogFilter("bmp文件", "*.bmp"));
                    if (save.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        path = save.FileName.Replace("\\", "/");
                        path += ".bmp";
                        BitmapImage bitmapimage = (BitmapImage)img1.Source;
                        ImageTranslater.SaveBitmapImageIntoFile(bitmapimage, path);
                    }
                    img2.Source = null;
                    break;
                case "指纹登记":
                    StateChange(false);
                    if (textBox_Grade.Text == null || textBox_Name.Text == null || textBox_Sno.Text == null || textBox_Grade.Text == "" || textBox_Name.Text == "" || textBox_Sno.Text == "")
                    {
                        MessageBox.Show("内容不能为空，请重新填写！");
                    }
                    else
                    {
                        FingerDatas fd = new FingerDatas();
                        fd.Name = textBox_Name.Text;
                        fd.Sno = textBox_Sno.Text;
                        fd.Grade = textBox_Grade.Text;
                        fd.PBuffer = g_FPBuffer;
                        using (FingerDataEntities context = new FingerDataEntities())
                        {
                            try
                            {
                                context.FingerDatas.Add(fd);

                                context.SaveChanges();
                                MessageBox.Show("保存成功");
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("数据保存失败，发生错误：" + ex.Message);
                               
                            }
                        }
                    }
                    textBox_Grade.Clear();
                    textBox_Name.Clear();
                    textBox_Sno.Clear();
                    img2.Source = null;
                    break;
                case "指纹识别":
                    StateChange(true);
                    byte[] newFinger = g_FPBuffer;
                    byte[] newFingerModel = new byte[2048];
                    string resultStr;
                    
                    using (FingerDataEntities context = new FingerDataEntities())
                    {
                        try
                        {
                            var q1 = from t in context.FingerDatas
                                     select new {t.Name,t.Sno,t.Grade,t.PBuffer} ;
                            if (q1.Equals(null))
                            {
                                MessageBox.Show("数据库不包含任何指纹信息，请录入指纹信息！" );
                                break;
                            }
                            else
                            {
                                int match = 0;
                                int result_extract = ZKFinger10.BIOKEY_EXTRACT(g_biokeyHandle, newFinger, newFingerModel, 0);
                                if (result_extract > 0)
                                {
                                    foreach(var q in q1)
                                    {
                                        byte[] q_FingerModel = new byte[2048];
                                        byte[] q_Finger = q.PBuffer;
                                        ZKFinger10.BIOKEY_EXTRACT(g_biokeyHandle, q_Finger, q_FingerModel, 0);
                                        match = ZKFinger10.BIOKEY_VERIFY(g_biokeyHandle, newFingerModel, q_FingerModel);
                                        if (match > 0)
                                        {
                                            resultStr = string.Format("识别成功，匹配到的指纹信息为：\n 姓名：{0}\n 学号:{1} \n 年级：{2}  \n 相似度：{3}%", q.Name, q.Sno, q.Grade, match);

                                            MemoryStream ms = new MemoryStream();
                                            BitmapFormat.GetBitmap(q_Finger, g_nWidth, g_nHeight, ref ms);
                                            Bitmap bt = new Bitmap(ms);
                                            BitmapImage bi = ImageTranslater.BitmapToBitmapImage(bt);
                                            img2.Source = bi;
                                            MessageBox.Show(resultStr);
                                            break;
                                        }
                                    }
                                    if (match == 0)
                                    {
                                        resultStr = "数据库中未找到与之相似的指纹信息，请录入信息后再次尝试";
                                        MessageBox.Show(resultStr);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("出现意外");
                                    break;
                                }
                               
                            }
                 
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("发生错误：" + ex.Message);

                        }
                    }
                    break;
                case "停止":
                    StateChange(true);
                    FreeSensor();
                    ZKFinger10.BIOKEY_DB_CLEAR(g_biokeyHandle);
                    ZKFinger10.BIOKEY_CLOSE(g_biokeyHandle);
                    g_Connected = false;
                    btn_Op3.IsEnabled = true;
                    btn_Op4.IsEnabled = btn_Op5.IsEnabled = btn_Op6.IsEnabled = btn_Op7.IsEnabled = false;
                    MessageBox.Show("指纹器已经停止！");
                    break;
                case "退出":
                    this.Close();
                    break;
                case "": break;
            }
        }
        private void DoCapture()
        {
            while (!g_bIsTimeToDie)
            {
                int ret = ZKFPCap.sensorCapture(g_Handle, g_FPBuffer, g_FPBufferSize);
                if (ret > 0)
                {
                    //SendMessage(g_FormHandle, MESSAGE_FP_RECEIVED, IntPtr.Zero, IntPtr.Zero);
                    MemoryStream ms = new MemoryStream();
                    BitmapFormat.GetBitmap(g_FPBuffer, g_nWidth, g_nHeight, ref ms);
                   Bitmap bt = new Bitmap(ms);
                    ShowImg(bt);
                   // pic1.Image = bmp;
                }
            }
        }
        private void ShowImg(Bitmap bt)
        {
            img1.Dispatcher.Invoke(() =>
            {
                img1.Source = ImageTranslater.BitmapToBitmapImage(bt);

            });
        }
        private void FreeSensor()
        {
            g_bIsTimeToDie = true;
            Thread.Sleep(1000);
            ZKFPCap.sensorClose(g_Handle);

            // Disable log
            byte[] paramValue = new byte[4];
            paramValue[0] = 0;
            ZKFPCap.sensorSetParameterEx(g_Handle, 1100, paramValue, 4);

            ZKFPCap.sensorFree();
        }
        private void StateChange(bool b)
        {
            if (b)
            {
                lbl.Visibility = System.Windows.Visibility.Hidden;
                bor.Visibility = System.Windows.Visibility.Hidden;
                lbl1.Visibility = System.Windows.Visibility.Hidden;
                lbl2.Visibility = System.Windows.Visibility.Hidden;
                lbl3.Visibility = System.Windows.Visibility.Hidden;
                textBox_Grade.Visibility = System.Windows.Visibility.Hidden;
                textBox_Name.Visibility = System.Windows.Visibility.Hidden;
                textBox_Sno.Visibility = System.Windows.Visibility.Hidden;

            }
            else
            {
                lbl.Visibility = System.Windows.Visibility.Visible;
                bor.Visibility = System.Windows.Visibility.Visible;
                lbl1.Visibility = System.Windows.Visibility.Visible;
                lbl2.Visibility = System.Windows.Visibility.Visible;
                lbl3.Visibility = System.Windows.Visibility.Visible;
                textBox_Grade.Visibility = System.Windows.Visibility.Visible;
                textBox_Name.Visibility = System.Windows.Visibility.Visible;
                textBox_Sno.Visibility = System.Windows.Visibility.Visible;


            }
        }
    }
}
