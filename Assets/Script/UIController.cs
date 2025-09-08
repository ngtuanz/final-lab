using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject backroom;
    public GameObject optionMenu;

    public void OnOptionPressed()
    {
        backroom.SetActive(false);
        optionMenu.SetActive(true);
    }

    public void OnBackPressed()
    {
        optionMenu.SetActive(false);
        backroom.SetActive(true);
    }
}
