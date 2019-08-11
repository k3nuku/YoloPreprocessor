using System;
namespace YoloPreprocessor
{
    public class YOLOv3BoundingBox
    {
        public YOLOv3BoundingBox(int xCoordFromCenter, int yCoordFromCenter,
                                 int boxWidth, int boxHeight,
                                 int wholeWidth, int wholeHeight)
        {
            XCoordFromCenter = xCoordFromCenter;
            YCoordFromCenter = yCoordFromCenter;
            BoxWidth = boxWidth;
            BoxHeight = boxHeight;
            WholeWidth = wholeWidth;
            WholeHeight = wholeHeight;
        }

        public int XCoordFromCenter { get; set; }
        public int YCoordFromCenter { get; set; }
        public int BoxWidth { get; set; }
        public int BoxHeight { get; set; }
        public int WholeWidth { get; set; }
        public int WholeHeight { get; set; }
    }
}