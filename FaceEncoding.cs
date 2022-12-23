using DlibDotNet;
using System.Collections.Generic;

namespace FaceRecognitionSharp
{
    public class FaceEncoding
    {
        public Matrix<RgbPixel> Matrix { get; set; }

        public FaceEncoding(Matrix<RgbPixel> matrix)
        {
            Matrix = matrix;
        }
    }
}