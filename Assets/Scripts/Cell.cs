using UnityEngine;
using System.Collections;

//represents a cell on a map
public class Cell {

    public Cell() {
    }

    public Cell(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public int x { get; set; }
    public int y { get; set; }
}

