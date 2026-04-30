using Game.Domain.Common;

namespace Game.Infrastructure.Random
{
    public class SeededRandomSource : IRandomSource
    {
        private readonly System.Random random;

        public SeededRandomSource(int seed)
        {
            random = new System.Random(seed);
        }

        public double NextDouble()
        {
            return random.NextDouble();
        }

        public int NextInt(
            int minInclusive,
            int maxExclusive)
        {
            return random.Next(minInclusive, maxExclusive);
        }
    }
}
