using UnityEngine;
using UnityEngine.UI;

public class ProfileGenerator : MonoBehaviour
{
    

    [SerializeField] private InputField ProfileNameField;
    [SerializeField] private Transform[] ProfileQuestions;

    private CharacterProfile profile = null;

    private string profileName = null;

    [SerializeField] private Questionnaire questionnaire = null;

    [SerializeField] private GameObject ErrorMessage = null;
    [SerializeField] private GameObject NameErrorMessage = null;

    [SerializeField] private CharacterGenerator characterGenerator = null;

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

    public void ResetInputField()
    {
        ProfileNameField.text = "";
    }

    public void CheckInputField()
    {
        if(ProfileNameField.text == "")
        {
            NameErrorMessage.SetActive(true);
        }
        else
        {
            NameErrorMessage.SetActive(false);

        }
    }

    public void DefineNewProfileName()
    {
        if (ProfileNameField.text == "")
        {
            NameErrorMessage.SetActive(true);
        }
        else
        {
            NameErrorMessage.SetActive(false);
            profileName = ProfileNameField.text;

            string filePath = System.IO.Path.Combine(Application.persistentDataPath, "Tables", profileName);
            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(filePath);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            characterGenerator.SaveTables(profileName, true);

            windowBehaviours.ChangeActiveWindow(1);
        }
    }
    
    public void SaveNewProfile(CharacterProfile profile)
    {
        string filePath = System.IO.Path.Combine(Application.persistentDataPath, "Profiles", profileName);
        
        FileReadWrite.WriteToBinaryFile(filePath, profile);
    }

    public void DefineCurrentProfile(string name)
    {
        profileName = name;
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

            characterGenerator.LoadTables(profileName);

            characterGenerator.LoadGeneration(filePath);

            characterGenerator.NextGenerationBatch();
        }
        else
        {
            Debug.Log("No profile selected");
        }
    }
}
