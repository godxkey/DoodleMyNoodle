
# Game Console

Pour ouvrir la GameConsole, appuyer sur F1. Vous y verrez:
- les logs Unity 
- les "commandes"
- les "vars"

### Commande

Les **commandes** peuvent être executer en entrant leur nom dans la console.

Vous pouvez entrer **help** afin de voir une liste des commandes disponibles.

### ConfigVar
Les **vars** (ou ConfigVar dans le code) sont des variable statique qui peuvent être lue et modifié.

Lire: Simplement entrer le nom de la variable dans la console. 
Écrire: Entrez le nom de la variable suivie de la valeur. Ex: ``playerspeed 300``.

Les ConfigVars supporte les type suivant:
- int, e.g: "30"
- float, e.g: "15.12"
- string, e.g: "hello"
- bool, e.g: "true" ou "false"

Vous pouvez entrer **vars** afin de voir une liste des ConfigVars disponibles.

## Créer une commande

```csharp
GameConsole.AddCommand("macommande", Cmd_MaCommande, "La description de la commande");

// ...

GameConsole.RemoveCommand("macommande");

voir Cmd_MaCommande(string[] args)
{
    //...
}
```


## Créer une ConfigVar
```csharp
[ConfigVar(name: "myfeature.variablename", defaultValue: "0", description: "The variable description")]
static ConfigVar s_myConfigVar;
```
That's it!