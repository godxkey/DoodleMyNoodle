# Saves

Goal is we want to easily save anything. Since accessing a file can be complicated (needs async), we need an intermediate between gameplay 
code for writing/reading data process. That's where the Saves come into play. This document will explain how to make it work and how to use it. 

## Setup

### 1. Implement Save Service

Using the existing Service system, add the "SaveService.cs" as a new service to the bank 

![(Here is the link to the file)](https://gitlab.com/PlaymindIP/playlib/blob/master/Saves/SaveService.cs) 

This will enable the whole process of saving data into files for your game. If you don't do that first step, nothing will work. 

### 2. Create scriptable objects

For every type of things you want to save, you'll need a specific scriptable objects. Each one of them will save in a file created
just for them. When we'll want to save or load anything, we just need the reference to the scriptable objects containing what we need. 
We went with this approach for simplicity. 

Here's what you can customize inside the scriptable object :


### 3. Link it to your script

Inside any script, make yourself a public DataSaver reference.

``` C#
public DataSaver playerStatsSaver;
```

This will enable your code to have access to the scriptable object functionnalities. It's inside it that you'll be able to save and load.

We strongly suggest you to create an essily accessible Key for the data type and another one to identify the data itself. Since saved data
are associated with a string, making static keys make it really easy to save and load multiple data. If you're familiar with playerprefs, the workflow is similar.

## Using it

``` C#

// This could be added in any class

// Our keys accessible anywhere
public const string PLAYERSTATKEY = "Playerstatkey_";
public const string STAT_HEALTH = "health";
public const string STAT_MANA = "mana";

// Our Game Data
public float playerHealth;
public float playerMana;

// Let's Load as soon as the game starts
void Awake()
{
    LoadEverything();
}

// Before we leave the game, let's save our data
public void LeaveGame()
{
    SaveEverything();
}

// Save Function Example
public void SaveEverything()
{
    playerStatsSaver.SetFloat(PLAYERSTATKEY+STAT_HEALTH,playerHealth);
    playerStatsSaver.SetFloat(PLAYERSTATKEY+STAT_MANA,playerMana);
    playerStatsSaver.Save();
}

// Loading Function Example
public void LoadEverything()
{
    playerHealth = playerStatsSaver.GetFloat(PLAYERSTATKEY+STAT_HEALTH);
    playerMana = playerStatsSaver.Save(PLAYERSTATKEY+STAT_MANA);
}
```


### (Advanced) Saving a Data Structure

TODO

