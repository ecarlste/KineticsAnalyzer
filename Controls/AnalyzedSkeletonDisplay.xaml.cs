using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KinectSkeletonAnalyzer;
using Microsoft.Kinect;

namespace KineticsAnalyzer.Controls
{
    /// <summary>
    /// Interaction logic for AnalyzedSkeletonDisplay.xaml
    /// </summary>
    public partial class AnalyzedSkeletonDisplay : UserControl
    {
        private InjuryRiskAnalyzer riskAnalyzer;
        private Skeleton skeleton;

        public AnalyzedSkeletonDisplay()
        {
            InitializeComponent();

            this.AnalyzedSkeletonCanvasPanel.Children.Add(new AnalyzedKinectSkeleton());
        }

        public InjuryRiskAnalyzer RiskAnalyzer
        {
            get { return riskAnalyzer; }
            set { riskAnalyzer = value; }
        }

        internal void DetermineSkeletonFrameUsed(List<Skeleton> skeletons)
        {
            this.skeleton = null;

            foreach (Skeleton skeleton in skeletons)
            {
                if (IsCompletelyTracked(skeleton))
                {
                    this.skeleton = skeleton;
                    break;
                }
            }
        }

        private bool IsCompletelyTracked(Skeleton skeleton)
        {
            foreach (Joint joint in skeleton.Joints)
            {
                if (!joint.TrackingState.Equals(JointTrackingState.Tracked))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
