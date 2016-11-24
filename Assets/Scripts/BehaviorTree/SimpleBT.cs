using UnityEngine;

public class SimpleBT : MonoBehaviour {

    //private int switchTarget = 0;
   // private AnimalState lastState = AnimalState.Idle;

    //private void Update() {
    //    if (Input.GetKeyDown(KeyCode.H)) {
    //        //when finished will run constantly
    //        Debug.Log("Running BT");
    //        RunBehaviorTree();
    //    }
    //}

    //public string GetTarget() {
    //    switch (switchTarget) {
    //        case 1:
    //            return GameManager.TagName.Water.ToString();
    //        case 2:
    //            return GameManager.TagName.Food.ToString();
    //        case 3:
    //            return GameManager.TagName.RestArea.ToString();
    //        case 4:
    //            return GameManager.TagName.None.ToString();
    //        case 5:
    //            return GameManager.TagName.Wolf.ToString();
    //        default:
    //            return null;
    //    }
    //}

    //public AnimalState GetWolfState() {
    //    return lastState;
    //}

    //private void RunBehaviorTree() {
    //    {
    //        Needs currNeeds = GetComponent<Needs>();

    //        //fake way to make it work
    //        if (currNeeds.GetNeed(GameManager.NeedType.Energy) < 50f) {
    //            if (lastState != GameManager.WolfState.Run && lastState != GameManager.WolfState.Sleep) {
    //                lastState = GameManager.WolfState.Run;
    //                switchTarget = 3;
    //            } else if (GetComponent<WolfBehavoir>().m_currLocation == GameManager.TagName.RestArea.ToString()) {
    //                lastState = GameManager.WolfState.Sleep;
    //                switchTarget = 4;
    //            }
    //        } else if (currNeeds.GetNeed(GameManager.NeedType.Thirst) > 40f && lastState != GameManager.WolfState.Drink) {
    //            //supposedly the wolf first goes to the place and upon arrival enters drink state
    //            lastState = GameManager.WolfState.Drink;
    //            switchTarget = 1;
    //        } else if (currNeeds.GetNeed(GameManager.NeedType.Hunger) > 40f && lastState != GameManager.WolfState.Feed) {
    //            //supposedly the wolf first goes to the place and upon arrival enters eat state
    //            lastState = GameManager.WolfState.Feed;
    //            switchTarget = 2;
    //        } else if (currNeeds.GetNeed(GameManager.NeedType.Energy) > 30 &&
    //                   currNeeds.GetNeed(GameManager.NeedType.Playfulness) > 30 &&
    //                   lastState != GameManager.WolfState.Play) {
    //            //supposedly the wolf first goes to the wolf and upon arrival enters play state
    //            lastState = GameManager.WolfState.Play;
    //            switchTarget = 5;
    //        } else {
    //            lastState = GameManager.WolfState.Idle;
    //            switchTarget = 0;
    //        }
    //    }

    //    //the real way it will work
    //    //switchTarget = btBuilder.ExecuteTree(GetComponent<Needs>());    -- baaakkaaa, it won't work like that ^^
    //}
}
