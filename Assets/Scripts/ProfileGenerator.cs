using UnityEngine;
using UnityEngine.UI;

public class ProfileGenerator : MonoBehaviour
{
    

    [SerializeField] private InputField ProfileNameAnswer;
    [SerializeField] private Transform[] ProfileQuestions;

    private CharacterProfile profile = null;

    private string profileName = null;

    [SerializeField] private Questionnaire questionnaire;

    [SerializeField] private GameObject ErrorMessage;

    [SerializeField] private CharacterGenerator characterGenerator;

    [SerializeField] private WindowBehaviours windowBehaviours = null;

    private void Awake()
    {
        if (questionnaire != null)
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
    }

    public void SubmitAnswers()
    {

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

            CharacterProfile newProfile = new CharacterProfile(profileName, values);

            SaveNewProfile(newProfile);

            windowBehaviours.ChangeActiveWindow(2);
            //CharacterGenerator.NewGeneration();

            string filePath = System.IO.Path.Combine(Application.persistentDataPath, "Generations", profileName);
            filePath += "_Generation";
            characterGenerator.DefineGenerationPath(filePath);

            characterGenerator.NextGenerationBatch();
        }
    }

    void ShowErrorMessage()
    {
        ErrorMessage.SetActive(true);
    }

    public void DefineNewProfileName()
    {
        profileName = ProfileNameAnswer.text;
        Debug.Log("Profile name: " + profileName);
    }

    public void SaveNewProfile(CharacterProfile profile)
    {
        Debug.Log("SaveNewProfile");

        Debug.Log(Application.persistentDataPath + "/" + profileName);

        string filePath = System.IO.Path.Combine(Application.persistentDataPath, "Profiles", profileName);
        
        FileReadWrite.WriteToBinaryFile(filePath, profile);
    }

    public void DefineCurrentProfile(string name)
    {
        profileName = name;
        Debug.Log("Profile name: " + profileName);

    }

    public void LoadExistingProfile()
    {
        if (profileName != null)
        {
            string filePath = System.IO.Path.Combine(Application.persistentDataPath, "Profiles", profileName);
            CharacterProfile existingProfile = FileReadWrite.ReadFromBinaryFile<CharacterProfile>(filePath);

            CharacterGenerator.profileValues = existingProfile.ProfileValues;

            //load GA progress
            windowBehaviours.ChangeActiveWindow(2);

            filePath = System.IO.Path.Combine(Application.persistentDataPath, "Generations", profileName);
            filePath = filePath + "_Generation";

            characterGenerator.LoadGeneration(filePath);

            characterGenerator.NextGenerationBatch();
        }
        else
        {
            Debug.Log("No profile selected");
        }
    }
}
