using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGlue : MonoBehaviour

{
    public Market Market;
    public Text Text;
    public GameObject PrefTrader;
    public GameObject PanelTraders;

    public void CreateTraders() {
        foreach (Market.Trader trader in Market.Traders) {
            GameObject go = Instantiate(PrefTrader, PanelTraders.transform);
            TraderUIGlue ui = go.GetComponent<TraderUIGlue>();
            ui.RefTrader = trader;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Text.text = Market.turn.ToString();
    }
}
