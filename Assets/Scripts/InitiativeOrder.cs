/*
 *   This class holds the game's initiative order, with the following rules:
 *   
 *   - New GameObjects added to the list cannot replace an existing object. If trying to add an object at index X, and there is already an object at X,
 *     then attempt to insert at X+1. Repeat until a "vacant" index is found.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InitiativeOrder {
    private SortedList<int, GameObject> _order = new SortedList<int, GameObject>();
    private int minimumIndex = -1;  // minimum index currently in use, -1 when order is empty

    public class InitiativeEvent : UnityEvent<SortedList<int, GameObject>> { }
    public InitiativeEvent OnInitiativeChanged;

    public int MinimumIndex { get => minimumIndex; private set => minimumIndex = value; }
    public int Count { get => _order.Count; }

    public InitiativeOrder() { 
        if (OnInitiativeChanged == null) { OnInitiativeChanged = new InitiativeEvent(); }
        OnInitiativeChanged.AddListener(UpdateMinimumIndex);
    }

    /// <summary>
    /// Attempts to insert an object into the initiative order at the specified index. If that index is already
    /// "occupied", increments the index until a "vacant" one is found.
    /// </summary>
    /// <param name="index">The index to insert the object into.</param>
    /// <param name="obj">The GameObject to insert.</param>
    /// <param name="floor">The minimum index for inserting an object (while not insert if index is less than floor).</param>
    /// <returns>The index the object was actually inserted at, or -1 if a null object was passed to obj or index is below the floor value.</returns>
    public int InsertAt(int index, GameObject obj, int floor=0) {
        if (obj == null || index < floor) {
            return -1;
        }

        while (_order.ContainsKey(index)) {
            index++;
        }

        _order.Add(index, obj);
        OnInitiativeChanged?.Invoke(_order);
        return index;
    }

    /// <summary>
    /// Removes and returns the GameObject from the specified index. If that index is "unoccupied", returns null.
    /// </summary>
    /// <param name="index">The index to remove from the initiative order.</param>
    /// <returns>The GameObject at the specified index, or null if there was nothing there.</returns>
    public GameObject RemoveAt(int index) {
        if (ContainsIndex(index)) {
            GameObject obj = _order[index];
            _order.Remove(index);
            OnInitiativeChanged?.Invoke(_order);
            return obj;
        } else {
            return null;
        }
    }

    /// <summary>
    /// Determines whether a GameObject exists at the specified index.
    /// </summary>
    /// <param name="index">The index to check.</param>
    /// <returns>True if there is something at that index (potentially null), False otherwise.</returns>
    public bool ContainsIndex(int index) {
        return _order.ContainsKey(index);
    }

    private void UpdateMinimumIndex(SortedList<int, GameObject> ord) {
        if (_order.Count == 0) {
            MinimumIndex = -1;
            return;
        }

        MinimumIndex = _order.Keys[0];
    }

    /// <summary>
    /// Remove all GameObjects in the initiative order that appear before the specified time.
    /// </summary>
    /// <param name="newFloor">The new minimum time to apply to the initiative order.</param>
    public void Purge(int newFloor) {
        List<int> removeList = new List<int>();
        foreach (int index in _order.Keys) {
            if (index < newFloor) {
                removeList.Add(index);
            }
        }

        if (removeList.Count > 0) {
            foreach (int index in removeList) {
                // not using InitiativeOrder.RemoveAt() method, as want to remove everything before invoking the event
                _order.Remove(index);   
            }
            OnInitiativeChanged?.Invoke(_order);
        }

    }

}
