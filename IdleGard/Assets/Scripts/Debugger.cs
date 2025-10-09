using UnityEngine;

public class Debugger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) SaveManager.instance.ClearData();
        if (Input.GetKeyDown(KeyCode.L)) SaveManager.instance.LoadSave();
        if (Input.GetKeyDown(KeyCode.F))
        {
            Time.timeScale = Time.timeScale == 1 ? 4f : 1;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
