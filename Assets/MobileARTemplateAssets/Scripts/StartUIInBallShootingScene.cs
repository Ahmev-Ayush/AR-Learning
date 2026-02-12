using UnityEngine;

public class StartUIInBallShootingScene : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject welcomePanel;
    public GameObject adjustPanel;

    void Start()
    {
        // When the app starts, show Welcome and hide Adjust
        welcomePanel.SetActive(true);
        adjustPanel.SetActive(false);
    }

    // Called by the 'Start' button on the Welcome Panel
    public void StartSetup()
    {
        welcomePanel.SetActive(false);
        adjustPanel.SetActive(true);
    }

    // Called by the 'Done' button on the Adjust Panel
    public void FinishAndLock()
    {
        adjustPanel.SetActive(false);
        // The UI is now gone, and the user can play!
    }
}