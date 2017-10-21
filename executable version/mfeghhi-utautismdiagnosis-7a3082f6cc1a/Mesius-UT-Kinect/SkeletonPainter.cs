using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.Drawing;

namespace UTKinectSkeletonMovementDetector
{
    class SkeletonPainter
    {


        /// <summary>   
        /// returns the the size of head cover square
        /// </summary>
        /// <param name="bg">frame to draw skeleton on</param>
        private static Point getHeadCoverSize(double headz)
        {
            Point result = new Point();
            result.Y = Convert.ToInt32(((5.0 - headz) * 60) / 2.63);
            result.X = Convert.ToInt32(result.Y / 1.3);
            return result;
        }

        /// <summary>   
        /// simply converts a 3d point to display on 640 * 480 : 2d frame
        /// </summary>
        /// <param name="bg">frame to draw skeleton on</param>
        /// <param name="skeleton">skeleton of the frame</param>
        /// <param name="initsize">skeleton frame initial size</param>
        /// <param name="proportion">ratio of skeleton fram scale</param>
        /// <param name="paintskeleton">determines whether or not all skeleton bones should be painted</param>       
        /// <param name="hidedead">whether or not the head should be hided with a black square or not</param>
        private static System.Drawing.Image paint(Image bg, MySkeleton skeleton, Point initsize, double proportion, Boolean paintskeleton, Boolean hidehead)
        {
            if (skeleton == null)
                return bg;
            Graphics g = Graphics.FromImage(bg);
            Pen bonePen = new Pen(System.Drawing.ColorTranslator.FromHtml("#008000"), 6);
            Pen jointPen = new Pen(System.Drawing.ColorTranslator.FromHtml("#44c044"), 4);
            Point head = SkeletonPointConverter.convertAndScale(skeleton.getJoint(JointType.Head.ToString()), initsize, proportion);
            Point shoulderCenter = SkeletonPointConverter.convertAndScale(skeleton.getJoint(JointType.ShoulderCenter.ToString()), initsize, proportion);
            Point shoulderLeft = SkeletonPointConverter.convertAndScale(skeleton.getJoint(JointType.ShoulderLeft.ToString()), initsize, proportion);
            Point shoulderRight = SkeletonPointConverter.convertAndScale(skeleton.getJoint(JointType.ShoulderRight.ToString()), initsize, proportion);
            Point ankleLeft = SkeletonPointConverter.convertAndScale(skeleton.getJoint(JointType.AnkleLeft.ToString()), initsize, proportion);

            Point ankleRight = SkeletonPointConverter.convertAndScale(skeleton.getJoint(JointType.AnkleRight.ToString()), initsize, proportion);
            Point elbowLeft = SkeletonPointConverter.convertAndScale(skeleton.getJoint(JointType.ElbowLeft.ToString()), initsize, proportion);
            Point elbowRight = SkeletonPointConverter.convertAndScale(skeleton.getJoint(JointType.ElbowRight.ToString()), initsize, proportion);
            Point footLeft = SkeletonPointConverter.convertAndScale(skeleton.getJoint(JointType.FootLeft.ToString()), initsize, proportion);
            Point footRight = SkeletonPointConverter.convertAndScale(skeleton.getJoint(JointType.FootRight.ToString()), initsize, proportion);

            Point handLeft = SkeletonPointConverter.convertAndScale(skeleton.getJoint(JointType.HandLeft.ToString()), initsize, proportion);
            Point handRight = SkeletonPointConverter.convertAndScale(skeleton.getJoint(JointType.HandRight.ToString()), initsize, proportion);
            Point hipCenter = SkeletonPointConverter.convertAndScale(skeleton.getJoint(JointType.HipCenter.ToString()), initsize, proportion);
            Point hipLeft = SkeletonPointConverter.convertAndScale(skeleton.getJoint(JointType.HipLeft.ToString()), initsize, proportion);
            Point hipRight = SkeletonPointConverter.convertAndScale(skeleton.getJoint(JointType.HipRight.ToString()), initsize, proportion);

            Point kneeLeft = SkeletonPointConverter.convertAndScale(skeleton.getJoint(JointType.KneeLeft.ToString()), initsize, proportion);
            Point kneeRight = SkeletonPointConverter.convertAndScale(skeleton.getJoint(JointType.KneeRight.ToString()), initsize, proportion);
            Point spine = SkeletonPointConverter.convertAndScale(skeleton.getJoint(JointType.Spine.ToString()), initsize, proportion);
            Point wristLeft = SkeletonPointConverter.convertAndScale(skeleton.getJoint(JointType.WristLeft.ToString()), initsize, proportion);
            Point wristRight = SkeletonPointConverter.convertAndScale(skeleton.getJoint(JointType.WristRight.ToString()), initsize, proportion);
            if (paintskeleton)
            {
                g.DrawLine(bonePen, head, shoulderCenter);
                g.DrawLine(bonePen, shoulderCenter, shoulderLeft);
                g.DrawLine(bonePen, shoulderCenter, shoulderRight);
                g.DrawLine(bonePen, shoulderCenter, spine);
                g.DrawLine(bonePen, spine, hipCenter);
                g.DrawLine(bonePen, hipCenter, hipLeft);
                g.DrawLine(bonePen, hipCenter, hipRight);

                g.DrawLine(bonePen, shoulderLeft, elbowLeft);
                g.DrawLine(bonePen, elbowLeft, wristLeft);
                g.DrawLine(bonePen, wristLeft, handLeft);

                g.DrawLine(bonePen, shoulderRight, elbowRight);
                g.DrawLine(bonePen, elbowRight, wristRight);
                g.DrawLine(bonePen, wristRight, handRight);

                g.DrawLine(bonePen, hipLeft, kneeLeft);
                g.DrawLine(bonePen, kneeLeft, ankleLeft);
                g.DrawLine(bonePen, ankleLeft, footLeft);

                g.DrawLine(bonePen, hipRight, kneeRight);
                g.DrawLine(bonePen, kneeRight, ankleRight);
                g.DrawLine(bonePen, ankleRight, footRight);

                g.DrawEllipse(jointPen, head.X - 2, head.Y - 2, 4, 4);
                g.DrawEllipse(jointPen, shoulderCenter.X - 2, shoulderCenter.Y - 2, 4, 4);
                g.DrawEllipse(jointPen, shoulderLeft.X - 2, shoulderLeft.Y - 2, 4, 4);
                g.DrawEllipse(jointPen, shoulderRight.X - 2, shoulderRight.Y - 2, 4, 4);
                g.DrawEllipse(jointPen, elbowLeft.X - 2, elbowLeft.Y - 2, 4, 4);
                g.DrawEllipse(jointPen, elbowRight.X - 2, elbowRight.Y - 2, 4, 4);
                g.DrawEllipse(jointPen, handLeft.X - 2, handLeft.Y - 2, 4, 4);
                g.DrawEllipse(jointPen, handRight.X - 2, handRight.Y - 2, 4, 4);
                g.DrawEllipse(jointPen, wristLeft.X - 2, wristLeft.Y - 2, 4, 4);
                g.DrawEllipse(jointPen, wristRight.X - 2, wristRight.Y - 2, 4, 4);
                g.DrawEllipse(jointPen, hipCenter.X - 2, hipCenter.Y - 2, 4, 4);
                g.DrawEllipse(jointPen, hipLeft.X - 2, hipLeft.Y - 2, 4, 4);
                g.DrawEllipse(jointPen, hipRight.X - 2, hipRight.Y - 2, 4, 4);
                g.DrawEllipse(jointPen, kneeLeft.X - 2, kneeLeft.Y - 2, 4, 4);
                g.DrawEllipse(jointPen, kneeRight.X - 2, kneeRight.Y - 2, 4, 4);
                g.DrawEllipse(jointPen, footLeft.X - 2, footLeft.Y - 2, 4, 4);
                g.DrawEllipse(jointPen, footRight.X - 2, footRight.Y - 2, 4, 4);
                g.DrawEllipse(jointPen, spine.X - 2, spine.Y - 2, 4, 4);
                g.DrawEllipse(jointPen, ankleLeft.X - 2, ankleLeft.Y - 2, 4, 4);
                g.DrawEllipse(jointPen, ankleRight.X - 2, ankleRight.Y - 2, 4, 4);
            }
            if (hidehead)
            {
                Point headCoverSize = getHeadCoverSize(skeleton.getJoint(JointType.Head.ToString()).z);
                int coverx = head.X - Convert.ToInt32(headCoverSize.X / 2);
                int covery = head.Y - Convert.ToInt32(headCoverSize.Y / 3);

                System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
                g.FillRectangle(myBrush, new Rectangle(coverx, covery, headCoverSize.X, headCoverSize.Y)); 
            }
            
            return bg;
        }

        /// <summary>   
        /// draw on a blank black frame
        /// </summary>
        /// <param name="skeleton">skeleton to draw</param>
        public static System.Drawing.Image paintonblank(MySkeleton skeleton)
        {
            return paint(new Bitmap(640,480), skeleton, new Point(640,480), 1, true, false);
        }

        /// <summary>   
        /// draw on an img
        /// </summary>
        /// <param name="img">background of the drawing</param>
        /// <param name="skeleton">skeleton to be drawn on the background</param>
        public static System.Drawing.Image paintSkeletonAndHideHead(Image img, MySkeleton skeleton)
        {
            return paint(img, skeleton, new Point(640, 480), (double)(271.0/300.0), true, true);
        }


        /// <summary>   
        /// draw on a background and also hide head
        /// </summary>
        /// <param name="img">background to draw skeleton on</param>
        /// <param name="skeleton">skeleton to be drawn on the background</param>
        public static System.Drawing.Image hideHead(Image img, MySkeleton skeleton)
        {
            return paint(img, skeleton, new Point(640, 480), (double)(271.0 / 300.0), false, true);
        }
    }
}
