using System;
using System.IO;
using System.Threading;

namespace PS3000ACSConsole
{
    struct ChannelSettings
    {
        public bool DCcoupled;
        public Imports.Range range;
        public bool enabled;
    }

    class Pwq
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

    public class PS3000ACSConsole1
    {

        private readonly short _handle;
        public const int BUFFER_SIZE = 1024;
        public const int MAX_CHANNELS = 4;
        public const int QUAD_SCOPE = 4;
        public const int DUAL_SCOPE = 2;


        uint _timebase = 1;
        short _oversample = 1;
        bool _scaleVoltages = true;

        ushort[] inputRanges = { 10, 20, 50, 100, 200, 500, 1000, 2000, 500, 10000, 20000, 50000 };
        bool _ready = false;
        //short _trig = 0;
        //uint _trigAt = 0;
        //int _sampleCount = 0;
        //uint _startIndex = 0;
        //bool _autoStop;

        //short[][] appBuffers;
        //short[][] buffers;
        //short[][] appDigiBuffers;
        //short[][] digiBuffers;

        private ChannelSettings[] _channelSettings;
        private int _channelCount;
        private Imports.Range _firstRange;
        private Imports.Range _lastRange;
        private int _digitalPorts;
        private Imports.ps3000aBlockReady _callbackDelegate;
        //private string StreamFile = "stream.txt";
        private string BlockFile = "RapBlockTrace.txt";
        /****************************************************************************
         * Callback
         * used by PS3000a data block collection calls, on receipt of data.
         * used to set global flags etc checked by user routines
         ****************************************************************************/
        void BlockCallback(short handle, short status, IntPtr pVoid)
        {
            // flag to say done reading data
            if (status != (short)Imports.PICO_CANCELLED)
            {
                _ready = true;  //_ready信号是代表波形是否准备好，在回调函数BlockCallback()中设成true
            }
        }

        /****************************************************************************
         * SetDefaults - restore default settings
         ****************************************************************************/
        void SetDefaults()
        {
            for (int i = 0; i < _channelCount; i++) // reset channels to most recent settings
            {
                Imports.SetChannel(_handle, Imports.Channel.ChannelA + i,
                                   (short)(_channelSettings[(int)(Imports.Channel.ChannelA + i)].enabled ? 1 : 0),
                                   (short)(_channelSettings[(int)(Imports.Channel.ChannelA + i)].DCcoupled ? 1 : 0),
                                   _channelSettings[(int)(Imports.Channel.ChannelA + i)].range,
                                   0);
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

            Console.WriteLine(status != (short)Imports.PICO_OK ? "DisableAnalogue:Imports.SetChannel Status = 0x{0:X6}" : "", status);
        }


        /****************************************************************************
         * adc_to_mv
         *
         * Convert an 16-bit ADC count into millivolts
         ****************************************************************************/
        int adc_to_mv(int raw, int ch)
        {
            return (raw * inputRanges[ch]) / Imports.MaxValue;
        }

        /****************************************************************************
         * mv_to_adc
         *
         * Convert a millivolt value into a 16-bit ADC count
         *
         *  (useful for setting trigger thresholds)
         ****************************************************************************/
        short mv_to_adc(short mv, short ch)
        {
            return (short)((mv * Imports.MaxValue) / inputRanges[ch]);
        }
        /****************************************************************************
         * RapidBlockDataHandler
         * - Used by the CollectBlockRapid routine
         * - acquires data (user sets trigger mode before calling), displays 10 items
         *   and saves all to data.txt
         * Input :
         * - nRapidCaptures : the user specified number of blocks to capture
         ****************************************************************************/
        private void RapidBlockDataHandler(uint nRapidCaptures)
        {
            uint status;
            int numChannels = _channelCount;
            uint numSamples = BUFFER_SIZE;

            // Run the rapid block capture
            int timeIndisposed;
            _ready = false; //用于检测波形是否Ready

            _callbackDelegate = BlockCallback; //回调里将_ready已经设成了true
            
            //设置RunBlock触发前采多少波

            Imports.RunBlock(_handle,
                        10,//触发前
                        (int)numSamples,//触发后
                        _timebase,
                        _oversample,
                        out timeIndisposed,
                        0,
                        _callbackDelegate,
                        IntPtr.Zero);

            Console.WriteLine("Waiting for data...Press a key to abort");

            while (!_ready && !Console.KeyAvailable)
            {
                Thread.Sleep(100);
            }
            
            if (Console.KeyAvailable) Console.ReadKey(true); // clear the key

            Imports.Stop(_handle);


            // Set up the data arrays and pin them
            short[][][] values = new short[nRapidCaptures][][];
            PinnedArray<short>[,] pinned = new PinnedArray<short>[nRapidCaptures, numChannels];

            for (ushort segment = 0; segment < nRapidCaptures; segment++)
            {
                values[segment] = new short[numChannels][];
                for (short channel = 0; channel < numChannels; channel++)
                {
                    if (_channelSettings[channel].enabled)
                    {
                        values[segment][channel] = new short[numSamples];
                        pinned[segment, channel] = new PinnedArray<short>(values[segment][channel]);

                        status = Imports.SetDataBuffer(_handle, (Imports.Channel)channel, values[segment][channel], (int) numSamples, segment, Imports.RatioMode.None);

                        if (status != Imports.PICO_OK)
                        {
                            Console.WriteLine("RapidBlockDataHandler:Imports.SetDataBuffer Channel {0} Status = 0x{1:X6}", (char)('A' + channel), status);
                        }

                    }
                    else
                    {
                        status = Imports.SetDataBuffer(_handle, (Imports.Channel)channel, null, 0, segment, Imports.RatioMode.None);

                        if (status != Imports.PICO_OK)
                        {
                            Console.WriteLine("RapidBlockDataHandler:Imports.SetDataBuffer Channel {0} Status = 0x{1:X6}", (char)('A' + channel), status);
                        }

                    }
                }
            }

            // Read the data
            short[] overflows = new short[nRapidCaptures];

            status = Imports.GetValuesRapid(_handle, ref numSamples, 0, (ushort)(nRapidCaptures - 1), 1, Imports.RatioMode.None, overflows);

            /* Print out the first 10 readings, converting the readings to mV if required */
            Console.WriteLine("\nValues in {0}", (_scaleVoltages) ? ("mV") : ("ADC Counts"));
            Console.WriteLine();

            for (int seg = 0; seg < nRapidCaptures; seg++)
            {
                Console.WriteLine("Capture {0}:", seg);
                Console.WriteLine();

                TextWriter writer = new StreamWriter(BlockFile, false);
                writer.Write("For each of the {0} Channels, results shown are....", _channelCount);
                writer.WriteLine();
                writer.WriteLine("Time interval Maximum Aggregated value ADC Count & mV, Minimum Aggregated value ADC Count & mV");
                writer.WriteLine();

                for (int i = 0; i < _channelCount; i++)
                {
                    if (_channelSettings[i].enabled)
                    {
                        writer.Write("num   Ch        mV   ");
                    }
                }
                 
                 

                writer.WriteLine();

                for (int i = 0; i < 30; i++)    //在这里设置快速块模式每块采多少个点
                {
                    for (int chan = 0; chan < _channelCount; chan++)
                    {
                        if (_channelSettings[chan].enabled)
                        {
                            Console.Write("{0}\t", _scaleVoltages ?
                                                    adc_to_mv(pinned[seg, chan].Target[i], (int)_channelSettings[(int)(Imports.Channel.ChannelA + chan)].range) // If _scaleVoltages, show mV values
                                                    : pinned[seg, chan].Target[i]);                                                                          // else show ADC counts
                            writer.Write("{0,5}  ", (i));
                            writer.Write("Ch{0} {1,7}   ",
                                           (char)('A' + chan),
                                            adc_to_mv(pinned[seg, chan].Target[i], (int)_channelSettings[(int)(Imports.Channel.ChannelA + chan)].range));
                        }
                        writer.WriteLine();
                    }

                    Console.WriteLine();
                }






                Console.WriteLine();
            }
            // Un-pin the arrays
            foreach (PinnedArray<short> p in pinned)
            {
                if (p != null)
                    p.Dispose();
            }

            //TODO: Do what ever is required with the data here.
        }


        /****************************************************************************
        *  WaitForKey
        *  Wait for user's keypress
        ****************************************************************************/
        private static void WaitForKey()
        {
            while (!Console.KeyAvailable) Thread.Sleep(100);
            if (Console.KeyAvailable) Console.ReadKey(true); // clear the key
        }

        /****************************************************************************
        *  SetTrigger  (Non-Digital Version)
        *  this function sets all the required trigger parameters, and calls the 
        *  triggering functions
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
       *  SetTrigger
       *  this overloaded version of SetTrigger includes digital parameters
       ****************************************************************************/
        uint SetTrigger(Imports.TriggerChannelProperties[] channelProperties,
        short nChannelProperties,
        Imports.TriggerConditionsV2[] triggerConditions,
        short nTriggerConditions,
        Imports.ThresholdDirection[] directions,
        Pwq pwq,
        uint delay,
        short auxOutputEnabled,
        int autoTriggerMs,
        Imports.DigitalChannelDirections[] digitalDirections,
        short nDigitalDirections)
        {
            uint status;

            if (
              (status = Imports.SetTriggerChannelProperties(_handle, channelProperties, nChannelProperties, auxOutputEnabled,
                                                   autoTriggerMs)) != 0)
            {
                return status;
            }

            if ((status = Imports.SetTriggerChannelConditionsV2(_handle, triggerConditions, nTriggerConditions)) != 0)
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

            if (_digitalPorts > 0)
            {
                if ((status = Imports.SetTriggerDigitalPort(_handle, digitalDirections, nDigitalDirections)) != 0)
                {
                    return status;
                }
            }

            return status;
        }
	    /****************************************************************************
        *  CollectBlockRapid
        *  this function demonstrates how to collect blocks of data
        * using the RapidCapture function
        ****************************************************************************/
        void CollectBlockRapid()
        {

            uint numRapidCaptures = 1;
            bool valid = false;
            short triggerVoltage = mv_to_adc(150, (short)_channelSettings[(int)Imports.Channel.ChannelA].range); // ChannelInfo stores ADC counts
            Imports.TriggerChannelProperties[] sourceDetails = new Imports.TriggerChannelProperties[] {
                new Imports.TriggerChannelProperties(triggerVoltage,
                                                         256*10,
                                                         triggerVoltage,
                                                         256*10,
                                                         Imports.Channel.ChannelA,
                                                         Imports.ThresholdMode.Level)};

            Imports.TriggerConditions[] conditions = new Imports.TriggerConditions[] {
              new Imports.TriggerConditions(Imports.TriggerState.True,
                                            Imports.TriggerState.DontCare,
                                            Imports.TriggerState.DontCare,
                                            Imports.TriggerState.DontCare,
                                            Imports.TriggerState.DontCare,
                                            Imports.TriggerState.DontCare,
                                            Imports.TriggerState.DontCare)};

            Imports.ThresholdDirection[] directions = new Imports.ThresholdDirection[]
	                                        { Imports.ThresholdDirection.Rising,
                                            Imports.ThresholdDirection.None, 
                                            Imports.ThresholdDirection.None, 
                                            Imports.ThresholdDirection.None, 
                                            Imports.ThresholdDirection.None,
                                            Imports.ThresholdDirection.None };

            Console.WriteLine("Collect rapid block triggered...");


            Console.Write("Collects when value rises past {0}", (_scaleVoltages) ?
                          adc_to_mv(sourceDetails[0].ThresholdMajor,
                                    (int)_channelSettings[(int)Imports.Channel.ChannelA].range)
                                    : sourceDetails[0].ThresholdMajor);
            Console.WriteLine("{0}", (_scaleVoltages) ? ("mV") : (" ADC Counts"));

            Console.WriteLine("Press a key to start...");
            Console.WriteLine("Collect rapid block...");
            //Console.WriteLine("Specify number of captures:");

            do
            {
                try
                {
                    //numRapidCaptures = uint.Parse(Console.ReadLine());这里改快速块模式多少块！！！！！目前是1块！
                    numRapidCaptures = 1;
                    valid = true;
                }
                catch
                {
                    valid = false;
                    Console.WriteLine("\nEnter numeric values only:");
                }

            } while (Imports.SetNoOfRapidCaptures(_handle, numRapidCaptures) > 0 || !valid);



            int maxSamples;
            Imports.MemorySegments(_handle, numRapidCaptures, out maxSamples);

            Console.WriteLine("Collecting {0} rapid blocks. Press a key to start", numRapidCaptures);

            //WaitForKey();

            SetDefaults();

            /* Trigger is optional, disable it for now	*/
            SetTrigger(sourceDetails, 1, conditions, 1, directions, null, 0, 0, 0);

           // BlockDataHandler("Ten readings after trigger", 0, Imports.Mode.ANALOGUE);
            
            RapidBlockDataHandler(numRapidCaptures);
        }
        /****************************************************************************
        * Initialise unit' structure with Variant specific defaults
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

            System.Text.StringBuilder line = new System.Text.StringBuilder(80);

            if (_handle >= 0)
            {
                for (int i = 0; i < 11; i++)
                {
                    short requiredSize;
                    Imports.GetUnitInfo(_handle, line, 80, out requiredSize, i);
                    if (i == 3)
                    {
                        if (line[1] == '4')    // PS340XA/B device
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
                    Console.WriteLine("{0}: {1}", description[i], line);
                }
            }
        }
        /****************************************************************************
         * Select input voltage ranges for channels A and B
         ****************************************************************************/
        void SetVoltages()
        {
            bool valid = false;
            short count = 0;

            /* See what ranges are available... */
            for (int i = (int)_firstRange; i <= (int)_lastRange; i++)
            {
                Console.WriteLine("{0} . {1} mV", i, inputRanges[i]);
            }

            do
            {
                /* Ask the user to select a range */
                Console.WriteLine("\nSpecify voltage range ({0}..{1})", _firstRange, _lastRange);
                Console.WriteLine("99 - switches channel off");
                for (int ch = 0; ch < _channelCount; ch++)
                {
                    Console.WriteLine("");
                    uint range = 8;

                    do
                    {
                        try
                        {
                            Console.WriteLine("Channel: {0}", (char)('A' + ch));
                            //range = uint.Parse(Console.ReadLine());   //输入电压值选项
                            range = 5;   //自写：5选项代表500mv
                            valid = true;
                        }
                        catch
                        {
                            valid = false;
                            Console.WriteLine("\nEnter numeric values only");
                        }

                    } while ((range != 99 && (range < (uint)_firstRange || range > (uint)_lastRange) || !valid));


                    if (range != 99)
                    {
                        _channelSettings[ch].range = (Imports.Range)range;
                        Console.WriteLine(" = {0} mV", inputRanges[range]);
                        _channelSettings[ch].enabled = true;
                        count++;
                    }
                    else
                    {
                        Console.WriteLine("Channel Switched off");
                        _channelSettings[ch].enabled = false;
                        _channelSettings[ch].range = Imports.Range.Range_MAX_RANGE - 1;
                    }
                }
                Console.Write(count == 0 ? "\n*** At least 1 channel must be enabled *** \n" : "");
            }
            while (count == 0); // must have at least one channel enabled

            SetDefaults();  // Set defaults now, so that if all but 1 channels get switched off, timebase updates to timebase 0 will work
        }

        /****************************************************************************
         *
         * Select _timebase, set _oversample to on and time units as nano seconds
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
                    _timebase = uint.Parse(Console.ReadLine());
                    valid = true;
                }
                catch
                {
                    valid = false;
                    Console.WriteLine("\nEnter numeric values only");
                }

            } while (!valid);

            while (Imports.GetTimebase(_handle, _timebase, BUFFER_SIZE, out timeInterval, 1, out maxSamples, 0) != 0)
            {
                Console.WriteLine("Selected timebase index {0} could not be used", _timebase);
                _timebase++;
            }

            Console.WriteLine("Timebase index {0} - {1} ns", _timebase, timeInterval);
            _oversample = 1;
        }

        /****************************************************************************
       * DisplaySettings 
       * Displays information about the user configurable settings in this example
       ***************************************************************************/
        void DisplaySettings()
        {
            int ch;
            int voltage;

            Console.WriteLine("\n\nReadings will be scaled in {0}", (_scaleVoltages) ? ("mV") : ("ADC counts"));

            for (ch = 0; ch < _channelCount; ch++)
            {
                if (!_channelSettings[ch].enabled)
                {
                    Console.WriteLine("Channel {0} Voltage Range = Off", (char)('A' + ch));
                }
                else
                {
                    voltage = inputRanges[(int)_channelSettings[ch].range];
                    Console.Write("Channel {0} Voltage Range = ", (char)('A' + ch));

                    if (voltage < 1000)
                    {
                        Console.WriteLine("{0}mV", voltage);
                    }
                    else
                    {
                        Console.WriteLine("{0}V", voltage / 1000);
                    }
                }
            }
            Console.WriteLine();
        }

        /****************************************************************************
        * Run - show menu and call user selected options
        ****************************************************************************/
        public void Run()
        {
            // setup devices
            GetDeviceInfo();
            _timebase = 1;

            _firstRange = Imports.Range.Range_100MV;
            _lastRange = Imports.Range.Range_20V;
            _channelSettings = new ChannelSettings[MAX_CHANNELS];

            for (int i = 0; i < MAX_CHANNELS; i++)
            {
                _channelSettings[i].enabled = true;
                _channelSettings[i].DCcoupled = true;
                _channelSettings[i].range = Imports.Range.Range_5V;
            }


            DisplaySettings();//核心1
            SetVoltages();//核心2
            CollectBlockRapid();//核心3
            //WaitForKey();
        }

        public PS3000ACSConsole1(short handle)
        {
            _handle = handle;
        }



        /****************************************************************************
        * PowerSourceSwitch - Handle status errors connected with the power source
        ****************************************************************************/
        static uint PowerSourceSwitch(short handle, uint status)           //更换供电模式 开机的源代码
        {
            
            status = Imports.ChangePowerSource(handle, status);
            return status;
        }



        public static uint deviceOpen(out short handle)      //打开设备
        {
            uint status = Imports.OpenUnit(out handle, null);

            if (status != (short)Imports.PICO_OK)
            {
                status = PowerSourceSwitch(handle, status);

                if (status == (short)Imports.PICO_POWER_SUPPLY_UNDERVOLTAGE)
                {
                    status = PowerSourceSwitch(handle, status);
                }
                else if (status == (short)Imports.PICO_USB3_0_DEVICE_NON_USB3_0_PORT)
                {
                    status = PowerSourceSwitch(handle, Imports.PICO_POWER_SUPPLY_NOT_CONNECTED);
                }
                else
                {
                    // Do nothing
                }
            }


            if (status != (short)Imports.PICO_OK)
            {
                Console.WriteLine("Unable to open device");
                Console.WriteLine("Error code : 0x{0}", Convert.ToString(status, 16));
                WaitForKey();
            }
            else
            {
                Console.WriteLine("Handle: {0}", handle);
            }

            return status;
        }
        static void Main()
        {
            short handle;
            if (deviceOpen(out handle) == 0)//Main函数的工作1：开机
            {
                PS3000ACSConsole1 consoleExample = new PS3000ACSConsole1(handle);
                consoleExample.Run();//Main函数的工作2：执行采波存波

                Imports.CloseUnit(handle);//Main函数的工作3：关闭设备
                Console.WriteLine("Hello!");
            }
            WaitForKey();
        }//Main
    }//类PS3000ACSConsole1
}//命名空间
