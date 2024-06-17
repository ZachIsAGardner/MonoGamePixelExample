using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public static class Utility
{
    public static Rectangle GetSourceRectangle(int tileNumber, Texture2D texture, int tileSize = CONSTANTS.TILE_SIZE)
    {
        Vector2 coord = CoordinatesFromNumber(tileNumber, texture, tileSize);
        return new Rectangle((int)coord.X, (int)coord.Y, tileSize, tileSize);
    }

    public static Vector2 CoordinatesFromNumber(int tileNumber, Texture2D texture, int tileSize = CONSTANTS.TILE_SIZE)
    {
        if (texture == null || tileSize > texture.Width || tileSize > texture.Height) return Vector2.Zero;

        var sheetWidth = texture.Width;
        var tilesPerRow = sheetWidth / tileSize;
        tileNumber--;
        int row = 0;

        while (tileNumber >= tilesPerRow)
        {
            tileNumber -= tilesPerRow;
            row++;
        }

        return new Vector2(tileNumber * tileSize, row * tileSize);
    }
}