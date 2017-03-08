using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
public class Ccity  {
    public static Tcityaddons[] addons =
    {
        new Tcityaddons("city",1000,Etilesprite.BUILDINGS_CITY),
        new Tcityaddons("farm",10,Etilesprite.BUILDINGS_IMPROVEMENTS_FARM),
        new Tcityaddons("mine",20,Etilesprite.BUILDINGS_IMPROVEMENTS_MINE),
        new Tcityaddons("resource exploiter",100,Etilesprite.BUILDINGS_IMPROVEMENTS_GENERIC_RESOURCE_EXPLOITATION),
        new Tcityaddons("town hall",100,Etilesprite.BUILDINGS_TOWN_HALL),
        new Tcityaddons("factory",100,Etilesprite.BUILDINGS_FACTORY),
        new Tcityaddons("trading post",100,Etilesprite.BUILDINGS_TRADING_POST),
        new Tcityaddons("barracks",100,Etilesprite.BUILDINGS_BARRACKS),
        new Tcityaddons("market",200,Etilesprite.BUILDINGS_MARKET),
        new Tcityaddons("allotments",200,Etilesprite.BUILDINGS_ALLOTMENTS),
        new Tcityaddons("port and docks",200,Etilesprite.BUILDINGS_PORT_AND_DOCKS),
        new Tcityaddons("guard post",350,Etilesprite.BUILDINGS_GUARD_POST),
        new Tcityaddons("armourer",400,Etilesprite.BUILDINGS_ARMOURER),
        new Tcityaddons("blacksmith",400,Etilesprite.BUILDINGS_BLACKSMITH),
        new Tcityaddons("teleporter",2000,Etilesprite.BUILDINGS_TELEPORTER)
      
    };

    //public static List<string> citynames = new List<string>{
     // "atown","btown","ctown" };
    
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
        "Bastok","Windurst","Jeuno","Adoulin","Norg",
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
    
    public static int numcities;

    public string name;
    public int posx, posy;//loc of city on map for teleport etc.

    //public int stored_production = 0;
    //public int stored_food = 0;

    //store resources
    public int[] stored_resources=new int[14];//hardcoded amount of resources seems dodgy but what can ya do

    public int[] perturnresources = new int[14];
    public yields perturnyields=new yields(0,0,0);

    public List<Cell> influenced = new List<Cell>();

    public int growthrate = 0;

    public Ccity(int x,int y)
    {
        numcities++;
        if (numcities > citynames.Count) name = "No more names!";
        else name = citynames[numcities];
        posx = x;posy = y;
    }

    

}

