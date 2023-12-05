
/*******************************************************************************
* 软件			：SAKURA-G开发板采波软件
* 程序名		: 对话框主程序
* 实验内容		: 通过USB接口向SAKURA-G开发板的FPGA 2#发送明文和密钥数据，进而控制FPGA 1#中的AES进行加密；
*                 接收SAKURA-G返回的密文数据；同时将示波器采集到的能量波形保存。
*
* 发布者		：北京数缘科技有限公司
* 网址			：http://www.mathmagic.cn
* 邮箱			：sales@mathmagic.cn
* 发布日期		：2018.5.30
* 版权声明		：本程序版权归本公司所有，严禁利用其从事任何商业行为，否则将追究法律责任
*******************************************************************************/


using PS3000ACSConsole;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using SAKURA;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;
using System.Threading;

using Ivi.Visa.Interop;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MathMagic
{
    public partial class MainForm : Form
    {

        public static bool IsOtherScope = false;            // 是否使用非Pico示波器
        private appState state = appState.Initialize;       // 标识当前运行的状态：开始，停止，采波中
        private RandGen rand;
        //2018.10.20
        public Random timerand;

        private Controller ctrl; 
        private ControllerArgs args;
        private Stopwatch sw = new Stopwatch();             // 计时？？？
        private int waveformNumber = 1;                     // 记录用户输入的“查看波形”编号
        private bool[] IsError = new bool[3] { false, false, false };
                                                            // 有明文，明文掩码，密钥三个输入框
                                                            // IsError用于标识用户这三个位置的输入是否有误
        private uint T3;                                    //T4-T3记录点开始采波后总耗时
        private uint T4;
        public static MainForm form1;//20201018为了画图
        /// <summary>   
        /// 构造函数
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            
            form1 = this;//20201018为了画图
            comboBox_target.SelectedIndex = 0;
            UpdateFormEnabling();                           // 设置UI界面里button，textbox的enable状态
            ctrl = new Controller();                        // 实例化Controller对象，主要逻辑执行
            rand = new RandGen(1, 0);                       // 用于生成随机数。
            timerand = new Random();// 用于生成随机数。2018.10.20
            //2018.10.21
            Controller.drawLine += DrawByEvent;             // 添加事件监听，每次采波完成后，会调用drawLine事件，执行
                                                            // DrawByEvent函数绘制波形
            Controller.sendErrorMessage = statusMessage;    // 有错误信息时，提示错误信息


        }

        /**************************************************************************
         * 此部分为绘制波形，通过监听事件绘制波形，或从traces&data文件夹读取数据绘制波形
        ***************************************************************************/
  
        // 通过事件通知画点
        public void DrawByEvent(Object sender, DrawLineEventArgs e)
        {
            /* 
             * 加密完成后，会通过事件，加密线程运行到这里，到加密线程和UI线程不是一个线程。
             * 画波形只能在创建控件的线程，即UI线程。所有会先判断当前运行的线程是否和chart1控件
             * 在同一个线程，若不是，则转到创建chart1的线程
             */
            if (chart1.InvokeRequired)                      //判断当前线程是否和控件chart1在同一线程，
            {                                               //不是同一线程，chart1.InvokeRequired的值为true
                while (!this.textBox_ShowNum.IsHandleCreated)
                {
                    // 解决窗体关闭时出现“访问已释放句柄“的异常
                    if (this.textBox_ShowNum.Disposing || this.textBox_ShowNum.IsDisposed)
                        return;
                }
                DrawLineEventHander h = new DrawLineEventHander(DrawByEvent);
                BeginInvoke(h, new object[] { sender, e }); //不是同一线程，通过委托切换到UI线程。
            }
            else
            {
                List<double> listStrArr = e.listStrArr;
                //List<int> xData = e.xData;
                DataTable dt=null;

                waveformNumber = (int)e.current_trace;
                BindChart(dt,0, waveformNumber, listStrArr, e.num);                      // 第二个参数'0'，表示是采波过程中显示的波形
            }
        }

        /// <summary>
        /// 创建chart控件的数据源DataTable(dt)
        /// </summary>
        /// <param name="n">要显示的波形编号</param>
        private bool DrawByFile(int n)
        {
            string folderPath = Controller.folderPath;
            string filePath = folderPath + "\\#" + n.ToString() + "#.csv";
            List<double> listStrArr;
            //Console.WriteLine("!!folderPath={0}", folderPath);
            DataTable dt = OpenFile(n);                 //声明一个DataTable表，存波形数据
                if (dt != null)
                {
                    dt.Columns.Add("time");                 //增加time列，作为横坐标数据
                    int dtLength = dt.Rows.Count;
                    ctrl.ReadCsv(filePath, dtLength, out listStrArr);
                    Console.WriteLine("!!dtLength={0}", dtLength.ToString());
                    //设置x时间轴单位
                    for (int i = 0; i < dtLength; i++)
                    {
                        dt.Rows[i]["time"] = i + 1;         //横坐标为数据的个数
                    }
                    BindChart(dt, n, 0, listStrArr, dtLength);
                }
                else
                {
                    toolStripProgressBar_progress.Value = 0;// 清空进度条
                    return false;                           // 找不到波形文件，返回false;
                }
                return true;
            
        }

        /// <summary>
        /// 从traces&data文件夹读取波形数据
        /// </summary>
        /// <param name="n">读取的波形编号</param>
        private DataTable OpenFile(int n)
        {
            statusMessage("",false);                        //清空左下角的提示信息
            DataTable dt = new DataTable();                 //用列表存波形数据，一共有两列，channelA和channelB
            dt.Columns.Add("channelA");
            dt.Columns.Add("channelB");
            string[] strArray = new string[2];
            string strLine = "";
            string folderPath = Controller.folderPath;
            //Console.WriteLine("folderPath={0}", folderPath);
            //string filePath = folderPath+"\\Trace"+ n.ToString().PadLeft(6, '0') + ".csv";
            string filePath = folderPath + "\\#" + n.ToString()+ "#.csv";
            //Console.WriteLine("filePath={0}", filePath);
            System.Text.Encoding encoding = Encoding.ASCII;
            try
            {
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs, encoding);

                while ((strLine = sr.ReadLine()) != null)
                {
                    {
                        DataRow dr = dt.NewRow();           //列表添加新的一行，没有数据
                        strArray = strLine.Split(',');
                        for (int j = 0; j < 1; j++)         //读入A通道数据
                        //for (int j = 0; j < 2; j++)         //读入A通道和B通道两组数据
                        {
                            dr[j] = strArray[j];
                            //Console.WriteLine("strArray={0}", strArray[j]);
                        }
                        dt.Rows.Add(dr);                    //将数据填入列表对应行

                    }
                }
                sr.Close();                                 //关闭文件流
                fs.Close();
            }
            catch(Exception ex)
            {
                DataTable nullTable = null; 
                statusMessage(ex.Message, true);            //文件不存在或被占用，左下角给出提示信息
                return nullTable;
            }
            return dt;
        }
        
        /// <summary>
        /// 用chart控件绘制波形
        /// </summary>
        /// <param name="dt">chart的数据源DataSource</param>
        /// <param name="n1">不为0时，表示要查找的波形编号</param>
        /// <param name="n2">加密时的波形编号</param>
        private void BindChart(DataTable dt,int n1,int n2, List<double> listStrArr,int Length)
        {
            chart1.Visible = true;                          //一开始chart1是不可见的，先设为可见
            chart1.Series.Clear();                          //清空已存在的Series，这样做可提高波形显示速度
            List<int> xData = new List<int>();

            
            for (int i = 0; i < Length; i++)
            {
                xData.Add(i + 2);//为了调整下标从1-1201
                //MessageBox.Show(line);
            }

            Series A = new Series();
            A.ChartType = SeriesChartType.Line;             //点之间的连线选择折现
            A.YAxisType = AxisType.Primary;
            //A.YAxisType = AxisType.Secondary;               //触发通道电压为副轴
            A.Color = Color.CornflowerBlue;                 //设置通道A线的颜色为浅蓝色
            chart1.Series.Add(A);

            chart1.Series[0].Name = "AES";

            chart1.Series[0].Points.DataBindXY(xData, listStrArr.ToArray());


            /*Series B = new Series();
            B.ChartType = SeriesChartType.Line;
            B.YAxisType = AxisType.Primary;                  
            B.Color = Color.OrangeRed;                      //设置通道B线的颜色为橘红色
            chart1.Series.Add(B);
            chart1.Series[1].YValueMembers = "channelB";    //设置Series[1]的数据源为channelA列
            chart1.Series[1].XValueMember = "time";*/

            //chart1.DataSource = dt;
            //设置图表Y轴对应项
            //chart1.Series[0].YValueMembers = "channelA";    //设置Series[0]的数据源为channelA列
            //chart1.Series[0].XValueMember = "time";
            //chart1.DataBind();                              //开始绘图

            /*
             * 加密过程现实波形和通过查找显示波形都是通过这个函数，所以用n1进行区分。
             * n1为0表示加密过程显示波形，否则是通过查找显示波形。
             */
            if (n1 != 0)                                    
            {
                statusMessage("波形编号" + n1 + ": 查找完成", false);
                toolStripProgressBar_progress.Value = 100;
            }
            else
                textBox_ShowNum.Text = n2.ToString();
        }

        /**************************************************************************
         * 此部分通过run方法启动采波，然后采波中worker_ProgressChanged更新UI控件
         * 采波完成worker_RunWorkerCompleted更新UI控件
        ***************************************************************************/
        /// <summary>
        /// 更新UI控件的显示
        /// 每完成一次明文加密、储存波形数据后调用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //ControllerArgs用于两个线程之间传递数据
            ControllerArgs args = (ControllerArgs)e.UserState;  
            sw.Stop();                                      //停止计时
            if (args.current_trace == 1 || args.last || sw.ElapsedMilliseconds >= 30)
            {
                //Worker_DoWork传递过来的res数据，更新文本显示
                textBox_rtraces.Text = args.current_trace.ToString();
                T4 = Controller.GetTickCount();
                textBox_AllTime.Text = ((double)(T4 - T3) / 1000).ToString();
                textBox_s.Text = args.Time.ToString("f3");
                
                if(args.alternateGeneration||args.FRRFGeneration||args.RFGeneration||args.randomGeneration)
                    textBox_plaintext.Text = Utils.byteArrayToString(args.random_plaintext);
                //2023//11/8 为新增全随机明文密钥功能，加了下面的代码
                    textBox_key.Text = Utils.byteArrayToString(args.random_key);
                //2023//11/8 为新增全随机明文密钥功能，加的代码
                if (args.FFFFGeneration|| args.FFGeneration)
                    textBox_plaintext.Text = Utils.byteArrayToString(args.fixedplaintext2);
                //2018.10.24 交替明文使用上一次的密文
                /*if (args.alternateGeneration == true)
                {
                    textBox_fixedplaintext.Text = Utils.byteArrayToString(args.alternate_ciphertext);
                }*/

                textBox_rplaintext.Text = Utils.byteArrayToString(args.plaintext);
                
                textBox_ranswer.Text = Utils.byteArrayToString(args.answer);
                textBox_rciphertext.Text = Utils.byteArrayToString(args.ciphertext);
                textBox_rdifference.Text = Utils.byteArrayToString(args.difference);
                statusMessage("Running...", false);
                if (toolStripProgressBar_progress.ProgressBar != null)
                {
                    toolStripProgressBar_progress.Value = e.ProgressPercentage;
                }
                sw.Reset();

                textBox_rtraces.Update();
                textBox_AllTime.Update();
                textBox_s.Update();
                textBox_rplaintext.Update();
                textBox_ranswer.Update();
                textBox_rciphertext.Update();
                textBox_rdifference.Update();
            }
            sw.Start();

        }

        /// <summary>
        /// 指定明文加密完成或点击停止采波按钮后调用，更新UI控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                statusMessage("Cancelled.",false);
                toolStripProgressBar_progress.Value = 0;
            }
            else
            {
                ControllerArgs res = (ControllerArgs)e.Result;
                if (res.error)
                {
                    statusMessage("Error.", true);//2018.11.5
                    toolStripProgressBar_progress.Value = 0;
                }
                else
                {
                    statusMessage("Completed.",false);
                }
            }

            if (!args.single)
            {
                rand.restartPlaintextPrng();
            }
            ctrl.Close();
            sw.Stop();
            T4 = Controller.GetTickCount();
            textBox_AllTime.Text = ((double)(T4 - T3) / 1000).ToString();
            state = appState.Idle;
            UpdateFormEnabling();
            toolStripProgressBar_progress.Style = ProgressBarStyle.Blocks;
        }

        /// <summary>
        /// 根据state(采波进行中、完成、开始），设置button、textbox的enable状态
        /// </summary>
        private void UpdateFormEnabling()
        {
            switch (state)
            {
                //Initialize为界面初始化阶段
                case appState.Initialize:
                    comboBox_target.Enabled = true;
                    textBox_traces.Enabled = true;
                    checkBox_endless.Enabled = true;
                    textBox_key.Enabled = true;
                    button_changekey.Enabled = true;
                    textBox_plaintext.Enabled = true;
                    textBox_plaintext_mask.Enabled = true;
                    button_changeplaintext.Enabled = true;
                    button_changeplaintext_mask.Enabled = true;
                    checkBox_randomgeneration.Enabled = true;
                    //2018.10.22
                    checkBox_alternate.Enabled = true;
                    textBox_fixedplaintext.Enabled = false;
                    checkBox_RF.Enabled = true;
                    checkBox_FRRF.Enabled = true;
                    checkBox_FFFF.Enabled = true;
                    checkBox_FF.Enabled = true;
                    checkBox_classification4.Enabled = true;
                    checkBox_classification8.Enabled = true;
                    checkBox_classificationN.Enabled = true;
                    checkBox_PseudoRandom.Enabled = true;

                    checkBox_randomgeneration_mask.Enabled = true;
                    textBox_wait.Enabled = true;
                    button_single.Enabled = false;
                    button_start.Enabled = false;
                    button_stop.Enabled = false;
                    checkBox_continueiferror.Enabled = true;
                    checkBox_IsOtherScope.Enabled = true;
                    textBox_buffer.Enabled = true;
                    textBox_trigger.Enabled = true;
                    button_OpenPico.Enabled = true;
                    textBox_ShowNum.Enabled = true;
                    button1.Enabled = true;
                    buttonLast.Enabled = true;
                    buttonNext.Enabled = true;
                    break;

                //Idle为准备阶段
                case appState.Idle:
                    //textBox_interface.Enabled = true;
                    comboBox_target.Enabled = true;
                    textBox_traces.Enabled = true;
                    checkBox_endless.Enabled = true;
                    textBox_key.Enabled = true;
                    button_changekey.Enabled = true;      
                    textBox_plaintext.Enabled = true;
                    textBox_plaintext_mask.Enabled = true;
                    button_changeplaintext.Enabled = true;
                    button_changeplaintext_mask.Enabled = true;
                    checkBox_randomgeneration.Enabled = true;
                    checkBox_RF.Enabled = true;
                    checkBox_FRRF.Enabled = true;
                    checkBox_FFFF.Enabled = true;
                    checkBox_FF.Enabled = true;
                    checkBox_classification4.Enabled = true;
                    checkBox_classification8.Enabled = true;
                    checkBox_classificationN.Enabled = true;
                    checkBox_PseudoRandom.Enabled = true;

                    //2018.10.22
                    checkBox_alternate.Enabled = true;
                    if (checkBox_alternate.Checked)
                    {
                        textBox_fixedplaintext.Enabled = true;
                    }
                    else
                    {
                        textBox_fixedplaintext.Enabled = false;
                    }

                    checkBox_randomgeneration_mask.Enabled = true;
                    textBox_wait.Enabled = true;
                    button_single.Enabled = true;
                    button_start.Enabled = true;
                    button_stop.Enabled = false;
                    checkBox_continueiferror.Enabled = true;
                    checkBox_IsOtherScope.Enabled = true;
                    textBox_buffer.Enabled = true;
                    textBox_trigger.Enabled = true;
                    button_OpenPico.Enabled = true;
                    textBox_ShowNum.Enabled = true;
                    button1.Enabled = true;
                    buttonLast.Enabled = true;
                    buttonNext.Enabled = true;
                    break;

                //采波进行中
                case appState.Running:
                    //textBox_interface.Enabled = false;
                    comboBox_target.Enabled = false;
                    textBox_traces.Enabled = false;
                    checkBox_endless.Enabled = false;
                    textBox_key.Enabled = false;
                    button_changekey.Enabled = false;
                    textBox_plaintext.Enabled = false;
                    textBox_plaintext_mask.Enabled = false;                          
                    button_changeplaintext.Enabled = false;
                    button_changeplaintext_mask.Enabled = false;                    
                    checkBox_randomgeneration.Enabled = false;
                    checkBox_RF.Enabled = false;
                    checkBox_FRRF.Enabled = false;
                    checkBox_FFFF.Enabled = false;
                    checkBox_FF.Enabled = false;
                    checkBox_classification4.Enabled = false;
                    checkBox_classification8.Enabled = false;
                    checkBox_classificationN.Enabled = false;
                    checkBox_PseudoRandom.Enabled = false;
                    //2018.10.22
                    checkBox_alternate.Enabled = false;
                    textBox_fixedplaintext.Enabled = false;

                    checkBox_randomgeneration_mask.Enabled = false;                 
                    textBox_wait.Enabled = false;
                    button_single.Enabled = false;
                    button_start.Enabled = false;
                    button_stop.Enabled = true;
                    checkBox_continueiferror.Enabled = false;
                    checkBox_IsOtherScope.Enabled = false;
                    textBox_buffer.Enabled = false;
                    textBox_trigger.Enabled = false;
                    button_OpenPico.Enabled = false;
                    textBox_ShowNum.Enabled = false;
                    button1.Enabled = false;
                    buttonLast.Enabled = false;
                    buttonNext.Enabled = false;
                    break;

                //采波停止
                case appState.Stop:
                    //textBox_interface.Enabled = false;
                    comboBox_target.Enabled = false;
                    textBox_traces.Enabled = false;
                    checkBox_endless.Enabled = false;
                    textBox_key.Enabled = false;
                    button_changekey.Enabled = false;
                    textBox_plaintext.Enabled = false;
                    textBox_plaintext_mask.Enabled = false;
                    button_changeplaintext.Enabled = false;
                    button_changeplaintext_mask.Enabled = false;
                    checkBox_randomgeneration.Enabled = false;
                    checkBox_RF.Enabled = false;
                    checkBox_FRRF.Enabled = false;
                    checkBox_FFFF.Enabled = false;
                    checkBox_FF.Enabled = false;
                    checkBox_classification4.Enabled = false;
                    checkBox_classification8.Enabled = false;
                    checkBox_classificationN.Enabled = false;
                    checkBox_PseudoRandom.Enabled = false;
                    //2018.10.22
                    checkBox_alternate.Enabled = false;
                    textBox_fixedplaintext.Enabled = false;

                    checkBox_randomgeneration_mask.Enabled = false;
                    textBox_wait.Enabled = false;
                    button_single.Enabled = false;
                    button_start.Enabled = false;
                    button_stop.Enabled = false;
                    checkBox_continueiferror.Enabled = false;
                    checkBox_IsOtherScope.Enabled = true;
                    textBox_buffer.Enabled = true;
                    textBox_trigger.Enabled = true;
                    button_OpenPico.Enabled = true;
                    textBox_ShowNum.Enabled = true;
                    button1.Enabled = true;
                    buttonLast.Enabled = true;
                    buttonNext.Enabled = true;
                    break;
            }
            if (checkBox_classification4.Checked|| checkBox_classification8.Checked|| checkBox_classificationN.Checked)
            {
                textBox_plaintext.Enabled = false;
                textBox_fixedplaintext.Enabled = false;
                textBox_key.Enabled = false;
            }
        }


        /// <summary>
        /// 检查端口，启动主逻辑部分
        /// </summary>
        /// <param name="single"></param>
        private void Run(bool single)
        {
            args = new ControllerArgs();                            //实例化结构，用于明文加密线程和UI线程之间传值。
            string filePath = Controller.folderPath+"\\Plaintext.csv";
            if (File.Exists(filePath))                              //若Plaintext.csv存在，就先删了
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (Exception ex)
                {
                   
                    statusMessage(ex.Message, true);             //文件被占用，不能删除
                    return;
                }
            }
            try
            {
                uint port = 0;
                if (String.Compare("USB", textBox_interface.Text.Substring(0, 3), true) == 0)
                {
                    if (!uint.TryParse(textBox_interface.Text.Substring(3), out port))
                    {
                        throw new Exception("Error: Invalid Interface.");
                    }
                }
                else
                {
                    throw new Exception("Error: Invalid Interface.");
                }

                ctrl.Open(port);                                        //通过端口新建CipherModule(index),打不开会返回错误信息
                //注册更新事件和完成事件
                ctrl.AddProgressChangedEventHandler(new ProgressChangedEventHandler(Worker_ProgressChanged));
                ctrl.AddCompletedEventHandler(new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted));
                args.single = single;                                   //传过来的参数Single是false;
                args.traces = Convert.ToInt64(textBox_traces.Text);     //textBox_traces波形总数
                args.endless = checkBox_endless.Checked;
                if (!args.single)
                {
                    rand.restartPlaintextPrng();                        //重置随机种子
                }

                sw.Start();
                statusMessage("Running...",false);                      //左下角显示"Running"

                toolStripProgressBar_progress.Value = 0;
                if (args.endless)   //判断是否选择无限采波
                {
                    toolStripProgressBar_progress.Style = ProgressBarStyle.Marquee;
                }                                                       //设置进度条为连续滚动
                else
                {
                    toolStripProgressBar_progress.Style = ProgressBarStyle.Blocks;
                }                                                       //设置进度条为Blocks形式

                //读取明文、秘钥、波形条数等
                args.key = Utils.stringToByteArray(textBox_key.Text);   //有非法字符会报错
                args.plaintext = Utils.stringToByteArray(textBox_plaintext.Text);
                args.fixedplaintext2 = Utils.stringToByteArray(textBox_plaintext.Text);
                args.plaintext_mask = Utils.stringToByteArray(textBox_plaintext_mask.Text);
                args.randomGeneration = checkBox_randomgeneration.Checked;

                //2018.10.22读取交替明文
                args.alternateGeneration = checkBox_alternate.Checked;
                args.fixedplaintext = Utils.stringToByteArray(textBox_fixedplaintext.Text);
                //2019.4.8 FRRF交替明文
                args.FRRFGeneration=checkBox_FRRF.Checked;
                args.RFGeneration = checkBox_RF.Checked;
                args.FFFFGeneration = checkBox_FFFF.Checked;
                args.FFGeneration = checkBox_FF.Checked;
                args.classification4 = checkBox_classification4.Checked;
                args.classification8 = checkBox_classification8.Checked;
                args.classificationN = checkBox_classificationN.Checked;

                args.PseudoRandom = checkBox_PseudoRandom.Checked;
                args.textNclass = Convert.ToInt32(textBox_classificationN.Text);         //n分类


                args.randomGeneration_mask = checkBox_randomgeneration_mask.Checked;
                args.wait = Convert.ToInt32(textBox_wait.Text);         //时间间隔
                args.continueIfError = checkBox_continueiferror.Checked;//错误时是否继续
                args.rand = rand;
                //2018.10.20
                args.timerand = timerand;
                ctrl.Run(args);                                         //！！！启用明文加密和采波线程，并立即开始执行Worker_DoWork函数
                                                                        //！！！注：Worker_DoWork函数是主要的加密采波函数，请用户关注

            }
            catch (Exception ex)
            {
                statusMessage(ex.Message,true);
                toolStripProgressBar_progress.Value = 0;
                toolStripProgressBar_progress.Style = ProgressBarStyle.Blocks;
                state = appState.Idle;
                UpdateFormEnabling();                                   //更新UI状态
            }
        }

        //左下角显示信息
        private void statusMessage(string message,bool isErrorMessage)
        {
            if (statusStrip_status.InvokeRequired)                      // 判断是否和chart1控件在同一个线程，若不是，则转到创建chart1的线程
            {

                Action<string, bool> h = statusMessage;
                BeginInvoke(h, new object[] { message, isErrorMessage });
            }
            else
            {
                if (isErrorMessage)                                     //如果是错误信息，字体颜色为红色，否则为黑色
                    toolStripStatusLabel_message.ForeColor = Color.Red;
                else toolStripStatusLabel_message.ForeColor = Color.Black;
                toolStripStatusLabel_message.Text = message;
            }
        }
        //标识程序所属的状态
        private enum appState : int
        {
            Initialize=0,
            Idle,
            Start,
            Running,
            Stop  
        }

        /**************************************************************************
         * 这部分是UI控件响应执行部分，如button的点击、textbox的输入、checkbox的勾选。
        ***************************************************************************/
        //点击更换按钮，更换秘钥
        private void button_changekey_Click(object sender, EventArgs e)
        {
            textBox_key.Text = Utils.byteArrayToString(rand.generateKey(false));
            IsError[0] = false;                                         //自动输入秘钥，输入框输入格式无误
            statusMessage("", false);                                   //将左下角错误提示清空
        }

        //点击更换按钮，更换明文
        private void button_changeplaintext_Click(object sender, EventArgs e)
        {
            textBox_plaintext.Text = Utils.byteArrayToString(rand.generatePlaintext());

            IsError[1] = false;
            statusMessage("", false);                                   //将左下角错误提示清空
        }

        //点击更换按钮，更换明文掩码
        private void button_changeplaintext_mask_Click(object sender, EventArgs e)  
        {
            textBox_plaintext_mask.Text = Utils.byteArrayToString(rand.generatePlaintext());
            IsError[2] = false;
            statusMessage("", false);                                   //将左下角错误提示清空
        }

        //点击单条波形按钮，只发送一次明文，只采一条波
        private void button_single_Click(object sender, EventArgs e)
        {                                                                        
            foreach (bool b in IsError)                                 //若明文、明文掩码、密文输入格式有误，则IsError[i]为false.
            {                                                           //这里先遍历数组，判断明文等输入格式是否正确。
                if (b)
                    return;
            }
            state = appState.Running;                                   //state表示应用进行的状态，Running是运行中
            UpdateFormEnabling();                                       //根据state值，设置button,text等控件enable属性
            Run(true);                                                  //采波开始执行
        }

        //点击开始采波按钮
        private void button_start_Click(object sender, EventArgs e)
        {
            //2018.10.23
            System.DateTime currentTime = new System.DateTime();
            currentTime = System.DateTime.Now;
            ctrl.folderPathLast = "DSO-X 3034T WaveData " +
                    currentTime.Year.ToString() + "_" +
                    currentTime.Month.ToString() + "_" +
                    currentTime.Day.ToString() + "_" +
                    currentTime.Hour.ToString() + "h" +
                    currentTime.Minute.ToString()+ "m" +
                    currentTime.Second.ToString() + "s";
            //Console.Out.WriteLine("folderPathLast :{0} " ,folderPathLast);
            Controller.folderPath = Controller.appPath.FullName + ctrl.folderPathLast;
            //Console.Out.WriteLine("Controller.folderPath :{0} ", Controller.folderPath);

            //2018.10.22设置采样数据点
            //ctrl.myScope.DoCommand(":WAVEFORM:POINTS 20000");

            foreach (bool b in IsError)                                 //若明文、明文掩码、密文输入格式有误，则IsError[i]为false.
            {                                                           //这里先遍历数组，判断明文等输入格式是否正确。
                if (b)
                    return;
            }
            T3 = Controller.GetTickCount();
            state = appState.Running;                                   //state表示应用进行的状态，Running是运行中
            UpdateFormEnabling();                                       //根据state值，设置button,text等控件enable属性
            Run(false);                                                 //！！！调用Run函数，准备开始加密和采波   
               
        }

        //点击停止采波按钮
        private void button_stop_Click(object sender, EventArgs e)
        {
            state = appState.Stop;                                      //state表示应用进行的状态,Stop是停止加密
            UpdateFormEnabling();                                       //根据state值，设置button,text等控件enable属性
            ctrl.Cancel();
        }

        //输入秘钥
        private void textBox_key_Leave(object sender, EventArgs e)
        {
            try
            {
                textBox_key.Text = Utils.formHexlString(textBox_key.Text);
                CheckTextContent(textBox_key.Text, 0);                  //检查用于输入格式，"0"表示检查的是秘钥
            }
            catch (Exception ex)
            {
                IsError[0] = true;
                statusMessage(ex.Message, true);
            }
        }

        //限制秘钥内容输入
        private void textBox_key_KeyPress(object sender, KeyPressEventArgs e)
        {
            LimitKeyPress2(e);
        }

        //输入明文
        private void textBox_plaintext_Leave(object sender, EventArgs e)
        {
            try
            {
                textBox_plaintext.Text = Utils.formHexlString(textBox_plaintext.Text);
                CheckTextContent(textBox_plaintext.Text, 1);            //检查用于输入格式，"1"表示检查的是明文
            }
            catch(Exception ex)
            {
                IsError[1] = true;
                statusMessage(ex.Message, true);
            }
        }

        //限制明文内容输入
        private void textBox_plaintext_KeyPress(object sender, KeyPressEventArgs e)
        {
            LimitKeyPress2(e);
        }

        //输入明文掩码
        private void textBox_plaintext_mask_Leave(object sender, EventArgs e)
        {
            try
            {
                textBox_plaintext_mask.Text = Utils.formHexlString(textBox_plaintext_mask.Text);
                CheckTextContent(textBox_plaintext_mask.Text, 2);       //检查用于输入格式，"2"表示检查的是明文掩码
            }
            catch (Exception ex)
            {
                IsError[2] = true;
                statusMessage(ex.Message, true);
            }
        }

        //限制明文掩码内容输入
        private void textBox_plaintext_mask_KeyPress(object sender, KeyPressEventArgs e)
        {
            LimitKeyPress2(e);
        }

       
        /// <summary>
        /// 检查是否使用其他示波器，若使用其他示波器，则只进行明文加密，不采波
        /// 其他示波器指除了PicoScope 3000 Series外的示波器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox_IsOther_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_IsOtherScope.Checked == true)
            {
                IsOtherScope = true;
                //2018.10.22
                //state = appState.Idle;  //若选择其他示波器，则开始采波按钮可以点击
                state = appState.Initialize;
                UpdateFormEnabling();
            }
            else if (!checkBox_PicoState.Checked) {
                IsOtherScope = false;
                state = appState.Initialize;                            //若没有选择其他示波器，且pico示波器没有打开，
                UpdateFormEnabling();                                   //则开始采波按钮不能点击
            }
            else { IsOtherScope = false; }
        }

        //示波器状态发生改变时
        private void checkBox_PicoState_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_PicoState.Checked||IsOtherScope)               //若选择其他示波器或打开pico示波器，则开始采波按钮可以点击
            {
                state = appState.Idle;
                UpdateFormEnabling();
            }
            else
            {
                state = appState.Initialize;
                UpdateFormEnabling();
            }
            
        }
        // buffer大小： 限制用户的输入，只允许输入数字和删除
        private void textBox_buffer_KeyPress(object sender, KeyPressEventArgs e)
        {
            LimitKeyPress1(e); 
        }

        // 每次输入文本且文本失去焦点后都会调用这个函数，设置buffer大小
        private void textBox_buffer_Leave(object sender, EventArgs e)
        {
            if (textBox_buffer.Text != "")                                      //先判断文本内容是否为空
            {
                Controller.BUFFER_SIZE = (uint)int.Parse(textBox_buffer.Text);  //通过textbox输入设置BUFFER_SIZE
                //每次改变BUFFER_SIZE值时，重新计算TriggerAfterNum值
                Controller.TriggerAfterNum = (int)Controller.BUFFER_SIZE - Controller.TriggerBeforeNum;
                if (Controller.TriggerAfterNum < 0)
                    Controller.TriggerAfterNum = 0;
            }
        }

        //限制用户的输入，只允许输入数字和删除健
        private void textBox_trigger_KeyPress(object sender, KeyPressEventArgs e)
        {   
            LimitKeyPress1(e);
        }

        // 通过textbox输入，设置触发前一条波形采样点数
        private void textBox_trigger_Leave(object sender, EventArgs e)
        {
            if (textBox_trigger.Text != "")                                     //先判断文本内容是否为空
            {
                Controller.TriggerBeforeNum = int.Parse(textBox_trigger.Text);  //通过textbox输入设置trigger大小
                //每次改变TriggerBeforeNum值时，重新计算TriggerAfterNum值
                Controller.TriggerAfterNum = (int)Controller.BUFFER_SIZE - Controller.TriggerBeforeNum;
                if (Controller.TriggerAfterNum < 0)
                    Controller.TriggerAfterNum = 0;
            }
                
        }
       
        //输入查看波形编号，查看波形
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar < '0' || e.KeyChar > '9')
            {
                e.Handled = true;                                       //如果输入小于0，大于9，阻止操作；
            }       
            if (e.KeyChar == '\r')
            {
                string str = textBox_ShowNum.Text;

                if (str != "" )
                {
                    waveformNumber = int.Parse(str);
                    DrawByFile(waveformNumber);                         //从traces&data文件夹加载波形数据
                }
            }
            if (e.KeyChar == 8)                                         //不阻止backspace健
            {
                e.Handled = false;
            }
        }

        // 时间间隔： 限制用户的输入，只允许输入数字和删除
        private void textBox_wait_KeyPress(object sender, KeyPressEventArgs e)
        {
            LimitKeyPress1(e);
        }

        // 波形总数： 限制用户的输入，只允许输入数字和删除
        private void textBox_traces_KeyPress(object sender, KeyPressEventArgs e)
        {
            LimitKeyPress1(e);
        }

        //限制textbox输入内容
        private void LimitKeyPress1(KeyPressEventArgs e)
        {
            if (e.KeyChar < '0' || e.KeyChar > '9')
            {
                e.Handled = true;                                       //如果输入小于0，大于9，阻止操作；
            }
            if (e.KeyChar == 8)                                         //不阻止backspace健
            {
                e.Handled = false;
            }
        }
        
        //限制明文、密文、明文掩码字符输入
        private void LimitKeyPress2(KeyPressEventArgs e)
        {
            if (!((e.KeyChar>='0'&&e.KeyChar<='9')||(e.KeyChar>='A'&&e.KeyChar<='F')))
            {
                e.Handled = true;                                       //只允许输入0-9,A-F，其他阻止操作；
            }
            if (e.KeyChar == 8||e.KeyChar==' ')                         //不阻止backspace健
            {
                e.Handled = false;
            }
        }

        //检查明文、密文、明文掩码输入内容
        private void CheckTextContent(string textString,int i)
        {
            byte[] bytes;
            try
            {
                bytes = Utils.stringToByteArray(textString);               
                IsError[i] = false;
                foreach (bool b in IsError)
                {
                    if (b)
                        return;
                }
                statusMessage("", false);
            }
            catch(Exception ex)
            {
                IsError[i] = true;
                statusMessage(ex.Message, true);
            }
        }

        //点击查看波形按钮，根据输入查找波形
        private void button1_Click(object sender, EventArgs e)
        {
            string str = textBox_ShowNum.Text;

            Console.WriteLine("waveformNumber={0}", waveformNumber.ToString());
            if (str != "")
            {
                Console.WriteLine("str={0}", str);
                waveformNumber = int.Parse(str);
                DrawByFile(waveformNumber);                             //从traces&data文件夹加载波形数据
            }
        }

        //点击查找上一条波形按钮
        private void buttonLast_Click(object sender, EventArgs e)
        {
            if (waveformNumber > 1)                                     //查找波形编号不能小于1；
            {
                waveformNumber--;
                textBox_ShowNum.Text = waveformNumber.ToString();       //更新textBox_ShowNum的值
                DrawByFile(waveformNumber);                             //通过文件读取波形
            }
        }

        //点击查找下一条波形按钮
        private void buttonNext_Click(object sender, EventArgs e)
        {
            waveformNumber++;
            if (DrawByFile(waveformNumber))                             //通过文件读取波形,能找到文件返回true；
                textBox_ShowNum.Text = waveformNumber.ToString();       //显示波形编号
            else waveformNumber--;
        }

        //button_OpenPico_Click 点击打开/关闭示波器按钮。
        private void button_OpenPico_Click(object sender, EventArgs e)
        {
            if (checkBox_PicoState.Checked)                             //根据复选框的值判断示波器是否打开
            {
                button_OpenPico.Text = "打开示波器";
                checkBox_PicoState.Checked = false;
                //Imports.CloseUnit(Controller.handle);
                //2018.10.14关闭示波器的连接
                try
                {
                    // Close the connection to the instrument and free the reference to the session.
                    ctrl.myScope.Close();
                }
                catch (Exception errorClosing)
                {
                    Console.Out.WriteLine("Closing error; " + errorClosing.Message);
                }
                statusMessage("示波器已关闭", false);
            }
            else
            {
                //2018.10.13打开示波器的连接
                try
		        {
                    string strResults;
                    ctrl.myScope = new VisaComInstrument("USB0::0x2A8D::0x1764::MY56310472::0::INSTR");
                      ctrl.myScope.SetTimeoutSeconds(10);
                      ctrl.myScope.DoCommand(":RUN");
                      // Get and display the device's *IDN? string.
                      strResults = ctrl.myScope.DoQueryString("*IDN?");
                      Console.WriteLine("*IDN? result is: {0}", strResults);
                    checkBox_PicoState.Checked = true;
                    button_OpenPico.Text = "关闭示波器";
                    //示波器配置函数
                    //Thread.Sleep(2500);                                   //刚打开示波器时，延时2.5秒             
                    state = appState.Idle;
                    UpdateFormEnabling();
                    statusMessage("成功打开示波器", false);
                    

                }
                catch (System.ApplicationException err)
                {
                    //若示波器打不开，则提示出错
                    statusMessage("Error：请检查示波器电源", true);
                    Controller.isOpen = true;
                    Console.WriteLine("*** VISA COM Error : " + err.Message);
                }
                catch (System.SystemException err)
                {
                    //若示波器打不开，则提示出错
                    statusMessage("Error：请检查示波器电源", true);
                    Controller.isOpen = true;
                    Console.WriteLine("*** System Error Message : " + err.Message);
                }
                catch (System.Exception err)
                {
                    //若示波器打不开，则提示出错
                    statusMessage("Error：请检查示波器电源", true);
                    Controller.isOpen = true;
                    System.Diagnostics.Debug.Fail("Unexpected Error");
                    Console.WriteLine("*** Unexpected Error : " + err.Message);
                }


                /*ctrl.deviceOpen(out Controller.handle);
                if (!Controller.isOpen)                                 //若示波器打不开，则提示出错
                {
                    statusMessage("Error：请检查示波器电源", true);
                    Controller.isOpen = true;
                    return;                                 
                }
                checkBox_PicoState.Checked = true;
                button_OpenPico.Text = "关闭示波器";
                //示波器配置函数
                //Thread.Sleep(2500);                                     //刚打开示波器时，延时2.5秒             
                state = appState.Idle;
                UpdateFormEnabling();
                statusMessage("成功打开示波器", false);*/

            }
            
        }
        //关闭程序时，关闭示波器
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // Close the connection to the instrument and free the reference to the session.
                ctrl.myScope.Close();
            }
            catch (Exception errorClosing)
            {
                Console.Out.WriteLine("Closing error; " + errorClosing.Message);
            }
            //Imports.CloseUnit(Controller. handle);//2018.10.12 关闭示波器
        }

        private void checkBox_randomgeneration_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_randomgeneration.Checked)
            {
                checkBox_alternate.Checked = false;
                textBox_fixedplaintext.Enabled = false;
                checkBox_RF.Checked = false;
                checkBox_FRRF.Checked = false;
                checkBox_FFFF.Checked = false;
                checkBox_FF.Checked = false;
                checkBox_classification4.Checked = false;
                checkBox_classification8.Checked = false;
                checkBox_classificationN.Checked = false;
            }
        }

        private void checkBox_alternate_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_alternate.Checked)
            {
                checkBox_randomgeneration.Checked = false;
                checkBox_FRRF.Checked = false;
                checkBox_RF.Checked = false;
                textBox_fixedplaintext.Enabled = true;
                checkBox_FFFF.Checked = false;
                checkBox_FF.Checked = false;
                checkBox_classification4.Checked = false;
                checkBox_classification8.Checked = false;
                checkBox_classificationN.Checked = false;
            }
            else
            {
                textBox_fixedplaintext.Enabled = false;
            }
        }

        private void checkBox_FRRF_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_FRRF.Checked)
            {
                checkBox_randomgeneration.Checked = false;
                checkBox_RF.Checked = false;
                checkBox_alternate.Checked = false;
                textBox_fixedplaintext.Enabled = true;
                checkBox_FF.Checked = false;
                checkBox_FFFF.Checked = false;
                checkBox_classification4.Checked = false;
                checkBox_classification8.Checked = false;
                checkBox_classificationN.Checked = false;
            }
            else
            {
                textBox_fixedplaintext.Enabled = false;
            }

        }

        private void checkBox_RF_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_RF.Checked)
            {
                checkBox_FRRF.Checked = false;
                checkBox_randomgeneration.Checked = false;
                checkBox_alternate.Checked = false;
                textBox_fixedplaintext.Enabled = true;
                checkBox_FF.Checked = false;
                checkBox_FFFF.Checked = false;
                checkBox_classification4.Checked = false;
                checkBox_classification8.Checked = false;
                checkBox_classificationN.Checked = false;
            }
            else
            {
                textBox_fixedplaintext.Enabled = false;
            }
        }
        private void checkBox_FFFF_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_FFFF.Checked)
            {
                checkBox_FRRF.Checked = false;
                checkBox_randomgeneration.Checked = false;
                checkBox_alternate.Checked = false;
                checkBox_FF.Checked = false;
                checkBox_RF.Checked = false;
                checkBox_classification4.Checked = false;
                checkBox_classification8.Checked = false;
                checkBox_classificationN.Checked = false;
                textBox_fixedplaintext.Enabled = true;
            }
            else
            {
                textBox_fixedplaintext.Enabled = false;
            }
        }

        private void checkBox_FF_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_FF.Checked)
            {
                checkBox_FRRF.Checked = false;
                checkBox_randomgeneration.Checked = false;
                checkBox_alternate.Checked = false;
                checkBox_FFFF.Checked = false;
                checkBox_RF.Checked = false;
                checkBox_classification4.Checked = false;
                checkBox_classification8.Checked = false;
                checkBox_classificationN.Checked = false;
                textBox_fixedplaintext.Enabled = true;
            }
            else
            {
                textBox_fixedplaintext.Enabled = false;
            }
        }
        private void checkBox_classification4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_classification4.Checked)
            {
                checkBox_FRRF.Checked = false;
                checkBox_randomgeneration.Checked = false;
                checkBox_alternate.Checked = false;
                checkBox_FFFF.Checked = false;
                checkBox_RF.Checked = false;
                checkBox_classification8.Checked = false;
                textBox_fixedplaintext.Enabled = false;
                textBox_plaintext.Enabled = false;
                checkBox_FF.Checked = false;
                checkBox_classificationN.Checked = false;
            }
        }
        private void checkBox_classification8_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_classification8.Checked)
            {
                checkBox_FRRF.Checked = false;
                checkBox_randomgeneration.Checked = false;
                checkBox_alternate.Checked = false;
                checkBox_FFFF.Checked = false;
                checkBox_RF.Checked = false;
                checkBox_classification4.Checked = false;
                textBox_fixedplaintext.Enabled = false;
                textBox_plaintext.Enabled = false;
                checkBox_classificationN.Checked = false;
                checkBox_FF.Checked = false;
            }
        }
        private void checkBox_classificationN_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_classificationN.Checked)
            {
                checkBox_FRRF.Checked = false;
                checkBox_randomgeneration.Checked = false;
                checkBox_alternate.Checked = false;
                checkBox_FFFF.Checked = false;
                checkBox_RF.Checked = false;
                checkBox_classification4.Checked = false;
                checkBox_classification8.Checked = false;
                textBox_fixedplaintext.Enabled = false;
                textBox_plaintext.Enabled = false;
            }
        }

        private void checkBox_PseudoRandom_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_PseudoRandom.Checked)
            {
                checkBox_FRRF.Checked = false;
                checkBox_randomgeneration.Checked = false;
                checkBox_alternate.Checked = false;
                checkBox_FFFF.Checked = false;
                checkBox_RF.Checked = false;
                checkBox_classification4.Checked = false;
                checkBox_classification8.Checked = false;
                textBox_fixedplaintext.Enabled = false;
                textBox_plaintext.Enabled = false;
                checkBox_classificationN.Checked = false;
            }
        }
        private void button_fixedplaintext_Click(object sender, EventArgs e)
        {
            try
            {
                textBox_fixedplaintext.Text = Utils.formHexlString(textBox_fixedplaintext.Text);
                CheckTextContent(textBox_fixedplaintext.Text, 1);            //检查用于输入格式，"1"表示检查的是明文
            }
            catch (Exception ex)
            {
                IsError[1] = true;
                statusMessage(ex.Message, true);
            }
        }

        private void textBox_fixedplaintext_KeyPress(object sender, KeyPressEventArgs e)
        {
            LimitKeyPress2(e);
        }

        private void textBox_fixedplaintext_Leave(object sender, EventArgs e)
        {
            try
            {
                textBox_fixedplaintext.Text = Utils.formHexlString(textBox_fixedplaintext.Text);
                CheckTextContent(textBox_fixedplaintext.Text, 1);            //检查用于输入格式，"1"表示检查的是明文
            }
            catch (Exception ex)
            {
                IsError[1] = true;
                statusMessage(ex.Message, true);
            }
        }
        //2018.10.30visa出现异常时停止加密
        public void VISA_Wrong_Exit()
        {
            state = appState.Stop;                                      //state表示应用进行的状态,Stop是停止加密
            UpdateFormEnabling();                                       //根据state值，设置button,text等控件enable属性
            ctrl.Cancel();
        }

    }//MainForm类结束
   
}