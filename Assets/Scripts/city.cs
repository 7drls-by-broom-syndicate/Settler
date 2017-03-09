using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//"Bremen","Hamburg","Lübeck","Franfurt","Geneva","Kraków","Fiume","Danzig","Shanghai","Tangier","Memel","Trieste","Jerusalem","Monaco","Singapore","Vatican City","Hong Kong","Gibraltar","Guanabara","Macau","Ceuta","Melilla"

    //class for your city, city parts, city states and barbarian camps and citadel
    //and tile improvements 

//public class building
//{

//}

   

public enum Ebuildables {
    city,
    farm, mine, exploit,
    townhall, factory, tradingpost, barracks,
    market, allotments, portanddocks,guardpost,
    armourer,blacksmith,  teleporter}

public class Tcityaddons
{
    public string name;
    public int cost;
    public Etilesprite tile;

    public Tcityaddons(string _n, int _cost,Etilesprite _tile)
    {
        name = _n;
        cost = _cost;
        tile = _tile;
    }
}

public class addoninstance
{
    public Tcityaddons type;
    public int storedproduction;
    public mobarchetype mobtoproduce;

}

public class Ccity  {

    public int arbitrary_growth_value = 100;
    public int arbitrary_growth_value_add = 100;
    public int arbitrary_growth_value_add_add = 100;

    public static Tcityaddons[] addons =
    {
        new Tcityaddons("city",1000,Etilesprite.BUILDINGS_CITY),
        new Tcityaddons("farm",10,Etilesprite.BUILDINGS_IMPROVEMENTS_FARM),
        new Tcityaddons("mine",20,Etilesprite.BUILDINGS_IMPROVEMENTS_MINE),
        new Tcityaddons("resource exploiter",100,Etilesprite.BUILDINGS_IMPROVEMENTS_GENERIC_RESOURCE_EXPLOITATION),

        new Tcityaddons("town hall",100,Etilesprite.BUILDINGS_TOWN_HALL),//makes city grow faster
        new Tcityaddons("factory",100,Etilesprite.BUILDINGS_FACTORY),//10 production
        new Tcityaddons("trading post",100,Etilesprite.BUILDINGS_TRADING_POST),
        new Tcityaddons("barracks",100,Etilesprite.BUILDINGS_BARRACKS),//produces units
        new Tcityaddons("market",200,Etilesprite.BUILDINGS_MARKET),//10 gold
        new Tcityaddons("allotments",200,Etilesprite.BUILDINGS_ALLOTMENTS),//10 food
        new Tcityaddons("port and docks",200,Etilesprite.BUILDINGS_PORT_AND_DOCKS),
        new Tcityaddons("guard post",350,Etilesprite.BUILDINGS_GUARD_POST),//ups city defence
        new Tcityaddons("armourer",400,Etilesprite.BUILDINGS_ARMOURER),//+1 def for units produced
        new Tcityaddons("blacksmith",400,Etilesprite.BUILDINGS_BLACKSMITH),//+1 atk for units produced
        new Tcityaddons("teleporter",2000,Etilesprite.BUILDINGS_TELEPORTER)//telport between cities
      
    };

    public List<addoninstance> thiscitysaddons=new List<addoninstance>();

 

    public static List<string> citynamesevil = new List<string>
    {
        "Scab","Fester","Deathly","Ooze","Rattle",
        "Terminus","Panic","Stab","Knife","Jagged","Black",
        "Stale","Crusty","Weeping","Dark","Spike","Flail","Blade's Edge",
        "Crush","Peel","Torture","Pain","Rape","Suicide","Abortion",
        "Murder","Crime","Punishment","Sin","Greed","Gluttony","Disease",
        "Pallor","Failure","Depression","Infanticide","Bully","Jeer","Leer",
        "Puncture","Penetration","Slice","Hack","Chop","Slash","Acidbath"
    };


    public static List<string> citynames = new List<string>{
        "Bristletown","Broomsville","Scorbee","Spunkton",
        "Chogalog","Frome","Cottingham","Tunstall",
        "Heslop","Wentworth","Goodricke","Vanbrugh",
        "Langwith","Derwent","Zabra","Moog","Moopshire",
        "Boblington","Pilkington","Ozric","Flicky Lumpton",
        "Rogueton","Roguechester","Rogue Town","Dredmor","Angband",
        "Steam City","Nethack","Korg","Ableton","Oblivion","Skyrim",
        "Boulderdash","Harambe","Bigby","Douglas",
        "Hupple Town","CCD Republic","Thin Walls",
        "DS Creamton","Jabtown","The LN Frontier",
        "Maurogistan", "Ondrasia", "Tanthie on Wye","Tenhiville",
        "Ebenopolis","Rsaa Realm","Jdaysia","Oddmundia",
        "Nootsville","Tussmehtown", "Seven Day City",
        "Asda","Tesco","Pepsi","Coke","Tesla","Wallaby Stokesville",
        "Sinus","Cobble Bunton","Smacktown","Ipod","Ipad","Iphone","Android",
        "Gervais","Merchant","Roland","Playstation","XBox","Nintendo","Microsoft",
        "Sony","Square Enix","Final Fantasy","Stormwind","Ironforge","San d'Oria",
        "Bastok","Windurst","Jeuno","Adoulin","Norg","Ottertown","Rogue Basin",
        "Lisp","Cobol","Kobold","Slashie","Darkgod","Katana","Schwackabee",

        //austria minus ones with accents
        "Vienna","Graz","Linz","Salzburg","Innsbruck","Klagenfurt","Villach","Wels","Dornbirn",
"Wiener Neustadt","Steyr","Feldkirch","Bregenz","Leonding","Klosterneuburg",
"Baden bei Wien","Wolfsberg","Leoben","Krems","Traun","Amstetten","Lustenau",
"Kapfenberg","Hallein","Kufstein","Traiskirchen","Schwechat","Braunau am Inn",
"Stockerau","Saalfelden","Ansfelden","Tulln","Hohenems","Spittal an der Drau",
"Telfs","Ternitz","Perchtoldsdorf","Feldkirchen","Bludenz","Bad Ischl","Eisenstadt",
"Schwaz","Hall in Tirol","Gmunden","Wals-Siezenheim","Marchtrenk","Bruck an der Mur",
"Sankt Veit an der Glan","Korneuburg","Neunkirchen","Hard","Lienz","Rankweil","Hollabrunn",
"Enns","Brunn am Gebirge","Ried im Innkreis","Waidhofen","Knittelfeld","Trofaiach",
"Mistelbach","Zwettl","Sankt Johann im Pongau","Gerasdorf bei Wien","Ebreichsdorf",
"Bischofshofen","Seekirchen am Wallersee",
//england - "st. something"s
"Aberdeen","Armagh","Bangor","Bath","Belfast","Birmingham","Bradford",
"Brighton","Bristol","Cambridge","Canterbury","Cardiff","Carlisle",
"Chelmsford","Chester","Chichester","Coventry","Derby","Derry",
"Dundee","Durham","Edinburgh","Ely","Exeter","Glasgow","Gloucester",
"Hereford","Inverness","Hull","Lancaster","Leeds",
"Leicester","Lichfield","Lincoln","Lisburn","Liverpool","London",
"Manchester","Newcastle upon Tyne","Newport","Newry","Norwich",
"Nottingham","Oxford","Perth","Peterborough","Plymouth","Portsmouth",
"Preston4","Ripon","Salford","Salisbury","Sheffield","Southampton",
"Stirling","Stoke-on-Trent","Sunderland","Swansea","Truro","Wakefield",
"Wells","Westminster","Winchester","Wolverhampton","Worcester","York"
    };

    public bool isfrenzleecity;

    public List<mob> unitlist;
    public int armycostperturn_food=0;
    public int armycostperturn_gold = 0;

    public static int numcities;

    public string name;
    public int posx, posy;//loc of city on map for teleport etc.

    //public int stored_production = 0;
    public int stored_food = 0;

    //store resources
    public int[] stored_resources=new int[14];//hardcoded amount of resources seems dodgy but what can ya do

    public int[] perturnresources = new int[14];
    public yields perturnyields=new yields(0,0,0);

    public List<Cell> influenced = new List<Cell>();

    public bool growthboost = false;
    public int growthcounter=0;

    RLMap map;Player player;
    MessageLog log;

    public int evilgold;//enemy cities don't spend gold. instead they accumulate it and it's given to player when they kill it

    public int hp = 100;
    public int defence = 2;
    public static int numcitiesevil=0;

    public Spiral spiral;


    public Ccity(bool frenz,int x,int y,RLMap m,Player p,MessageLog ml=null)

    {
        //copy the static list of distances 
        spiral = new Spiral(m.width, m.height,x,y);


        unitlist = new List<mob>();

        isfrenzleecity = frenz;
        if(ml!=null)log = ml;
        player = p;

        if (isfrenzleecity)
        {
            numcities++;
            if (numcities > citynames.Count) name = "No more names!";
            else name = citynames[numcities];
        }
        else
        {
            numcitiesevil++;
            if (numcitiesevil > citynamesevil.Count) name = "!";
            else name = citynamesevil[numcitiesevil];
        }
        posx = x;posy = y;
        map = m;


        map.citylist.Add(this);
        map.citythathasinfluence[player.posx, player.posy] = this;
        //log.Printline("The city of " + citeh.name + " was founded!", Color.magenta);

        //influence
        grabinitialsquares();
        //set up initial yields
        //recalcyield();

    }

   public void recalcyield()
    {
        //add up the "currentyield" in each square this city owns
        //currentyield is the base yield of the tile plus any improvements like worked resources, mines or farms
        yields y = new yields(0, 0, 0);
        foreach (var x in influenced)
        {
            y.add(map.currentyield[x.x, x.y]);
        }
       // Debug.Log("yield now " + y.production + " " + y.gold + " " + y.food);
        perturnyields = y;
    }



    public void takeaturn()
    {
        //we need to do the yields and the resources and the growth

        //stored resources  
        for(int i = 0; i < 14; i++)
        {
            stored_resources[i] += perturnresources[i];
        }

        //gold goes to standing army. any left over goes to player. evil city: all gold goes to pot for player when they defeat it

        int GOLD = perturnyields.gold;                     //let g be the amount of gold we have on hand 
        if (GOLD >= armycostperturn_gold)                  //if the gold bill for standing army is exactly what we have or less....
        {
            GOLD -= armycostperturn_gold;                  
            if (isfrenzleecity) player.gold += GOLD;       //if it's a player city, give excess gold to player
            else evilgold += GOLD;                         //if enemy city, gold goes to pot for when player kills the city
        }
        else                                            //we can't afford all the units that are currently out
        {
            bool bust = false;
            foreach(var u in unitlist)
            {
                GOLD -= u.archetype.upkeepgold;
                if(GOLD<0 || bust)
                {
                    bust = true;
                    if (log != null) log.Printline(u.archetype.name + " leaves: " + name + " can't pay.", Color.red);
                    armycostperturn_gold -= u.archetype.upkeepgold;
                    armycostperturn_food -= u.archetype.upkeepfood;//remove the cost in both food and gold when mob leaves
                    map.killoffamob(u);//mob leaves
                   
                }
                unitlist.RemoveAll(x => x.tile == Etilesprite.EMPTY);//remove mobs that left from the unitlist for this city
            }

        }

        //food goes to standing army. any left over makes city grow!

        int FOOD = perturnyields.food;                     //let f be the amount of food we have on hand
        if (FOOD >= armycostperturn_food)                  //if the food bill for standing army is exactly what we have or less....
        {
            FOOD -= armycostperturn_food;                   //take the food for the army
            if (growthboost) FOOD= (int)((float)FOOD * 1.25);//multiply excess food if you have growthboost addon
            growthcounter += FOOD;

            if (growthcounter > arbitrary_growth_value)
            {//now grow the city
                growthcounter -= arbitrary_growth_value;
                arbitrary_growth_value += arbitrary_growth_value_add;
                arbitrary_growth_value_add += arbitrary_growth_value_add_add;
                foreach (var tty in spiral.l)
                {
                    int tx = tty.c.x;
                    int ty = tty.c.y;
                    //   if (tx < 0 || ty < 0 || tx >= map.width || ty >= map.height) tty.flagfordeletion = true;
                    // else
                    //{//co-ord is on map
                    bool? b = map.influence[tx, ty];
                    if (b != null || b == !isfrenzleecity) tty.flagfordeletion = true;//remove cells that are enemy held or already held by us (where enemy of enemy is player)
                    else
                    {
                        grabsquare(tx, ty);

                        break;
                    }
                    // }

                }
            }

        }
        else                                            //we can't afford all the units that are currently out
        {
            bool bust = false;
            foreach (var u in unitlist)
            {
                FOOD -= u.archetype.upkeepfood;
                if (FOOD < 0 || bust)
                {
                    bust = true;
                    if (log != null) log.Printline(u.archetype.name + " leaves: " + name + " can't feed.", Color.red);
                    armycostperturn_gold -= u.archetype.upkeepgold;
                    armycostperturn_food -= u.archetype.upkeepfood;
                    map.killoffamob(u);//mob leaves

                }
                unitlist.RemoveAll(x => x.tile == Etilesprite.EMPTY);//remove mobs that left from the unitlist for this city
            }

        }


        //production goes to barracks (and trader?)



    }
    public void grabsquare(int eggs, int why)
    {
        perturnyields.add(map.currentyield[eggs, why]);
        map.influence[eggs,why] = isfrenzleecity;
        influenced.Add(new Cell(eggs,why));
        map.citythathasinfluence[eggs,why] = this;
    }
    public void grabinitialsquares()
    {
        for (int zz = -1; zz < 2; zz++)
        {
            for (int ff = -1; ff < 2; ff++)
            {
                int tx =posx + zz;
                int ty = posy + ff;
                if (tx > 0 && ty > 0 && tx < map.width && ty < map.height)
                {
                    if (map.influence[tx, ty] == null)
                    {
                        grabsquare(tx, ty);
                    }
                }
            }
        }
    }


}

