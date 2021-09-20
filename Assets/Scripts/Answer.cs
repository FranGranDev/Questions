using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Answer : MonoBehaviour
{
    public QuestionData.Type Type;
    public int index { get; private set; }
    public string text;
    public bool answerTRUE;  //-------For Single and Multi
    public int droppedIndex; //-------For Image Drop

    private event OnClick OnAnswerClick;
    public delegate void OnClick(int index);

    [Header("Components")]
    public Image backGround;
    public TextMeshProUGUI answer;
    public RectTransform Rect;

    public void SetQuestion(string text, int index)
    {
        this.text = text;
        answer.text = text;
        this.index = index;
        switch(Type)
        {
            case QuestionData.Type.Image:
                OnAnswerClick += Main.active.TakeAnswer;
                droppedIndex = -1;
                break;
            case QuestionData.Type.Multi:
                OnAnswerClick += Main.active.SetToggleFor;
                break;
            case QuestionData.Type.Single:
                OnAnswerClick += Main.active.AnswerFor;
                break;
        }
    }

    public void ButtonClick()
    {
        OnAnswerClick.Invoke(index);
    }

    public void TurnTaken(bool on)
    {
        if(on)
        {
            Color temp = backGround.color;
            answer.raycastTarget = false;
            backGround.raycastTarget = false;
            backGround.color = new Color(temp.r, temp.g, temp.b, 0);
            transform.SetParent(Main.active.Mouse);
        }
        else
        {
            Color temp = backGround.color;
            backGround.raycastTarget = true;
            answer.raycastTarget = true;
            backGround.color = new Color(temp.r, temp.g, temp.b, 1);
            transform.SetParent(Main.active.QuestionContent);
        }
    } //-------For Image Drop
    public void TurnVisible(bool on)
    {
        answer.enabled = on;
        backGround.enabled = on;
    }
}
