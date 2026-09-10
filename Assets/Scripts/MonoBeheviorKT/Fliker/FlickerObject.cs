using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class FlickerObject : MonoBehaviour
{
    [Header("Color Settings")]
    [SerializeField] private Color _flickerColor;
    [SerializeField] private float _duration = 1f;
    [SerializeField] private float _coldown = 2f;
    
    private Color _cachedColor;
    private Material _material;
    private bool _swapped = false;
    
    private readonly int _emisionId = Shader.PropertyToID("_EmissionColor");
    private void Start()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        _material = meshRenderer.material;
        
        _cachedColor = _material.GetColor(_emisionId);
        StartCycle();
    }

    private void StartCycle()
    {
            ChangingColor(_flickerColor, _cachedColor).Forget();
    }

    private async UniTaskVoid ChangingColor(Color baseColor, Color endColor)
    {
        
        for (float i = 0; i < _duration; i += Time.deltaTime)
        {
            float t = Mathf.Min(i / _duration, 1f);
            
            _material.SetColor(_emisionId,Color.Lerp(baseColor, endColor, t));

            await UniTask.Yield(PlayerLoopTiming.Update);
        }
        
        for (float i = 0; i < _duration; i += Time.deltaTime)
        {
            float t = Mathf.Min(i / _duration, 1f);
            
            _material.SetColor(_emisionId,Color.Lerp(endColor, baseColor, t));

            await UniTask.Yield(PlayerLoopTiming.Update);
        }
        
        _material.SetColor(_emisionId, baseColor);
        _swapped = !_swapped;

        await UniTask.WaitForSeconds(_coldown);
        
        StartCycle();
    }
}
