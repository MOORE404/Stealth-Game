using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unlockCursor : MonoBehaviour
{
    //ensures the cursor works when returning to menu
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

}
