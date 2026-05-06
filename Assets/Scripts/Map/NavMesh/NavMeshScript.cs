using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshScript : MonoBehaviour
{
    [SerializeField]
    NavMeshSurface mesh;
    void Awake()
    {
        if (MapManager.Singleton != null)
        {
            MapManager.Singleton.navSurface = mesh;
        }
        else
        {
            Debug.Assert(false);
        }
    }
}
