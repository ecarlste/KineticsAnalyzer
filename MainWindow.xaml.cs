
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
        private static readonly int Bgr32BytesPerPixel = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;

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
                kinectSensor.SkeletonStream.Enable();

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
            int imageWidth = 0;
            int imageHeight = 0;
            int minDepth = 0;
            int maxDepth = 0;

            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame != null)
                {
                    imageWidth = depthFrame.Width;
                    imageHeight = depthFrame.Height;

                    depthFrame.CopyDepthImagePixelDataTo(depthImagePixels);

                    minDepth = depthFrame.MinDepth;
                    maxDepth = depthFrame.MaxDepth;
                }

                if (imageWidth != 0)
                {
                    DepthColorizer.ConvertDepthFrame(depthImagePixels, minDepth, maxDepth, colorPixels);

                    depthImageBitmap.WritePixels(
                        new Int32Rect(0, 0, imageWidth, imageHeight),
                        colorPixels, imageWidth * Bgr32BytesPerPixel, 0);
                }
            }
        }
    }
}
