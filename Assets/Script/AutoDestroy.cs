using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    private void Start()
    {
        // Lấy animation length và tự xóa sau đó
        float lifeTime = GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        Destroy(gameObject, lifeTime);
    }
}
