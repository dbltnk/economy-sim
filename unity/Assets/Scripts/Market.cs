using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Market : MonoBehaviour

{
    public int turn;
    public void NextTurn () {
        turn++;
        DoTurn();
    }

    public int TraderAmount;

    public enum Commodity {FEO, C, O, FE};
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

    List<Offer> TotalSupply = new List<Offer>();
    List<Offer> TotalDemand = new List<Offer>();

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

        public override string ToString () {
            return string.Concat("Trader ", Name, " has ", Amount, " ", Commodity, " and ", Cash, " cash.");
        }
    }

    List<Trader> Traders = new List<Trader>();

    public class ResourceMarket {
        public Commodity Commodity;
        public List<Offer> Supply;
        public List<Offer> Demand;
        public bool AllMatched;

        public ResourceMarket (Commodity commodity) {
            Commodity = commodity;
            Supply = new List<Offer>();
            Demand = new List<Offer>();
            AllMatched = false;
        }
    }

    List<ResourceMarket> ResourceMarkets = new List<ResourceMarket>();

    public void Register(Offer offer) {
        if(offer.Type == OfferType.buy) {
            TotalDemand.Add(offer);
        }
        if (offer.Type == OfferType.sell) {
            TotalSupply.Add(offer);
        }
    }

    Commodity GetRandomResource () {
        int count = System.Enum.GetValues(typeof(Commodity)).Length;
        int value = Random.Range(0,count);
        Commodity c = (Commodity)value;
        return c;
    }

    // Start is called before the first frame update
    void Start() {
        SetupTraders();
        SetupMarkets();
    }

    private void SetupMarkets () {
        // DETERMINE HOW MANY MARKETS WE NEED
        int count = System.Enum.GetValues(typeof(Commodity)).Length;
        for (int i = 0; i <= count - 1; i++) {
            // SPLIT INTO SEPERATE MARKETS
            ResourceMarket m = new ResourceMarket((Commodity)i);
            ResourceMarkets.Add(m);
        }
    }

    private void SetupTraders () {
        // SETUP SOME TRADERS
        print("TRADERS");
        for (int i = 0; i <= TraderAmount - 1; i++) {
            int a = Random.Range(10, 20);
            float c = Random.Range(50f, 150f);
            var comm = GetRandomResource();
            Trader t = new Trader(comm, a, c, i.ToString());
            print(t.ToString());
            Traders.Add(t);
            float p = Random.Range(1.2f, 2.4f);
            int am = Random.Range(1, a);
            OfferType type = (i % 2 == 0) ? OfferType.buy : OfferType.sell;
            Offer o = new Offer(t.Commodity, am, p, type, t);
            Register(o);
        }
    }

    void DoTurn () {
        print("MARKET OPENS");

        foreach (ResourceMarket market in ResourceMarkets) {

            print(market.Commodity.ToString());

            for (int i = 0; i <= TotalSupply.Count - 1; i++) {
                if (TotalSupply[i].Commodity == market.Commodity) {
                    market.Supply.Add(TotalSupply[i]);
                }
            }
            for (int i = 0; i <= TotalDemand.Count - 1; i++) {
                if (TotalDemand[i].Commodity == market.Commodity) {
                    market.Demand.Add(TotalDemand[i]);
                }
            }
            // MATCH EACH MARKET SEPERATELY
            print("BEFORE TRADE");
            if (market.AllMatched == false) {
                DisplayMarketStatus(market);
                print("TRADES");
                MatchOffers(market);
            }
            print("AFTER TRADE");
            DisplayMarketStatus(market);
        }

        print("MARKET CLOSES");
    }

    void DisplayMarketStatus(ResourceMarket market) {

        print("SUPPLY");
        market.Supply.Sort((a, b) => (b.Price.CompareTo(a.Price)));
        foreach (Offer o in market.Supply) {
            print(o.ToString());
        }

        print("DEMAND");
        market.Demand.Sort((a, b) => (b.Price.CompareTo(a.Price)));
        foreach (Offer o in market.Demand) {
            print(o.ToString());
        }

    }
    
    void CleanOffers (ResourceMarket market) {
        for (int i = 0; i <= market.Supply.Count-1; i++) {
            if (market.Supply[i].Amount <= 0) market.Supply.RemoveAt(i);
        }

        for (int i = 0; i <= market.Demand.Count-1; i++) {
            if (market.Demand[i].Amount <= 0) market.Demand.RemoveAt(i);
        }
    }

    void MatchOffers(ResourceMarket market) {
        // REMOVE ALL OFFERS WITH 0 amount
        CleanOffers(market);
        // STOP IF THERE IS EITHER NO SUPPLY OR DEMAND TO MATCH
        if (market.Demand.Count == 0 || market.Supply.Count == 0) {
            market.AllMatched = true;
            return;
        }
        // GET THE MOST EXPENSIVE DEMAND
        market.Demand.Sort((a, b) => (b.Price.CompareTo(a.Price)));
        Offer offerDemand = market.Demand[0];
        // GET THE CHEAPEST SUPPLY
        market.Supply.Sort((a, b) => (b.Price.CompareTo(a.Price)));
        int l = market.Supply.Count;
        Offer offerSupply = market.Supply[l-1];
        // IF S.price > D.price END
        if (offerSupply.Price > offerDemand.Price) {
            market.AllMatched = true;
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
        MatchOffers(market);
    }
}
