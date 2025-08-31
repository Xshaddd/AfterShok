using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    GameObject player;
    [SerializeField] Transform spawnPoint;

    void Start()
    {

    }

    void Update()
    {
        
    }

    void SpawnCharacter(string prefabPath = "Prefabs/Player", string name = "Player")
    {
        foreach (GameObject child in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (child.name == name)
            {
                Destroy(child);
                break;
            }
        }

        GameObject charPrefab = Resources.Load<GameObject>(prefabPath);
        if (spawnPoint != null && charPrefab != null)
        {
            player = Instantiate(charPrefab, spawnPoint.position, spawnPoint.rotation);
            player.name = name;
            Debug.Log($"[GameManager] Spawned new character with name {player.name}");
        }
        else
        {
            Debug.LogError("[GameManager] SpawnPoint or Player prefab is null.");
        }
    }
}
