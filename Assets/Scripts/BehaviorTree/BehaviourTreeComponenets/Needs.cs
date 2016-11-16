using UnityEngine;

public class Needs : MonoBehaviour {

    private const int MIN_VALUE = 0;
    private const int MAX_VALUE = 100;

    [SerializeField] private float _hunger = MIN_VALUE;
    [SerializeField] private float _thirst = MIN_VALUE;
    [SerializeField] private float _playfulness = MIN_VALUE;
    [SerializeField] private float _fear = MIN_VALUE;
    [SerializeField] private float _energy = MAX_VALUE;
    [SerializeField] private float _curiosity = MAX_VALUE;
    [SerializeField] private float _health = MAX_VALUE;

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
            case GameManager.NeedType.Hunger: _hunger += value; if (_hunger < MIN_VALUE) _hunger = MIN_VALUE; if (_hunger > MAX_VALUE) _hunger = MAX_VALUE; break;
            case GameManager.NeedType.Playfulness: _playfulness += value; if (_playfulness < MIN_VALUE) _playfulness = MIN_VALUE; if (_playfulness > MAX_VALUE) _playfulness = MAX_VALUE; break;
            case GameManager.NeedType.Fear: _fear += value; if (_fear < MIN_VALUE) _fear = MIN_VALUE; if (_fear > MAX_VALUE) _fear = MAX_VALUE; break;
            case GameManager.NeedType.Energy: _energy += value; if (_energy < MIN_VALUE) _energy = MIN_VALUE; if (_energy > MAX_VALUE) _energy = MAX_VALUE; break;
            case GameManager.NeedType.Curiousity: _curiosity += value; if (_curiosity < MIN_VALUE) _curiosity = MIN_VALUE; if (_curiosity > MAX_VALUE) _curiosity = MAX_VALUE; break;
            case GameManager.NeedType.Thirst: _thirst += value; if (_thirst < MIN_VALUE) _thirst = MIN_VALUE; if (_thirst > MAX_VALUE) _thirst = MAX_VALUE; break;
            case GameManager.NeedType.Health: _health += value; if (_health < MIN_VALUE) _health = MIN_VALUE; if (_health > MAX_VALUE) _health = MAX_VALUE; break;
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
            default: Debug.Log("Unknown need requested"); return MIN_VALUE;
        }
    }
}
