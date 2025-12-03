using UnityEngine;
using UnityEngine.InputSystem;

public class FireCrackerDemo : MonoBehaviour
{
    public PlayerInputActions playerInput;
    private void Awake() {
        playerInput = new PlayerInputActions();
    }
    //...

    public InputAction fire;
    private void OnEnable() {
        fire = playerInput.XRIRightInteraction.Activate;
        fire.Enable();
    }
    private void OnDisable() {
        fire.Disable();
    }
    //...

    public float cooldown = 3, currentCooldown = 0;
    public ParticleSystem firecracker;
    void Update() {
        // 버튼 타입의 인풋 이벤트일 때 --> inputAction.triggered
        // "Trigger"의 아날로그 값 따위의 1D 값을 읽어들일 때 --> inputAction.ReadValue<float>();
        // 조이스틱 등의 2D 값을 읽어들일 때 --> inputAction.ReadValue<Vector2>();
        // ...
        if (fire.triggered && currentCooldown <= 0)
        {
            firecracker.Play();
            currentCooldown = cooldown;
        }
        if (currentCooldown > 0) currentCooldown -= Time.deltaTime;
    }
}
