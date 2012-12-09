
using Microsoft.Kinect;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using System.ComponentModel;

namespace KinectSkeletonAnalyzer
{
    public class SkeletonMeasurer
    {
        private bool useInferredJoints;
        public bool UseInferredJoints
        {
            get { return useInferredJoints; }
            set { useInferredJoints = value; }
        }

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
            : this(skeleton, false)
        {}

        public SkeletonMeasurer(Skeleton skeleton, bool useInferredJoints)
        {
            testMeasurements = new List<TestMeasurement>();
            bones = new Dictionary<SkeletonBoneType, SkeletonBone>();

            this.skeleton = skeleton;
            this.useInferredJoints = useInferredJoints;
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

            double kneeFlexionLeftValue = double.NaN;
            double kneeFlexionRightValue = double.NaN;

            if (MeasureAllowed(lowerLegLeft, upperLegRight))
            {
                kneeFlexionLeftValue = 180.0 - Vector3D.AngleBetween(lowerLegLeft.Vector, -upperLegLeft.Vector);
            }

            if (MeasureAllowed(lowerLegRight, upperLegRight))
            {

                kneeFlexionRightValue = 180.0 - Vector3D.AngleBetween(lowerLegRight.Vector, -upperLegRight.Vector);
            }

            testMeasurements.Add(new TestMeasurement(TestMeasurementType.KneeFlexionLeft, kneeFlexionLeftValue));
            testMeasurements.Add(new TestMeasurement(TestMeasurementType.KneeFlexionRight, kneeFlexionRightValue));
        }

        private void measureHipFlexion()
        {
            SkeletonBone backLower = getBone(SkeletonBoneType.BackLower);
            SkeletonBone upperLegLeft = getBone(SkeletonBoneType.UpperLegLeft);
            SkeletonBone upperLegRight = getBone(SkeletonBoneType.UpperLegRight);

            double hipFlexionLeftValue = double.NaN;
            double hipFlexionRightValue = double.NaN;

            if (MeasureAllowed(backLower, upperLegLeft))
            {
                hipFlexionLeftValue = 180.0 - Vector3D.AngleBetween(upperLegLeft.Vector, backLower.Vector);
            }

            if (MeasureAllowed(backLower, upperLegRight))
            {
                hipFlexionRightValue = 180.0 - Vector3D.AngleBetween(upperLegRight.Vector, backLower.Vector);
            }

            testMeasurements.Add(new TestMeasurement(TestMeasurementType.HipFlexionLeft, hipFlexionLeftValue));
            testMeasurements.Add(new TestMeasurement(TestMeasurementType.HipFlexionRight, hipFlexionRightValue));
        }

        private bool MeasureAllowed(SkeletonBone b1, SkeletonBone b2)
        {
            bool allTracked = b1.TrackingState.Equals(JointTrackingState.Tracked)
                && b2.TrackingState.Equals(JointTrackingState.Tracked);

            bool allTrackedOrInferred = !b1.TrackingState.Equals(JointTrackingState.NotTracked) 
                && !b2.TrackingState.Equals(JointTrackingState.NotTracked);

            if (allTracked || (useInferredJoints && allTrackedOrInferred))
            {
                return true;
            }

            return false;
        }

        private void measureKneeValgus()
        {
            SkeletonBone lowerLegLeft = getBone(SkeletonBoneType.LowerLegLeft);
            SkeletonBone upperLegLeft = getBone(SkeletonBoneType.UpperLegLeft);
            SkeletonBone lowerLegRight = getBone(SkeletonBoneType.LowerLegRight);
            SkeletonBone upperLegRight = getBone(SkeletonBoneType.UpperLegRight);

            double kneeValgusLeft = double.NaN;
            double kneeValgusRight = double.NaN;

            // in order to calculate the hipAsis, both the left and right hips have to be valid usable joints, if both
            // the left and right upper leg bones are valid then we are guaranteed that both the left and right hips
            // are valid and measurable
            if (MeasureAllowed(upperLegLeft, upperLegRight))
            {
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

                if (MeasureAllowed(upperLegLeft, lowerLegLeft))
                {
                    Vector3D upperLegLeftProjected =
                        upperLegLeft.Vector - Vector3D.DotProduct(upperLegLeft.Vector, planeNormal) * planeNormal;
                    Vector3D lowerLegLeftProjected =
                        lowerLegLeft.Vector - Vector3D.DotProduct(lowerLegLeft.Vector, planeNormal) * planeNormal;
                    kneeValgusLeft = Vector3D.AngleBetween(upperLegLeftProjected, lowerLegLeftProjected);
                }

                if (MeasureAllowed(upperLegRight, lowerLegRight))
                {
                    Vector3D upperLegRightProjected =
                        upperLegRight.Vector - Vector3D.DotProduct(upperLegRight.Vector, planeNormal) * planeNormal;
                    Vector3D lowerLegRightProjected =
                        lowerLegRight.Vector - Vector3D.DotProduct(lowerLegRight.Vector, planeNormal) * planeNormal;
                    kneeValgusRight = Vector3D.AngleBetween(upperLegRightProjected, lowerLegRightProjected);
                }
            }

            testMeasurements.Add(new TestMeasurement(TestMeasurementType.KneeValgusLeft, kneeValgusLeft));
            testMeasurements.Add(new TestMeasurement(TestMeasurementType.KneeValgusRight, kneeValgusRight));
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
                else if (j1.TrackingState.Equals(JointTrackingState.NotTracked)
                    || j2.TrackingState.Equals(JointTrackingState.NotTracked))
                {
                    trackingState = JointTrackingState.NotTracked;
                }
                else
                {
                    trackingState = JointTrackingState.Inferred;
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
