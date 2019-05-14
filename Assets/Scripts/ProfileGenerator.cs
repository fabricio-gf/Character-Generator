using UnityEngine;
using UnityEngine.UI;

public class ProfileGenerator : MonoBehaviour
{
    private static GameObject CurrentWindow = null;
    private GameObject OpenedWindow = null;

    [SerializeField] private InputField ProfileNameAnswer;
    [SerializeField] private Transform[] ProfileQuestions;

    private CharacterProfile profile = null;

    [SerializeField] private Questionnaire questionnaire;

    [SerializeField] private GameObject ErrorMessage;

    [SerializeField] private GameObject[] Windows;

    [SerializeField] private CharacterGenerator CharacterGenerator;

    private void Awake()
    {
        if(questionnaire != null)
        {
            for(int i = 0; i < ProfileQuestions.Length; i++)
            {
                ProfileQuestions[i].GetComponent<Text>().text = questionnaire.questions[i].QuestionName;
                for(int j = 0; j < questionnaire.questions[i].AnswerValues.Length; j++)
                {
                    ProfileQuestions[i].GetChild(0).GetChild(j).Find("Label").GetComponent<Text>().text = questionnaire.questions[i].AnswerValues[j];
                }
            }
        }

        SetActiveWindow(Windows[0]);
    }

    public void SetActiveWindow(GameObject newWindow)
    {
        CurrentWindow = newWindow;
    }

    public void ChangeActiveWindow(GameObject newWindow)
    {
        CurrentWindow?.SetActive(false);
        newWindow?.SetActive(true);
        CurrentWindow = newWindow;
    }

    public void OpenWindow(GameObject newWindow)
    {
        newWindow?.SetActive(true);
        OpenedWindow = newWindow;
    }

    public void SubmitAnswers()
    {
        string name = ProfileNameAnswer.text;

        int[] values = new int[ProfileQuestions.Length];

        int count;
        bool error = false;

        for(int i = 0; i < ProfileQuestions.Length; i++)
        {
            count = 0;
            for(int j = 0; j < ProfileQuestions[i].GetChild(0).childCount; j++)
            {
                if (ProfileQuestions[i].GetChild(0).GetChild(j).GetComponent<Toggle>().isOn)
                {
                    values[i] = j;
                    break;
                }
                count++;
            }
            print(count);
            if(count >= ProfileQuestions[i].GetChild(0).childCount)
            {
                error = true;
            }
        }

        if(error == true)
        {
            ShowErrorMessage();
        }
        else
        {
            CharacterGenerator.profileValues = values;

            CharacterProfile newProfile = new CharacterProfile(name, values);

            /*Debug.Log(Application.persistentDataPath + "\\" + name);
            string filePath = System.IO.Path.Combine(Application.persistentDataPath, name);
            FileReadWrite.WriteToBinaryFile(filePath, newProfile);*/

            ChangeActiveWindow(Windows[1]);
            //CharacterGenerator.NewGeneration();
            CharacterGenerator.NextGenerationBatch();
        }
    }

    void ShowErrorMessage()
    {
        ErrorMessage.SetActive(true);
    }
}
