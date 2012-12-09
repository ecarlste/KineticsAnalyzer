
using Microsoft.Kinect;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using System.ComponentModel;

namespace KinectSkeletonAnalyzer
{
    public class SkeletonMeasurer
    {
        private List<TestMeasurement> testMeasurements;
        public List<TestMeasurement> TestMeasurements
        {
            get { return testMeasurements; }
        }

        private Dictionary<SkeletonBoneType, SkeletonBone> bones;

        private Skeleton skeleton;
        public Skeleton Skeleton
        {
            get { return skeleton; }
            set { skeleton = value; }
        }
        
        public SkeletonMeasurer(Skeleton skeleton)
        {
            testMeasurements = new List<TestMeasurement>();
            bones = new Dictionary<SkeletonBoneType, SkeletonBone>();

            this.skeleton = skeleton;
        }

        public void determineMeasurements()
        {
            measureKneeFlexion();

            measureHipFlexion();

            measureKneeValgus();
        }

        private void measureKneeFlexion()
        {
            SkeletonBone lowerLegLeft = getBone(SkeletonBoneType.LowerLegLeft);
            SkeletonBone upperLegLeft = getBone(SkeletonBoneType.UpperLegLeft);
            SkeletonBone lowerLegRight = getBone(SkeletonBoneType.LowerLegRight);
            SkeletonBone upperLegRight = getBone(SkeletonBoneType.UpperLegRight);

            testMeasurements.Add(new TestMeasurement(TestMeasurementType.KneeFlexionLeft,
                180.0 - Vector3D.AngleBetween(lowerLegLeft.Vector, -upperLegLeft.Vector)));
            testMeasurements.Add(new TestMeasurement(TestMeasurementType.KneeFlexionRight,
                180.0 - Vector3D.AngleBetween(lowerLegRight.Vector, -upperLegRight.Vector)));
        }

        private void measureHipFlexion()
        {
            SkeletonBone backLower = getBone(SkeletonBoneType.BackLower);
            SkeletonBone upperLegLeft = getBone(SkeletonBoneType.UpperLegLeft);
            SkeletonBone upperLegRight = getBone(SkeletonBoneType.UpperLegRight);

            testMeasurements.Add(new TestMeasurement(TestMeasurementType.HipFlexionLeft,
                180.0 - Vector3D.AngleBetween(backLower.Vector, upperLegLeft.Vector)));
            testMeasurements.Add( new TestMeasurement(TestMeasurementType.HipFlexionRight,
                180.0 - Vector3D.AngleBetween(backLower.Vector, upperLegRight.Vector)));
        }

        private void measureKneeValgus()
        {
            SkeletonBone lowerLegLeft = getBone(SkeletonBoneType.LowerLegLeft);
            SkeletonBone upperLegLeft = getBone(SkeletonBoneType.UpperLegLeft);
            SkeletonBone lowerLegRight = getBone(SkeletonBoneType.LowerLegRight);
            SkeletonBone upperLegRight = getBone(SkeletonBoneType.UpperLegRight);

            SkeletonPoint hipLeft = skeleton.Joints[JointType.HipLeft].Position;
            SkeletonPoint hipRight = skeleton.Joints[JointType.HipRight].Position;

            Vector3D hipAsis = new Vector3D(
                   hipLeft.X - hipRight.X,
                   hipLeft.Y - hipRight.Y,
                   hipLeft.Z - hipRight.Z
               );

            hipAsis.Normalize();

            Vector3D up = new Vector3D(0.0, 1.0, 0.0);

            // get the normal vector that represents the plane in which up and
            // hipAsis are contained within, then normalize it
            Vector3D planeNormal = Vector3D.CrossProduct(up, hipAsis);
            planeNormal.Normalize();

            // determine both upper and 
            Vector3D upperLegLeftProjected =
                upperLegLeft.Vector - Vector3D.DotProduct(upperLegLeft.Vector, planeNormal) * planeNormal;
            Vector3D lowerLegLeftProjected =
                lowerLegLeft.Vector - Vector3D.DotProduct(lowerLegLeft.Vector, planeNormal) * planeNormal;
            testMeasurements.Add(new TestMeasurement(TestMeasurementType.KneeValgusLeft,
                Vector3D.AngleBetween(upperLegLeftProjected, lowerLegLeftProjected)));

            Vector3D upperLegRightProjected =
                upperLegRight.Vector - Vector3D.DotProduct(upperLegRight.Vector, planeNormal) * planeNormal;
            Vector3D lowerLegRightProjected =
                lowerLegRight.Vector - Vector3D.DotProduct(lowerLegRight.Vector, planeNormal) * planeNormal;
            testMeasurements.Add(new TestMeasurement(TestMeasurementType.KneeValgusRight,
                Vector3D.AngleBetween(upperLegRightProjected, lowerLegRightProjected)));
        }

        private SkeletonBone getBone(SkeletonBoneType boneType)
        {
            if (!bones.ContainsKey(boneType))
            {
                Joint j1;
                Joint j2;

                switch (boneType)
                {
                    case SkeletonBoneType.LowerLegLeft:
                        j1 = skeleton.Joints[JointType.KneeLeft];
                        j2 = skeleton.Joints[JointType.AnkleLeft];
                        break;
                    case SkeletonBoneType.LowerLegRight:
                        j1 = skeleton.Joints[JointType.KneeRight];
                        j2 = skeleton.Joints[JointType.AnkleRight];
                        break;
                    case SkeletonBoneType.UpperLegLeft:
                        j1 = skeleton.Joints[JointType.HipLeft];
                        j2 = skeleton.Joints[JointType.KneeLeft];
                        break;
                    case SkeletonBoneType.UpperLegRight:
                        j1 = skeleton.Joints[JointType.HipRight];
                        j2 = skeleton.Joints[JointType.KneeRight];
                        break;
                    case SkeletonBoneType.BackLower:
                        j1 = skeleton.Joints[JointType.HipCenter];
                        j2 = skeleton.Joints[JointType.Spine];
                        break;
                    default:
                        throw new InvalidEnumArgumentException();
                }

                JointTrackingState trackingState;

                if (j1.TrackingState.Equals(JointTrackingState.Tracked)
                    && j2.TrackingState.Equals(JointTrackingState.Tracked))
                {
                    trackingState = JointTrackingState.Tracked;
                }
                else
                {
                    trackingState = JointTrackingState.NotTracked;
                }
                
                SkeletonBone bone = new SkeletonBone(
                    boneType,
                    trackingState,
                    j2.Position.X - j1.Position.X,
                    j2.Position.Y - j1.Position.Y,
                    j2.Position.Z - j1.Position.Z
                    );

                bones[boneType] = bone;

                return bone;
            }

            return bones[boneType];
        }
    }
}
