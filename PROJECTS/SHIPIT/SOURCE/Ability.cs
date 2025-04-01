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