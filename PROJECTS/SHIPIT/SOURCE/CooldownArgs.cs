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