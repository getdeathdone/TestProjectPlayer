using UnityEngine;

namespace Prototype.Input
{
    public interface IInputService
    {
        Vector2 Move { get; }
        Vector2 Look { get; }
        bool AimPressed { get; }
        bool FirePressed { get; }
        void SetMove(Vector2 value);
        void SetLook(Vector2 value);
        void SetAimPressed(bool value);
        void SetFirePressed(bool value);
    }
}
