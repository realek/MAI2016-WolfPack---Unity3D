using UnityEngine;

public class WolfInfo : MonoBehaviour {

    public bool RecentlyAte;    //used for heirarchical eating
    public bool RecentlyDrank;    //used for heirarchical eating
    public bool IsMale;         //true - male, false - female
    public bool IsHowling;      //is it currently howling

    public int Type;            //0 - omega, 1 - beta, 2 alpha
    public int PackNumb;        //0 - no pack, 1-2 packs
    public int NearbyFriendly;  //number of friendly wolves nearby
    public int Location;        //0 - neutral ground, 1 - friendly groun, 2 - enemy ground
    public int CurrAge;         //0-2 pups; 3-6 young; 7-death old
    public int MaxAge;          //between 11 and 15 years
    public int Personality;     //0 - undefined yet, 1 - weak, 2 - average, 3 - strong

    public string Task;
    public string Target;

    private void Start() {
        if (Type == 0) Personality = 1;
    }
}
