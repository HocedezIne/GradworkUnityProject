using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Camera playerCamera;

    public int forwardAxis;   // 0 = none, 1 = forward, 2 = backward
    public int rightAxis;     // 0 = none, 1 = right, 2 = left

    public float forwardValue;
    public float rightValue;

    private Vector2 move;

    private int m_InputEvents;
    public int GetInputEventCount() {  return m_InputEvents; }

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();

        if (context.performed) ++m_InputEvents;
    }

    private void Update()
    {
        forwardValue = move.y;
        rightValue = move.x;

        forwardAxis = move.y > 0.1f ? 1 : move.y < -0.1f ? 2 : 0;
        rightAxis = move.x > 0.1f ? 1 : move.x < -0.1f ? 2 : 0;
    }

    public void ResetInputLogging()
    {
        m_InputEvents = 0;
    }
}
