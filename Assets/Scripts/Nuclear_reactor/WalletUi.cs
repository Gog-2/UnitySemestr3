using System;
using TMPro;
using UnityEngine;

namespace Nuclear_reactor
{
    public class WalletUi : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        private void Start()
        {
            WalletManager.Instance.OnValueChanged += ChangeValue;
        }

        private void ChangeValue(int value)
        {
            _text.text = value.ToString();
        }
    }
}
