using UnityEngine;


/// <summary>
/// Helper class to display NavMesh Agent path
/// </summary>
public static class NavMeshPathDisplay
{
    public static void DisplayPath(NavMeshPath path, Color color)
    {
        for(int i = 0; i < path.corners.Length-1; i++)
        {
            Debug.DrawLine(path.corners[i], path.corners[i + 1], color);

        }
    }
}
