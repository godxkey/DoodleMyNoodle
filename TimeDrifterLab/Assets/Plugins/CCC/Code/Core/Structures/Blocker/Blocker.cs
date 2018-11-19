using System;
using System.Collections.Generic;

/// <summary>
/// Similar to a bool. The element evaluates to true by default. If one or more keys are inserted, the element evaluates to false.
/// </summary>
public class Blocker
{
    public event Action<bool> OnLockStateChange;
    private List<object> keys = new List<object>();

    public Blocker() { }

    /// <summary>
    /// Removes the first key equal to that instance
    /// </summary>
    public bool Unblock(object key)
    {
        bool wasUnlocked = Evaluate();

        bool result = keys.Remove(key);

        if (Evaluate() && !wasUnlocked && OnLockStateChange != null)
            OnLockStateChange(true);

        return result;
    }

    /// <summary>
    /// Check if the key exists
    /// </summary>
    public bool HasKey(object key)
    {
        for (int i = 0; i < keys.Count; i++)
        {
            if (keys[i] == key)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Remove all the keys equal to that instance
    /// </summary>
    public int UnblockAll(object key)
    {
        bool wasUnlocked = Evaluate();

        int result = keys.RemoveAll(x => x.Equals(key));

        if (Evaluate() && !wasUnlocked && OnLockStateChange != null)
            OnLockStateChange(true);

        return result;
    }

    /// <summary>
    /// Add a key
    /// </summary>
    public void Block(object key)
    {
        bool wasUnlocked = Evaluate();

        keys.Add(key);

        if (!Evaluate() && wasUnlocked && OnLockStateChange != null)
            OnLockStateChange(false);
    }

    /// <summary>
    /// Add a key if an equal instance isn't already inserted. Return true if a new insert took place
    /// </summary>
    public bool BlockUnique(object key)
    {
        bool wasUnlocked = Evaluate();

        if (!keys.Contains(key))
        {
            keys.Add(key);

            if (!Evaluate() && wasUnlocked && OnLockStateChange != null)
                OnLockStateChange(false);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Returns true if there are no keys
    /// </summary>
    public bool Evaluate()
    {
        return keys.Count == 0;
    }

    public override string ToString()
    {
        return Evaluate().ToString();
    }

    public static implicit operator bool(Blocker locker)
    {
        return locker.Evaluate();
    }
    public static implicit operator string(Blocker locker)
    {
        return locker.ToString();
    }
}