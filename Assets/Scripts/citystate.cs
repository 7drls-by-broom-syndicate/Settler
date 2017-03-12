using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Citystate  {

    public static List<string> citystatenames = new List<string>
    {
        "Bremen","Hamburg","Lubeck","Franfurt","Geneva","Krakow","Fiume","Danzig","Shanghai","Tangier","Memel",
        "Trieste","Jerusalem","Monaco","Singapore","Vatican City","Hong Kong","Gibraltar","Guanabara","Macau","Ceuta","Melilla"
    };

    public string name;
    public static int numcitystates=-1;

    public int desiredresource;

    public bool metyet = false;

    public Citystate(int x,int y)
    {
        numcitystates++; 
        name = citystatenames[numcitystates];
        desiredresource = lil.randi(0, 13);
    }



}
