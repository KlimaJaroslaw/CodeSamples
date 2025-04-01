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
    //List<CooldownArgs> cooldowns;
    AssetAccess assetAccess;
    #endregion

    #region Construction
    public AbilityMate(Cooldowner cooldowner_, IActionInterface actionInterface_, IAbilityUser user_, AssetAccess assetAccess_)
    {
        this.cooldowner = cooldowner_;
        this.actionInterface = actionInterface_;
        this.user = user_;
        this.abilities = new List<Ability>();
        this.assetAccess = assetAccess_;
        //this.cooldowns = new List<CooldownArgs>();
    }
    #endregion

    #region Methods
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

    #region Getters section
    public List<AssetIdentifier> GetAbilities()
    {        
        return abilities?.Select(a=>a.Id).ToList();
    }

    private Ability GetAbility(AssetIdentifier id)
    {
        if(id==null || abilities==null)
            return null;

        return abilities.Find(a=>a.Id == id);
    }    

    private async Task<AbilityData> GetData(AssetIdentifier id)
    {
        if(id==null)
            return null;

        return await assetAccess.LoadAsset<AbilityData>(id);
    }
    #endregion

    #endregion

    #region Interface methods
    public void CooldownComplete(CooldownArgs arg)
    {
        //cooldowns.Remove(arg);
        Ability ability = this.GetAbility(arg.id);
        if(ability!=null)
            ability.Ready = true;
    }

    public List<LinearModifier> GetModifiers()
    {
        List<LinearModifier> result = new List<LinearModifier>();
        if (abilities==null)
            return result;

        foreach(Ability a in abilities)
        {
            if(a?.Data?.passiveModifiers==null)
                continue;
            result.AddRange(a.Data.passiveModifiers);
        }
        return result;
    }

    public List<CooldownArgs> GetCooldowns()
    {
        return this.cooldowner.GetCooldowns(this);
    }    

    public void React(List<AbilityActionArgs> args)
    {
        throw new System.NotImplementedException();
    }
    #endregion
}