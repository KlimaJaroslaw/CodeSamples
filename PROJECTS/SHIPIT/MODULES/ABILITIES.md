[:arrow_up: ShipIt (Project)](/PROJECTS/SHIPIT/SHIPIT.md)

# Abilities & Linear Modifiers
Implementation of character ability system.

## SOURCE CODE FILES
:link: [Ability.cs](/PROJECTS/SHIPIT/SOURCE/Ability.cs)\
:link: [AbilityActionArgs.cs](/PROJECTS/SHIPIT/SOURCE/AbilityActionArgs.cs)\
:link: [AbilityData.cs](/PROJECTS/SHIPIT/SOURCE/AbilityData.cs)\
:link: [AbilityMate.cs](/PROJECTS/SHIPIT/SOURCE/AbilityMate.cs)\
:link: [IAbilityUser.cs](/PROJECTS/SHIPIT/SOURCE/IAbilityUser.cs)\
:link: [LinearModifier.cs](/PROJECTS/SHIPIT/SOURCE/LinearModifier.cs)

# How system works
Ability system is divided into 5 classes:
- **AbilityData** – *ScriptableObject* class that only holds essential data regarding ability
- **Ability** – Class used by **AbilityMate** and across system. It contains **AbilityData** object among other properties
- **AbilityMate** – Manager class and heart of all operations on **Ability** objects. It is also responsible for communication with other system modules.
- **AbilityActionArgs** – Hold data about what action should **AbilityMate** perform (example: unlock ability “x”, replace ability “y” with ability “z”)
- **IAbilityUser** – **AbilityMate** notifies **IAbiliyUser** when there is change in active abilities.
 
# Ability Data
``` csharp
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityData", menuName = "AbilitySystems/AbilityData")]
public class AbilityData : ScriptableObject
{    
    public List<AssetIdentifier> requirements;

    //Passive section
    public List<LinearModifier> passiveModifiers;
    public List<Action> acquireActions;

    //Active section
    public float cooldown;
    public bool active;
    public List<Action> activeActions;
}
```
###### Code @ AbilityData.cs
**AbilityData** class has following fields:
- **requirements**: this is list of all ids of abilities that needs to be acquired before unlocking this ability (see: [AssetIdentifer](/PROJECTS/SHIPIT/MODULES/ASSETACCESS.md))
- **passiveModifiers**: list of value modifications (see **LinearModifiers** below)
- **acquireActions**: list of actions that should be performed by *ActionOperator* (Other system module) upon acquiring this ability
- **cooldown**: cooldown in seconds 
- **active**: boolean field, tells if ability is either passive or active
- **activeActions**: list of action on activation

In my systems abilities tweak values of different parameters of character (example: movement speed, firing range, etc.). To hold data on how much certain value should be modified I implemented **LinearModifier** class:
```csharp
using System.Collections.Generic;

public class LinearModifier
{
    #region Properties & Variables
    public float Value { get=>value_; set { value_ = value; } }
    public float Multiplier { get=>multiplier; set {multiplier = value;}  }
    public AssetIdentifier Id { get=>id; }
    public List<AssetIdentifier> Sources { get=>sources; }

    private float value_;
    private float multiplier;
    private AssetIdentifier id;
    private List<AssetIdentifier> sources = new List<AssetIdentifier>();
    #endregion

    #region Construction
    public LinearModifier() {}
    //copy
    public LinearModifier(LinearModifier other)
    {
        value_ = other.Value;
        multiplier = other.Multiplier;
        sources = other.Sources;
        id = other.Id;
    }
    #endregion
}
```
###### Code @ LinearModifier.cs
**LinearModifier's** field id, tells which parameter should be modified, whereas fields value and multiplier inform by how much.

# Ability
```csharp
public class Ability
{                
    public AssetIdentifier Id { get=>id; }
    public AbilityData Data { get=>data; }
    public bool Active { get=>active; }
    public bool Unlocked { get=>unlocked; set { unlocked = value; } }
    public bool Ready { get=>ready; set { ready = value; } }

    private AssetIdentifier id;
    private AbilityData data;
    private bool active;
    private bool unlocked;
    private bool ready;

    public Ability(AssetIdentifier id_, AbilityData data_)
    {
        this.id = id_;
        this.data = data_;
        this.active = data==null? false : data.active;        
        this.unlocked = false;
        this.ready = false;
    }
}
```
###### Code @ Ability.cs
**Ability** is container class with **AbilityData** field inside.

# AbilityMate
**AbilityMate** as main module of ability system is responsible for following task:
- Acquisition/Loss/Replacement of abilities
- Activation of abilities
- Communication with **Cooldowner** class object
- Notification of **IAbilityUser** when there is change in active abilities

To tackle first task, **AbilityMate** features *AcquireAbility, LoseAbility, ReplaceAbilities* methods:
``` csharp
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class AbilityMate : ICooldownOrder, ILinearModifierProvider, IActionReceiver<AbilityActionArgs>
{
    #region Variables & Properties
    List<Ability> abilities;
    IAbilityUser user;
    IActionInterface actionInterface;
    Cooldowner cooldowner;
    AssetAccess assetAccess;
    #endregion

    #region Acquire section
    public async Task<bool> ReplaceAbilities(AssetIdentifier oldId, AssetIdentifier  newId)
    {
        Ability oldAbility = this.GetAbility(oldId);
        if(oldAbility==null || !this.abilities.Contains(oldAbility))
            return false;

        AbilityData newAbilityData = await this.GetData(newId);        
        Ability newAbility = new Ability(newId, newAbilityData);

        this.abilities.Remove(oldAbility);
        this.abilities.Add(newAbility);
        this.AbilitiesChanged();
        return true;
    }

    public bool LoseAbility(AssetIdentifier abilityId)
    {
        Ability ability = this.GetAbility(abilityId);
        if(ability==null || !this.abilities.Contains(ability))
            return false;
        
        this.abilities.Remove(ability);
        this.AbilitiesChanged();
        return true;
    }
    
    public async Task<bool> AcquireAbility(AssetIdentifier abilityId)
    {
        if (await this.ChcekRequirements(abilityId))
        {
            Ability ability = new Ability(abilityId, await this.GetData(abilityId));            
            abilities.Add(ability);
            actionInterface.PassAction(ability.Data.acquireActions);
            AbilitiesChanged();
            return true;
        }
        else        
            return false;
    }

    public async Task<bool> ChcekRequirements(AssetIdentifier abilityiId)
    {        
        if(abilityiId == null)
            return false;

        AbilityData abilityData = await assetAccess.LoadAsset<AbilityData>(abilityiId);
        List<AssetIdentifier> requirements = abilityData?.requirements;

        if(requirements?.Count==null)
            return true;

        if(this.abilities?.Count==null && requirements.Count==0 )
            return true;

        List<AssetIdentifier> alreadyAcquired = abilities.Select(a => a.Id).ToList();
        foreach (AssetIdentifier requirement in requirements)
        {
            if (alreadyAcquired.Find(a => a == requirement) == null)
                return false;
        }
        return true;
    }
    
    private void AbilitiesChanged()
    {
        user.ProcessAbilities(abilities);
    }        
    #endregion

    private async Task<AbilityData> GetData(AssetIdentifier id)
    {
        if(id==null)
            return null;

        return await assetAccess.LoadAsset<AbilityData>(id);
    }
}
```
###### Code @ AbilityMate.cs (fragment)

In order to manipulate ability list one simply needs to pass **AssetIdentifer** object, other modules dont need to communicate with AbilityMate using whole **AbilityData** objects, because
**AbilityMate** loads **AbilityData** on its own with help from **AssetAccess** (see: [AssetAccess](/PROJECTS/SHIPIT/MODULES/ASSETACCESS.md))

Each of this three methods (*AcquireAbility, LoseAbility, ReplaceAbilities*) calls *AbilitiesChanged* method which notifies **IAbilityUser** about performed modifications.

To activate skills, **AbilityMate** provides *ActiveAbility* method:
```csharp
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class AbilityMate : ICooldownOrder, ILinearModifierProvider, IActionReceiver<AbilityActionArgs>
{
    #region Variables & Properties
    List<Ability> abilities;
    IAbilityUser user;
    IActionInterface actionInterface;
    Cooldowner cooldowner;
    AssetAccess assetAccess;
    #endregion

    #region Active section
    public bool ActiveAbility(AssetIdentifier assetId)
    {
        Ability ability = this.GetAbility(assetId);
        if(ability==null || !ability.Active || !ability.Ready)
            return false;

        if(actionInterface!=null)
            actionInterface.PassAction(ability.Data.activeActions);

        CooldownArgs cooldown = new CooldownArgs(ability.Id, ability.Data.cooldown, this);        
        cooldowner.AddCooldown(cooldown);
        ability.Ready = false;
        return true;        
    }
    #endregion

    private Ability GetAbility(AssetIdentifier id)
    {
        if(id==null || abilities==null)
            return null;

        return abilities.Find(a=>a.Id == id);
    }

    public void CooldownComplete(CooldownArgs arg)
    {
        Ability ability = this.GetAbility(arg.id);
        if(ability!=null)
            ability.Ready = true;
    }
}
```
###### Code @ AbilityMate.cs (fragment)
Ability activation forces **AbilityMate** to send signal to **Cooldowner** object and set ability temporary inactive (ability.Ready = false) for duration of a cooldown value, and when **Cooldowner** sends signal back on cooldown completion (*CooldownComplete* method) **AbilityMate** sets abiliy.Ready back to true (see [Cooldowner](/PROJECTS/SHIPIT/MODULES/COOLDOWNER.md)).

With a help of **AbilityActionArgs** class, **AbilityMate** provides alternate way of acquiring or losing abilities:

```csharp
using System.Collections.Generic;

public class AbilityActionArgs : ActionArgs
{
    public List<AssetIdentifier> acquire;
    public List<AssetIdentifier> lose;
    public Dictionary<AssetIdentifier,AssetIdentifier> replace;
}
```
###### Code @ AbilityActionArgs.cs
```csharp
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class AbilityMate : ICooldownOrder, ILinearModifierProvider, IActionReceiver<AbilityActionArgs>
{
    public void React(List<AbilityActionArgs> args)
    {
        if(args==null)
            return;

        if(args?.acquire!=null)
        {
            foreach(AssetIdentifier id in args.acquire)
                this.AcquireAbility(id);
        }
        
        if(args?.lose!=null)
        {
            foreach(AssetIdentifier id in args.lose)
                this.LoseAbility(id);
        }

        if(args?.replace!=null)
        {
            foreach (KeyValuePair<AssetIdentifier, AssetIdentifier> item in args.replace)
            {
                AssetIdentifier oldId = item.Key;
                AssetIdentifier newId = item.Value;
                this.ReplaceAbilities(oldId, newId);
            }
        }
    }
}
```
###### Code @ AbilityMate.cs (fragment)