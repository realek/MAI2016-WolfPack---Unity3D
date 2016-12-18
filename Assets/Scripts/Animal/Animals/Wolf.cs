using CustomConsts;
using UnityEngine;

public enum WolfPackRole
{
    None,
    Alpha,
    Beta,
    Omega
}



public class Wolf : Animal {


    [SerializeField]
    private WolfPackRole m_packRole;
    public WolfPackRole packRole
    {
        get
        {
            return m_packRole;
        }
    }

	// Use this for initialization
	private void Awake () {

        Heal();
        m_needs.Initialize(this);

        dmg1 = GlobalVars.WolfAtk1;
        dmg2 = GlobalVars.WolfAtk2;
        dmg3 = GlobalVars.WolfAtk3;
        dmg4 = GlobalVars.WolfAtk4;
        m_strength = AnimalStrength.Strong;
        CarcassQnt = GlobalVars.WolfCarcassQnt;

    }

    private void OnEnable()
    {
        m_needs.Initialize(this);
    }

    private void OnDisable()
    {
        m_needs.Shutdown();
    }

}
