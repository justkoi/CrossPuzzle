using UnityEngine;

public class DontDestroyObject : MonoBehaviour
{
    public void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
