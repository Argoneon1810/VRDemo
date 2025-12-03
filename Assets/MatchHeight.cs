using UnityEngine;

public class MatchHeight : MonoBehaviour
{
    public float uprightHeight = 1.6f;
    public float currentHeight = 1.3f;

    void Start()
    {
        if (uprightHeight == 0) return;
        transform.localScale = Vector3.one * (currentHeight / uprightHeight);
    }
}
