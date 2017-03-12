using UnityEngine;
using System.Collections;


public class mobarchetype
{
    public Emobtype type;
    public string name;
    public string weaponname;   
    public bool skates;
    public bool flies;
    public bool hostile_toplayer;
    public bool hostile_toenemies;
  //  public bool undead;
    public Etilesprite tile;
   // public Etilesprite tile_dead;
   // public Etilesprite tile_undead;
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


    public mobarchetype(Emobtype _type,string _name,string _wep,int _hp,bool _hostileplayer,bool _hostileenemy,Etilesprite _tile,
        int _upkeepgold,int _upkeepfood,int _buildprod,int _buildhorse,int _buildiron,
        int _al,int _ah,int _def,int _sr,bool _tank,bool _parry,int _moves,int _attacks)
    {
        type = _type;
        name = _name; weaponname = _wep; hp = _hp; tile = _tile;hostile_toplayer = _hostileplayer;hostile_toenemies = _hostileenemy;
        upkeepgold = _upkeepgold;
        upkeepfood = _upkeepfood;
        buildcostproduction = _buildprod;
        buildcostiron = _buildiron;
        buildcosthorses =  _buildhorse;

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

public enum Emobtype { playersnake,trader,
    playerscout,playerclubber,playerspearman,playerknight,playerpikeman,playermountedclubber,playermountedknight,
    enemyscout, enemyclubber, enemyspearman, enemyknight, enemypikeman, enemymountedclubber, enemymountedknight,
    enemychampion,enemylord,
    broom,moop,torkinspider,reginaldmarsby,chickadeemonserat,benzedreneESR,creepysalado,doctorbarnaby,pangopangolin
}



public class mob {

    public static mobarchetype[] archetypes =
    {//                 
        new mobarchetype(Emobtype.playersnake,"@snake","sword",40,false,true,Etilesprite.UNITS_PLAYER,//name,attackname, hp,hostile to player, hostile to enemy,tile
            0,0,// upkeep gold,food,
            0,0,0,// buildcost production,iron,horses
            1,5,2, //attacklow,high,defence
            5,false,false,1,1//sightradius,tank?,parry?,#moves,#attacks
            ),
             new mobarchetype(Emobtype.trader,"Trader","",40,false,true,Etilesprite.UNITS_PLAYER_TRADER,//name,attackname, hp,hostile to player, hostile to enemy,tile
            0,0,// upkeep gold,food,
            0,0,0,// buildcost production,iron,horses
            0,0,0, //attacklow,high,defence
            0,false,false,1,0//sightradius,tank?,parry?,#moves,#attacks
            ),
            new mobarchetype(Emobtype.playerscout,"Scout","penknife",2,false,true,Etilesprite.UNITS_PLAYER_SCOUT,//name,attackname, hp,hostile to player, hostile to enemy,tile
            0,1,// upkeep gold,food,
            50,0,0,// buildcost production,iron,horses
            1,2,0, //attacklow,high,defence
            3,false,false,1,1//sightradius,tank?,parry?,#moves,#attacks
            ),
           new mobarchetype(Emobtype.playerclubber,"Clubber","club",8,false,true,Etilesprite.UNITS_PLAYER_CLUBBER,//name,attackname, hp,hostile to player, hostile to enemy,tile
            0,2,// upkeep gold,food,
            100,0,0,// buildcost production,iron,horses
            1,4,0, //attacklow,high,defence
            1,false,false,1,1//sightradius,tank?,parry?,#moves,#attacks
            ),
            new mobarchetype(Emobtype.playerspearman,"Spearman","spear",8,false,true,Etilesprite.UNITS_PLAYER_SPEARMAN,//name,attackname, hp,hostile to player, hostile to enemy,tile
            0,3,// upkeep gold,food,
            100,0,0,// buildcost production,iron,horses
            1,6,0, //attacklow,high,defence
            1,false,false,1,1//sightradius,tank?,parry?,#moves,#attacks
            ),
             new mobarchetype(Emobtype.playerknight,"Knight","sword",20,false,true,Etilesprite.UNITS_PLAYER_KNIGHT,//name,attackname, hp,hostile to player, hostile to enemy,tile
            0,5,// upkeep gold,food,
            400,10,0,// buildcost production,iron,horses
            2,8,2, //attacklow,high,defence
            1,false,false,1,1//sightradius,tank?,parry?,#moves,#attacks
            ),
             new mobarchetype(Emobtype.playerpikeman,"Pikeman","pike",12,false,true,Etilesprite.UNITS_PLAYER_PIKEMAN,//name,attackname, hp,hostile to player, hostile to enemy,tile
            0,5,// upkeep gold,food,
            300,8,0,// buildcost production,iron,horses
            3,9,0, //attacklow,high,defence
            1,false,false,1,1//sightradius,tank?,parry?,#moves,#attacks
            ),
            new mobarchetype(Emobtype.playermountedclubber,"Mounted Clubber","club",10,false,true,Etilesprite.UNITS_PLAYER_MOUNTED_CLUBBER,//name,attackname, hp,hostile to player, hostile to enemy,tile
            0,8,// upkeep gold,food,
            400,8,0,// buildcost production,iron,horses
           1,4,0, //attacklow,high,defence
            1,false,false,2,1//sightradius,tank?,parry?,#moves,#attacks
            ),
            new mobarchetype(Emobtype.playermountedknight,"Mounted Knight","sword",22,false,true,Etilesprite.UNITS_PLAYER_MOUNTED_KNIGHT,//name,attackname, hp,hostile to player, hostile to enemy,tile
            0,8,// upkeep gold,food,
            500,10,10,// buildcost production,iron,horses
            2,8,2, //attacklow,high,defence
            1,false,false,2,1//sightradius,tank?,parry?,#moves,#attacks
            ),
             new mobarchetype(Emobtype.enemyscout,"Barb. Scout","penknife",2,true,false,Etilesprite.UNITS_ENEMY_SCOUT,//name,attackname, hp,hostile to player, hostile to enemy,tile
            0,1,// upkeep gold,food,
            50,0,0,// buildcost production,iron,horses
            1,2,0, //attacklow,high,defence
            3,false,false,1,1//sightradius,tank?,parry?,#moves,#attacks
            ),
           new mobarchetype(Emobtype.enemyclubber,"Barb. Clubber","club",8,true,false,Etilesprite.UNITS_ENEMY_CLUBBER,//name,attackname, hp,hostile to player, hostile to enemy,tile
            0,2,// upkeep gold,food,
            100,0,0,// buildcost production,iron,horses
            1,4,0, //attacklow,high,defence
            1,false,false,1,1//sightradius,tank?,parry?,#moves,#attacks
            ),
            new mobarchetype(Emobtype.enemyspearman,"Barb. Spearman","spear",8,true,false,Etilesprite.UNITS_ENEMY_SPEARMAN,//name,attackname, hp,hostile to player, hostile to enemy,tile
            0,3,// upkeep gold,food,
            100,0,0,// buildcost production,iron,horses
            1,6,0, //attacklow,high,defence
            1,false,false,1,1//sightradius,tank?,parry?,#moves,#attacks
            ),
             new mobarchetype(Emobtype.enemyknight,"Barb. Knight","sword",20,true,false,Etilesprite.UNITS_ENEMY_KNIGHT,//name,attackname, hp,hostile to player, hostile to enemy,tile
            0,5,// upkeep gold,food,
            400,10,0,// buildcost production,iron,horses
            2,8,2, //attacklow,high,defence
            1,false,false,1,1//sightradius,tank?,parry?,#moves,#attacks
            ),
             new mobarchetype(Emobtype.enemypikeman,"Barb. Pikeman","pike",12,true,false,Etilesprite.UNITS_ENEMY_PIKEMAN,//name,attackname, hp,hostile to player, hostile to enemy,tile
            0,5,// upkeep gold,food,
            300,8,0,// buildcost production,iron,horses
            3,9,0, //attacklow,high,defence
            1,false,false,1,1//sightradius,tank?,parry?,#moves,#attacks
            ),
            new mobarchetype(Emobtype.enemymountedclubber,"Barb. Mounted Clubber","club",10,true,false,Etilesprite.UNITS_ENEMY_MOUNTED_CLUBBER,//name,attackname, hp,hostile to player, hostile to enemy,tile
            0,8,// upkeep gold,food,
            400,8,0,// buildcost production,iron,horses
           1,4,0, //attacklow,high,defence
            1,false,false,2,1//sightradius,tank?,parry?,#moves,#attacks
            ),
            new mobarchetype(Emobtype.enemymountedknight,"Barb. Mounted Knight","sword",22,true,false,Etilesprite.UNITS_ENEMY_MOUNTED_KNIGHT,//name,attackname, hp,hostile to player, hostile to enemy,tile
            0,8,// upkeep gold,food,
            500,10,10,// buildcost production,iron,horses
            2,8,2, //attacklow,high,defence
            1,false,false,2,1//sightradius,tank?,parry?,#moves,#attacks
            ),

            new mobarchetype(Emobtype.enemychampion,"Barb. Champion","dual kris",40,true,false,Etilesprite.UNITS_BARBARIAN_CHAMPION,//name,attackname, hp,hostile to player, hostile to enemy,tile
            0,0,// upkeep gold,food,
            500,10,10,// buildcost production,iron,horses
            1,10,3, //attacklow,high,defence
            1,false,false,2,1//sightradius,tank?,parry?,#moves,#attacks
            ),

            new mobarchetype(Emobtype.enemylord,"Lord Malprong","daikatana",80,true,false,Etilesprite.UNITS_BARBARIAN_LORD,//name,attackname, hp,hostile to player, hostile to enemy,tile
            0,0,// upkeep gold,food,
            500,10,10,// buildcost production,iron,horses
            3,10,4, //attacklow,high,defence
            1,false,false,2,1//sightradius,tank?,parry?,#moves,#attacks
            ),

            new mobarchetype(Emobtype.broom,"Broom","bristle",40,false,true,Etilesprite.UNITS_PLAYER_HERO_BROOM,//name,attackname, hp,hostile to player, hostile to enemy,tile
            10,0,// upkeep gold,food,
            50,0,0,// buildcost production,iron,horses
            1,10,1, //attacklow,high,defence
            3,false,false,1,1//sightradius,tank?,parry?,#moves,#attacks
            ),
        


            //still more mobs to do
    };

    public int numattacksleft;
    public int nummovesleft;

    public bool haveyoumovedthisgo;

    public bool usedDEFthisturn;

    public Ccity citythatownsit;
    public int attackbonus = 0;
    public int defencebonus = 0;
    public bool isplayer=false;
    public bool reversesprite = false;
    public mobarchetype archetype;
    public int hp;
    public AI AIformob;
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
        AIformob = new AI();
        mobarchetype at = archetypes[(int)typ];
        archetype = at;
        hp = at.hp;
        tile = at.tile;
        hostile_toenemies_currently = at.hostile_toenemies;
        hostile_toplayer_currently = at.hostile_toplayer;
        //undead_currently = at.undead;
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
