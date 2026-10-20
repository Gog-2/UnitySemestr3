using System;
using System.Collections.Generic;
using Alchemy.Inspector;
using Alchemy.Serialization;
using UnityEngine;

namespace Nuclear_reactor
{
    [AlchemySerialize]
    public partial class Inventory : Singltone<Inventory>
    {
        [AlchemySerializeField, NonSerialized] private Dictionary<UpgradeType, int> _upgrades;
        [AlchemySerializeField, NonSerialized] private Dictionary<UnitType, bool> _unitiles;
        
        public Dictionary<UpgradeType, int> Upgrades { get { return _upgrades; } }
        public Dictionary<UnitType, bool> Unitiles { get { return _unitiles; } }
        
        public event Action<UpgradeType, int> OnUpgradeChanged;
        public event Action<UnitType, bool> OnUnitilesChanged;

        private void Awake()
        {
            base.Awake();
            
            _upgrades ??= new Dictionary<UpgradeType, int>();
            _unitiles ??= new Dictionary<UnitType, bool>();
            
            _upgrades[UpgradeType.Passive] = PlayerPrefs.GetInt("Passive", 0);
            _upgrades[UpgradeType.CoinValue] = PlayerPrefs.GetInt("CoinValue", 0);
            _upgrades[UpgradeType.SpawnSpeed] = PlayerPrefs.GetInt("SpawnSpeed", 0);
            _unitiles[UnitType.Magnet] = Convert.ToBoolean(PlayerPrefs.GetInt("Magnet", 0));
        }

        public void Upgrade(UpgradeType upgradeType, int amount)
        {
            _upgrades[upgradeType] += amount;
            OnUpgradeChanged?.Invoke(upgradeType, _upgrades[upgradeType]);
        }

        public void Unitile(UnitType upgradeType, bool value)
        {
            _unitiles[upgradeType] = value;
            OnUnitilesChanged?.Invoke(upgradeType, _unitiles[upgradeType]);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus) SaveData();
        }

        private void OnApplicationQuit()
        {
            SaveData();
        }

        private void SaveData()
        {
            if (_upgrades == null || _unitiles == null) return;

            if (_upgrades.TryGetValue(UpgradeType.Passive, out var passive)) PlayerPrefs.SetInt("Passive", passive);
            if (_upgrades.TryGetValue(UpgradeType.CoinValue, out var coinVal)) PlayerPrefs.SetInt("CoinValue", coinVal);
            if (_upgrades.TryGetValue(UpgradeType.SpawnSpeed, out var spawnSpd)) PlayerPrefs.SetInt("SpawnSpeed", spawnSpd);
            if (_unitiles.TryGetValue(UnitType.Magnet, out var magnet)) PlayerPrefs.SetInt("Magnet", magnet ? 1 : 0);

            PlayerPrefs.Save();
        }
    }
    public enum UpgradeType
    {
        Passive,
        CoinValue,
        SpawnSpeed
    }

    public enum UnitType
    {
        Magnet
    }
}
