using System;
using System.Collections.Generic;
using Microsoft.DirectX;

namespace Steering
{
    public static class Helper2
    {
        public static Vector2 Perp(Vector2 v)
        {
            return new Vector2(-v.Y, +v.X);
        }

        public static void Truncate(ref Vector2 v, float max)
        {
            if (v.Length() > max)
            {
                v.Normalize();
                v *= max;
            }
        }

        public static void WrapRound(ref Vector2 pos, int maxX, int maxY)
        {
            if (pos.X > maxX)
                pos.X = 0.0f;
            if (pos.X < 0)
                pos.X = maxX;
            if (pos.Y < 0)
                pos.Y = maxY;
            if (pos.Y > maxX)
                pos.Y = 0.0f;
        }
    }
}
