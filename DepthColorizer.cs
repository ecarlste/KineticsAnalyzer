
using System;
using System.Windows.Media;
using Microsoft.Kinect;

namespace KineticsAnalyzer
{

    /// <summary>
    /// Creates a color representation of a depth frame
    /// </summary>
    internal class DepthColorizer
    {
        /// <summary>
        /// Number of bits per pixel in a Bgr32 format bitmap. We need to add 7 to guarantee that our int math
        /// doesn't truncate the remainder, but rather rounds up.
        /// </summary>
        private static readonly int Bgr32BytesPerPixel = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;

        /// <summary>
        /// Offset in bytes to the Red byte in a Bgr32 pixel
        /// </summary>
        private const int RedIndex = 2;

        /// <summary>
        /// Offset in bytes to the Green byte in a Bgr32 pixel
        /// </summary>
        private const int GreenIndex = 1;

        /// <summary>
        /// Offset in bytes to the Blue byte in a Bgr32 pixel
        /// </summary>
        private const int BlueIndex = 0;

        /// <summary>
        /// Converts an array of DepthImagePixels into a byte array in Bgr32 format.
        /// Pixel intensity represents depth; green indicates a player being tracked.
        /// </summary>
        /// <param name="depthFrame">The depth buffer to convert.</param>
        /// <param name="minDepth">The minimum reliable depth for this frame.</param>
        /// <param name="maxDepth">The maximum reliable depth for this frame.</param>
        /// <param name="colorFrame">The buffer to fill with color pixels.</param>
        public static void ConvertDepthFrame(DepthImagePixel[] depthFrame, int minDepth, int maxDepth, byte[] colorFrame)
        {
            // Check to see if both buffer lengths are the same.
            if ((depthFrame.Length * Bgr32BytesPerPixel) != colorFrame.Length)
            {
                throw new InvalidOperationException();
            }

            for (int depthIndex = 0, colorIndex = 0; colorIndex < colorFrame.Length;
                depthIndex++, colorIndex += Bgr32BytesPerPixel)
            {
                short depth = depthFrame[depthIndex].Depth;
                int player = depthFrame[depthIndex].PlayerIndex;

                byte intensity = (byte)(depth >= minDepth && depth <= maxDepth ? depth : 0);

                byte[] colorShift = new byte[3] {0, 0, 0};

                if (player != 0)
                {
                    colorShift[RedIndex] = 2;
                    colorShift[BlueIndex] = 2;
                }

                colorFrame[colorIndex + RedIndex] = (byte)(intensity >> colorShift[RedIndex]);
                colorFrame[colorIndex + GreenIndex] = (byte)(intensity >> colorShift[GreenIndex]);
                colorFrame[colorIndex + BlueIndex] = (byte)(intensity >> colorShift[BlueIndex]);
            }
        }
    }
}
