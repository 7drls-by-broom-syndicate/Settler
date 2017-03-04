using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//"lil helpers"

enum direction { UP,DOWN,RIGHT,LEFT}

public static class lil {
    //rotational style
    public static int[] rot_deltax = {  0, 1, 1, 1, 0, -1, -1, -1 };
    public static int[] rot_deltay = {  -1, -1, 0, 1, 1, 1, 0, -1 };
    //public string[] dir = { "-none-", "N", "NE", "E", "SE", "S", "SW", "W", "NW" };
    public static int[] rot_lookup = { 0, 1, 2, 3, 4, -3, -2, -1 };

    //up down right left
    public  static sbyte[] deltax = { 0, 0, 1, -1,    -1,1,-1,1 };
	public  static sbyte[] deltay = { -1, 1, 0, 0,     -1,-1,1,1 };
    
    public static sbyte[] opdir = { 1, 0, 3, 2 };
    //public static char[] dirchar =		{ '^', 'v', '>', '<' };
	//public static char[] dirchar_rev=	{ 'v', '^', '<', '>' };
    
   // public static Etilesprite[] dirchar = { Etilesprite.TORCHUP, Etilesprite.TORCHDOWN, Etilesprite.TORCHRIGHT, Etilesprite.TORCHLEFT };
   // public static Etilesprite[] dirchar_rev=	{ Etilesprite.TORCHDOWN, Etilesprite.TORCHUP,Etilesprite.TORCHLEFT, Etilesprite.TORCHRIGHT };
    
    public static UnityEngine.Color rgb_unitycolour(int r,int g,int b)
    {
        return new UnityEngine.Color((float)r / 255.0f, (float)g / 255.0f, (float)b / 255.0f);
        
    }

    public static int percentof(int self, int x){
		return (int) ((((float) x / 100.0f)*(float) self) + 0.5f);
	}
    public static float randf(float low, float high) {
        return UnityEngine.Random.Range(low, high);
    }
    public static int randi(int low, int high) {
        return UnityEngine.Random.Range(low, high+1);
    }
    public static bool coinflip()
    {
        return (randi(0, 1000) < 500);
    }
    public static byte randb(byte low, byte high) {
        return (byte)UnityEngine.Random.Range(low, high+1);
    }
    public static void seed(int s) {
        UnityEngine.Random.seed = s;
    }
    public static void seednow()
    {
        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        int cur_time = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;
        UnityEngine.Random.seed = cur_time;
    }

    public static T randmember<T>(this List<T> l) {
        return l[randi(0, l.Count-1)];
    }
    
    //public static T randmember<T>(this Array<T> a)
    //{
     //   return a[randi(0, a.length)];
    //}


    public static Color colouradd(Color a, Color b) {
        float c1 = a.r + b.r;
        float c2 = a.g + b.g;
        float c3 = a.b + b.b;
        if (c1 < 0.1f) c1 = 0.1f; if (c2 < 0.1f) c2 = 0.1f; if (c3 < 0.1f) c3 = 0.1f;
        if (c1 > 1.0f) c1 = 1.0f; if (c2 > 1.0f) c2 = 1.0f; if (c3 > 1.0f) c3 = 1.0f;
        return new Color(c1, c2, c3);
    }
    public static float totalcolour(Color c) {
        return c.r + c.g + c.b;
    }

    public static Color colourclamp(Color c) {
        if (c.r < 0.1) c.r = 0.1f; if (c.g < 0.1) c.g = 0.1f; if (c.b < 0.1) c.b= 0.1f;
        if (c.r > 1.0) c.r = 1.0f; if (c.g > 1.0) c.g = 1.0f; if (c.b > 1.0) c.r = 1.0f;
        return c;
    }
    //public static clamp(int x,int low,int high){
     //   Color.black.
    //}

}
