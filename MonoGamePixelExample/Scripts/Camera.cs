using Microsoft.Xna.Framework;

public class Camera
{
    public static Matrix AbsoluteGameTransformation;
    public static Matrix RelativeGameTransformation;
    public static Matrix ViewportTransformation = Matrix.CreateScale(new Vector3(1, 1, 1));
    
    public static float AbsoluteScreenMultiplier => (float)MyGame.Gameport.Width / (float)CONSTANTS.VIRTUAL_WIDTH;
    public static float RelativeScreenMultiplier => AbsoluteScreenMultiplier;

    public static Thing Target;
    static Vector3 position;
    static Vector2 destination;

    private static void UpdateScaleMatrices()
    {
        RelativeGameTransformation = Matrix.CreateScale(new Vector3(RelativeScreenMultiplier, RelativeScreenMultiplier, 1));
        AbsoluteGameTransformation = Matrix.CreateScale(new Vector3(AbsoluteScreenMultiplier, AbsoluteScreenMultiplier, 1));
    }

    public static void Refresh()
    {
        UpdateScaleMatrices();
    }

    public static void Update()
    {
        if (Target == null) return;

        destination = new Vector2(

            (-(Target.Position.X) * RelativeScreenMultiplier) + (MyGame.Gameport.Width / 2f),
            (-(Target.Position.Y) * RelativeScreenMultiplier) + (MyGame.Gameport.Height / 2f)
        );

        position = new Vector3(
            destination.X,
            destination.Y,
            position.Z
        );

        RelativeGameTransformation.Translation = new Vector3(
            position.X,
            position.Y,
            position.Z
        );
    }
}