using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Nuclear_reactor
{
    public class PassiveCoinsManager : MonoBehaviour
    {
        [SerializeField] private float _coinsPassiveColdown;
        private int _passiveIncreace;
        private CancellationTokenSource _cts;

        private void Start()
        {
            UpgradePassive(
                UpgradeType.Passive, 
                Inventory.Instance.Upgrades[UpgradeType.Passive]);
            
            Inventory.Instance.OnUpgradeChanged += UpgradePassive;
            
            _cts = new CancellationTokenSource();
            
            Run(_cts.Token).Forget();
        }

        private async UniTaskVoid Run(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                bool isCanceled = await UniTask.WaitForSeconds(
                        _coinsPassiveColdown,
                        ignoreTimeScale: false,
                        cancellationToken: token)
                    .SuppressCancellationThrow();
                
                if (isCanceled) break;
                
                AddMoney();
            }          
        }

        private void AddMoney()
        {
            WalletManager.Instance.AddCurrency(_passiveIncreace);
        }

        private void UpgradePassive(UpgradeType upgradeType, int value)
        {
            if (upgradeType == UpgradeType.Passive)
            {
                _passiveIncreace =  value;
            }
        }

        private void OnDisable()
        {
            Inventory.Instance.OnUpgradeChanged -= UpgradePassive;    
        }
        
    }
}