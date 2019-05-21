using UnityEngine;

public class ForceResolution : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
#if UNITY_STANDALONE_WIN
        Screen.SetResolution(360, 640, false);
#endif
    }

}
