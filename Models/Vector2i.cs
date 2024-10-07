using System;
using Godot;
using Newtonsoft.Json;

public class Vector2i : Godot.Object, IEquatable<Vector2i>
{

  public static Vector2i Zero = new Vector2i(0, 0);

  [JsonProperty("x")]
  public int X;

  [JsonProperty("y")]
  public int Y;

  [JsonConstructor]
  public Vector2i(int x, int y)
  {
    X = x;
    Y = y;
  }

  public static Vector2i operator +(Vector2i v1, Vector2i v2) => new Vector2i(v1.X + v2.X, v1.Y + v2.Y);
  public static Vector2i operator -(Vector2i v1, Vector2i v2) => new Vector2i(v1.X - v2.X, v1.Y - v2.Y);

  public static bool operator ==(Vector2i optionsA, Vector2i optionsB)
  {
    return Vector2i.Equals(optionsA, optionsB);
  }

  public static bool operator !=(Vector2i optionsA, Vector2i optionsB)
  {
    return !Vector2i.Equals(optionsA, optionsB);
  }

  public override string ToString()
  {
    return $"({X}, {Y})";
  }

  // See: https://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-overriding-gethashcode/263416#263416
  public override int GetHashCode()
  {
    unchecked // Overflow is fine, just wrap
    {
      int hash = (int)2166136261;
      hash = hash * 16777619 ^ X.GetHashCode();
      hash = hash * 16777619 ^ Y.GetHashCode();
      return hash;
    }
  }

  public override bool Equals(object obj)
  {
    return Equals(obj as Vector2i);
  }

  public bool Equals(Vector2i obj)
  {
    return obj != null && obj.X == this.X && obj.Y == this.Y;
  }
}

public static class Vector2iExtensions
{
  public static Vector2 ToVector2(this Vector2i v)
  {
    return new Vector2((float)v.X, (float)v.Y);
  }
}