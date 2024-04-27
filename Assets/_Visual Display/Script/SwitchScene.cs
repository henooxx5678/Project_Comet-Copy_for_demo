using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    // Start is called before the first frame update
    public void SwitchSceneTrigger()
    {
        SceneManager.LoadScene(1);
    }
}
