using UnityEngine;
using System.Collections;

public class Array2D<T>  {
    int floodcount = 0;
    T[,] internalarray;

    public int width, height;

    public Array2D() {
        width = -1; height = -1;
    }

    public Array2D(int w,int h,T initialvalue){
        width = w; height = h;
        internalarray = new T[w,h];
        internalarray.Fill(initialvalue);
    }

    public Array2D(int w, int h) {//change fn name to initnovalue if it complains
        width = w; height = h;
        internalarray = new T[w, h];
    }

    //example of how to do indexers in c#
    //so we can have pistash=customthing[x,y]; or customthing[x,y]=pistash;
    public T this[int x, int y] {
        get {
            return internalarray[x, y];
        }
        set {
            internalarray[x, y] = value;
        }
    }

    public T AtGet(int x, int y) {
        return internalarray[x, y];
    }

    public void AtSet(int x, int y, T value) {
        internalarray[x, y] = value;
    }

    public void Fill(T value){
        internalarray.Fill(value);
    }

    public void FloodFill(int x, int y, T space, T safetemp) {
        if (x < 0 || y < 0 || x >= width || y >= height) return;// false;
        if (!this.AtGet(x, y).Equals(safetemp)) return;// false;
        this.AtSet(x, y,space);
        floodcount++;
        FloodFill(x - 1, y, space, safetemp);
        FloodFill(x + 1, y, space, safetemp);
        FloodFill(x, y - 1, space, safetemp);
        FloodFill(x, y + 1, space, safetemp);
    }

    //ret: if all connected. space=flood into this, safetemp: temp value guaranteed not in grid
    public bool FloodTest(T space, T safetemp) {
        int cellcount = 0;
        bool virgin = true;
        int xt = 0, yt = 0;
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (this.AtGet(x, y).Equals(space)) {
                    cellcount++;
                    this.AtSet(x, y, safetemp);
                    if (virgin) {
                        virgin = false;
                        xt = x;
                        yt = y;
                    }
                }
            }
        }
        if (cellcount == 0) return false;
        floodcount = 0;
        FloodFill(xt, yt, space, safetemp);
        return (floodcount < cellcount) ? false : true;
    }

}
