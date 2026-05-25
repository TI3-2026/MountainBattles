using System;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Behavior : MonoBehaviour
{
    public Slider slider;

    [Header("Skill Check")]
    public Slider skillCheck;
    public float skillCheckSpeed = 1f;
    public float skillCheckGain = 2f;
    public float skillCheckMax = 0.65f;
    public float skillCheckMin = 0.35f;

    private bool skillCheckDirection = true; // true -> right, false -> left

    private void Start() {
        Manager_Battle.Instance.hud = this;

        skillCheckDirection = true;
        skillCheck.value = 0f;
    }

    private void Update()
    {
        SkillCheckBehavior();
    }

    public void UpdateSlider(float value) {
        slider.value = value;
    }

    public bool CheckSkillCheck()
    {
        if (skillCheck.value <= skillCheckMax && skillCheck.value >= skillCheckMin) return true;
        return false;
    }

    private void SkillCheckBehavior()
    {
        if (skillCheckDirection)
        {
            skillCheck.value += skillCheckSpeed * Time.deltaTime;
            if (skillCheck.value >= 1f)
            {
                skillCheckDirection = false;
                skillCheck.value = 1f;
            }
        }
        else
        {
            skillCheck.value -= skillCheckSpeed * Time.deltaTime;
            if (skillCheck.value <= 0f)
            {
                skillCheckDirection = true;
                skillCheck.value = 0f;
            }
        }
    }
    
}
