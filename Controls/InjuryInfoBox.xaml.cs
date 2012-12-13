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
    /// Interaction logic for InjuryInfoBox.xaml
    /// </summary>
    public partial class InjuryInfoBox : UserControl
    {
        private JointType _jointType;
        private InjuryRiskType _riskFactor;
        private List<string> _riskReasons;
        private List<string> _correctiveExercise;
        private Point _jointPosition;
        
        public JointType JointType
        {
            get { return _jointType; }
            set { _jointType = value; }
        }

        public InjuryRiskType RiskFactor
        {
            get { return _riskFactor; }
            set { _riskFactor = value; }
        }

        public List<string> RiskReasons
        {
            get { return _riskReasons; }
            set { _riskReasons = value; }
        }

        public List<string> CorrectiveExercise
        {
            get { return _correctiveExercise; }
            set { _correctiveExercise = value; }
        }

        public Point JointPosition
        {
            get { return _jointPosition; }
            set { _jointPosition = value; }
        }

        public InjuryInfoBox()
        {
            InitializeComponent();
        }
    }
}
