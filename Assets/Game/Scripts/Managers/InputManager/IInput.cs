public interface IInput
{
    float GetHorizontalCameraInput();
    float GetVerticalCameraInput();

    float GetHorizontalMovementInput();
    float GetVerticalMovementInput();

    bool IsJumpKeyPressed();
}