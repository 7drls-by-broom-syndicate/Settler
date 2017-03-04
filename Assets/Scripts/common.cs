using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;


//this is nowhere near being a generic multimap but it has nods to being generic such as letting you specify the types
public class Multimap<K, V> {
    public int nodecount = 0;
    SortedDictionary<K, Queue<V>> dic=new SortedDictionary<K,Queue<V>>(); //is there a way to say this type name = new whatever it is?
    
    public bool Empty() { return nodecount == 0; }
    public void Insert(K key, V value) {
        if (!dic.ContainsKey(key)) dic[key] = new Queue<V>();
        dic[key].Enqueue(value);
        nodecount++;
    }
    //returns first element and removes it
    public V Begin() {
        if (nodecount == 0) throw new SystemException("BROOM WARNZ JOO! Attempt to get first element of an empty multimap.");//LEARN MOAR ABOUT EXCEPTIONS
        if (dic.Count == 0) throw new SystemException("BROOM WARNZ JOO! Attempt to get first element of an empty multimap.");//LEARN MOAR ABOUT EXCEPTIONS
        var kvp = dic.First();//kvp is a keyvaluepair of int to queue of v
        V val = kvp.Value.Dequeue(); nodecount--;
        if (kvp.Value.Count == 0) dic.Remove(kvp.Key);
        return val;
    }
}

public class Pair<T, U> {
    public Pair() {
    }

    public Pair(T first, U second) {
        this.First = first;
        this.Second = second;
    }

    public T First { get; set; }
    public U Second { get; set; }
}

//pango's first extension method
public static class Extendythings {
  
    //could put these directly into the array2d class but i'm leaving them
    //like this as an example of coding exttension methods

    
    public static T OneFromTheTop<T>(this List<T> v) {
        if (v.Count == 0) {
            Debug.Log("fatal error: attempt to onefromthetop empty list");
            return v[0]; //won't work of course but prog won't compile otherwise
        } else {
            T tempers = v[v.Count - 1];
           // T tempers = v[0];
            //v.RemoveAt(0);
            v.RemoveAt(v.Count - 1);
            return tempers;
        }
    }

    //quicker to call v.add than this fn but blah
    public static void StickOnTheBottom<T>(this List<T> v, T data) {
        v.Add(data);
    }

    public static List<T> Shuffle<T>(this List<T> list) {
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = lil.randi(0, n );//removed +1, because rand changed
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }

    public static T[,] Fill<T>(this T[,] thearray, T thevalue) {
        for (var f = 0; f < thearray.GetLength(0); f++) {
            for (var g = 0; g < thearray.GetLength(1); g++) {
                thearray[f, g] = thevalue;
            }
        }
        return thearray;
    }
    public static T[,] FillInstance<T>(this T[,] thearray) {
        for (var f = 0; f < thearray.GetLength(0); f++) {
            for (var g = 0; g < thearray.GetLength(1); g++) {
                thearray[f, g] = (T)Activator.CreateInstance(typeof(T));
            }
        }
        return thearray;
    }

}