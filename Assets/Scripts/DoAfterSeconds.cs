using UnityEngine;
using UnityEngine.Events;

public class DoAfterSeconds : MonoBehaviour
{

    [SerializeField]
    private UnityEvent onTimeExpired;
    [SerializeField]
    private float time;

    private void Update()
    {
        if(Time.time >= time)
        {
            if(onTimeExpired != null)
            {
                onTimeExpired.Invoke();
                this.enabled = false;
            }
        }
    }
}
