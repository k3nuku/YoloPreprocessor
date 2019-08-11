using System;
namespace YoloPreprocessor.Annotations.ExDark
{
    /*
     *  2nd-5th column : Bounding box coordinates [l t w h]
        l - pixel number from left of image
        t - pixel number from top of image
        w - width of bounding box
        h - height of bounding box
     */
    public class ExDarkBoundingBox
    {
        public ExDarkBoundingBox(int fromLeft, int fromTop, int boxWidth, int boxHeight)
        {
            PixelLeft = fromLeft;
            PixelTop = fromTop;
            BoxWidth = boxWidth;
            BoxHeight = boxHeight;
        }

        public int PixelLeft { get; set; }
        public int PixelTop { get; set; }
        public int BoxWidth { get; set; }
        public int BoxHeight { get; set; }
    }
}
