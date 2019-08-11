using System;
using System.IO;
using System.Collections.Generic;
using YoloPreprocessor.Annotations.ExDark;
using YoloPreprocessor.Annotations.YOLOv3;
using Cairo;
using System.Linq;

namespace YoloPreprocessor
{
    class Program
    {
        static ExDarkImage[] GetExDarkImages(
            string srcFolder, string srcAnnoFolder)
        {
            var a = new List<ExDarkImage>();
            var classes = Directory.GetDirectories(srcAnnoFolder);

            Console.Write("Loading ExDark dataset to the memory... ");

            using (var pb = new ProgressBar())
            {
                for (var i = 0; i < classes.Length; i++) // cls in sfolder
                {
                    var b = Directory.GetFiles(classes[i]);

                    foreach (var c in b) // file in cls
                    {
                        var exDarkImg = new ExDarkImage(srcFolder, c);

                        if (exDarkImg.Objects != null)
                            a.Add(exDarkImg);
                    }

                    pb.Report(i / classes.Length * 100);
                }
            }

            Console.WriteLine("Done. {0} of images loaded.", a.Count);

            return a.ToArray();
        }

        static YOLOv3Image[] ConvertExDarkToYolov3(ExDarkImage[] exdarkImages)
        {
            List<YOLOv3Image> yOLOv3Images = new List<YOLOv3Image>();

            Console.Write("Converting In-memory ExDark dataset to YoloV3... ");

            using(var pb = new ProgressBar())
            {
                for (int i = 0; i < exdarkImages.Length; i++) // 이미지 파일
                {
                    var image = new List<YOLOv3Object>();
                    var filename = new FileInfo(exdarkImages[i].ImageFile).Name;
                    int wholeWidth, wholeHeight;

                    using (ImageSurface sf = new ImageSurface(exdarkImages[i].ImageFile))
                    {
                        wholeWidth = sf.Width;
                        wholeHeight = sf.Height;

                        if (wholeWidth == 0 || wholeHeight == 0)
                        {
                            //Console.WriteLine("file {0} cannot be loaded properly, skipping...",
                            //    exdarkImages[i].ImageFile);
                            continue;
                        }
                        //else Console.WriteLine("file {0} ({1}x{2})",
                        //  exdarkImages[i].ImageFile, wholeWidth, wholeHeight);
                    }

                    foreach (var c in exdarkImages[i].Objects)
                        image.Add(new YOLOv3Object()
                        {
                            Class = c.Class,
                            Box = new YOLOv3BoundingBox
                            (
                                Math.Round((double)(c.Box.PixelLeft + (c.Box.BoxWidth / 2)) / wholeWidth, 6),
                                Math.Round((double)(c.Box.PixelTop + (c.Box.BoxHeight / 2)) / wholeHeight, 6),
                                c.Box.BoxWidth,
                                c.Box.BoxHeight,
                                wholeWidth,
                                wholeHeight
                            )
                        });

                    yOLOv3Images.Add(new YOLOv3Image(filename, image.ToArray()));

                    pb.Report((double)i / exdarkImages.Length);
                }
            }

            Console.WriteLine("Done. {0} images converted.", yOLOv3Images.Count);

            return yOLOv3Images.ToArray();
        }

        static void WriteYolov3ObjectToFile(YOLOv3Image[] imgs, string dstFolder, string srcFolder)
        {
            // 필요한 파일
            // /images/img.jpg v
            // /labels/img.txt v
            // /set_training_exdarkN.txt
            // /set_validation_exdarkN.txt
            // /exdark.names
            // /data/coco.data

            // 루틴 개선 필요
            var classes = new List<string>();

            Console.Write("Loading image classes... ");
            using (var pg = new ProgressBar())
                for (int i = 0; i < imgs.Length; i++)
                {
                    foreach (var obj in imgs[i].Objects)
                        classes.Add(obj.Class);

                    pg.Report(i / imgs.Length * 100);
                }

            classes = classes.Distinct().ToList();
            Console.WriteLine("{0} of classes found.", classes.Count);

            Directory.CreateDirectory(string.Format("{0}", dstFolder));

            using (StreamWriter swClasses = new StreamWriter(
                string.Format("{0}/{1}", dstFolder, "exdark.names")))
            {
                foreach (var cls in classes)
                    swClasses.WriteLine(cls);
            }

            Console.Write("Saving yolov3 classes to the disk... ");
            using (var pg = new ProgressBar())
            {
                for (var i = 0; i < imgs.Length; i++)
                {
                    var imgFileName = string.Format("/images/exdark/exdark_{0}", imgs[i].ImageFile);
                    var annoFileName = string.Format("/labels/exdark/exdark_{0}.{1}",
                        imgs[i].ImageFile.Replace(new FileInfo(imgs[i].ImageFile).Extension, "")
                        , "txt");

                    Directory.CreateDirectory(string.Format("{0}/{1}", dstFolder, "images/exdark"));
                    Directory.CreateDirectory(string.Format("{0}/{1}", dstFolder, "labels/exdark"));

                    string foundImgFile = Directory.GetFiles(srcFolder, imgs[i].ImageFile, SearchOption.AllDirectories)[0];
                    File.Copy(foundImgFile, string.Format("{0}/{1}", dstFolder, imgFileName)); // image file copy

                    using (StreamWriter swAnno = new StreamWriter(
                        string.Format("{0}/{1}", dstFolder, annoFileName))) // write annotation
                    {
                        foreach (var obj in imgs[i].Objects)
                        {
                            var clsNo = classes.IndexOf(obj.Class);
                            var xCenter = obj.Box.XCoordCenter;
                            var yCenter = obj.Box.YCoordCenter;
                            var width = obj.Box.BoxWidth;
                            var height = obj.Box.BoxHeight;

                            swAnno.WriteLine("{0} {1} {2} {3} {4}",
                                clsNo, xCenter, yCenter, width, height);
                        }
                    }

                    pg.Report(i / imgs.Length * 100);
                }
            }

            Console.WriteLine("Done.");
        }

        static void Main(string[] args) // args0: src_img, 1: src_anno, 2: targetfolder
        {
            if (args.Length < 3)
                Console.WriteLine("Usage: ./yolopreprocessor [exdark sourceimage folder] [exdark source annotation folder], [output folder]");

            Console.WriteLine("hello, world!");

            var imagesExDark = GetExDarkImages(args[0], args[1]);
            var imagesYOLOv3 = ConvertExDarkToYolov3(imagesExDark);

            WriteYolov3ObjectToFile(imagesYOLOv3, args[2], args[0]);
            Console.WriteLine("batch completed.");
        }
    }
}
