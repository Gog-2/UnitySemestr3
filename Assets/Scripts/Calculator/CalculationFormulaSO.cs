using UnityEngine;

namespace Calculator
{
    [CreateAssetMenu(fileName = "Formula", menuName = "Calculator/Formula")]
    public class CalculationFormulaSO : ScriptableObject
    {
        public string Formula;    
    }
}