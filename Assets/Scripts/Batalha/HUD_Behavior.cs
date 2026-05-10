using UnityEngine;
using UnityEngine.UI;

public class HUD_Behavior : MonoBehaviour
{
    public Slider slider;

    private void Start() {
        Manager_Battle.Instance.hud = this;
    }

    public void UpdateSlider(float value) {
        slider.value = value;
    }
}
