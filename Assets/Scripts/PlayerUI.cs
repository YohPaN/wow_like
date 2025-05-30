using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthUI : MonoBehaviour
{
    public Canvas uiCanvas;
    public Image healthFill;
    public Image XpFill;
    public TextMeshProUGUI levelNumber;
    public TextMeshProUGUI healthText;

    public void UpdateHealth(float current, float max)
    {
        float percent = current / max;
        healthFill.fillAmount = percent;

        if (healthText != null)
            healthText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
    }

    public void ShowUI(bool visible)
    {
        uiCanvas.enabled = visible;
    }

    public void UpdateXp(float current, float max, int level)
    {
        float percent = current / max;

        // we set fill amount to 0.09 if under 0.08 because otherwise, 
        // we didn't see the xp bar (it is too short)
        XpFill.fillAmount = (float)(percent < 0.08 ? 0.09 : percent);

        // set the level number on the UI
        levelNumber.text = level.ToString();
    }
}