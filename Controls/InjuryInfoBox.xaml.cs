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

namespace KineticsAnalyzer.Controls
{
    /// <summary>
    /// Interaction logic for InjuryInfoBox.xaml
    /// </summary>
    public partial class InjuryInfoBox : UserControl
    {
        private string _riskType;
        public string RiskType
        {
            get { return _riskType; }
            set { _riskType = value; }
        }

        private InjuryRiskType _riskFactor;
        public InjuryRiskType RiskFactor
        {
            get { return _riskFactor; }
            set { _riskFactor = value; }
        }

        private List<string> _riskReasons;
        public List<string> RiskReasons
        {
            get { return _riskReasons; }
            set { _riskReasons = value; }
        }

        private List<string> _correctiveExercise;
        public List<string> CorrectiveExercise
        {
            get { return _correctiveExercise; }
            set { _correctiveExercise = value; }
        }

        public InjuryInfoBox()
        {
            InitializeComponent();
        }
    }
}
