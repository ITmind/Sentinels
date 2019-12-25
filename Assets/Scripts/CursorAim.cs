using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;



public class CursorAim : MonoBehaviour {

    public enum CursorTypeEnum
    {
        Collect,
        Avatar,
        Rock
    }

    public GameObject CollectCursor;
    public GameObject EnterAvatarCursor;
    public GameObject AvatarCursor;
    public GameObject RockCursor;

    public ResourceObject AvatarPerfab;
    public ResourceObject RockPerfab;

    GameObject resourceCursor;

    Player player;
    Vector3 viewportCenter = new Vector3(0.5f, 0.5f);
    Camera mainCamera;
    bool buttonPress;

    ResourceObject selectedRes;

    public CursorTypeEnum CursorType = CursorTypeEnum.Rock;

    public static CursorAim instance;

    // Use this for initialization
    void Start () {
        mainCamera = Camera.main;
        var goPlayer = GameObject.FindGameObjectWithTag("Player");
        if (goPlayer != null)
        {
            player = goPlayer.GetComponent<Player>();
        }

        OnChangeCollectCursor(true);
        instance = this;
    }


    // Update is called once per frame
    void Update()
    {
        //проверка есть ли вообще пересечение с чем либо
        var ray = mainCamera.ViewportPointToRay(viewportCenter);
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit))
        {
            HideCursor();
            return;
        }

        //пересечение есть. рисуем курсор в точке
        ShowCursor(hit);

        if (CrossPlatformInputManager.GetButtonUp("Fire1") && !buttonPress)
        {
            //Debug.Log("Fire1");
            Action(hit);
        }
        buttonPress = false;

    }

    void ShowCursor(RaycastHit hit)
    {
        Debug.DrawLine(player.transform.position, hit.point);
        //currentCursor
        bool isAviable = hit.distance <= player.MaxDistance;
        if (!isAviable)
        {
            HideCursor();
            return;
        }

        var res = hit.transform.GetComponent<ResourceObject>();
        //если объект в курсоре не ресурс
        if (res == null)
        {
            //здесь нужно дополнительно проверить возможность установки по углу и высоте
            if(Vector3.Angle(hit.normal, Vector3.up)>40f || hit.point.y>player.transform.position.y+0.5f)
            {
                HideCursor();
                return;
            }

            ShowResourceCursor();
            
            if (CursorType != CursorTypeEnum.Collect)
            {
                //print("putinpoint");
                ResourceObject.PutInPoint(resourceCursor, hit.point, true);
            }
        }
        else if(res.IsAviable)
        {
            selectedRes = res.GetAtop();
            //print(selectedRes);

            if (selectedRes.type == ResourceType.Rock && CursorType != CursorTypeEnum.Collect)
            {
                ShowResourceCursor();
                ResourceObject.PutInPoint(resourceCursor, hit.point, true);
            }
            else
            {
                var sp = mainCamera.WorldToScreenPoint(selectedRes.transform.position);
                CollectCursor.transform.position = sp;
                ShowCollectCursor();
            }
        }
        else
        {
            HideCursor();
        }
    }

    void Action(RaycastHit hit)
    {

        //if (resourceCursor!=null && !resourceCursor.activeSelf) return;

        if (selectedRes!=null)
        {
            selectedRes.Click(hit.point);
        }
        else
        {
            if (CursorType == CursorTypeEnum.Avatar)
            {
                ResourceObject.Create(instance.AvatarPerfab, hit.point);

            }
            else if (CursorType == CursorTypeEnum.Rock)
            {
                ResourceObject.Create(instance.RockPerfab, hit.point);
            }
        }
    }

    public void OnChangeAvatarCursor(bool value)
    {
        if (!value) return;
        if (CursorType == CursorTypeEnum.Avatar)
        {
            CrossPlatformInputManager.SetButtonUp("Fire1");
        }
        else
        {
            CursorType = CursorTypeEnum.Avatar;
            resourceCursor = AvatarCursor;
            AvatarCursor.SetActive(true);
            RockCursor.SetActive(false);
            buttonPress = true;
        }
    }

    public void OnChangeCollectCursor(bool value)
    {
        if (!value) return;
        if (CursorType == CursorTypeEnum.Collect)
        {
            CrossPlatformInputManager.SetButtonUp("Fire1");
        }
        else
        {
            CursorType = CursorTypeEnum.Collect;
            resourceCursor = null;
            AvatarCursor.SetActive(false);
            RockCursor.SetActive(false);
            buttonPress = true;
        }
    }

    public void OnChangRockCursor(bool value)
    {
        if (!value) return;
        if (CursorType == CursorTypeEnum.Rock)
        {
            CrossPlatformInputManager.SetButtonUp("Fire1");
        }
        else
        {
            CursorType = CursorTypeEnum.Rock;
            resourceCursor = RockCursor;
            AvatarCursor.SetActive(false);
            RockCursor.SetActive(true);
            buttonPress = true;
        }

    }


    void ShowCollectCursor()
    {
        if (resourceCursor != null)
        {
            resourceCursor.SetActive(false);
        }
        CollectCursor.SetActive(true);
    }

    void ShowResourceCursor()
    {
        selectedRes = null;

        if (resourceCursor != null)
        {
            resourceCursor.SetActive(true);
        }
        CollectCursor.SetActive(false);
    }

    void HideCursor()
    {
        //print("HideCursor");
        selectedRes = null;
        if (resourceCursor != null)
        {
            resourceCursor.SetActive(false);
        }

        CollectCursor.SetActive(false);
    }
}
