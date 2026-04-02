using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public static readonly string MoveVertical = "Vertical";
    public static readonly string MoveHorizontal = "Horizontal";
    public static readonly string FireButton = "Fire1";

    public float Vertical { get; private set; }
    public float Horizontal { get; private set; }
    public bool Fire { get; private set; }

    // Update is called once per frame
    void Update()
    {
        Vertical = Input.GetAxis(MoveVertical);
        Horizontal = Input.GetAxis(MoveHorizontal); 
        Fire = Input.GetButton(FireButton);
    }
}
