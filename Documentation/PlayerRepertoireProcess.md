# Player Repertoire Sync Process

Here is the replication flow of players in a game:

![](./Images/PlayerRepertoireSynchronization.PNG)

## Classes involved (pseudo-code)

*Data structures*

```c++
class PlayerId
{
    int id;
}

class PlayerInfo
{
    PlayerId id;
    string	 name;
    
    // extras
    bool isServer;
    int ping;
    etc.
}


class PlayerRepertoire
{
    list<PlayerInfo> players;
}
```

*Net Messages*

```c#
// sent by client
class NetMessageClientHello : NetMessage
{
    string playerName;
}

// sent by server
class NetMessagePlayerIdAssignment : NetMessage
{
    PlayerId id;
}

// sent by server
class NetMessagePlayerRepertoireSync : NetMessage
{
    list<PlayerInfo> players;
}

// sent by server
class NetMessagePlayerJoined : NetMessage
{
    PlayerInfo player;
}

// sent by server
class NetMessagePlayerLeft : NetMessage
{
    PlayerId playerId;
}
```
