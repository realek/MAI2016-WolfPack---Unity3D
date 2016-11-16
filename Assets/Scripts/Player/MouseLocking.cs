using UnityEngine;
using UnityEngine.SceneManagement;

public class MouseLocking : MonoBehaviour {

    public static MouseLocking Instance;
    [SerializeField, Tooltip("If true, on game start the mouse will be locked and invisible")] private bool LockedByDefault;

    private int _currentState;
    private int _previousState;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }
    }

    private void Start() {
        if (LockedByDefault) ChangeMouseState(1);
        else ChangeMouseState(2);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.L)) SwitchMouseState();
    }

    public void SwitchMouseState() {
        ChangeMouseState(GetCurrMouseState() == 1 ? 2 : 1);
    }

    public void ChangeMouseState(int newState) {
        _previousState = _currentState;
        _currentState = newState;
        ChangeLock(_currentState);
    }

    public int GetOldMouseState() {
        if (_previousState != 0) return _previousState;
        if (_currentState != 0) return _currentState;
        return 1;
    }

    public int GetCurrMouseState() {
        if (_currentState != 0) return _currentState;
        return 1;
    }

    private void ChangeLock(int newState) {
        if (newState == 1) {
            Cursor.lockState = CursorLockMode.Locked;
        } else if (newState == 2) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        } else {
            Debug.Log("Unknown Mouse state called");
        }
    }
}
