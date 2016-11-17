using UnityEngine;
using System.Collections;

public class SimpleBT : MonoBehaviour {

    private bool switchTarget = false;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.H)) {
            switchTarget = true;
        }
    }

    public string GetTarget() {
        if (switchTarget) {
            switchTarget = false;
            return "Water";
        }
        return null;
    }
}
