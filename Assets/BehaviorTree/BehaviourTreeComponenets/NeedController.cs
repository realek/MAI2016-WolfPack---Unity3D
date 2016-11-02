using UnityEngine;

[RequireComponent(typeof(Needs))]
public class NeedController : MonoBehaviour {

    private Needs _myNeeds;

    /* 
     * placing the constants here instead of in the Game Manager so that we can adjust them for each wolf to give them personalities
     * such as one wolf getting bored easily, another being always hungry and so on
     */
    #region Constants
    [SerializeField] private float _enEdNoSleep = -0.1f;
    [SerializeField] private float _enEdSleep = 0.3f;
    [SerializeField] private float _enEdRun = -0.4f;
    [Space(10)]
    [SerializeField] private float _huEdNoFeedAwake = 0.05f;
    [SerializeField] private float _huEdNoFeedAsleep = 0.03f;
    [SerializeField] private float _huEdFeed = 5f;
    [Space(10)]
    [SerializeField] private float _thEdNoDrinkAwake = 0.07f;
    [SerializeField] private float _thEdNoDrinkAsleep = 0.02f;
    [SerializeField] private float _thEdDrinking = 1.5f;
    [Space(10)]
    [SerializeField] private float _feEdFriendDie = 30f;
    [SerializeField] private float _feEdIsAttacked = 2f;
    [SerializeField] private float _feEdCalm = -0.5f;
    [Space(10)]
    [SerializeField] private float _plEdBored = 2f;
    [SerializeField] private float _plEdPlayFr = -10f;
    [Space(10)]
    [SerializeField] private float _cuEdExplore = -1f;
    [SerializeField] private float _cuEdNoExplore = 0.5f;
    [SerializeField] private float _cuEdFriendDie = -10f;
    #endregion

    [Space(20)]
    #region Adjustments
    [SerializeField] private float _adjFearForLife = 50f; //when health is below this value fear increases twice as fast upon hit
    #endregion

    void Start () {
	    _myNeeds = GetComponent<Needs>();
	}
	
	void Update () {
	    /*
         * wolfState = ReadState();
         * if (wolfState == Sleep) {
         *      Sleeping();
         * }
         * and so on for all the other functions here
         * 
         * 
         */
	}

    #region EnergyRelated

    private void BedTime() {
        //if gametime is between 23:00 and 6:00 and wolf is not in sleep state
        _myNeeds.EditNeed(GameManager.NeedType.Energy, _enEdNoSleep);
    }

    private void Running() {
        //if in run state
        _myNeeds.EditNeed(GameManager.NeedType.Energy, _enEdRun);
    }
    #endregion

    #region HungerRelated

    private void EatingAnimal() {
        //if eating an animal
        _myNeeds.EditNeed(GameManager.NeedType.Hunger, _huEdFeed);
    }

    private void GrowHunger() {
        //if a set amount of time has passed since last food & not in sleep state
        _myNeeds.EditNeed(GameManager.NeedType.Hunger, _huEdNoFeedAwake);
    }
    #endregion

    #region ThirstRelated

    private void GrowThirsty() {
        //if a set amount of time has passed since last drink & not in sleep state
        _myNeeds.EditNeed(GameManager.NeedType.Thirst, _thEdNoDrinkAwake);
    }

    private void DrinkingWater() {
        //if drinking water
        _myNeeds.EditNeed(GameManager.NeedType.Thirst, _thEdDrinking);
    }
    #endregion

    #region FearRelated

    private void HitByAttack() {
        //if being hit by another creature increase fear; if at low health, increase fear by twice the value
        if (_myNeeds.GetNeed(GameManager.NeedType.Health) > _adjFearForLife) {
            _myNeeds.EditNeed(GameManager.NeedType.Fear, _feEdIsAttacked);
        } else {
            _myNeeds.EditNeed(GameManager.NeedType.Fear, 2 * _feEdIsAttacked);
        }
    }

    private void CalmDown() {
        //if no enemy nearby reduce fear
        _myNeeds.EditNeed(GameManager.NeedType.Fear, _feEdCalm);
    }
    #endregion

    #region PlayfulnessRelated

    private void BoredomIncrease() {
        //if not playing, fighting, eating or exploring increase playfulness
        _myNeeds.EditNeed(GameManager.NeedType.Playfulness, _plEdBored);
    }

    private void AttackedFriend() {
        _myNeeds.EditNeed(GameManager.NeedType.Playfulness, _plEdPlayFr);
    }
    #endregion

    #region CuriousityRelated

    private void UnexploredArea() {
        //if not within the recognized area
        _myNeeds.EditNeed(GameManager.NeedType.Curiousity, _cuEdExplore);
    }
    private void ExploredArea() {
        //if not within the recognized area
        _myNeeds.EditNeed(GameManager.NeedType.Curiousity, _cuEdNoExplore);
    }
    #endregion

    #region RelatedToMany

    private void Sleeping() {
        //if in sleep state
        _myNeeds.EditNeed(GameManager.NeedType.Energy, _enEdSleep);
        _myNeeds.EditNeed(GameManager.NeedType.Hunger, _huEdNoFeedAsleep);
        _myNeeds.EditNeed(GameManager.NeedType.Thirst, _thEdNoDrinkAsleep);
    }

    private void SawFriendDie() {
        //if another wolf died nearby
        _myNeeds.EditNeed(GameManager.NeedType.Fear, _feEdFriendDie);
        _myNeeds.EditNeed(GameManager.NeedType.Curiousity, _cuEdFriendDie);
    }
    #endregion

}
