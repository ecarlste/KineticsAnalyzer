
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using KinectWpfViewers;
using Microsoft.Kinect;

namespace KineticsAnalyzer.Controls
{
    class AnalyzedKinectSkeleton : Control
    {
        private Dictionary<JointType, JointMapping> jointMapping;

        public Dictionary<JointType, JointMapping> JointMapping
        {
            get { return jointMapping; }
            set { jointMapping = value; }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            // Render Torso
            this.DrawBone(drawingContext, JointType.Head, JointType.ShoulderCenter);
            this.DrawBone(drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft);
            this.DrawBone(drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight);
            this.DrawBone(drawingContext, JointType.ShoulderCenter, JointType.Spine);
            this.DrawBone(drawingContext, JointType.Spine, JointType.HipCenter);
            this.DrawBone(drawingContext, JointType.HipCenter, JointType.HipLeft);
            this.DrawBone(drawingContext, JointType.HipCenter, JointType.HipRight);

            // Left Arm
            this.DrawBone(drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft);
            this.DrawBone(drawingContext, JointType.ElbowLeft, JointType.WristLeft);
            this.DrawBone(drawingContext, JointType.WristLeft, JointType.HandLeft);

            // Right Arm
            this.DrawBone(drawingContext, JointType.ShoulderRight, JointType.ElbowRight);
            this.DrawBone(drawingContext, JointType.ElbowRight, JointType.WristRight);
            this.DrawBone(drawingContext, JointType.WristRight, JointType.HandRight);

            // Left Leg
            this.DrawBone(drawingContext, JointType.HipLeft, JointType.KneeLeft);
            this.DrawBone(drawingContext, JointType.KneeLeft, JointType.AnkleLeft);
            this.DrawBone(drawingContext, JointType.AnkleLeft, JointType.FootLeft);

            // Right Leg
            this.DrawBone(drawingContext, JointType.HipRight, JointType.KneeRight);
            this.DrawBone(drawingContext, JointType.KneeRight, JointType.AnkleRight);
            this.DrawBone(drawingContext, JointType.AnkleRight, JointType.FootRight);
        }

        private void DrawBone(DrawingContext drawingContext, JointType jointType1, JointType jointType2)
        {
            JointMapping joint1;
            JointMapping joint2;

            // If we can't find either of these joints, exit
            if (!this.JointMapping.TryGetValue(jointType1, out joint1) ||
                !this.JointMapping.TryGetValue(jointType2, out joint2))
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = new Pen(Brushes.Green, 6);
            
            drawingContext.DrawLine(drawPen, joint1.MappedPoint, joint2.MappedPoint);
        }
    }
}
