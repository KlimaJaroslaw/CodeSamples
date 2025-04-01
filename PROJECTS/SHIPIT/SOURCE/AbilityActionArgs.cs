using System.Collections.Generic;

public class AbilityActionArgs : ActionArgs
{
    public List<AssetIdentifier> acquire;
    public List<AssetIdentifier> lose;
    public Dictionary<AssetIdentifier,AssetIdentifier> replace;
}