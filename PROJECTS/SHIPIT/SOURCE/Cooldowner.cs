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