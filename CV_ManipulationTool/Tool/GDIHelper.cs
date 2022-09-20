using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CV_ManipulationTool.Tool
{
    public class GDIHelper
    {
        [DllImport("gdi32")]
        public static extern int DeleteObject(IntPtr o);
    }
}
