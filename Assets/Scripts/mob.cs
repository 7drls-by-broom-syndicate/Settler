using UnityEngine;
using System.Collections;

public enum Emobtype { playerpango, playermarsby, playerpapa, playermoop,
    polarmoop, lancer, antipaladin, swinger, tefrog, bat,
    kobbybomber, mage, necro,
    wolf, kitten1, kitten2, kitten3, kitten4, skelsumd, golem, lich, fox }

public class mobarchetype
{
   
    public string name;
    public string weaponname;   
    public bool skates;
    public bool flies;
    public bool hostile_toplayer;
    public bool hostile_toenemies;
    public bool undead;
    public Etilesprite tile;
    public Etilesprite tile_dead;
    public Etilesprite tile_undead;
    public int hp;
    public bool heavy;

    //new for settler:
    public int upkeepgold;
    public int upkeepfood;
    public int buildcostproduction;
    public int buildcostiron;
    public int buildcosthorses;

    public int attacklow;
    public int attackhigh;
    public int defence;
    public int sightradius;
    public bool tank;
    public bool parry;
    public int moves;
    public int attacks;


    public mobarchetype(string _name,string _wep,int _hp,bool _hostileplayer,bool _hostileenemy,Etilesprite _tile,
        int _upkeepgold,int _upkeepfood,int _buildprod,int _buildhorse,int _buildiron,
        int _al,int _ah,int _def,int _sr,bool _tank,bool _parry,int _moves,int _attacks)
    {
        name = _name; weaponname = _wep; hp = _hp; tile = _tile;hostile_toplayer = _hostileplayer;hostile_toenemies = _hostileenemy;
        upkeepgold = _upkeepgold;
        upkeepfood = _upkeepfood;
        buildcostproduction = _buildprod;
        buildcostiron = _buildiron;
        buildcosthorses = _buildhorse;

        attacklow = _al;
        attackhigh = _ah;
        defence = _def;
        sightradius = _sr;
        tank = _tank;
        parry = _parry;
        moves = _moves;
        attacks = _attacks;
    }
}

public class mob {

    public static mobarchetype[] archetypes =
    {//                 
        new mobarchetype("@snake","sword",40,false,true,Etilesprite.UNITS_PLAYER,//name,attackname, hp,hostile to player, hostile to enemy,tile
            0,0,0,0,0, // upkeep gold,food,buildcost production,iron,horses
            1,5,2, //attacklow,high,defence
            5,false,false,1,1//sightradius,tank?,parry?,#moves,#attacks
            )
    
            //do other units
           
 };

    public bool isplayer=false;
    public bool reversesprite = false;
    public mobarchetype archetype;
    public int hp;
    public int posx, posy;//maybe not needed
    public bool noticedyou = false;
    public Etilesprite tile;

    public bool hostile_toplayer_currently;
    public bool hostile_toenemies_currently;
    public bool undead_currently;
    public bool dead_currently = false;
    public bool flies_currently;
    public bool skates_currently;

    public int facing;
    public int speed;
    public bool hasbeads = false;//sort this out better later but for now...
    public bool hasattackup = false;
    public bool hasdefenseup = false;
    public int attackuptimer = 0;
    public int defenseuptimer = 0;

    //animation for mages
    public bool magepointing = false;
    public float magepointing_timer = 0;

	public mob(Emobtype typ)
    {
        
        mobarchetype at = archetypes[(int)typ];
        archetype = at;
        hp = at.hp;
        tile = at.tile;
        hostile_toenemies_currently = at.hostile_toenemies;
        hostile_toplayer_currently = at.hostile_toplayer;
        undead_currently = at.undead;
        flies_currently = at.flies;
        skates_currently = at.skates;

        facing = 0;
        speed = 0;
    }

    public bool IsAdjacentTo(mob other)
    {
        return (
            (Mathf.Abs(posx - other.posx) == 1 && posy == other.posy) ||
            (Mathf.Abs(posy - other.posy) == 1 && posx == other.posx)
            );
    }
}
