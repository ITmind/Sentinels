using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.Events;
using System;
using DG.Tweening;

[Serializable]
public class MyOnScoreChanged: UnityEvent<int>
{

}

public class Player : MonoBehaviour
{

    public float MaxDistance = 10f;    
    private int score;
    private GameObject currentAvatar;

    public int Score
    {
        get { return score; }
        set
        {
            if (value < 0)
            {
                score = 0;
            }
            else
            {
                score = value;
            }
            OnScoreChanged.Invoke(score);
        }
    }

    [SerializeField] private MouseLook m_MouseLook;

    public MyOnScoreChanged OnScoreChanged;

    private Camera m_Camera;

    void Start ()
	{
        m_Camera = Camera.main;
        m_MouseLook.Init(transform, m_Camera.transform);
        Score = 10;
    }

    void Update()
    {
        RotateView();
    }

    private void RotateView()
    {
        m_MouseLook.LookRotation(transform, m_Camera.transform);
    }

    public void GoToAvatar(GameObject avatar)
    {
        //transform.position = avatar.transform.position;
        //avatar.SetActive(false);

        if(currentAvatar!=null) currentAvatar.SetActive(true);

        currentAvatar = avatar;
        transform.DOMove(avatar.transform.position,3f).SetSpeedBased(true).SetEase(Ease.InOutCubic);
        //LeanTween.move(gameObject, avatar.transform.position, 1f).setEase(LeanTweenType.easeInOutCubic).setOnComplete(moveComplite);
    }

    private void moveComplite()
    {
        currentAvatar.SetActive(false);
        //throw new NotImplementedException();
    }
}
