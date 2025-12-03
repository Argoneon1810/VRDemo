using UnityEngine;

public static class MyVector
{
    public static float Dot(this ref Vector3 self, ref Vector3 other)
    {
        return Vector3.Dot(self, other);
    }
}