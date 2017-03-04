using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu  {

    public enum Emenuidentity { graphics, wallabyselection, test, foreskin_shop }

    public List<byte[]> optionstrings;
    public int currently_selected_option;
    public int number_of_options;

    public Rect r, r2;//rects for background tinted window

    public Color colBackground = new Color(0, 0, 0, 0.6f);
    public Color colText = Color.white;
    public Color colTitle = Color.yellow;
    public byte[] title;

    public Emenuidentity type;

    int bgwidth, bgheight;
    public int titlepos;
    public int locx, locy;//pos in the 640x360 space
    public Menu(Emenuidentity _type,string t,List<string> s)
    {

        type = _type;

        title = System.Text.Encoding.ASCII.GetBytes(t);

        number_of_options = 0;

        int longeststring = t.Length;//if title is longer than longest option, we want that for width

        optionstrings = new List<byte[]>();

        foreach (string x in s)
        {
          
            number_of_options++;
            if (x.Length > longeststring) longeststring = x.Length;//keep tabs on longest string so we can size menubox
            optionstrings.Add(System.Text.Encoding.ASCII.GetBytes(x));
        }

        bgwidth = longeststring+3;//1 extra for each side and 1 for the "selector lane"
        bgheight = number_of_options+3;//1 extra top and bottom plus 1 for title

        

        byte c = 219; //full block
        int xpos = c % 32;
        int ypos = 7 - (c / 32);
        
        const float xratio = 1f / 32f;
        const float yratio = 1f / 8f;
        
        r2.x = xratio * xpos;
        r2.y = yratio * ypos;
        r2.width = xratio;
        r2.height = yratio;
    
        r.x = 0;//pos on screen of bg window
        r.y = 0;//pos on screen of bg window
        r.width = 6 * Game.zoomfactor * bgwidth;
        r.height = 12 * Game.zoomfactor*bgheight;

    }

    public void setpos(int x,int y)
    {
        locx = x;locy = y;
        r.x = x*Game.zoomfactor;
        r.y = y*Game.zoomfactor;

        titlepos = locx + ((bgwidth - title.Length) * 6) / 2;
    }
}
