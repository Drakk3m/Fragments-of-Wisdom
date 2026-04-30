namespace Game.Domain.Common
{
    public interface IRandomSource
    {
        double NextDouble();

        int NextInt(
            int minInclusive,
            int maxExclusive);
    }
}
