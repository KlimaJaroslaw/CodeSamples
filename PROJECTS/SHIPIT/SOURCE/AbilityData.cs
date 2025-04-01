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