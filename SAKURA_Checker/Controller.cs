/**************************************************************************
 * 新增日期：2018年5月14日
 * 作者：数缘科技
 * 
 * 内容说明：此.cs文件主要是进行明文加密、示波器配置、波形采集、波形存储
***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;                                                                   
using PS3000ACSConsole;
using System.Threading;
using System.Runtime.InteropServices;
using System.Data;
using SAKURA;
using System.Windows.Forms;

namespace MathMagic
{
   
    public partial class  Controller
    {
        [DllImport("kernel32.dll",
           CallingConvention = CallingConvention.Winapi)]

        public extern static uint GetTickCount();                   //为后面使用GetTickCount而写的DllImport，用于计时
        public static event DrawLineEventHander drawLine;           //静态事件，传递波形数据
        public static SendErrorMessage sendErrorMessage;            //传递错误信息    
        public static bool IsError = false;                         //当有错误发生时为true，用于检测触发通道是否连接或文件夹是否被占用
        public static int Ratio;                                    //当波形超过1万过点时，压缩成2000点的比例。Ratio=总点数/2000
        public static uint BUFFER_SIZE = 2500;                      //设置buffer大小
        public static int TriggerBeforeNum = 2100;                  //触发前采样点数
        public static int TriggerAfterNum = 400;
        public static DirectoryInfo appPath;                        //应用启动路径
        public static string folderPath;                            //traces&data文件夹路径
        public static short handle;                                 //打开示波器参数
        public const int MAX_CHANNELS = 4;
        public const int QUAD_SCOPE = 4;                            //若是四通道，则_channelCount设为QUAD_SCOPE         
        public const int DUAL_SCOPE = 2;
        public int num;                                             //记录当前采样波形编号
         
        private readonly short _handle;
        private string deleteDatFilePath = "00快速删除当前文件夹下所有文件.bat";
        public string plainTextFilePath = "Plaintext.csv";         //存PlainText文件名字
        public string folderPathLast;//= "\\traces&data";             //存文件夹名字
        private ChannelSettings[] _channelSettings;                 //存示波器通道相关设置
        private int _channelCount;                                  //示波器的通道数
        private uint _timebase =2;                                  //设置采样率
        private short _oversample = 1;
        public static bool isOpen = true;                           //示波器是否打开          
        private bool _ready = false;                                //用于标识示波器是否准备好采集波形数据
        ushort[] inputRanges = { 10, 20, 50, 100, 200, 500, 1000, 2000, 5000, 10000, 20000, 50000 };
        
        private Imports.ps3000aBlockReady _callbackDelegate;        //用于runblock调用的委托
        private BackgroundWorker worker;
        private CipherModule targetModule;                          //硬件加密
        private AES pcModule;                                       //软件加密
        
        public Controller()                                         //sakura程序Controller函数的构造函数
        {
            worker = new BackgroundWorker();                        //BackgroundWorker新建线程
            worker.WorkerReportsProgress = true;                    //为true,可以回调ProgressChanged事件
            worker.WorkerSupportsCancellation = true;               //为true,可以手动终止线程
            
            worker.DoWork += new DoWorkEventHandler(Worker_DoWork); //正式干活的函数！！！
            pcModule = new AES();
            appPath = new DirectoryInfo(string.Format(@"{0}..\..\..\..\..\", Application.StartupPath));
            //获取应用程序启动目录
            
            //2018.10.22
            /*System.DateTime currentTime = new System.DateTime();
            currentTime = System.DateTime.Now;
            folderPathLast = "DSO-X 3034T WaveData "+
                    currentTime.Year.ToString() + "_" +
                    currentTime.Month.ToString() + "_" +
                    currentTime.Day.ToString() + "_"+
                    currentTime.Hour.ToString() + "_" +
                    currentTime.Minute.ToString();
            //Console.Out.WriteLine("folderPathLast :{0} " ,folderPathLast);
            folderPath = appPath.FullName + folderPathLast;
            //Console.Out.WriteLine("folderPath :{0} ", folderPath);*/
        }
        public Controller(short handle)                             //原pico程序PS3000ACSConsole1的构造函数
        {
            _handle = handle;
        }
        //停止命令出错时的处理方法
        private void VISAStopWrong()
        {
            myScope.Close();
            myScope = new VisaComInstrument("USB0::0x2A8D::0x1764::MY56310472::0::INSTR");
            // Get and display the device's *IDN? string.
            string strResults = myScope.DoQueryString("*IDN?");
            Console.WriteLine("stop wrong and try open *IDN? result is: {0}", strResults);
            myScope.DoCommand(":STOP");
            System.Threading.Thread.Sleep(20);
        }
        //VISA读取数据并保存
        private string VISADataSave(long sum,out int maxLength)
        {
            byte[] ResultsArray1, ResultsArray2; // Results array.
            int nLength1, nLength2; // Number of bytes returned from instrument.    
            //int points = 20000;
            // Read waveform data. 
            myScope.DoCommand(":WAVEFORM:POINTS 3000");
            /*Console.WriteLine("CHANNEL3:");
            myScope.DoCommand(":WAVeform:SOURce CHANnel3");*/
            //System.Threading.Thread.Sleep(10);//
            // Read the waveform points mode.
            /*Console.WriteLine("Waveform points available: {0}", myScope.DoQueryString(":WAVeform:POINts?"));
            Console.WriteLine("Waveform points mode: {0}",myScope.DoQueryString(":WAVeform:POINts:MODE?"));
            Console.WriteLine("Waveform SOURce: {0}", myScope.DoQueryString(":WAVeform:SOURce?"));
            Console.WriteLine("Waveform format: {0}",myScope.DoQueryString(":WAVeform:FORMat?"));*/

            /*myScope.SetTimeoutSeconds(200);*/
            // Read waveform data.
            //string opc1 = myScope.DoQueryString("*OPC?");
            //Console.WriteLine("OPC1: {0}", opc1);

            //myScope.CheckInstrumentErrors(":WAVeform: DATA?");//2018.10.21


            /*ResultsArray1 = myScope.DoQueryIEEEBlock(":WAVeform:DATA?");*/
            //System.Threading.Thread.Sleep(10);//

            /*if (opc1.CompareTo("1")==0)
            {
                Console.WriteLine("It will make I/O wrong!");
                //若opc1！=1意味着IO出错，如何进行错误处理？
            }*/
            /*nLength1 = ResultsArray1.Length;
            //Console.WriteLine("Number of data values: {0}", nLength1);

            double[] fResultsArray1;
            myScope.SetTimeoutSeconds(200);
            fResultsArray1 = myScope.DoQueryNumbers(":WAVeform:PREamble?");
            //System.Threading.Thread.Sleep(10);//
            double fFormat1 = fResultsArray1[0];
            //FormatFun(fFormat1);

            double fType1 = fResultsArray1[1];
            //TypeFun(fType1);
            double fPoints1 = fResultsArray1[2];
            //Console.WriteLine("Waveform points: {0:e}", fPoints1);
            double fCount1 = fResultsArray1[3];
            //Console.WriteLine("Waveform average count: {0:e}", fCount1);
            double fXincrement1 = fResultsArray1[4];
            //Console.WriteLine("Waveform X increment: {0:e}", fXincrement1);
            double fXorigin1 = fResultsArray1[5];
            //Console.WriteLine("Waveform X origin: {0:e}", fXorigin1);
            double fXreference1 = fResultsArray1[6];
            //Console.WriteLine("Waveform X reference: {0:e}", fXreference1);
            double fYincrement1 = fResultsArray1[7];
            //Console.WriteLine("Waveform Y increment: {0:e}", fYincrement1);
            double fYorigin1 = fResultsArray1[8];
            //Console.WriteLine("Waveform Y origin: {0:e}", fYorigin1);
            double fYreference1 = fResultsArray1[9];
            //Console.WriteLine("Waveform Y reference: {0:e}", fYreference1);*/

            //channel2
            Console.WriteLine("CHANNEL2:");
            myScope.DoCommand(":WAVeform:SOURce CHANnel2");
            //System.Threading.Thread.Sleep(10);//
            // Read the waveform points mode.
            /*Console.WriteLine("Waveform points available: {0}", myScope.DoQueryString(":WAVeform:POINts?"));
            Console.WriteLine("Waveform points mode: {0}", myScope.DoQueryString(":WAVeform:POINts:MODE?"));
            Console.WriteLine("Waveform SOURce: {0}", myScope.DoQueryString(":WAVeform:SOURce?"));
            Console.WriteLine("Waveform format: {0}", myScope.DoQueryString(":WAVeform:FORMat?"));*/
            myScope.SetTimeoutSeconds(20000);
            // Read waveform data.
            //string opc2 = myScope.DoQueryString("*OPC?");
            //Console.WriteLine("OPC2: {0}", opc2);
            ResultsArray2 = myScope.DoQueryIEEEBlock(":WAVeform:DATA?");
            //System.Threading.Thread.Sleep(10);//

            nLength2 = ResultsArray2.Length;
            //Console.WriteLine("Number of data values: {0}", nLength2);

            double[] fResultsArray2;
            myScope.SetTimeoutSeconds(20000);
            fResultsArray2 = myScope.DoQueryNumbers(":WAVeform:PREamble?");
            //System.Threading.Thread.Sleep(10);//
            double fFormat2 = fResultsArray2[0];
            //FormatFun(fFormat2);

            double fType2 = fResultsArray2[1];
            //TypeFun(fType2);
            double fPoints2 = fResultsArray2[2];
            //Console.WriteLine("Waveform points: {0:e}", fPoints2);
            double fCount2 = fResultsArray2[3];
            //Console.WriteLine("Waveform average count: {0:e}", fCount2);
            double fXincrement2 = fResultsArray2[4];
            //Console.WriteLine("Waveform X increment: {0:e}", fXincrement2);
            double fXorigin2 = fResultsArray2[5];
            //Console.WriteLine("Waveform X origin: {0:e}", fXorigin2);
            double fXreference2 = fResultsArray2[6];
            //Console.WriteLine("Waveform X reference: {0:e}", fXreference2);
            double fYincrement2 = fResultsArray2[7];
            //Console.WriteLine("Waveform Y increment: {0:e}", fYincrement2);
            double fYorigin2 = fResultsArray2[8];
            //Console.WriteLine("Waveform Y origin: {0:e}", fYorigin2);
            double fYreference2 = fResultsArray2[9];
            //Console.WriteLine("Waveform Y reference: {0:e}", fYreference2);

            string strPath;

            Console.WriteLine("sum: {0}", sum.ToString());
            strPath = folderPath + "\\" + "#" + sum.ToString() + "#" + ".csv";
            Console.WriteLine("folderPath={0}", folderPath);
            if (File.Exists(strPath)) File.Delete(strPath);
            // Open file for output.
            StreamWriter writer = File.CreateText(strPath);
            // Output waveform data in CSV format.
            /*if (nLength1 > nLength2) maxLength = nLength1;
            else*/ maxLength = nLength2;
            for (int i = 0; i < maxLength - 1; i++)
            {
                 /*writer.WriteLine("{0:f9}, {1:f9}, {2:f9}",
                 fXorigin1 + ((float)i * fXincrement1),
                 (((double)ResultsArray1[i] - fYreference1) * fYincrement1) + fYorigin1,
                 (((double)ResultsArray2[i] - fYreference2) * fYincrement2) + fYorigin2);*/

                writer.WriteLine("{0:f9}",
                    (((double)ResultsArray2[i] - fYreference2) * fYincrement2) + fYorigin2);

                /*VisaDraw(points, sum, fXincrement1, fXincrement2, fXorigin1 + ((float)i * fXincrement1),
                (((double)ResultsArray1[i] - fYreference1) * fYincrement1) + fYorigin1,
                (((double)ResultsArray2[i] - fYreference2) * fYincrement2) + fYorigin2);*/
            }

            myScope.DoCommand("*CLS");//2018.10.28清除状态数据结构
            myScope.DoCommand(":SINGle");
            System.Threading.Thread.Sleep(20);//
            //System.Threading.Thread.Sleep(100);

            // Close output file.
            writer.Close();

            //存数据点
            /*myScope.fType1 = fType1;
            myScope.fType2 = fType2;

            myScope.fPoints1 = fPoints1;
            myScope.fPoints2 = fPoints2;

            myScope.fCount1 = fCount1;
            myScope.fCount2 = fCount2;

            myScope.fXincrement1 = fXincrement1;
            myScope.fXincrement2 = fXincrement2;

            myScope.fYincrement1 = fYincrement1;
            myScope.fYincrement2 = fYincrement2;

            myScope.fYorigin1 = fYorigin1;
            myScope.fYorigin2 = fYorigin2;

            myScope.fYreference1 = fYreference1;
            myScope.fYreference2 = fYreference2;*/

            //Console.WriteLine("Waveform format BYTE data written to {0}", strPath);

            return strPath;
        }
        //VISA读取数据并保存时错误的处理方法
        private void VISADataSaveWrong(long sum)
        {
            int maxLength;
            myScope.Close();
            myScope = new VisaComInstrument("USB0::0x2A8D::0x1764::MY56310472::0::INSTR");
            // Get and display the device's *IDN? string.
            string strResults = myScope.DoQueryString("*IDN?");
            Console.WriteLine("save wrong and try open *IDN? result is: {0}", strResults);
            VISADataSave(sum,out maxLength);
            System.Threading.Thread.Sleep(20);
        }
        public void ReadCsv(string filename, int Num, out List<double> listStrArr)//20201018读取文件数据
        {
            StreamReader reader = new StreamReader(filename);
            string line = "";
            listStrArr = new List<double>();//数组List，相当于可以无限扩大的二维数组。

            for (int i = 0; i < Num; i++)
            {
                line = reader.ReadLine();
                listStrArr.Add(double.Parse(line));//这里是直接将Temp中的string类型转化为double类型；
                //MessageBox.Show(line);
            }
        }
        public void DrawPower(string filename, int Num,long current_trace)//20201018为了画图
        {
            List<double> listStrArr;//数组List，相当于可以无限扩大的二维数组。
            List<int> xData=new List<int>();//数组List，相当于可以无限扩大的二维数组。
            ReadCsv(filename, Num, out listStrArr);
            for (int i = 0; i < Num; i++)
            {
                xData.Add(i + 2);//为了调整下标从1-1201
                //MessageBox.Show(line);
            }
            //20201019画图
            DrawLineEventArgs ee = new DrawLineEventArgs(current_trace, Num,xData, listStrArr);
            drawLine(this, ee);                              //调用事件绘图

        }

        /// <summary>
        /// 主要的逻辑都在这个方法里面。大概步骤如下： Imports.OpenUnit打开示波器，对示波器进行配置，
        /// 设置通道电压，设置触发条件（通道A上升沿触发），然后Imports.RunBlock设置触发前后的采波数目，
        /// 提示示波器开始准备采集波形。然后开始AES明文加密，SAKURA明文加密，并得到密文。然后Imports.SetDataBuffer
        /// 设置buffer大小，数据储存位置，最后将波形数据储存在磁盘。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
           
            ControllerArgs args = (ControllerArgs)e.Argument;       //e.Argument就是MainForm.cs里面ctrl.Run(args)中args;
            ControllerArgs res = args.Clone();                      //深度复制，res和args是独立的

            

            int progress = 0;                                       //显示采集进度
            e.Cancel = false;
            IsError = false;
            // initialize
            res.last = false;
            res.error = false;
            res.current_trace = 0;
            res.Time = 0;
            // pcModule.SetKey(res.key);                               //设置软件加密密钥
            targetModule.Reset();
            targetModule.SetModeEncrypt(true);                      //设置硬件为加密模式
            // targetModule.SetKey(res.key);
            worker.ReportProgress(0, (object)res);
            //*************************************************************************准备示波器
            //Controller consoleExample = null;
            num = 0;
            res.tmpFRRF = 0;
            //uint numRapidCaptures = 1;
            //test code
            /*if (!MainForm.IsOtherScope)                             //勾选其他示波器,MainForm.IsOtherScope=true;
            {
                consoleExample = new Controller(handle);            //又另外创建了一个类
                consoleExample.Run();                               //示波器配置函数
                
            }*/
            byte[] classNkey = new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0 };
            byte[][] classNplaintext = new byte[32][];
            for (int i = 0; i < 32; i++) { classNplaintext[i] = new byte[16]; }//初始化

            if (res.classificationN)
            {
                
                for (int j = 0; j < res.textNclass; j++)
                {
                    for (int i = 0; i < 16; i++)
                    {
                        //随机生成明文
                        classNplaintext[j][i] =  (byte)res.timerand.Next(256);
                        //Console.Out.WriteLine("timerand {0} " + res.plaintext[i].ToString());
                    }
                }
            }

            byte[] SoftwareEncryption = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            //循环采集波，直到达到要求的波形数
            while (res.endless || res.current_trace < res.traces)
            {
                uint T1 = GetTickCount();                            //计算机启动后的时间间隔，获取采一条波的开始时间，用于加密采波计时
                res.answer = null;
                res.ciphertext = null;
                res.difference = null;
                res.current_trace++;

                
                if (!res.endless)
                {
                    progress = (int)(100 * res.current_trace / res.traces);
                }

                if (res.randomGeneration)                           //勾选明文随机生成后，每次都随机生成明文
                {
                    //res.plaintext = res.rand.generatePlaintext();
                    for (int i = 0; i < 16; i++)
                    {
                        //2018.10.20随机生成明文
                        res.plaintext[i] = (byte)res.timerand.Next(256);
                        //Console.Out.WriteLine("timerand {0} " + res.plaintext[i].ToString());
                    }
                    res.random_plaintext = res.plaintext;
                }

                //2018.10.23
                if (res.alternateGeneration)
                {
                    if (res.current_trace % 2 != 0)
                    {
                        //勾选明文交替生成后，每次都交替生成明文,第一次是输入框的明文，第二次是随机生成的明文
                        //第三次是第一次生成的密文，第四次是随机，第五次是第三次随机生成的。以此类推。
                        /*if (res.current_trace == 1)
                        {
                            res.plaintext = res.fixedplaintext;
                        }
                        else
                        {
                            res.plaintext = res.alternate_ciphertext;
                        }*/

                        //勾选明文交替生成后，每次都交替生成明文,一次是输入框的固定明文，一次是随机明文
                        for (int i = 0; i < 16; i++)
                        {
                            res.plaintext[i] = res.fixedplaintext[i];
                        }

                    }
                    else 
                    {
                        for (int i = 0; i < 16; i++)
                        {
                            res.plaintext[i] = (byte)res.timerand.Next(256);
                        }
                        res.random_plaintext = res.plaintext;
                    }
                    
                }
                //2018.4.8 FRRF交替明文,第一次是固定，第二次第三次随机，第四次固定

                if (res.FRRFGeneration)
                {
                    res.tmpFRRF++;
                    if (res.tmpFRRF == 1|| res.tmpFRRF == 4)//固定
                    {
                        for (int i = 0; i < 16; i++)
                        {
                            res.plaintext[i] = res.fixedplaintext[i];
                        }
                        if (res.tmpFRRF == 4) res.tmpFRRF = 0;

                    }
                    else//随机
                    {
                        for (int i = 0; i < 16; i++)
                        {
                            res.plaintext[i] = (byte)res.timerand.Next(256);
                        }
                        res.random_plaintext = res.plaintext;
                    }
                }
                //2019.4.8 FR各50%的机会交替
                if(res.RFGeneration)
                {
                    int tmp = res.timerand.Next(2);//0则随机，1则固定
                    SaveIntTextData(tmp,"RFrecord.txt");

                    if (tmp==0)//随机
                    {
                        for (int i = 0; i < 16; i++)
                        {
                            res.plaintext[i] = (byte)res.timerand.Next(256);
                        }
                        res.random_plaintext = res.plaintext;
                    }
                    else//固定
                    {
                        for (int i = 0; i < 16; i++)
                        {
                            res.plaintext[i] = res.fixedplaintext[i];
                        }
                    }
                }
                //2019.5.15 明文F'FF'F循环
                if (res.FFFFGeneration)
                {
                    res.tmpFRRF++;
                    if (res.tmpFRRF == 1 || res.tmpFRRF == 4)//固定1
                    {
                        for (int i = 0; i < 16; i++)
                        {
                            res.plaintext[i] = res.fixedplaintext2[i];

                        }
                        if (res.tmpFRRF == 4) res.tmpFRRF = 0;

                    }
                    else//固定2
                    {
                        for (int i = 0; i < 16; i++)
                        {
                            res.plaintext[i] = res.fixedplaintext[i];

                        }
                    }
                }
                //2019.5.15 明文FF'循环,2023/11/7,目前已经把这个改为明文密钥全随机的模式
                if (res.FFGeneration)
                {   //  进入FF'


                    for (int i = 0; i < 16; i++)
                    {
                        res.plaintext[i] = (byte)res.timerand.Next(256);
                    }
                    res.random_plaintext = res.plaintext;   //随机明文
                    for (int i = 0; i < 16; i++)
                    {
                        res.key[i] = (byte)res.timerand.Next(256);
                    }
                    res.random_key = res.key;       //随机密钥
                }
                //    res.tmpFRRF++;
                //    if (res.tmpFRRF == 1)//固定1
                //    {
                //        for (int i = 0; i < 16; i++)
                //        {
                //            res.plaintext[i] = res.fixedplaintext2[i];
                //        }
                //    }
                //    else//固定2
                //    {
                //        for (int i = 0; i < 16; i++)
                //        {
                //            res.plaintext[i] = res.fixedplaintext[i];
                //        }
                //        res.tmpFRRF = 0;
                //    }
                //}

                //if (res.FFGeneration)
                //{
                //    res.tmpFRRF++;
                //    if (res.tmpFRRF == 1)//固定1
                //    {
                //        for (int i = 0; i < 16; i++)
                //        {
                //            res.plaintext[i] = res.fixedplaintext2[i];
                //        }
                //    }
                //    else//固定2
                //    {
                //        for (int i = 0; i < 16; i++)
                //        {
                //            res.plaintext[i] = res.fixedplaintext[i];
                //        }
                //        res.tmpFRRF = 0;
                //    }
                //}

                //20200622指定四分类
                if (res.classification4)
                {
                    byte[] key1 = new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0 };
                    byte[] plaintext1 = new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10 };
                    //byte[] plaintext1 = new byte[] { 0x8C, 0x4E, 0xB1, 0xCD, 0x3B, 0xC6, 0x4B, 0x94, 0x0F, 0xA4, 0xDC, 0xCB, 0x0D, 0x7D, 0xFC, 0x83 };
                    //byte[] plaintext1 = new byte[] { 0x5B, 0x16, 0xC4, 0x6C, 0xE0, 0x51, 0xF1, 0x8B, 0x89, 0xF8, 0x28, 0x68, 0x3D, 0x19, 0xB6, 0xED };


                    byte[] key2 = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0, 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF };
                    //byte[] plaintext2 = new byte[] { 0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10, 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF };
                    byte[] plaintext2 = new byte[] { 0xda, 0x39, 0xa3, 0xee, 0x5e, 0x6b, 0x4b, 0x0d, 0x32, 0x55, 0xbf, 0xef, 0x95, 0x60, 0x18, 0x90 };
                    //byte[] plaintext2 = new byte[] { 0x8D, 0x1A, 0x8D, 0x60, 0x6D, 0x2A, 0x46, 0xFC, 0x0C, 0x83, 0xFD, 0x6A, 0x21, 0x61, 0xF8, 0x4F };

                    byte[] key3 = new byte[] { 0x89, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0, 0x01, 0x23, 0x45, 0x67 };
                    //byte[] plaintext3 = new byte[] { 0x89, 0xAB, 0xCD, 0xEF, 0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10, 0x01, 0x23, 0x45, 0x67 };
                    //byte[] plaintext3 = new byte[] { 0x48, 0x89, 0xF2, 0x38, 0xE9, 0xC3, 0xDF, 0x75, 0x0D, 0x97, 0xA5, 0xE3, 0x27, 0x99, 0xA7, 0x4E };
                    //byte[] plaintext3 = new byte[] { 0x2A, 0xC1, 0xB7, 0x32, 0x13, 0x5C, 0x52, 0xCF, 0xE5, 0x19, 0xFB, 0x03, 0xC1, 0xE7, 0xDC, 0x55 };
                    byte[] plaintext3 = new byte[] { 0xC5, 0x90, 0x48, 0x00, 0xF7, 0xB2, 0x96, 0x43, 0x1B, 0x8A, 0x89, 0x00, 0x58, 0x6C, 0xE7, 0x0F };

                    byte[] key4 = new byte[] { 0xAB, 0xCD, 0xEF, 0x12, 0x01, 0x23, 0x45, 0x67, 0x89, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0 };
                    byte[] plaintext4 = new byte[] { 0x81, 0x67, 0xC5, 0x0B, 0xA5, 0xC4, 0x47, 0x9E, 0xBA, 0xD6, 0x56, 0x3E, 0x12, 0x28, 0x2B, 0x60 };
                    //byte[] plaintext4 = new byte[] { 0x59, 0x08, 0xD3, 0xB5, 0x4A, 0xAC, 0x5E, 0x40, 0xF1, 0x74, 0x49, 0x3F, 0x97, 0xB9, 0x58, 0xE2 };

                    byte[] plaintext5 = new byte[] { 0x08, 0x93, 0xB3, 0xB0, 0x93, 0x11, 0x87, 0xC6, 0x40, 0xEA, 0x74, 0xF4, 0xF0, 0xCC, 0x85, 0xC7 };
                    byte[] plaintext6 = new byte[] { 0x94, 0x21, 0x7A, 0x68, 0xC9, 0x90, 0xBD, 0xFE, 0x1A, 0x31, 0x20, 0x41, 0xB5, 0x79, 0xC5, 0x57 };
                    byte[] plaintext7 = new byte[] { 0x07, 0x03, 0xFD, 0xCE, 0xDB, 0x10, 0x83, 0x86, 0x45, 0xFE, 0xB0, 0x62, 0x6F, 0xAC, 0xAD, 0x79 };
                    byte[] plaintext8 = new byte[] { 0xDC, 0xBA, 0x98, 0x76, 0x89, 0xAB, 0xCD, 0xEF, 0xFE, 0x54, 0x32, 0x10, 0x01, 0x23, 0x45, 0x67 };

                    byte[] plaintext9 = new byte[] { 0xC9, 0x90, 0xBD, 0xFE, 0x1A, 0x31, 0x20, 0x76, 0x54, 0x32, 0x10, 0x41, 0xAB, 0xCD, 0x31, 0xEF };
                    byte[] plaintext10 = new byte[] { 0x32, 0x10, 0xAB, 0xFE, 0x1A, 0x31, 0x08, 0x93, 0xB3, 0xB0, 0x93, 0x11, 0xAB, 0x23, 0x45, 0x67 };
                    res.tmpFRRF++;
                    switch(res.tmpFRRF)
                    {
                        case 1:
                            res.plaintext = plaintext1;
                            res.key = key1;
                            break;
                        case 2:
                            
                            res.plaintext = plaintext2;
                            res.key = key2;
                            
                            break;
                        case 3:
                            
                            res.plaintext = plaintext3;
                            res.key = key3;
                            
                            break;
                        case 4:
                           /*
                            for (int i = 0; i < 16; i++)
                            {
                                plaintext4[i] = (byte)res.timerand.Next(256);
                            }
                           */
                            res.plaintext = plaintext4;
                            res.key = key4;
                            res.tmpFRRF = 0;
                            break;
                        /*case 5:
                            for (int i = 0; i < 16; i++)
                            {
                                plaintext6[i] = (byte)res.timerand.Next(256);
                            }
                            res.plaintext = plaintext6;
                            res.key = key3;
                            res.tmpFRRF = 0;
                            break;
                        /*case 6:
                            
                            res.plaintext = plaintext6;
                            res.key = key1;
                            
                            break;
                        case 7:
                            for (int i = 0; i < 16; i++)
                            {
                                plaintext7[i] = (byte)res.timerand.Next(256);
                            }
                            res.plaintext = plaintext7;
                            res.key = key1;
                            res.tmpFRRF = 0;
                            break;
                            /*res.key = key1;
                                         for (int i = 0; i < 16; i++)
                                         {
                                             plaintext5[i] = (byte)res.timerand.Next(256);
                                         }
                                         res.plaintext = plaintext5;

                                         break;
                                     case 6:
                                         res.plaintext = plaintext6;
                                         res.key = key1;
                                         break;
                                     case 7:
                                         res.plaintext = plaintext7;
                                         res.key = key1;
                                         break;
                                     case 8:
                                         res.plaintext = plaintext8;
                                         res.key = key1;
                                         break;
                                     case 9:
                                         res.plaintext = plaintext9;
                                         res.key = key1;
                                         break;
                                     case 10:
                                         res.plaintext = plaintext10;
                                         res.key = key1;
                                         res.tmpFRRF = 0;
                                         break;*/
                    }

                }

                //20200622指定八分类
                if (res.classification8)
                {
                    /*
                    res.tmpFRRF++;
                    int n = res.tmpFRRF + 0;

                    byte[] key = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    byte[] plaintext = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                    plaintext[0] = (byte)((n / 16) * 16);

                    key[0] = (byte)res.timerand.Next(16);
                    key[0] = (byte)(((n % 16) * 16) + key[0]);
                    for (int i = 1; i < 16; i++)
                    {
                        key[i] = (byte)res.timerand.Next(256);
                    }

                    if (n == 256)
                    {
                        res.tmpFRRF = 0;
                    }
                    res.plaintext = plaintext;
                    res.key = key;
                    */
                    
                    byte[] key1 = new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0 };
                    byte[] plaintext1 = new byte[] { 0x8C, 0x4E, 0xB1, 0xCD, 0x3B, 0xC6, 0x4B, 0x94, 0x0F, 0xA4, 0xDC, 0xCB, 0x0D, 0x7D, 0xFC, 0x83 };

                    byte[] key2 = new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    byte[] plaintext2 = new byte[] { 0xda, 0x39, 0xa3, 0xee, 0x5e, 0x6b, 0x4b, 0x0d, 0x32, 0x55, 0xbf, 0xef, 0x95, 0x60, 0x18, 0x90 };

                    byte[] key3 = new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    byte[] plaintext3 = new byte[] { 0xC5, 0x90, 0x48, 0x00, 0xF7, 0xB2, 0x96, 0x43, 0x1B, 0x8A, 0x89, 0x00, 0x58, 0x6C, 0xE7, 0x0F };

                    byte[] key4 = new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    byte[] plaintext4 = new byte[] { 0x81, 0x67, 0xC5, 0x0B, 0xA5, 0xC4, 0x47, 0x9E, 0xBA, 0xD6, 0x56, 0x3E, 0x12, 0x28, 0x2B, 0x60 };

                    byte[] key5 = new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0 };
                    byte[] plaintext5 = new byte[] { 0x08, 0x93, 0xB3, 0xB0, 0x93, 0x11, 0x87, 0xC6, 0x40, 0xEA, 0x74, 0xF4, 0xF0, 0xCC, 0x85, 0xC7 };

                    byte[] key6 = new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0 };
                    byte[] plaintext6 = new byte[] { 0x94, 0x21, 0x7A, 0x68, 0xC9, 0x90, 0xBD, 0xFE, 0x1A, 0x31, 0x20, 0x41, 0xB5, 0x79, 0xC5, 0x57 };

                    byte[] key7 = new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0 };
                    byte[] plaintext7 = new byte[] { 0x07, 0x03, 0xFD, 0xCE, 0xDB, 0x10, 0x83, 0x86, 0x45, 0xFE, 0xB0, 0x62, 0x6F, 0xAC, 0xAD, 0x79 };

                    byte[] key8 = new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0 };
                    byte[] plaintext8 = new byte[] { 0xDC, 0xBA, 0x98, 0x76, 0x89, 0xAB, 0xCD, 0xEF, 0xFE, 0x54, 0x32, 0x10, 0x01, 0x23, 0x45, 0x67 };

                    byte[] key9 = new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0 };
                    byte[] plaintext9 = new byte[] { 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                    byte[] key10 = new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0 };
                    byte[] plaintext10 = new byte[] { 0xA0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                    byte[] key11 = new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0 };
                    byte[] plaintext11 = new byte[] { 0xB0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                    byte[] key12 = new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0 };
                    byte[] plaintext12 = new byte[] { 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                    byte[] key13 = new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0 };
                    byte[] plaintext13 = new byte[] { 0xD0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                    byte[] key14 = new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0 };
                    byte[] plaintext14 = new byte[] { 0xE0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                    byte[] key15 = new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0 };
                    byte[] plaintext15 = new byte[] { 0xF0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                    byte[] key16 = new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0 };
                    byte[] plaintext16 = new byte[] { 0xF0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };



                    res.tmpFRRF++;
                    switch (res.tmpFRRF)
                    {
                        case 1:
                            res.plaintext = plaintext1;
                            res.key = key1;
                            break;
                        case 2:
                            res.plaintext = plaintext2;
                            res.key = key1;
                            break;
                        case 3:
                            res.plaintext = plaintext3;
                            res.key = key1;
                            break;
                        case 4:
                            res.plaintext = plaintext4;
                            res.key = key1;
                            break;
                        case 5:
                            res.plaintext = plaintext5;
                            res.key = key1;
                            break;
                        case 6:
                            res.plaintext = plaintext6;
                            res.key = key1;
                            break;
                        case 7:
                            res.plaintext = plaintext7;
                            res.key = key1;
                            break;
                        case 8:
                            res.plaintext = plaintext8;
                            res.key = key1;
                            res.tmpFRRF = 0;
                            break;
                    }
                    /*
                case 9:
                    res.plaintext = plaintext9;
                    res.key = key1;
                    break;
                case 10:
                    res.plaintext = plaintext10;
                    res.key = key1;
                    break;
                case 11:
                    res.plaintext = plaintext11;
                    res.key = key1;
                    break;
                case 12:
                    res.plaintext = plaintext12;
                    res.key = key1;
                    break;
                case 13:
                    res.plaintext = plaintext13;
                    res.key = key1;
                    break;
                case 14:
                    res.plaintext = plaintext14;
                    res.key = key1;
                    break;
                case 15:
                    res.plaintext = plaintext15;
                    res.key = key1;
                    break;
                case 16:
                    res.plaintext = plaintext16;
                    res.key = key1;
                    res.tmpFRRF = 0;
                    break;

            }
            */
                }
                //指定N分类
                if (res.classificationN)
                {
                    byte[] tmpplaintext = new byte[16];
                    if(res.tmpFRRF + 1 == res.textNclass)//R
                    {
                        for (int i = 0; i < 16; i++)
                        {
                            tmpplaintext[i] = (byte)res.timerand.Next(256);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 16; i++)
                        {
                            tmpplaintext[i] = classNplaintext[res.tmpFRRF][i];
                        }
                    }

                    if (res.tmpFRRF + 1 == res.textNclass) res.tmpFRRF = 0;
                    else res.tmpFRRF++;

                    //Console.WriteLine("s={0}\n", res.tmpFRRF);

                    res.plaintext = tmpplaintext;
                    res.key = classNkey;
                    

                }
                //20200915根据ISO17825产生随机明文，第一次加密使用全0，后一个的输入是前一个的输出
                if (res.PseudoRandom)
                {
                    byte[] key = new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0 };
                    res.key = key;

                    if (res.current_trace<=4)//第一次加密使用全0
                    {
                        byte[] tmpplaintext = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                        res.plaintext = tmpplaintext;
                    }
                    else//后一个的输入是前一个的输出
                    {
                        byte[] tmpplaintext = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                        //res.plaintext = tmpplaintext;
                        res.plaintext = SoftwareEncryption;
                    }
                }


                if (res.randomGeneration_mask)                      //勾选明文掩码随机生成后，每次都随机生成明文掩码                     
                {
                    res.plaintext_mask = res.rand.generatePlaintext();
                }
                int k = 0;                                          //记录重试的次数

                do {
                                                         
                    IsError = false;
                    if (!MainForm.IsOtherScope)                     //如果是其他示波器，则不启动PicoScope
                    {
                        //consoleExample.RapidBlockDataHandler1(numRapidCaptures);
                        //通过RunBlock设置触发前和触发后采样点数
                        num++;                                      //采集的波形编号加1 

                    }
                    _ready = false;
                    uint t1 = GetTickCount();
                    uint t2 = GetTickCount();
                    //字节异或，掩码！20200924
                    byte[] result = new byte[res.plaintext.Length];
                    for (int i = 0; i < res.plaintext.Length; i++)
                        result[i] = (byte) (res.plaintext[i] ^ res.plaintext_mask[i]);

                    pcModule.SetKey(res.key);                               //设置软件加密密钥
                    targetModule.SetKey(res.key);       //硬件加密

                    //软件加密
                    //pcModule.Encrypt(ref res.answer, res.key, res.plaintext);
                    pcModule.Encrypt(ref res.answer, res.key, result);
                    SoftwareEncryption = res.answer;
                    //SAKURA硬件加密
                    targetModule.Run(ref res.ciphertext, res.plaintext, res.wait, ref res.elapsed, res.plaintext_mask);
                    //比较软件加密和硬件加密的不同
                    res.diff = Utils.differenceByteArray(ref res.difference, res.answer, res.ciphertext);
                    if (!MainForm.IsOtherScope)                     //如果启用其他示波器，则不启用PicoScope
                    {
                        //RapidBlockDataHandler2在加密过程中显示波形，储存波形数据
                        //consoleExample.RapidBlockDataHandler2(t1, ref t2, numRapidCaptures, num);

                        


                        //2018.10.14
                        if (IsError)                                //RunBlock没有在1.9秒内设置成功，IsError=true;
                        {
                            k++;                        
                            num--;                                  //采波失败，波形编号-1；
                            if (k > 5)
                            {
                                res.error = true;
                                e.Result = (object)res;             //将res传递给UI线程，res里面包含加密时间，明文，密文，加密进度等信息
                                sendErrorMessage("5次重试失败，请检查通道A连线或其他连线", true);
                                return;
                            }
                            sendErrorMessage("正在进行第" + k + "次重试", true);
                        }

                    }
                } while (IsError);
            

                if (res.diff)                                       //硬件加密和软件加密结果不一样时，
                {                                                   //若勾选了错误时继续，则程序继续执行
                    res.error = true;                               //否则，退出循环
                    if (!res.continueIfError)                       
                    {
                        Console.Out.WriteLine("continueIfError");
                        break;
                    }
                }

                if (worker.CancellationPending)                     //点击停止采波，worker.CancellationPending                       
                {
                    Console.Out.WriteLine("CancellationPending");                                                  //设为true，退出循环
                    e.Cancel = true;
                    break;
                }

                if (res.single)
                {
                    progress = 100;
                    break;
                }

                uint T2 = GetTickCount();                           //获取采一条波的结束时间
                res.Time = T2 - T1;                                 //单位是毫秒 T毫秒 输出采一条波要T毫秒        

                //2018.10.24记录交替明文时所需的密文
                if (res.current_trace%2!=0)
                {
                    res.alternate_ciphertext = res.ciphertext;
                }
                Console.WriteLine("write current :{0}", res.current_trace);

                SaveTextData1(res.current_trace);                   // 把每次加密的编号写入Plaintext.csv
                SaveTextData(res.key);                              // 把密钥写入Plaintext.csv
                SaveTextData(res.plaintext);                        // 把明文写入Plaintext.csv
                SaveTextData(res.plaintext_mask);                   // 把明文掩码写入Plaintext.csv
                SaveTextData(res.ciphertext);                       // 把密文写入Plaintext.csv

                 try
                 {
                     myScope.DoCommand(":STOP");
                 }
                 catch (System.ApplicationException err)
                 {
                     Console.WriteLine("*** VISA STOP COM Error Application Exception: " + err.Message);
                     //VISAStopWrong();
                 }
                 catch (System.SystemException err)
                 {
                     Console.WriteLine("*** STOP System Exception Error Message : " + err.Message);//System.IO.IOException	发生I/O错误时引发的异常。
                     //VISAStopWrong();
                 }
                 catch (System.Exception err)
                 {
                     Console.WriteLine("*** STOP Exception Error : " + err.Message);
                     //VISAStopWrong();
                 }
                worker.ReportProgress(progress, (object)res);       //传递进度progress,结构res更新UI
                
                Console.WriteLine("go to visa current :{0}", res.current_trace);
                int maxLength;
                string filenmame=VisaDataGet(res.current_trace, out maxLength);
               
                Console.WriteLine("filenmame :{0}", filenmame);
                //DrawPower(filenmame, maxLength-1, res.current_trace);
                System.Threading.Thread.Sleep(85);

                

            }//while循环加密结束


            res.last = true;
            worker.ReportProgress(progress, (object)res);
            e.Result = (object)res;                                                         
        }//worker_DoWork结束，这里执行完后，会在执行一次worker_RunWorkerCompleted

        /// <summary>
        /// 将波形编号，明文，明文掩码，密文存入Plaintext.csv
        /// </summary>
        /// <param name="byte_buffer">储存内容的字节数组形式</param>
        private void SaveTextData1(long n)
        {
            string hex_String = n.ToString("D8");
            /*if (!Directory.Exists(folderPath))                      //如果路径不存在,创建这个路径的文件夹
            {
                Directory.CreateDirectory(folderPath);
            }*/
            if (false == System.IO.Directory.Exists(folderPath))
            {
                //创建文件夹
                System.IO.Directory.CreateDirectory(folderPath);
            }
            StreamWriter sw = new StreamWriter(Path.Combine(folderPath, plainTextFilePath), true);
            sw.Write(hex_String);
            sw.WriteLine();
            sw.Flush();
            sw.Close();
        }
        private void SaveIntTextData(long n,string path)
        {
            string hex_String = n.ToString();
            /*if (!Directory.Exists(folderPath))                      //如果路径不存在,创建这个路径的文件夹
            {
                Directory.CreateDirectory(folderPath);
            }*/
            if (false == System.IO.Directory.Exists(folderPath))
            {
                //创建文件夹
                System.IO.Directory.CreateDirectory(folderPath);
            }
            StreamWriter sw = new StreamWriter(Path.Combine(folderPath, path), true);
            sw.Write(hex_String);
            //sw.WriteLine();
            sw.Flush();
            sw.Close();
        }
        public void SaveTextData(byte[] byte_buffer)                                            
        {
            // 数据被写到Plaintext.csv里，进来的是byte数组，输出的是16进制的string，一个数存一行
            string hex_String = string.Empty;
            if (byte_buffer != null)
            {
                StringBuilder strB = new StringBuilder();
                for (int i = 0; i < byte_buffer.Length; i++)
                    strB.Append(byte_buffer[i].ToString("X2"));     //X表示16进制，2表示两位16进制   
                hex_String = strB.ToString();
            }
            /*if (!Directory.Exists(folderPath))                      //如果路径不存在,创建这个路径的文件夹
            {
                Directory.CreateDirectory(folderPath);
            }*/
            if (false == System.IO.Directory.Exists(folderPath))
            {
                //创建文件夹
                System.IO.Directory.CreateDirectory(folderPath);
            }
            StreamWriter sw = new StreamWriter(Path.Combine(folderPath, plainTextFilePath), true);
            sw.Write(hex_String);
            sw.WriteLine();
            sw.Flush();
            sw.Close();
        }                                                           // 自定义文件存储函数结束


        /****************************************************************************
         * 设置buffer大小，储存波形数据，显示波形，更改储存波形位置以及显示波形频率在这里修改
         ****************************************************************************/
        public void RapidBlockDataHandler2(uint t1, ref uint t2, uint nRapidCaptures, int num)//num为波形编号
        {
            uint status;
            int numChannels = _channelCount;
            uint numSamples = BUFFER_SIZE;
            int enableNumChannels = 0;
            while (!_ready && t2 - t1 < 1000)                       //RunBlock设置成功后，将调用_callbackDelegate委托，
            {                                                       //将_ready将设为true。这里设置1.9秒等待时间，若1.9秒后
                t2 = GetTickCount();                                //_ready仍为false，则认为触发通道或其他通道没接好 
            }
            if (!_ready)
            {
                Imports.Stop(_handle);                              //退出示波器
                IsError = true;
                return;
            }

            else
            {
                // 储存波形的数组，根据采样点数设置数组大小
                short[][][] values = new short[nRapidCaptures][][];
                //波形数据存在这个数组里面
                PinnedArray<short>[,] pinned = new PinnedArray<short>[nRapidCaptures, numChannels];

                for (ushort segment = 0; segment < nRapidCaptures; segment++)
                {
                    values[segment] = new short[numChannels][];
                    for (short channel = 0; channel < numChannels; channel++)
                    {
                        if (_channelSettings[channel].enabled)
                        {
                            enableNumChannels++;
                            values[segment][channel] = new short[numSamples];
                            pinned[segment, channel] = new PinnedArray<short>(values[segment][channel]);

                            //设置buffer大小，数据储存的位置等
                            status = Imports.SetDataBuffer(_handle, (Imports.Channel)channel, values[segment][channel],
                                (int)numSamples, segment, Imports.RatioMode.None);
                            
                        }
                        else
                        {
                            status = Imports.SetDataBuffer(_handle, (Imports.Channel)channel, null, 0, segment, Imports.RatioMode.None);

                        }
                    }
                }

                // Read the data
                short[] overflows = new short[nRapidCaptures];
                //数据收集完成后，得到的时间反馈信息，包括真正得到的numSamples数量。
                status = Imports.GetValuesRapid(_handle, ref numSamples, 0, (ushort)(nRapidCaptures - 1), 1, Imports.RatioMode.None, overflows);
                for (int i = 0; i < 50; i++)
                {
                    if (pinned[0, 1].Target[i] != 0)
                        break;
                    if (i == 49)
                    {
                        IsError = true;
                        return;
                    }
                }
                //numSamples和BUFFER_SIZE大小可能会不一样，按最小的
                numSamples = Math.Min(numSamples, BUFFER_SIZE);


                if (num == 1 || num % 20 == 0)                          //第一条和每20条显示波形
                {
                    DataTable dt = new DataTable();                 //用DataTable数据结构容纳波形数据
                    dt.Columns.Add("channelA");                     //这个表有channelA和channelB两列
                    dt.Columns.Add("channelB");

                    for (int seg = 0; seg < nRapidCaptures; seg++)
                    {

                        for (int i = 0; i < numSamples;)
                        {

                            if (numSamples < 10000)
                            {
                                Ratio = 1;
                                DataRow drS = dt.NewRow();          //dt表增加一个空白行
                                if (_channelSettings[0].enabled)
                                {
                                    //将ADC计数转化为毫伏值
                                    drS[0] = adc_to_mv(pinned[seg, 0].Target[i],
                                        (int)_channelSettings[(int)(Imports.Channel.ChannelA + 0)].range);
                                }
                                if (_channelSettings[1].enabled)
                                {
                                    drS[1] = adc_to_mv(pinned[seg, 1].Target[i],
                                        (int)_channelSettings[(int)(Imports.Channel.ChannelA + 1)].range);
                                }
                                i++;
                                dt.Rows.Add(drS);                   //将一行数据填入表中对应行
                            }
                            /*
                             * 超过10000个点，则压缩成2000个点，如10000个点，则每隔10个点，选一个最大值
                             * 点和一个最小值点。
                             */
                            else
                            {
                                Ratio = (int)numSamples / 2000;     //计算点筛选比例      
                                float[] dataA = new float[Ratio * 2];
                                float[] dataB = new float[Ratio * 2];
                                for (int k = 0; k < Ratio * 2; k++)
                                {
                                    dataA[k] = adc_to_mv(pinned[seg, 0].Target[i],
                                        (int)_channelSettings[(int)(Imports.Channel.ChannelA + 0)].range);
                                    dataB[k] = adc_to_mv(pinned[seg, 1].Target[i],
                                        (int)_channelSettings[(int)(Imports.Channel.ChannelA + 1)].range);

                                }
                                Array.Sort(dataA);                  //将数组dataA由大到小排序
                                Array.Sort(dataB);                  //将数组dataB由大到小排序
                                DataRow drL1 = dt.NewRow();
                                drL1[0] = dataA[Ratio * 2 - 1];
                                drL1[1] = dataB[Ratio * 2 - 1];
                                dt.Rows.Add(drL1);
                                DataRow drL2 = dt.NewRow();
                                drL2[0] = dataA[0];
                                drL2[1] = dataB[0];
                                dt.Rows.Add(drL2);
                                i = i + Ratio * 2;
                            }

                        }
                    }
                    DrawLineEventArgs e = new DrawLineEventArgs(dt, num);
                    drawLine(this, e);                              //调用事件绘图
                }

                string filePath = "Trace" + num.ToString().PadLeft(6, '0') + ".csv";
                // 把波形数据存入traces&data文件夹;
                //if (!Directory.Exists(folderPath))                  //如果路径不存在
                if (false == System.IO.Directory.Exists(folderPath))
                {

                    //创建文件夹
                    System.IO.Directory.CreateDirectory(folderPath);
                    //Directory.CreateDirectory(folderPath);          //创建一个路径的文件夹
                    StreamWriter sw = new StreamWriter(Path.Combine(folderPath,deleteDatFilePath));
                    sw.WriteLine("del Plaintext.csv");              //写入windows批处理，快速删除文件
                    sw.WriteLine("del Trace*.csv");
                    sw.Close();
                }
                TextWriter writer1 = null;
                try
                {
                    writer1 = new StreamWriter(Path.Combine(folderPath, filePath));
                for (int seg = 0; seg < nRapidCaptures; seg++)
                {
                    for (int i = 0; i < numSamples; i++)
                    {
                        if (_channelSettings[0].enabled)
                        {
                            //向文件写入波形数据，保留小数点后四位
                            writer1.Write("{0:000.0000},",
                                            adc_to_mv(pinned[seg, 0].Target[i],
                                            (int)_channelSettings[(int)(Imports.Channel.ChannelA + 0)].range));
                        }
                        if (_channelSettings[1].enabled)
                        {
                            //波形数据存在Pinned里面，存的是16位ADC形式
                            writer1.Write("{0:000.0000},",
                                            adc_to_mv(pinned[seg, 1].Target[i],
                                            (int)_channelSettings[(int)(Imports.Channel.ChannelA + 1)].range));
                        }
                        writer1.WriteLine();                        //换行
                    }
                }
                writer1.Flush();
                writer1.Close();
                }
                 catch (Exception ex)
                {
                    IsError = true;
                    sendErrorMessage(ex.Message, true);              //文件占用，不能将波形数据写入文件
                    return;
                }
                // Un-pin the arrays
                foreach (PinnedArray<short> p in pinned)            //清除内存占用
                {
                    if (p != null)
                        p.Dispose();
                }
            }
            Imports.Stop(handle);
        }//RapidBlockDataHandler2在这里结束



        /****************************************************************************
         * 检查示波器是否准备好，准备好就用RunBlock设置触发前采多少波
         ***************************************************************************/
        public void RapidBlockDataHandler1(uint nRapidCaptures)
        {
            int numChannels = _channelCount;
            int timeIndisposed;
            _ready = false;                                         //用于检测波形是否Ready

            _callbackDelegate = BlockCallback;                      //BlockCallback注册委托

            //设置RunBlock触发前采多少波
            Imports.RunBlock(_handle,
                        TriggerBeforeNum,                           //触发前采样点数，要改触发前后采样点数，在这里修改
                        TriggerAfterNum,                            //触发后采样点数
                        _timebase,
                        _oversample,
                        out timeIndisposed,
                        0,
                        _callbackDelegate,                          //RunBlock设置成功后，将调用_callbackDelegate委托，
                        IntPtr.Zero);                               //也就是会调用BlockCallback函数
        }


        //*******************************************************************************************************************

        public void Open(uint index)
        {
            targetModule = new CipherModule(index);                 //用于硬件加密
        }

        public void Close()
        {
            targetModule.Dispose();
        }

        public void AddCompletedEventHandler(RunWorkerCompletedEventHandler handler)
        {
            worker.RunWorkerCompleted += handler;                   //DoWork完成后触发，想要采波完成后做一些事情，可在这添加
        }

        public void AddProgressChangedEventHandler(ProgressChangedEventHandler handler)
        {
            worker.ProgressChanged += handler;                      //添加进度监听
        }
        public void Run(ControllerArgs args)
        {
            worker.RunWorkerAsync((object)args);                    //启动BackgroundWorker线程（也就是加密采波线程），随后自动执行Worker_DoWork函数
                                                                    //！！！注：Worker_DoWork函数是主要的加密采波函数，请用户关注          
        }

        public void Cancel()
        {
            Console.Out.WriteLine("Worker_DoWork exit!!");
            worker.CancelAsync();                                   //该方法只会将BackGroundWorker的CancellationPending
                                                                    //属性设为true，但不会实际终止线程！Worker_DoWork会
        }                                                           //检测CancellationPending的值，若为true，则退出Worker_DoWork。
        public void VisaDraw(int points, long nRapidCaptures, double x, double y, double xbuf, double y1buf, double y2buf)
        {
            DataTable dt = new DataTable();                 //用DataTable数据结构容纳波形数据
            dt.Columns.Add("channelA");                     //这个表有channelA和channelB两列
            dt.Columns.Add("channelB");

            for (int seg = 0; seg < nRapidCaptures; seg++)
            {

                for (int i = 0; i < points;)
                {

                    if (points < 10000)
                    {
                        Ratio = 1;
                        DataRow drS = dt.NewRow();          //dt表增加一个空白行
                        if (_channelSettings[0].enabled)
                        {
                            drS[0] = xbuf;
                        }
                        if (_channelSettings[1].enabled)
                        {
                            drS[1] = y1buf;
                        }
                        i++;
                        dt.Rows.Add(drS);                   //将一行数据填入表中对应行
                    }
                    /*
                     * 超过10000个点，则压缩成2000个点，如10000个点，则每隔10个点，选一个最大值
                     * 点和一个最小值点。
                     */
                else
                {
                    Ratio = (int)points / 2000;     //计算点筛选比例      
                    double[] dataA = new double[Ratio * 2];
                    double[] dataB = new double[Ratio * 2];
                    for (int k = 0; k < Ratio * 2; k++)
                    {
                        dataA[k] = x;
                        dataB[k] = y;

                    }
                    Array.Sort(dataA);                  //将数组dataA由大到小排序
                    Array.Sort(dataB);                  //将数组dataB由大到小排序
                    DataRow drL1 = dt.NewRow();
                    drL1[0] = dataA[Ratio * 2 - 1];
                    drL1[1] = dataB[Ratio * 2 - 1];
                    dt.Rows.Add(drL1);
                    DataRow drL2 = dt.NewRow();
                    drL2[0] = dataA[0];
                    drL2[1] = dataB[0];
                    dt.Rows.Add(drL2);
                    i = i + Ratio * 2;
                }

            }
        }
            DrawLineEventArgs e = new DrawLineEventArgs(dt, num);
            drawLine(this, e);                              //调用事件绘图
        }
        public string VisaDataGet(long sum, out int maxLength)
        {
            string strPath="";
            maxLength = 0;
            try
            {
                strPath=VISADataSave(sum, out maxLength);
            }
            catch(System.ApplicationException err)
            {
                Console.WriteLine("*** VISA COM Error Application Exception: " + err.Message);
                //VISADataSaveWrong(sum);
            }
            catch (System.SystemException err)
            {
                Console.WriteLine("*** System Exception Error Message : " + err.Message);//System.IO.IOException	发生I/O错误时引发的异常。
                //VISADataSaveWrong(sum);
            }
            catch (System.Exception err)
            {
                Console.WriteLine("*** Exception Error : " + err.Message);
                //VISADataSaveWrong(sum);
            }

            return strPath;
        }
        
        public void FormatFun(double fFormat)
        {
            if (fFormat == 0.0)
            {
                Console.WriteLine("Waveform format: BYTE");
            }
            else if (fFormat == 1.0)
            {
                Console.WriteLine("Waveform format: WORD");
            }
            else if (fFormat == 2.0)
            {
                Console.WriteLine("Waveform format: ASCii");
            }
        }
        public void TypeFun(double fType)
        {
            if (fType == 0.0)
            {
                Console.WriteLine("Acquire type: NORMal");
            }
            else if (fType == 1.0)
            {
                Console.WriteLine("Acquire type: PEAK");
            }
            else if (fType == 2.0)
            {
                Console.WriteLine("Acquire type: AVERage");
            }
            else if (fType == 3.0)
            {
                Console.WriteLine("Acquire type: HRESolution");
            }
        }
        
    }// Controller类结束


    //结构ControllerArgs用于信息的载体
    public struct ControllerArgs
    {
        public bool single;                 //是否只采集单条波
        public long traces;                 //总的文明加密次数
        public bool endless;                //无休止采波
        public long current_trace;          //当前采波编号
        public long Time;                   //加密采波总耗时
        public byte[] key;                  //秘钥
        public byte[] plaintext;            //明文
        public byte[] plaintext_mask;       //明文掩码                                            
        public byte[] elapse_total;
        public bool randomGeneration;       //明文随机生成
        //2018.10.22
        public bool alternateGeneration;       //明文交替
        public bool FRRFGeneration;       //FRRF明文交替
        public bool FFFFGeneration;       //F'FF'F明文交替
        public bool FFGeneration;       //FF明文交替
        public bool RFGeneration;       //FR各50%的机会明文交替
        public bool classification4;       //四分类
        public bool classification8;       //八分类
        public bool classificationN;       //n分类
        public bool PseudoRandom;//17825规定下的伪随机数产生
        public int textNclass;       //n分类

        public byte[] fixedplaintext;            //固定明文1
        public byte[] fixedplaintext2;            //固定明文2

        public int tmpFRRF;//2019.4.8 记录FRRF
        public bool randomGeneration_mask;  //明文掩码随机生成                                         
        public int wait;                    //硬件加密等待时间       
        public byte[] ciphertext;           //SAKURA硬件加密的密文
        //2018.10.24
        public byte[] alternate_ciphertext;           //记录交替明文时需使用的密文
        public byte[] random_plaintext;           //记录随机明文
        public byte[] random_key;                 // 2023//11/8记录随机密钥

        public byte[] answer;               //软件加密的密文
        public byte[] difference;           //每一字节异或的结果
        public bool diff;                   //总的异或的结果
        public bool continueIfError;        //出现错误是否继续
        public bool error;                  //是否出错
        public double elapsed;              //硬件加密耗时
        public RandGen rand;
        //2018.10.20
        public Random timerand;
        public bool last;

        public ControllerArgs Clone()
        {
            return (ControllerArgs)MemberwiseClone();
                                            //浅复制
        }

        
    }
}
