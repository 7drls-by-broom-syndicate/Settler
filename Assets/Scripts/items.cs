using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class item_instance {
    public Etilesprite tile;
    public bool ismob;
    public mob mob;
    public int bombcount;
    public int bombx, bomby; //sloppy- should use mob instead

    public item_instance(Etilesprite _tile,bool _ismob=false,mob m=null,int _bombcount=0)
    {
        tile = _tile;
        ismob = _ismob;
        mob = m;
        bombcount = _bombcount;
    }

    public static readonly List<Etilesprite> possible_barrel_contents = new List<Etilesprite>{
            Etilesprite.ITEM_RAW_MEAT,
            Etilesprite.ITEM_BOMB,
            Etilesprite.ITEM_TRAP,
            Etilesprite.ITEM_SCROLL_FIRELANCE,
            Etilesprite.ITEM_SCROLL_ICECUBE,
            Etilesprite.ITEM_POTION_SPEED,
            Etilesprite.ITEM_POTION_SPECIAL,
            Etilesprite.ITEM_FISHING_ROD,
            Etilesprite.ENEMY_HOPPED_UP_FOX
    };
    public static readonly List<Etilesprite> holdable_items = new List<Etilesprite>
    {
            Etilesprite.ITEM_RAW_MEAT,
            Etilesprite.ITEM_BOMB,
            Etilesprite.ITEM_TRAP,
            Etilesprite.ITEM_SCROLL_FIRELANCE,
            Etilesprite.ITEM_SCROLL_ICECUBE,
            Etilesprite.ITEM_POTION_SPEED,
            Etilesprite.ITEM_POTION_SPECIAL,
            Etilesprite.ITEM_FISHING_ROD,
            Etilesprite.ITEM_FISH
    };



    //possible barrel contents, not counting warp beads which is guaranteed to be in 1 and only 1 barrel
    public static readonly Etilesprite[] OLDpossible_barrel_contents = {
            Etilesprite.ITEM_RAW_MEAT,
            Etilesprite.ITEM_BOMB,
            Etilesprite.ITEM_TRAP,
            Etilesprite.ITEM_SCROLL_FIRELANCE,
            Etilesprite.ITEM_SCROLL_ICECUBE,
            Etilesprite.ITEM_POTION_SPEED,
            Etilesprite.ITEM_POTION_SPECIAL,
            Etilesprite.ITEM_FISHING_ROD,
            Etilesprite.ENEMY_HOPPED_UP_FOX
        };
    //items you can hold in your hand and use
    public static readonly Etilesprite[] OLDholdable_items = {
            Etilesprite.ITEM_RAW_MEAT,
            Etilesprite.ITEM_BOMB,
            Etilesprite.ITEM_TRAP,
            Etilesprite.ITEM_SCROLL_FIRELANCE,
            Etilesprite.ITEM_SCROLL_ICECUBE,
            Etilesprite.ITEM_POTION_SPEED,
            Etilesprite.ITEM_POTION_SPECIAL,
            Etilesprite.ITEM_FISHING_ROD,
            Etilesprite.ITEM_FISH
        };

}
