using UnityEngine;

[RequireComponent(typeof(Needs))]
public class NeedController : MonoBehaviour {


    [SerializeField] private WolfState _currentState = WolfState.Idle;
    [SerializeField] private WolfState _previousState = WolfState.Idle;

    private DayNightCycler _currTimePeriod;
    private Needs _myNeeds;
    private float _currTime;
    private float _lastDrank;
    private float _lastAte;

    /* 
     * placing the constants here instead of in the Game Manager so that we can adjust them for each wolf to give them personalities
     * such as one wolf getting bored easily, another being always hungry and so on
     */
    #region Constants
    [SerializeField] private float _enEdNoSleep = -0.02f;
    [SerializeField] private float _enEdSleep = 0.2f;
    [SerializeField] private float _enEdRun = -0.04f;
    [SerializeField] private float _enEdIdle = 0.01f;
    [Space(10)]
    [SerializeField] private float _huEdNoFeedAwake = 0.05f;
    [SerializeField] private float _huEdNoFeedAsleep = 0.03f;
    [SerializeField] private float _huEdFeed = -5f;
    [Space(10)]
    [SerializeField] private float _thEdNoDrinkAwake = 0.07f;
    [SerializeField] private float _thEdNoDrinkAsleep = 0.02f;
    [SerializeField] private float _thEdDrinking = -1.5f;
    [Space(10)]
    [SerializeField] private float _feEdFriendDie = 30f;
    [SerializeField] private float _feEdIsAttacked = 2f;
    [SerializeField] private float _feEdCalm = -0.5f;
    [Space(10)]
    [SerializeField] private float _plEdBored = 0.001f;
    [SerializeField] private float _plEdPlayFr = -0.5f;
    [Space(10)]
    [SerializeField] private float _cuEdExplore = -1f;
    [SerializeField] private float _cuEdNoExplore = 0.05f;
    [SerializeField] private float _cuEdFriendDie = -10f;
    [Space(10)]
    [SerializeField] private float _heEdTooHungry = -0.01f;
    #endregion

    [Space(20)]
    #region Adjustments
    [SerializeField] private float _adjFearForLife = 50f; //when health is below this value fear increases twice as fast upon hit
    [SerializeField] private float _timeBeforeGetThirsty = 1f; //when health is below this value fear increases twice as fast upon hit
    [SerializeField] private float _timeBeforeGetHungry = 2.5f; //when health is below this value fear increases twice as fast upon hit
    [SerializeField] private float _bearableHunger = 90f; //when health is below this value fear increases twice as fast upon hit
    #endregion

    void Start () {
	    _myNeeds = GetComponent<Needs>();
        _currTimePeriod = DayNightCycler.Instance;

        _lastDrank = _currTimePeriod.GetTimeStamp();
        _lastAte = _currTimePeriod.GetTimeStamp();
    }

    void Update() {
        _currentState = ReadState();

        CheckStats();

        if (_currentState == WolfState.Dead) return; //add "if distance to player is a lot, destroy game object"

        _currTime = _currTimePeriod.GetTimeStamp();

        if (_currentState == WolfState.Sleep) {
            Sleeping();
        } else {
            if (_currTimePeriod.CurrentTime == DNCycleTime.Morning) {
                BedTime();
            }
            if (_currentState == WolfState.Idle) {
                Idle();
            } else if (_currentState == WolfState.Run) {
                Running();
            }

            if (_currentState == WolfState.Drink) {
                DrinkingWater();
            } else if (_currTime > _lastDrank + _timeBeforeGetThirsty) {
                GrowThirsty();
            }

            if (_currentState == WolfState.Feed) {
                EatingAnimal();
            } else if (_currTime > _lastAte + _timeBeforeGetHungry) {
                GrowHungry();
            }

            if (_currentState == WolfState.Play) {
                PlayingWithFriend();
            } else if (_currentState != WolfState.Fight && _currentState != WolfState.Explore) {
                BoredomIncrease();
            }
        }

        if (_previousState == WolfState.Feed && _currentState != WolfState.Feed) {
            _lastAte = _currTimePeriod.GetTimeStamp();
        } else if (_previousState == WolfState.Drink && _currentState != WolfState.Drink) {
            _lastDrank = _currTimePeriod.GetTimeStamp();
        }

        _previousState = _currentState;
    }

    private WolfState ReadState() {
        if (this.name == "PlayerWolf") return WolfState.Idle; //TODO must be fixed
        return this.GetComponent<SimpleBT>().GetWolfState();
    }

    // checks if any passive stat changes which are not related to the Wolf's current state should happen
    private void CheckStats() {
        if (_myNeeds.GetNeed(WolfNeedType.Hunger) > _bearableHunger) {
            TooHungry();
        }
    }

    #region EnergyRelated

    private void BedTime() {
        //if gametime is between 23:00 and 6:00 and wolf is not in sleep state
        _myNeeds.EditNeed(WolfNeedType.Energy, _enEdNoSleep);
    }

    private void Idle() {
        //if staying in one place
        _myNeeds.EditNeed(WolfNeedType.Energy, _enEdIdle);
    }

    private void Running() {
        //if in run state
        _myNeeds.EditNeed(WolfNeedType.Energy, _enEdRun);
    }
    #endregion

    #region HungerRelated
    private void EatingAnimal() {
        //if eating an animal
        _myNeeds.EditNeed(WolfNeedType.Hunger, _huEdFeed);
    }

    private void GrowHungry() {
        //if a set amount of time has passed since last food & not in sleep state
        _myNeeds.EditNeed(WolfNeedType.Hunger, _huEdNoFeedAwake);
    }
    #endregion

    #region ThirstRelated

    private void GrowThirsty() {
        //if a set amount of time has passed since last drink & not in sleep state
        _myNeeds.EditNeed(WolfNeedType.Thirst, _thEdNoDrinkAwake);
    }

    private void DrinkingWater() {
        //if drinking water
        _myNeeds.EditNeed(WolfNeedType.Thirst, _thEdDrinking);
    }
    #endregion

    #region FearRelated

    private void HitByAttack() {
        //if being hit by another creature increase fear; if at low health, increase fear by twice the value
        if (_myNeeds.GetNeed(WolfNeedType.Health) > _adjFearForLife) {
            _myNeeds.EditNeed(WolfNeedType.Fear, _feEdIsAttacked);
        } else {
            _myNeeds.EditNeed(WolfNeedType.Fear, 2 * _feEdIsAttacked);
        }
    }

    private void CalmDown() {
        //if no enemy nearby reduce fear
        _myNeeds.EditNeed(WolfNeedType.Fear, _feEdCalm);
    }
    #endregion

    #region PlayfulnessRelated

    private void BoredomIncrease() {
        //if not sleeping, playing, fighting or exploring increase playfulness
        _myNeeds.EditNeed(WolfNeedType.Playfulness, _plEdBored);
    }

    private void PlayingWithFriend() {
        //while playing with friend decrease playfulness
        _myNeeds.EditNeed(WolfNeedType.Playfulness, _plEdPlayFr);
    }
    #endregion

    #region CuriousityRelated

    private void UnexploredArea() {
        //if not within the recognized area
        _myNeeds.EditNeed(WolfNeedType.Curiousity, _cuEdExplore);
    }
    private void ExploredArea() {
        //if within the recognized area
        _myNeeds.EditNeed(WolfNeedType.Curiousity, _cuEdNoExplore);
    }
    #endregion

    #region HealthRelated

    private void TooHungry() {
        _myNeeds.EditNeed(WolfNeedType.Health, _heEdTooHungry);
    }

    #endregion

    #region RelatedToMany

    private void Sleeping() {
        //if in sleep state
        _myNeeds.EditNeed(WolfNeedType.Energy, _enEdSleep);
        _myNeeds.EditNeed(WolfNeedType.Hunger, _huEdNoFeedAsleep);
        _myNeeds.EditNeed(WolfNeedType.Thirst, _thEdNoDrinkAsleep);
    }

    private void SawFriendDie() {
        //if another wolf died nearby
        _myNeeds.EditNeed(WolfNeedType.Fear, _feEdFriendDie);
        _myNeeds.EditNeed(WolfNeedType.Curiousity, _cuEdFriendDie);
    }
    #endregion

}
