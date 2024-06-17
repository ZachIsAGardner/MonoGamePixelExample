using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Character : Thing
{
    float speed = 2f;
    Texture2D texture;
    Vector2 lastPosition = Vector2.Zero;

    public Character() { }
    public Character(Vector2 position) 
    {
        Position = position;
    }

    public override void Start()
    {
        base.Start();

        texture = MyGame.Textures["Garbanzo"];
    }

    public override void Update()
    {
        base.Update();

        if (Input.IsHeld("Left")) Position.X -= speed * Time.Delta;
        if (Input.IsHeld("Right")) Position.X += speed * Time.Delta;
        if (Input.IsHeld("Up")) Position.Y -= speed * Time.Delta;
        if (Input.IsHeld("Down")) Position.Y += speed * Time.Delta;

        if (Position.X != lastPosition.X)
        {
            if (Position.X < lastPosition.X) SpriteEffects = SpriteEffects.FlipHorizontally;
            else SpriteEffects = SpriteEffects.None;
        }

        lastPosition = Position;
    }

    public override void Draw()
    {
        base.Draw();

        MyGame.SpriteBatch.Draw(
            texture: texture,
            position: Position,
            sourceRectangle: null,
            color: Color,
            rotation: Rotation,
            origin: new Vector2(texture.Width / 2f, texture.Height / 2f),
            scale: Scale,
            effects: SpriteEffects,
            layerDepth: 0f
        );
    }
}