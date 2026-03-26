using Prototype.Config;
using UnityEngine;

namespace Prototype.Input
{
    public class InputService : IInputService
    {
        public Vector2 Move { get; private set; }
        public Vector2 Look { get; private set; }
        public bool AimPressed { get; private set; }
        public bool FirePressed { get; private set; }

        public void SetMove(Vector2 value)
        {
            Move = Vector2.ClampMagnitude(value, GameplayConfig.Movement.MaxInputMagnitude);
        }

        public void SetLook(Vector2 value)
        {
            Look = Vector2.ClampMagnitude(value, GameplayConfig.Movement.MaxInputMagnitude);
        }

        public void SetAimPressed(bool value)
        {
            AimPressed = value;
        }

        public void SetFirePressed(bool value)
        {
            FirePressed = value;
        }
    }
}
