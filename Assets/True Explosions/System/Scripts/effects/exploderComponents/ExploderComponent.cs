using UnityEngine;

public abstract class ExploderComponent : MonoBehaviour
{
    public abstract void onExplosionStarted(Exploder exploder);
    void Start()
    {
        // is needed to have the ability to disable the components
    }
}
