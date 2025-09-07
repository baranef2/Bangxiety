using UnityEngine;

public class Executer : MonoBehaviour
{
    private Transform target;
    [SerializeField] private float speed = 15f;
    [SerializeField] private float destroyDistance = 0.5f;
    [SerializeField] private float lifeTime = 5f;

    private Vector3 direction;

    public void Initialize(Transform targetTransform)
    {
        target = targetTransform;
        if (target)
            direction = (target.position - transform.position).normalized;
    }

    private void Update()
    {
        if (!target)
        {
            Destroy(gameObject);
            return;
        }

        transform.position += direction * speed * Time.deltaTime;

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= destroyDistance)
        {
            Debug.Log($"[Executer] Hedefe ulaþýldý: {target.name}");
            Destroy(gameObject);
        }

        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0f)
        {
            Debug.Log("[Executer] Süre doldu, yok edildi.");
            Destroy(gameObject);
        }
    }
}
