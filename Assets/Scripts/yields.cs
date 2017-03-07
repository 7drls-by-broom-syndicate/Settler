﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class yields  {
    public int production = 0,gold = 0,food = 0;    

    public yields(int p,int g,int f)
    {
        production = p;
        gold = g;
        food = f;
    }

    public yields()
    {
        production = 0;
        gold = 0;
        food = 0;
    }

    public void add(int p,int g,int f)
    {
        production += p;
        gold += g;
        food += f;
    }


    public void set(int p,int g,int f)
    {
         production = p;
        gold = g;
        food = f;
    }

	public static yields operator +(yields a,yields b)
    {
        yields temp = new yields(a.production, a.gold,a.food);
        temp.production += b.production;
        temp.gold += b.gold;
        temp.food += b.food;

        return temp;
    }
}