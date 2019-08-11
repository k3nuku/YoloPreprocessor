using System;
using System.Collections.Generic;
using System.IO;

namespace YoloPreprocessor.Annotations.ExDark
{
    /*
       *exdark
        The annotations are generated using Piotr's Computer Vision Matlab Toolbox (PMT)
        The format in each '.txt' is:
        (a) First 16 characters : Annotation tool data (unused)
        (b) 1st column : Object class name
        (c) 2nd-5th column : Bounding box coordinates [l t w h]
        l - pixel number from left of image
        t - pixel number from top of image
        w - width of bounding box
        h - height of bounding box
        (d) 6th-12th column : occlusion and orientation annotation (unused)
        (e) For images with more than 1 object annotated, (b)-(d) is repeated
     */
    public struct ExDarkObject
    {
        public string Class { get; set; }
        public ExDarkBoundingBox Box { get; set; }
    }

    public class ExDarkImage
    {
        public ExDarkImage(string imageBaseLocation, string imageAnnoLocation)
        {
            var di = new DirectoryInfo(imageAnnoLocation);
            ImageFile = String.Format("{0}/{1}/{2}",
                imageBaseLocation, di.Parent.Name,
                di.Name.Replace(di.Extension, ""));

            if (new FileInfo(ImageFile).Exists)
            {
                using (var exdarkStream = new StreamReader(imageAnnoLocation))
                {
                    var exdarkData = exdarkStream.ReadToEnd();
                    var lines = exdarkData.Split('\n');

                    var objLists = new List<ExDarkObject>();

                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (lines[i].Equals("\n")
                            || lines[i].Equals("")) continue;
                        else if (!i.Equals(0))
                        {
                            var objectInfo = lines[i].Split(' ');

                            objLists.Add(new ExDarkObject
                            {
                                Class = objectInfo[0],
                                Box = new ExDarkBoundingBox(
                                        Int32.Parse(objectInfo[1]),
                                        Int32.Parse(objectInfo[2]),
                                        Int32.Parse(objectInfo[3]),
                                        Int32.Parse(objectInfo[4]))
                            });
                        }
                        else ToolData = lines[i];
                    }

                    Objects = objLists.ToArray();
                }
            }
            else Console.WriteLine("No imagefile found about annotation {0}. skipping..",
                imageAnnoLocation);
        }

        public string ImageFile { get; set; }
        public string ToolData { get; set; }
        public ExDarkObject[] Objects { get; set; }
    }
}
