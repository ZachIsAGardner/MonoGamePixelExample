using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Text : Thing
{
    string message = "";

    public Text() { }
    public Text(string message, Vector2 position) 
    {
        this.message = message;
        Position = position;
        Color = Color.Black;
    }

    public override void Start()
    {
        base.Start();

    }

    public override void Draw()
    {
        base.Draw();

        MyGame.SpriteBatch.DrawString
        (
            spriteFont: MyGame.Font,
            text: message,
            position: Position - MyGame.Font.MeasureString(message) / 2f,
            color: Color
        );
    }
}