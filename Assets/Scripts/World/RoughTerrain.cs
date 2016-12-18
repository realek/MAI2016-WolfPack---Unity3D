using UnityEngine;
using UnityEngine.AI;

public class RoughTerrain : MonoBehaviour {

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Prey") {
            other.gameObject.GetComponent<NavMeshAgent>().speed = 2f;
            other.gameObject.GetComponent<Needs>().ModNeed(NeedType.Energy, -10f);
        }
    }
    private void OnCollisionExit(Collision other) {
        if (other.gameObject.tag == "Prey") {
            other.gameObject.GetComponent<NavMeshAgent>().speed = 3.5f;
        }
    }
}
