using DlibDotNet;
using System;
using System.Collections.Generic;
using FaceRecognitionSharp.Properties;
using System.IO;

namespace FaceRecognitionSharp
{
    public class FaceRecognition
    {
        private static FrontalFaceDetector frontalFaceDetector = Dlib.GetFrontalFaceDetector();
        private static ShapePredictor shapePredictor = ShapePredictor.Deserialize(Resources.shapepredictor);
        private static ShapePredictor landMarksPredictor = ShapePredictor.Deserialize(Resources.landmarks);
        private static DlibDotNet.Dnn.LossMetric lossMetric = DlibDotNet.Dnn.LossMetric.Deserialize(Resources.lossmetric);
        private static ProtoRandom.ProtoRandom random = new ProtoRandom.ProtoRandom(5);
        private static char[] _characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
        private static string tempDir = "";
        private static bool initialized = false;

        private static string GenerateRandomFileName()
        {
            return GetTempDirectory() + random.GetRandomString(_characters, random.GetRandomInt32(8, 28)) + ".png";
        }

        private static string GetTempDirectory()
        {
            if (tempDir == "")
            {
                string folderName = random.GetRandomString(_characters, random.GetRandomInt32(16, 32));

                if (!System.IO.Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.System).Substring(0, 1) + ":\\Temp"))
                {
                    System.IO.Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.System).Substring(0, 1) + ":\\Temp");
                }

                if (!System.IO.Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.System).Substring(0, 1) + ":\\Temp\\" + folderName))
                {
                    System.IO.Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.System).Substring(0, 1) + ":\\Temp\\" + folderName);
                }

                tempDir = Environment.GetFolderPath(Environment.SpecialFolder.System).Substring(0, 1) + ":\\Temp\\" + folderName + "\\";
            }

            return tempDir;
        }

        public static void ExtractFace(string inputImagePath, string outputImagePath)
        {
            Array2D<RgbPixel> sourceImage = Dlib.LoadImage<RgbPixel>(inputImagePath);
            Rectangle face = frontalFaceDetector.Operator(sourceImage)[0];
            Dlib.SaveJpeg(Dlib.ExtractImage4Points(sourceImage, new DPoint[]
            {
            new DPoint(face.TopLeft.X, face.TopLeft.Y),
                new DPoint(face.TopRight.X, face.TopRight.Y),
                new DPoint(face.BottomLeft.X, face.BottomLeft.Y),
            new DPoint(face.BottomRight.X, face.BottomRight.Y),
            },
            (int)face.Width, (int)face.Height), outputImagePath);
        }

        public static System.Drawing.Image ExtractFace(string inputImagePath)
        {
            string fileName = GenerateRandomFileName();
            ExtractFace(inputImagePath, fileName);
            System.Drawing.Image outputImage = System.Drawing.Image.FromFile(fileName);
            return outputImage;
        }

        public static void ExtractFace(System.Drawing.Image sourceImage, string outputImagePath)
        {
            string fileName = GenerateRandomFileName();
            sourceImage.Save(fileName);
            Array2D<RgbPixel> theImage = Dlib.LoadImage<RgbPixel>(fileName);
            Rectangle face = frontalFaceDetector.Operator(theImage)[0];
            Dlib.SaveJpeg(Dlib.ExtractImage4Points(theImage, new DPoint[]
            {
            new DPoint(face.TopLeft.X, face.TopLeft.Y),
            new DPoint(face.TopRight.X, face.TopRight.Y),
            new DPoint(face.BottomLeft.X, face.BottomLeft.Y),
            new DPoint(face.BottomRight.X, face.BottomRight.Y),
            },
            (int)face.Width, (int)face.Height), outputImagePath);
        }

        public static System.Drawing.Image ExtractFace(System.Drawing.Image sourceImage)
        {
            string fileName = GenerateRandomFileName();
            ExtractFace(sourceImage, fileName);
            System.Drawing.Image outputImage = System.Drawing.Image.FromFile(fileName);
            return outputImage;
        }

        public static void DrawRectsToFaces(string inputImagePath, string outputImagePath, RgbPixel rectColor, int borderThickness)
        {
            Array2D<RgbPixel> img = Dlib.LoadImage<RgbPixel>(inputImagePath);
            Rectangle[] faces = frontalFaceDetector.Operator(img);

            foreach (Rectangle face in faces)
            {
                Dlib.DrawRectangle(img, face, color: rectColor, thickness: (uint)borderThickness);
            }

            Dlib.SaveJpeg(img, outputImagePath);
        }

        public static void DrawRectsToFaces(System.Drawing.Image sourceImage, string outputImagePath, RgbPixel rectColor, int borderThickness)
        {
            string fileName = GenerateRandomFileName();
            sourceImage.Save(fileName);
            Array2D<RgbPixel> img = Dlib.LoadImage<RgbPixel>(fileName);
            Rectangle[] faces = frontalFaceDetector.Operator(img);

            foreach (Rectangle face in faces)
            {
                Dlib.DrawRectangle(img, face, color: rectColor, thickness: (uint)borderThickness);
            }

            Dlib.SaveJpeg(img, outputImagePath);
        }

        public static System.Drawing.Image DrawRectsToFaces(string inputImagePath, RgbPixel rectColor, int borderThickness)
        {
            string fileName = GenerateRandomFileName();
            DrawRectsToFaces(inputImagePath, fileName, rectColor, borderThickness);
            System.Drawing.Image outputImage = System.Drawing.Image.FromFile(fileName);
            return outputImage;
        }

        public static System.Drawing.Image DrawRectsToFaces(System.Drawing.Image sourceImage, RgbPixel rectColor, int borderThickness)
        {
            string fileName = GenerateRandomFileName();
            DrawRectsToFaces(sourceImage, fileName, rectColor, borderThickness);
            System.Drawing.Image outputImage = System.Drawing.Image.FromFile(fileName);
            return outputImage;
        }

        public static void DrawLandmarksToFaces(string inputImagePath, string outputImagePath, RgbPixel rectColor, int borderThickness)
        {
            Array2D<RgbPixel> img = Dlib.LoadImage<RgbPixel>(inputImagePath);
            Rectangle[] faces = frontalFaceDetector.Operator(img);

            foreach (Rectangle face in faces)
            {
                FullObjectDetection shape = landMarksPredictor.Detect(img, face);

                for (int i = 0; i < shape.Parts; i++)
                {
                    Point point = shape.GetPart((uint) i);
                    Rectangle rect = new Rectangle(point);
                    Dlib.DrawRectangle(img, rect, rectColor, (uint) borderThickness);
                }
            }

            Dlib.SaveJpeg(img, outputImagePath);
        }

        public static void DrawLandmarksToFaces(System.Drawing.Image sourceImage, string outputImagePath, RgbPixel rectColor, int borderThickness)
        {
            string fileName = GenerateRandomFileName();
            sourceImage.Save(fileName);
            Array2D<RgbPixel> img = Dlib.LoadImage<RgbPixel>(fileName);
            Rectangle[] faces = frontalFaceDetector.Operator(img);

            foreach (Rectangle face in faces)
            {
                FullObjectDetection shape = landMarksPredictor.Detect(img, face);

                for (int i = 0; i < shape.Parts; i++)
                {
                    Point point = shape.GetPart((uint)i);
                    Rectangle rect = new Rectangle(point);
                    Dlib.DrawRectangle(img, rect, rectColor, (uint)borderThickness);
                }
            }

            Dlib.SaveJpeg(img, outputImagePath);
        }

        public static System.Drawing.Image DrawLandmarksToFaces(string inputImagePath, RgbPixel rectColor, int borderThickness)
        {
            string fileName = GenerateRandomFileName();
            DrawLandmarksToFaces(inputImagePath, fileName, rectColor, borderThickness);
            System.Drawing.Image outputImage = System.Drawing.Image.FromFile(fileName);
            return outputImage;
        }

        public static System.Drawing.Image DrawLandmarksToFaces(System.Drawing.Image sourceImage, RgbPixel rectColor, int borderThickness)
        {
            string fileName = GenerateRandomFileName();
            DrawLandmarksToFaces(sourceImage, fileName, rectColor, borderThickness);
            System.Drawing.Image outputImage = System.Drawing.Image.FromFile(fileName);
            return outputImage;
        }

        public static void DrawRectsAndLandmarksToFaces(string inputImagePath, string outputImagePath, RgbPixel faceRectColor, int faceRectBorderThickness, RgbPixel faceLandmarkColor, int faceLandmarkBorderThickness)
        {
            Array2D<RgbPixel> img = Dlib.LoadImage<RgbPixel>(inputImagePath);
            Rectangle[] faces = frontalFaceDetector.Operator(img);

            foreach (Rectangle face in faces)
            {
                FullObjectDetection shape = landMarksPredictor.Detect(img, face);

                for (int i = 0; i < shape.Parts; i++)
                {
                    Point point = shape.GetPart((uint)i);
                    Rectangle rect = new Rectangle(point);
                    Dlib.DrawRectangle(img, rect, faceLandmarkColor, (uint)faceLandmarkBorderThickness);
                }

                Dlib.DrawRectangle(img, face, color: faceRectColor, thickness: (uint)faceRectBorderThickness);
            }

            Dlib.SaveJpeg(img, outputImagePath);
        }

        public static void DrawRectsAndLandmarksToFaces(System.Drawing.Image sourceImage, string outputImagePath, RgbPixel faceRectColor, int faceRectBorderThickness, RgbPixel faceLandmarkColor, int faceLandmarkBorderThickness)
        {
            string fileName = GenerateRandomFileName();
            sourceImage.Save(fileName);
            Array2D<RgbPixel> img = Dlib.LoadImage<RgbPixel>(fileName);
            Rectangle[] faces = frontalFaceDetector.Operator(img);

            foreach (Rectangle face in faces)
            {
                FullObjectDetection shape = landMarksPredictor.Detect(img, face);

                for (int i = 0; i < shape.Parts; i++)
                {
                    Point point = shape.GetPart((uint)i);
                    Rectangle rect = new Rectangle(point);
                    Dlib.DrawRectangle(img, rect, faceLandmarkColor, (uint)faceLandmarkBorderThickness);
                }

                Dlib.DrawRectangle(img, face, color: faceRectColor, thickness: (uint)faceRectBorderThickness);
            }

            Dlib.SaveJpeg(img, outputImagePath);
        }

        public static System.Drawing.Image DrawRectsAndLandmarksToFaces(string inputImagePath, RgbPixel faceRectColor, int faceRectBorderThickness, RgbPixel faceLandmarkColor, int faceLandmarkBorderThickness)
        {
            string fileName = GenerateRandomFileName();
            DrawRectsAndLandmarksToFaces(inputImagePath, fileName, faceRectColor, faceRectBorderThickness, faceLandmarkColor, faceLandmarkBorderThickness);
            System.Drawing.Image outputImage = System.Drawing.Image.FromFile(fileName);
            return outputImage;
        }

        public static System.Drawing.Image DrawRectsAndLandmarksToFaces(System.Drawing.Image sourceImage, RgbPixel faceRectColor, int faceRectBorderThickness, RgbPixel faceLandmarkColor, int faceLandmarkBorderThickness)
        {
            string fileName = GenerateRandomFileName();
            DrawRectsAndLandmarksToFaces(sourceImage, fileName, faceRectColor, faceRectBorderThickness, faceLandmarkColor, faceLandmarkBorderThickness);
            System.Drawing.Image outputImage = System.Drawing.Image.FromFile(fileName);
            return outputImage;
        }

        public static bool IsFacePresent(string inputImagePath)
        {
            try
            {
                Array2D<RgbPixel> img = Dlib.LoadImage<RgbPixel>(inputImagePath);
                Rectangle[] faces = frontalFaceDetector.Operator(img);
                return faces.Length > 0;
            }
            catch
            {
                return false;
            }
        }

        public static uint GetFacesCount(string inputImagePath)
        {
            try
            {
                Array2D<RgbPixel> img = Dlib.LoadImage<RgbPixel>(inputImagePath);
                Rectangle[] faces = frontalFaceDetector.Operator(img);
                return (uint)faces.Length;
            }
            catch
            {
                return 0;
            }
        }

        public static bool IsFacePresent(System.Drawing.Image sourceImage)
        {
            string fileName = GenerateRandomFileName();
            sourceImage.Save(fileName);
            bool isPresent = IsFacePresent(fileName);
            return isPresent;
        }

        public static uint GetFacesCount(System.Drawing.Image sourceImage)
        {
            string fileName = GenerateRandomFileName();
            sourceImage.Save(fileName);
            uint facesCount = GetFacesCount(fileName);
            return facesCount;
        }

        public static System.Drawing.Image[] ExtractFaces(string inputImagePath)
        {
            List<System.Drawing.Image> outputImages = new List<System.Drawing.Image>();
            Array2D<RgbPixel> sourceImage = Dlib.LoadImage<RgbPixel>(inputImagePath);
            Rectangle[] faces = frontalFaceDetector.Operator(sourceImage);

            foreach (Rectangle face in faces)
            {
                string fileName = GenerateRandomFileName();

                Dlib.SaveJpeg(Dlib.ExtractImage4Points(sourceImage, new DPoint[]
                {
                new DPoint(face.TopLeft.X, face.TopLeft.Y),
                new DPoint(face.TopRight.X, face.TopRight.Y),
                    new DPoint(face.BottomLeft.X, face.BottomLeft.Y),
                new DPoint(face.BottomRight.X, face.BottomRight.Y),
                },
                (int)face.Width, (int)face.Height), fileName);

                outputImages.Add(System.Drawing.Image.FromFile(fileName));
            }

            return outputImages.ToArray();
        }

        public static System.Drawing.Image[] ExtractFaces(System.Drawing.Image sourceImage)
        {
            string fileName = GenerateRandomFileName();
            sourceImage.Save(fileName);
            System.Drawing.Image[] images = ExtractFaces(fileName);
            return images;
        }

        public static void Initialize()
        {
            if (initialized)
            {
                return;
            }

            string path = GetTempDirectory() + random.GetRandomString(_characters, random.GetRandomInt32(32, 64)) + ".png";
            Resources.faces.Save(path);
            Array2D<RgbPixel> img = Dlib.LoadImage<RgbPixel>(path);
            List<Matrix<RgbPixel>> chips = new List<Matrix<RgbPixel>>();

            foreach (Rectangle face in frontalFaceDetector.Operator(img))
            {
                FullObjectDetection shape = shapePredictor.Detect(img, face);
                FullObjectDetection otherShape = landMarksPredictor.Detect(img, face);
                ChipDetails faceChipDetail = Dlib.GetFaceChipDetails(shape, 150, 0.25);
                Array2D<RgbPixel> faceChip = Dlib.ExtractImageChip<RgbPixel>(img, faceChipDetail);
                Matrix<RgbPixel> matrix = new Matrix<RgbPixel>(faceChip);
                chips.Add(matrix);
            }

            DlibDotNet.Dnn.OutputLabels<Matrix<float>> descriptors = lossMetric.Operator(chips);
            List<SamplePair> edges = new List<SamplePair>();

            for (uint i = 0; i < descriptors.Count; ++i)
            {
                for (var j = i; j < descriptors.Count; ++j)
                {
                    if (Dlib.Length(descriptors[i] - descriptors[j]) < 0.6)
                    {
                        edges.Add(new SamplePair(i, j));
                    }
                }
            }

            uint clusters = 0;
            uint[] labels = new uint[] { };
            Dlib.ChineseWhispers(edges, 100, out clusters, out labels);
            initialized = true;
        }

        public static bool CompareFaces(params string[] paths)
        {
            try
            {
                List<Matrix<RgbPixel>> chips = new List<Matrix<RgbPixel>>();

                foreach (string path in paths)
                {
                    Array2D<RgbPixel> img = Dlib.LoadImage<RgbPixel>(path);

                    foreach (Rectangle face in frontalFaceDetector.Operator(img))
                    {
                        FullObjectDetection shape = shapePredictor.Detect(img, face);
                        ChipDetails faceChipDetail = Dlib.GetFaceChipDetails(shape, 150, 0.25);
                        Array2D<RgbPixel> faceChip = Dlib.ExtractImageChip<RgbPixel>(img, faceChipDetail);
                        Matrix<RgbPixel> matrix = new Matrix<RgbPixel>(faceChip);
                        chips.Add(matrix);
                    }
                }

                DlibDotNet.Dnn.OutputLabels<Matrix<float>> descriptors = lossMetric.Operator(chips);
                List<SamplePair> edges = new List<SamplePair>();

                for (uint i = 0; i < descriptors.Count; ++i)
                {
                    for (var j = i; j < descriptors.Count; ++j)
                    {
                        if (Dlib.Length(descriptors[i] - descriptors[j]) < 0.6)
                        {
                            edges.Add(new SamplePair(i, j));
                        }
                    }
                }

                uint clusters = 0;
                uint[] labels = new uint[] { };
                Dlib.ChineseWhispers(edges, 100, out clusters, out labels);
                return clusters == 1;
            }
            catch
            {
                return false;
            }
        }

        public static float FaceDistance(string facePath1, string facePath2)
        {
            try
            {
                List<Matrix<RgbPixel>> chips = new List<Matrix<RgbPixel>>();

                {
                    Array2D<RgbPixel> img = Dlib.LoadImage<RgbPixel>(facePath1);

                    foreach (Rectangle face in frontalFaceDetector.Operator(img))
                    {
                        FullObjectDetection shape = shapePredictor.Detect(img, face);
                        ChipDetails faceChipDetail = Dlib.GetFaceChipDetails(shape, 150, 0.25);
                        Array2D<RgbPixel> faceChip = Dlib.ExtractImageChip<RgbPixel>(img, faceChipDetail);
                        Matrix<RgbPixel> matrix = new Matrix<RgbPixel>(faceChip);
                        chips.Add(matrix);
                    }
                }

                {
                    Array2D<RgbPixel> img = Dlib.LoadImage<RgbPixel>(facePath2);

                    foreach (Rectangle face in frontalFaceDetector.Operator(img))
                    {
                        FullObjectDetection shape = shapePredictor.Detect(img, face);
                        ChipDetails faceChipDetail = Dlib.GetFaceChipDetails(shape, 150, 0.25);
                        Array2D<RgbPixel> faceChip = Dlib.ExtractImageChip<RgbPixel>(img, faceChipDetail);
                        Matrix<RgbPixel> matrix = new Matrix<RgbPixel>(faceChip);
                        chips.Add(matrix);
                    }
                }

                DlibDotNet.Dnn.OutputLabels<Matrix<float>> descriptors = lossMetric.Operator(chips);
                List<SamplePair> edges = new List<SamplePair>();

                for (uint i = 0; i < descriptors.Count; ++i)
                {
                    for (var j = i; j < descriptors.Count; ++j)
                    {
                        return Dlib.Length(descriptors[i] - descriptors[j]);
                    }
                }

                return 0.0F;
            }
            catch
            {
                return 0.0F;
            }
        }

        public static float FaceDistance(System.Drawing.Image faceImage1, System.Drawing.Image faceImage2)
        {
            try
            {
                List<Matrix<RgbPixel>> chips = new List<Matrix<RgbPixel>>();

                {
                    string fileName = GenerateRandomFileName();
                    faceImage1.Save(fileName);
                    Array2D<RgbPixel> img = Dlib.LoadImage<RgbPixel>(fileName);
                    File.Delete(fileName);

                    foreach (Rectangle face in frontalFaceDetector.Operator(img))
                    {
                        FullObjectDetection shape = shapePredictor.Detect(img, face);
                        ChipDetails faceChipDetail = Dlib.GetFaceChipDetails(shape, 150, 0.25);
                        Array2D<RgbPixel> faceChip = Dlib.ExtractImageChip<RgbPixel>(img, faceChipDetail);
                        Matrix<RgbPixel> matrix = new Matrix<RgbPixel>(faceChip);
                        chips.Add(matrix);
                    }
                }

                {
                    string fileName = GenerateRandomFileName();
                    faceImage1.Save(fileName);
                    Array2D<RgbPixel> img = Dlib.LoadImage<RgbPixel>(fileName);
                    File.Delete(fileName);

                    foreach (Rectangle face in frontalFaceDetector.Operator(img))
                    {
                        FullObjectDetection shape = shapePredictor.Detect(img, face);
                        ChipDetails faceChipDetail = Dlib.GetFaceChipDetails(shape, 150, 0.25);
                        Array2D<RgbPixel> faceChip = Dlib.ExtractImageChip<RgbPixel>(img, faceChipDetail);
                        Matrix<RgbPixel> matrix = new Matrix<RgbPixel>(faceChip);
                        chips.Add(matrix);
                    }
                }

                DlibDotNet.Dnn.OutputLabels<Matrix<float>> descriptors = lossMetric.Operator(chips);
                List<SamplePair> edges = new List<SamplePair>();

                for (uint i = 0; i < descriptors.Count; ++i)
                {
                    for (var j = i; j < descriptors.Count; ++j)
                    {
                        return Dlib.Length(descriptors[i] - descriptors[j]);
                    }
                }

                return 0.0F;
            }
            catch
            {
                return 0.0F;
            }
        }

        public static bool CompareFaces(params System.Drawing.Image[] images)
        {
            List<string> paths = new List<string>();

            foreach (System.Drawing.Image image in images)
            {
                string fileName = GenerateRandomFileName();
                image.Save(fileName);
                paths.Add(fileName);
            }

            return CompareFaces(paths.ToArray());
        }

        public static FaceEncoding[] GetFaceEncodings(params string[] paths)
        {
            try
            {
                List<FaceEncoding> encodings = new List<FaceEncoding>();
                List<Matrix<RgbPixel>> chips = new List<Matrix<RgbPixel>>();

                foreach (string path in paths)
                {
                    Array2D<RgbPixel> img = Dlib.LoadImage<RgbPixel>(path);

                    foreach (Rectangle face in frontalFaceDetector.Operator(img))
                    {
                        FullObjectDetection shape = shapePredictor.Detect(img, face);
                        ChipDetails faceChipDetail = Dlib.GetFaceChipDetails(shape, 150, 0.25);
                        Array2D<RgbPixel> faceChip = Dlib.ExtractImageChip<RgbPixel>(img, faceChipDetail);
                        Matrix<RgbPixel> matrix = new Matrix<RgbPixel>(faceChip);
                        chips.Add(matrix);
                    }
                }

                foreach (Matrix<RgbPixel> chip in chips)
                {
                    encodings.Add(new FaceEncoding(chip));
                }

                return encodings.ToArray();
            }
            catch
            {
                return new FaceEncoding[] { };
            }
        }

        public static FaceEncoding[] GetFaceEncodings(params System.Drawing.Image[] images)
        {
            List<string> paths = new List<string>();

            foreach (System.Drawing.Image image in images)
            {
                string fileName = GenerateRandomFileName();
                image.Save(fileName);
                paths.Add(fileName);
            }

            return GetFaceEncodings(paths.ToArray());
        }

        public static FaceEncoding GetFaceEncoding(params string[] paths)
        {
            return GetFaceEncodings(paths)[0];
        }

        public static FaceEncoding GetFaceEncoding(params System.Drawing.Image[] images)
        {
            return GetFaceEncodings(images)[0];
        }

        public static bool CompareFaceEncodings(params FaceEncoding[] encodings)
        {
            try
            {
                List<Matrix<RgbPixel>> chips = new List<Matrix<RgbPixel>>();

                foreach (FaceEncoding encoding in encodings)
                {
                    chips.Add(encoding.Matrix);
                }

                DlibDotNet.Dnn.OutputLabels<Matrix<float>> descriptors = lossMetric.Operator(chips);
                List<SamplePair> edges = new List<SamplePair>();

                for (uint i = 0; i < descriptors.Count; ++i)
                {
                    for (var j = i; j < descriptors.Count; ++j)
                    {
                        if (Dlib.Length(descriptors[i] - descriptors[j]) < 0.6)
                        {
                            edges.Add(new SamplePair(i, j));
                        }
                    }
                }

                uint clusters = 0;
                uint[] labels = new uint[] { };
                Dlib.ChineseWhispers(edges, 100, out clusters, out labels);
                return clusters == 1;
            }
            catch
            {
                return false;
            }
        }

        public static bool EasyCompareFaceEncodings(FaceEncoding encoding, params string[] paths)
        {
            List<FaceEncoding> encodings = new List<FaceEncoding>() { encoding };
            encodings.AddRange(GetFaceEncodings(paths));
            return CompareFaceEncodings(encodings.ToArray());
        }

        public static bool EasyCompareFaceEncodings(FaceEncoding encoding, params System.Drawing.Image[] images)
        {
            List<FaceEncoding> encodings = new List<FaceEncoding>() { encoding };
            encodings.AddRange(GetFaceEncodings(images));
            return CompareFaceEncodings(encodings.ToArray());
        }
    }
}