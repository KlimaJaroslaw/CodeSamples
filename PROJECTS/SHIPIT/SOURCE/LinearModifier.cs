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