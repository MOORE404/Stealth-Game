using UnityEngine;
using UnityEngine.SceneManagement;
public class UImanager : MonoBehaviour
{

    public void LoadScene(int Level){
        SceneManager.LoadScene(Level);
    }
}
