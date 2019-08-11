using System;
namespace YoloPreprocessor.Annotations.YOLOv3
{
    public class YOLOv3Object
    {
        public string Class { get; set; }
        public YOLOv3BoundingBox Box { get; set; }
    }

    public class YOLOv3Image
    {
        public YOLOv3Image(string imgFile, YOLOv3Object[] objects)
        {
            ImageFile = imgFile;
            Objects = objects;
        }

        public string ImageFile { get; set; }
        public YOLOv3Object[] Objects { get; set; }
    }
}