using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CV_ManipulationTool.Tool
{
    public class LogHelper
    {
        private static LogHelper instance;
        private static readonly object locker = new object();
        private TextBlock LogControl;

        private  LogHelper()
        {

        }

        public static LogHelper GetInstance()
        {
            if (instance == null)
            {
                lock (locker)
                {
                    if (instance == null)
                    {
                        instance = new LogHelper();
                    }
                }
            }
            return instance;
        }

        public void Reecord(string content)
        {
            string log = DateTime.Now.ToString("hh:mm:ff") + "\t" + content + "\n";
            LogControl.Text += log;
        }

        public void Register(TextBlock control)
        {
            LogControl = control;
        }
    }
}
