using System;

namespace UnityTools.Utilities
{
    public class GaussianDistribution
    {
        private float mean;
        private float variance;

        private static Random random = new Random();

        public GaussianDistribution(float mean, float variance)
        {
            this.mean = mean;
            this.variance = variance;
        }

        public float Generate()
        {
            return Generate(mean, variance);
        }

        // Taken from https://stackoverflow.com/a/218600
        public static float GenerateStandard()
        {
            double u1 = 1.0 - random.NextDouble();
            double u2 = 1.0 - random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2);
            return (float)randStdNormal;
        }

        public static float Generate(float mean, float variance)
        {
            return GenerateStandard() * variance + mean;
        }
    }
}