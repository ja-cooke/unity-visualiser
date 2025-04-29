using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ButtonToAppartment : MonoBehaviour
{
    private int SceneIndex { get; set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.gameObject.GetComponent<Button>().onClick.AddListener(() => { ChangeScene(SceneIndex); } );
    }

    void ChangeScene(int sceneIndex)
    {
        SceneManager.LoadScene("ApartmentScene");
    }
}
