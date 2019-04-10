using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Market : MonoBehaviour
{
    public enum Commodity {FEO};
    public enum OfferType {buy, sell};

    public class Offer {
        public Commodity Commodity;
        public int Amount;
        public float Price;
        public OfferType Type;
        public Trader Trader;

        public Offer (Commodity commodity, int amount, float price, OfferType type, Trader trader) {
            Commodity = commodity;
            Amount = amount;
            Price = price;
            Type = type;
            Trader = trader;
        }

        public override string ToString () {
            string verb = (Type == OfferType.buy) ? "buying" : "selling";
            return string.Concat("Trader ", Trader.Name, " is ", verb, " ", Amount, " ", Commodity, " for ", Price);
        }
    }

    List<Offer> Supply = new List<Offer>();
    List<Offer> Demand = new List<Offer>();

    public class Trader {
        public Commodity Commodity;
        public int Amount;
        public int Cash;
        public string Name;

        public Trader (Commodity commodity, int amount, int cash, string name) {
            Commodity = commodity;
            Amount = amount;
            Cash = cash;
            Name = name;
        }
    }

    List<Trader> Traders = new List<Trader>();

    public void Register(Offer offer) {
        if(offer.Type == OfferType.buy) {
            Demand.Add(offer);
        }
        if (offer.Type == OfferType.sell) {
            Supply.Add(offer);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i <= 9; i++) {
            int a = Random.Range(10, 20);
            int c = Random.Range(50, 150);
            Trader t = new Trader(Commodity.FEO, a, c, i.ToString());
            Traders.Add(t);
            float p = Random.Range(1.2f, 2.4f);
            int am = Random.Range(1, a);
            OfferType type = (i % 2 == 0) ? OfferType.buy : OfferType.sell;
            Offer o = new Offer(t.Commodity, am, p, type, t);
            Register(o);
        }

        DisplayMarketStatus();
    }

    void DisplayMarketStatus() {

        print("Supply");
        Supply.Sort((a, b) => (b.Price.CompareTo(a.Price)));
        foreach (Offer o in Supply) {
            print(o.ToString());
        }

        print("Demand");
        Demand.Sort((a, b) => (b.Price.CompareTo(a.Price)));
        foreach (Offer o in Demand) {
            print(o.ToString());
        }

    }
}
