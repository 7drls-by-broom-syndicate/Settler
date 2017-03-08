using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tresource {

    public yields yieldwhenworked;
    public string name;
    public string explain;
    public string nameofexploiter;
    public Etilesprite tile;
    public bool destroyedbybuilding;
    public Eresourcetype ert;

    public Tresource( Eresourcetype _ert,string _name,int _p, int _g, int _f, string _exploit, Etilesprite _tile,string _explain,bool destroyed)
    {
        ert = _ert;
        yieldwhenworked = new yields(_p, _g, _f);
        name = _name;
        nameofexploiter = _exploit;
        tile = _tile;
        explain = _explain;
        destroyedbybuilding = destroyed;
    }

}

public enum Eresourcetype { leather,oranges,sheep,coffee,gems,oil,copper,silver, gold,tropical_plants,fish,honey,iron,horses}

public class Cresource
{
    public static Tresource[] resourcetypes =
    {
    new Tresource(Eresourcetype.leather,"leather",3,0,0,"leatherworks",Etilesprite.RESOURCE_LEATHER,"Leather for many things, e.g. thongs.",true),
    new Tresource(Eresourcetype.oranges,"oranges",0,0,3,"citrus grove",Etilesprite.RESOURCE_ORANGE,"Oranges are a top nom-froot.",true),
    new Tresource(Eresourcetype.sheep,"sheep",5,0,5,"sheep farm",Etilesprite.RESOURCE_SHEEP,"A sheep is a sheep but also meat and wool.",true),
    new Tresource(Eresourcetype.coffee,"coffee",1,0,3,"coffee plantation",Etilesprite.RESOURCE_COFFEE,"He who controls the coffee, controls the universe!",true),
    new Tresource(Eresourcetype.gems,"gems",0,5,0,"gem mine",Etilesprite.RESOURCE_GEMS,"Gems have many uses in the poultry industry. WHAAAAT.",false),
    new Tresource(Eresourcetype.oil,"oil",7,2,0,"oil well",Etilesprite.RESOURCE_OIL,"Black spurting liquid. *shrug*",false),
    new Tresource(Eresourcetype.copper,"copper",2,3,0,"CU metalworks",Etilesprite.RESOURCE_COPPER,"We are told copper tastes like blood. Who's licking copper to find this out?",false),
    new Tresource(Eresourcetype.silver,"silver",1,4,0,"AG metalworks",Etilesprite.RESOURCE_SILVER,"Silver is the most commonly eaten metal after some others.",false),
    new Tresource(Eresourcetype.gold,"gold",0,7,0,"AU metalworks",Etilesprite.RESOURCE_GOLD,"Gold is delicious however your chef prepares it.",false ),
    new Tresource(Eresourcetype.tropical_plants,"tropical plants",5,0,0,"plant farm",Etilesprite.RESOURCE_TROPICAL_PLANTS,"Score! I bet you could make some top drugs with these.",true),
    new Tresource(Eresourcetype.fish,"fish",0,0,3,"trawlers",Etilesprite.RESOURCE_FISH,"But imma just touch fishy, kay?",true),
    new Tresource(Eresourcetype.honey,"honey",0,0,4,"apiary",Etilesprite.RESOURCE_HONEY,"An apiary for bees?! Do you put gorillas in a hive?!",true),
    new Tresource(Eresourcetype.iron,"iron",5,0,0,"Ironworks",Etilesprite.RESOURCE_IRON,"If you want to make stuff out of iron you need this I think.",false),
    new Tresource(Eresourcetype.horses,"horses",5,0,0,"Corral",Etilesprite.RESOURCE_HORSES,"Horses are cows but don't bounce. Wait, cows don't bounce.",true)
};
}

