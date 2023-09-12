using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV_ManipulationTool.Model
{
    public class ImageInfo
    {
        public Point LeftTop { get; set; }

        public Point Center { get; set; }

        public Point RightBottom { get; set; }

        public double Value { get; set; }
    }
}
