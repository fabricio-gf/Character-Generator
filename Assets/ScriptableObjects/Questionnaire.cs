using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Questionnaire", menuName = "Questionnaire")]
public class Questionnaire : ScriptableObject
{
    public Questions[] questions;
}

[System.Serializable]
public class Questions
{
    public string QuestionName;
    public string[] AnswerValues;
}
