using System.Collections.Generic;
using Godot;

public interface IRandomNumberGenerator
{
    public int RandomInt(int min, int max);

    public float RandomFloat(float min, float max);

    public Vector2 RandomVec2(float minX, float maxX, float minY, float maxY);

    public T RandomListElement<T>(IReadOnlyList<T> list);

    public void ShuffleList<T>(IList<T> list);
}
