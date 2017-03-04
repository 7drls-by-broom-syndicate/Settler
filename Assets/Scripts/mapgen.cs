using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


public class Patch
{
    //public const char EMPTY = ' ';
    public Array2D<Etilesprite> cells;
    //int floodcount = 0;
    public float deltax, deltay;
    public float posx, posy;
    public bool out_of_bounds = false;
    public bool stamped_down = false;



    public bool AnyMatching(int x, int y, Etilesprite ets)
    {
        for (int i = 0; i < 8; i++)
        {
            int tempx = x + lil.deltax[i];
            int tempy = y + lil.deltay[i];
            if (tempx > 0 && tempy > 0 && tempx < cells.width - 1 && tempy < cells.height - 1)
                if (cells[tempx, tempy] == ets) return true;

        }
        return false;

    }
    public Patch(int w, int h)
    {
        cells = new Array2D<Etilesprite>(w, h, Etilesprite.MAP_STONE_WALL_RUIN); //2016 was wall
    }
    public void drawchar(int x, int y)
    {
        if (x < 0 || y < 0 || x >= cells.width || y >= cells.height)
        {
            Debug.Log("drawchar out of bounds!");
        }
        cells.AtSet(x, y, Etilesprite.MAP_STONE_FLOOR_RUIN);//2016 was floor
    }
    void fillrect(int x0, int y0, int w, int h)
    {
        for (int y = y0; y < y0 + h; y++)
        {
            for (int x = x0; x < x0 + w; x++)
            {
                drawchar(x, y);
            }
        }
    }
    public void bresline(int x0, int y0, int x1, int y1)
    {

        int dx = Math.Abs(x1 - x0);
        int dy = Math.Abs(y1 - y0);
        int sx = (x0 < x1) ? 1 : -1;
        int sy = (y0 < y1) ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            drawchar(x0, y0);
            if (x0 == x1 && y0 == y1) break;
            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }
    void fillcircle(int x0, int y0, int radius)
    {
        int f = 1 - radius;
        int ddF_x = 1;
        int ddF_y = -2 * radius;
        int x = 0;
        int y = radius;

        drawchar(x0, y0 + radius);
        drawchar(x0, y0 - radius);
        bresline(x0 + radius, y0, x0 - radius, y0);

        while (x < y)
        {
            if (f >= 0)
            {
                y--;
                ddF_y += 2;
                f += ddF_y;
            }
            x++;
            ddF_x += 2;
            f += ddF_x;
            bresline(x0 + x, y0 + y, x0 - x, y0 + y);
            bresline(x0 + x, y0 - y, x0 - x, y0 - y);
            bresline(x0 + y, y0 + x, x0 - y, y0 + x);
            bresline(x0 + y, y0 - x, x0 - y, y0 - x);
        }
    }
    void mirror_lr()
    {
        for (int y = 0; y < cells.height; y++)
        {
            for (int x = 0; x < cells.width / 2; x++)
            {
                cells.AtSet(cells.width - 1 - x, y, cells.AtGet(x, y));
            }
        }
    }
    void mirror_rl()
    {
        for (int y = 0; y < cells.height; y++)
        {
            for (int x = 0; x < cells.width / 2; x++)
            {
                cells.AtSet(x, y, cells.AtGet(cells.width - 1 - x, y));
            }
        }
    }
    void mirror_du()
    {
        for (int x = 0; x < cells.width; x++)
        {
            for (int y = 0; y < cells.height / 2; y++)
            {
                cells.AtSet(x, y, cells.AtGet(x, cells.height - 1 - y));
            }
        }
    }
    void mirror_ud()
    {
        for (int x = 0; x < cells.width; x++)
        {
            for (int y = 0; y < cells.height / 2; y++)
            {
                cells.AtSet(x, cells.height - 1 - y, cells.AtGet(x, y));
            }
        }
    }
    public bool CellCountTest()
    {
        //returns true if there are 4 cells or more
        int cellcount = 0;
        for (int y = 0; y < cells.height; y++)
        {
            for (int x = 0; x < cells.width; x++)
            {
                if (cells.AtGet(x, y) == Etilesprite.MAP_STONE_FLOOR_RUIN)
                {//2016 was floor
                    cellcount++;
                    if (cellcount > 3) return true;
                }
            }
        }
        return false;
    }
    public void patchfill()
    {

        //each patch has 1-3 shapes within it
        byte num_shapes_within_patch = lil.randb(1, 6);
        if (num_shapes_within_patch > 3) num_shapes_within_patch = 1;

        for (; num_shapes_within_patch > 0; num_shapes_within_patch--)
        {

            //each shape is either a rect or a circle at the moment
            byte shape = lil.randb(1, 6); //was 15
            switch (shape)
            {
                case 1:
                    //circle
                    {
                        int r = lil.randi(2, ((cells.width - 2) / 2));
                        fillcircle(
                            lil.randi(r, cells.width - r - 1),
                            lil.randi(r, cells.width - r - 1),
                            r);
                    }
                    break;
                case 2:
                //quad
                //	break;
                case 3:
                //triangle
                //	break;
                case 4:
                //ngon 3-8
                //	break;
                case 5:
                //break;
                case 6:
                    //rect
                    {
                        int w = lil.randi(2, cells.width);
                        int h = lil.randi(2, cells.width);
                        fillrect(
                            lil.randi(0, cells.width - w),
                            lil.randi(0, cells.height - h),
                            w, h);
                    }
                    break;
            }
        }
        //sometimes, take half the patch and mirror it
        if (lil.randb(1, 6) == 1)
        {
            byte x = lil.randb(1, 6);
            switch (x)
            {
                case 1://mirror left side onto right
                    mirror_lr();
                    break;
                case 2://mirror right side onto left
                    mirror_rl();
                    break;
                case 3://mirror top half to bottom
                    mirror_ud();
                    break;
                case 4://mirror bottom half to top
                    mirror_du();
                    break;
                case 5:
                case 6://mirror both ways. there's 4 ways to do that
                    {
                        byte which = lil.randb(1, 4);
                        switch (which)
                        {
                            case 1:
                                mirror_lr(); mirror_ud();
                                break;
                            case 2:
                                mirror_rl(); mirror_ud();
                                break;
                            case 3:
                                mirror_lr(); mirror_du();
                                break;
                            case 4:
                                mirror_rl(); mirror_du();
                                break;
                        }
                    }
                    break;
            }
        }
    }

}


public partial class RLMap
{
    int shootray(int x, int y, int deltax, int deltay, Etilesprite seek)
    {
        int currentx = x, currenty = y;
        while (true)
        {
            currentx += deltax; currenty += deltay;
            if (currentx < 0 || currenty < 0 || currentx >= width || currenty >= height) return -1;
            if (displaychar.AtGet(currentx, currenty) == seek)
            {
                return ((deltax != 0) ? Math.Abs(x - currentx) : Math.Abs(y - currenty));
            }
        }
    }
    bool shootrays(int x, int y, Etilesprite seek, out int outx, out int outy, out sbyte direction, bool fudge = false)
    {
        List<sbyte> v = new List<sbyte>();
        int lowest = 999;
        for (sbyte f = 0; f < 4; f++)
        {
            int temp = shootray(x, y, lil.deltax[f], lil.deltay[f], seek);
            if (temp != -1)
            {
                if (temp < lowest)
                {
                    lowest = temp;
                    v.Clear();
                }
                if (temp <= lowest)
                {
                    v.Add(f);
                }
            }
        }
        if (lowest == 999) { outx = -1; outy = -1; direction = 0; return false; }
        direction = v.randmember();
        if (fudge) lowest--;

        outx = x + (lowest * lil.deltax[direction]);
        outy = y + (lowest * lil.deltay[direction]);
        return true;

    }

    /*
    public void genlevelsplitterstyle() {
        BSPDungeon bspd;
        do {
            bspd = new BSPDungeon(width, height, 8);
        }
        while (bspd.hasfailed == true);
        
        emptyspaces = new List<Cell>();

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {

                switch (bspd.map[x, y]) {
                    case mapTileType.MTT_NIL:
                    case mapTileType.MTT_WALL:
                        displaychar.AtSet(x, y, Etilesprite.WALL);
                        passable.AtSet(x, y, false);
                        blocks_sight.AtSet(x, y, true);
                        break;

                    case mapTileType.MTT_DOOR_CLOSED_HORIZONTAL:
                    case mapTileType.MTT_DOOR_OPEN_HORIZONTAL:
                        displaychar.AtSet(x, y, Etilesprite.DOOR_H_OPEN);
                        passable.AtSet(x, y, true);
                        blocks_sight.AtSet(x, y, false);
                        emptyspaces.Add(new Cell(x, y));
                        break;
                    case mapTileType.MTT_DOOR_CLOSED_VERTICAL:
                    case mapTileType.MTT_DOOR_OPEN_VERTICAL:
                        displaychar.AtSet(x, y, Etilesprite.DOOR_V_OPEN);
                        passable.AtSet(x, y, true);
                        blocks_sight.AtSet(x, y, false);
                        emptyspaces.Add(new Cell(x, y));
                        break;

                    case mapTileType.MTT_ROOM:
                        displaychar.AtSet(x, y, Etilesprite.FLOOR);
                        passable.AtSet(x, y, true);
                        blocks_sight.AtSet(x, y, false);
                        emptyspaces.Add(new Cell(x, y));
                        break;
                    case mapTileType.MTT_CORRIDOR:
                        displaychar.AtSet(x, y, Etilesprite.CORRIDOR);
                        passable.AtSet(x, y, true);
                        blocks_sight.AtSet(x, y, false);
                       // emptyspaces.Add(new Cell(x, y));
                        break;

                }

               
            }

        }
        emptyspaces.Shuffle();

    }
    public void genlevelsuckerstyle() {
        const int numberofpatches=75;
        const int patchwidth =10;
        const int patchheight=10;

        emptyspaces = new List<Cell>(); 
        List<Patch> patches=new List<Patch>();
	    Patch patch;

	//create a set of patches. a patch is a small area representing one room. it's made of 1-3 shapes: rects or circles
	//the shapes are randomly positioned in the patch to make interesting room shapes. They are guaranteed to have at 
	//least 4 cells of "room" and to be connected.
	for (int f = 0; f < numberofpatches; f++){
		patch = new Patch(patchwidth, patchheight);							//make a new patch and
		patches.Add(patch);											//add it to the list of patches
		patch.posx = (width - patchwidth) / 2;							//position patch in the middle of the map
		patch.posy = (height - patchheight) / 2;
		float degrees = (float)f * (360.0f / (float) numberofpatches);		//give patch a trajectory so they move away from the middle
		float radians = degrees*0.0174532925f;
		patch.deltax = (float)Math.Cos(radians);
		patch.deltay = (float)Math.Sin(radians);										
	tryagain:
		patch.patchfill();													//put room shapes into the patch
		if (!patch.CellCountTest())goto tryagain;							//must have >=4 cells 
		//if (!patch->FloodConnectTest())goto tryagain;						//all cells must be connected
		if (!patch.cells.FloodTest(Etilesprite.FLOOR, Etilesprite.FLOODTEMP))goto tryagain;
	}
		
	//now we actually have our set of patches, move them outwards and 
	//stamp them down when they don't overlap
	//RLMap patchmap(width, height);	
	int num_left = numberofpatches;

	while (num_left > 0){
	//	int upto = -1; print(" ");
		foreach (var p in patches){
		//	upto++;
		//	std::cout << upto << ((p->stamped_down) ? "SD" : "--") << "  " << ((p->out_of_bounds) ? "OOB" : "--" )
			//	<< p->posx << " " << p->posy  << std::endl;
			if (!p.stamped_down && !p.out_of_bounds){//if this patch is still moving
				for (int celly = 0; celly < patchheight; celly ++){
					for (int cellx = 0; cellx < patchwidth; cellx ++){//go through each cell
						if(p.cells.AtGet(cellx,celly) == Etilesprite.FLOOR){//if the cell is a "room" cell
							int mapx = (int)p.posx + cellx;
							int mapy = (int)p.posy + celly;//get map pos under this cell
							if (mapx < 0 || mapy < 0 || mapx >= width || mapy >= height){//if map cell under this cell=out of bounds
								p.out_of_bounds = true;//set this patch out of bounds to true
								//print("out of bounds");
								num_left--;//one fewer to process
								goto NEXT_PATCH;//break to next patch
							}//if mapx or mapy out of bounds
                            if (displaychar.AtGet(mapx, mapy) != Etilesprite.EMPTY)    {     //WAS 0!!! {//if the map underneath this cell is already occupied
                                goto MOVE_PHASE;                //can't stamp down this turn. break to move phase
                            } 
						}//if cell is room cell
					}//next cellx
				}//next celly
				//having got through every cell of the patch without breaking
				for (int celly = 0; celly < patchheight; celly++){          //stampdown! update map with patch
					for (int cellx = 0; cellx < patchwidth; cellx++){
						if (p.cells.AtGet(cellx, celly) == Etilesprite.FLOOR){
                            displaychar.AtSet((int)p.posx + cellx, (int)p.posy + celly
                                , Etilesprite.FLOOR);                       //, p.cells.AtGet(cellx, celly));
						}
					}
				}
				//print("stampdown");
				p.stamped_down = true;//set stamped down to true
				num_left--;//decrease num_left
				MOVE_PHASE://move phase
				p.posx += p.deltax;//move the patch along its trajectory
				p.posy += p.deltay;

			}//if not p stamped down and p out of bounds
		NEXT_PATCH:;
		}//auto p in patches
	}


    
    for (int y = 0; y < height; y++)
	{
		for (int x = 0; x < width; x++)
		{
			if (displaychar.AtGet(x, y) == Etilesprite.EMPTY){
				displaychar.AtSet(x, y, Etilesprite.WALL);
				passable.AtSet(x, y, false);
				blocks_sight.AtSet(x, y, true);
			}
			else {//change this when you have stuff other than wall and floor
				passable.AtSet(x, y, true);
				blocks_sight.AtSet(x, y, false);
				emptyspaces.Add(new Cell(x,y));
			}
		}
 
	}   
	emptyspaces.Shuffle();
    

	//cleanup
    patches.Clear();
	

	//add static lights
	for (int ly = 0; ly < this.height; ly += 10){
		for (int lx = 0; lx < this.width; lx += 10){
			int tentx = lx + lil.randi(0, 9);
			int tenty = ly + lil.randi(0, 9);
			if (tentx >= this.width || tenty >= this.height)Debug.Log("ERROR OUT OF BOUNDS");
			//move light to wall
			int outx, outy;
			sbyte direction;
			//bool suc;
			if (displaychar[tentx, tenty] == Etilesprite.WALL){
				if (shootrays(tentx, tenty, Etilesprite.FLOOR, out outx, out outy, out direction, true)){//DODGY - WAS ' '
					displaychar[outx, outy]=lil.dirchar[direction];
					do_fov_foralight(outx, outy, 9,walllight,direction);//255,255,128 pampkin 255, 117, 24
				}
				
			}
			else {
				if (shootrays(tentx, tenty, Etilesprite.WALL, out outx, out outy, out direction)){
					displaychar[outx, outy]= lil.dirchar_rev[direction];
					do_fov_foralight(outx, outy, 9,  walllight , lil.opdir[direction]);//255,255,128
				}
			}
			
		}
	}

	//debug test maziacs
	//if (PathfindAStar(player.posx, player.posy, x, y)){
	//	for (auto& f : lastpath){
	//		displaychar.at(f.x, f.y) = 'L';
	//	}
	//}
	//


	//PLACE STUFF ITEMS PLAYER MOBS EXIT ETC

	//current cheats 20 extra swords, 20 extra imps
	//additem(20, Eitemtype::ITEM_SWORD);
	//addmob(20, Eitemtype::MOB_ESR);
	//addmob(20, Eitemtype::MOB_SKEL);
	//additem(20, Eitemtype::ITEM_BATTERY);
	//additem(20, Eitemtype::ITEM_MEDPACK);

	//player.hp = 10;


	//place player
	//freespace(player.posx, player.posy);
	//player.level++;
	//player.turns = 0;

	//place exit
	//int x, y;
	//freespace(x, y);
	//displaychar.at(x, y) = 'E';
	//locked.set(x, y, true);

	//place rabbit gate
	//freespace(x, y);
	//displaychar.at(x, y) = 'G';
	//rabbitgatex = x, rabbitgatey = y;

	//place broom cupboard containerizing broom
	//freespace(x, y);
	//displaychar.at(x, y) = 'C';
	//locked.set(x, y, true);


	//place chest containerizing golden tziken on level 10
	//if (player.level == 10){
	//	freespace(x, y);
	//	displaychar.at(x, y) = 'c';
	//	locked.set(x, y, true);
	//}
	

	//add 20 mobs
	//TODO distance from player check
	//for (int f = 0; f < 20; f++){
	//	addmob(1,(Eitemtype) lil::rand( (int)Eitemtype::MOB_SKEL, (int)Eitemtype::MOB_BAT ));
	//	addmob(1, Eitemtype::MOB_KOBBY);
	//}

	//add 20 items
	//5 gems
	//additem(5, Eitemtype::ITEM_GEM);
	//10 gold
	//additem(10, Eitemtype::ITEM_GOLD);
	//2 keys on level 1-9, 3 on 10
	//additem((player.level==10)?3:2, Eitemtype::ITEM_KEY);
	//3 random items from batteries/shield/sword/stopwatch/medpack/junk
	//for (size_t i = 0; i < 3; i++)
	//{
		
	//	additem(1, (Eitemtype) lil::rand((int)Eitemtype::ITEM_BATTERY, (int)Eitemtype::ITEM_JUNK));
	//}

	//QuickdumpToConsole();
	//floodtest it
	//bool itpassed = this->displaychar.floodtest(' ', 1);
	//std::cout << (itpassed ? "pass " : "fail ") << std::endl;
	//if (!itpassed){
	//	displaychar.ReplaceXWithY(1, ' ');
	//}
}
*/

    public void genlevelskaterstyle()
    {

        //handy item/powerup balancing in terms of numbers
        const int NUMBEROF_CAIRNS = 5;
        //barrels have a min and a max because the number of barrels with items in is set separately to the number of barrels
        const int NUMBEROF_BARRELS_MIN = 5;
        const int NUMBEROF_BARRELS_MAX = 20;
        int NUMBEROF_BARRELS = lil.randi(NUMBEROF_BARRELS_MIN, NUMBEROF_BARRELS_MAX);
        int NUMBEROF_BARRELS_THAT_HAVE_ITEMS = lil.randi(1,NUMBEROF_BARRELS);
        Etilesprite[] barrelitems = {            
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
        //number of moops on level is not set, hence number of moop crates not set, but:
        const int PERCENT_CHANCE_MOOP_CRATE_NOT_EMPTY = 30;//this needs to be somewhere more scoped out.

        //
        emptyspaces = new List<Cell>();

        regen:

        float r = lil.randf(-1000, 1000);
        float r2 = lil.randf(-1000, 1000);

        int highpoint_x = 0, highpoint_y = 0;
        float high = -100;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {


                float n = SimplexNoise.Noise.Generate(((float)x) / width, ((float)y) / height, r);//-1 to +1 i think
                float n2 = SimplexNoise.Noise.Generate(((float)x) / 1.0f, ((float)y) / 1.0f, r2);


                if (n > high)
                {
                    high = n;
                    highpoint_x = x; highpoint_y = y;
                }

                if (n < -0.7)
                {
                    displaychar.AtSet(x, y, Etilesprite.MAP_WATER);
                    passable.AtSet(x, y, false);
                    blocks_sight.AtSet(x, y, false);

                }
                else if (n < -0.5)
                {
                    if (n2 > 0 && n2 < 0.00001)
                    {
                        displaychar.AtSet(x, y, Etilesprite.ITEM_SIGN_THIN_ICE);
                        passable.AtSet(x, y, false);
                        blocks_sight.AtSet(x, y, false);
                    }
                    else {
                        displaychar.AtSet(x, y, Etilesprite.MAP_THIN_ICE);
                        passable.AtSet(x, y, true);
                        blocks_sight.AtSet(x, y, false);
                    }
                }
                else if (n < 0.3)
                {
                    //  if (lil.randi(1, 200) > 199)
                    //  {
                    //      displaychar.AtSet(x, y, Etilesprite.ITEM_LANTERN_ON_A_STICK_FOR_NO_REASON);
                    //      passable.AtSet(x, y, false);
                    //      blocks_sight.AtSet(x, y, false);
                    //  }
                    //  else
                    // {
                    displaychar.AtSet(x, y, Etilesprite.MAP_ICE);
                    passable.AtSet(x, y, true);
                    blocks_sight.AtSet(x, y, false);
                    emptyspaces.Add(new Cell(x, y));
                    // }
                }
                else if (n < 0.9)
                {
                    if (n2 > 0.9)
                    {
                        displaychar[x, y] = Etilesprite.MAP_SNOW;
                        itemgrid[x, y] = new item_instance(Etilesprite.MAP_TREE_BARE_1 + lil.randi(0, 3));
                        passable.AtSet(x, y, false);
                        blocks_sight.AtSet(x, y, true);
                    }
                    else {
                        displaychar.AtSet(x, y, Etilesprite.MAP_SNOW);
                        passable.AtSet(x, y, true);
                        blocks_sight.AtSet(x, y, false);
                        emptyspaces.Add(new Cell(x, y));
                    }
                }
                else if (n < 0.91)
                {
                    displaychar.AtSet(x, y, Etilesprite.MAP_HENGE_STONE_1 + lil.randi(0, 3));
                    passable.AtSet(x, y, false);
                    blocks_sight.AtSet(x, y, true);
                }
                else
                {
                    displaychar.AtSet(x, y, Etilesprite.MAP_SNOW);
                    passable.AtSet(x, y, true);
                    blocks_sight.AtSet(x, y, false);
                    emptyspaces.Add(new Cell(x, y));
                }

            }
        }//end of xy loop

        displaychar.AtSet(highpoint_x, highpoint_y, Etilesprite.ITEM_WARP_GATE_ANIM_1);
        passable.AtSet(highpoint_x, highpoint_y, false);
        blocks_sight.AtSet(highpoint_x, highpoint_y, true);
        //need to remove this square from emptyspaces!! urgent
        // Debug.Log("empty spaces " + emptyspaces.Count);
        emptyspaces.RemoveAll(i => i.x == highpoint_x && i.y == highpoint_y);
        //Debug.Log("empty spaces NOW " + emptyspaces.Count);
        //do_fov_foralight(highpoint_x, highpoint_y, 3, gatelight);//do all lights at end of map gen



        if (emptyspaces.Count < 1)
        {
            Debug.Log("No empty spaces-all water?!");
            goto regen;
        }
        emptyspaces.Shuffle();

        var emptyspaces2 = new List<Cell>();

        //add snow-topped rocks
        for (int ly = 0; ly < this.height; ly += 10)
        {
            for (int lx = 0; lx < this.width; lx += 10)
            {
                //  int tentx = lx + lil.randi(0, 9);
                //  int tenty = ly + lil.randi(0, 9);
                int tentx, tenty;
                FreeSpace(out tentx, out tenty);

                if (tentx >= this.width || tenty >= this.height) Debug.Log("ERROR OUT OF BOUNDS");
                //move rock to snow/ice border
                int outx, outy;
                sbyte direction;
                //bool suc;
                if (displaychar[tentx, tenty] == Etilesprite.MAP_SNOW)
                {
                    if (shootrays(tentx, tenty, Etilesprite.MAP_ICE, out outx, out outy, out direction, true)
                        && displaychar[outx,outy]==Etilesprite.MAP_SNOW)//new. only put boulder there if it's blank
                    {//DODGY - WAS ' '
                        displaychar[outx, outy] = Etilesprite.MAP_SNOW_COVERED_ROCK_1 + lil.randi(0, 3);
                        blocks_sight[outx, outy] = true;
                        passable[outx, outy] = false;
                        ///bookmark george takkei
                        emptyspaces.RemoveAll(thing => thing.x == outx && thing.y == outy);//remove freespace where we put snow rock
                        if(tentx!=outx || tenty!=outy)
                            emptyspaces2.Add(new Cell(tentx, tenty));//put original placing of rock back if it's not where it ended up
                        //do_fov_foralight(outx, outy, 3, walllight);

                    }

                }
                else {
                    if (shootrays(tentx, tenty, Etilesprite.MAP_SNOW, out outx, out outy, out direction)
                        && displaychar[outx, outy] == Etilesprite.MAP_SNOW)//new. only put boulder there if it's blank
                    {
                        displaychar[outx, outy] = Etilesprite.MAP_SNOW_COVERED_ROCK_1 + lil.randi(0, 3);
                        blocks_sight[outx, outy] = true;
                        passable[outx, outy] = false;
                        // do_fov_foralight(outx, outy, 3, walllight);
                        emptyspaces.RemoveAll(thing => thing.x == outx && thing.y == outy);//remove freespace where we put snow rock
                        if (tentx != outx || tenty != outy)
                            emptyspaces2.Add(new Cell(tentx, tenty));//put original placing of rock back if it's not where it ended up

                    }
                }

            }
        }

        //deal with spare spaces
        //Debug.Log("first " + emptyspaces.Count);
        emptyspaces.AddRange(emptyspaces2);
        emptyspaces.Shuffle();
        emptyspaces2.Clear();
        //Debug.Log("second " + emptyspaces.Count);

        //do cairns
        int cx, cy;

        for (int i = 0; i < NUMBEROF_CAIRNS; i++)
        {
            FreeSpace(out cx, out cy);
            passable[cx, cy] = false;
            blocks_sight[cx, cy] = true;
            int whichcairn = lil.randi(0, 3);
            itemgrid[cx, cy] = new item_instance(Etilesprite.ITEM_CAIRN_RED + whichcairn);
            if (whichcairn == 3)
            {
                //if random cairn chosen was purple then make a twin to teleport to
                int a, b;
                FreeSpace(out a, out b);
                itemgrid[a, b] = new item_instance(Etilesprite.ITEM_CAIRN_PURPLE);
                passable[a, b] = false;
                blocks_sight[a, b] = true;
                //use extradata to point cairns to each other
                extradata[cx, cy] = new Cell(a, b);
                extradata[a, b] = new Cell(cx, cy);
            }
        }


        //0-3 buildings
        //keep track of spaces generated in a building
        List<Cell> spaceinbild = new List<Cell>();

        int numbilds = lil.randi(0, 3);
        for (int bilds = 0; bilds < numbilds; bilds++)
        {


            //stone building
            Patch patch = new Patch(10, 10);                         //make a new patch and
            patch.posx = (width - 10) / 2;                          //position patch in the middle of the map
            patch.posy = (height - 10) / 2;
            tryagain:
            patch.patchfill();                                                  //put room shapes into the patch
            if (!patch.CellCountTest())
            {
                //Debug.Log("cellcount fail"); 
                goto tryagain;                          //must have >=4 cells 
            }
            //if (!patch->FloodConnectTest())goto tryagain;						//all cells must be connected
            if (!patch.cells.FloodTest(Etilesprite.MAP_STONE_FLOOR_RUIN, Etilesprite.FLOODTEMP))
            {
                //Debug.Log("Floodtest fail");
                goto tryagain;
            }
            //copy patch into bigger patch to ensure walls all the way round
            Patch newpatch = new Patch(12, 12);
            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    newpatch.cells[x + 1, y + 1] = patch.cells[x, y];
                }
            }
            //remove unneeded walls
            for (int y = 0; y < 12; y++)
            {
                for (int x = 0; x < 12; x++)
                {
                    if (newpatch.cells[x, y] == Etilesprite.MAP_STONE_WALL_RUIN &&
                    !newpatch.AnyMatching(x, y, Etilesprite.MAP_STONE_FLOOR_RUIN))
                        newpatch.cells[x, y] = Etilesprite.EMPTY;
                }
            }
            //make a list of candidate wall sections that can become a door
            List<Cell> cl = new List<Cell>();
            for (int y = 0; y < 12; y++)
            {
                for (int x = 0; x < 12; x++)
                {
                    if (newpatch.cells[x, y] == Etilesprite.MAP_STONE_WALL_RUIN)
                    {
                        int count = 0;
                        if (x > 0 && newpatch.cells[x - 1, y] == Etilesprite.MAP_STONE_FLOOR_RUIN) count++;
                        if (y > 0 && newpatch.cells[x, y - 1] == Etilesprite.MAP_STONE_FLOOR_RUIN) count++;
                        if (x < newpatch.cells.width - 1 && newpatch.cells[x + 1, y] == Etilesprite.MAP_STONE_FLOOR_RUIN) count++;
                        if (y < newpatch.cells.height - 1 && newpatch.cells[x, y + 1] == Etilesprite.MAP_STONE_FLOOR_RUIN) count++;
                        if (count == 1) cl.Add(new Cell(x, y));
                    }
                }
            }
            //pick a door

            if (cl.Count == 0) { Debug.Log("ERROR NO CANDIDATE WALL SECTION TO USE AS DOOR FOUND"); }
            else
            {
                Cell bar = lil.randmember(cl);
                newpatch.cells[bar.x, bar.y] = Etilesprite.EMPTY;
                //place lanterns
                for (int y = bar.y-1; y < bar.y+2; y++)
                {
                    for (int x = bar.x-1; x < bar.x+2; x++)
                    {
                        if (x > -1 && y > -1 && x < newpatch.cells.width && y < newpatch.cells.height
                            && newpatch.cells[x, y] == Etilesprite.EMPTY && x != bar.x && y != bar.y)
                            newpatch.cells[x, y] = Etilesprite.ITEM_LANTERN_ON_A_STICK_FOR_NO_REASON;
                    }
                }
            }



            //attempt to stamp down the ruin onto the map
            //each square has to be snow or snow-topped rock and there has to be no item (which so far is tree or cairn
            // int candx = lil.randi(0, width - newpatch.cells.width - 1);
            // int candy = lil.randi(0, height - newpatch.cells.height - 1);

            //make a list of places the building could go
            List<Cell> candspaces = new List<Cell>();

            for (int candy = 0; candy < height - newpatch.cells.height; candy++)
            {
                for (int candx = 0; candx < width - newpatch.cells.width; candx++)
                {
                    bool violation = false;
                    for (int y = 0; y < 12; y++)
                    {
                        for (int x = 0; x < 12; x++)
                        {
                            if (displaychar[x + candx, y + candy] != Etilesprite.MAP_SNOW && displaychar[x + candx, y + candy] != Etilesprite.MAP_SNOW_COVERED_ROCK_1)
                            { violation = true; goto pangos_considered_harmful; }
                            if (itemgrid[x + candx, y + candy] != null)
                            { violation = true; goto pangos_considered_harmful; }
                        }
                    }
                    pangos_considered_harmful:
                    if (!violation) candspaces.Add(new Cell(candx, candy));
                }
            }
            // Debug.Log("number of cand spaces=" + candspaces.Count);

            //Debug.Log("free1 " + emptyspaces.Count);
            if (candspaces.Count > 0)
            {
                Cell cc = lil.randmember(candspaces);//pick a candidate space at random
                //stamp it down
                for (int y = 0; y < 12; y++)
                {
                    for (int x = 0; x < 12; x++)
                    {
                        if (newpatch.cells[x, y] != Etilesprite.EMPTY)
                        {
                            emptyspaces.RemoveAll(i => i.x == x + cc.x && i.y == y + cc.y);  //remove square if it exists in the freespaces list
                            if (newpatch.cells[x, y] == Etilesprite.ITEM_LANTERN_ON_A_STICK_FOR_NO_REASON)
                            {
                                itemgrid[x+cc.x, y+cc.y] = new item_instance(Etilesprite.ITEM_LANTERN_ON_A_STICK_FOR_NO_REASON);
                            } else
                            {
                                displaychar[x + cc.x, y + cc.y] = newpatch.cells[x, y];
                            }
                            
                            if (newpatch.cells[x, y] == Etilesprite.MAP_STONE_WALL_RUIN||newpatch.cells[x,y]==Etilesprite.ITEM_LANTERN_ON_A_STICK_FOR_NO_REASON)
                            {
                                passable[x + cc.x, y + cc.y] = false;
                                blocks_sight[x + cc.x, y + cc.y] = true;
                            }
                            else spaceinbild.Add(new Cell(x + cc.x, y + cc.y));


                        }

                    }
                }
            }
            // Debug.Log("free2 " + emptyspaces.Count);

        }

        if (spaceinbild.Count > 0) spaceinbild.Shuffle();

        //let's do barrels. barrels are fun. i love barrels. do you love barrels, broom? broom loves barrels.
        List<Cell> barrels = new List<Cell>();
        bool outsidevirgin = true;
        int lastoutx = 0; int lastouty = 0;

        while (NUMBEROF_BARRELS > 0)
        {

            int tx, ty;
            if (spaceinbild.Count > 0)
            {
                Cell c = spaceinbild.OneFromTheTop();
                tx = c.x; ty = c.y;
            }
            else
            {
                if (!outsidevirgin && lil.randi(1, 100) > 25)
                {
                    Cell ctt = Random9way(lastoutx, lastouty);
                    if (ctt == null)
                    {
                        FreeSpace(out tx, out ty);
                    }
                    else { tx = ctt.x; ty = ctt.y; emptyspaces.RemoveAll(i => i.x == tx && i.y == ty); }
                }
                else {
                    FreeSpace(out tx, out ty);
                    outsidevirgin = false;
                    // lastoutx = tx; lastouty = ty;
                }
                lastoutx = tx; lastouty = ty;
            }

            itemgrid[tx, ty] = new item_instance(Etilesprite.ITEM_BARREL);
            passable[tx, ty] = false;
            barrels.Add(new Cell(tx, ty));
            NUMBEROF_BARRELS--;
        }

        if (barrels.Count == 0)
        {
            Debug.Log("suspiciously there are no barrels in the list of barrels");
        }
        else
        {
            //fill barrels
            barrels.Shuffle();//CRASH
            for (int i = 0; i < NUMBEROF_BARRELS_THAT_HAVE_ITEMS; i++)
            {
                int w;

                //w = (i == 0) ? (int)Etilesprite.ITEM_WARP_BEADS : (int)item_instance.possible_barrel_contents.randmember();
                w = (int)Etilesprite.ENEMY_HOPPED_UP_FOX;
                //debug always fochs
                Cell c = barrels.OneFromTheTop();
                extradata[c.x, c.y] = new Cell(w, w);
                //so a barrel with extradata of 1,1 has something in it and extradata of null means empty! yay magic numbers.
                //revision, using co-ord to store which item is in it
            }
        }

        //map all done- generate the static light map
        dostaticlights();



        //place mobs
        const int MOBS_NUMBER_OF = 15;
        for (int i = 0; i < MOBS_NUMBER_OF; i++)
        {
            int x = 0;
            int which = lil.randi(1, 100);
            if (which > 95)//95
            {
                 x = lil.randi(0, 2)+(int)Emobtype.kobbybomber;                
            } else
            {
                 x = lil.randi(0, 5)+(int)Emobtype.polarmoop;
            }
            mob m = new mob((Emobtype)x);
            //mob m = new mob(Emobtype.kobbybomber);//DEBUG
            int tentx, tenty;
            FreeSpace(out tentx, out tenty);
           
            m.posx = tentx;m.posy = tenty;
            itemgrid[tentx, tenty] = new item_instance(m.tile,true,m);
            passable[tentx, tenty] = false;
            moblist.Add(m);

        }

        //displaychar[outx, outy] = Etilesprite.ITEM_LANTERN_ON_A_STICK_FOR_NO_REASON;
        //do_fov_foralight(outx, outy, 3, walllight);






        //cleanup


        //debug test maziacs
        //if (PathfindAStar(player.posx, player.posy, x, y)){
        //	for (auto& f : lastpath){
        //		displaychar.at(f.x, f.y) = 'L';
        //	}
        //}
        //


        //PLACE STUFF ITEMS PLAYER MOBS EXIT ETC

        //current cheats 20 extra swords, 20 extra imps
        //additem(20, Eitemtype::ITEM_SWORD);
        //addmob(20, Eitemtype::MOB_ESR);
        //addmob(20, Eitemtype::MOB_SKEL);
        //additem(20, Eitemtype::ITEM_BATTERY);
        //additem(20, Eitemtype::ITEM_MEDPACK);

        //player.hp = 10;


        //place player
        //freespace(player.posx, player.posy);
        //player.level++;
        //player.turns = 0;

        //place exit
        //int x, y;
        //freespace(x, y);
        //displaychar.at(x, y) = 'E';
        //locked.set(x, y, true);

        //place rabbit gate
        //freespace(x, y);
        //displaychar.at(x, y) = 'G';
        //rabbitgatex = x, rabbitgatey = y;

        //place broom cupboard containerizing broom
        //freespace(x, y);
        //displaychar.at(x, y) = 'C';
        //locked.set(x, y, true);


        //place chest containerizing golden tziken on level 10
        //if (player.level == 10){
        //	freespace(x, y);
        //	displaychar.at(x, y) = 'c';
        //	locked.set(x, y, true);
        //}


        //add 20 mobs
        //TODO distance from player check
        //for (int f = 0; f < 20; f++){
        //	addmob(1,(Eitemtype) lil::rand( (int)Eitemtype::MOB_SKEL, (int)Eitemtype::MOB_BAT ));
        //	addmob(1, Eitemtype::MOB_KOBBY);
        //}

        //add 20 items
        //5 gems
        //additem(5, Eitemtype::ITEM_GEM);
        //10 gold
        //additem(10, Eitemtype::ITEM_GOLD);
        //2 keys on level 1-9, 3 on 10
        //additem((player.level==10)?3:2, Eitemtype::ITEM_KEY);
        //3 random items from batteries/shield/sword/stopwatch/medpack/junk
        //for (size_t i = 0; i < 3; i++)
        //{

        //	additem(1, (Eitemtype) lil::rand((int)Eitemtype::ITEM_BATTERY, (int)Eitemtype::ITEM_JUNK));
        //}

        //QuickdumpToConsole();
        //floodtest it
        //bool itpassed = this->displaychar.floodtest(' ', 1);
        //std::cout << (itpassed ? "pass " : "fail ") << std::endl;
        //if (!itpassed){
        //	displaychar.ReplaceXWithY(1, ' ');
        //}
    }
}
