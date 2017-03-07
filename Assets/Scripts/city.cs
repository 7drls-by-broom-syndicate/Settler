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

    public Tcityaddons(string _n, int _cost)
    {
        name = _n;
        cost = _cost;
    }
}
public class Ccity  {
    public static Tcityaddons[] addons =
    {
        new Tcityaddons("city",1000),
        new Tcityaddons("farm",10),new Tcityaddons("mine",20),new Tcityaddons("resource exploiter",100),
        new Tcityaddons("town hall",100),
        new Tcityaddons("factory",100),
        new Tcityaddons("trading post",100),
        new Tcityaddons("barracks",100),
        new Tcityaddons("market",200),
        new Tcityaddons("allotments",200),
        new Tcityaddons("port and docks",200),
        new Tcityaddons("guard post",350),
        new Tcityaddons("armourer",400),
        new Tcityaddons("blacksmith",400),
        new Tcityaddons("teleporter",2000)
      
    };

    static List<string> citynames = new List<string>{
        "Bristletown","Broomsville","Scorbee",
        "Chogalog","Frome","Cottingham","Tunstall",
        "Heslop","Wentworth","Goodricke","Vanbrugh",
        "Langwith","Derwent","Zabra","Moog","Moopshire",
        "Boblington","Pilkington","Ozric","Flicky Lumpton",
        "Rogueton","Roguechester","Rogue Town","Dredmor","Angband",
        "Steam City","Nethack","Korg","Ableton","Oblivion","Skyrim",
        "Boulderdash","Harambe","Bigby","Douglas",
        "Hupple Town","CCD Republic","Thin Walls",
        "DS Creamton","Jabtown","The LN Frontier",
        "Maurogistan", "Ondrasia", "Tanthie on Wye",
        "Ebenopolis","Rsaa Realm","Jdaysia",
        "Nootsville","Tussmehtown", "Seven Day City",
        "Asda","Tesco","Pepsi","Coke","Tesla","Wallaby Stokesville"
        //add moar
    };

    public string name;
    public int posx, posy;//loc of city on map for teleport etc.

    public int stored_production = 0;
    public int stored_food = 0;
    //store resources
    public int[] stored_resources=new int[14];//hardcoded amount of resources seems dodgy but what can ya do

    public int[] perturnresources = new int[14];
    public yields perturnyields=new yields(0,0,0);

    public List<Cell> influenced = new List<Cell>();

}

