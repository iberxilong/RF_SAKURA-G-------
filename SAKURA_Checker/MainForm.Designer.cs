using System;

namespace MathMagic
{
    partial class MainForm
    {
   
        private System.ComponentModel.IContainer components = null;

       
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows.Winform 生成的代码

        /// <summary>
        /// 初始化控件，该处代码不要修改。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.comboBox_target = new System.Windows.Forms.ComboBox();
            this.label_interface = new System.Windows.Forms.Label();
            this.label_target = new System.Windows.Forms.Label();
            this.label_traces = new System.Windows.Forms.Label();
            this.textBox_traces = new System.Windows.Forms.TextBox();
            this.label_key = new System.Windows.Forms.Label();
            this.label_plaintext = new System.Windows.Forms.Label();
            this.checkBox_endless = new System.Windows.Forms.CheckBox();
            this.textBox_key = new System.Windows.Forms.TextBox();
            this.button_changekey = new System.Windows.Forms.Button();
            this.textBox_plaintext = new System.Windows.Forms.TextBox();
            this.button_changeplaintext = new System.Windows.Forms.Button();
            this.checkBox_randomgeneration = new System.Windows.Forms.CheckBox();
            this.label_wait = new System.Windows.Forms.Label();
            this.textBox_wait = new System.Windows.Forms.TextBox();
            this.button_single = new System.Windows.Forms.Button();
            this.button_start = new System.Windows.Forms.Button();
            this.button_stop = new System.Windows.Forms.Button();
            this.checkBox_continueiferror = new System.Windows.Forms.CheckBox();
            this.label_rtraces = new System.Windows.Forms.Label();
            this.textBox_rtraces = new System.Windows.Forms.TextBox();
            this.label_rplaintext = new System.Windows.Forms.Label();
            this.label_rciphertext = new System.Windows.Forms.Label();
            this.label_ranswer = new System.Windows.Forms.Label();
            this.label_rdifference = new System.Windows.Forms.Label();
            this.label_relapsed = new System.Windows.Forms.Label();
            this.textBox_rplaintext = new System.Windows.Forms.TextBox();
            this.textBox_rciphertext = new System.Windows.Forms.TextBox();
            this.textBox_ranswer = new System.Windows.Forms.TextBox();
            this.textBox_rdifference = new System.Windows.Forms.TextBox();
            this.textBox_AllTime = new System.Windows.Forms.TextBox();
            this.textBox_s = new System.Windows.Forms.TextBox();
            this.label_waitms = new System.Windows.Forms.Label();
            this.label_relapsed_ms = new System.Windows.Forms.Label();
            this.textBox_interface = new System.Windows.Forms.TextBox();
            this.statusStrip_status = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar_progress = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel_message = new System.Windows.Forms.ToolStripStatusLabel();
            this.checkBox_randomgeneration_mask = new System.Windows.Forms.CheckBox();
            this.button_changeplaintext_mask = new System.Windows.Forms.Button();
            this.textBox_plaintext_mask = new System.Windows.Forms.TextBox();
            this.label_plaintext_mask = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBox_IsOtherScope = new System.Windows.Forms.CheckBox();
            this.label_buffer = new System.Windows.Forms.Label();
            this.textBox_buffer = new System.Windows.Forms.TextBox();
            this.textBox_ShowNum = new System.Windows.Forms.TextBox();
            this.textBox_trigger = new System.Windows.Forms.TextBox();
            this.label_trigger = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label_wave = new System.Windows.Forms.Label();
            this.button_OpenPico = new System.Windows.Forms.Button();
            this.checkBox_PicoState = new System.Windows.Forms.CheckBox();
            this.buttonNext = new System.Windows.Forms.Button();
            this.buttonLast = new System.Windows.Forms.Button();
            this.checkBox_alternate = new System.Windows.Forms.CheckBox();
            this.textBox_fixedplaintext = new System.Windows.Forms.TextBox();
            this.checkBox_FRRF = new System.Windows.Forms.CheckBox();
            this.checkBox_RF = new System.Windows.Forms.CheckBox();
            this.checkBox_FFFF = new System.Windows.Forms.CheckBox();
            this.checkBox_FF = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBox_classification4 = new System.Windows.Forms.CheckBox();
            this.checkBox_classification8 = new System.Windows.Forms.CheckBox();
            this.textBox_classificationN = new System.Windows.Forms.TextBox();
            this.checkBox_classificationN = new System.Windows.Forms.CheckBox();
            this.checkBox_PseudoRandom = new System.Windows.Forms.CheckBox();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.statusStrip_status.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBox_target
            // 
            this.comboBox_target.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_target.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox_target.FormattingEnabled = true;
            this.comboBox_target.Items.AddRange(new object[] {
            "SAKURA-G Quick Start"});
            this.comboBox_target.Location = new System.Drawing.Point(83, 46);
            this.comboBox_target.Name = "comboBox_target";
            this.comboBox_target.Size = new System.Drawing.Size(163, 23);
            this.comboBox_target.TabIndex = 3;
            // 
            // label_interface
            // 
            this.label_interface.AutoSize = true;
            this.label_interface.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_interface.Location = new System.Drawing.Point(33, 20);
            this.label_interface.Name = "label_interface";
            this.label_interface.Size = new System.Drawing.Size(35, 15);
            this.label_interface.TabIndex = 0;
            this.label_interface.Text = "接口";
            // 
            // label_target
            // 
            this.label_target.AutoSize = true;
            this.label_target.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_target.Location = new System.Drawing.Point(5, 49);
            this.label_target.Name = "label_target";
            this.label_target.Size = new System.Drawing.Size(63, 15);
            this.label_target.TabIndex = 2;
            this.label_target.Text = "目标设备";
            // 
            // label_traces
            // 
            this.label_traces.AutoSize = true;
            this.label_traces.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_traces.Location = new System.Drawing.Point(290, 21);
            this.label_traces.Name = "label_traces";
            this.label_traces.Size = new System.Drawing.Size(63, 15);
            this.label_traces.TabIndex = 4;
            this.label_traces.Text = "波形总数";
            // 
            // textBox_traces
            // 
            this.textBox_traces.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_traces.Location = new System.Drawing.Point(366, 17);
            this.textBox_traces.Name = "textBox_traces";
            this.textBox_traces.Size = new System.Drawing.Size(75, 23);
            this.textBox_traces.TabIndex = 5;
            this.textBox_traces.Text = "2000";
            this.textBox_traces.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBox_traces.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_traces_KeyPress);
            // 
            // label_key
            // 
            this.label_key.AutoSize = true;
            this.label_key.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_key.Location = new System.Drawing.Point(33, 106);
            this.label_key.Name = "label_key";
            this.label_key.Size = new System.Drawing.Size(35, 15);
            this.label_key.TabIndex = 7;
            this.label_key.Text = "密钥";
            // 
            // label_plaintext
            // 
            this.label_plaintext.AutoSize = true;
            this.label_plaintext.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_plaintext.Location = new System.Drawing.Point(14, 165);
            this.label_plaintext.Name = "label_plaintext";
            this.label_plaintext.Size = new System.Drawing.Size(63, 15);
            this.label_plaintext.TabIndex = 11;
            this.label_plaintext.Text = "固定明文";
            // 
            // checkBox_endless
            // 
            this.checkBox_endless.AutoSize = true;
            this.checkBox_endless.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox_endless.Location = new System.Drawing.Point(453, 19);
            this.checkBox_endless.Name = "checkBox_endless";
            this.checkBox_endless.Size = new System.Drawing.Size(96, 19);
            this.checkBox_endless.TabIndex = 6;
            this.checkBox_endless.Text = "无休止采波";
            this.checkBox_endless.UseVisualStyleBackColor = true;
            // 
            // textBox_key
            // 
            this.textBox_key.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_key.Location = new System.Drawing.Point(83, 104);
            this.textBox_key.Name = "textBox_key";
            this.textBox_key.Size = new System.Drawing.Size(358, 23);
            this.textBox_key.TabIndex = 8;
            this.textBox_key.Text = "01 23 45 67 89 ab cd ef 12 34 56 78 9a bc de f0";
            this.textBox_key.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_key_KeyPress);
            this.textBox_key.Leave += new System.EventHandler(this.textBox_key_Leave);
            // 
            // button_changekey
            // 
            this.button_changekey.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_changekey.Location = new System.Drawing.Point(452, 104);
            this.button_changekey.Name = "button_changekey";
            this.button_changekey.Size = new System.Drawing.Size(46, 23);
            this.button_changekey.TabIndex = 9;
            this.button_changekey.Text = "变更";
            this.button_changekey.UseVisualStyleBackColor = true;
            this.button_changekey.Click += new System.EventHandler(this.button_changekey_Click);
            // 
            // textBox_plaintext
            // 
            this.textBox_plaintext.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_plaintext.Location = new System.Drawing.Point(83, 132);
            this.textBox_plaintext.Name = "textBox_plaintext";
            this.textBox_plaintext.Size = new System.Drawing.Size(358, 23);
            this.textBox_plaintext.TabIndex = 12;
            this.textBox_plaintext.Text = "01 23 45 67 89 AB CD EF FE DC BA 98 76 54 32 10";
            this.textBox_plaintext.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_plaintext_KeyPress);
            this.textBox_plaintext.Leave += new System.EventHandler(this.textBox_plaintext_Leave);
            // 
            // button_changeplaintext
            // 
            this.button_changeplaintext.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_changeplaintext.Location = new System.Drawing.Point(452, 132);
            this.button_changeplaintext.Name = "button_changeplaintext";
            this.button_changeplaintext.Size = new System.Drawing.Size(46, 23);
            this.button_changeplaintext.TabIndex = 13;
            this.button_changeplaintext.Text = "变更";
            this.button_changeplaintext.UseVisualStyleBackColor = true;
            this.button_changeplaintext.Click += new System.EventHandler(this.button_changeplaintext_Click);
            // 
            // checkBox_randomgeneration
            // 
            this.checkBox_randomgeneration.AutoSize = true;
            this.checkBox_randomgeneration.Checked = true;
            this.checkBox_randomgeneration.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_randomgeneration.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox_randomgeneration.Location = new System.Drawing.Point(504, 135);
            this.checkBox_randomgeneration.Name = "checkBox_randomgeneration";
            this.checkBox_randomgeneration.Size = new System.Drawing.Size(54, 19);
            this.checkBox_randomgeneration.TabIndex = 14;
            this.checkBox_randomgeneration.Text = "随机";
            this.checkBox_randomgeneration.UseVisualStyleBackColor = true;
            this.checkBox_randomgeneration.CheckedChanged += new System.EventHandler(this.checkBox_randomgeneration_CheckedChanged);
            // 
            // label_wait
            // 
            this.label_wait.AutoSize = true;
            this.label_wait.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_wait.Location = new System.Drawing.Point(5, 78);
            this.label_wait.Name = "label_wait";
            this.label_wait.Size = new System.Drawing.Size(63, 15);
            this.label_wait.TabIndex = 15;
            this.label_wait.Text = "加密间隔";
            // 
            // textBox_wait
            // 
            this.textBox_wait.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_wait.Location = new System.Drawing.Point(83, 75);
            this.textBox_wait.Name = "textBox_wait";
            this.textBox_wait.Size = new System.Drawing.Size(75, 23);
            this.textBox_wait.TabIndex = 16;
            this.textBox_wait.Text = "0";
            this.textBox_wait.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBox_wait.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_wait_KeyPress);
            // 
            // button_single
            // 
            this.button_single.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_single.Location = new System.Drawing.Point(364, 223);
            this.button_single.Name = "button_single";
            this.button_single.Size = new System.Drawing.Size(75, 23);
            this.button_single.TabIndex = 18;
            this.button_single.Text = "单条波形";
            this.button_single.UseVisualStyleBackColor = true;
            this.button_single.Click += new System.EventHandler(this.button_single_Click);
            // 
            // button_start
            // 
            this.button_start.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_start.Location = new System.Drawing.Point(176, 223);
            this.button_start.Name = "button_start";
            this.button_start.Size = new System.Drawing.Size(75, 23);
            this.button_start.TabIndex = 19;
            this.button_start.Text = "开始采波";
            this.button_start.UseVisualStyleBackColor = true;
            this.button_start.Click += new System.EventHandler(this.button_start_Click);
            // 
            // button_stop
            // 
            this.button_stop.Enabled = false;
            this.button_stop.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_stop.Location = new System.Drawing.Point(270, 223);
            this.button_stop.Name = "button_stop";
            this.button_stop.Size = new System.Drawing.Size(75, 23);
            this.button_stop.TabIndex = 20;
            this.button_stop.Text = "停止采波";
            this.button_stop.UseVisualStyleBackColor = true;
            this.button_stop.Click += new System.EventHandler(this.button_stop_Click);
            // 
            // checkBox_continueiferror
            // 
            this.checkBox_continueiferror.AutoSize = true;
            this.checkBox_continueiferror.Checked = true;
            this.checkBox_continueiferror.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_continueiferror.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox_continueiferror.Location = new System.Drawing.Point(453, 48);
            this.checkBox_continueiferror.Name = "checkBox_continueiferror";
            this.checkBox_continueiferror.Size = new System.Drawing.Size(96, 19);
            this.checkBox_continueiferror.TabIndex = 21;
            this.checkBox_continueiferror.Text = "错误时继续";
            this.checkBox_continueiferror.UseVisualStyleBackColor = true;
            // 
            // label_rtraces
            // 
            this.label_rtraces.AutoSize = true;
            this.label_rtraces.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_rtraces.Location = new System.Drawing.Point(809, 20);
            this.label_rtraces.Name = "label_rtraces";
            this.label_rtraces.Size = new System.Drawing.Size(63, 15);
            this.label_rtraces.TabIndex = 22;
            this.label_rtraces.Text = "波形编号";
            // 
            // textBox_rtraces
            // 
            this.textBox_rtraces.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_rtraces.Location = new System.Drawing.Point(885, 17);
            this.textBox_rtraces.Name = "textBox_rtraces";
            this.textBox_rtraces.ReadOnly = true;
            this.textBox_rtraces.Size = new System.Drawing.Size(80, 23);
            this.textBox_rtraces.TabIndex = 23;
            this.textBox_rtraces.Text = "0";
            this.textBox_rtraces.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label_rplaintext
            // 
            this.label_rplaintext.AutoSize = true;
            this.label_rplaintext.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_rplaintext.Location = new System.Drawing.Point(837, 95);
            this.label_rplaintext.Name = "label_rplaintext";
            this.label_rplaintext.Size = new System.Drawing.Size(35, 15);
            this.label_rplaintext.TabIndex = 27;
            this.label_rplaintext.Text = "明文";
            // 
            // label_rciphertext
            // 
            this.label_rciphertext.AutoSize = true;
            this.label_rciphertext.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_rciphertext.Location = new System.Drawing.Point(809, 120);
            this.label_rciphertext.Name = "label_rciphertext";
            this.label_rciphertext.Size = new System.Drawing.Size(63, 15);
            this.label_rciphertext.TabIndex = 29;
            this.label_rciphertext.Text = "FPGA密文";
            // 
            // label_ranswer
            // 
            this.label_ranswer.AutoSize = true;
            this.label_ranswer.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_ranswer.Location = new System.Drawing.Point(823, 145);
            this.label_ranswer.Name = "label_ranswer";
            this.label_ranswer.Size = new System.Drawing.Size(49, 15);
            this.label_ranswer.TabIndex = 31;
            this.label_ranswer.Text = "PC密文";
            // 
            // label_rdifference
            // 
            this.label_rdifference.AutoSize = true;
            this.label_rdifference.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_rdifference.Location = new System.Drawing.Point(809, 170);
            this.label_rdifference.Name = "label_rdifference";
            this.label_rdifference.Size = new System.Drawing.Size(63, 15);
            this.label_rdifference.TabIndex = 33;
            this.label_rdifference.Text = "二者异或";
            // 
            // label_relapsed
            // 
            this.label_relapsed.AutoSize = true;
            this.label_relapsed.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_relapsed.Location = new System.Drawing.Point(809, 45);
            this.label_relapsed.Name = "label_relapsed";
            this.label_relapsed.Size = new System.Drawing.Size(63, 15);
            this.label_relapsed.TabIndex = 24;
            this.label_relapsed.Text = "加密耗时";
            // 
            // textBox_rplaintext
            // 
            this.textBox_rplaintext.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_rplaintext.Location = new System.Drawing.Point(885, 92);
            this.textBox_rplaintext.Name = "textBox_rplaintext";
            this.textBox_rplaintext.ReadOnly = true;
            this.textBox_rplaintext.Size = new System.Drawing.Size(336, 23);
            this.textBox_rplaintext.TabIndex = 28;
            // 
            // textBox_rciphertext
            // 
            this.textBox_rciphertext.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_rciphertext.Location = new System.Drawing.Point(885, 117);
            this.textBox_rciphertext.Name = "textBox_rciphertext";
            this.textBox_rciphertext.ReadOnly = true;
            this.textBox_rciphertext.Size = new System.Drawing.Size(336, 23);
            this.textBox_rciphertext.TabIndex = 30;
            // 
            // textBox_ranswer
            // 
            this.textBox_ranswer.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_ranswer.Location = new System.Drawing.Point(885, 142);
            this.textBox_ranswer.Name = "textBox_ranswer";
            this.textBox_ranswer.ReadOnly = true;
            this.textBox_ranswer.Size = new System.Drawing.Size(336, 23);
            this.textBox_ranswer.TabIndex = 32;
            // 
            // textBox_rdifference
            // 
            this.textBox_rdifference.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_rdifference.Location = new System.Drawing.Point(885, 167);
            this.textBox_rdifference.Name = "textBox_rdifference";
            this.textBox_rdifference.ReadOnly = true;
            this.textBox_rdifference.Size = new System.Drawing.Size(336, 23);
            this.textBox_rdifference.TabIndex = 34;
            // 
            // textBox_AllTime
            // 
            this.textBox_AllTime.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_AllTime.Location = new System.Drawing.Point(885, 67);
            this.textBox_AllTime.Name = "textBox_AllTime";
            this.textBox_AllTime.ReadOnly = true;
            this.textBox_AllTime.Size = new System.Drawing.Size(80, 23);
            this.textBox_AllTime.TabIndex = 25;
            this.textBox_AllTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBox_s
            // 
            this.textBox_s.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_s.Location = new System.Drawing.Point(885, 42);
            this.textBox_s.Name = "textBox_s";
            this.textBox_s.ReadOnly = true;
            this.textBox_s.Size = new System.Drawing.Size(80, 23);
            this.textBox_s.TabIndex = 40;
            this.textBox_s.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label_waitms
            // 
            this.label_waitms.AutoSize = true;
            this.label_waitms.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_waitms.Location = new System.Drawing.Point(164, 77);
            this.label_waitms.Name = "label_waitms";
            this.label_waitms.Size = new System.Drawing.Size(35, 15);
            this.label_waitms.TabIndex = 17;
            this.label_waitms.Text = "毫秒";
            // 
            // label_relapsed_ms
            // 
            this.label_relapsed_ms.AutoSize = true;
            this.label_relapsed_ms.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_relapsed_ms.Location = new System.Drawing.Point(971, 45);
            this.label_relapsed_ms.Name = "label_relapsed_ms";
            this.label_relapsed_ms.Size = new System.Drawing.Size(35, 15);
            this.label_relapsed_ms.TabIndex = 26;
            this.label_relapsed_ms.Text = "毫秒";
            // 
            // textBox_interface
            // 
            this.textBox_interface.Enabled = false;
            this.textBox_interface.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_interface.Location = new System.Drawing.Point(83, 17);
            this.textBox_interface.Name = "textBox_interface";
            this.textBox_interface.Size = new System.Drawing.Size(75, 23);
            this.textBox_interface.TabIndex = 1;
            this.textBox_interface.Text = "USB0";
            this.textBox_interface.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // statusStrip_status
            // 
            this.statusStrip_status.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip_status.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar_progress,
            this.toolStripStatusLabel_message});
            this.statusStrip_status.Location = new System.Drawing.Point(0, 614);
            this.statusStrip_status.Name = "statusStrip_status";
            this.statusStrip_status.Size = new System.Drawing.Size(1233, 22);
            this.statusStrip_status.SizingGrip = false;
            this.statusStrip_status.TabIndex = 35;
            this.statusStrip_status.Text = "statusStrip1";
            // 
            // toolStripProgressBar_progress
            // 
            this.toolStripProgressBar_progress.MarqueeAnimationSpeed = 20;
            this.toolStripProgressBar_progress.Name = "toolStripProgressBar_progress";
            this.toolStripProgressBar_progress.Size = new System.Drawing.Size(50, 16);
            this.toolStripProgressBar_progress.Step = 1;
            // 
            // toolStripStatusLabel_message
            // 
            this.toolStripStatusLabel_message.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.toolStripStatusLabel_message.Name = "toolStripStatusLabel_message";
            this.toolStripStatusLabel_message.Size = new System.Drawing.Size(0, 17);
            // 
            // checkBox_randomgeneration_mask
            // 
            this.checkBox_randomgeneration_mask.AutoSize = true;
            this.checkBox_randomgeneration_mask.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox_randomgeneration_mask.Location = new System.Drawing.Point(504, 197);
            this.checkBox_randomgeneration_mask.Name = "checkBox_randomgeneration_mask";
            this.checkBox_randomgeneration_mask.Size = new System.Drawing.Size(54, 19);
            this.checkBox_randomgeneration_mask.TabIndex = 39;
            this.checkBox_randomgeneration_mask.Text = "随机";
            this.checkBox_randomgeneration_mask.UseVisualStyleBackColor = true;
            // 
            // button_changeplaintext_mask
            // 
            this.button_changeplaintext_mask.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_changeplaintext_mask.Location = new System.Drawing.Point(452, 194);
            this.button_changeplaintext_mask.Name = "button_changeplaintext_mask";
            this.button_changeplaintext_mask.Size = new System.Drawing.Size(46, 23);
            this.button_changeplaintext_mask.TabIndex = 38;
            this.button_changeplaintext_mask.Text = "变更";
            this.button_changeplaintext_mask.UseVisualStyleBackColor = true;
            this.button_changeplaintext_mask.Click += new System.EventHandler(this.button_changeplaintext_mask_Click);
            // 
            // textBox_plaintext_mask
            // 
            this.textBox_plaintext_mask.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_plaintext_mask.Location = new System.Drawing.Point(83, 194);
            this.textBox_plaintext_mask.Name = "textBox_plaintext_mask";
            this.textBox_plaintext_mask.Size = new System.Drawing.Size(358, 23);
            this.textBox_plaintext_mask.TabIndex = 37;
            this.textBox_plaintext_mask.Text = "00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00";
            this.textBox_plaintext_mask.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_plaintext_mask_KeyPress);
            this.textBox_plaintext_mask.Leave += new System.EventHandler(this.textBox_plaintext_mask_Leave);
            // 
            // label_plaintext_mask
            // 
            this.label_plaintext_mask.AutoSize = true;
            this.label_plaintext_mask.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_plaintext_mask.Location = new System.Drawing.Point(5, 196);
            this.label_plaintext_mask.Name = "label_plaintext_mask";
            this.label_plaintext_mask.Size = new System.Drawing.Size(63, 15);
            this.label_plaintext_mask.TabIndex = 36;
            this.label_plaintext_mask.Text = "明文掩码";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(971, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(21, 15);
            this.label1.TabIndex = 41;
            this.label1.Text = "秒";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(823, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 15);
            this.label2.TabIndex = 42;
            this.label2.Text = "总耗时";
            // 
            // checkBox_IsOtherScope
            // 
            this.checkBox_IsOtherScope.AutoSize = true;
            this.checkBox_IsOtherScope.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox_IsOtherScope.Location = new System.Drawing.Point(453, 77);
            this.checkBox_IsOtherScope.Name = "checkBox_IsOtherScope";
            this.checkBox_IsOtherScope.Size = new System.Drawing.Size(96, 19);
            this.checkBox_IsOtherScope.TabIndex = 46;
            this.checkBox_IsOtherScope.Text = "其它示波器";
            this.checkBox_IsOtherScope.UseVisualStyleBackColor = true;
            this.checkBox_IsOtherScope.CheckedChanged += new System.EventHandler(this.checkBox_IsOther_CheckedChanged);
            // 
            // label_buffer
            // 
            this.label_buffer.AutoSize = true;
            this.label_buffer.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_buffer.Location = new System.Drawing.Point(289, 49);
            this.label_buffer.Name = "label_buffer";
            this.label_buffer.Size = new System.Drawing.Size(63, 15);
            this.label_buffer.TabIndex = 47;
            this.label_buffer.Text = "采波点数";
            // 
            // textBox_buffer
            // 
            this.textBox_buffer.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_buffer.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textBox_buffer.Location = new System.Drawing.Point(366, 46);
            this.textBox_buffer.Name = "textBox_buffer";
            this.textBox_buffer.Size = new System.Drawing.Size(75, 23);
            this.textBox_buffer.TabIndex = 48;
            this.textBox_buffer.Text = "2500";
            this.textBox_buffer.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBox_buffer.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_buffer_KeyPress);
            this.textBox_buffer.Leave += new System.EventHandler(this.textBox_buffer_Leave);
            // 
            // textBox_ShowNum
            // 
            this.textBox_ShowNum.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_ShowNum.Location = new System.Drawing.Point(915, 194);
            this.textBox_ShowNum.Name = "textBox_ShowNum";
            this.textBox_ShowNum.Size = new System.Drawing.Size(75, 23);
            this.textBox_ShowNum.TabIndex = 44;
            this.textBox_ShowNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBox_ShowNum.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            // 
            // textBox_trigger
            // 
            this.textBox_trigger.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_trigger.Location = new System.Drawing.Point(366, 75);
            this.textBox_trigger.Name = "textBox_trigger";
            this.textBox_trigger.Size = new System.Drawing.Size(75, 23);
            this.textBox_trigger.TabIndex = 50;
            this.textBox_trigger.Text = "2100";
            this.textBox_trigger.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBox_trigger.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_trigger_KeyPress);
            this.textBox_trigger.Leave += new System.EventHandler(this.textBox_trigger_Leave);
            // 
            // label_trigger
            // 
            this.label_trigger.AutoSize = true;
            this.label_trigger.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_trigger.Location = new System.Drawing.Point(262, 77);
            this.label_trigger.Name = "label_trigger";
            this.label_trigger.Size = new System.Drawing.Size(91, 15);
            this.label_trigger.TabIndex = 51;
            this.label_trigger.Text = "触发前采点数";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Consolas", 8.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(1027, 194);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(68, 23);
            this.button1.TabIndex = 54;
            this.button1.Text = "查看波形";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label_wave
            // 
            this.label_wave.AutoSize = true;
            this.label_wave.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_wave.Location = new System.Drawing.Point(781, 197);
            this.label_wave.Name = "label_wave";
            this.label_wave.Size = new System.Drawing.Size(91, 15);
            this.label_wave.TabIndex = 55;
            this.label_wave.Text = "显示波形编号";
            // 
            // button_OpenPico
            // 
            this.button_OpenPico.Location = new System.Drawing.Point(82, 223);
            this.button_OpenPico.Name = "button_OpenPico";
            this.button_OpenPico.Size = new System.Drawing.Size(75, 23);
            this.button_OpenPico.TabIndex = 56;
            this.button_OpenPico.Text = "打开示波器";
            this.button_OpenPico.UseVisualStyleBackColor = true;
            this.button_OpenPico.Click += new System.EventHandler(this.button_OpenPico_Click);
            // 
            // checkBox_PicoState
            // 
            this.checkBox_PicoState.AutoSize = true;
            this.checkBox_PicoState.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.checkBox_PicoState.Enabled = false;
            this.checkBox_PicoState.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox_PicoState.Location = new System.Drawing.Point(452, 225);
            this.checkBox_PicoState.Name = "checkBox_PicoState";
            this.checkBox_PicoState.Size = new System.Drawing.Size(110, 19);
            this.checkBox_PicoState.TabIndex = 57;
            this.checkBox_PicoState.Text = "示波器已打开";
            this.checkBox_PicoState.UseVisualStyleBackColor = false;
            this.checkBox_PicoState.CheckedChanged += new System.EventHandler(this.checkBox_PicoState_CheckedChanged);
            // 
            // buttonNext
            // 
            this.buttonNext.Image = global::MathMagic.Properties.Resources.箭头2;
            this.buttonNext.Location = new System.Drawing.Point(996, 196);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(24, 19);
            this.buttonNext.TabIndex = 59;
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // buttonLast
            // 
            this.buttonLast.Image = global::MathMagic.Properties.Resources._3;
            this.buttonLast.Location = new System.Drawing.Point(885, 196);
            this.buttonLast.Name = "buttonLast";
            this.buttonLast.Size = new System.Drawing.Size(24, 19);
            this.buttonLast.TabIndex = 58;
            this.buttonLast.UseVisualStyleBackColor = true;
            this.buttonLast.Click += new System.EventHandler(this.buttonLast_Click);
            // 
            // checkBox_alternate
            // 
            this.checkBox_alternate.AutoSize = true;
            this.checkBox_alternate.Location = new System.Drawing.Point(553, 136);
            this.checkBox_alternate.Name = "checkBox_alternate";
            this.checkBox_alternate.Size = new System.Drawing.Size(48, 16);
            this.checkBox_alternate.TabIndex = 60;
            this.checkBox_alternate.Text = "FRFR";
            this.checkBox_alternate.UseVisualStyleBackColor = true;
            this.checkBox_alternate.CheckedChanged += new System.EventHandler(this.checkBox_alternate_CheckedChanged);
            // 
            // textBox_fixedplaintext
            // 
            this.textBox_fixedplaintext.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_fixedplaintext.Location = new System.Drawing.Point(83, 160);
            this.textBox_fixedplaintext.Name = "textBox_fixedplaintext";
            this.textBox_fixedplaintext.Size = new System.Drawing.Size(358, 23);
            this.textBox_fixedplaintext.TabIndex = 61;
            this.textBox_fixedplaintext.Text = "da 39 a3 ee 5e 6b 4b 0d 32 55 bf ef 95 60 18 90";
            this.textBox_fixedplaintext.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_fixedplaintext_KeyPress);
            this.textBox_fixedplaintext.Leave += new System.EventHandler(this.textBox_fixedplaintext_Leave);
            // 
            // checkBox_FRRF
            // 
            this.checkBox_FRRF.AutoSize = true;
            this.checkBox_FRRF.Location = new System.Drawing.Point(596, 136);
            this.checkBox_FRRF.Name = "checkBox_FRRF";
            this.checkBox_FRRF.Size = new System.Drawing.Size(48, 16);
            this.checkBox_FRRF.TabIndex = 62;
            this.checkBox_FRRF.Text = "FRRF";
            this.checkBox_FRRF.UseVisualStyleBackColor = true;
            this.checkBox_FRRF.CheckedChanged += new System.EventHandler(this.checkBox_FRRF_CheckedChanged);
            // 
            // checkBox_RF
            // 
            this.checkBox_RF.AutoSize = true;
            this.checkBox_RF.Location = new System.Drawing.Point(453, 158);
            this.checkBox_RF.Name = "checkBox_RF";
            this.checkBox_RF.Size = new System.Drawing.Size(48, 16);
            this.checkBox_RF.TabIndex = 63;
            this.checkBox_RF.Text = "任意";
            this.checkBox_RF.UseVisualStyleBackColor = true;
            this.checkBox_RF.CheckedChanged += new System.EventHandler(this.checkBox_RF_CheckedChanged);
            // 
            // checkBox_FFFF
            // 
            this.checkBox_FFFF.AutoSize = true;
            this.checkBox_FFFF.Location = new System.Drawing.Point(552, 158);
            this.checkBox_FFFF.Name = "checkBox_FFFF";
            this.checkBox_FFFF.Size = new System.Drawing.Size(60, 16);
            this.checkBox_FFFF.TabIndex = 64;
            this.checkBox_FFFF.Text = "FF\'F\'F";
            this.checkBox_FFFF.UseVisualStyleBackColor = true;
            this.checkBox_FFFF.CheckedChanged += new System.EventHandler(this.checkBox_FFFF_CheckedChanged);
            // 
            // checkBox_FF
            // 
            this.checkBox_FF.AutoSize = true;
            this.checkBox_FF.Location = new System.Drawing.Point(504, 158);
            this.checkBox_FF.Name = "checkBox_FF";
            this.checkBox_FF.Size = new System.Drawing.Size(42, 16);
            this.checkBox_FF.TabIndex = 65;
            this.checkBox_FF.Text = "FF\'";
            this.checkBox_FF.UseVisualStyleBackColor = true;
            this.checkBox_FF.CheckedChanged += new System.EventHandler(this.checkBox_FF_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(33, 139);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 15);
            this.label3.TabIndex = 66;
            this.label3.Text = "明文";
            // 
            // checkBox_classification4
            // 
            this.checkBox_classification4.AutoSize = true;
            this.checkBox_classification4.Location = new System.Drawing.Point(607, 158);
            this.checkBox_classification4.Name = "checkBox_classification4";
            this.checkBox_classification4.Size = new System.Drawing.Size(60, 16);
            this.checkBox_classification4.TabIndex = 64;
            this.checkBox_classification4.Text = "四分类";
            this.checkBox_classification4.UseVisualStyleBackColor = true;
            this.checkBox_classification4.CheckedChanged += new System.EventHandler(this.checkBox_classification4_CheckedChanged);
            // 
            // checkBox_classification8
            // 
            this.checkBox_classification8.AutoSize = true;
            this.checkBox_classification8.Location = new System.Drawing.Point(453, 175);
            this.checkBox_classification8.Name = "checkBox_classification8";
            this.checkBox_classification8.Size = new System.Drawing.Size(60, 16);
            this.checkBox_classification8.TabIndex = 64;
            this.checkBox_classification8.Text = "八分类";
            this.checkBox_classification8.UseVisualStyleBackColor = true;
            this.checkBox_classification8.CheckedChanged += new System.EventHandler(this.checkBox_classification8_CheckedChanged);
            // 
            // textBox_classificationN
            // 
            this.textBox_classificationN.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_classificationN.Location = new System.Drawing.Point(553, 175);
            this.textBox_classificationN.Name = "textBox_classificationN";
            this.textBox_classificationN.Size = new System.Drawing.Size(32, 23);
            this.textBox_classificationN.TabIndex = 67;
            this.textBox_classificationN.Text = "2";
            this.textBox_classificationN.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // checkBox_classificationN
            // 
            this.checkBox_classificationN.AutoSize = true;
            this.checkBox_classificationN.Location = new System.Drawing.Point(590, 179);
            this.checkBox_classificationN.Name = "checkBox_classificationN";
            this.checkBox_classificationN.Size = new System.Drawing.Size(54, 16);
            this.checkBox_classificationN.TabIndex = 68;
            this.checkBox_classificationN.Text = "n分类";
            this.checkBox_classificationN.UseVisualStyleBackColor = true;
            this.checkBox_classificationN.CheckedChanged += new System.EventHandler(this.checkBox_classificationN_CheckedChanged);
            // 
            // checkBox_PseudoRandom
            // 
            this.checkBox_PseudoRandom.AutoSize = true;
            this.checkBox_PseudoRandom.Location = new System.Drawing.Point(650, 135);
            this.checkBox_PseudoRandom.Name = "checkBox_PseudoRandom";
            this.checkBox_PseudoRandom.Size = new System.Drawing.Size(90, 16);
            this.checkBox_PseudoRandom.TabIndex = 69;
            this.checkBox_PseudoRandom.Text = "伪随机17825";
            this.checkBox_PseudoRandom.UseVisualStyleBackColor = true;
            this.checkBox_PseudoRandom.CheckedChanged += new System.EventHandler(this.checkBox_PseudoRandom_CheckedChanged);
            // 
            // chart1
            // 
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(17, 262);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(1216, 349);
            this.chart1.TabIndex = 70;
            this.chart1.Text = "chart1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1233, 636);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.checkBox_PseudoRandom);
            this.Controls.Add(this.checkBox_classificationN);
            this.Controls.Add(this.textBox_classificationN);
            this.Controls.Add(this.label_rdifference);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.checkBox_FF);
            this.Controls.Add(this.checkBox_classification8);
            this.Controls.Add(this.checkBox_classification4);
            this.Controls.Add(this.checkBox_FFFF);
            this.Controls.Add(this.checkBox_RF);
            this.Controls.Add(this.checkBox_FRRF);
            this.Controls.Add(this.textBox_fixedplaintext);
            this.Controls.Add(this.checkBox_alternate);
            this.Controls.Add(this.buttonNext);
            this.Controls.Add(this.buttonLast);
            this.Controls.Add(this.checkBox_PicoState);
            this.Controls.Add(this.button_OpenPico);
            this.Controls.Add(this.label_wave);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox_ShowNum);
            this.Controls.Add(this.label_trigger);
            this.Controls.Add(this.textBox_trigger);
            this.Controls.Add(this.textBox_buffer);
            this.Controls.Add(this.label_buffer);
            this.Controls.Add(this.checkBox_IsOtherScope);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkBox_randomgeneration_mask);
            this.Controls.Add(this.button_changeplaintext_mask);
            this.Controls.Add(this.textBox_plaintext_mask);
            this.Controls.Add(this.label_plaintext_mask);
            this.Controls.Add(this.statusStrip_status);
            this.Controls.Add(this.textBox_interface);
            this.Controls.Add(this.label_relapsed_ms);
            this.Controls.Add(this.label_waitms);
            this.Controls.Add(this.textBox_AllTime);
            this.Controls.Add(this.textBox_s);
            this.Controls.Add(this.textBox_rdifference);
            this.Controls.Add(this.textBox_ranswer);
            this.Controls.Add(this.textBox_rciphertext);
            this.Controls.Add(this.textBox_rplaintext);
            this.Controls.Add(this.label_relapsed);
            this.Controls.Add(this.label_ranswer);
            this.Controls.Add(this.label_rciphertext);
            this.Controls.Add(this.label_rplaintext);
            this.Controls.Add(this.textBox_rtraces);
            this.Controls.Add(this.label_rtraces);
            this.Controls.Add(this.checkBox_continueiferror);
            this.Controls.Add(this.button_stop);
            this.Controls.Add(this.button_start);
            this.Controls.Add(this.button_single);
            this.Controls.Add(this.textBox_wait);
            this.Controls.Add(this.label_wait);
            this.Controls.Add(this.checkBox_randomgeneration);
            this.Controls.Add(this.button_changeplaintext);
            this.Controls.Add(this.textBox_plaintext);
            this.Controls.Add(this.button_changekey);
            this.Controls.Add(this.textBox_key);
            this.Controls.Add(this.checkBox_endless);
            this.Controls.Add(this.label_plaintext);
            this.Controls.Add(this.label_key);
            this.Controls.Add(this.textBox_traces);
            this.Controls.Add(this.label_traces);
            this.Controls.Add(this.label_target);
            this.Controls.Add(this.label_interface);
            this.Controls.Add(this.comboBox_target);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "侧信道泄露评估";
            this.TransparencyKey = System.Drawing.Color.Gray;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.statusStrip_status.ResumeLayout(false);
            this.statusStrip_status.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox_target;
        private System.Windows.Forms.Label label_interface;
        private System.Windows.Forms.Label label_target;
        private System.Windows.Forms.Label label_traces;
        private System.Windows.Forms.TextBox textBox_traces;
        private System.Windows.Forms.Label label_key;
        private System.Windows.Forms.Label label_plaintext;
        private System.Windows.Forms.CheckBox checkBox_endless;
        private System.Windows.Forms.TextBox textBox_key;
        private System.Windows.Forms.Button button_changekey;
        private System.Windows.Forms.TextBox textBox_plaintext;
        private System.Windows.Forms.Button button_changeplaintext;
        private System.Windows.Forms.CheckBox checkBox_randomgeneration;
        private System.Windows.Forms.Label label_wait;
        private System.Windows.Forms.TextBox textBox_wait;
        private System.Windows.Forms.Button button_single;
        private System.Windows.Forms.Button button_start;
        private System.Windows.Forms.Button button_stop;
        private System.Windows.Forms.CheckBox checkBox_continueiferror;
        private System.Windows.Forms.Label label_rtraces;
        private System.Windows.Forms.TextBox textBox_rtraces;
        private System.Windows.Forms.Label label_rplaintext;
        private System.Windows.Forms.Label label_rciphertext;
        private System.Windows.Forms.Label label_ranswer;
        private System.Windows.Forms.Label label_rdifference;
        private System.Windows.Forms.Label label_relapsed;
        private System.Windows.Forms.TextBox textBox_rplaintext;
        private System.Windows.Forms.TextBox textBox_rciphertext;
        private System.Windows.Forms.TextBox textBox_ranswer;
        private System.Windows.Forms.TextBox textBox_rdifference;
        private System.Windows.Forms.TextBox textBox_AllTime;
        private System.Windows.Forms.TextBox textBox_s;
        private System.Windows.Forms.Label label_waitms;
        private System.Windows.Forms.Label label_relapsed_ms;
        private System.Windows.Forms.TextBox textBox_interface;
        private System.Windows.Forms.StatusStrip statusStrip_status;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar_progress;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_message;
        private System.Windows.Forms.CheckBox checkBox_randomgeneration_mask;
        private System.Windows.Forms.Button button_changeplaintext_mask;
        private System.Windows.Forms.TextBox textBox_plaintext_mask;
        private System.Windows.Forms.Label label_plaintext_mask;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_ShowNum;
        private System.Windows.Forms.CheckBox checkBox_IsOtherScope;
        private System.Windows.Forms.Label label_buffer;
        private System.Windows.Forms.TextBox textBox_buffer;
        private System.Windows.Forms.TextBox textBox_trigger;
        private System.Windows.Forms.Label label_trigger;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label_wave;
        private System.Windows.Forms.Button button_OpenPico;
        private System.Windows.Forms.CheckBox checkBox_PicoState;
        private System.Windows.Forms.Button buttonLast;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.CheckBox checkBox_alternate;
        private System.Windows.Forms.TextBox textBox_fixedplaintext;
        private System.Windows.Forms.CheckBox checkBox_FRRF;
        private System.Windows.Forms.CheckBox checkBox_RF;
        private System.Windows.Forms.CheckBox checkBox_FFFF;
        private System.Windows.Forms.CheckBox checkBox_FF;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBox_classification4;
        private System.Windows.Forms.CheckBox checkBox_classification8;
        private System.Windows.Forms.TextBox textBox_classificationN;
        private System.Windows.Forms.CheckBox checkBox_classificationN;
        private System.Windows.Forms.CheckBox checkBox_PseudoRandom;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
    }
}

