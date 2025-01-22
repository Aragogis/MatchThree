using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GemObjects gemObjects;
    public void StartLevel(LevelData level)
    {
        GameObject gemManager = new GameObject("GemManager");
        gemManager.transform.parent = GameObject.FindWithTag("GameScreen").transform;
        gemManager.transform.position = Vector3.zero;

        gemManager.AddComponent<GemManager>();
        gemManager.AddComponent<GemList>();

        gemManager.GetComponent<GemManager>().level = level;
        gemManager.GetComponent<GemManager>().gemObjects = gemObjects.ToDictionary();
    }

}
