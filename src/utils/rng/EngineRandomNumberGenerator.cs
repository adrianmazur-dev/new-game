using Godot;

public class EngineRandomNumberGenerator : IRandomNumberGenerator
{
    private readonly RandomNumberGenerator _rng;

    public EngineRandomNumberGenerator()
    {
        _rng = new RandomNumberGenerator();
    }

    public EngineRandomNumberGenerator(ulong seed)
    {
        _rng = new RandomNumberGenerator { Seed = seed };
    }

    public int RandomInt(int min, int max)
    {
        return _rng.RandiRange(min, max - 1);
    }

    public float RandomFloat(float min, float max)
    {
        return _rng.RandfRange(min, max);
    }

    public Vector2 RandomVec2(float minX, float maxX, float minY, float maxY)
    {
        return new Vector2(RandomFloat(minX, maxX), RandomFloat(minY, maxY));
    }
}
