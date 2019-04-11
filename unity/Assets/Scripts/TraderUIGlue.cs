using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TraderUIGlue : MonoBehaviour
{
    public Market.Trader RefTrader;

    public Text TextName;
    public Text TextResources;
    public Text TextCash;
    public Text TextReactors;

    public string Name;
    public string ResourceType;
    public int ResourceCount;
    public float Cash;
    public int Reactors;

    // Update is called once per frame
    void Update()
    {
        Name = RefTrader.Name;
        ResourceType = RefTrader.Commodity.ToString();
        ResourceCount = RefTrader.Amount;
        Cash = RefTrader.Cash;
        Reactors = RefTrader.Reactors;

        TextName.text = "Trader " + Name;
        TextResources.text = ResourceCount + " " + ResourceType;
        TextCash.text = "€" + Cash;
        TextReactors.text = "Reactors " + Reactors;
    }
}
