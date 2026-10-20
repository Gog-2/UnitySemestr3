using System;
using TMPro;
using UnityEngine;

namespace Nuclear_reactor.Upgrader
{
    public class BuyUpgradeStation : MonoBehaviour
    {
        [SerializeField] private UpgradeType _type;
        [SerializeField] private TMP_Text _info;

        private void Start()
        {
            UpdateText();
        }

        private void UpdateText()
        {
            _info.text = $"Type: {_type}, " +
                         $"Lvl: {Inventory.Instance.Upgrades[_type]} ," +
                         $"Price: {UpgradeManager.Instance.GetPrice(_type)}";
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<FirstPersonController>(out _))
            {
                UpgradeManager.Instance.TrySpendToUpgrade(_type);
                UpdateText();
            }
        }
    }
}