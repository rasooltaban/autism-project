using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.Kinect;

namespace UTKinectSkeletonMovementDetector
{
    /// <summary>   
    /// conversion logic for converting 3d skeleton points to 2d
    /// </summary>
    class SkeletonPointConverter
    {
        /// <summary>   
        /// simply converts a 3d point to display on 640 * 480 : 2d frame
        /// </summary>
        public static Point convert(JointPosition pos)
        {
            double z = pos.z;
            if (pos.z == 0)
            {
                return new Point(-1000, -1000);
            }
            int x = (int)(pos.x * 320 / (z * 0.543));
            x += 320;
            int y = (int)(pos.y * 240 / (z * 0.394));
            y += 240;
            y = 480 - y;
            return new Point(x, y);
        }

        /// <summary>   
        /// without this skeleton and color frames does not coinside completely
        /// </summary>
        public static Point convertAndScale(JointPosition pos, Point init_size, double ratio)
        {
            Point p = convert(pos);
            double newwidth = init_size.X * ratio;
            double newheight = init_size.Y * ratio;
            p.X = Convert.ToInt32(p.X * ratio + ((init_size.X - newwidth) / 2));
            p.Y = Convert.ToInt32(p.Y * ratio + (init_size.Y - newheight));
            return p;
        }
    }
}
