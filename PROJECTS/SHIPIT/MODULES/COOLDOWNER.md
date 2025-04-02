[:arrow_up: ShipIt (Project)](/PROJECTS/SHIPIT/SHIPIT.md)

[:arrow_right: NEXT MODULE: Asset Access](/PROJECTS/SHIPIT/MODULES/ASSETACCESS.md)

# Cooldowner
Class responsible for updating cooldowns and notifying senders when cooldown is completed. It is designed to work with all sorts of timers (e.g. ability duration, skill cooldown, etc.)

## SOURCE CODE FILES
:link: [Cooldowner.cs](/PROJECTS/SHIPIT/SOURCE/Cooldowner.cs)\
:link: [CooldownArgs.cs](/PROJECTS/SHIPIT/SOURCE/CooldownArgs.cs)\
:link: [ICooldownOrder.cs](/PROJECTS/SHIPIT/SOURCE/ICooldownOrder.cs)

# CooldownArgs and ICooldownOrder
Before I jump to **Cooldowner** class and describe how it stores cooldowns and notify senders when cooldown is completed, I would like to present two other classes that make cooldown management possible at all.

**CooldownArgs, ICooldownOrder**

**Cooldowner** object uses list of **CooldownArgs** class instances to keep track of all concurrent cooldowns, and each object of that list contains sender information (**ICooldownOrder**) so that **Cooldowner** can notify right interface when cooldown time is over.


Here is structure of **CooldownArgs** class:
``` csharp
using System.Collections.Generic;

public class CooldownArgs
{
    private AssetIdentifier id;     
    private float duration;
    private ICooldownOrder order;

    public float progress = 0;   
    public float Progress100 { get => duration==0? 1 : progress / duration; }

    public CooldownArgs(AssetIdentifier id_, float cooldownDuration_, ICooldownOrder order_)
    {
        this.id = id_;
        this.duration = cooldownDuration_;
        this.order = order_;
    }
}
```
###### Code @ CooldownArgs.cs

**CooldownArgs** is container type class, its fields hold essential information for cooldown management such as:
- **id:** tells which assets this cooldown refers to. (example: id of ability  “cannon fire”)\
- **duration:** cooldown time in seconds
- **order:** tells which object of **ICooldownOrder** type sent this cooldown order (example: AbilityMate : **ICooldownOrder** awaits cooldown completion)
- **progress:** how much time have already passed in seconds

And this is **ICooldownerOrder** interface:
``` csharp
using System.Collections.Generic;

public interface ICooldownOrder
{
    public void CooldownComplete(CooldownArgs arg);
    public List<CooldownArgs> GetCooldowns();    
}
```
###### Code @ ICooldownOrder.cs

Interface features two methods that:
- Allow **Cooldowner** to notify **ICooldownOrder** on cooldown completion
- Require implementation of method that will return ordered cooldowns

---
To summarize before going further:

- **CooldownArgs:** Class that holds complete information about cooldown (progress, duration, etc.)
- **ICooldownOrder:** Interface that allows to notify sender when cooldown is completed. It also enables others to access all ordered cooldowns by this instance.
- **Cooldowner:** Class that stores and manages all cooldowns and notifies senders on completion.

# Cooldowner (Main module)

**Cooldowner** class derives form *Unity’s MonoBehaviour* class and uses *Update* method to update time on all cooldowns.
``` csharp 
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cooldowner : MonoBehaviour
{
    List<CooldownArgs> cooldowns = new List<CooldownArgs>();

    private void Update()
    {
        foreach(CooldownArgs c in cooldowns)
        {
            c.progress += Time.deltaTime;            
            if (c.Progress100>=1)
            {
                c.order.CooldownComplete(c);
                cooldowns.Remove(c);
            }                                        
        }
    }    
}
```
###### Code @ Cooldowner.cs (fragment)

As shown above: when cooldown is completed **Cooldowner** calls *CooldownComplete* method on c.order object which is field of type **ICooldownOrder** in **CooldownArg** object, telling sender that cooldown is over.

To add, remove or reset cooldown one need to call *AddCooldown(CooldownArgs args)* or *RemoveCooldown(CooldownArgs args)* method.
``` csharp 
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Cooldowner : MonoBehaviour
{
    List<CooldownArgs> cooldowns = new List<CooldownArgs>();
    public void AddCooldown(CooldownArgs args) 
    {
        CooldownArgs a = cooldowns.Find(x => x.id == args.id);
        if (a == null)        
            cooldowns.Add(args);        
        else        
            a.duration = 0;            
    }

    public void RemoveCooldown(CooldownArgs args)
    {
        if(cooldowns.Contains(args))
            cooldowns.Remove(args);
    }
}
```
###### Code @ Cooldowner.cs (fragment)

Cooldowner also features two getter methods:
``` csharp 
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Cooldowner : MonoBehaviour
{    
    List<CooldownArgs> cooldowns = new List<CooldownArgs>();

    public CooldownArgs GetCooldown(ICooldownOrder o, AssetIdentifier id)
    {
        List<CooldownArgs> cooldownsRelevant = this.GetCooldowns(o);
        if(cooldownsRelevant==null || cooldownsRelevant.Count<1)
            return null;
        return cooldownsRelevant.Find(a=>a.id==id);
    }

    public List<CooldownArgs> GetCooldowns(ICooldownOrder o)
    {
        if(cooldowns==null)
            return null;

        return cooldowns.Where(c=>c.order==o)?.ToList();
    }
}
```
###### Code @ Cooldowner.cs (fragment)

- *GetCooldown(**ICooldownOrder** o, AssetIdentifier id)*: allows to access specific cooldown
- *GetCooldowns(**ICooldownOrder** o)*: returns all cooldowns ordered by certain **ICooldownOrder** sender