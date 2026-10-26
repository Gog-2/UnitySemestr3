using System.Collections.Generic;
using System.Linq;
using Calculator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CalculatorService : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _dropdown;
    [SerializeField] private CalculatorSO[] _calculatorSos;
    private CalculationFormulaSO _activeFormula;
    
    [SerializeField] private TMP_Text _discriprionA;
    [SerializeField] private TMP_InputField _inputFieldA;
    
    [SerializeField] private TMP_Text  _discriprionB;
    [SerializeField] private TMP_InputField _inputFieldB;
    
    [SerializeField] private Button _calculateButton;
    [SerializeField] private TMP_Text _resultText;

    private void Start()
    {
        List<string> options = _calculatorSos.Select(name => name.CalculationsName).ToList();
        
        _dropdown.ClearOptions();
        _dropdown.AddOptions(options);
        _dropdown.value = 0;
        SetFormula();
    }

    public void SetFormula()
    {
        int index = _dropdown.value;
        _activeFormula = _calculatorSos[index].Formula;
        _discriprionA.text = _calculatorSos[index].DiscriprionA;
        _discriprionB.text = _calculatorSos[index].DiscriprionB;
    }

    public void CheckResultAvalible()
    {
        if (_inputFieldA.text == "" || _inputFieldB.text == "")
        {
            _calculateButton.interactable = false;
        }
        else
        {
            _calculateButton.interactable = true;
        }
    }
    
    public void Result()
    {
        string formula = _activeFormula.Formula;
        
        formula = formula.Replace("A",_inputFieldA.text);
        formula = formula.Replace("B",_inputFieldB.text);

        _resultText.text = s_CalculatorStringToResult.Evaluate(formula).ToString();
    }
}
