using UnityEngine;
using TMPro;

public class piscapisca : MonoBehaviour
{
    public float speed = 2f;
    TMP_Text txt;

    void Start()
    {
        txt = GetComponent<TMP_Text>();
    }

    void Update()
    {
        Color color = txt.color;
        color.a = Mathf.PingPong(Time.time * speed, 1f);
        txt.color = color;
    }
}
