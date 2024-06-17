using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class MyGame : Game
{
    /// <summary>The inner play area.</summary>
    public static Rectangle Gameport = new Rectangle();
    public static Rectangle StretchedGameport = new Rectangle();

    /// <summary>The actual window the application is running in (minus the title bar).</summary>
    public static Point ViewportDimensions => new Point(
        (int)(Graphics.GraphicsDevice?.Viewport.Width),
        (int)(Graphics.GraphicsDevice?.Viewport.Height)
    );
    /// <summary>
    /// If the Gameport is larger than the Viewport, this will represent how much cutoff there is.
    /// Useful for making sure UI stays on the screen. 
    /// (Represents one side).
    /// </summary>
    public static Point GameportCutoffDimensions = new Point();

    /// <summary>The texture we draw to before drawing to the screen/ main buffer.</summary>
    public static RenderTarget2D GameportRenderTarget;

    /// <summary>Clear color displayed in the Gameport</summary>
    public static Color ClearColor = Color.Gray;

    public static GraphicsDeviceManager Graphics;
    public static SpriteBatch SpriteBatch;

    public static List<Thing> Things = new List<Thing>() { };

    public static bool DidStart { get; private set; }

    // Options

    bool integerScaling = true;
    // 100% == no cutoff allowed
    float cutoffAllowance = 100;

    // Assets

    public static SpriteFont Font;
    public static Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>() { };

    // ...

    public MyGame()
    {
        Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        Camera.Refresh();

        // Don't cap fps when window is not focused.
        InactiveSleepTime = new TimeSpan(0);

        // Consistent game update?
        IsFixedTimeStep = true;

        // FPS
        TargetElapsedTime = TimeSpan.FromSeconds(1d / 60);

        // Graphics

        // VSync
        Graphics.SynchronizeWithVerticalRetrace = true;
        // Dimensions
        Graphics.PreferredBackBufferWidth = CONSTANTS.VIRTUAL_WIDTH * CONSTANTS.DEFAULT_SCREEN_MULTIPLIER;
        Graphics.PreferredBackBufferHeight = CONSTANTS.VIRTUAL_HEIGHT * CONSTANTS.DEFAULT_SCREEN_MULTIPLIER;
        Graphics.ApplyChanges();

        // Window

        Window.AllowUserResizing = true;
        // Window.IsBorderless = DeviceSettings.Instance.IsBorderless;
        Window.Title = "Pixel Example";
        Window.ClientSizeChanged += (object sender, EventArgs args) =>
        {
            if (!Graphics.IsFullScreen)
            {
                ScaleGameport();
            }
        };

        // Setup

        ScaleGameport();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        SpriteBatch = new SpriteBatch(GraphicsDevice);

        Font = this.Content.Load<SpriteFont>("Assets/Fonts/MonogramExtended");
        Textures["Garbanzo"] = Texture2D.FromFile(GraphicsDevice, "Content/Assets/Sprites/Garbanzo.png");
        Textures["GridBlock"] = Texture2D.FromFile(GraphicsDevice, "Content/Assets/Sprites/GridBlock.png");

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if (!DidStart)
        {
            InitializeExample();
            DidStart = true;
        }

        Input.Update();

        Things.ForEach(t =>
        {
            if (!t.DidStart) t.Start();
            t.Update();
        });

        Camera.Update();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        DrawToGamePort();
        DrawToMainBuffer();

        base.Draw(gameTime);
    }

    void DrawThings()
    {
        // Begin Relative Draw (Relative to the world, use this for everything else)
        SpriteBatch.Begin(
            sortMode: SpriteSortMode.Deferred,
            samplerState: SamplerState.PointClamp,
            transformMatrix: Camera.RelativeGameTransformation,
            blendState: BlendState.AlphaBlend
        );

        Things.Where(t => !t.Absolute).ToList().ForEach(t =>
        {
            t.Draw();
        });

        SpriteBatch.End();

        // Begin Absolute Draw (Sticks to Camera, use this for UI stuff)
        SpriteBatch.Begin(
            sortMode: SpriteSortMode.Deferred,
            samplerState: SamplerState.PointClamp,
            transformMatrix: Camera.AbsoluteGameTransformation,
            blendState: BlendState.AlphaBlend
        );

        Things.Where(t => t.Absolute).ToList().ForEach(t =>
        {
            t.Draw();
        });

        SpriteBatch.End();
    }

    void DrawToGamePort()
    {
        Graphics.GraphicsDevice.SetRenderTarget(GameportRenderTarget);
        Graphics.GraphicsDevice.Clear(ClearColor);

        DrawThings();
    }

    void DrawToMainBuffer()
    {
        Graphics.GraphicsDevice.SetRenderTarget(null);
        Graphics.GraphicsDevice.Clear(Color.Black);

        // Integer Scaling (Perfect)
        if (integerScaling)
        {
            SpriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                samplerState: SamplerState.PointClamp,
                transformMatrix: Camera.ViewportTransformation,
                blendState: BlendState.AlphaBlend
            );

            // Draw Gameport to Window
            SpriteBatch.Draw(
                texture: GameportRenderTarget,
                destinationRectangle: new Rectangle(
                    Gameport.X,
                    Gameport.Y,
                    Gameport.Width,
                    Gameport.Height
                ),
                color: Color.White
            );

            SpriteBatch.End();
        }
        // Stretch Scaling (Blurry)
        else
        {
            float factorX = (float)ViewportDimensions.X / (float)Gameport.Width;
            float factorY = (float)ViewportDimensions.Y / (float)Gameport.Height;
            float factor = Math.Min(factorX, factorY);
            StretchedGameport.X = (int)MathF.Round(((ViewportDimensions.X - (Gameport.Width * factor)) / 2f));
            StretchedGameport.Y = (int)MathF.Round(((ViewportDimensions.Y - (Gameport.Height * factor)) / 2f));
            StretchedGameport.Width = (int)MathF.Round((Gameport.Width * factor));
            StretchedGameport.Height = (int)MathF.Round((Gameport.Height * factor));

            SpriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                samplerState: SamplerState.LinearClamp,
                transformMatrix: Camera.ViewportTransformation,
                blendState: BlendState.AlphaBlend
            );

            // Draw gameport to screen
            SpriteBatch.Draw(
                texture: GameportRenderTarget,
                position: new Vector2(StretchedGameport.X, StretchedGameport.Y),
                sourceRectangle: null,
                color: Color.White,
                rotation: 0f,
                origin: new Vector2(0, 0),
                scale: new Vector2(factor, factor),
                effects: SpriteEffects.None,
                layerDepth: 0f
            );

            SpriteBatch.End();
        }
    }

    void ScaleGameport()
    {
        if (integerScaling)
        {
            int factorX = (int)(
                MathF.Floor(ViewportDimensions.X
                    / (CONSTANTS.VIRTUAL_WIDTH * (cutoffAllowance / 100f))
                ));
            int factorY = (int)(
                MathF.Floor(ViewportDimensions.Y
                    / (CONSTANTS.VIRTUAL_HEIGHT * (cutoffAllowance / 100f))
                ));
            int factor = Math.Max(1, Math.Min(factorX, factorY));

            Gameport.Width = CONSTANTS.VIRTUAL_WIDTH * factor;
            Gameport.Height = CONSTANTS.VIRTUAL_HEIGHT * factor;
        }
        else
        {
            float factorX = (float)ViewportDimensions.X / (float)CONSTANTS.VIRTUAL_WIDTH;
            float factorY = (float)ViewportDimensions.Y / (float)CONSTANTS.VIRTUAL_HEIGHT;
            float factor = Math.Max(1, Math.Min(factorX, factorY));

            Gameport.Width = (int)(CONSTANTS.VIRTUAL_WIDTH * Math.Max(1, MathF.Round(factor)));
            Gameport.Height = (int)(CONSTANTS.VIRTUAL_HEIGHT * Math.Max(1, MathF.Round(factor)));
        }

        // Center GamePort in Viewport

        Gameport.X = (int)((ViewportDimensions.X / 2f) - (Gameport.Width / 2f));
        Gameport.Y = (int)((ViewportDimensions.Y / 2f) - (Gameport.Height / 2f));

        // Update GameportCutoffDimensions

        if (integerScaling)
        {
            if (Gameport.X < 0) GameportCutoffDimensions.X = (int)(-Gameport.X / Camera.AbsoluteScreenMultiplier);
            else GameportCutoffDimensions.X = 0;

            if (Gameport.Y < 0) GameportCutoffDimensions.Y = (int)(-Gameport.Y / Camera.AbsoluteScreenMultiplier);
            else GameportCutoffDimensions.Y = 0;
        }
        else
        {
            GameportCutoffDimensions = Point.Zero;
        }

        Graphics.PreferredBackBufferWidth = ViewportDimensions.X;
        Graphics.PreferredBackBufferHeight = ViewportDimensions.Y;

        // Update render target
        if (GameportRenderTarget != null) GameportRenderTarget.Dispose();
        GameportRenderTarget = new RenderTarget2D(
            graphicsDevice: Graphics.GraphicsDevice,
            width: Gameport.Width,
            height: Gameport.Height
        );
        GameportRenderTarget.Name = "GameportRenderTarget";

        StretchedGameport = Gameport;

        Camera.Refresh();
    }

    // Example

    void InitializeExample()
    {
        Character character = new Character(new Vector2(0, -16));
        Camera.Target = character;
        Things.Add(character);
        Things.Add(new Block(1, new Vector2(64, 64)));
        Things.Add(new Block(2, new Vector2(80, 64)));
        Things.Add(new Block(2, new Vector2(96, 64)));
        Things.Add(new Block(2, new Vector2(112, 64)));
        Things.Add(new Block(3, new Vector2(128, 64)));

        Things.Add(new Text("Hello Test.", new Vector2(0, 0)));
        Things.Add(new Text("Use the arrow keys to move!", new Vector2(0, Font.MeasureString("X").Y)));
        Things.Add(new Text("Resize the window, we got integer scaling!", new Vector2(0, Font.MeasureString("X").Y * 2)));

    }
}