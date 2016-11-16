using UnityEngine;
using System.Collections;

public class ModuleTester : MonoBehaviour {

    public GameObject Target;
    public AIDetectionModule DetectionModule;
    public AIMovementModule MovementModule;
    // Use this for initialization
    void Start () {

        DetectionModule.Initialize(this);
        MovementModule.Initialize(this);


    }

    void OnEnable()
    {
        DetectionModule.Initialize(this);
        MovementModule.Initialize(this);
    }
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            MovementModule.Move(Target);
        }
	
	}

    void OnDrawGizmos()
    {
        DetectionModule.DrawGizmos();
        MovementModule.DrawGizmos();
    }

    void OnDisable()
    {
        DetectionModule.Shutdown();
        MovementModule.Shutdown();
    }
}
