using UnityEngine;

public static class MyPlane
{
    public static Vector3 GetPoint(this ref Plane self) => self.distance * -self.normal;
}