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
        // ��ư Ÿ���� ��ǲ �̺�Ʈ�� �� --> inputAction.triggered
        // "Trigger"�� �Ƴ��α� �� ������ 1D ���� �о���� �� --> inputAction.ReadValue<float>();
        // ���̽�ƽ ���� 2D ���� �о���� �� --> inputAction.ReadValue<Vector2>();
        // ...
        if (fire.triggered && currentCooldown <= 0)
        {
            firecracker.Play();
            currentCooldown = cooldown;
        }
        if (currentCooldown > 0) currentCooldown -= Time.deltaTime;
    }
}
