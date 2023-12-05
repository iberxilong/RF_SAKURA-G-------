/**************************************************************************
 * 新增日期：2018年5月14日
 * 作者：数缘科技
 * 
 * 内容说明：此.cs文件主要是示波器的配置函数，一般不需要改动
***************************************************************************/

using PS3000ACSConsole;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Ivi.Visa.Interop;
namespace MathMagic
{
    // 用于两个线程传递错误信息
    public delegate void SendErrorMessage(string ex,bool isErrorMessage);
    

    // 用于两个线程传递波形数据
    public delegate void DrawLineEventHander(Object sender, DrawLineEventArgs e);
    public class DrawLineEventArgs : EventArgs
    {
        public DataTable dt;
        public long current_trace;
        public int num;
        public List<double> listStrArr;//数组List，相当于可以无限扩大的二维数组。
        public List<int> xData;//数组List，相当于可以无限扩大的二维数组。
        public DrawLineEventArgs(DataTable dt, long current_trace)
        {
            this.dt = dt;
            this.current_trace = current_trace;
        }
        public DrawLineEventArgs(long current_trace,int num, List<int> xData, List<double> listStrArr)
        {
            this.xData = xData;
            this.listStrArr = listStrArr;
            this.num = num;
            this.current_trace = current_trace;
        }
    }

    struct ChannelSettings                                          // 示波器通道设置
    {
        public bool DCcoupled;
        public Imports.Range range;
        public bool enabled;
    }

    class Pwq                                                       //pico中引入的类
    {
        public Imports.PwqConditions[] conditions;
        public short nConditions;
        public Imports.ThresholdDirection direction;
        public uint lower;
        public uint upper;
        public Imports.PulseWidthType type;

        public Pwq(Imports.PwqConditions[] conditions,
            short nConditions,
            Imports.ThresholdDirection direction,
            uint lower, uint upper,
            Imports.PulseWidthType type)
        {
            this.conditions = conditions;
            this.nConditions = nConditions;
            this.direction = direction;
            this.lower = lower;
            this.upper = upper;
            this.type = type;
        }
    }
    public partial class  Controller
    {
        private int _digitalPorts;
        internal VisaComInstrument myScope;//2018.10.14
        /// <summary>
        /// 打开示波器，判断切换示波器的供电方式
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public uint deviceOpen(out short handle)//2018.10.12 no use deviceOpen //打开设备
        {
            uint status = Imports.OpenUnit(out handle, null);
            if (status != (short)Imports.PICO_OK)
            {
                status = PowerSourceSwitch(handle, status);

                if (status == (short)Imports.PICO_POWER_SUPPLY_UNDERVOLTAGE)
                                                                    //切换供电/接口方式
                {
                    status = PowerSourceSwitch(handle, status);
                }
                else if (status == (short)Imports.PICO_USB3_0_DEVICE_NON_USB3_0_PORT)
                {
                    status = PowerSourceSwitch(handle, Imports.PICO_POWER_SUPPLY_NOT_CONNECTED);
                }

            }
            if (status != (short)Imports.PICO_OK)
            {
                Cancel();                                           //示波器打不开，则取消
                isOpen = false;                                     //判断是否能打开示波器
            }
            return status;
        }

        /****************************************************************************
         * Run - 获取示波器型号，配置示波器，通道电压范围，触发条件等
         ****************************************************************************/
        public void Run()
        {

            /*GetDeviceInfo();
            _channelSettings = new ChannelSettings[MAX_CHANNELS];

            for (int i = 0; i < _channelCount; i++)
            {
                _channelSettings[i].enabled = true;
                _channelSettings[i].DCcoupled = true;
                _channelSettings[i].range = Imports.Range.Range_500MV;
            }
            _channelSettings[1].range = Imports.Range.Range_100MV;  //B通道的量程设置为100mV
            CollectBlockRapid();*/

        }

        /****************************************************************************
         * GetDeviceInfo - 初始化示波器的配置信息
         ****************************************************************************/
        void GetDeviceInfo()
        {
            string[] description = {
                           "Driver Version    ",
                           "USB Version       ",
                           "Hardware Version  ",
                           "Variant Info      ",
                           "Serial            ",
                           "Cal Date          ",
                           "Kernel Ver        ",
                           "Digital Hardware  ",
                           "Analogue Hardware ",
                           "Firmware 1        ",
                           "Firmware 2        "
                         };

            System.Text.StringBuilder line = new System.Text.StringBuilder(80);//指定line字符数组最大长度为80

            if (_handle >= 0)
            {
                for (int i = 0; i < 11; i++)
                {
                    short requiredSize;
                    //根据不同i,查找示波器类型不同信息
                    Imports.GetUnitInfo(_handle, line, 80, out requiredSize, i);
                    if (i == 3)
                    {
                        if (line[1] == '4')                         // PS340XA/B device
                            _channelCount = QUAD_SCOPE;
                        else
                            _channelCount = DUAL_SCOPE;
                    }
                    if (i == 3)
                    {
                        if (line.ToString().EndsWith("MSO"))
                            _digitalPorts = 2;
                        else
                            _digitalPorts = 0;

                    }
                }
            }
        }

        /****************************************************************************
         * CollectBlockRapid - 设置触发条件
         ****************************************************************************/
        void CollectBlockRapid()
        {

            uint numRapidCaptures = 1;
            bool valid = false;
            //触发电压
            short triggerVoltage = mv_to_adc(150, (short)_channelSettings[(int)Imports.Channel.ChannelA].range);
            Imports.TriggerChannelProperties[] sourceDetails = new Imports.TriggerChannelProperties[] {
                new Imports.TriggerChannelProperties(triggerVoltage,
                                                         256*10,
                                                         triggerVoltage,
                                                         256*10,
                                                         Imports.Channel.ChannelA,      //通道A触发
                                                         Imports.ThresholdMode.Level)};

            Imports.TriggerConditions[] conditions = new Imports.TriggerConditions[] {
              new Imports.TriggerConditions(Imports.TriggerState.True,                  //A作为触发源，设置为true
                                            Imports.TriggerState.DontCare,              //其他不关心
                                            Imports.TriggerState.DontCare,
                                            Imports.TriggerState.DontCare,
                                            Imports.TriggerState.DontCare,
                                            Imports.TriggerState.DontCare,
                                            Imports.TriggerState.DontCare)};

            Imports.ThresholdDirection[] directions = new Imports.ThresholdDirection[]
                                            { Imports.ThresholdDirection.Rising,        //设置上升沿触发
                                            Imports.ThresholdDirection.None,
                                            Imports.ThresholdDirection.None,
                                            Imports.ThresholdDirection.None,
                                            Imports.ThresholdDirection.None,
                                            Imports.ThresholdDirection.None };
            do
            {
                try
                {
                    numRapidCaptures = 1;
                    valid = true;
                }
                catch
                {
                    valid = false;
                }

            } while (Imports.SetNoOfRapidCaptures(_handle, numRapidCaptures) > 0 || !valid);

            int maxSamples;
            Imports.MemorySegments(_handle, numRapidCaptures, out maxSamples);

            SetDefaults();
            /* 设置触发	*/
            SetTrigger(sourceDetails, 1, conditions, 1, directions, null, 0, 0, 0);

        }

        /****************************************************************************
        * SetDefaults - 设置通道状态:enable、电压
        ****************************************************************************/
        void SetDefaults()
        {
            for (int i = 0; i < _channelCount; i++) 
            {
                Imports.SetChannel(_handle, Imports.Channel.ChannelA + i,
                                   (short)(_channelSettings[(int)(Imports.Channel.ChannelA + i)].enabled ? 1 : 0),
                                   (short)(_channelSettings[(int)(Imports.Channel.ChannelA + i)].DCcoupled ? 1 : 0),
                                   _channelSettings[(int)(Imports.Channel.ChannelA + i)].range,
                                   0);
            }
        }

        /****************************************************************************
      *  SetTrigger  (Non-Digital Version)
      *  设置所有的触发参数，并且调用触发方法
      ****************************************************************************/
        uint SetTrigger(Imports.TriggerChannelProperties[] channelProperties,
            short nChannelProperties,
            Imports.TriggerConditions[] triggerConditions,
            short nTriggerConditions,
            Imports.ThresholdDirection[] directions,
            Pwq pwq,
            uint delay,
            short auxOutputEnabled,
            int autoTriggerMs)
        {
            uint status;

            if ((status = Imports.SetTriggerChannelProperties(_handle, channelProperties, nChannelProperties, auxOutputEnabled,
                                                                autoTriggerMs)) != 0)
            {
                return status;
            }

            if ((status = Imports.SetTriggerChannelConditions(_handle, triggerConditions, nTriggerConditions)) != 0)
            {
                return status;
            }

            if (directions == null) directions = new Imports.ThresholdDirection[] { Imports.ThresholdDirection.None,
                Imports.ThresholdDirection.None, Imports.ThresholdDirection.None, Imports.ThresholdDirection.None,
                Imports.ThresholdDirection.None, Imports.ThresholdDirection.None};

            if ((status = Imports.SetTriggerChannelDirections(_handle,
                                                              directions[(int)Imports.Channel.ChannelA],
                                                              directions[(int)Imports.Channel.ChannelB],
                                                              directions[(int)Imports.Channel.ChannelC],
                                                              directions[(int)Imports.Channel.ChannelD],
                                                              directions[(int)Imports.Channel.External],
                                                              directions[(int)Imports.Channel.Aux])) != 0)
            {
                return status;
            }

            if ((status = Imports.SetTriggerDelay(_handle, delay)) != 0)
            {
                return status;
            }

            if (pwq == null) pwq = new Pwq(null, 0, Imports.ThresholdDirection.None, 0, 0, Imports.PulseWidthType.None);

            status = Imports.SetPulseWidthQualifier(_handle, pwq.conditions,
                                                    pwq.nConditions, pwq.direction,
                                                    pwq.lower, pwq.upper, pwq.type);

            return status;
        }

        /****************************************************************************
         * BlockCallback - RunBlock设置示波器采样点数，采样率后，等待示波器准备好后
         * 回调BlockCallback。将_ready设为true提示波形准备好
         ****************************************************************************/
        void BlockCallback(short handle, short status, IntPtr pVoid)
        {
            // flag to say done reading data
            if (status != (short)Imports.PICO_CANCELLED)
            {
                //_ready信号是代表波形是否准备好，在回调函数BlockCallback()中将_ready设成true
                _ready = true;  
            }
        }



        /****************************************************************************
        * SetDigitals - enable Digital Channels
        ****************************************************************************/
        void SetDigitals()
        {
            Imports.Channel port;
            uint status;
            short logicLevel;
            float logicVoltage = 1.5f;
            short maxLogicVoltage = 5;
            short enabled = 1;

            status = Imports.PICO_OK;

            // Set logic threshold
            logicLevel = (short)((logicVoltage / maxLogicVoltage) * Imports.MaxLogicLevel);

            // Enable Digital ports
            for (port = Imports.Channel.PS3000A_DIGITAL_PORT0; port < Imports.Channel.PS3000A_DIGITAL_PORT2; port++)
            {
                status = Imports.SetDigitalPort(_handle, port, enabled, logicLevel);
            }
            Console.WriteLine(status != (short)Imports.PICO_OK ? "SetDigitals:Imports.SetDigitalPort Status = 0x{0:X6}" : "", status);

        }


        /****************************************************************************
         * DisableDigital - disable Digital Channels
         ****************************************************************************/
        void DisableDigital()
        {
            Imports.Channel port;
            uint status;

            status = Imports.PICO_OK;

            // Disable Digital ports 
            for (port = Imports.Channel.PS3000A_DIGITAL_PORT0; port <= Imports.Channel.PS3000A_DIGITAL_PORT1; port++)
            {
                status = Imports.SetDigitalPort(_handle, port, 0, 0);
            }
            Console.WriteLine(status != (short)Imports.PICO_OK ? "DisableDigital:Imports.SetDigitalPort Status = 0x{0:X6}" : "", status);
        }


        /****************************************************************************
        * DisableAnalogue - disable analogue Channels
        ****************************************************************************/
        void DisableAnalogue()
        {
            uint status;

            status = Imports.PICO_OK;

            // Disable analogue ports
            for (int i = 0; i < _channelCount; i++)
            {
                status = Imports.SetChannel(_handle, Imports.Channel.ChannelA + i, 0, 0, 0, 0);
            }
        }


        /****************************************************************************
         * adc_to_mv
         *
         * 16位ADC计数转换成一个毫伏值
         * 
         ****************************************************************************/
        float adc_to_mv(int raw, int ch)
        {
            //Imports.MaxValue;屏幕划分的最大行数
            return (raw * inputRanges[ch])*1.0f / Imports.MaxValue;
        }

        /****************************************************************************
         * mv_to_adc
         *
         *一个毫伏值转换成16位ADC计数
         *
         *  (useful for setting trigger thresholds)
         ****************************************************************************/
        short mv_to_adc(short mv, short ch)
        {
            return (short)((mv * Imports.MaxValue) / inputRanges[ch]);
        }


        /****************************************************************************
         *
         * SelectTimebase - 设置TIEMBASE
         *
         ****************************************************************************/
        void SetTimebase()
        {
            int timeInterval;
            int maxSamples;
            bool valid = false;

            Console.WriteLine("Specify timebase index:");

            do
            {
                try
                {

                    _timebase = 2;
                    valid = true;
                }
                catch
                {
                    valid = false;
                    Console.WriteLine("\nEnter numeric values only");
                }

            } while (!valid);

            while (Imports.GetTimebase(_handle, _timebase, (int)BUFFER_SIZE, out timeInterval, 1, out maxSamples, 0) != 0)
            {
                Console.WriteLine("Selected timebase index {0} could not be used", _timebase);
                _timebase++;
            }

            Console.WriteLine("Timebase index {0} - {1} ns", _timebase, timeInterval);
            _oversample = 1;
        }

        /****************************************************************************
        * PowerSourceSwitch - 处理状态的错误与电源连接
        ****************************************************************************/
        static uint PowerSourceSwitch(short handle, uint status)    //更换供电模式
        {
            status = Imports.ChangePowerSource(handle, status);
            return status;
        }
    }
    //2018.10.13定义VISA类用于连接操作示波器
    class VisaComInstrument
    {
	    private ResourceManager m_ResourceManager;
	    private FormattedIO488 m_IoObject;
	    private string m_strVisaAddress;
        //2018.10.17
        /*public int maxLength;
        public double fType1 , fType2;
        public double fPoints1, fPoints2;
        public double fCount1, fCount2;
        public double fXincrement1, fXincrement2;
        public double fXorigin1, fXorigin2;
        public double fXreference1, fXreference2;
        public double fYincrement1, fYincrement2;
        public double fYorigin1, fYorigin2;
        public double fYreference1, fYreference2;*/

        


        // Constructor.
        public VisaComInstrument(string strVisaAddress)
	    {
		    // Save VISA address in member variable.
		    m_strVisaAddress = strVisaAddress;
		    // Open the default VISA COM IO object.
		    OpenIo();
		    // Clear the interface.
		    m_IoObject.IO.Clear();
	    }
	    public void DoCommand(string strCommand)
	    {
		    // Send the command.
		    m_IoObject.WriteString(strCommand, true);
		    // Check for inst errors.
		    CheckInstrumentErrors(strCommand);//2018.10.18
	    }
	    public void DoCommandIEEEBlock(string strCommand,byte[] DataArray)
	    {
		    // Send the command to the device.
		    m_IoObject.WriteIEEEBlock(strCommand, DataArray, true);
		    // Check for inst errors.
		    CheckInstrumentErrors(strCommand);
	    }
	    public string DoQueryString(string strQuery)
	    {
		    // Send the query.
		    m_IoObject.WriteString(strQuery, true);
		    // Get the result string.
		    string strResults;
		    strResults = m_IoObject.ReadString();
		    // Check for inst errors.
		    CheckInstrumentErrors(strQuery);//2018.10.18
		    // Return results string.
		    return strResults;
	    }
	    public double DoQueryNumber(string strQuery)
	    {
		    // Send the query.
		    m_IoObject.WriteString(strQuery, true);
		    // Get the result number.
		    double fResult;

		    fResult = (double)m_IoObject.ReadNumber(IEEEASCIIType.ASCIIType_R8, true);
		    // Check for inst errors.
		    CheckInstrumentErrors(strQuery);
		    // Return result number.
		    return fResult;
	    }
        public double[] DoQueryNumbers(string strQuery)
        {
            // Send the query.
            m_IoObject.WriteString(strQuery, true);
            // Get the result numbers.
            double[] fResultsArray;
            fResultsArray = (double[])m_IoObject.ReadList(IEEEASCIIType.ASCIIType_R8, ",;");
           
            // Check for inst errors.
            CheckInstrumentErrors(strQuery);//2018.10.18
            // Return result numbers.
            return fResultsArray;
        }
        public byte[] DoQueryIEEEBlock(string strQuery)
        {
            // Send the query.
            m_IoObject.WriteString(strQuery, true);
            // Get the results array. 
            //System.Threading.Thread.Sleep(2000); // Delay before reading.
		    byte[] ResultsArray;
            ResultsArray = (byte[])m_IoObject.ReadIEEEBlock(IEEEBinaryType.BinaryType_UI1, false, true);
            // Check for inst errors.
            CheckInstrumentErrors(strQuery);//2018.10.18
		    // Return results array.
		    return ResultsArray;
	    }
	    public void CheckInstrumentErrors(string strCommand)
	    {
		    // Check for instrument errors.
		    string strInstrumentError;
		    bool bFirstError = true;
		    do // While not "0,No error".
		    {
			    m_IoObject.WriteString(":SYSTem:ERRor?", true);
			    strInstrumentError = m_IoObject.ReadString();
			    if (!strInstrumentError.ToString().StartsWith("+0,"))
			    {
				    if (bFirstError)
				    {
					    Console.WriteLine("KETSIGHT ERROR(s) for command '{0}': ",strCommand);
					    bFirstError = false;
				    }
			    }
		    } while (!strInstrumentError.ToString().StartsWith("+0,"));
	    }
	    private void OpenIo()
	    {
		    m_ResourceManager = new ResourceManager();
		    m_IoObject = new FormattedIO488();
		    // Open the default VISA COM IO object.
		    try
		    {
			    m_IoObject.IO =(IMessage)m_ResourceManager.Open(m_strVisaAddress,AccessMode.NO_LOCK, 0, "");
		    }
		    catch (Exception e)
		    {
			    Console.WriteLine("An error occurred: {0}", e.Message);
		    }
	    }
	    public void SetTimeoutSeconds(int nSeconds)
	    {
		    m_IoObject.IO.Timeout = nSeconds * 1000;
	    }
	    public void Close()
	    {
		    try
		    {
			    m_IoObject.IO.Close();
		    }
		    catch { }
            //2018.10.22
            /*m_IoObject.IO = null;
            System.Runtime.InteropServices.Marshal.ReleaseComObject(m_IoObject);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(m_ResourceManager);*/
            //垃圾回收机制
            /*try
		    {
			    Marshal.ReleaseComObject(m_IoObject);
		    }
		    catch { }
		    try
		    {
			    Marshal.ReleaseComObject(m_ResourceManager);
		    }
		    catch { }*/
        }
    }
}
