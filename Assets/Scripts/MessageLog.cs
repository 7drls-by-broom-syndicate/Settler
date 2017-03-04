using UnityEngine;
using System.Collections;

public class MessageLog {
    int width, height;
    
    public byte[,] screenmap;
    public Color[,] screenfg;
    private Color currentfg = Color.gray;

    private int cursorx,cursory;

    private void blankline(int y) {
        for (int f = 0; f < width; f++)
            screenmap[f, y] = 32;
    }

    public void Printline(string s, Color? c=null) {
        if (cursorx != 0) {

            cursorx = 0;
            cursory++;
            if (cursory == height) cursory = 0;
            blankline(cursory);
        }

        Print(s, c);
        
    }
    public int GetCurrentLine() {
        return (this.cursory == height-1) ? 0 : this.cursory + 1;
    }

    public void Print(string s, Color? c=null) {
        byte[] b = System.Text.Encoding.ASCII.GetBytes(s);
        c = c ?? currentfg;
        foreach (var f in b) {
            screenmap[cursorx, cursory] = f;
            screenfg[cursorx,cursory]=(Color)c;
            cursorx++;
            if (cursorx == width) {
                cursorx = 0; cursory++;
                if (cursory == height) cursory = 0;
                blankline(cursory);
            }
        }
    }

    public void Clear() {
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                screenmap[x, y] = 32;
                screenfg[x, y] = this.currentfg;
            }
        }
    }

    public MessageLog(int w, int h) {
        this.width = w; this.height = h;
        cursorx=0;cursory=height-1;

        screenmap = new byte[width, height];
        screenfg = new Color[width, height];
        Clear();
    }
}
