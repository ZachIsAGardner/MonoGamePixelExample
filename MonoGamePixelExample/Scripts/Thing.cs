using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Thing
{
    public Vector2 Position;
    public SpriteEffects SpriteEffects = SpriteEffects.None;
    public float Rotation = 0f;
    public Vector2 Scale = new Vector2(1, 1);
    public Color Color = Color.White;
    public bool Absolute = false;

    public bool DidStart { get; private set; }

    public virtual void Start()
    {
        DidStart = true;
    }

    public virtual void Update()
    {

    }

    public virtual void Draw()
    {

    }
}