using System;
using System.Collections.Generic;
using Alchemy.Inspector;
using Alchemy.Serialization;

namespace Nuclear_reactor
{
    [AlchemySerialize]
    public partial class Inventory : Singltone<Inventory>
    {
        [AlchemySerializeField, NonSerialized] private Dictionary<UpgradeType, int> _upgrades;
        
        public Dictionary<UpgradeType, int> Upgrades { get { return _upgrades; } }
        
        public event Action<UpgradeType, int> OnValueChanged;

        public void Upgrade(UpgradeType upgradeType, int amount)
        {
            _upgrades[upgradeType] += amount;
            OnValueChanged?.Invoke(upgradeType, _upgrades[upgradeType]);
        }
    }
    public enum UpgradeType
    {
        Passive,
        CoinValue,
        SpawnSpeed
    }
}
