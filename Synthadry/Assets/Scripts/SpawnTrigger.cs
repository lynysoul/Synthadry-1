using UnityEngine;

public class SpawnTrigger : MonoBehaviour
{
    public GameObject mobPrefab; 
    public Transform[] spawnPoints; 

    private bool isActive = true; 
    private void OnTriggerEnter(Collider other)
    {

        if (!isActive) return; 

        if (other.CompareTag("Player"))
        {
            foreach (Transform point in spawnPoints)
            {
                Instantiate(mobPrefab, point.position, point.rotation);
            }
            isActive = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (spawnPoints != null)
        {
            Gizmos.color = Color.red;
            foreach (Transform t in spawnPoints)
            {
                Gizmos.DrawSphere(t.position, 0.5f);
            }
        }
    }
}
