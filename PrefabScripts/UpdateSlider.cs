/// Author: Jonathan Cooke
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attaches to the Slider UI element on the visualiser UI panels.
/// Handles automatic update of the slider position, and maps position
/// to audio timecode.
/// </summary>
public class UpdateSlider : MonoBehaviour
{
    Slider Slider;
    AudioSource AudioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Slider = GetComponent<Slider>();

        // Ascends the GameObject heirarchy to find the player AudioSource
        // If the order of nesting is changed in the Editor this will need
        // to be updated as well.
        Transform graphBoundaryParent = transform.parent.parent.parent;
        AudioSource = graphBoundaryParent.GetChild(0).GetComponent<AudioSource>();

        // Set the slider values from the start to the end of the music
        Slider.minValue = 0;
        Slider.maxValue = AudioSource.clip.length;
    }

    // Update is called once per frame
    void Update()
    {
        // Updates the slider position once per frame
        Slider.value = AudioSource.time;
    }
}
