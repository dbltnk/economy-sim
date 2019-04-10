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
        public float Cash;
        public string Name;

        public Trader (Commodity commodity, int amount, float cash, string name) {
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

    bool allMatched = false;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i <= 9; i++) {
            int a = Random.Range(10, 20);
            float c = Random.Range(50f, 150f);
            Trader t = new Trader(Commodity.FEO, a, c, i.ToString());
            Traders.Add(t);
            float p = Random.Range(1.2f, 2.4f);
            int am = Random.Range(1, a);
            OfferType type = (i % 2 == 0) ? OfferType.buy : OfferType.sell;
            Offer o = new Offer(t.Commodity, am, p, type, t);
            Register(o);
        }

        print("MARKET OPENS");
        if (allMatched == false) {
            DisplayMarketStatus();
            MatchOffers();
        }
        print("MARKET CLOSES");
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
    
    void CleanOffers () {
        for (int i = 0; i <= Supply.Count-1; i++) {
            if (Supply[i].Amount <= 0) Supply.RemoveAt(i);
        }

        for (int i = 0; i <= Demand.Count-1; i++) {
            if (Demand[i].Amount <= 0) Demand.RemoveAt(i);
        }
    }

    void MatchOffers() {
        // REMOVE ALL OFFERS WITH 0 amount
        CleanOffers();
        // GET THE MOST EXPENSIVE DEMAND
        Demand.Sort((a, b) => (b.Price.CompareTo(a.Price)));
        Offer offerDemand = Demand[0];
        // GET THE CHEAPEST SUPPLY
        Supply.Sort((a, b) => (b.Price.CompareTo(a.Price)));
        int l = Supply.Count;
        Offer offerSupply = Supply[l-1];
        // IF S.price > D.price END
        if (offerSupply.Price > offerDemand.Price) {
            allMatched = true;
            return;
        }
        // HOW MUCH CAN WE FULFILL?
        int amountToFulfill = Mathf.Min(offerSupply.Amount, offerDemand.Amount);
        // FOR HOW MUCH CASH?
        float totalPrice = amountToFulfill * offerSupply.Price;
        // REMOVE AMOUNT FROM SUPPLY
        offerSupply.Trader.Amount -= amountToFulfill;
        offerSupply.Amount -= amountToFulfill;
        // ADD AMOUNT TO DEMANDER
        offerDemand.Trader.Amount += amountToFulfill;
        offerDemand.Amount -= amountToFulfill;
        // REMOVE CASH from DEMANDER
        offerDemand.Trader.Cash -= totalPrice;
        // ADD CASH TO SUPPLIER
        offerSupply.Trader.Cash += totalPrice;
        // DO IT AGAIN
        print(string.Concat("Trader ", offerSupply.Trader.Name, " sold ", amountToFulfill, " ", offerDemand.Commodity, " for ", offerSupply.Price, " each to trader ", offerDemand.Trader.Name, "."));
        MatchOffers();
    }
}
