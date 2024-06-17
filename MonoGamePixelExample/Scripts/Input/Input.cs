using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

public static class Input
{
    public static List<InputState> KeyStates = new List<InputState>();
    
    public static void Update()
    {
        KeyboardState keyboardState = Keyboard.GetState();
        Keys[] currentKeys = keyboardState.GetPressedKeys();

        // Iterate over keys being pressed
        // Deal with "pressed" and "held."
        foreach (Keys key in currentKeys)
        {
            if (keyboardState.IsKeyDown(key))
            {
                // New Input
                if (!KeyStates.Any(i => i.Name == key.ToString()))
                {
                    KeyStates.Add(new InputState()
                    {
                        Name = key.ToString(),
                        State = InputStateType.Pressed
                    });
                }
                // Existing Input
                else
                {
                    foreach (InputState existingKey in KeyStates.Where(i => i.Name == key.ToString()))
                    {
                        existingKey.State = InputStateType.Held;
                    }
                }
            }
        }

        KeyStates = KeyStates.Where(i => i.State != InputStateType.Released).ToList();

        // Deal with "on released."
        foreach (InputState input in KeyStates)
        {
            // Current inputs aren't being pressed.
            if (!currentKeys.Any(k => k.ToString() == input.Name))
            {
                input.State = InputStateType.Released;
            }
        }
    }

    public static bool IsPressed(string name)
    {
        return KeyStates.Any(i => i.Name == name && i.State == InputStateType.Pressed);
    }

    // Heheh
    private static bool _IsHeld(string name)
    {
        return KeyStates.Any(i => i.Name == name && i.State == InputStateType.Held);
    }

    public static bool IsHeld(string name)
    {
        bool result = false;

        result = IsPressed(name) || _IsHeld(name);

        return result;
    }

    public static bool IsReleased(string name)
    {
        return KeyStates.Any(i => i.Name == name && i.State == InputStateType.Released);
    }
}