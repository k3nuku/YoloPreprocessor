using System;
namespace YoloPreprocessor.Annotations.YOLOv3
{
    public class YOLOv3BoundingBox
    {
        public YOLOv3BoundingBox(double xCoordCenter, double yCoordCenter,
                                 int boxWidth, int boxHeight,
                                 int wholeWidth, int wholeHeight)
        {
            XCoordCenter = xCoordCenter;
            YCoordCenter = yCoordCenter;
            BoxWidth = boxWidth;
            BoxHeight = boxHeight;
            WholeWidth = wholeWidth;
            WholeHeight = wholeHeight;
        }

        public double XCoordCenter { get; set; }
        public double YCoordCenter { get; set; }
        public int BoxWidth { get; set; }
        public int BoxHeight { get; set; }
        public int WholeWidth { get; set; }
        public int WholeHeight { get; set; }
    }
}