using System.Collections.Generic;

public interface ICooldownOrder
{
    public void CooldownComplete(CooldownArgs arg);
    public List<CooldownArgs> GetCooldowns();    
}