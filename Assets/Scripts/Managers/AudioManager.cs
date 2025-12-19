using UnityEngine;
using AK.Wwise;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Function calling wwise events
    public static void ExampleFn()
    {
        //Put some logic (preparing data)
        Debug.Log("Hit example fn");

        //Call wwise events
    }
}
