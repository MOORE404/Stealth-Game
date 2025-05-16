using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Slider staminaBar; 
    public PlayerController player;

    public Text noiseMakerText;
    public Text keyAmountText;   

    public KeyManager keyManager; 

    void Start()
    {
        if (player != null && staminaBar != null)
        {
            staminaBar.maxValue = player.maxSprintTime;
            staminaBar.value = player.sprintRemaining;
        }

        UpdateKeyInfo();
    }

    void Update()
    {
        if (player != null && staminaBar != null)
        {
            staminaBar.value = player.sprintRemaining;
        }

        UpdateKeyInfo();
    }

    void UpdateKeyInfo()
    {
        if (keyManager != null)
        {
            if (noiseMakerText != null)
                noiseMakerText.text = "Noise Makers: " + keyManager.noiseMakerAmount;

            if (keyAmountText != null)
                keyAmountText.text = "Keys: " + keyManager.keyAmount;
        }
    }
}
