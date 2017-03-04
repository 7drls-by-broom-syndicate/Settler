using UnityEngine;
using System.Collections;
using System.Collections.Generic;

enum mapTileType {
    MTT_NIL, MTT_WALL, MTT_DOOR_CLOSED_HORIZONTAL, MTT_DOOR_CLOSED_VERTICAL, MTT_BLOKERIZE, //blokerizing
    MTT_CORRIDOR, MTT_ROOM, MTT_STAIRS_DOWN, MTT_DOOR_OPEN_VERTICAL, MTT_DOOR_OPEN_HORIZONTAL, //non-blokerizing
    MTT_END
}; //marker for end

enum splitDirType { HORIZ, VERT };

class BSPDungeon {

    public Array2D<mapTileType> map;
    public int width, height, depth;
    //public List<List<BSPDungeonNode>> letsjoinlist;
    public List<BSPDungeonNode>[] letsjoinlist;
    BSPDungeonNode nodes;
    public bool hasfailed;

    public BSPDungeon(int _width, int _height, int _max_depth) {
        width = _width; height = _height; depth = _max_depth; hasfailed = false;

        //letsjoinlist = new List<List<BSPDungeonNode>>(depth);
        letsjoinlist = new List<BSPDungeonNode>[depth];
        for (int f = 0; f < depth;f++ )letsjoinlist[f]=new List<BSPDungeonNode>();
        

            map = new Array2D<mapTileType>(width, height);

        nodes = new BSPDungeonNode(this, null, 0, 0, width - 1, height - 1);
        //walls
        if (!hasfailed) {
            for (int g = 0; g < height; g++)
                for (int f = 0; f < width; f++)
                    if (map[f, g] == mapTileType.MTT_NIL &&
                        (
                            (f > 0 && map[f - 1, g] >= mapTileType.MTT_CORRIDOR)
                            || ((f + 1) < width && map[f + 1, g] >= mapTileType.MTT_CORRIDOR)
                            || (g > 0 && map[f, g - 1] >= mapTileType.MTT_CORRIDOR)
                            || ((g + 1) < height && map[f, g + 1] >= mapTileType.MTT_CORRIDOR)
                        )
                    )
                        map[f, g] = mapTileType.MTT_WALL;
            //doors
            for (int g = 1; g < height - 1; g++)
                for (int f = 1; f < width - 1; f++)
                    if (map[f, g] == mapTileType.MTT_CORRIDOR) {
                        if (((map[f - 1, g] == mapTileType.MTT_ROOM && map[f, g - 1] == mapTileType.MTT_WALL && map[f, g + 1] == mapTileType.MTT_WALL)) //room to left
                            || ((map[f + 1, g] == mapTileType.MTT_ROOM && map[f, g - 1] == mapTileType.MTT_WALL && map[f, g + 1] == mapTileType.MTT_WALL)))//room to right
                            map[f, g] = mapTileType.MTT_DOOR_CLOSED_VERTICAL;
                        else if (((map[f, g - 1] == mapTileType.MTT_ROOM && map[f - 1, g] == mapTileType.MTT_WALL && map[f + 1, g] == mapTileType.MTT_WALL))//room above
                            || ((map[f, g + 1] == mapTileType.MTT_ROOM && map[f - 1, g] == mapTileType.MTT_WALL && map[f + 1, g] == mapTileType.MTT_WALL)))//room below
                            map[f, g] = mapTileType.MTT_DOOR_CLOSED_HORIZONTAL;
                    }


        }


        //remove_walls();
        //show_to_console();
    }
    void RemoveWalls() {
        for (int f = 0; f < width; f++) {
            for (int g = 0; g < height; g++) {
                if (map[f, g] == mapTileType.MTT_NIL) {
                    if ((f > 0 && map[f - 1, g] > mapTileType.MTT_NIL) ||
                        (f < (width - 1) && map[f + 1, g] > mapTileType.MTT_NIL) ||
                        (g > 0 && map[f, g - 1] > mapTileType.MTT_NIL) ||
                        (g < (height - 1) && map[f, g + 1] > mapTileType.MTT_NIL)) {
                        map[f, g] = mapTileType.MTT_NIL;
                    }
                }
            }
        }
    }

}



class BSPDungeonNode {

    public int depth, x, y, x2, y2;
    BSPDungeonNode parent;
    BSPDungeon dungeon;
    BSPDungeonNode left, right;
    splitDirType splitdir;

    public BSPDungeonNode(BSPDungeon _dungeon, BSPDungeonNode _parent, int _x, int _y, int _x2, int _y2) {
        x = _x; y = _y; x2 = _x2; y2 = _y2; parent = _parent; dungeon = _dungeon;

        left = null; right = null;

        depth = (parent == null) ? 1 : parent.depth + 1;//if parent is null then this is the top level


        if (depth == dungeon.depth) {
            shrink();
        } else {
            split();
            dungeon.letsjoinlist[depth].Add(this);
        }
        if (parent == null) {
            //we are at the top level and the split has been done and recursed all the way down
            //join and complete!

            //if it failed somewhere, exit before joining
            if (dungeon.hasfailed) return;

            for (int f = dungeon.depth - 1; f > 0; f--) {
                foreach (var g in dungeon.letsjoinlist[f]) {
                    g.join();
                }
            }

        }

    }

    int width() {
        return (x2 - x) + 1;
    }

    int height() {
        return (y2 - y) + 1;
    }

    void split() {
        int amount = lil.randi(40, 60);
        int choice = lil.randi(1, 10);

        int pivotx = lil.percentof(amount, width());
        int pivoty = lil.percentof(amount, height());

        if (choice < 6) {
            if (!splithoriz(pivotx)) {
                if (!splitvert(pivoty)) {

                    dungeon.hasfailed = true;
                    //error_out(" can't split h or v (ch<6) ");


                }
            }
        } else {
            if (!splitvert(pivoty)) {
                if (!splithoriz(pivotx)) {

                    dungeon.hasfailed = true;
                    //error_out(" can't split h or v  ");


                }
            }
        }
    }

    bool splithoriz(int p) {
        if (p < 4 || (x2 - (x + p)) < 3 || (depth == dungeon.depth - 1 && y2 - y == (dungeon.height - 1))) {
            // cout << "splithoriz: p " << p << endl;
            return false;
        } else {
            splitdir = splitDirType.HORIZ;
            left = new BSPDungeonNode(dungeon, this, x, y, x + p - 1, y2);
            right = new BSPDungeonNode(dungeon, this, x + p, y, x2, y2);
            return true;
        }
    }

    bool splitvert(int p) {
        if (p < 4 || (y2 - (y + p)) < 3 || (depth == dungeon.depth - 1 && x2 - x == (dungeon.width - 1))) {
            //  cout << "splitvert: p " << p << endl;
            return false;
        } else {
            splitdir = splitDirType.VERT;
            left = new BSPDungeonNode(dungeon, this, x, y, x2, y + p - 1);
            right = new BSPDungeonNode(dungeon, this, x, y + p, x2, y2);
            return true;
        }
    }
    void error_out(string s) {
        Debug.Log("Error" + s);
        dump();
        Application.Quit();

    }

    void dump() {
        Debug.Log("depth " + depth + " width " + width() + " height " + height() + " x " + x + " y " + y + " x2 " + x2 + " y2 " + y2);
        if (left != null) left.dump();
        if (right != null) right.dump();

    }


    void shrink() {
        int sx = x, sy = y, sx2 = x2, sy2 = y2;
        x++; y++; x2--; y2--;
        bool shrink_w = lil.randi(1, 10) < 6 ? true : false;
        bool shrink_h = lil.randi(1, 10) < 6 ? true : false;
        if (width() < 3) shrink_w = false;
        if (height() < 3) shrink_h = false;
        int width60, height60, delta, leftside, rightside, topside, bottomside;
        int newwidth = 0, newheight = 0;
        if (shrink_w) {
            width60 = lil.percentof(0, width());
            if (width60 < 2) width60 = 2;
            if (width() - 1 == width60) {
                newwidth = width() - 1;
            } else {
                newwidth = lil.randi(width60, width() - 1);
            }
            if (!(newwidth == 0)) {
                delta = width() - newwidth;
                leftside = lil.randi(0, delta);
                rightside = delta - leftside;
                x += leftside;
                x2 -= rightside;
            }
        }
        if (shrink_h) {
            height60 = lil.percentof(0, height());
            if (height60 < 2) height60 = 2;
            if (height() - 1 == height60) {
                newheight = height() - 1;
            } else {
                newheight = lil.randi(height60, height() - 1);
            }
            if (!(newheight == 0)) {
                delta = height() - newheight;
                topside = lil.randi(0, delta);
                bottomside = delta - topside;
                y += topside;
                y2 -= bottomside;
            }
        }
        drawroom_in_map();
        x = sx; y = sy; x2 = sx2; y2 = sy2;
    }

    void drawroom_in_map() {
        for (int g = y; g < y2 + 1; g++) {
            for (int f = x; f < x2 + 1; f++) {
                dungeon.map[f, g] = mapTileType.MTT_ROOM;
            }
        }

        /*
        //top wall
        if(y>0)
            for(int z=x-1;z<x2+2;z++)
                if(z>0 && z<dungeon->width)dungeon->map[z][y-1]=mapTileType.MTT_WALL;
        //bottom wall
        if((y2+1)<dungeon->height)
            for(int z=x-1;z<x2+2;z++)
                if(z>0 && z<dungeon->width)dungeon->map[z][y2+1]=mapTileType.MTT_WALL;
          //left wall
         if(x>0)
            for(int z=y-1;z<y2+2;z++)
                if(z>0 && z<dungeon->height)dungeon->map[x-1][z]=mapTileType.MTT_WALL;
         //right wall
          if((x2+1)<dungeon->width)
            for(int z=y-1;z<y2+2;z++)
                if(z>0 && z<dungeon->height)dungeon->map[x2+1][z]=mapTileType.MTT_WALL;
                */
    }

    void join() {
        int genesisx, genesisy;
        List<Cell> path;
        if (splitdir == splitDirType.VERT) {
            genesisx = lil.randi(x, x2);
            path = shootray(left, genesisx, left.y2, direction.UP);
            if (path != null) {
                filldungeoncorridor(path);
            } else {
                bentjoin(left, genesisx, left.y2, direction.UP);
            }
            path = shootray(right, genesisx, right.y, direction.DOWN);
            if (path != null) {
                filldungeoncorridor(path);
            } else {
                bentjoin(right, genesisx, right.y, direction.DOWN);
            }
        } else { //horiz
            genesisy = lil.randi(y, y2);
            path = shootray(left, left.x2, genesisy, direction.LEFT);
            if (path != null) {
                filldungeoncorridor(path);
            } else {
                bentjoin(left, left.x2, genesisy, direction.LEFT); //bug was here. it said left->y2 should have been left->x2
            }
            path = shootray(right, right.x, genesisy, direction.RIGHT);
            if (path != null) {
                filldungeoncorridor(path);
            } else {
                bentjoin(right, right.x, genesisy, direction.RIGHT);
            }
        }
    }
    void bentjoin(BSPDungeonNode half, int xx, int yy, direction dir) {
        List<Cell> path;
        path = shootraystoside(half, xx, yy, dir);

        if (path.Count == 0) {
            dungeon.hasfailed = true;
            //error_out(" path is empty ");
        } else {
            filldungeoncorridor(path);
        }
    }

    void filldungeoncorridor(List<Cell> p) {
        foreach (var f in p) {
            dungeon.map[f.x, f.y] = mapTileType.MTT_CORRIDOR;
        }
    }

    List<Cell> shootray(BSPDungeonNode half, int startx, int starty, direction dir) {
        int dx = lil.deltax[(int)dir]; int dy = lil.deltay[(int)dir];

        int tx = startx, ty = starty;
        List<Cell> steps = new List<Cell>();
        while (true) {
            if ((dx == -1 && tx == half.x) || (dx == 1 && tx == half.x2) || (dy == -1 && ty == half.y) || (dy == 1 && ty == half.y2)) {
                return null;
            }
            steps.Add(new Cell(tx, ty)); //THESE ALL USED TO BE !=mapTileType.MTT_NIL
            if ((dx != 1 && tx - 1 >= half.x && dungeon.map[tx - 1, ty] >= mapTileType.MTT_CORRIDOR)
                    || (dx != -1 && tx + 1 <= half.x2 && dungeon.map[tx + 1, ty] >= mapTileType.MTT_CORRIDOR)
                    || (dy != 1 && ty - 1 >= half.y && dungeon.map[tx, ty - 1] >= mapTileType.MTT_CORRIDOR)
                    || (dy != -1 && ty + 1 <= half.y2 && dungeon.map[tx, ty + 1] >= mapTileType.MTT_CORRIDOR)) {
                return steps;
            }
            tx += dx; ty += dy;
        }
    }

    List<Cell> shootraystoside(BSPDungeonNode half, int startx, int starty, direction dir)
        // follows the same path as "shootray" but instead of checking one square ahead and to sides
        // it shoots secondary rays out to each side. Also rather than returning with the first hit, we take all
        // hits and return one of them at random. if there are  no paths the return array will be empty rather than
        // return false
    {
        int dx = lil.deltax[(int)dir]; int dy = lil.deltay[(int)dir];
        int tx = startx, ty = starty;
        List<Cell> result;
        List<List<Cell>> steps = new List<List<Cell>>();
        List<Cell> firstel = new List<Cell>();
        List<Cell> temp;
        while (true) { //loop for ever!
            if (dx != 0) { //we are going left or right so shoot rays up and down
                //FIXME instead of copying what firstel is, currently each time and adding it, plus result to steps, we could just add result to steps
                //and when we return a random result from steps we could um somehow have saved how many elements were in firstel and add that many on
                //before returning it
                result = shootray(half, tx, ty, direction.UP); if (result != null) { temp = new List<Cell>(); temp.AddRange(firstel); temp.AddRange(result); steps.Add(temp); }
                result = shootray(half, tx, ty, direction.DOWN); if (result != null) { temp = new List<Cell>(); temp.AddRange(firstel); temp.AddRange(result); steps.Add(temp); }
            } else { //we are going up or down so shoot rays left and right
                result = shootray(half, tx, ty, direction.LEFT); if (result != null) { temp = new List<Cell>(); temp.AddRange(firstel); temp.AddRange(result); steps.Add(temp); }
                result = shootray(half, tx, ty, direction.RIGHT); if (result != null) { temp = new List<Cell>(); temp.AddRange(firstel); temp.AddRange(result); steps.Add(temp); }
            }
            //if we got to the edge then return a random path from steps 
            if ((dx == -1 && tx == half.x) || (dx == 1 && tx == half.x2) || (dy == -1 && ty == half.y) || (dy == 1 && ty == half.y2)) {
                return steps.randmember();
            }
            //add  current square to firstel, which represents the first line of the L-shaped corridor 
            firstel.Add(new Cell(tx, ty));
            //move one square further in the direction we are going
            tx += dx; ty += dy;
        }
    }
}