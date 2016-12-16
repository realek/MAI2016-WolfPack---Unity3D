using UnityEngine;

public class Perishable : MonoBehaviour {

    private int quantity;
    private float expiration;
    private float creationTime;

    private void Start() {
        creationTime = DayNightCycler.Instance.GetTimeStamp();
    }

    public void SetExpiration(float exp) {
        expiration = exp;
    }

    public void SetQuantity(int qnt) {
        quantity = qnt;
    }

    public void Reduce(int value) {
        quantity -= value;
    }

    public void CheckExpired() {
        if (quantity < 1 || DayNightCycler.Instance.GetTimeStamp() > creationTime + expiration) {
            Destroy(gameObject);
        }
    }
}
