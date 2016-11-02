using UnityEngine;

public class Needs : MonoBehaviour {

    [SerializeField] private float _hunger = 0;
    [SerializeField] private float _thirst = 0;
    [SerializeField] private float _playfulness = 0;
    [SerializeField] private float _fear = 0;
    [SerializeField] private float _energy = 100;
    [SerializeField] private float _curiosity = 100;
    [SerializeField] private float _health = 100;

    public void SetNeed(GameManager.NeedType need, float value) {
        switch (need) {
          case GameManager.NeedType.Hunger: _hunger = value; break;
          case GameManager.NeedType.Playfulness: _playfulness = value; break;
          case GameManager.NeedType.Fear: _fear = value; break;
          case GameManager.NeedType.Energy: _energy = value; break;
          case GameManager.NeedType.Curiousity: _curiosity = value; break;
          case GameManager.NeedType.Thirst: _thirst = value; break;
          case GameManager.NeedType.Health: _health = value; break;
          default: Debug.Log("Unknown need set"); break;
        }
    }

    public void EditNeed(GameManager.NeedType need, float value) {
        switch (need) {
            case GameManager.NeedType.Hunger: _hunger += value; if (_hunger < 0) _hunger = 0; break;
            case GameManager.NeedType.Playfulness: _playfulness += value; if (_playfulness < 0) _playfulness = 0; break;
            case GameManager.NeedType.Fear: _fear += value; if (_fear < 0) _fear = 0; break;
            case GameManager.NeedType.Energy: _energy += value; if (_energy < 0) _energy = 0; break;
            case GameManager.NeedType.Curiousity: _curiosity += value; if (_curiosity < 0) _curiosity = 0; break;
            case GameManager.NeedType.Thirst: _thirst += value; if (_thirst < 0) _thirst = 0; break;
            case GameManager.NeedType.Health: _health += value; if (_health < 0) _health = 0; break;
            default: Debug.Log("Unknown need set"); break;
        }
    }

    public float GetNeed(GameManager.NeedType need) {
        switch (need) {
            case GameManager.NeedType.Hunger: return _hunger;
            case GameManager.NeedType.Playfulness: return _playfulness;
            case GameManager.NeedType.Fear: return _fear;
            case GameManager.NeedType.Energy: return _energy;
            case GameManager.NeedType.Curiousity: return _curiosity;
            case GameManager.NeedType.Thirst: return _thirst;
            case GameManager.NeedType.Health: return _health;
            default: Debug.Log("Unknown need requested"); return 0;
        }
    }
}
