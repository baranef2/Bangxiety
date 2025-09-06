using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;
    public float speed = 12f;
    public float destroyAfterSeconds = 3f;

    public void Initialize(Transform targetTransform)
    {
        target = targetTransform;
        Destroy(gameObject, destroyAfterSeconds); // emniyet
    }

    private void Update()
    {
        if (target == null) { Destroy(gameObject); return; }

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );
    }
}
