public static class MathHelper
{
  public static int Mod(int x, int m)
  {
    return (x % m + m) % m;
  }

  public static float Mod(float x, float m)
  {
    return (x % m + m) % m;
  }
}