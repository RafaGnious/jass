using UnityEngine;

/// <summary>
/// Simple MonoBehaviour to add a constant Speed to an object along with other useful functions
/// </summary>
public class ConstantSpeed : MonoBehaviour
{

    [Tooltip("Object Pool to go to on screen exit. If null nothing will happen. Screen exit detection will require a Renderer.")]
    public ObjectPool Repool;
    [SerializeField]
    Vector3 Speed;

    /// <summary>
    /// Manually places Object back in it's pool. Will throw exception if Repool is null;
    /// </summary>
    public void SelfRepool()
    {
        Repool.Repool(gameObject);
    }

    //Will be called on screen exit as long as there is a renderer
    //Won't do anything if Repool is null
    private void OnBecameInvisible()
    {
        if (Repool != null)
        {
            SelfRepool();
        }
    }

    void Update()
    {
        if (gameObject.activeSelf) transform.position += Speed * Time.deltaTime;
    }
}
