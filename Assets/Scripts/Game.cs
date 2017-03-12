//#define FLICKERING_LANTERN

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

class FloatingTextItem
{
    public byte[] message;
    public Color colour;
    public float fade = 1.0f;

    public int rise = 0;
    public int sqx, sqy;
    public int mwoffset;

    public FloatingTextItem(string m, int _sqx, int _sqy, Color c)
    {
        message = System.Text.Encoding.ASCII.GetBytes(m);
        colour = c;
        sqx = _sqx;
        sqy = _sqy;
        mwoffset = (m.Length * 6) / 2;
    }

}

class Pangotimer
{
    public float period = 0;
    public float timer = 0;
    public Pangotimer(float p)
    {
        period = p;
    }
    public bool test()
    {
        return (Time.time > timer);
    }
    public void reset()
    {
        timer = Time.time + period;
    }
}

public enum Egamestate : int { initializing, titlescreen, playing, gameover,youwon }

public enum CradleOfTime { dormant, ready_to_process_turn, waiting_for_player, player_is_done, processing_other_actors }

public partial class Game : MonoBehaviour
{

    public bool showresource = true;
    public bool showyield = false;
    //experimental snow
    //const int number_snow_particles = 200;
    //float[] snowx = new float[number_snow_particles];
    //float[] snowy = new float[number_snow_particles];
    //
    byte[] bstrGameOver = System.Text.Encoding.ASCII.GetBytes("Game Over");
    //  byte[] statusline = System.Text.Encoding.ASCII.GetBytes("HP:     SPEED:     LEVEL:     SCORE:");

    byte[] statusline;
    int statusgold;
    int statushp;
    int statuslevel;
    int statusscore=45;

    byte[] winstring = System.Text.Encoding.ASCII.GetBytes("A winner is you!Score is:");
    Color whiteblend = new Color(1f, 1f, 1f, 0.8f);
    Color whiteblendvariable = new Color(0.9f, 0.1f, 0.1f, 0f);
    Color highlight = new Color(0.7f, 0.4f, 0.2f, 0.05f);
    byte[] bstrPressStart;//=System.Text.Encoding.ASCII.GetBytes("why won't you work") ;


    int pressstartx = 0;
    int pressstarty = 350 - 6;//180

    //AudioSource[] soundfx;

    public Egamestate gamestate = Egamestate.initializing;
    CradleOfTime TimeEngine = CradleOfTime.dormant;

    public Texture wednesdayfont;
    public Texture sprites;
    public Texture titlescreen;
    public Texture2D particle;
    AudioSource MyAudioSource;
    public AudioClip backmusic, titlemusic;


    float[] StoredNoise = new float[640];
    float[] StoredNoise2 = new float[640];
    List<FloatingTextItem> FloatingTextItems = new List<FloatingTextItem>();

    //const int zoomfactor = 2;           //1,2 or 3 for 640x360,1280x720 or 1920x1080

    public static int zoomfactor = 2;//was const int

    public static int zoomfactorx16 = zoomfactor * 16;//was const int
    const bool fullscreenp = true;

    MessageLog log;             //message log

    //vars for drawing sprites 
    const float xratio = 1f / 32f;
    const float yratio = 1f / 8f;
    float spriteratio, spriteratio3;
    static Rect rbigtext = new Rect(0, 0, (12 * 6) * zoomfactor, (20 * 12) * zoomfactor);

    static Rect r = new Rect(0, 0, 6 * zoomfactor, 12 * zoomfactor);
    static Rect r2 = new Rect(0, 0, xratio, yratio);
    static Rect r3 = new Rect(0, 0, 16 * zoomfactor, 16 * zoomfactor);
    static Rect r3b = new Rect(0, 0, 8 * zoomfactor, 8 * zoomfactor);
    static Rect r3c = new Rect(0, 0, 3 * zoomfactor, 3 * zoomfactor);
    static Rect r4 = new Rect(0, 0, 0, 1f);
    static Rect r4reverse = new Rect(0, 0, 0, 1f);
    static Rect r5 = new Rect(0, 0, 0, 1f);
    static Rect Rectparticle = new Rect(0, 0, zoomfactor, zoomfactor);
    static Rect Rectparticle2 = new Rect(0, 0, zoomfactor, zoomfactor);

    static Rect r_minimap; //= new Rect(339 * zoomfactor, 0, 80 * 2 * zoomfactor, 50 * 2 * zoomfactor);
    static Rect r2_minimap = new Rect(0, 0, 1f, -1f);
    //

    //viewport dimensions
    static int VIEWPORT_WIDTH = 21;
    static int VIEWPORT_HEIGHT = 21;
    static int XHALF = 10;
    static int YHALF = 10;
    //

    public static int originx = 0, originy = 0;
    RLMap map;
    Player player;

    Pangotimer floating_text_timer = new Pangotimer(0.008f);
    //Pangotimer snow_timer = new Pangotimer(0.005f);

    static Rect wholescreen = new Rect(0, 0, 640 * zoomfactor, 360 * zoomfactor);

#if FLICKERING_LANTERN
    float LF_period=0.1f; //frequency of lantern flicker
    float LF_timer = 0.0f; //progression towards period
    float LF_amount=0.0f; //deviation from full light
#endif


    public static Menu currentmenu=null;
    public static bool menuup = false;
    public static bool menucommandwaiting = false;

    void OnGUI()
    {
        Action<int, int, byte> PrintOneChar = (int xx, int yy, byte c) =>
        {
           // for (int x = 0; x < b.Length; x++)
           // {
                int y = 0;
               // byte c = b[x];
                int xpos = c % 32;
                int ypos = 7 - (c / 32);
                r.x = (xx + (0 * 6)) * zoomfactor;
                r.y = yy * zoomfactor;

                r2.x = xratio * xpos;
                r2.y = yratio * ypos;

                GUI.DrawTextureWithTexCoords(r, wednesdayfont, r2);

          //  }
        };
        Action<int, int, byte[]> PrintString = (int xx, int yy, byte[] b) =>
        {
            for (int x = 0; x < b.Length; x++)
            {
                int y = 0;
                byte c = b[x];
                int xpos = c % 32;
                int ypos = 7 - (c / 32);
                r.x = (xx + (x * 6)) * zoomfactor;
                r.y = yy * zoomfactor;

                r2.x = xratio * xpos;
                r2.y = yratio * ypos;

                GUI.DrawTextureWithTexCoords(r, wednesdayfont, r2);

            }
        };

        Action<int, int, int> PrintNumber = (int xx, int yy, int value) =>
        {
         //   Action<int, int, int> DrawSprite_pixelaccurate = (int x, int y, int s) => {
         //       r3.x = x * zoomfactor;
         //       r3.y = y * zoomfactor;
         //       r4.x = spriteratio * s;
         //       GUI.DrawTextureWithTexCoords(r3, sprites, r4);
         //   };

            for (int x = 0; x < 3; x++)
            {//48 is 0
                int y = 0;
                byte c = 0; 
                switch (x)
                {
                    case 0:c = (value<0)?(byte)45:(byte)(48+(value / 100)); break;
                    case 1:c= (value < 0) ? (byte)45 : (byte)(48+((value%100)/10)); break;
                    case 2:c = (value < 0) ? (byte)45 : (byte)(48 + (value % 10)); break;
                }
                int xpos = c % 32;
                int ypos = 7 - (c / 32);
                r.x = (xx + (x * 6)) * zoomfactor;
                r.y = yy * zoomfactor;

                r2.x = xratio * xpos;
                r2.y = yratio * ypos;

                GUI.DrawTextureWithTexCoords(r, wednesdayfont, r2);

              //  DrawSprite_pixelaccurate(x, y, 3 + (value / 100));
               // DrawSprite_pixelaccurate(x + 16, y, 3 + ((value % 100) / 10));
               // DrawSprite_pixelaccurate(x + 32, y, 3 + (value % 10));
            }
        };


        GUI.color = Color.white;
        switch (gamestate)
        {
            case Egamestate.youwon:
                GUI.color = Color.yellow;
                PrintString(50, 50, winstring);
                PrintNumber(50 + (6 * 27), 50, player.score);

                break;
            case Egamestate.titlescreen:
                GUI.DrawTexture(wholescreen, titlescreen);

                GUI.color = whiteblendvariable;
                whiteblendvariable.a = 0.5f + ((((Time.time / 2) - (int)(Time.time / 2))) / 2);
                //Debug.Log(Input.GetButtonDown("start").ToString());
                for (int x = 0; x < bstrPressStart.Length; x++)
                {
                    int y = 0;
                    byte c = bstrPressStart[x];
                    int xpos = c % 32;
                    int ypos = 7 - (c / 32);
                    r.x = (pressstartx + (x * 6)) * zoomfactor;
                    r.y = pressstarty * zoomfactor;

                    r2.x = xratio * xpos;
                    r2.y = yratio * ypos;

                    GUI.DrawTextureWithTexCoords(r, wednesdayfont, r2);

                }

                break;
            case Egamestate.playing:
            case Egamestate.gameover:





                int curnoi = 0;

                for (int f = 0; f < 640 * zoomfactor; f += zoomfactor)
                {
                    float ff = (float)(((float)f / (640.0f * (float)zoomfactor)) * 2.0f) - 1.0f;
                    float gg = Time.time;
                    //Rectparticle.x = f; 
                    StoredNoise[curnoi] = SimplexNoise.Noise.Generate(ff * 128, gg);
                    StoredNoise2[curnoi] = (StoredNoise[curnoi] / 2f) + 1f;
                    //Rectparticle.y = (int)(180+ ((4) * StoredNoise[curnoi]))*zoomfactor;
                    curnoi++;
                    //GUI.DrawTextureWithTexCoords(Rectparticle, particle, Rectparticle2);
                }

                Action<int, int, int> DrawSprite = (int x, int y, int s) =>
                {
                    r3.x = x * zoomfactorx16;
                    r3.y = y * zoomfactorx16;
                    r4.x = spriteratio * (s - 1);//s-1 is new for 2016
                    GUI.DrawTextureWithTexCoords(r3, sprites, r4);
                };
                Action<int, int, int> DrawSpriteReverse = (int x, int y, int s) =>
                {
                    r3.x = x * zoomfactorx16;
                    r3.y = y * zoomfactorx16;
                    r4reverse.x = spriteratio * (s);//s-1 is new for 2016
                    GUI.DrawTextureWithTexCoords(r3, sprites, r4reverse);
                };
                Action<int, int, int> DrawSprite_Particle = (int x, int y, int s) =>
                {
                    r3b.x = x;
                    r3b.y = y;

                    r5.x = spriteratio * s;
                    r5.y = 0.5f;
                    r5.width = spriteratio / 2;
                    r5.height = 0.5f;
                    GUI.DrawTextureWithTexCoords(r3b, sprites, r5);
                };
                Action<int, int> DrawSprite3x3 = (int x, int y) => {
                    r3c.x = x * zoomfactor;
                    r3c.y = y * zoomfactor;

                  //  r5.x = spriteratio * ((int)Etilesprite.EFFECT_FULLSQUARE-1);           //s
                    r5.y = (1f / 16f) * 10f;
                    r5.width = spriteratio3;
                    r5.height = (1f / 16f) * 3f;//0.5f;
                    GUI.DrawTextureWithTexCoords(r3c, sprites, r5);
                };
            
                originx = (player.posx < XHALF) ? 0 : player.posx - XHALF;
                originy = (player.posy < YHALF) ? 0 : player.posy - YHALF;

                if (player.posx > (map.width - (XHALF + 1))) originx = map.width - VIEWPORT_WIDTH;
                if (player.posy > (map.height - (YHALF + 1))) originy = map.height - VIEWPORT_HEIGHT;

                //draw message log
                int currentline = log.GetCurrentLine();
                for (int y = 0; y < 15; y++)
                {
                    for (int x = 0; x < 50; x++)
                    {
                        byte c = log.screenmap[x, currentline];
                        int xpos = c % 32;
                        int ypos = 7 - (c / 32);
                        r.x = (339 + (x * 6)) * zoomfactor;
                        r.y = (180 + (y * 12)) * zoomfactor;
                        r2.x = xratio * xpos;
                        r2.y = yratio * ypos;
                        GUI.color = log.screenfg[x, currentline];
                        GUI.DrawTextureWithTexCoords(r, wednesdayfont, r2);
                    }
                    currentline++; if (currentline == 15) currentline = 0;
                }
                //draw minimap

                GUI.color = Color.white;
                GUI.DrawTextureWithTexCoords(r_minimap, map.minimap, r2_minimap);
                //draw viewport of map
                //draw mob if there is one. item. noticed player icon if it has
                //draw locked padlock if appropriate
                //consider player stealth. 
                //draw user interface: lives, score , statuses, held items etc.

                int screenx = 0; int screeny = 0;
                for (int yy = originy; yy < originy + VIEWPORT_HEIGHT; yy++)
                {
                    for (int xx = originx; xx < originx + VIEWPORT_WIDTH; xx++)
                    {

                        // if (map.in_FOV.AtGet(xx, yy))
                        if(!map.fogofwar.AtGet(xx, yy))
                        {

#if FLICKERING_LANTERN
                    if (Time.time > LF_timer) {
                        if (lil.randi(1, 100) > 50) {
                            LF_amount -= 0.1f; if (LF_amount < 0.0f) LF_amount = 0.0f;
                        } else {
                            LF_amount += 0.1f; if (LF_amount > 0.5f) LF_amount = 0.5f;
                        }
                        LF_timer = Time.time + LF_period;
                    }
                    Color c = map.dynamiclight[xx, yy];
                    c.r -= LF_amount; if (c.r < 0) c.r = 0;
                    c.g -= LF_amount; if (c.g < 0) c.g = 0;
                    c.b -= LF_amount; if (c.b < 0) c.b = 0;
                    GUI.color = lil.colouradd(c, map.staticlight.AtGet(xx, yy));
#else
                            //  GUI.color = lil.colouradd(map.staticlight[xx, yy], map.dynamiclight[xx, yy]);
                            GUI.color = Color.white;
#endif

                            //if (map.displaychar[xx, yy] == Etilesprite.ITEM_WARP_GATE_ANIM_1)
                            //{
                            //    int i = (int)(Time.time * 3.0f) % 3;
                            //    DrawSprite(screenx, screeny, (int)Etilesprite.ITEM_WARP_GATE_ANIM_1 + i);

                            //    if (player.mob.hasbeads)
                            //    {
                            //        int ii = (int)(Time.time * 3.0f) % 2;
                            //        DrawSprite(screenx, screeny, (int)Etilesprite.ITEM_WARP_GATE_RAYS_1 + ii);
                            //    }
                            //}
                            //else {

                            //draw the base tile, biome or sea

                            DrawSprite(screenx, screeny, (int)map.displaychar.AtGet(xx, yy));

                            //draw hill,mountain,trees

                            if (map.mountain[xx, yy]) DrawSprite(screenx, screeny, (int)Etilesprite.MOUNTAIN_OVERLAY);
                            else if(map.hill[xx, yy]) DrawSprite(screenx, screeny, (int)Etilesprite.HILL_OVERLAY);

                            if(map.tree[xx, yy]) DrawSprite(screenx, screeny,(int)map.treechar[xx,yy]);

                            //draw yields
                            if (showyield)
                            {
                                if (map.yield[xx, yy].production > 0)
                                    DrawSprite(screenx, screeny, -1 + map.yield[xx, yy].production + (int)Etilesprite.YIELD_PRODUCTION_1);
                                if (map.yield[xx, yy].gold > 0)
                                    DrawSprite(screenx, screeny, -1 + map.yield[xx, yy].gold + (int)Etilesprite.YIELD_GOLD_1);
                                if (map.yield[xx, yy].food > 0)
                                    DrawSprite(screenx,screeny, -1 + map.yield[xx, yy].food + (int)Etilesprite.YIELD_FOOD_1);
                            }

                            //draw resources

                            if (showresource && map.resource[xx, yy] != null)
                                DrawSprite(screenx, screeny, (int)map.resource[xx, yy].tile);


                            //draw building
                            if (map.buildings[xx, yy] != Etilesprite.EMPTY)
                            {
                                DrawSprite(screenx, screeny, (int)map.buildings[xx, yy]);
                                if (map.citystates[xx,yy]!=null && map.citystates[xx, yy].metyet == false)
                                {
                                    map.citystates[xx, yy].metyet = true;
                                    log.Printline("Greetings from the City State of " + map.citystates[xx, yy].name+".",Color.yellow);
                                    log.Printline("If you desire to trade we need " + Cresource.resourcetypes[map.citystates[xx,yy].desiredresource].name+".",Color.yellow);

                                }
                            }

                            //draw influence. is this the right order in the layer chain? :D
                            if (map.influence[xx, yy] == true)
                            {
                                GUI.color = colour_influenceplayer;
                                DrawSprite(screenx, screeny, (int)Etilesprite.EFFECT_FULLSQUARE);
                            }
                            else
                            {
                                if (map.influence[xx, yy] == false)
                                {
                                    GUI.color = colour_influenceenemy;
                                    DrawSprite(screenx, screeny, (int)Etilesprite.EFFECT_FULLSQUARE);
                                }

                            }

                            //}
                            //blood layer
                            //if (map.bloodgrid[xx, yy] != null)
                            //{
                            //    DrawSprite(screenx, screeny, (int)Etilesprite.EFFECT_BLOOD_1+(int) map.bloodgrid[xx, yy]);
                            //}
                            //item layer draw item draw mob

                            GUI.color = Color.white;//new 

                            if (map.itemgrid[xx, yy] != null)
                            {
                                if (map.itemgrid[xx, yy].ismob)
                                {
                                    if (map.itemgrid[xx, yy].mob.noticedyou == false)
                                    {
                                        map.itemgrid[xx, yy].mob.noticedyou = true;
                                        if(map.itemgrid[xx,yy].mob.hostile_toplayer_currently)log.Printline(map.itemgrid[xx, yy].mob.archetype.name + " noticed you!");
                                    }

                                    if (!map.itemgrid[xx, yy].mob.magepointing)
                                    {
                                        if (map.itemgrid[xx, yy].mob.reversesprite)
                                            DrawSpriteReverse(screenx, screeny, (int)map.itemgrid[xx, yy].tile);
                                        else
                                            DrawSprite(screenx, screeny, (int)map.itemgrid[xx, yy].tile);
                                    } else
                                    {
                                        if (map.itemgrid[xx, yy].mob.reversesprite)
                                            DrawSpriteReverse(screenx, screeny, 1+(int)map.itemgrid[xx, yy].tile);
                                        else
                                            DrawSprite(screenx, screeny, 1+(int)map.itemgrid[xx, yy].tile);

                                        if (Time.time > map.itemgrid[xx, yy].mob.magepointing_timer)
                                            map.itemgrid[xx, yy].mob.magepointing = false;

                                        
                                    }


                               //    if(!map.itemgrid[xx,yy].mob.dead_currently) DrawSprite(screenx, screeny, (int)Etilesprite.EFFECT_DIRECTION_INDICATOR_1 + map.itemgrid[xx, yy].mob.facing);
                                }
                                else DrawSprite(screenx, screeny, (int)map.itemgrid[xx, yy].tile);
                            }

                            //player
                            if (player.posx == xx && player.posy == yy)
                            {
                                GUI.color = Color.white;
                                if (player.mob.reversesprite)
                                    DrawSpriteReverse(screenx, screeny, (int)player.mob.tile);
                                else DrawSprite(screenx, screeny, (int)player.mob.tile);

                                //draw facing
                               // DrawSprite(screenx, screeny, (int)Etilesprite.EFFECT_DIRECTION_INDICATOR_1 + player.mob.facing);
                            }

                            //smoke/cloud/gas
                            GUI.color = new Color(GUI.color.r, 255, GUI.color.b, 0.6f);
                            if (map.fogoffog[xx, yy])
                            {
                                for (int f = 0; f < 8; f++)
                                {
                                    float tx = (StoredNoise[(screenx * 16) + f] + 1.0f) * 4.5f;
                                    float ty = (StoredNoise[(screeny * 16) + f + 8] + 1.0f) * 4.5f;

                                    DrawSprite_Particle((screenx * 16 * zoomfactor) + zoomfactor * (int)tx, (screeny * 16 * zoomfactor) + zoomfactor * (int)ty, 13);
                                }
                            }
                            //if (map.onfire[xx, yy]!=null)
                            //{
                            //    int i = (int)(Time.time * 3.0f) % 3;
                            //    DrawSprite(screenx, screeny, (int)Etilesprite.EFFECT_FIRE_ANIM_1 + i);
                                    

                            //}



                        }
                        //else {
                        //    if (!map.fogofwar.AtGet(xx, yy))
                        //    {
                        //        GUI.color = RLMap.memorylight;
                        //        DrawSprite(screenx, screeny, (int)map.playermemory.AtGet(xx, yy));
                        //    }
                        //}

                        //draw the brief colour overlay on squares that is used for snake spit and 
                        //wizard bomb (if we get round to bomb that is)
                        if (map.gridflashtime[xx, yy] > Time.time)
                        {
                            GUI.color = map.gridflashcolour[xx, yy];
                            DrawSprite(screenx, screeny,(int) Etilesprite.EFFECT_FULLSQUARE);
                        }






                        screenx++;
                    }
                    screeny++; screenx = 0;
                }//end of screen loop

              


                //floating text
                for (int f = 0; f < FloatingTextItems.Count; f++)
                {
                    FloatingTextItem fti = FloatingTextItems[f];
                    int myx = (((fti.sqx - originx) * 16) - fti.mwoffset + 8) * zoomfactor;
                    int myy = (((fti.sqy - originy) * 16) - fti.rise + 5) * zoomfactor;

                    fti.colour.a = fti.fade;
                    GUI.color = fti.colour;

                    for (int x = 0; x < fti.message.Length; x++)
                    {
                        byte c = fti.message[x];
                        int xpos = c % 32;
                        int ypos = 7 - (c / 32);
                        r.x = myx + (6 * x * zoomfactor);
                        r.y = myy;
                        r2.x = xratio * xpos;
                        r2.y = yratio * ypos;

                        GUI.DrawTextureWithTexCoords(r, wednesdayfont, r2);
                    }


                }

                //statusline

            

                if (statusgold != player.gold || statushp != player.hp || statuslevel!=player.dunlevel|| statusscore!=player.score) {
                    statusline = System.Text.Encoding.ASCII.GetBytes(("Gold: " + player.gold + " Hp: " + player.hp+" Level:"+player.dunlevel+" Score:"+player.score));
                    statusgold = player.gold;
                    statushp = player.hp;
                    statusscore = player.score;
                    statuslevel = player.dunlevel;
                        }

                GUI.color = Color.white;
                PrintString(0, 359 - 12, statusline);

                //PrintNumber(3*6, 359 - 12, player.hp);
                //PrintNumber(14 * 6, 359 - 12, player.mob.speed);
                //PrintNumber(25 * 6, 359 - 12, player.dunlevel);
                //PrintNumber(37 * 6, 359 - 12, player.score);


                //DO YOU GOT THE BEEDZ, PANGO? THE BEEEEEDS! THE BEEEEEEEDS!
                //if (player.mob.hasbeads) { DrawSprite(15, 21, (int)Etilesprite.ITEM_WARP_BEADS); }
                //draw other buffs if applicz
                //if(player.mob.hasattackup) {
                //    DrawSprite(16, 21, (int)Etilesprite.ICON_ATTACK_UP);
                //    GUI.color = Color.yellow;
                //    PrintNumber(16 * 16, 14+(21 * 16), player.mob.attackuptimer);
                //}
                //if (player.mob.hasdefenseup) {
                //    DrawSprite(17, 21, (int)Etilesprite.ICON_DEFENSE_UP);
                //    GUI.color = Color.yellow;
                //    PrintNumber(17 * 16, 14+(21 * 16), player.mob.defenseuptimer);
                //}
                //later: def down and attack down too
               // DrawSprite(20, 21, (int)Etilesprite.INVENTORY_HAND);
                //if (player.held != Etilesprite.EMPTY) DrawSprite(19, 21, (int)player.held);
                //experimental snow
                //GUI.color = new Color(255, 255, 255, 0.5f);
                //for (int f = 0; f < number_snow_particles; f++)
                //{
                    //       float tx = (StoredNoise[(screenx * 16) + f] + 1.0f) * 4.5f;
                    //       float ty = (StoredNoise[(screeny * 16) + f + 8] + 1.0f) * 4.5f;
                    //
                   // int sxf = (int)snowx[f];
                   // int syf = (int)snowy[f];
                   // if (sxf < 0) sxf = 0;if (syf < 0) syf = 0;
                   // if (sxf > 639) sxf = 639;if (syf > 479) syf = 479;
                   // snowx[f] += StoredNoise[sxf];
                   // snowy[f] += (StoredNoise[syf]+1)*2f;
                   // if (snowy[f] > 479) { snowy[f] = 0;snowx[f]= lil.randf(0, 21*16); }
                  //  DrawSprite3x3((int)snowx[f], (int)snowy[f]);

                        //_Particle((screenx * 16 * zoomfactor) + zoomfactor * (int)tx, (screeny * 16 * zoomfactor) + zoomfactor * (int)ty, 13);
                    //}               
                //end snow 





                //tooltipz. the last thing to do!
                //31,214 (*zoomfactor)

                int fx = (int)Input.mousePosition.x; int fy = (int)((360 * zoomfactor) - 1 - Input.mousePosition.y);

                int mausx = ((fx / zoomfactor) - 0) / 16;//((fx) / zoomfactorx16);//-31;
                int mausy = ((fy / zoomfactor) - 0) / 16;//((fy) / zoomfactorx16);//-214;

                int mapx = mausx + originx;
                int mapy = mausy + originy;

                if (mausx >= 0 && mausy >= 0 && mausx < VIEWPORT_WIDTH && mausy < VIEWPORT_HEIGHT && !map.fogofwar[mapx, mapy]) //&& map.in_FOV[mapx,mapy]
                {//general idea is setting s to be the string to display for tooltip

                    //if (!showyield)
                    //{
                    //    if (map.yield[mapx, mapy].production > 0)
                    //        DrawSprite(mausx, mausy, -1 + map.yield[mapx, mapy].production + (int)Etilesprite.YIELD_PRODUCTION_1);
                    //    if (map.yield[mapx, mapy].gold > 0)
                    //        DrawSprite(mausx, mausy, -1 + map.yield[mapx, mapy].gold + (int)Etilesprite.YIELD_GOLD_1);
                    //    if (map.yield[mapx, mapy].food > 0)
                    //        DrawSprite(mausx, mausy, -1 + map.yield[mapx, mapy].food + (int)Etilesprite.YIELD_FOOD_1);
                    //}

                    //string s = "";

                    string[] s = { "", "", "", "", "", "" };

                    //line 1 Tundra, Hills, Forest [P:2 G:0 F:1]
                    s[0] = Tilestuff.tilestring[(int)map.displaychar[mapx, mapy] + 2];
                    if (map.mountain[mapx, mapy]) s[0] += ",Mountains";
                    else if (map.hill[mapx, mapy]) s[0] += ",Hills";
                    if (map.tree[mapx, mapy]) s[0] += ",Forest";

                    s[0] += " [P:" + map.yield[mapx, mapy].production + " G:" + map.yield[mapx, mapy].gold + " F:" + map.yield[mapx, mapy].food + "]";

                    //line 2 Resource: oranges Infl: player (Biscuit)
                    s[1] = "Resource: " + ((map.resource[mapx, mapy] != null) ? map.resource[mapx, mapy].name : "none")+" Owner: ";
                    if (map.citystates[mapx, mapy] != null) s[1] += map.citystates[mapx, mapy].name;

                    else
                    {
                        if (map.influence[mapx, mapy] == null) s[1] += "none";
                        else
                        {
                            if (map.influence[mapx, mapy] == true) s[1] += "player (";
                            else s[1] += "barbs (";

                            s[1] += map.citythathasinfluence[mapx, mapy].name + ")";
                        }
                    }
                    //line 3 name of building + improved yields
                    if (map.buildings[mapx, mapy] != Etilesprite.EMPTY)
                    {
                        if (map.buildings[mapx, mapy] == Etilesprite.BUILDINGS_IMPROVEMENTS_GENERIC_RESOURCE_EXPLOITATION) s[2] = map.resource[mapx, mapy].nameofexploiter + " ";
                        else s[2]= Tilestuff.tilestring[(int)map.buildings[mapx,mapy]+2]+" ";
                        if (map.addons[mapx, mapy] != null) s[2] += "(" + map.addons[mapx,mapy].owner.name+")";
                    }
                    s[2] += "[P:" + map.currentyield[mapx, mapy].production + " G:" + map.currentyield[mapx, mapy].gold + " F:" + map.currentyield[mapx, mapy].food + "]";
                    
                    if (map.buildings[mapx, mapy] == Etilesprite.BUILDINGS_CITY || map.buildings[mapx, mapy] == Etilesprite.BUILDINGS_BARBARIAN_CAMP || map.buildings[mapx, mapy] == Etilesprite.BUILDINGS_BARBARIAN_CITADEL)
                    {//x units use g:10 f:20 of [p:1 g:3 f:3]
                        Ccity cctv = map.citythathasinfluence[mapx, mapy];
                        s[3] += cctv.unitlist.Count + " units use G:" + cctv.armycostperturn_gold + " F:" + cctv.armycostperturn_food + " of [P:" + cctv.perturnyields.production + " G:" + cctv.perturnyields.gold + " F:" + cctv.perturnyields.food + "]";
                        s[4] = "Growth " + cctv.growthcounter + "/" + cctv.arbitrary_growth_value+" HP:"+cctv.hp+"/"+cctv.hpmax+" DEF:"+cctv.defence;
                    }
                    else if (map.buildings[mapx, mapy] == Etilesprite.BUILDINGS_BARRACKS)
                    {
                        s[4] = "Building:";
                        if (map.addons[mapx, mapy].mobtoproduce == null) s[4] += " nothing, idle.";
                        else s[4] += map.addons[mapx, mapy].mobtoproduce.name +
                                " P:" + map.addons[mapx, mapy].storedproduction + "/" + map.addons[mapx, mapy].mobtoproduce.buildcostproduction +
                                " I:" + map.addons[mapx, mapy].storediron+ "/" + map.addons[mapx, mapy].mobtoproduce.buildcostiron+
                        " H:" + map.addons[mapx, mapy].storedhorse + "/" + map.addons[mapx, mapy].mobtoproduce.buildcosthorses;


                    }
                    //s[5] is for mob details
                    s[5] = "Mob:";
                    item_instance i = map.itemgrid[mapx, mapy];
                    if (i != null)
                    {
                        s[5]+= Tilestuff.tilestring[(int)i.tile + 2];

                        if (i.ismob)// && !i.mob.dead_currently)
                        {
                            s[5] += " HP: " + i.mob.hp + "/" + i.mob.archetype.hp;                      
                        }

                    }
                    //else
                    //{
                    //    s = Tilestuff.tilestring[(int)map.displaychar[mapx, mapy] + 2];
                    //    if (map.mountain[mapx, mapy]) s += "[MTN]";
                    //    else if (map.hill[mapx, mapy]) s += "[HIL]";
                    //    if (map.tree[mapx, mapy]) s += "[FOR]";
                    //}

                    //debug:
                    //s += (map.passable[mapx, mapy]) ? " PASS" : " NOPASS";

                    for (int ii = 0; ii < 6; ii++)
                    {
                        if (s[ii] != "")
                        {
                            //string s = mob.mobname[(int)m.type]+" HP: "+m.hp;
                            byte[] bstr = System.Text.Encoding.ASCII.GetBytes(s[ii]);

                            int waffle = -16 - 8;// ((bstr.Length * 6) - 16) / 2;

                            int mausysubst = (mausy > 18) ? 18 : mausy;

                            for (int x = 0; x < bstr.Length; x++)
                            {
                                int y =(12*ii)+ (0 + (16 * mausysubst)) + 2;//- 12 - 12;
                               // if (y + (6 * 12) > 359) y = 359 - (6 * 12);
                                byte c = bstr[x];
                                int xpos = c % 32;
                                int ypos = 7 - (c / 32);
                                //   r.x =  ((31+(mausx*16) + (x * 6))-waffle) * zoomfactor;

                                r.x = ((0 + (mausx * 16)) - waffle);
                                if (r.x < 0) r.x = 0;
                                r.x = (r.x + (x * 6)) * zoomfactor;

                                r.y = (y) * zoomfactor;

                                r2.x = xratio * xpos;
                                r2.y = yratio * ypos;

                                GUI.color = Color.grey;
                                GUI.DrawTextureWithTexCoords(r, wednesdayfont, r2, false);
                                GUI.color = Color.white;
                                GUI.DrawTextureWithTexCoords(r, wednesdayfont, r2, true);
                               

                            }
                        }
                    }
               
                    

                }
                //end tooltips

                if (gamestate == Egamestate.gameover)
                {
                    GUI.color = whiteblend;
                    for (int x = 0; x < bstrGameOver.Length; x++)
                    {
                        int y = 0;
                        byte c = bstrGameOver[x];
                        int xpos = c % 32;
                        int ypos = 7 - (c / 32);
                        rbigtext.x = (0 + (x * 6 * 12)) * zoomfactor;
                        rbigtext.y = (0 + (y * 120)) * zoomfactor;

                        r2.x = xratio * xpos;
                        r2.y = yratio * ypos;

                        GUI.DrawTextureWithTexCoords(rbigtext, wednesdayfont, r2);
                    }

                    GUI.color = whiteblendvariable;
                    whiteblendvariable.a = 0.5f + ((((Time.time / 2) - (int)(Time.time / 2))) / 2);
                    //Debug.Log(Input.GetButtonDown("start").ToString());
                    for (int x = 0; x < bstrPressStart.Length; x++)
                    {
                        int y = 0;
                        byte c = bstrPressStart[x];
                        int xpos = c % 32;
                        int ypos = 7 - (c / 32);
                        r.x = (pressstartx + (x * 6)) * zoomfactor;
                        r.y = (pressstarty + 30) * zoomfactor;

                        r2.x = xratio * xpos;
                        r2.y = yratio * ypos;

                        GUI.DrawTextureWithTexCoords(r, wednesdayfont, r2);

                    }
                }//end if gameover state


                break;

        }//end switch gamestate

        //draw menu
        if (menuup)
        {
            //draw background
            GUI.color = currentmenu.colBackground;
            GUI.DrawTextureWithTexCoords(currentmenu.r, wednesdayfont, currentmenu.r2, true);
            //draw title
            GUI.color = currentmenu.colTitle;
            PrintString(currentmenu.titlepos, currentmenu.locy, currentmenu.title);
            //draw choices
            //GUI.color = currentmenu.colText;
            for(int upto = 0; upto < currentmenu.number_of_options; upto++)
            {
                if (currentmenu.optiondisabled[upto]) GUI.color = currentmenu.colDisabled;
                else GUI.color = currentmenu.colText;
                PrintString(currentmenu.locx+12,currentmenu.locy+((upto+2)*12),currentmenu.optionstrings[upto]);
            }
            //draw selector
            GUI.color = currentmenu.colTitle;
            PrintOneChar(currentmenu.locx + 6, currentmenu.locy + (currentmenu.currently_selected_option+2)*12, 16);
            
        }

    }

    //0 -> -1, (640*zoomfactor) -> 1
    //map the whole thing to go from 0 to 1
    //x/(640*zoomfactor)

    void Start()
    {
        //music 
        MyAudioSource = GetComponent<AudioSource>();

        

        //experimental snow
        //for (int i = 0; i < number_snow_particles; i++)
        //{
        //    snowx[i] = lil.randi(0,21*16);
        //    snowy[i] = lil.randi(0,479);
        //}
        //
        bstrPressStart = System.Text.Encoding.ASCII.GetBytes("Press " + "start" + " or Klik Left Maus");
        pressstartx = (640 - (bstrPressStart.Length * 6)) / 2;
        bool wtf = PlayerPrefs.GetInt("Screenmanager Is Fullscreen mode") == 1 ? true : false;

       // Debug.Log("width and height are " + Screen.currentResolution.width + " " + Screen.currentResolution.height);

        // Screen.SetResolution(640 * zoomfactor, 360 * zoomfactor, fullscreenp);

        spriteratio = 1f / (float)(sprites.width / 16);
   spriteratio3=(spriteratio / 16) * 3;
        r4.width = spriteratio;
        r4reverse.width = -spriteratio;
        particle = new Texture2D(1, 1, TextureFormat.ARGB32, false, false);//zoomfactor by zoomfactor
        particle.filterMode = FilterMode.Point;
        // for (int f = 0; f < zoomfactor; f++)
        //     for (int g = 0; g < zoomfactor;g++ )
        //        particle.SetPixel(f,g, Color.white);
        particle.SetPixel(0, 0, Color.white);
        particle.Apply();

        gamestate = Egamestate.titlescreen;
        MyAudioSource.clip = titlemusic;
        MyAudioSource.loop = true;
        MyAudioSource.Play();
    }

    void StartAGame()
    {
        

        //MyAudioSource.Stop();
        MyAudioSource.clip =backmusic;
        MyAudioSource.loop = true;
        MyAudioSource.Play();

        log = new MessageLog(50, 15);
        log.Printline("Settler by The Broom Institute: 7DRL 2017");
        log.Printline("This is the 7DRL work in progress.");
      
        //log.Printline("Resolution is " + Screen.currentResolution.width + " x " + Screen.currentResolution.height);
        lil.seednow();
        player = new Player(0);

        NextLevel();
    }
    void NextLevel()
    {
        //reset command like things 
        nextfire = 0.0f;
        firerate = 0.2f;
        initialdelay = 0.5f;
        currentcommand = -1;
        keydown = false;
        firstpress = false;
        mauswalking = false;
        //
        if (player.dunlevel == 10)
        {
            log.Printline("Now it is safe to settle in these lands!", Color.white);
            gamestate = Egamestate.youwon;
            MyAudioSource.Stop();
            return;
        }
        map = new RLMap(player, DungeonGenType.Settler2017);
        
        r_minimap = new Rect(336 * zoomfactor, 0, map.width * 2 * zoomfactor, map.height * 2 * zoomfactor);//was 339

        //map.fillminimap();

        //take this map reveal cheat out 
        /*
        for (int y = 0; y < map.height; y++)
        {
            for (int x = 0; x < map.width; x++)
            {
                Etilesprite et = map.displaychar.AtGet(x, y);
                int pango = (int)et;
                // if (pango < 1 || pango > 255) Debug.Log("bad pango " + pango);

                if (map.itemgrid[x, y] != null) map.minimap.SetPixel(x, y, (Color)map.minimapcolours[(int)map.itemgrid[x, y].tile]);
                else map.minimap.SetPixel(x, y, (Color)map.minimapcolours[(int)et]);
            }
        }*/
        //end cheat 


        int freex, freey;
        map.FreeSpace(out freex, out freey);
        player.emerge(freex, freey);

        moveplayer();

        gamestate = Egamestate.playing;
        TimeEngine = CradleOfTime.ready_to_process_turn;

        for (int y = 0; y <map.height; y++)
        {
            for (int x = 0; x <map.width; x++)
            {
                map.gridflashcolour[x,y] = new Color(lil.randf(0f, 1f), lil.randf(0f, 1f), lil.randf(0f, 1f),0.2f);
                map.gridflashtime[x, y] = Time.time + lil.randf(0f, 1f);
            }
        }


    }




    void moveplayer()
    {
        //map.do_fov_rec_shadowcast(player.posx, player.posy, 11);
        doplayerfovandlights();
        
    }

    float nextfire = 0.0f;
    float firerate = 0.2f;
    float initialdelay = 0.5f;
    int currentcommand = -1;
    bool keydown = false;
    bool firstpress = false;
    bool mauswalking = false;

    void Update()
    {
        switch (gamestate)
        {
            case Egamestate.titlescreen:
                if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("start"))
                {
                    StartAGame();
                }
                break;
            case Egamestate.playing:
            case Egamestate.gameover:
            case Egamestate.youwon:

                if (Input.GetButtonDown("showresource")) showresource = !showresource;
                if (Input.GetButtonDown("showyield")) showyield = !showyield;


                //FIRST LET'S MOVE THE FLOATZING TEXT
                if (floating_text_timer.test())
                {
                    for (int f = FloatingTextItems.Count - 1; f > -1; f--)
                    {
                        FloatingTextItem fti = FloatingTextItems[f];
                        fti.fade -= 0.01f;
                        fti.rise++;
                        if (fti.fade <= 0) FloatingTextItems.Remove(fti);
                    }

                    floating_text_timer.reset();
                }
                //do snow
                //if (snow_timer.test()) { 
                //    for (int f = 0; f < number_snow_particles; f++)
                //    {
                //        //       float tx = (StoredNoise[(screenx * 16) + f] + 1.0f) * 4.5f;
                //        //       float ty = (StoredNoise[(screeny * 16) + f + 8] + 1.0f) * 4.5f;
                //        //
                //        int sxf = (int)snowx[f];
                //        int syf = (int)snowy[f];
                //        if (sxf < 0) sxf = 0; if (syf < 0) syf = 0;
                //        if (sxf > 639) sxf = 639; if (syf > 479) syf = 479;
                //        snowx[f] += StoredNoise[sxf] * 2;
                //        snowy[f] += (StoredNoise2[syf] + StoredNoise2[sxf]) * 1f;// + lil.randf(0, 1) ;
                //        if (snowy[f] > 479) { snowy[f] = 0; snowx[f] = lil.randf(0, 21*16); }
                //    }
                //    snow_timer.reset();
                //}
                //NEXT LETS PROCESS LE TURN
                if (TimeEngine == CradleOfTime.ready_to_process_turn ||
                    TimeEngine == CradleOfTime.player_is_done)
                    ProcessTurn();

                //RIGHT CLICK
                if (gamestate == Egamestate.gameover||gamestate==Egamestate.youwon)
                {
                    if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("start"))
                    {
                        gamestate = Egamestate.titlescreen;
                        MyAudioSource.clip = titlemusic;
                        MyAudioSource.loop = true;
                        MyAudioSource.Play();
                        return;
                    }
                }
                //unless it's the player's turn, go away!
                if (TimeEngine != CradleOfTime.waiting_for_player) return;

                //is this the right place to put this?
                //deal with input for menus
                if (Game.menuup)
                {
                    if (Input.GetButtonDown("up"))
                    {
                        currentmenu.currently_selected_option--;
                        if (currentmenu.currently_selected_option == -1) currentmenu.currently_selected_option = currentmenu.number_of_options-1;

                    }
                    else
                         if (Input.GetButtonDown("down"))
                    {
                        currentmenu.currently_selected_option++;
                        if (currentmenu.currently_selected_option == currentmenu.number_of_options) currentmenu.currently_selected_option = 0;

                    }
                    else
                         if (Input.GetButtonDown("start")&&!Game.currentmenu.optiondisabled[currentmenu.currently_selected_option])
                    {
                        Game.menuup = false;
                        Game.menucommandwaiting = true;
                    }
                    else
                         if (Input.GetButtonDown("cancel"))
                    {
                        Game.menuup = false;
                    }

                    return;//skipping the rest of the stuff in this function (maus walking, commands etc.) is it ok?
                    //tell meh if it's ok, barnabeh. ah need to know, barnabeh! hold meh, barnabeh!
                } else
                {
                    //menu is not up. maybe a command is waiting
                    if (Game.menucommandwaiting)
                    {
                        switch (Game.currentmenu.type)
                        {
                            
                            case Menu.Emenuidentity.build:
                                //take the money, place the building, alter yields, do city stuff if city

                                player.gold -= Ccity.addons[Game.currentmenu.currently_selected_option].cost;
                                map.buildings[player.posx, player.posy] = Ccity.addons[Game.currentmenu.currently_selected_option].tile;

                                //building on a tile destroys resources unless they are deep in the earth. deep! beyond the touch of bristle. that meanz joo, broomster.
                                if (Game.currentmenu.currently_selected_option != 3 && map.resource[player.posx,player.posy]!=null && map.resource[player.posx,player.posy].destroyedbybuilding) map.resource[player.posx, player.posy] = null;

                                Ccity citeh;

                                //alter yields, do city stuff for city
                                switch (Game.currentmenu.currently_selected_option)
                                {
                                    case 0://city
                                        citeh = new Ccity(true,player.posx, player.posy,map,player,log);
                                        log.Printline("The city of " + citeh.name + " was founded!", Color.magenta);
                                        break;
                                    case 1://farm
                                        map.currentyield[player.posx, player.posy].add(0, 0, 2);
                                        map.citythathasinfluence[player.posx, player.posy].perturnyields.add(0, 0, 2);
                                        log.Printline("You build a farm. [+2 food]", Color.yellow);
                                        //map.citythathasinfluence[player.posx, player.posy].recalcyield();
                                        break;
                                    case 2://mine
                                        map.currentyield[player.posx, player.posy].add(1, 2, 0);
                                        map.citythathasinfluence[player.posx, player.posy].perturnyields.add(1,2,0);
                                        log.Printline("You build a mine. [+1 prod +2 gold]", Color.yellow);
                                        //map.citythathasinfluence[player.posx, player.posy].recalcyield();
                                        break;
                                    case 3://resource exploiter
                                        map.currentyield[player.posx, player.posy].add(map.resource[player.posx, player.posy].yieldwhenworked);
                                        map.citythathasinfluence[player.posx, player.posy].perturnyields.add(map.resource[player.posx, player.posy].yieldwhenworked);
                                        //add resources too, as in coffee, horses:
                                        map.citythathasinfluence[player.posx, player.posy].perturnresources[(int)map.resource[player.posx, player.posy].ert]++;
                                        log.Printline("You build a " + map.resource[player.posx, player.posy].nameofexploiter
                                            + ". [P:" + map.resource[player.posx, player.posy].yieldwhenworked.production
                                            + " G:" + map.resource[player.posx, player.posy].yieldwhenworked.gold
                                            + " F:" + map.resource[player.posx, player.posy].yieldwhenworked.food + "]"
                                            , Color.yellow);
                                        //map.citythathasinfluence[player.posx, player.posy].recalcyield();
                                        break;
                                }

                                if (Game.currentmenu.currently_selected_option >= 4)
                                {
                                    //find owning city
                                    Ccity thecity = findcity9way(player.posx, player.posy);
                                    if (thecity == null) log.Printline("ERROR CITY ADDON CAN'T FIND CITY IT BELONGS TO!");
                                    else
                                    {
                                        //make a new city addon owned by the city we found and of type according to menu option (they're in the right order)
                                        addoninstance a = new addoninstance(thecity, Ccity.addons[Game.currentmenu.currently_selected_option], player.posx, player.posy);
                                        //add the city addon to the map's grid of them
                                        map.addons[player.posx, player.posy] = a;
                                        log.Printline("You build a " + a.type.name + ".", Color.yellow);

                                        switch (a.type.tile)
                                        {
                                            case Etilesprite.BUILDINGS_FACTORY:
                                                map.currentyield[player.posx, player.posy].production += 10;                                                
                                                break;
                                            case Etilesprite.BUILDINGS_ALLOTMENTS:
                                                map.currentyield[player.posx, player.posy].food += 10;
                                                break;
                                            case Etilesprite.BUILDINGS_MARKET:
                                                map.currentyield[player.posx, player.posy].gold += 10;
                                                break;
                                            case Etilesprite.BUILDINGS_PORT_AND_DOCKS:
                                                 map.currentyield[player.posx, player.posy].food += 6;
                                                map.currentyield[player.posx, player.posy].production += 6;
                                                    map.currentyield[player.posx, player.posy].gold += 6;
                                                break;

                                            case Etilesprite.BUILDINGS_GUARD_POST:
                                                thecity.defence += 1;
                                                break;

                                            case Etilesprite.BUILDINGS_ARMOURER:
                                                thecity.defencebonus++;
                                                break;
                                            case Etilesprite.BUILDINGS_BLACKSMITH:
                                                thecity.attackbonus++;
                                                break;
                                            case Etilesprite.BUILDINGS_TOWN_HALL:
                                                thecity.growthboost = true;
                                                break;

                                                //teleporter
                                                //trader


                                        }
                                        thecity.recalcyield();

                                    }

                                    

                                }






                                break;
                            //end of build menu
                            case Menu.Emenuidentity.unitproduce:
                                if (Game.currentmenu.currently_selected_option == 0)
                                {
                                    log.Printline("You halt production.", Color.yellow);
                                    map.addons[player.posx, player.posy].mobtoproduce = null;
                                }
                                else
                                {
                                    int which = Game.currentmenu.currently_selected_option + 1;
                                    log.Printline("Now building " + mob.archetypes[which].name + ".", Color.yellow);
                                    map.addons[player.posx, player.posy].mobtoproduce = mob.archetypes[which];
                                }
                                break;

                                //end of unitproduce menu
                        }
                        Game.menucommandwaiting = false;
                        TimeEngine = CradleOfTime.player_is_done;
                        return;
                    }//end game command waiting
                }
                
                

                    if (Input.GetMouseButtonDown(0))
                {
                    int fx = (int)Input.mousePosition.x; int fy = (int)((360 * zoomfactor) - 1 - Input.mousePosition.y);
                    //log.Printline("x = " + fx.ToString() + " y = " + fy.ToString());
                    int x = fx / zoomfactorx16;
                    int y = fy / zoomfactorx16;

                    if (x < VIEWPORT_WIDTH && y < VIEWPORT_HEIGHT)
                    {
                        x = originx + x; y = originy + y;
                        if (map.passablecheck(x, y, player.mob) && !(x == player.posx && y == player.posy))//change this to check moop
                        {
                            bool worked = map.PathfindAStar(player.posx, player.posy, x, y, true, true,player.mob.archetype.heavy);
                            if (worked)
                            {
                                mauswalking = true;
                                //log.Printline("And we're walking!");
                            } //else log.Printline("didn't work");
                        } //else log.Printline("not passable or clicked player");
                    }
                    else {//outside viewport
                        int mmx = ((fx / zoomfactor) - 336) / 2;
                        int mmy = ((fy / zoomfactor) / 2);
                        //log.Printline(mmx.ToString() + " " + mmy.ToString() + " ");
                        if (mmx >= 0 && mmx < map.width && mmy >= 0 && mmy < map.height)
                        {
                            if (map.passablecheck(x, y, player.mob) && !(mmx == player.posx && mmy == player.posy))//change this to check moop
                            {
                                bool worked = map.PathfindAStar(player.posx, player.posy, mmx, mmy, true, true,player.mob.archetype.heavy);
                                if (worked)
                                {
                                    mauswalking = true;
                                }
                            }
                        }
                    }
                }
                // else if (Input.GetMouseButtonDown(1))
                // {
                // FloatingTextItems.Add(new FloatingTextItem("-10 hp", player.posx, player.posy, Color.red));
                // FloatingTextItems.Add(new FloatingTextItem("In the valleh of the shadow of death", player.posx, player.posy, Color.red));
                // }
                //todo: fix it so if you press a key or press the maus when mauswalking it breaks the auto walk

                if (mauswalking)
                {
                    //log.Printline(map.lastpath.Count.ToString());
                    if (map.lastpath.Count < 1)
                    {
                        mauswalking = false;
                        currentcommand = -1;
                    }
                    else {
                        Cell next = map.lastpath.Dequeue();
                        if (next.x == player.posx && next.y < player.posy) { currentcommand = 0; nextfire = 0.0f; firstpress = false; } //up
                        else if (next.x == player.posx && next.y > player.posy) { currentcommand = 1; nextfire = 0.0f; firstpress = false; }//down
                        else if (next.x < player.posx && next.y == player.posy) { currentcommand = 2; nextfire = 0.0f; firstpress = false; }//left
                        else if (next.x > player.posx && next.y == player.posy) { currentcommand = 3; nextfire = 0.0f; firstpress = false; }//right
                        else if (next.x < player.posx && next.y < player.posy) { currentcommand = 6; nextfire = 0.0f; firstpress = false; }//upleft
                        else if (next.x > player.posx && next.y < player.posy) { currentcommand = 7; nextfire = 0.0f; firstpress = false; }//upright
                        else if (next.x < player.posx && next.y > player.posy) { currentcommand = 8; nextfire = 0.0f; firstpress = false; }//downleft
                        else if (next.x > player.posx && next.y > player.posy) { currentcommand = 9; nextfire = 0.0f; firstpress = false; }//downright
                    }
                }//end if mauswalking
                else {
                    if (Input.GetButtonDown("up")) { currentcommand = 0; nextfire = 0.0f; firstpress = true; }
                    else if (Input.GetButtonDown("down")) { currentcommand = 1; nextfire = 0.0f; firstpress = true; }
                    else if (Input.GetButtonDown("left")) { currentcommand = 2; nextfire = 0.0f; firstpress = true; }
                    else if (Input.GetButtonDown("right")) { currentcommand = 3; nextfire = 0.0f; firstpress = true; }
                    else if (Input.GetButtonDown("use")) { currentcommand = 4; nextfire = 0.0f; firstpress = true; }
                    else if (Input.GetButtonDown("wait")) { currentcommand = 5; nextfire = 0.0f; firstpress = true; }
                    else if (Input.GetButtonDown("upleft")) { currentcommand = 6; nextfire = 0.0f; firstpress = true; }
                    else if (Input.GetButtonDown("upright")) { currentcommand = 7; nextfire = 0.0f; firstpress = true; }
                    else if (Input.GetButtonDown("downleft")) { currentcommand = 8; nextfire = 0.0f; firstpress = true; }
                    else if (Input.GetButtonDown("downright")) { currentcommand = 9; nextfire = 0.0f; firstpress = true; }
                    else if (Input.GetButtonDown("action")) { currentcommand = 10; nextfire = 0.0f; firstpress = true; }
                }

                if (currentcommand > -1 && Time.time > nextfire)
                {
                    switch (currentcommand)
                    {
                        case 0:
                            trytomove(player.mob,0,-1);//trytomove(0, -1);//N
                            break;
                        case 1:
                            trytomove(player.mob, 0,1);// trytomove(0, 1);//S
                            break;
                        case 2:
                            trytomove(player.mob, -1,0);// trytomove(-1, 0);//W
                            break;
                        case 3:
                            trytomove(player.mob, 1,0);// trytomove(1, 0);//E
                            break;
                        case 4://was 4
                            //from when command 4 was lantern. now it's "use"
                            //{ player.lantern = !player.lantern; moveplayer(); }
                            useobject(); 
                           
                            break;

                        case 5:
                            log.Printline((player.mob.speed>0)?"You coast.":"You wait.");trytomove(player.mob, 0,0);
                            break;
                        case 6:
                            trytomove(player.mob, -1,-1);//trytomove(-1, -1);// NW
                            break;
                        case 7:
                            trytomove(player.mob, 1,-1);//trytomove(1, -1);//NE
                            break;
                        case 8:
                            trytomove(player.mob, -1,1);// trytomove(-1, 1);//SW
                            break;
                        case 9:
                            trytomove(player.mob, 1,1);// trytomove(1, 1);//SE
                            break;
                        case 10://was 10
                            doaction();
                            break;
                    }
                    nextfire = Time.time + firerate;
                    if (firstpress) { nextfire += initialdelay; firstpress = false; }
                }

                if ((Input.GetButtonUp("up")) ||
                (Input.GetButtonUp("down")) ||
                (Input.GetButtonUp("left")) ||
              (Input.GetButtonUp("right")) ||
                 (Input.GetButtonUp("use")) ||
                 (Input.GetButton("action")) || //notice it's GetButton not GetButtonUp, so this doesn't work and "action" doesn't repeat
                (Input.GetButtonUp("wait")) ||
                 (Input.GetButtonUp("upleft")) ||
                (Input.GetButtonUp("upright")) ||
                 (Input.GetButtonUp("downleft")) ||
                (Input.GetButtonUp("downright"))) { currentcommand = -1; }

                break;
        }

    }
}
