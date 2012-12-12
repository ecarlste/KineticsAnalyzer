
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
using KinectSkeletonAnalyzer;

namespace KineticsAnalyzer.Controls
{
    class AnalyzedKinectSkeleton : Control
    {
        private readonly Color HighRiskBoneColor = Color.FromRgb(255, 0, 0);
        private readonly Color ModerateRiskBoneColor = Color.FromRgb(127, 127, 0);
        private readonly Color LowRiskBoneColor = Color.FromRgb(0, 255, 0);

        private readonly Color HighRiskJointColor = Color.FromRgb(192, 68, 68);
        private readonly Color ModerateRiskJointColor = Color.FromRgb(130, 130, 68);
        private readonly Color LowRiskJointColor = Color.FromRgb(68, 192, 68);

        private Dictionary<JointType, JointMapping> jointMapping;
        private Dictionary<JointType, InjuryRiskType> injuryRisks;

        public Dictionary<JointType, JointMapping> JointMapping
        {
            get { return jointMapping; }
            set { jointMapping = value; }
        }

        public Dictionary<JointType, InjuryRiskType> InjuryRisks
        {
            get { return injuryRisks; }
            set { injuryRisks = value; }
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

            // Render Joints
            foreach (JointMapping joint in this.JointMapping.Values)
            {
                Brush drawBrush = new SolidColorBrush(LowRiskJointColor);

                if (injuryRisks.ContainsKey(joint.Joint.JointType))
                {
                    switch (injuryRisks[joint.Joint.JointType])
                    {
                        case InjuryRiskType.Moderate:
                            drawBrush = new SolidColorBrush(ModerateRiskJointColor);
                            break;
                        case InjuryRiskType.High:
                            drawBrush = new SolidColorBrush(HighRiskJointColor);
                            break;
                    }
                }

                drawingContext.DrawEllipse(drawBrush, null, joint.MappedPoint, 3, 3);
            }
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

            LinearGradientBrush gradient = new LinearGradientBrush();
            gradient.StartPoint = new Point(0, 0);
            gradient.EndPoint = new Point(1, 1);

            GradientStop color1 = new GradientStop();
            color1.Color = GetBoneColor(jointType1);
            color1.Offset = 0.0;
            gradient.GradientStops.Add(color1);

            GradientStop color2 = new GradientStop();
            color2.Color = GetBoneColor(jointType2);
            color2.Offset = 1.0;
            gradient.GradientStops.Add(color2);

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = new Pen(gradient, 6);
            
            drawingContext.DrawLine(drawPen, joint1.MappedPoint, joint2.MappedPoint);
        }

        private Color GetBoneColor(JointType jointType)
        {
            if (injuryRisks.ContainsKey(jointType))
            {
                switch (injuryRisks[jointType])
                {
                    case InjuryRiskType.Moderate:
                        return ModerateRiskBoneColor;
                    case InjuryRiskType.High:
                        return HighRiskBoneColor;
                }
            }

            return LowRiskBoneColor;
        }
    }
}
