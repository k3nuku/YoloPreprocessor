using System;
namespace YoloPreprocessor
{
    public class YOLOv3Object
    {
        public string Class { get; set; }
        public YOLOv3BoundingBox Box { get; set; }
    }

    public class YOLOv3Image
    {
        public YOLOv3Object[] Objects { get; set; }
    }
}