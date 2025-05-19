/// Author: Jonathan Cooke

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Class <c>ButtonToAbstract</c> script for the behaviour of a button to
/// load the abstract scene.
/// </summary>
public class ButtonToAbstract : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.gameObject.GetComponent<Button>().onClick.AddListener(() => { ChangeScene(); } );
    }

    void ChangeScene()
    {
        SceneManager.LoadScene("AbstractScene");
    }
}
