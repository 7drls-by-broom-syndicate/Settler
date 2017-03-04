using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public enum DungeonGenType { Splitter2013, Sucker2014,Synthesizer2015_nothere, Skater2016 }

//THESE HAVE TO BE IN SPRITE ORDER FROM LEFT TO RIGHT
//public enum Etilesprite : int { FLOODTEMP=-2, EMPTY = -1, THEREISNTAZERO=0,NOSPRITE=1,FLOOR=106, WALL=109, PLAYER=2, KOBOLD, TORCHDOWN=46, TORCHRIGHT, TORCHLEFT, TORCHUP,
  //                      DOOR_V, DOOR_H, DOOR_V_OPEN, DOOR_H_OPEN, CORRIDOR}


public partial class RLMap  {
    // public static Color?[] minimapcolours = { new Color (0.25f,0.25f,0.25f), Color.grey, null, null, Color.grey, Color.grey, Color.grey, Color.grey,
    //                                          Color.yellow,Color.yellow,Color.yellow,Color.yellow,new Color (0.25f,0.25f,0.25f)};
    public Array2D<Color> gridflashcolour;
    public Array2D<float> gridflashtime;
    public List<item_instance> bomblist;
    public List<item_instance> walllist;
    public List<Cell> firelist;
    public Color?[] minimapcolours;

    public Texture2D minimap;

    //public Array2D<int?> wizwalltime;
    public Array2D<int?> onfire;
    public Array2D<int?> bloodgrid;
    public Array2D<Cell> extradata;
    public int width, height;                               //width and height of the map
    public Array2D<Etilesprite> displaychar;                                    //ascii char for each cell
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

    public bool passablecheck(int x,int y,mob m)
    {
        return (passable[x, y] || displaychar[x, y] == Etilesprite.MAP_WATER && m.archetype.heavy);
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
        }

        minimapcolours = new Color?[256];
        // public static Color?[] minimapcolours = { new Color (0.25f,0.25f,0.25f), Color.grey, null, null, Color.grey, Color.grey, Color.grey, Color.grey,
    //                                          Color.yellow,Color.yellow,Color.yellow,Color.yellow,new Color (0.25f,0.25f,0.25f)};


        //setup minimap colours
        for (int i = 0; i < 256; i++)
        {
            minimapcolours[i] = lightgrey;
        }

        minimapcolours[(int)Etilesprite.MAP_SNOW] = Color.white;
        minimapcolours[(int)Etilesprite.MAP_WATER] = lil.rgb_unitycolour(38, 43, 55);
        minimapcolours[(int)Etilesprite.MAP_ICE] = lil.rgb_unitycolour(101, 147, 232);
        minimapcolours[(int)Etilesprite.MAP_THIN_ICE] = lil.rgb_unitycolour(188, 212, 255);
        minimapcolours[(int)Etilesprite.ITEM_WARP_GATE_ANIM_1] = lil.rgb_unitycolour(184, 133, 217);
        minimapcolours[(int)Etilesprite.ITEM_CAIRN_BLUE ]= Color.blue;
        minimapcolours[(int)Etilesprite.ITEM_CAIRN_RED ]= Color.red;
        minimapcolours[(int)Etilesprite.ITEM_CAIRN_GREEN] = Color.green;
        minimapcolours[(int)Etilesprite.ITEM_CAIRN_PURPLE] = lil.rgb_unitycolour(100,27,136) ;

        //locked=new BitArray(width*height,false);
        itemgrid =new Array2D<item_instance>(width, height,null);
        moblist = new List<mob>(); //will the old mobs in the old moblist be garbage collected?
        newmoblist = new List<mob>();
        displaychar =new Array2D<Etilesprite>(width,height,Etilesprite.EMPTY);
		passable=new Array2D<bool>(width,height,true);
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
                genlevelskaterstyle();
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

    /* public void fillminimap() {
         for (int y = 0; y < height; y++) {
             for (int x = 0; x < width; x++) {
                 Etilesprite et=this.displaychar.AtGet(x,y);
                 Color c;
                 switch(et){
                     case Etilesprite.WALL:
                     case Etilesprite.TORCHDOWN:
                     case Etilesprite.TORCHUP:
                    case Etilesprite.TORCHLEFT:
                    case Etilesprite.TORCHRIGHT:
                    case Etilesprite.

                 }
                 minimap.SetPixel(x,y,)
             }
         }
     }*/
}
