using Godot;

public interface IRandomNumberGenerator
{
    public abstract int RandomInt(int min, int max);

    public abstract float RandomFloat(float min, float max);

    public abstract Vector2 RandomVec2(float minX, float maxX, float minY, float maxY);
}
