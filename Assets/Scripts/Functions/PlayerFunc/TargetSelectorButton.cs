using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TargetSelectorButton : MonoBehaviour
{
    [SerializeField] private GameObject targetPlayerObject;

    private void Reset()
    {
        if (!TryGetComponent<Collider>(out _)) gameObject.AddComponent<BoxCollider>();
    }

    private void OnMouseDown()
    {
        if (targetPlayerObject == null)
        {
            Debug.LogWarning("TargetSelectorButton: targetPlayerObject atanmad�.");
            return;
        }

        if (targetPlayerObject.TryGetComponent<Player>(out var targetPlayer))
        {
            TargetSelectorUI.Instance.SelectTarget(targetPlayer);
        }
        else
        {
            Debug.LogWarning($"TargetSelectorButton: {targetPlayerObject.name} �zerinde Player component'i yok.");
        }
    }
}
