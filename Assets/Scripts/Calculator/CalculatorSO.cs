using UnityEngine;

namespace Calculator
{
    [CreateAssetMenu(fileName = "FormulaDiscription", menuName = "Calculator/FormulaDiscription")]
    public class CalculatorSO : ScriptableObject
    {
        public string CalculationsName;
        public CalculationFormulaSO Formula;
        [TextArea]public string DiscriprionA;
        [TextArea]public string DiscriprionB;
    }
}