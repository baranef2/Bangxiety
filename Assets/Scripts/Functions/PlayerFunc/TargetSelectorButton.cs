using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TargetSelectorButton : MonoBehaviour
{
    [SerializeField] private GameObject targetPlayerObject;

    private Player target;
    private Player shooter;

    private void Awake()
    {
        if (targetPlayerObject) target = targetPlayerObject.GetComponent<Player>();
    }

    // TargetSelectorUI çaðýrýr; artýk bool döndürüyor
    public bool SetContext(Player currentShooter)
    {
        shooter = currentShooter;
        bool canSelect = (target != null) && target.IsAlive && !ReferenceEquals(target, shooter);

        Debug.Log($"[{name}] SetContext -> target={(target ? target.name : "NULL")}, shooter={(shooter ? shooter.name : "NULL")}, isAlive={(target?.IsAlive)}, isSelf={ReferenceEquals(target, shooter)}, canSelect={canSelect}");
        Debug.Log($"[{name}] SetContext -> target={(target ? target.name : "NULL")}, shooter={(shooter ? shooter.name : "NULL")}, isAlive={(target?.IsAlive)}, isSelf={ReferenceEquals(target, shooter)}, canSelect={canSelect}");

        gameObject.SetActive(canSelect);
        return canSelect;
    }



    private void OnMouseDown()
    {
        if (target == null || !target.IsAlive || ReferenceEquals(target, shooter)) return;
        TargetSelectorUI.Instance.SelectTarget(target);
    }
}
