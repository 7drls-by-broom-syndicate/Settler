using UnityEngine;
using System.Collections;
using System;

public partial class RLMap
{
    static int[,] multipliers = new int[4, 8]{
        { 1, 0, 0, -1, -1, 0, 0, 1 },
        { 0, 1, -1, 0, 0, -1, 1, 0 },
        { 0, 1, 1, 0, 0, -1, -1, 0 },
        { 1, 0, 0, 1, -1, 0, 0, -1 }
    };
    static Color walllight = new Color(1.0f, 0.30859375f, 0f);
    static Color gatelight = lil.rgb_unitycolour(184, 133, 217);
    static Color lightgrey = new Color(0.7f, 0.7f, 0.7f);
    //    static Color walllight = new Color(1.0f/8f, 0.30859375f/8f, 0f);
    public static Color memorylight = new Color(0f, 0f, 0.3f);
    //static Color walllight = new Color(1.0f, 0.30859375f, 0.5f);

    //FOV STUFF
    //from http://www.roguebasin.com/index.php?title=C%2B%2B_shadowcasting_implementation
    // "A C++ implementation of Bjorn Bergstrom's recursive shadowcasting FOV algorithm."

    void cast_light(int x, int y, int radius, int row,
        float start_slope, float end_slope, int xx, int xy, int yx,
        int yy, Action<int, int> WhatToDo)
    {
        if (start_slope < end_slope)
        {
            return;
        }
        float next_start_slope = start_slope;
        for (int i = row; i <= radius; i++)
        {
            bool blocked = false;
            for (int dx = 0 - i, dy = 0 - i; dx <= 0; dx++)
            {
                float l_slope = (dx - 0.5f) / (dy + 0.5f);
                float r_slope = (dx + 0.5f) / (dy - 0.5f);
                if (start_slope < r_slope)
                {
                    continue;
                }
                else if (end_slope > l_slope)
                {
                    break;
                }

                int sax = dx * xx + dy * xy;
                int say = dx * yx + dy * yy;
                if ((sax < 0 && (int)Math.Abs(sax) > x) ||
                    (say < 0 && (uint)Math.Abs(say) > y))
                {
                    continue;
                }
                int ax = x + sax;
                int ay = y + say;
                if (ax >= width || ay >= height)
                {
                    continue;
                }

                int radius2 = radius * radius;
                if ((uint)(dx * dx + dy * dy) < radius2)
                {
                    if (!FOV_set_this_run.AtGet(ax, ay))
                    {
                        FOV_set_this_run.AtSet(ax, ay, true);
                        WhatToDo(ax, ay);
                    }
                }

                if (blocked)
                {
                    if (blocks_sight.AtGet(ax, ay))
                    {
                        next_start_slope = r_slope;
                        continue;
                    }
                    else {
                        blocked = false;
                        start_slope = next_start_slope;
                    }
                }
                else if (blocks_sight.AtGet(ax, ay))
                {
                    blocked = true;
                    next_start_slope = r_slope;
                    cast_light(x, y, radius, i + 1, start_slope, l_slope, xx,
                        xy, yx, yy, WhatToDo);
                }
            }
            if (blocked)
            {
                break;
            }
        }
    }

    public void do_fov_rec_shadowcast(int x, int y, int radius)
    {
        FOV_set_this_run.Fill(false);
        in_FOV.Fill(false);

        Action<int, int> ff = (int xx, int yy) =>
        {
            in_FOV.AtSet(xx, yy, true);
            fogofwar.AtSet(xx, yy, false);
            Etilesprite et = displaychar.AtGet(xx, yy);
            //new debug
            if ((int)et < 0 || (int)et > 255) Debug.Log("ET OUT OF BOUNDS =" + (int)et);
            //end new debug
            playermemory.AtSet(xx, yy, et);

            if (itemgrid[xx, yy] != null && itemgrid[xx,yy].ismob==false) minimap.SetPixel(xx, yy, (Color)minimapcolours[(int)itemgrid[xx, yy].tile]);
            else minimap.SetPixel(xx, yy, (Color)minimapcolours[(int)et]);
            //if (et == Etilesprite.WALL) minimap.SetPixel(xx, yy, Color.grey);
            //else if (et == Etilesprite.FLOOR) minimap.SetPixel(xx, yy, Color.black);

        };

        for (sbyte i = 0; i < 8; i++)
        {
            cast_light(x, y, radius, 1, 1.0f, 0.0f, multipliers[0, i],
                multipliers[1, i], multipliers[2, i], multipliers[3, i],
                ff
            );
        }
        ff(x, y);

        minimap.SetPixel(this.player.posx, this.player.posy, Color.cyan);
        minimap.Apply(false);
    }

    void do_fov_foralight(int callx, int cally, int radius, Color clr, sbyte directional = -1)
    {
        //if (directional != 1) return;
        FOV_set_this_run.Fill(false);

        //auto ff = [this, callx, cally, radius, clr,directional](uint xx, uint yy){
        Action<int, int> ff = (int xx, int yy) =>
        {
            if (directional == -1) goto lightit;
            if (callx == xx && cally == yy) goto lightit;
            switch (directional)
            {
                case 0://n
                    if (yy >= cally) return;
                    break;
                case 1://s
                    if (yy <= cally) return;
                    break;
                case 2://e
                    if (xx <= callx) return;
                    break;
                case 3://w
                    if (xx >= callx) return;
                    break;
            }
            lightit:
            dolight(callx, cally, xx, yy, radius, clr, true);//staticlight.AtGet(xx, yy)); new 18 jan



        };




        for (uint i = 0; i < 8; i++)
        {
            cast_light(callx, cally, radius, 1, 1.0f, 0.0f, multipliers[0, i],
                multipliers[1, i], multipliers[2, i], multipliers[3, i],
                ff//crash
                );
        }
        ff(callx, cally);
    }

    // void dolight(int x1, int y1, int x2, int y2,
    //		int r, Color colour, Color store){

    void dolight(int x1, int y1, int x2, int y2,//new 18 jan
       int r, Color colour, bool dostatic)
    {                    //new 18 jan

        //nice lava with 3 + ds /5 and radius 9
        float factor = (r == 3) ? 1.5f : 70f;
        float c1 = 1.0f / (1.0f + (float)(Distance_Squared(x1, y1, x2, y2)) / factor);//was 20. bigger = more light
        float c2 = c1 - 1.0f / (1.0f + (float)(r * r));
        float c3 = c2 / (1.0f - 1.0f / (1.0f + (float)(r * r)));


        if (dostatic)
        {
            Color store = this.staticlight[x2, y2];
            store.r += (colour.r * c3);
            store.g += (colour.g * c3);
            store.b += (colour.b * c3);
            this.staticlight[x2, y2] = store;
        }
        else {
            Color store = this.dynamiclight[x2, y2];
            store.r += (colour.r * c3);
            store.g += (colour.g * c3);
            store.b += (colour.b * c3);
            this.dynamiclight[x2, y2] = store;
        }


        //store.total = store.r + store.g + store.b;//i changed this from r+g+g to r+g+b because seemed like mistake

    }

    public void do_fov_foradynamiclight(int callx, int cally, int radius, Color clr)
    {
        FOV_set_this_run.Fill(false);

        //

        Action<int, int> ff = (int xx, int yy) =>
        {

            dolight(callx, cally, xx, yy, radius, clr, false);//dynamiclight.AtGet(xx, yy)); new 18 jan

        };

        for (int i = 0; i < 8; i++)
        {
            cast_light(callx, cally, radius, 1, 1.0f, 0.0f, multipliers[0, i],
                multipliers[1, i], multipliers[2, i], multipliers[3, i],
                ff
            );
        }
        ff(callx, cally);
    }

    //recalc lighting
    public void dostaticlights()
    {
        staticlight.Fill(Color.black);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                switch (displaychar.AtGet(x, y))
                {

                    case Etilesprite.ITEM_WARP_GATE_ANIM_1:
                        do_fov_foralight(x, y, 3, gatelight);
                        break;

                        //	case 'L':
                        //		do_fov_foralight(x, y, 5, new Color( 255, 0, 0 ));
                        //		break;
                }
                item_instance i = itemgrid[x, y];
                if (i != null)
                {
                    switch (i.tile)
                    {
                        case Etilesprite.ITEM_LANTERN_ON_A_STICK_FOR_NO_REASON:
                            do_fov_foralight(x, y, 3, walllight);
                            break;

                        case Etilesprite.ITEM_CAIRN_RED:
                            do_fov_foralight(x, y, 3, Color.red);
                            break;
                        case Etilesprite.ITEM_CAIRN_GREEN:
                            do_fov_foralight(x, y, 3, Color.green);
                            break;
                        case Etilesprite.ITEM_CAIRN_BLUE:
                            do_fov_foralight(x, y, 3, Color.blue);
                            break;
                        case Etilesprite.ITEM_CAIRN_PURPLE:
                            do_fov_foralight(x, y, 3, Color.magenta);
                            break;
                    }
                }
            }
        }
    }



}
