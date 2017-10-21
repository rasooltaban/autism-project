using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace UTKinectSkeletonMovementDetector
{
    class MyOverlayFrame
    {
        private long timetag;
        private MemoryStream overlayMemoryStream;
        private MemoryStream headhidedMemoryStream;


        public MyOverlayFrame(MySkeletonFrame skeletonframe, MyColorFrame colorframe)
        {
            this.timetag = colorframe.getTime();
            System.Drawing.Bitmap colorimg = (System.Drawing.Bitmap)System.Drawing.Image.FromStream(colorframe.getImageMemoryStream());
            System.Drawing.Image painted = SkeletonPainter.paintSkeletonAndHideHead(colorimg, skeletonframe.getSkeletons());
            overlayMemoryStream = new MemoryStream();
            painted.Save(overlayMemoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);

            System.Drawing.Bitmap colorimg2 = (System.Drawing.Bitmap)System.Drawing.Image.FromStream(colorframe.getImageMemoryStream());
            System.Drawing.Image painted2 = SkeletonPainter.hideHead(colorimg2, skeletonframe.getSkeletons());
            headhidedMemoryStream = new MemoryStream();
            painted2.Save(headhidedMemoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);

        }
 
        public MemoryStream getOverlayMemoryStream() { return overlayMemoryStream; }
        
        public MemoryStream getHeadHidedMemoryStream() { return headhidedMemoryStream; }

        public long getTime() { return this.timetag; }
    }
}
