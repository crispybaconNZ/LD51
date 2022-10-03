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
    private SortedList<int, IAbility> _order = new SortedList<int, IAbility>();
    private int minimumIndex = -1;  // minimum index currently in use, -1 when order is empty

    public class InitiativeEvent : UnityEvent<SortedList<int, IAbility>> { }
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
    /// <param name="floor">The minimum index for inserting an object (will not insert if index is less than floor).</param>
    /// <returns>The index the object was actually inserted at, or -1 if a null object was passed to obj or index is below the floor value.</returns>
    public int InsertAt(int index, IAbility obj, int floor=0) {
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
    public IAbility RemoveAt(int index) {
        if (ContainsIndex(index)) {
            IAbility obj = _order[index];
            _order.Remove(index);
            OnInitiativeChanged?.Invoke(_order);
            return obj;
        } else {
            return null;
        }
    }

    public void Remove(IAbility obj) {
        if (_order.ContainsValue(obj)) {
            int key = _order.IndexOfValue(obj);
            _order.Remove(key);
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

    private void UpdateMinimumIndex(SortedList<int, IAbility> ord) {
        if (_order.Count == 0) {
            MinimumIndex = -1;
            return;
        }

        MinimumIndex = _order.Keys[0];
    }

    public int GetNextIndex(int startIndex = 0) {
        // starting at the specified startIndex, find the *next* index
        int index = startIndex + 1;
        while (!_order.ContainsKey(index) && index < _order.Keys[_order.Keys.Count - 1]) {
            index++;
        }
        // Debug.Log($"Found next at index {index}: {_order[index]}");
        return index;
    }

    public void Reset() {
        _order.Clear();
        UpdateMinimumIndex(_order);
    }
}
