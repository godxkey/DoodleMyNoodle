using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLocker", menuName = "CCC/Structures/Blocker")]
public class BlockerSO : ScriptableObject
{
    private Blocker blocker = new Blocker();

    void OnEnable()
    {
        blocker = new Blocker();
    }

    void OnDisable()
    {
        blocker = new Blocker();
    }

    public event Action<bool> OnBlockStateChange
    {
        add { blocker.OnLockStateChange += value; }
        remove { blocker.OnLockStateChange -= value; }
    }
    /// <summary>
    /// Removes the first key equal to that instance
    /// </summary>
    public bool Unblock(object key) { return blocker.Unblock(key); }
    /// <summary>
    /// Check if the key exists
    /// </summary>
    public bool HasKey(object key) { return blocker.HasKey(key); }
    /// <summary>
    /// Remove all the keys equal to that instance
    /// </summary>
    public int UnblockAll(object key) { return blocker.UnblockAll(key); }
    /// <summary>
    /// Add a key
    /// </summary>
    public void Block(object key) { blocker.Block(key); }
    /// <summary>
    /// Add a key if an equal instance isn't already inserted
    /// </summary>
    public bool BlockUnique(object key) { return blocker.BlockUnique(key); }
    /// <summary>
    /// Returns true if there are no keys
    /// </summary>
    public bool Evaluate() { return blocker.Evaluate(); }
    public override string ToString() { return blocker.ToString(); }
    public static implicit operator bool(BlockerSO locker) { return locker; }
    public static implicit operator string(BlockerSO locker) { return locker; }
}
