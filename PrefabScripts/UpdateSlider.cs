using UnityEngine;
using UnityEngine.UI;

public class UpdateSlider : MonoBehaviour
{
    Slider Slider;
    AudioSource AudioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Slider = GetComponent<Slider>();

        Transform graphBoundaryParent = transform.parent.parent.parent;
        AudioSource = graphBoundaryParent.GetChild(0).GetComponent<AudioSource>();

        Slider.minValue = 0;
        Slider.maxValue = AudioSource.clip.length;
    }

    // Update is called once per frame
    void Update()
    {
        Slider.value = AudioSource.time;
    }
}
