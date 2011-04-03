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
    }
}
