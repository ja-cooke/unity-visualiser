/// Author: Jonathan Cooke

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Class <c>ButtonToAppartment</c> script for the behaviour of a button to
/// load the apartment scene.
/// </summary>
public class ButtonToAppartment : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.gameObject.GetComponent<Button>().onClick.AddListener(() => { ChangeScene(); } );
    }

    void ChangeScene()
    {
        SceneManager.LoadScene("ApartmentScene");
    }
}
