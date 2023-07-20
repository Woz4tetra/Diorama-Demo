using UnityEngine;

public class MovementVector
{
    public float horizontal;
    public float vertical;
    public static MovementVector zero = new MovementVector(0, 0);

    public MovementVector(float horizontal, float vertical)
    {
        this.horizontal = horizontal;
        this.vertical = vertical;
    }

    public float magnitude()
    {
        return Mathf.Sqrt(horizontal * horizontal + vertical * vertical);
    }
}
