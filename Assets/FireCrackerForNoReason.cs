using UnityEngine;
using UnityEngine.InputSystem;

public class FireCrackerForNoReason : MonoBehaviour
{
    public PlayerInputActions playerInput;
    public InputAction fire;
    public float cooldown = 3, currentCooldown = 0;
    public ParticleSystem firecracker;

    private void Awake()
    {
        playerInput = new PlayerInputActions();
    }

    private void OnEnable()
    {
        fire = playerInput.XRIRightInteraction.Activate;
        fire.Enable();
    }

    private void OnDisable()
    {
        fire.Disable();
    }

    void Update()
    {
        if(fire.triggered && currentCooldown <= 0)
        {
            firecracker.Play();
            currentCooldown = cooldown;
        }
        if (currentCooldown > 0) currentCooldown -= Time.deltaTime;
    }
}
