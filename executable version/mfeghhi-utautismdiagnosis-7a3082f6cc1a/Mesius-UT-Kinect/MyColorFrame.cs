using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace UTKinectSkeletonMovementDetector
{
    public class MyColorFrame
    {
        private long timeTag;
        private MemoryStream colorImageMemoryStream;
        public MyColorFrame(long timeTag, Bitmap colorbmp)
        {
            this.timeTag = timeTag;
            colorImageMemoryStream = new MemoryStream();
            colorbmp.Save(colorImageMemoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            colorbmp.Dispose();
        }

        public MemoryStream getImageMemoryStream()
        {
            return colorImageMemoryStream;
        }

        public long getTime()
        {
            return timeTag;
        }
        public void writeOnDisk(String directoryPath)
        {
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
            colorImageMemoryStream.WriteTo(File.Create(directoryPath + "/" + timeTag + ".jpg"));
        }
    }
}
