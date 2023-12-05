using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MathMagic
{   
    static class Program
    {
        /// <summary>
        /// 这是程序的开始位置。
        /// </summary>
        [STAThread]
        static void Main()
        {                                    
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
