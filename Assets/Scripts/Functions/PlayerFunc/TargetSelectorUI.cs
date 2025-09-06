using UnityEngine;
using System;

public class TargetSelectorUI : MonoBehaviour
{
    public static TargetSelectorUI Instance { get; private set; }

    [Header("Hedef UI Objeleri (Left/Right/Front)")]
    [SerializeField] private GameObject targetLeft;
    [SerializeField] private GameObject targetRight;
    [SerializeField] private GameObject targetFront;

    private Player currentShooter;
    private Action<Player> onTargetSelected;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        HideTargets();
    }

    public void ShowTargetOptions(Player shooter, Action<Player> onSelected)
    {
        currentShooter = shooter;
        onTargetSelected = onSelected;

        targetLeft.SetActive(true);
        targetRight.SetActive(true);
        targetFront.SetActive(true);
    }

    public void HideTargets()
    {
        if (targetLeft) targetLeft.SetActive(false);
        if (targetRight) targetRight.SetActive(false);
        if (targetFront) targetFront.SetActive(false);
    }

    public void SelectTarget(Player target)
    {
        if (target == null) { Debug.LogWarning("TargetSelectorUI: target null."); return; }
        onTargetSelected?.Invoke(target);
        HideTargets();
    }
}
