using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WindowsManager : MonoBehaviour
{
    public static WindowsManager instance;

    public GameObject SpecialistWindowPrefab;
    public GameObject ReportWindowPrefab;
    public GameObject SettingsWindowPrefab;
    public GameObject CommitmentWindowPrefab;


    private GameObject OpenWindow;

    public Color SelectedColor;
    public Color NormalColor;

    public enum WindowType
    {
        Report,
        Specialist,
        Settings,
        Commitment,
        NULL
    }

    [System.Serializable]
    public struct SelectButton
    {
        public WindowType windowType;
        public Image image;
    }

    public List<SelectButton> NavbarButtons;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    public static bool CheckOpenWindow()
    {
        if (instance == null)
        {
            return false;
        }

        return instance.OpenWindow != null || GameObject.FindGameObjectWithTag("PopUpWindow") != null;
    }

    public void GenerateReportWindow()
    {
        if (OpenWindow != null)
        {
            ClearWindow();
        }
        OpenWindow = Instantiate(instance.ReportWindowPrefab, instance.gameObject.transform, false);
        // ResetButtons(WindowType.Report);
    }

    public void GenerateSpecialistWindow()
    {
        if (OpenWindow != null)
        {
            ClearWindow();
        }
        OpenWindow = Instantiate(instance.SpecialistWindowPrefab, instance.gameObject.transform, false);
        // ResetButtons(WindowType.Specialist);
    }

    public void GenerateSettingsWindow()
    {
        if (OpenWindow != null)
        {
            ClearWindow();
        }
        OpenWindow = Instantiate(instance.SettingsWindowPrefab, instance.gameObject.transform, false);
        // ResetButtons(WindowType.Settings);
    }

    public void GenerateCommitmentWindow()
    {
        if (OpenWindow != null)
        {
            ClearWindow();
        }
        OpenWindow = Instantiate(instance.CommitmentWindowPrefab, instance.gameObject.transform, false);
        // ResetButtons(WindowType.Settings);
    }

    public void ClearWindow()
    {
        // ClearButtons();
        ReportUI.instance = null;
        Destroy(OpenWindow);
    }

    // public void ResetButtons(WindowType windowType)
    // {
    //     Debug.Log("Opened window: " + windowType.ToString());
    //     foreach (SelectButton selectButton in NavbarButtons)
    //     {
    //         if (selectButton.windowType == windowType)
    //         {
    //             Debug.Log("Set Button to selected color: " + selectButton.windowType.ToString() + " Opened Window: " + windowType.ToString());
    //             selectButton.image.color = SelectedColor;

    //         }
    //         else
    //         {
    //             Debug.Log("Set Button to normal color: " + selectButton.windowType.ToString() + " Opened Window: " + windowType.ToString());
    //             selectButton.image.color = NormalColor;
    //         }
    //     }
    // }

    // public void ClearButtons()
    // {
    //     foreach (SelectButton selectButton in NavbarButtons)
    //     {
    //         Debug.Log("Null so all normal");
    //         selectButton.image.color = NormalColor;
    //     }
    // }
}
