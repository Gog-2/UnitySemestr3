using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class WaveAnnouncer : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TMP_Text _text;

    [Header("Animation")]
    [SerializeField] private float _fadeInDuration = 0.35f;
    [SerializeField] private float _holdDuration = 1f;
    [SerializeField] private float _fadeOutDuration = 0.35f;

    [SerializeField] private Vector3 _startScale = new Vector3(0.7f, 0.7f, 1f);
    [SerializeField] private Vector3 _endScale = Vector3.one;

    private Sequence _sequence;

    private void Awake()
    {
        if (_canvasGroup == null)
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        gameObject.SetActive(false);
    }

    public void Announce(string message, Action onComplete)
    {
        KillSequence();

        if (_text == null || _canvasGroup == null)
        {
            Debug.LogWarning("WaveAnnouncer: text or canvas group is missing. Skipping animation.", this);
            onComplete?.Invoke();
            return;
        }

        _text.text = message;

        gameObject.SetActive(true);
        _canvasGroup.alpha = 0f;
        transform.localScale = _startScale;

        _sequence = DOTween.Sequence();

        _sequence.Append(_canvasGroup.DOFade(1f, _fadeInDuration));
        _sequence.Join(transform.DOScale(_endScale, _fadeInDuration).SetEase(Ease.OutBack));

        _sequence.AppendInterval(_holdDuration);

        _sequence.Append(_canvasGroup.DOFade(0f, _fadeOutDuration));
        _sequence.Join(transform.DOScale(_startScale, _fadeOutDuration).SetEase(Ease.InBack));

        _sequence.OnComplete(() =>
        {
            gameObject.SetActive(false);
            onComplete?.Invoke();
        });

        _sequence.SetUpdate(true);
    }

    private void KillSequence()
    {
        if (_sequence != null && _sequence.IsActive())
        {
            _sequence.Kill();
        }

        _sequence = null;
    }

    private void OnDestroy()
    {
        KillSequence();
    }
}