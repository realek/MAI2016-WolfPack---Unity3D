using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Helper class to display NavMesh Agent path
/// </summary>
public static class NavMeshPathDisplay
{
    /// <summary>
    /// Draws a gizmo for current path. 
    /// </summary>
    /// <param name="path"></param>
    /// <param name="color"></param>
    public static void DisplayPath(NavMeshPath path, Color color)
    {
        for(int i = 0; i < path.corners.Length-1; i++)
        {
            Gizmos.color = color;
            Gizmos.DrawLine(path.corners[i], path.corners[i + 1]);

        }
        Gizmos.color = Color.white;
    }
}
