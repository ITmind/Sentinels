using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ResPanelUI : MonoBehaviour {
    public Sprite avatarSprite;
    public Sprite rockSprite;
    public Sprite oneScoreSprite;
    public Sprite manySprite;
    int spriteSize = 32;
   
	// Use this for initialization
	void Start () {
        Refresh(0);
    }
	
    public void Refresh(int score)
    {
        Clear();
        //int pos = 9;

        int numRobot = Mathf.FloorToInt(score / 3);
        int numRock = Mathf.FloorToInt((score % 3) / 2);
        int numOneScore = score - numRock * 2 - numRobot * 3;
        int pos = (numRobot + numRock + numOneScore)-1;
        if (pos < 0) return;

        bool isMany = false;
         //всего выводи 10 позиций. если колво > 10 тогда последней пишем спец спрайт
        if (pos > 9)
        {
            isMany = true;
            pos = 10;
            AddResToPanel(manySprite, 0);
            pos--;
        }

        for (int i = 0; i < numRobot; i++)
        {
            if ((isMany && pos == 0) || pos < 0) break;
            AddResToPanel(avatarSprite,pos);
            pos--;
        }

        for (int i = 0; i < numRock; i++)
        {
            if ((isMany && pos == 0) || pos<0) break;
            AddResToPanel(rockSprite, pos);
            pos--;
        }

        for (int i = 0; i < numOneScore; i++)
        {
            if ((isMany && pos == 0) || pos < 0) break;
            AddResToPanel(oneScoreSprite, pos);
            pos--;
        }
    }

    void AddResToPanel(Sprite sprite,int pos)
    {
        var newImage = new GameObject("res", typeof(Image));
        newImage.transform.SetParent(transform, false);
        var rect = newImage.GetComponent<RectTransform>();
        rect.anchorMax = new Vector2(1, 0.5f);
        rect.anchorMin = new Vector2(1, 0.5f);
        rect.anchoredPosition = new Vector2(-16 - pos * spriteSize, 0);
        //rect.localPosition = new Vector2(pos * spriteSize, 0);
        rect.sizeDelta = new Vector2(spriteSize, spriteSize);
        var img = newImage.GetComponent<Image>();
        img.sprite = sprite;
    }

    void Clear()
    {
        foreach (Transform item in transform)
        {
            Destroy(item.gameObject);
        }
    }
}
