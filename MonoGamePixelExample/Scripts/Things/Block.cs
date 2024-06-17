using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Block : Thing
{
    Texture2D texture;
    Rectangle rectangle;
    int tileNumber = 1;

    public Block() { }
    public Block(int tileNumber, Vector2 position) 
    {
        this.tileNumber = tileNumber;
        Position = position;
    }

    public override void Start()
    {
        base.Start();

        texture = MyGame.Textures["GridBlock"];
        rectangle = Utility.GetSourceRectangle(tileNumber, texture);
    }

    public override void Draw()
    {
        base.Draw();

        MyGame.SpriteBatch.Draw(
            texture: texture,
            position: Position,
            sourceRectangle: rectangle,
            color: Color,
            rotation: Rotation,
            origin: new Vector2(texture.Width / 2f, texture.Height / 2f),
            scale: Scale,
            effects: SpriteEffects,
            layerDepth: 0f
        );
    }
}