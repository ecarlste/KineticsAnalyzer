using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.Windows.Media.Media3D;

namespace KinectWpfViewers
{
    class SkeletonAnalyzer
    {
        public void analyze(Skeleton skeleton)
        {
            Joint leftAnkle = skeleton.Joints[JointType.AnkleLeft];
            Joint leftKnee = skeleton.Joints[JointType.KneeLeft];
            Joint leftHip = skeleton.Joints[JointType.HipLeft];

            Vector3D leftLegLower = new Vector3D(
                    leftAnkle.Position.X - leftKnee.Position.X,
                    leftAnkle.Position.Y - leftKnee.Position.Y,
                    leftAnkle.Position.Z - leftKnee.Position.Z
                );
            Vector3D leftLegUpper = new Vector3D(
                    leftHip.Position.X - leftKnee.Position.X,
                    leftHip.Position.Y - leftKnee.Position.Y,
                    leftHip.Position.Z - leftKnee.Position.Z
                );

            leftLegLower.Normalize();
            leftLegUpper.Normalize();

            double angle = Vector3D.AngleBetween(leftLegLower, leftLegUpper);
        }
    }
}
