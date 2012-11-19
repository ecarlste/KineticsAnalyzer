
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;

namespace KineticsAnalyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Kinect sensor used to capture depth data
        /// </summary>
        private KinectSensor kinectSensor;

        /// <summary>
        /// Image for drawing depth information from the kinect
        /// </summary>
        private WriteableBitmap depthImageBitmap;

        /// <summary>
        /// Temporary storage space for frames fetched from the Kinect DepthStream
        /// </summary>
        private DepthImagePixel[] depthImagePixels;

        /// <summary>
        /// Temporary storage space for our depth data after it's converted to color
        /// </summary>
        private byte[] colorPixels;

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    kinectSensor = potentialSensor;
                    break;
                }
            }

            if (this.kinectSensor != null)
            {
                kinectSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                depthImagePixels = new DepthImagePixel[kinectSensor.DepthStream.FramePixelDataLength];
                colorPixels = new byte[kinectSensor.DepthStream.FramePixelDataLength * sizeof(int)];
                depthImageBitmap = new WriteableBitmap(kinectSensor.DepthStream.FrameWidth,
                    kinectSensor.DepthStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);

                DepthImage.Source = depthImageBitmap;
                kinectSensor.DepthFrameReady += KinectSensorDepthFrameReady;

                try
                {
                    kinectSensor.Start();
                }
                catch (IOException)
                {
                    this.kinectSensor = null;
                }
            }

            if (kinectSensor == null)
            {
                statusBarText.Text = Properties.Resources.NoKinectReady;
            }
            else
            {
                statusBarText.Text = Properties.Resources.KinectReady;
            }
        }

        private void KinectSensorDepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame != null)
                {
                    depthFrame.CopyDepthImagePixelDataTo(depthImagePixels);

                    int minDepth = depthFrame.MinDepth;
                    int maxDepth = depthFrame.MaxDepth;

                    int colorPixelIndex = 0;
                    for (int i = 0; i < depthImagePixels.Length; ++i)
                    {
                        short depth = depthImagePixels[i].Depth;

                        byte intensity = (byte)(depth >= minDepth && depth <= maxDepth ? depth : 0);

                        colorPixels[colorPixelIndex++] = intensity;
                        colorPixels[colorPixelIndex++] = intensity;
                        colorPixels[colorPixelIndex++] = intensity;

                        // not using alpha channel at this point, so skip the A part of BGRA by incrementing
                        // colorPixelIndex.
                        colorPixelIndex++;
                    }

                    depthImageBitmap.WritePixels(
                        new Int32Rect(0, 0, depthImageBitmap.PixelWidth, depthImageBitmap.PixelHeight),
                        colorPixels, depthImageBitmap.PixelWidth * sizeof(int), 0);
                }
            }
        }
    }
}
