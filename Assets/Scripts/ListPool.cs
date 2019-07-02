using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListPool<T> {

    static Stack<List<T>> stack = new Stack<List<T>> ();

    public static List<T> Get () {
        if (stack.Count > 0) {
            return stack.Pop ();
        }
        return new List<T> ();
    }

    public static void Add (List<T> list) {
        list.Clear ();
        stack.Push (list);
    }

}