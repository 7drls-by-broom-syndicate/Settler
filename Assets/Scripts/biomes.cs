using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Ebiometype { polar, tundra, taiga, alpine, mediterranean, prairie, temperate_forest, desert, savanna, tropical_rainforest }

public class Biomes  {

    public static Ebiometype[,] typearray =
    {
        {Ebiometype.polar,          Ebiometype.polar,           Ebiometype.polar,       Ebiometype.polar,   Ebiometype.polar,               Ebiometype.polar },
        {Ebiometype.tundra,         Ebiometype.tundra,          Ebiometype.tundra,      Ebiometype.tundra,  Ebiometype.tundra,              Ebiometype.tundra },
        {Ebiometype.taiga,          Ebiometype.taiga,           Ebiometype.taiga,       Ebiometype.taiga,   Ebiometype.taiga,               Ebiometype.taiga },
        {Ebiometype.alpine,         Ebiometype.alpine,          Ebiometype.alpine,      Ebiometype.alpine,  Ebiometype.alpine,              Ebiometype.alpine },
        {Ebiometype.mediterranean,  Ebiometype.mediterranean,   Ebiometype.prairie,     Ebiometype.prairie, Ebiometype.temperate_forest,    Ebiometype.temperate_forest},
        {Ebiometype.mediterranean,  Ebiometype.mediterranean,   Ebiometype.prairie,     Ebiometype.prairie, Ebiometype.temperate_forest,    Ebiometype.temperate_forest},
        {Ebiometype.desert,         Ebiometype.desert,          Ebiometype.savanna,     Ebiometype.savanna, Ebiometype.tropical_rainforest, Ebiometype.tropical_rainforest },
        {Ebiometype.desert,         Ebiometype.desert,          Ebiometype.savanna,     Ebiometype.savanna, Ebiometype.tropical_rainforest, Ebiometype.tropical_rainforest }

    };

    public static Ebiometype classify(float temperature, float moisture)
    {
        temperature = Mathf.Clamp(temperature, -1.0f, 1.0f);
        moisture = Mathf.Clamp(moisture, -1.0f, 1.0f);

        int t = (int)((temperature + 1.0f)*4.0f);
        int m = (int)((moisture + 1.0f) * 3.0f);
        //Debug.Log("biome " + t + " " + m);
        t = Mathf.Clamp(t, 0, 7);
        m = Mathf.Clamp(m,0, 5);

        
        return typearray[t,m];
    }


}
