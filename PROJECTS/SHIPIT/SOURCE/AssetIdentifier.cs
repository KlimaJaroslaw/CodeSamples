using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "AssetIdentifier", menuName = "AssetIdentification/AssetIdentifier")]
public class AssetIdentifier : ScriptableObject
{
    public int assetId;
    public string assetName;

    #region overrides
    public override bool Equals(object obj)
    {        
        if (ReferenceEquals(this, obj))
            return true;        
        if (obj is not AssetIdentifier other)
            return false;        
        return assetId == other.assetId && string.Equals(assetName, other.assetName, StringComparison.Ordinal);
    }

    public override int GetHashCode()
    {        
        return HashCode.Combine(assetId, assetName);
    }

    public static bool operator ==(AssetIdentifier left, AssetIdentifier right)
    {        
        if (ReferenceEquals(left, right))
            return true;
        if (left is null || right is null)
            return false;        
        return left.Equals(right);
    }

    public static bool operator !=(AssetIdentifier left, AssetIdentifier right)
    {
        return !(left == right);
    }
    #endregion
}
