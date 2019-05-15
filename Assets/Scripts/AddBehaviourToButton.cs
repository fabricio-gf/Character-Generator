using UnityEngine.UI;
using UnityEngine;

public class AddBehaviourToButton : MonoBehaviour
{
    private Button thisButton = null;
    private Text thisText = null;
    private ProfileGenerator profileGenerator = null;

    // Start is called before the first frame update
    void Start()
    {
        thisButton = GetComponent<Button>();
        thisText = transform.GetChild(0).GetComponent<Text>();

        profileGenerator = FindObjectOfType<ProfileGenerator>();

        thisButton.onClick.AddListener(() => profileGenerator.DefineCurrentProfile(thisText.text));
    }
}
