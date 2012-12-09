
namespace KinectSkeletonAnalyzer
{
    public class TestMeasurement
    {
        private TestMeasurementType type;
        public TestMeasurementType Type
        {
            get { return type; }
            set { type = value; }
        }

        private double value;
        public double Value
        {
            get { return value; }
            set { this.value = value; }
        }

        public TestMeasurement(TestMeasurementType type, double value)
        {
            this.type = type;
            this.value = value;
        }
    }
}
