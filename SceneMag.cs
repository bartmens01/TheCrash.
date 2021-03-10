using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMag : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void NextScene()
    {
        SceneManager.LoadScene("Scene");
        
    }
    public void NextScene2()
    {
        SceneManager.LoadScene("Control");

    }
    public void NextScene3()
    {
        SceneManager.LoadScene("Main");

    }
}
