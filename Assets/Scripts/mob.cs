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

    public mobarchetype(bool _heavy,string _name,string _wep,int _hp,
        bool _hostileplayer,bool _hostileenemy,
        bool _undead,bool _skates,bool _flies,
        Etilesprite _tile,Etilesprite _tile_dead,Etilesprite _tile_undead)
    {
        heavy = _heavy; name = _name; weaponname = _wep; hp = _hp; skates = _skates;flies = _flies;
        tile = _tile; tile_dead = _tile_dead;tile_undead = _tile_undead;
    }
}

public class mob {

    public static mobarchetype[] archetypes =
    {//                   heavy? name       wep       hp  hostp hoste unded   skates flies    tile,deadtile,undeadtile
        new mobarchetype(false,"pango","sword",       40, false,true,false,   true,false,     Etilesprite.PLAYER_PANGO_PANGOLIN,Etilesprite.EMPTY,Etilesprite.EMPTY),
        new mobarchetype(false,"marsby","assegai",    40, false,true,false,   true,false,     Etilesprite.PLAYER_REGINALD_MARSBY,Etilesprite.EMPTY,Etilesprite.EMPTY),
        new mobarchetype(false,"papa greebo","chainhook",40, false,true,false,true,false,  Etilesprite.PLAYER_PAPA_GREEBO,Etilesprite.EMPTY,Etilesprite.EMPTY),
        new mobarchetype(true,"polarmoop","claws",   60, false,true,false,   false,false,     Etilesprite.PLAYER_POLARMOOP,Etilesprite.EMPTY,Etilesprite.EMPTY),

        new mobarchetype(true,"polarmoop","claws",   15, true,false,false,   false,false,    Etilesprite.ENEMY_POLARMOOP,Etilesprite.ENEMY_POLARMOOP_CORPSE,Etilesprite.ENEMY_POLARMOOP_SKELETON) ,
        new mobarchetype(false,"lancer","spear",      10, true,false,false,   true,false,     Etilesprite.ENEMY_SKATER_SPEAR,Etilesprite.ENEMY_SKATER_CORPSE,Etilesprite.ENEMY_HUMAN_SKELETON) ,
        new mobarchetype(false,"antipaladin","sword", 10, true,false,false,   true,false,     Etilesprite.ENEMY_SKATER_SWORDANDBOARD,Etilesprite.ENEMY_SKATER_CORPSE,Etilesprite.ENEMY_HUMAN_SKELETON) ,
        new mobarchetype(false,"swinger","chain",     10, true,false,false,   true,false,     Etilesprite.ENEMY_SKATER_CHAIN,Etilesprite.ENEMY_SKATER_CORPSE,Etilesprite.ENEMY_HUMAN_SKELETON) ,
        new mobarchetype(false,"tef-rog","dagger",    10, true,false,false,   true,false,     Etilesprite.ENEMY_SKATER_DAGGER,Etilesprite.ENEMY_SKATER_CORPSE,Etilesprite.ENEMY_HUMAN_SKELETON),
        new mobarchetype(false,"giant bat","fangs",   5,  true,false,false,   false,true,     Etilesprite.ENEMY_GIANTBAT,Etilesprite.ENEMY_GIANTBAT_CORPSE,Etilesprite.ENEMY_GIANTBAT_SKELETON),
        new mobarchetype(false,"kobby bomber","knife",5,  true,false,false,   false,false,    Etilesprite.ENEMY_KOBBY_BOMBER,Etilesprite.ENEMY_KOBBY_BOMBER_CORPSE,Etilesprite.ENEMY_KOBBY_BOMBER_SKELETON),
        new mobarchetype(false,"ice mage","frost hands",    10, true,false,false,   false,false,    Etilesprite.ENEMY_MAGE,Etilesprite.ENEMY_MAGE_CORPSE,Etilesprite.ENEMY_HUMAN_SKELETON),
        new mobarchetype(false,"necromancer","drain life", 10, true,false,false,   false,false,    Etilesprite.ENEMY_NECROMANCER,Etilesprite.ENEMY_NECROMANCER_CORPSE,Etilesprite.ENEMY_LICH),

        new mobarchetype(false,"wolf","bite",         5,  false,true,false,   false,false,    Etilesprite.PLAYER_COMPANION_WOLF,Etilesprite.PLAYER_COMPANION_WOLF_CORPSE,Etilesprite.PLAYER_COMPANION_WOLF_SKELETON),
        new mobarchetype(false,"muhkitten","sharps",  1,  false,true,false,   false,false,    Etilesprite.PLAYER_COMPANION_MUHKITTENS_BLACK,Etilesprite.PLAYER_COMPANION_MUHKITTENS_BLACK_CORPSE,Etilesprite.PLAYER_COMPANION_MUHKITTENS_SKELETON),
        new mobarchetype(false,"muhkitten","sharps",  1,  false,true,false,   false,false,    Etilesprite.PLAYER_COMPANION_MUHKITTENS_BW,Etilesprite.PLAYER_COMPANION_MUHKITTENS_BW_CORPSE,Etilesprite.PLAYER_COMPANION_MUHKITTENS_SKELETON),
        new mobarchetype(false,"muhkitten","sharps",  1,  false,true,false,   false,false,    Etilesprite.PLAYER_COMPANION_MUHKITTENS_GINGER,Etilesprite.PLAYER_COMPANION_MUHKITTENS_GINGER_CORPSE,Etilesprite.PLAYER_COMPANION_MUHKITTENS_SKELETON),
        new mobarchetype(false,"muhkitten","sharps",  1,  false,true,false,   false,false,    Etilesprite.PLAYER_COMPANION_MUHKITTENS_BRITISHBLUE,Etilesprite.PLAYER_COMPANION_MUHKITTENS_BRITISHBLUE_CORPSE,Etilesprite.PLAYER_COMPANION_MUHKITTENS_SKELETON),

        new mobarchetype(false,"summoned skel","raw bones",5,true,false,true,    false,false,Etilesprite.ENEMY_HUMAN_SKELETON,Etilesprite.EMPTY,Etilesprite.ENEMY_HUMAN_SKELETON),
        new mobarchetype(true,"ice golem","icy prong",2,true,false,false,  false,false,    Etilesprite.ENEMY_ICE_GOLEM,Etilesprite.EMPTY,Etilesprite.EMPTY),
        //HP OF GOLEM IS 20
        new mobarchetype(false,"lich","drain life",        5,true,false,true,    false,true,     Etilesprite.ENEMY_LICH,Etilesprite.EMPTY,Etilesprite.EMPTY),

        new mobarchetype(false,"hopped-up fox","bodypart",5,true,true,false,false,false,    Etilesprite.ENEMY_HOPPED_UP_FOX,Etilesprite.ENEMY_HOPPED_UP_FOX_CORPSE,Etilesprite.ENEMY_HOPPED_UP_FOX_SKELETON)
           
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
