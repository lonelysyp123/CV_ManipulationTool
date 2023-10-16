using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace CV_ManipulationTool.Common
{
    public class Win32_Api_Helper
    {
        public static string[] GetCameraList()
        {
			try
			{
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE PNPClass = 'Camera'");
                List<string> CameraList = new List<string>();
                foreach (ManagementObject mo in searcher.Get())
                {
                    CameraList.Add(mo["Caption"].ToString());
                }
                return CameraList.ToArray();
            }
			catch (Exception)
			{
                return null;
			}
        }
    }
}
