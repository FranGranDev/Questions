using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Main : MonoBehaviour
{
    public static Main active;

    public int NowQuestionNum;
    public Answer[] answers;
    //----------Image Drop-----------
    public ImageTaken imageTaken;
    public void SetDroppedIndex(int i)
    {
        if (SelectedIndex == -1)
            return;
        answers[SelectedIndex].droppedIndex = i;
        //Debug.Log("Set Dropped Index " + i);
    }
    public void ClearDroppedIndex()
    {
        if (SelectedIndex == -1)
            return;
        answers[SelectedIndex].droppedIndex = -1;
        //Debug.Log("Clear Dropped Index");
    }
    private int SelectedIndex;
    private void SetSelectedIndex(int i)
    {
        SelectedIndex = i;
        //Debug.Log("Set Selected Index " + i);
    }
    //-------------Order-----------
    private int nowOrder;
    //-----------------------------
    public QuestionData[] questionData;


    [Header("Components")]
    public Transform Mouse;
    public Transform ImageContent;
    public Transform QuestionContent;
    public TextMeshProUGUI QuestionText;
    public ScrollRect QuestionScroll;
    [Header("Prefabs")]
    public Answer SingleItem;
    public Answer MultiItem;
    public Answer TakenItem;
    public Answer OrderItem;

    public void AnswerFor(int i)
    {
        answers[i].answerTRUE = true;

        ChechAnswer();
    }
    public void SetOrderFor(int i)
    {
        answers[i].SetOrder(nowOrder);
        nowOrder++;
    }
    public void SetToggleFor(int i)
    {
        answers[i].answerTRUE = !answers[i].answerTRUE;
    }
    public void TakeAnswer(int i)
    {
        SelectedIndex = i;
        QuestionScroll.StopMovement();
        answers[i].TurnTaken(true);
    }
    public void PutQuestion()
    {

    }

    public void SetQuestions()
    {
        QuestionData temp = questionData[NowQuestionNum];
        QuestionText.text = temp.Question;
        nowOrder = 0;
        switch(temp.QuestionType)
        {
            case QuestionData.Type.Image:
                {
                    if (temp.ImageInteract != null)
                    {
                        imageTaken = Instantiate(temp.ImageInteract, ImageContent);
                    }
                    else
                    {
                        Debug.Log("Отсутствует сслыка на картинку!");
                        return;
                    }

                    answers = new Answer[imageTaken.imageItem.Length];
                    for (int i = 0; i < answers.Length; i++)
                    {
                        answers[i] = Instantiate(TakenItem, QuestionContent);
                        answers[i].SetQuestion(imageTaken.imageItem[i].Name, i);
                    }
                }
                break;
            case QuestionData.Type.Multi:
                {
                    answers = new Answer[temp.Answer.Length];
                    for (int i = 0; i < temp.Answer.Length; i++)
                    {
                        answers[i] = Instantiate(MultiItem, QuestionContent);
                        answers[i].SetQuestion(temp.Answer[i].Text, i);
                    }
                }
                break;
            case QuestionData.Type.Single:
                {
                    answers = new Answer[temp.Answer.Length];
                    for (int i = 0; i < temp.Answer.Length; i++)
                    {
                        answers[i] = Instantiate(SingleItem, QuestionContent);
                        answers[i].SetQuestion(temp.Answer[i].Text, i);
                    }
                }
                break;
            case QuestionData.Type.Order:
                {
                    answers = new Answer[temp.Answer.Length];
                    for (int i = 0; i < temp.Answer.Length; i++)
                    {
                        answers[i] = Instantiate(OrderItem, QuestionContent);
                        answers[i].SetQuestion(temp.Answer[i].Text, i);
                    }
                }
                break;
        }

        RandomizePosition();
    }
    private void RandomizePosition()
    {
        for(int i = 0; i < answers.Length - 1; i++)
        {
            answers[i].transform.SetSiblingIndex(Random.Range(0, answers.Length));
        }
    }
    public void ClearQuestions()
    {
        QuestionText.text = "";
        for(int i = 0; i < QuestionContent.childCount; i++)
        {
            Destroy(QuestionContent.GetChild(i).gameObject);
        }
        answers = new Answer[0];
        if(ImageContent != null && ImageContent.childCount > 0)
        {
            Destroy(ImageContent.GetChild(0).gameObject);
        }
    }
    public void ChechAnswer()
    {
        QuestionData temp = questionData[NowQuestionNum];
        switch(temp.QuestionType)
        {
            case QuestionData.Type.Single:
                for (int i = 0; i < temp.Answer.Length; i++)
                {
                    if (temp.Answer[i].TRUE != answers[i].answerTRUE)
                    {
                        Failed();
                        return;
                    }
                }
                Done();
                break;
            case QuestionData.Type.Multi:
                for (int i = 0; i < temp.Answer.Length; i++)
                {
                    if (temp.Answer[i].TRUE != answers[i].answerTRUE)
                    {
                        Failed();
                        return;
                    }
                }
                Done();
                break;
            case QuestionData.Type.Image:
                for(int i = 0; i < temp.Answer.Length; i++)
                {
                    if (answers[i].droppedIndex != imageTaken.imageItem[i].index)
                    {
                        Failed();
                        return;
                    }
                }
                Done();
                break;
            case QuestionData.Type.Order:
                for(int i = 0; i < temp.Answer.Length; i++)
                {
                    if(temp.Answer[i].TRUE && answers[i].order != i)
                    {
                        Failed();
                        return;
                    }
                    else if(!temp.Answer[i].TRUE && answers[i].order != -1)
                    {
                        Failed();
                        return;
                    }
                }
                Done();
                break;
        }
        
    }
    public void Restart()
    {
        ClearQuestions();
        SetQuestions();
    }
    private void Done()
    {
        if(NowQuestionNum + 1 < questionData.Length)
        {
            NowQuestionNum++;
            ClearQuestions();
            SetQuestions();
        }
        else
        {
            NowQuestionNum = 0;
            ClearQuestions();
            SetQuestions();
        }

        Debug.Log("Успех!");
    }
    private void Failed()
    {
        Restart();
        Debug.Log("Неправильно");
    }

    private void Awake()
    {
        active = this;
    }
    private void Start()
    {
        SetQuestions();
    }
    private void Update()
    {
        Mouse.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (questionData[NowQuestionNum].QuestionType == QuestionData.Type.Image)
        {
            if (Input.GetMouseButtonUp(0))
            {
                //Совместить текст с картинкой
                if (SelectedIndex != -1 && answers[SelectedIndex].droppedIndex != -1)
                {
                    imageTaken.DropAnswer(answers[SelectedIndex]);
                    SelectedIndex = -1;
                }
                //Вернуть текст в колону ответов
                if (SelectedIndex != -1 && answers[SelectedIndex].droppedIndex == -1)
                {
                    answers[SelectedIndex].TurnTaken(false);
                }
            }
        }
    }
}

[System.Serializable]
public class QuestionData
{
    public string Name;

    public enum Type {Single, Multi, Image, Order}
    [Header("Тип вопросов")]
    public Type QuestionType;

    [Header("Картинка")]
    public ImageTaken ImageInteract;

    [Header("Вопрос")]
    public string Question;

    [Header("Массив ответов")]
    public QuestionItem[] Answer;
    public int RightAnswerNum()
    {
        return 0;
    }
}
[System.Serializable]
public struct QuestionItem
{
    public string Text;
    public bool TRUE;
}