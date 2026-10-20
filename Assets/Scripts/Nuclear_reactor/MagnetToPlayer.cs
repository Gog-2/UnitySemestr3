using Nuclear_reactor;
using UnityEngine;

public class MagnetToPlayer : MonoBehaviour
{
    [SerializeField] private bool _canMagnet = false;

    private void Start()
    {
        _canMagnet = Inventory.Instance.Unitiles[UnitType.Magnet];
        Inventory.Instance.OnUnitilesChanged += Unlocked;
    }

    private void Unlocked(UnitType type, bool value)
    {
        if (type == UnitType.Magnet)
        {
            _canMagnet = value;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_canMagnet && other.TryGetComponent(out Coin coin))
        {
            coin.IsMoveToPlayer(true, transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_canMagnet && other.TryGetComponent(out Coin coin))
        {
            coin.IsMoveToPlayer(false, transform);
        }
    }

    private void OnDisable()
    {
        Inventory.Instance.OnUnitilesChanged -= Unlocked;    
    }
}
