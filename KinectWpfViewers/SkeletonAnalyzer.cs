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
            // measure left knee flexion
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

            double leftKneeFlexion = Vector3D.AngleBetween(leftLegLower, leftLegUpper);

            // measure right knee flexion
            Joint rightAnkle = skeleton.Joints[JointType.AnkleRight];
            Joint rightKnee = skeleton.Joints[JointType.KneeRight];
            Joint rightHip = skeleton.Joints[JointType.HipRight];

            Vector3D rightLegLower = new Vector3D(
                    rightAnkle.Position.X - rightKnee.Position.X,
                    rightAnkle.Position.Y - rightKnee.Position.Y,
                    rightAnkle.Position.Z - rightKnee.Position.Z
                );
            Vector3D rightLegUpper = new Vector3D(
                    rightHip.Position.X - rightKnee.Position.X,
                    rightHip.Position.Y - rightKnee.Position.Y,
                    rightHip.Position.Z - rightKnee.Position.Z
                );

            rightLegLower.Normalize();
            rightLegUpper.Normalize();

            double rightKneeFlexion = Vector3D.AngleBetween(rightLegLower, rightLegUpper);
        }
    }
}
