using UnityEngine;
using System.Collections;

public static class Speed {

    public const int min = 0;
    public const int max = 12;
    public const int samecourse = +1;
    public const int turn45 = -3;
    public const int turn90 = -6;
    public const int turn135 = -9;
    public const int turn180 = -Speed.max;
    public const int coasting = -1;
    public const int nonice = -6;

    public static int[] deltasbyrelativeheading = { samecourse, turn45, turn90, turn135, turn180, turn135, turn90, turn45 };
    public static int[] ROTATIONdeltasbyrelativeheading = { 0, 1, 1, 2, 0, -2, -1, -1 };

    public static void SpeedAndDirectionChange(mob m,int pressed)
    {
        //in comes pressed, 0-7 for direction pressed 0 is n round to 7 is nw
        int reldir = (pressed - m.facing);
        if (reldir < 0) reldir += 8;
        //reldir is now 0-7 but is the difference, rotation-wise between the dir you were going and what you pressed
        //eg if you were going east and you pressed south it's 2
      //  int relcode = lil.rot_lookup[reldir]; //0 for straight ahead, 4 for opposite, 123 or -123 for turns

        //new thing to try to fix bug 
        
        if(m.speed==0 && reldir == 4)
        {
            m.facing += 4;
            if (m.facing > 7) m.facing -= 8;
            goto youmaycontinue;
        }
        
        //end of new thing


        m.speed += deltasbyrelativeheading[reldir];
        if (m.speed < min) m.speed = min;
        else if (m.speed > max) m.speed = max;

      //  int oldfacing = m.facing;

        m.facing += ROTATIONdeltasbyrelativeheading[reldir];
        if (m.facing < 0) m.facing += 8;
        else if (m.facing > 7) m.facing -= 8;

        youmaycontinue:

        if (m.facing > 0 && m.facing < 4) m.reversesprite = false;
        else if (m.facing > 4) m.reversesprite = true;

    }
    public static void change(mob x,int amount)
    {
        x.speed += amount;
        if (x.speed < min) x.speed = min;
        else if (x.speed > max) x.speed = max;
        
    }
    public static int findrel(int x,int y,int x2,int y2)
    {
        int deltax = x2-x;int deltay = y2-y;
        if (deltax == -1)
        {
            if (deltay == -1) return 7;
            else if (deltay == 0) return 6;
            else if (deltay == 1) return 5;
        }
        else if (deltax == 0)
        {
             if (deltay == -1) return 0;
            //else if (deltay == 0) return 6; that would be the square itself, no direction
            else if (deltay == 1) return 4;
        }
        if (deltax == 1)
        {
            if (deltay == -1) return 1;
            else if (deltay == 0) return 2;
            else if (deltay == 1) return 3;
        }
        Debug.Log("SHOULD NEVER BE HERE");
        return 9999999;
    }
}
