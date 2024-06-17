using Microsoft.Xna.Framework;
using System;

public static class Time
{
    public static float Delta = 1f;
    public static float Elapsed = 0;

    public static void Update(GameTime gameTime)
    {
        Delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Elapsed += Delta;
    }
}
