using UnityEngine;
using System;

public class TargetSelectorUI : MonoBehaviour
{
    public static TargetSelectorUI Instance { get; private set; }

    [Header("Butonlar (sahne instance'larý)")]
    [SerializeField] private GameObject targetLeft;
    [SerializeField] private GameObject targetRight;
    [SerializeField] private GameObject targetFront;

    private Player currentShooter;
    private Action<Player> onSelected;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // Slotlar boþsa tag ile sahne instance'ýný bul (prefab deðil!)
        if (!targetLeft)  targetLeft  = GameObject.FindGameObjectWithTag("TargetLeft");
        if (!targetRight) targetRight = GameObject.FindGameObjectWithTag("TargetRight");
        if (!targetFront) targetFront = GameObject.FindGameObjectWithTag("TargetFront");

        HideTargets();
        DebugDump("Awake");
    }

    public void HideTargets()
    {
        if (targetLeft)  targetLeft.SetActive(false);
        if (targetRight) targetRight.SetActive(false);
        if (targetFront) targetFront.SetActive(false);
    }

    public bool ShowTargetOptions(Player shooter, Action<Player> callback)
    {
        currentShooter = shooter;
        onSelected = callback;

        int shown = 0;
        shown += SetBtn(targetLeft,  shooter);
        shown += SetBtn(targetRight, shooter);
        shown += SetBtn(targetFront, shooter);

        DebugDump("ShowTargetOptions");
        if (shown == 0)
        {
            Debug.LogError("[TSUI] Gösterilecek hedef yok: slotlar null/prefab ya da tüm hedefler ölü/kendi.");
        }
        return shown > 0;
    }

    private int SetBtn(GameObject btnGO, Player shooter)
    {
        if (!btnGO) return 0;

        // Parent kapalýysa aç (aksi halde çocuk aktif edilemez)
        var parent = btnGO.transform.parent;
        if (parent && !parent.gameObject.activeSelf) parent.gameObject.SetActive(true);

        var btn = btnGO.GetComponent<TargetSelectorButton>();
        bool can = btn ? btn.SetContext(shooter) : false;
        //return can ? 1 : 0;

        // Sahne instance'ý deðilse uyar
        if (!btnGO.scene.IsValid())
            Debug.LogError($"[TSUI] {btnGO.name} sahne instance'ý deðil (prefab asset baðlanmýþ).");

        return can ? 1 : 0;
    }

    public void SelectTarget(Player p)
    {
        onSelected?.Invoke(p);
        HideTargets();
    }

    private void DebugDump(string where)
    {
        string L = targetLeft  ? $"{targetLeft.name} (scene={targetLeft.scene.IsValid()})"  : "NULL";
        string R = targetRight ? $"{targetRight.name} (scene={targetRight.scene.IsValid()})" : "NULL";
        string F = targetFront ? $"{targetFront.name} (scene={targetFront.scene.IsValid()})" : "NULL";
        Debug.Log($"[TSUI::{where}] Left:{L} Right:{R} Front:{F}");
    }
}
