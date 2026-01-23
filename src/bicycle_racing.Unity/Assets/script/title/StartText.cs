using UnityEngine;
using UnityEngine.UI;

public class StartText : MonoBehaviour
{
    Text text;
    [SerializeField] float speed;
    float time = 0;
    Outline outline;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<Text>();
        outline = GetComponent<Outline>();
    }

    // Update is called once per frame
    void Update()
    {
        text.color = GetTextColorAlpha(text.color);
        outline.effectColor = GetTextColorAlpha(outline.effectColor);
    }

    Color GetTextColorAlpha(Color color)
    {
        time += Time.deltaTime * speed * 5.0f;
        color.a = Mathf.Sin(time) + 1.5f;

       

        return color;
    }
}
