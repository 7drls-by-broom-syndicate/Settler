using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public enum DungeonGenType { Splitter2013, Sucker2014,Synthesizer2015_nothere, Skater2016, Settler2017 }

//THESE HAVE TO BE IN SPRITE ORDER FROM LEFT TO RIGHT
//public enum Etilesprite : int { FLOODTEMP=-2, EMPTY = -1, THEREISNTAZERO=0,NOSPRITE=1,FLOOR=106, WALL=109, PLAYER=2, KOBOLD, TORCHDOWN=46, TORCHRIGHT, TORCHLEFT, TORCHUP,
//                      DOOR_V, DOOR_H, DOOR_V_OPEN, DOOR_H_OPEN, CORRIDOR}




public class Cellndist
{
    public Cell c;
    public double dist;
    public bool flagfordeletion;

    public Cellndist(Cell _c, double _d)
    {
        c = _c;
        dist = _d;
        flagfordeletion = false;
    }
}

public class Spiral
{
    public List<Cellndist> l;
    public Spiral(int w,int h,int xp,int yp)
    {
        l = new List<Cellndist>();

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                l.Add(new Cellndist(new Cell(x, y), RLMap.Distance_EuclideanD(x, y, xp,yp)));
            }
        }
        l.Shuffle(); //not sure what happens here. will the ordering below change the relative order of items with the same value ?
        l=l.OrderBy(o => o.dist).ToList();
    }
}

public partial class RLMap  {
    // public static Color?[] minimapcolours = { new Color (0.25f,0.25f,0.25f), Color.grey, null, null, Color.grey, Color.grey, Color.grey, Color.grey,
    //                                          Color.yellow,Color.yellow,Color.yellow,Color.yellow,new Color (0.25f,0.25f,0.25f)};
    public Array2D<Color> gridflashcolour;
    public Array2D<float> gridflashtime;
    public List<item_instance> bomblist;
    public List<item_instance> walllist;
    public List<Cell> firelist;
    public Color[] minimapcolours;

    public Array2D<Etilesprite> buildings;

    public Texture2D minimap;

    public Array2D<bool> hill;
    public Array2D<bool> mountain;
    public Array2D<bool> tree;

    public Array2D<yields> yield;
    public Array2D<yields> currentyield;
    public Array2D<Tresource> resource;//this is the base resource for the tile and never changes
    public Array2D<bool?> influence;
    public Array2D<Ccity> citythathasinfluence;
    public List<Ccity> citylist;
    //public Array2D<int?> wizwalltime;
    public Array2D<int?> onfire;
    public Array2D<int?> bloodgrid;
    public Array2D<Cell> extradata;
    public int width, height;                               //width and height of the map
    public Array2D<Etilesprite> displaychar;                                    //ascii char for each cell
    public Array2D<Etilesprite> treechar;
    public Array2D<bool> passable;                                      //can player and mobs move through this square yes or no
    public Array2D<bool> blocks_sight;                                  //does this square block sight? e.g. wall yes, open door no
    public Array2D<bool> in_FOV;                                        //FOV routines set this to true in each square in FOV
    Array2D<bool> FOV_set_this_run;                              //temp array used in fov routine to avoid processing each in_fov square multiple times
    public Array2D<bool> fogofwar;                                      //is cell covered by fog of war, i.e. not discovered yet? starts off true for every cell
    public Array2D<bool> fogoffog;
    //BitArray locked;                                      //is square "locked". this is a bit kind of specific to sucker
    public Array2D<Etilesprite> playermemory;                                   //this is what player last saw on that square (architecture only)

    public Array2D<Color> staticlight;                            //light amount on cell e.g. torches. can be changed but not often
    public Array2D<Color> dynamiclight;

    Array2D<int> distance;                                        //used by pathfinding routines
    public Queue<Cell> lastpath=new Queue<Cell>();                                    //this is filled by the pathfinding routines
    public int firststepx, firststepy;                             //used by pathfinding
    public List<Cell> emptyspaces;                                 //free squares

    public void killoffamob(mob f)
    {
        f.tile = Etilesprite.EMPTY;         //this will make the processturn function garbage collect the mob from moblist
        passable[f.posx, f.posy] = true;    
        itemgrid[f.posx, f.posy] = null;
        

    }

    public bool passablecheck(int x,int y,mob m)
    {
        return true;
        // return (passable[x, y] || displaychar[x, y] == Etilesprite.MAP_WATER && m.archetype.heavy);
    }
    private Player player;

    public Array2D<item_instance> itemgrid;//if you want multiple objects on each square, need a 2d array of lists of items where each list can be null
    public List<mob> moblist;                                      //List<item_instance> moblist;//put mobs in with items? but this var is a list of mobs with no ref to where they are
    public List<mob> newmoblist;
                                           //list<item_instance> bomblist // specific to sucker


    public bool onmap(int x,int y)
    {
        return (x >= 0 && x < width && y >= 0 && y < height);
    }
    bool IsEmpty(int x, int y)
    {
        if (x < 0 || x >=width || y < 0 || y >= height) return false;//off map
        //if (displaychar[x, y] != Etilesprite.FLOOR) return false; //not an empty tile
        if (passable[x,y] ==false) return false;//not passable
        if (x == player.posx && y == player.posy) return false;//player is on square
        if (itemgrid[x, y] != null) return false;//item on square
        return true;
    }

    Cell Random9way(int x, int y)
    {
        
            List<Cell> bob = new List<Cell>();
            if (IsEmpty(x - 1, y)) bob.Add(new Cell(x - 1, y));
            if (IsEmpty(x + 1, y)) bob.Add(new Cell(x + 1, y));
            if (IsEmpty(x, y - 1)) bob.Add(new Cell(x, y - 1));
            if (IsEmpty(x, y + 1)) bob.Add(new Cell(x, y + 1));

            if (IsEmpty(x - 1, y - 1)) bob.Add(new Cell(x - 1, y - 1));
            if (IsEmpty(x + 1, y + 1)) bob.Add(new Cell(x + 1, y + 1));
            if (IsEmpty(x + 1, y - 1)) bob.Add(new Cell(x + 1, y - 1));
            if (IsEmpty(x - 1, y + 1)) bob.Add(new Cell(x - 1, y + 1));
            if (bob.Count == 0) return null;
            else return bob.randmember();
        
    }


    public List<Cell> BresLine(int x0, int y0, int x1, int y1){

		int dx = Math.Abs(x1 - x0);
		int dy = Math.Abs(y1 - y0);
		int sx = (x0 < x1) ? 1 : -1;
		int sy = (y0 < y1) ? 1 : -1;
		int err = dx - dy;

		var pibs=new List<Cell>();

		while (true){
            pibs.Add( new Cell(x0, y0 )) ;

			if (x0 == x1 && y0 == y1)break;
			int e2 = 2 * err;
			if (e2 > -dy){
				err -= dy;
				x0 += sx;
			}
			if (e2 < dx){
				err += dx;
				y0 += sy;
			}
		}
		return pibs;
	}

    public static int Distance_ChevyChase(int x, int y, int x2, int y2) {
		int dx = Math.Abs(x - x2);
		int dy = Math.Abs(y - y2);
		return (dx > dy) ? dx : dy;
	}

    public static int Distance_Manhatten(int x, int y, int x2, int y2) {
		int dx = Math.Abs(x - x2);
		int dy = Math.Abs(y - y2);
		return  dx + dy;
	}

    public static int Distance_Euclidean(int x, int y, int x2, int y2) {
		int dx = Math.Abs(x - x2);
		int dy = Math.Abs(y - y2);
		return (int)Math.Sqrt((dx*dx) + (dy*dy));
	}

   public static double Distance_EuclideanD(int x, int y, int x2, int y2) {
		int dx = Math.Abs(x - x2);
		int dy = Math.Abs(y - y2);
		return Math.Sqrt((dx*dx) + (dy*dy));
	}

    public static int Distance_Squared(int x, int y, int x2, int y2) {
		int dx = Math.Abs(x - x2);
		int dy = Math.Abs(y - y2);
		return (dx*dx) + (dy*dy);
	}


    

  

    public RLMap(Player pp,DungeonGenType dgt) {
        player = pp;
       // player.dunlevel++;
        switch ( dgt) {
            case DungeonGenType.Splitter2013:
                width = 90; height = 90;
                break;
            case DungeonGenType.Sucker2014:
                width = 80;height=50;
                break;
            case DungeonGenType.Skater2016:
                width = 80;height = 50;
                break;
            case DungeonGenType.Settler2017:
                width = 80;height = 50;
                break;
        }

        minimapcolours = new Color[256];
        // public static Color?[] minimapcolours = { new Color (0.25f,0.25f,0.25f), Color.grey, null, null, Color.grey, Color.grey, Color.grey, Color.grey,
    //                                          Color.yellow,Color.yellow,Color.yellow,Color.yellow,new Color (0.25f,0.25f,0.25f)};


        //setup minimap colours
        for (int i = 0; i < 256; i++)
        {
            minimapcolours[i] = lightgrey;
        }
        minimapcolours[(int)Etilesprite.BASE_TILE_OCEAN_1] = Color.blue;
        minimapcolours[(int)Etilesprite.BASE_TILE_OCEAN_2] = Color.blue;
        minimapcolours[(int)Etilesprite.BASE_TILE_OCEAN_3] = Color.blue;
        minimapcolours[(int)Etilesprite.BASE_TILE_COASTAL_WATER_1] = Color.cyan;
        minimapcolours[(int)Etilesprite.BASE_TILE_COASTAL_WATER_2] = Color.cyan;
        minimapcolours[(int)Etilesprite.BASE_TILE_COASTAL_WATER_3] = Color.cyan;
        minimapcolours[(int)Etilesprite.BASE_TILE_POLAR_1] = lil.rgb_unitycolour(0xf5, 0xf5, 0xf5);
        minimapcolours[(int)Etilesprite.BASE_TILE_POLAR_2] = lil.rgb_unitycolour(0xf5, 0xf5, 0xf5);
        minimapcolours[(int)Etilesprite.BASE_TILE_POLAR_3] = lil.rgb_unitycolour(0xf5, 0xf5, 0xf5);
        minimapcolours[(int)Etilesprite.BASE_TILE_TUNDRA_1] = lil.rgb_unitycolour(0xd9,0x7e,0x82);
        minimapcolours[(int)Etilesprite.BASE_TILE_TUNDRA_2] = lil.rgb_unitycolour(0xd9, 0x7e, 0x82);
        minimapcolours[(int)Etilesprite.BASE_TILE_TUNDRA_3] = lil.rgb_unitycolour(0xd9, 0x7e, 0x82);
        minimapcolours[(int)Etilesprite.BASE_TILE_TAIGA_1] = lil.rgb_unitycolour(0xb5, 0xf2, 0x99);
        minimapcolours[(int)Etilesprite.BASE_TILE_TAIGA_2] = lil.rgb_unitycolour(0xb5, 0xf2, 0x99);
        minimapcolours[(int)Etilesprite.BASE_TILE_TAIGA_3] = lil.rgb_unitycolour(0xb5, 0xf2, 0x99);
        minimapcolours[(int)Etilesprite.BASE_TILE_ALPINE_1] = lil.rgb_unitycolour(0x92, 0xc2, 0x7c);
        minimapcolours[(int)Etilesprite.BASE_TILE_ALPINE_2] = lil.rgb_unitycolour(0x92, 0xc2, 0x7c);
        minimapcolours[(int)Etilesprite.BASE_TILE_ALPINE_3] = lil.rgb_unitycolour(0x92, 0xc2, 0x7c);
        minimapcolours[(int)Etilesprite.BASE_TILE_MEDITERRANEAN_1] = lil.rgb_unitycolour(0xe3, 0xe0, 0xb6);
        minimapcolours[(int)Etilesprite.BASE_TILE_MEDITERRANEAN_2] = lil.rgb_unitycolour(0xe3, 0xe0, 0xb6);
        minimapcolours[(int)Etilesprite.BASE_TILE_MEDITERRANEAN_3] = lil.rgb_unitycolour(0xe3, 0xe0, 0xb6);
        minimapcolours[(int)Etilesprite.BASE_TILE_PRAIRIE_1] = lil.rgb_unitycolour(0xa6,0x7f,0x44);
        minimapcolours[(int)Etilesprite.BASE_TILE_PRAIRIE_2] = lil.rgb_unitycolour(0xa6, 0x7f, 0x44);
        minimapcolours[(int)Etilesprite.BASE_TILE_PRAIRIE_3] = lil.rgb_unitycolour(0xa6, 0x7f, 0x44);
        minimapcolours[(int)Etilesprite.BASE_TILE_TEMPERATE_FOREST_1] = lil.rgb_unitycolour(0x55,0x87,0x4d);
        minimapcolours[(int)Etilesprite.BASE_TILE_TEMPERATE_FOREST_2] = lil.rgb_unitycolour(0x55, 0x87, 0x4d);
        minimapcolours[(int)Etilesprite.BASE_TILE_TEMPERATE_FOREST_3] = lil.rgb_unitycolour(0x55, 0x87, 0x4d);
        minimapcolours[(int)Etilesprite.BASE_TILE_DESERT_1] = lil.rgb_unitycolour(0xd9,0xc1,0x62);
        minimapcolours[(int)Etilesprite.BASE_TILE_DESERT_2] = lil.rgb_unitycolour(0xd9, 0xc1, 0x62);
        minimapcolours[(int)Etilesprite.BASE_TILE_DESERT_3] = lil.rgb_unitycolour(0xd9, 0xc1, 0x62);
        minimapcolours[(int)Etilesprite.BASE_TILE_SAVANNA_1] = lil.rgb_unitycolour(0xc9,0xc9,0x73);
        minimapcolours[(int)Etilesprite.BASE_TILE_SAVANNA_2] = lil.rgb_unitycolour(0xc9, 0xc9, 0x73);
        minimapcolours[(int)Etilesprite.BASE_TILE_SAVANNA_3] = lil.rgb_unitycolour(0xc9, 0xc9, 0x73);
        minimapcolours[(int)Etilesprite.BASE_TILE_TROPICAL_RAINFOREST_1] = lil.rgb_unitycolour(0x3b,0x59,0x34);
        minimapcolours[(int)Etilesprite.BASE_TILE_TROPICAL_RAINFOREST_2] = lil.rgb_unitycolour(0x3b, 0x59, 0x34);
        minimapcolours[(int)Etilesprite.BASE_TILE_TROPICAL_RAINFOREST_3] = lil.rgb_unitycolour(0x3b, 0x59, 0x34);

        minimapcolours[(int)Etilesprite.BUILDINGS_BARBARIAN_CITADEL] = Color.red;
        minimapcolours[(int)Etilesprite.BUILDINGS_BARBARIAN_CAMP] = Color.red;
        minimapcolours[(int)Etilesprite.BUILDINGS_CITY_STATE] = Color.yellow;
        minimapcolours[(int)Etilesprite.BUILDINGS_CITY] = Color.blue;
        minimapcolours[(int)Etilesprite.BUILDINGS_FACTORY] = Color.blue;
        minimapcolours[(int)Etilesprite.BUILDINGS_TRADING_POST] = Color.blue;
        minimapcolours[(int)Etilesprite.BUILDINGS_BARRACKS] = Color.blue;
        minimapcolours[(int)Etilesprite.BUILDINGS_MARKET] = Color.blue;
        minimapcolours[(int)Etilesprite.BUILDINGS_ALLOTMENTS] = Color.blue;
        minimapcolours[(int)Etilesprite.BUILDINGS_GUARD_POST] = Color.blue;
        minimapcolours[(int)Etilesprite.BUILDINGS_ARMOURER] = Color.blue;
        minimapcolours[(int)Etilesprite.BUILDINGS_BLACKSMITH] = Color.blue;
        minimapcolours[(int)Etilesprite.BUILDINGS_PORT_AND_DOCKS] = Color.blue;
        minimapcolours[(int)Etilesprite.BUILDINGS_TELEPORTER] = Color.blue;
        minimapcolours[(int)Etilesprite.BUILDINGS_TOWN_HALL] = Color.blue;
        minimapcolours[(int)Etilesprite.BUILDINGS_GOODIE_HUT] = Color.yellow;




        /*
        minimapcolours[(int)Etilesprite.MAP_SNOW] = Color.white;
        minimapcolours[(int)Etilesprite.MAP_WATER] = lil.rgb_unitycolour(38, 43, 55);
        minimapcolours[(int)Etilesprite.MAP_ICE] = lil.rgb_unitycolour(101, 147, 232);
        minimapcolours[(int)Etilesprite.MAP_THIN_ICE] = lil.rgb_unitycolour(188, 212, 255);
        minimapcolours[(int)Etilesprite.ITEM_WARP_GATE_ANIM_1] = lil.rgb_unitycolour(184, 133, 217);
        minimapcolours[(int)Etilesprite.ITEM_CAIRN_BLUE ]= Color.blue;
        minimapcolours[(int)Etilesprite.ITEM_CAIRN_RED ]= Color.red;
        minimapcolours[(int)Etilesprite.ITEM_CAIRN_GREEN] = Color.green;
        minimapcolours[(int)Etilesprite.ITEM_CAIRN_PURPLE] = lil.rgb_unitycolour(100,27,136) ;
        */
        //locked=new BitArray(width*height,false);
        itemgrid =new Array2D<item_instance>(width, height,null);
        moblist = new List<mob>(); //will the old mobs in the old moblist be garbage collected?
        newmoblist = new List<mob>();
        displaychar =new Array2D<Etilesprite>(width,height,Etilesprite.EMPTY);
        treechar = new Array2D<Etilesprite>(width, height, Etilesprite.EMPTY);
        passable =new Array2D<bool>(width,height,true);

        tree = new Array2D<bool>(width, height, false);
        hill = new Array2D<bool>(width, height, false);
        mountain= new Array2D<bool>(width, height, false);

        yield= new Array2D<yields>(width, height);
        currentyield = new Array2D<yields>(width, height);
        influence = new Array2D<bool?>(width, height, null);

        citythathasinfluence = new Array2D<Ccity>(width, height, null);
        citylist = new List<Ccity>();

        resource = new Array2D<Tresource>(width, height,null);
        

        buildings = new Array2D<Etilesprite>(width, height, Etilesprite.EMPTY);

        onfire = new Array2D<int?>(width, height, null);
        bloodgrid = new Array2D<int?>(width, height, null);
        //wizwalltime = new Array2D<int?>(width, height, null);
        blocks_sight=new Array2D<bool>(width,height,false);
        distance=new Array2D<int>(width,height);
		in_FOV=new Array2D<bool>(width,height,false);
		FOV_set_this_run=new Array2D<bool>(width,height,false);
		//maxint = std::numeric_limits<int>::max();
		playermemory=new Array2D<Etilesprite>(width,height,Etilesprite.EMPTY);
		fogofwar=new Array2D<bool>(width,height,true);
        fogoffog = new Array2D<bool>(width, height, false);
        staticlight = new Array2D<Color>( width, height,Color.black);
        dynamiclight = new Array2D<Color>(width, height, Color.black);
        extradata = new Array2D<Cell>(width, height, null);
        gridflashcolour = new Array2D<Color>(width, height, Color.clear);
        gridflashtime = new Array2D<float>(width, height, 0f);
        bomblist = new List<item_instance>();
        walllist= new List<item_instance>();
        firelist = new List<Cell>();

        switch (dgt) {
            case DungeonGenType.Splitter2013:
               // genlevelsplitterstyle();
                break;
            case DungeonGenType.Sucker2014:
               // genlevelsuckerstyle();
                break;
            case DungeonGenType.Skater2016:
                //genlevelskaterstyle();
                break;
            case DungeonGenType.Settler2017:
                genlevelsettlerstyle();
                break;
        }
		

        minimap = new Texture2D(width,height,TextureFormat.ARGB32,false,false);
        minimap.filterMode = FilterMode.Point;

        Color c=new Color(0.1f,0.1f,0.1f);

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                minimap.SetPixel(x, y, c);
        minimap.Apply();
        
		
	}

     public void FreeSpace(out int a, out int b){
         Cell c = emptyspaces.OneFromTheTop();
         a = c.x; b = c.y;
		}

    public void fillminimap() {
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if(buildings[x,y]!=Etilesprite.EMPTY)minimap.SetPixel(x, y, minimapcolours[(int)buildings[x, y]]);
                else minimap.SetPixel(x, y, minimapcolours[(int)displaychar[x, y]]);
            }
        }
    }
}
