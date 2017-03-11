using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI
{
    //by calling it AI maybe it will be cooler and deeper and more betterer (sic)

    public bool randomlywalking;
    public int direction;

    public bool targetismob;
    public mob target;
    public Cell targetsquare;

    public AI()
    {
        randomlywalking = true;
        int direction = lil.randi(0, 7);


    }
}
