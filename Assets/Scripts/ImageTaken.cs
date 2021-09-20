using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;

public class ImageTaken : MonoBehaviour
{
    public ImageItem[] imageItem;

    public void DropAnswer(Answer answer)
    {
        int index = answer.droppedIndex;
        imageItem[index].answetText.text = answer.text;
        imageItem[index].answerDropped = true;

        answer.transform.SetParent(imageItem[index].answetText.transform);
        answer.TurnVisible(false);
        
    }

    public void OnPoinertEnter(int i)
    {
        Main.active.SetDroppedIndex(i);
    }
    public void OnPointerExit()
    {
        Main.active.ClearDroppedIndex();
    }
}
[System.Serializable]
public class ImageItem
{
    public string Name;
    public int index;
    public bool answerDropped;
    public TextMeshProUGUI answetText;
    
}
