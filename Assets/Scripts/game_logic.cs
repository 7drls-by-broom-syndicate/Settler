using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class Game : MonoBehaviour
{
    Color colour_snakespit = new Color(0.125f, 0.85f, 0.05f, 0.5f);
    Color ice_beam = new Color(0f, 0.2f, 0.7f, 0.5f);
    Color colour_damage = new Color(0.85f, 0.05f, 0.125f, 0.5f);

    Color colour_influenceplayer = new Color(0, 0, 0.85f, 0.25f);
    Color colour_influenceenemy = new Color(0.85f, 0, 0, 0.25f);


    // bool trytomove(int deltax, int deltay) {

    void doplayerfovandlights()
    {
        map.do_fov_rec_shadowcast(player.posx, player.posy, 5);//was 11
        map.dynamiclight.Fill(Color.black);
        if (player.lantern)
            map.do_fov_foradynamiclight(player.posx, player.posy, 11, Color.white);//was 9

        if (lil.totalcolour(map.dynamiclight.AtGet(player.posx, player.posy)) == 0f &&
            lil.totalcolour(map.dynamiclight.AtGet(player.posx, player.posy)) == 0f)
            player.stealthed = true;
        else
            player.stealthed = false;
    }

    void movemob(mob m, int tentx, int tenty)
    {
        if (m.isplayer)
        {
            m.posx = tentx; m.posy = tenty;
            moveplayer();
        }
        else
        {
            if (m.posx != tentx || m.posy != tenty)
            {
                //debug move onto bomb?
                if (map.itemgrid[tentx, tenty] != null)
                {
                    item_instance i = map.itemgrid[tentx, tenty];
                    log.Printline("WARNING MOB MOVING ONTO " +
                        Tilestuff.tilestring[(int)i.tile + 2]
                        );
                }
                //end debug
                map.itemgrid[tentx, tenty] = map.itemgrid[m.posx, m.posy];
                map.itemgrid[m.posx, m.posy] = null;
                m.posx = tentx; m.posy = tenty;
            }

        }
    }

    mob CreateMob(Emobtype t, int tentx, int tenty)
    {
        mob m = new mob(t);
        m.posx = tentx; m.posy = tenty;
        map.itemgrid[tentx, tenty] = new item_instance(m.tile, true, m);
        map.passable[tentx, tenty] = false;
        map.newmoblist.Add(m);
        return m;
    }

    bool IsEmpty(int x, int y)
    {
        if (x < 0 || x >= map.width || y < 0 || y >= map.height) return false;//off map
        if (!map.passable[x, y]) return false;//this ignores the fact moops(and golems=also heavy) or fliers could be fine on water
        if (map.itemgrid[x, y] != null) return false;//a mob is on the square
        if (x == player.posx && y == player.posy) return false;//player is on square
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
    bool checkforitemactivation(mob m, int tentx, int tenty)
    {
        return false;
        //item_instance i = map.itemgrid[tentx, tenty];
        //if (i == null) return false;
        //if (i.tile == Etilesprite.ITEM_BARREL)
        //{
        //    //open barrel
        //    if (map.extradata[tentx, tenty] != null)
        //    {

        //        i.tile = (Etilesprite)map.extradata[tentx, tenty].x; //was Etilesprite.ITEM_WARP_BEADS; when there was only warp beads in barrels
        //        if (i.tile == Etilesprite.ENEMY_HOPPED_UP_FOX)
        //        {
        //            i.ismob = true;
        //            mob mm = new mob(Emobtype.fox);
        //            mm.posx = tentx;mm.posy = tenty;
        //            i.mob = mm;
        //            map.moblist.Add(mm);
        //        }
        //        log.Printline("Inside the barrel was a "+Tilestuff.tilestring[(int)i.tile+2], Color.blue);
        //        map.extradata[tentx, tenty] = null;
        //        return true;
        //    }
        //    else
        //    {
        //        log.Printline("Take that, you barrel bastard!", Color.gray);
        //        i.tile = Etilesprite.ITEM_BARREL_BROKEN;
        //        map.extradata[tentx, tenty] = null;
        //        return true;
        //    }
        //}
        //else if (i.tile == Etilesprite.ITEM_BARREL_BROKEN)
        //{
        //    log.Printline(m.archetype.name + " plays cleanup!", Color.gray);
        //    map.itemgrid[tentx, tenty] = null;
        //    map.passable[tentx, tenty] = true;
        //    return true;
        //}
        //else if (i.tile == Etilesprite.ITEM_WARP_BEADS)
        //{
        //    log.Printline(m.archetype.name + " collects the Warp Beads!", Color.magenta);
        //    map.itemgrid[tentx, tenty] = null;
        //    map.passable[tentx, tenty] = true;
        //    m.hasbeads = true;
        //    return true;
        //}
        //else if (i.tile == Etilesprite.ITEM_CAIRN_RED)
        //{
        //    log.Printline("The ", Color.gray);
        //    log.Print("red cairn ", Color.red);
        //    log.Print("repairs your body!");
        //    FloatingDamage(m, m, +5 + lil.randi(0, 10), "magic");
        //    i.tile = Etilesprite.ITEM_CAIRN_USED_RED;
        //    map.dostaticlights();
        //    return true;
        //}
        //else if (i.tile == Etilesprite.ITEM_CAIRN_GREEN)
        //{
        //    log.Printline("The ", Color.gray);
        //    log.Print("green cairn ", Color.green);
        //    log.Print("is activated: nature is angry!");
        //    //now we want to reach out and strike 1-3 mobs on screen. if no mobs on screen it hits you!
        //    //List<mob> mablist = map.moblist.Where(s => RLMap.Distance_Euclidean(s.posx, s.posy, tentx, tenty) < 10);
        //    //later on figure out why this doesn't work
        //    List<mob> mablist = new List<mob>();
        //    foreach (var mab in map.moblist)
        //    {
        //        if (RLMap.Distance_Euclidean(mab.posx, mab.posy, tentx, tenty) < 10)
        //            mablist.Add(mab);
        //    }
        //    if (mablist.Count == 0)
        //    {
        //        //attack player
        //        FloatingDamage(m, m, -5 - lil.randi(0, 5), "nature");

        //    }
        //    else
        //    {
        //        foreach (var mab in mablist)
        //        {
        //            BresLineColour(tentx, tenty, mab.posx, mab.posy, true, false, colour_snakespit);
        //            FloatingDamage(mab, mab, -5 - lil.randi(0, 5), "nature");
        //        }
        //    }

        //    i.tile = Etilesprite.ITEM_CAIRN_USED_GREEN;
        //    map.dostaticlights();
        //    return true;
        //}
        //else if (i.tile == Etilesprite.ITEM_CAIRN_BLUE)
        //{
        //    log.Printline("The ", Color.gray);
        //    log.Print("blue cairn ", Color.blue);
        //    log.Print("lends you its power!");
        //    int which = lil.randi(1, 100);
        //    if (which < 50 || (m.hasattackup && !m.hasdefenseup))
        //    {
        //        log.Print(m.archetype.name + " gains the buff: defense up!", Color.blue);
        //        //if (m.hasdefenseup) log.Print("Which " + m.archetype.name + " already had, oh well.");
        //        m.hasdefenseup = true;
        //        m.defenseuptimer += 15;
        //    }
        //    else
        //    {
        //        log.Print(m.archetype.name + " gains the buff: attack up!", Color.blue);
        //        //if (m.hasattackup) log.Print("Which " + m.archetype.name + " already had, oh well.");
        //        m.hasattackup = true;
        //        m.attackuptimer += 15;
        //    }
        //    i.tile = Etilesprite.ITEM_CAIRN_USED_BLUE;
        //    map.dostaticlights();
        //    return true;
        //}
        //else if (i.tile == Etilesprite.ITEM_CAIRN_PURPLE)
        //{
        //    log.Printline("The ", Color.gray);
        //    log.Print("purple cairn ", Color.magenta);
        //    log.Print("twists space around you!");
        //    int otherx = map.extradata[tentx, tenty].x;
        //    int othery = map.extradata[tentx, tenty].y;
        //    Cell c = Random9way(otherx, othery);
        //    if (c == null)
        //    {
        //        log.Printline("The cairn should transport you", Color.blue);
        //        log.Printline("but it is having problems!", Color.blue);
        //    }
        //    else
        //    {
        //        //if a mob uses this it's going to be messed up because need to change map.itemgrids
        //        m.posx = c.x; m.posy = c.y;
        //        cairntransport_effect(c.x, c.y);

        //    }
        //    item_instance i2 = map.itemgrid[otherx, othery];
        //    if (i2 == null) log.Printline("ERROR at receiving cairn.");
        //    i.tile = Etilesprite.ITEM_CAIRN_USED_PURPLE;
        //    i2.tile = Etilesprite.ITEM_CAIRN_USED_PURPLE;
        //    map.dostaticlights();
        //    moveplayer();
        //    return true;
        //}
        ////new. pickup
        //else if (item_instance.holdable_items.Contains(i.tile)){
        //    Etilesprite prev = player.held;
        //    log.Printline(m.archetype.name + " collects a "+Tilestuff.tilestring[(int)i.tile+2], Color.grey);
        //    player.held = i.tile;
        //    if (prev == Etilesprite.EMPTY)
        //    {
        //        map.itemgrid[tentx, tenty] = null;
        //        map.passable[tentx, tenty] = true;
        //    }
        //    else
        //    {
        //        map.itemgrid[tentx, tenty].tile = prev;
        //        log.Printline(m.archetype.name + " puts down a " + Tilestuff.tilestring[(int)prev + 2],Color.grey);
        //    }


        //    return true;
        //}
        //return false;
    }
    void cairntransport_effect(int xx, int yy)
    {
        for (int y = 0; y < map.height; y++)
        {
            for (int x = 0; x < map.width; x++)
            {
                map.gridflashcolour[x, y] = new Color(110f / 255f, 37f / 255f, 125f / 255f, 0.8f);
                map.gridflashtime[x, y] = Time.time + (0 + (float)(RLMap.Distance_Euclidean(x, y, xx, yy) / 10.0f));
                //lil.randf(0f, 1f);
            }
        }
    }
    bool trytomove(mob m, int deltax, int deltay)//, int rotdir, bool coasting = false)
    {
        bool didsomething = false;

        if (deltax == 1) m.reversesprite = false;
        else if (deltax == -1) m.reversesprite = true;

        int tentx = m.posx + deltax;
        int tenty = m.posy + deltay;

        //if tentative spot is off map, return
        if (tentx < 0 || tentx >= map.width || tenty < 0 || tenty >= map.height)
        {
            return false;
        }

        //if tentative spot is goodie hut, let's open it
        if(m==player.mob && map.buildings[tentx, tenty] == Etilesprite.BUILDINGS_GOODIE_HUT && map.itemgrid[tentx,tenty]==null)
        {
            if (!map.havewehadbroomyet && lil.randi(1, 100) <= 20)
            {
                log.Printline("o shit waddup, heer come dat broom!", Color.cyan);
                log.Printline("Bristleboi is on the payroll nao. 10g/turn.", Color.cyan);
                mob mmm=CreateMob(Emobtype.broom, tentx, tenty);
                player.herogoldupkeep += mmm.archetype.upkeepgold;
            } else
            {
                log.Printline("You find a bunch of gold", Color.yellow);
                player.gold += 200;
            }
            map.buildings[tentx, tenty] = Etilesprite.EMPTY;
        }


        //if tentative spot is enemy city, attack it
        if (map.buildings[tentx, tenty] == Etilesprite.BUILDINGS_CITY
            || map.buildings[tentx,tenty]==Etilesprite.BUILDINGS_BARBARIAN_CAMP
            || map.buildings[tentx,tenty]==Etilesprite.BUILDINGS_BARBARIAN_CITADEL)
        {
            Ccity cc = map.citythathasinfluence[tentx, tenty];
            if (cc!=null && (m.hostile_toenemies_currently && !cc.isfrenzleecity
                || m.hostile_toplayer_currently && cc.isfrenzleecity))
            {
                MobAttacksCity(m, cc);
                didsomething = true;
                goto okhadfun;
            }
        }


        //if tentative spot has player and we are bad men, attack him!
        if(m.hostile_toplayer_currently && player.posx == tentx && player.posy == tenty)
        {
            MobAttacksMob(m, player.mob);
            didsomething = true;
            goto okhadfun;
        }

        //if spot is empty, move
        if (map.passablecheck(tentx, tenty, m))
        {
            movemob(m, tentx, tenty);
            didsomething = true;
            goto okhadfun;
            }

        //if spot has mob and it's one we hate, attack it
        item_instance i = map.itemgrid[tentx, tenty]; //1.is there a mob there:
        if (i != null && i.ismob &&
            (m.hostile_toenemies_currently && i.mob.hostile_toplayer_currently ||
                m.hostile_toplayer_currently && i.mob.hostile_toenemies_currently))
        {
            MobAttacksMob(m, i.mob);
            didsomething = true;
        }


        okhadfun:

        if (m.isplayer) { TimeEngine = CradleOfTime.player_is_done; return true; }
        return didsomething;
    }

    public bool water4way(int x, int y)
    {
        if (x >= 1 && water(x - 1, y)) return true;
        if (x <= map.width - 2 && water(x + 1, y)) return true;
        if (y >= 1 && water(x, y - 1)) return true;
        if (y <= map.height - 2 && water(x, y + 1)) return true;
        return false;

    }
    public bool water(int x, int y)
    {
        return (check3(map.displaychar[x, y], Etilesprite.BASE_TILE_OCEAN_1) || check3(map.displaychar[x, y], Etilesprite.BASE_TILE_COASTAL_WATER_1));
    }
    public bool checkforbuildings(int x, int y)
    {
        //check a 2 radius square round co-ords for buildings
        for (int xx = x - 2; xx < x + 3; xx++)
        {
            for (int yy = y - 2; yy < y + 3; yy++)
            {
                if (xx > 0 && yy > 0 && xx < map.width && yy < map.height && map.buildings[xx, yy] != Etilesprite.EMPTY
                    && map.buildings[xx, yy] != Etilesprite.BUILDINGS_IMPROVEMENTS_FARM
                    && map.buildings[xx, yy] != Etilesprite.BUILDINGS_IMPROVEMENTS_MINE
                    && map.buildings[xx, yy] != Etilesprite.BUILDINGS_IMPROVEMENTS_GENERIC_RESOURCE_EXPLOITATION) return true;
            }
        }


        return false;
    }
    public bool citycheck9way(int x, int y)
    {
        for (int xx = x - 1; xx < x + 2; xx++)
        {
            for (int yy = y - 1; yy < y + 2; yy++)
            {
                if (xx > 0 && yy > 0 && xx < map.width && yy < map.height && map.buildings[xx, yy] == Etilesprite.BUILDINGS_CITY)
                    return true;
            }
        }
        return false;
    }

    public Ccity findcity9way(int x, int y)
    {
        for (int xx = x - 1; xx < x + 2; xx++)
        {
            for (int yy = y - 1; yy < y + 2; yy++)
            {
                if (xx > 0 && yy > 0 && xx < map.width && yy < map.height && map.buildings[xx, yy] == Etilesprite.BUILDINGS_CITY)
                    return map.citythathasinfluence[xx, yy];//again, assuming every city has influence on itself at least, since you can't build one on an influenced square
            }
        }
        return null;
    }


    public bool check3(Etilesprite e, Etilesprite x)
    {
        //returns true if e matches x
        //check if supplied tile e is of type x, i.e. is the same or in the same 3 set
        //because there are 3 tiles for the biomes
        //this is just to neaten the code. speed is not an issue.

        int a = (int)e;
        int b = (int)x;

        return (a == b || a == b + 1 || a == b + 2);

    }


    public void doaction()
    {
        List<string> ls = new List<string>();

        if (map.buildings[player.posx, player.posy] != Etilesprite.EMPTY && map.buildings[player.posx, player.posy] == Etilesprite.BUILDINGS_BARRACKS)
        {
            var sit = map.addons[player.posx, player.posy].owner;
            ls.Add("--HALT PRODUCTION--");
            for (int i = 2; i < 9; i++)
            {
                var m = mob.archetypes[i];
                string s = "[P:" + m.buildcostproduction.ToString("D3") + " I:" + m.buildcostiron.ToString("D2") + " H:" + m.buildcosthorses.ToString("D2") + "] " +
                    m.name.PadRight(15) +
                    " (Atk:" + m.attacks + "@" + m.attacklow + "-" + m.attackhigh + "+" + sit.attackbonus + " Def:" + m.defence + "+" + sit.defencebonus + " Move:" + m.moves + " Upkeep:" + m.upkeepfood + "F," + m.upkeepgold + "G)";
                ls.Add(s);
            }

            Game.currentmenu = new Menu(Menu.Emenuidentity.unitproduce,
               "Unit to produce?", ls);
        }
        else
        {

            bool onbuilding = map.buildings[player.posx, player.posy] != Etilesprite.EMPTY;
            bool onwater = (
                check3(map.displaychar[player.posx, player.posy], Etilesprite.BASE_TILE_OCEAN_1) ||
                check3(map.displaychar[player.posx, player.posy], Etilesprite.BASE_TILE_COASTAL_WATER_1)
                );
            bool waternsew = water4way(player.posx, player.posy);

            bool onmountain = map.mountain[player.posx, player.posy];
            bool onhill = map.hill[player.posx, player.posy];
            bool onpolar = check3(map.displaychar[player.posx, player.posy], Etilesprite.BASE_TILE_POLAR_1);
            bool oninfluence = (map.influence[player.posx, player.posy] == true);
            int i = 0;



            foreach (var x in Ccity.addons)
            {
                //NOTE. WE DON'T ALLOW PLAYER TO BUILD A CITY UNLESS ON NEUTRAL GROUND
                //THIS IS BECAUSE UNLIKE NORMAL CIV, WHICH CITY "OWNS" A TILE IN TERMS OF INFFLUENCE IS IMPORTANT
                //BECAUSE RESOURCES AND TILE YIELDS GO TO THAT CITY. OR MAYBE THAT DOES HAPPEN IN CIV.
                //WHY AM I TYPING IN CAPS


                if (x.cost > player.gold //disable if can't afford it
                    || onbuilding//can't replace one building with another. change this later eg build city on farm etc.
                    || (i != 3 && onwater)//if on water disable everything except resource exploiter
                    || (i == 3 && map.resource[player.posx, player.posy] == null)//if no resource disable resource exploiter
                   || (i >= 4 && !citycheck9way(player.posx, player.posy))//if there isn't a city in 9 way disable all city addons
                    || (i == 10 && !waternsew)//if there isn't 4 way access to coastal water or ocean , disable port and docks
                    || (i == 2 && !(onhill || onmountain)) //if not on hills or mountains disable mine
                    || (i == 1 && (onmountain || onpolar || (onhill && !waternsew)))//if on mountains or polar biome or (hill with no 4way water) disable farm
                    || (i == 0 && (
                            onmountain ||
                            onwater ||
                            checkforbuildings(player.posx, player.posy) ||
                            map.influence[player.posx, player.posy] != null))//can't build unless influence is neutral.
                   || ((i > 0 && i < 4) && !oninfluence)
                    )
                    ls.Add(("/" + x.name + " (" + x.cost + ")").PadRight(26)+x.explain);
                else ls.Add((x.name + " (" + x.cost + ")").PadRight(26)+x.explain);

                i++;
            }

            //if not in city influence disable farm, mine and resource exploiter




            Game.currentmenu = new Menu(Menu.Emenuidentity.build,
                "Build", ls);

        }

        Game.menuup = true;
        Game.currentmenu.setpos(32, 32);
        //   log.Printline("-action- pressed", Color.grey);
        //  TimeEngine = CradleOfTime.player_is_done;
    }
    public void useobject()
    {

        //test
        // Game.currentmenu = new Menu("PICK WIESLY", new List<string> { "here's the first option", "the second option is about lemurs", "build a city", "whatever" });
        // Game.menuup = true;
        // Game.currentmenu.setpos(32, 32);
        // return;
        //end test


        //if (player.held == Etilesprite.EMPTY)
        //{
        //    log.Printline("Nothing to use", Color.grey);
        //}
        //else
        //{
        //    switch (player.held)
        //    {
        //        case Etilesprite.ITEM_RAW_MEAT:
        //            break;
        //        case Etilesprite.ITEM_BOMB:
        //            break;
        //        case Etilesprite.ITEM_TRAP:
        //            break;
        //        case Etilesprite.ITEM_SCROLL_FIRELANCE:
        //            break;
        //        case Etilesprite.ITEM_SCROLL_ICECUBE:
        //            break;
        //        case Etilesprite.ITEM_POTION_SPEED:
        //            break;
        //        case Etilesprite.ITEM_POTION_SPECIAL:
        //            break;
        //        case Etilesprite.ITEM_FISHING_ROD:
        //            break;
        //        case Etilesprite.ITEM_FISH:
        //            break;

        //    }


        //    log.Printline(player.mob.archetype.name + " uses the " + Tilestuff.tilestring[(int)player.held + 2], Color.blue);
        //}
        //TimeEngine = CradleOfTime.player_is_done;
    }

    void detonate(int xx, int yy)
    {
        for (int y = -1; y < 2; y++)
        {
            for (int x = -1; x < 2; x++)
            {
                int sqx = xx + x; int sqy = yy + y;
                if (sqx >= 0 && sqy >= 0 && sqx < map.width && sqy < map.height)
                {
                    //Debug.Log("ok");
                    map.gridflashcolour[sqx, sqy] = new Color(1.0f, 0f, 0f, 0.5f);
                    map.gridflashtime[sqx, sqy] = Time.time + 0.5f;
                    item_instance i = map.itemgrid[sqx, sqy];
                    if (player.posx == sqx && player.posy == sqy) FloatingDamage(player.mob, player.mob, -5, "explosion", true);
                    if (i != null)
                    {
                        if (i.ismob)
                        {
                            FloatingDamage(i.mob, i.mob, -5, "explosion", true);
                        }
                    }

                }
            }
        }
        //should maybe chain explosions
    }

    void ProcessTurn()
    {


        if (TimeEngine == CradleOfTime.ready_to_process_turn)
        {

            // if (map.timegrid.beatgrid[0, 0])
            // {
            TimeEngine = CradleOfTime.waiting_for_player;
            return;
            // }

        }

        TimeEngine = CradleOfTime.processing_other_actors;

        //effects and mobs get to act
        //stop being scared of "foreach": think of all the other shitty garbage you are creating

        //DEBUGGING THING
        //player.hp = 60; //HP CHEAT DEBUG
        //foreach (var debugMob in map.moblist)//there's a mob in the moblist
        //{
        //    if (map.itemgrid[debugMob.posx, debugMob.posy] == null)//the map's itemlist at the mob's position is null
        //    {
        //        log.Printline("BUG: mob at " + debugMob.posx + " " + debugMob.posy + " " + debugMob.archetype.name);
        //        log.Printline("where player at " + player.posx + " " + player.posy);
        //    }
        //}

        //END DEBUGGING THING

        //bool dirty= false;
        ////wizard walls
        //foreach(var f in map.walllist)
        //{
        //    f.bombcount--;
        //    if (f.bombcount == 0)
        //    {
        //        dirty = true;
        //        map.itemgrid[f.bombx, f.bomby] = null;
        //        map.passable[f.bombx, f.bomby] = true;
        //        map.blocks_sight[f.bombx, f.bomby] = false;

        //    }
        //}

        //if (dirty)
        //{
        //    map.walllist.RemoveAll(x => x.bombcount == 0);
        //    //redo FoV
        //    doplayerfovandlights();

        //    dirty = false;
        //}

        //city stuff

        foreach (var cc in map.citylist)
        {
            cc.takeaturn();
        }
        //bombs
        //foreach (var f in map.bomblist)
        //{
        //    f.bombcount++;
        //    if (f.bombcount >= 5)
        //    {
        //        dirty = true;
        //        log.Printline("Boom! The bomb explodes!", Color.red);
        //        int x = f.bombx; int y = f.bomby;
        //        map.itemgrid[x, y] = null;
        //        map.passable[x, y] = true;//fixing bug of invisible mobs, one hopes
        //        //do explosion and damage. set off other bombs
        //        detonate(x, y);
        //    }
        //    f.tile = Etilesprite.ITEM_BOMB_LIT_1 + (f.bombcount - 1);

        //}
        //if (dirty) {
        //    map.bomblist.RemoveAll(x => x.bombcount >= 5);
        //    dirty = false;
        //}


        //fire
        //foreach(var f in map.firelist)
        //{
        //    map.onfire[f.x, f.y]--;
        //    if (map.onfire[f.x, f.y] < 0)
        //    {
        //        map.onfire[f.x, f.y] = null;
        //        map.bloodgrid[f.x, f.y] = null;//blood is "used up" by the fire
        //        dirty = true;
        //    }
        //}
        //if (dirty)
        //    map.firelist.RemoveAll(z => map.onfire[z.x, z.y] == null);


        //mobs

        player.mob.usedDEFthisturn = false;

        foreach (var f in map.moblist)
        {
            for (int dodgynewbury = 0; dodgynewbury < f.archetype.moves;dodgynewbury++)
            {
                MobGetsToAct(f);
            }

        }

        //add any new mobs created this turn.
        if (map.newmoblist.Count > 0)
        {
            map.moblist.AddRange(map.newmoblist);
            map.newmoblist.Clear();
        }


        //check for mob hp so dead
        foreach (var f in map.moblist)
        {

            //if you can't afford heroes they have to go
            if (f.citythatownsit == null) {
                player.gold -= f.archetype.upkeepgold;
                if (player.gold < 0)
                {
                    player.gold = 0;
                    log.Printline("You can't afford to pay " + f.archetype.name + ".", Color.red);
                    log.Printline(f.archetype.name + " sadly has to leave.");
                    map.killoffamob(f);
                    continue; 
                }
                    }
            //if(f.dead_currently==true && f.hp < -4)
            //{
            //    //corpse has taken enough damage. we remove it. this stops corpses blocking doors
            //    //and means you can prevent zombie apocalypse by killing corpses

            //        f.tile = Etilesprite.EMPTY;
            //        map.passable[f.posx, f.posy] = true;
            //        map.itemgrid[f.posx, f.posy] = null;
            //}
            if (f.dead_currently == false && f.hp <= 0)
            {
                log.Printline("The " + f.archetype.name + " dies.", new Color(0.6f, 0, 0));
                if(f.tile==Etilesprite.UNITS_BARBARIAN_CHAMPION)player.score+=10;
                else if(f.tile==Etilesprite.UNITS_BARBARIAN_LORD)player.score+=50;
                f.speed = 0;
                f.dead_currently = true;

                f.hp = 0; //newly added. Even if you smack a mob for 10 damage when it has 1hp its corpse should take 5 damage to make disappear

                //  if (f.archetype.tile_dead != Etilesprite.EMPTY) //if the type of mob has a sprite for its dead state
                //  {
                //      f.tile = f.archetype.tile_dead; //mob's display tile = what it should be for dead
                //      map.itemgrid[f.posx, f.posy].tile = f.tile;//crash
                //  }
                //  else {

                map.killoffamob(f, true);

                //f.tile = Etilesprite.EMPTY;
                //map.passable[f.posx, f.posy] = true;
                //map.itemgrid[f.posx, f.posy] = null;

                //          }
                //if (map.itemgrid[f.posx, f.posy] == null)
                //   log.Printline("BUG: itemgrid null in mob death");
                // Debug.Log("error map thing in mob dies thing");

            }
        }
        //remove all really dead mobs. these are things like snow golem and undead mobs that go to nothing
        //int b4 = map.moblist.Count;

        map.moblist.RemoveAll(x => x.tile == Etilesprite.EMPTY);

        //if (b4 != map.moblist.Count) Debug.Log("removed somethnig");//was just checking it was actually working

        if (player.hp <= 0)
        {
            log.Printline("You have failed to protect civilization", Color.magenta);
            log.Printline("from the scourge of the barbarians.", Color.magenta);

            TimeEngine = CradleOfTime.dormant;
            gamestate = Egamestate.gameover;
            MyAudioSource.Stop();
            return;
        }
        player.turns++;


        TimeEngine = CradleOfTime.ready_to_process_turn;

    }
    void FloatingDamage(mob victim, mob attacker, int amount, string explanation = "", bool flashsupress = false)
    {
        Color c;
        //deal with super buffs
        if (victim != attacker)
        {
            if (attacker.hasattackup)
            {
                amount -= 100;
                if (!victim.dead_currently)
                    log.Printline("Aided by cairn power " + attacker.archetype.name + " strikes true!", Color.magenta);
            }
            if (victim.hasdefenseup)
            {
                if (amount < 0)
                {
                    amount = 0;
                    if (!victim.dead_currently)
                        log.Printline("The cairn power protects " + victim.archetype.name + "!", Color.magenta);
                }
            }

        }

        if (amount == 0) c = Color.grey;
        else c = (amount < 0) ? Color.red : Color.green;

        if (amount > 0 && victim.hp + amount > victim.archetype.hp) amount = victim.archetype.hp - victim.hp;

        FloatingTextItems.Add(new FloatingTextItem(explanation + " " + amount + " hp", victim.posx, victim.posy, c));

        //   if (attacker.tile == Etilesprite.ENEMY_GIANTBAT  && victim != attacker) FloatingDamage(attacker, attacker, -amount, "blood drain");        
        //  if ((attacker.tile == Etilesprite.ENEMY_NECROMANCER||attacker.tile==Etilesprite.ENEMY_LICH)  && victim != attacker) FloatingDamage(attacker, attacker, -amount, "drain life");

        if (attacker == player.mob && victim != player.mob)
        {
            if (!victim.dead_currently)
                log.Printline(attacker.archetype.name + " deals " + (-amount) + " to " + victim.archetype.name + " [" + explanation + "]", Color.green);
        }
        else
        {
            if (!victim.dead_currently)
            {
                log.Printline(victim.archetype.name, c);
                if (amount <= 0) log.Print(" takes ", c);
                else log.Print(" gains ", c);
                if (victim == attacker) log.Print(amount + " ", c);
                else log.Print(amount + " from " + attacker.archetype.name, c);
                if (explanation.Length > 0) log.Print("[" + explanation + "]", c);
            }
        }
        //actually do the damage
        victim.hp += amount;
        c.a = 0.5f;
        //if (map.displaychar[victim.posx, victim.posy] != Etilesprite.MAP_WATER && amount != 0)
        //    if (map.bloodgrid[victim.posx, victim.posy] == null)
        //    {
        //        if(victim.archetype.tile!=Etilesprite.ENEMY_ICE_GOLEM && victim.undead_currently==false)
        //        map.bloodgrid[victim.posx, victim.posy] = lil.randi(0, 3) + ((amount < -3) ? 4 : 0);
        //    }
        map.gridflashcolour[victim.posx, victim.posy] = c;
        map.gridflashtime[victim.posx, victim.posy] = Time.time + 0.5f;

        //if (attacker.tile == Etilesprite.ENEMY_MAGE && attacker!=victim)
        //{
        //    log.Printline(victim.archetype.name + " is slowed!");
        //    victim.speed = 0;
        //}

    }


    void FloatingDamagetoCity(Ccity victim, mob attacker, int amount, string explanation = "", bool flashsupress = false)
    {
        Color c;

        if (amount == 0) c = Color.grey;
        else c = (amount < 0) ? Color.red : Color.green;

        FloatingTextItems.Add(new FloatingTextItem(explanation + " " + amount + " hp", victim.posx, victim.posy, c));

        if (attacker == player.mob)
            log.Printline(attacker.archetype.name + " deals " + (-amount) + " to " + victim.name + " [" + explanation + "]", Color.green);

        else
        {
            log.Printline(victim.name, c);
            if (amount <= 0) log.Print(" takes ", c);
            else log.Print(" gains ", c);

            log.Print(amount + " from " + attacker.archetype.name, c);
            if (explanation.Length > 0) log.Print("[" + explanation + "]", c);
        }

        //actually do the damage
        victim.hp += amount;
        c.a = 0.5f;
        map.gridflashcolour[victim.posx, victim.posy] = c;
        map.gridflashtime[victim.posx, victim.posy] = Time.time + 0.5f;
    }





    void BresLineColour(int startx, int starty, int endx, int endy, bool includestart, bool includeend, Color c)
    {
        List<Cell> celllist = map.BresLine(startx, starty, endx, endy);
        for (int i = 0; i < celllist.Count; i++)
        {
            if (!includestart && startx == celllist[i].x && starty == celllist[i].y) continue;
            if (!includeend && endx == celllist[i].x && endy == celllist[i].y) continue;
            map.gridflashcolour[celllist[i].x, celllist[i].y] = c;
            map.gridflashtime[celllist[i].x, celllist[i].y] = Time.time + 0.5f;
        }
    }
    bool BresLineOfSight(int startx, int starty, int endx, int endy, bool includestart, bool includeend)
    {
        List<Cell> celllist = map.BresLine(startx, starty, endx, endy);
        for (int i = 0; i < celllist.Count; i++)
        {
            if (!includestart && startx == celllist[i].x && starty == celllist[i].y) continue;
            if (!includeend && endx == celllist[i].x && endy == celllist[i].y) continue;

            if (map.blocks_sight[celllist[i].x, celllist[i].y]) return false;
            var f = map.itemgrid[celllist[i].x, celllist[i].y];

            if (f != null && f.ismob && f.mob.dead_currently == false)
                return false;


        }
        return true;
    }


    void MobAttacksMob(mob attacker, mob target)
    {   
        //terrain mods for pango on pango combat only, not cities

        bool terrainATKbonus = (map.hill[attacker.posx,attacker.posy] && !map.hill[target.posx,target.posy]);
        bool terrainDEFbonus = (map.tree[target.posx, target.posy] && !map.tree[attacker.posx, target.posy]);

        int attackroll = lil.randi(attacker.archetype.attacklow, attacker.archetype.attackhigh);
        int defroll = (target.usedDEFthisturn) ? 0 : (target.archetype.defence + target.defencebonus);

        int t_atk = (attackroll + attacker.attackbonus) / 4;
        int t_def = defroll/4;

        if (t_atk == 0) t_atk = 1;
        if (t_def == 0) t_def = 1;

        if (!terrainATKbonus) t_atk = 0;
        if (!terrainDEFbonus) t_def = 0;
        
        log.Printline(attacker.archetype.name + " ATK " + attacker.archetype.attacklow + "-" + attacker.archetype.attackhigh +
            " +" + attacker.attackbonus + " rolls " + attackroll + " =" +(attackroll+attacker.attackbonus)+" + "+t_atk+" TRN",Color.grey);


      

        if (target.usedDEFthisturn) log.Printline(target.archetype.name + " DEF 0 (already used this turn)", Color.grey);
        else log.Printline(target.archetype.name + " DEF " + target.archetype.defence + " +" + target.defencebonus+" + "+t_def+" TRN");

       

        int damage = (attackroll + attacker.attackbonus) - defroll;
        if (damage < 1)  damage = 0;
        FloatingDamage(target, attacker, -damage, attacker.archetype.weaponname);
        target.usedDEFthisturn = true;

    }

    void MobAttacksCity(mob attacker, Ccity target)
    {
        int attackroll = lil.randi(attacker.archetype.attacklow, attacker.archetype.attackhigh);
        int defroll = (target.usedDEFthisturn) ? 0 : (target.defence);

        log.Printline(attacker.archetype.name + " ATK " + attacker.archetype.attacklow + "-" + attacker.archetype.attackhigh +
        " +" + attacker.attackbonus + " rolls " + attackroll + " =" + (attackroll + attacker.attackbonus), Color.grey);

        if (target.usedDEFthisturn) log.Printline(target.name + " DEF 0 (already used this turn)", Color.grey);
        else log.Printline(target.name + " DEF " + target.defence );

        int damage = attackroll + attacker.attackbonus - defroll;
        if (damage < 1) damage = 0;
        FloatingDamagetoCity(target, attacker, -damage, attacker.archetype.weaponname);
        target.usedDEFthisturn = true;
        
        
        //i think it's ok to actually kill the city at this point.
        if (target.hp <= 0)
        {
            log.Printline("The " + ((!target.isfrenzleecity) ? "barbarian ":"") + "city of " + target.name + " was destroyed", Color.red);
            log.Printline("by " + attacker.archetype.name,Color.red);

            if (!target.isfrenzleecity) player.score += 100;
            bool won = (map.buildings[target.posx, target.posy] == Etilesprite.BUILDINGS_BARBARIAN_CITADEL);
            map.CityKiller(target.posx, target.posy);
            if (won)
            {
                log.Printline("Peace has been brought to the land.", Color.yellow);
                NextLevel();
            }
        }

    }

    void MobGetsToAct(mob e)
    {

        e.numattacksleft = e.archetype.attacks;
        e.nummovesleft= e.archetype.moves;

        e.usedDEFthisturn = false;

        //new kludge for horsies
        e.haveyoumovedthisgo = false;

        haveanothergofrederick:

        if (e.AIformob.randomlywalking)//randomlywalking might as well be "bool haveatarget"
        {

            //do a quickscan for targets
            if (map.spiralscan(e))
            {
                //TARGET ACKWIERD. PRO-CEED. WE HAVE DIS-CO-VERED AN EN-EM-EH OF TEH DA-LECHS

                map.passable[e.posx, e.posy] = true;//we need square the mob starts on to be passable, for pathfinding.
                                                    //attempt to move 


                bool whattargetwas = map.passable[e.AIformob.targetsquare.x, e.AIformob.targetsquare.y];

                if (map.PathfindAStar(e.posx, e.posy, e.AIformob.targetsquare.x, e.AIformob.targetsquare.y, false))
                {
                    int deltax = map.firststepx - e.posx;
                    int deltay = map.firststepy - e.posy;
                    trytomove(e, deltax, deltay);
                    map.passable[e.posx, e.posy] = false;

                    map.passable[e.AIformob.targetsquare.x, e.AIformob.targetsquare.y] = whattargetwas;

                    //dodgy newbury
                   // if (e.haveyoumovedthisgo) { return; }
                  //  else { e.haveyoumovedthisgo = true; log.Printline("frederick"); goto haveanothergofrederick; }
                    //end dodgy newbury

                    return;
                }
            }


            if (lil.randi(1, 100) < 5)
            {
                e.AIformob.direction = lil.randi(0, 7); //randomly change direction for no reason
            }
            if (trytomove(e, lil.deltax[e.AIformob.direction], lil.deltay[e.AIformob.direction])) return;
            e.AIformob.direction = lil.randi(0, 7);//change direction because couldn't move

                    //dodgy newbury
                  //  if (e.haveyoumovedthisgo) { return; }
                  //  else { e.haveyoumovedthisgo = true; log.Printline("frederick"); goto haveanothergofrederick; }
                    //end dodgy newbury

            return;
        }

     

        //if (e.IsAdjacentTo(player.mob) &&
        //    e.hostile_toplayer_currently)
        //{
        //    MobAttacksMob(e, player.mob);
        //    return;
        //}


            
    }

   

}
