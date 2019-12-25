using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour {

    static public bool SetRobot = false;

    public GameObject RobotPerfab;
    public GameObject BlockPerfab;
    public TileMapGenerator Map;
    static public Player OwnPlayer;
    static public GameObject Diablo;
	
    //GUI
    float windowsHeight;
    float windowsWidth;
    public Texture2D RestartIcon;
    public Texture2D ExitIcon;
    public GUISkin Skin;
    bool gameIsEnd;

	void Start () {
        //инициализируем массив ресурсов

        //OwnPlayer = FindObjectsOfType<Player>().FirstOrDefault(p => p.Owner == OwnerEnum.Human);
        Diablo = GameObject.FindGameObjectWithTag("Diablo");
        if (OwnPlayer != null)
        {

            Map.RecalcAviable(OwnPlayer.transform.position);
        }
	}
		
	void Update () {

        if (Diablo == null)
        {
            Diablo = GameObject.FindGameObjectWithTag("Diablo");
        }

        if (OwnPlayer == null)
        {
            //OwnPlayer = FindObjectsOfType<Player>().FirstOrDefault(p => p.Owner == OwnerEnum.Human);
            if(OwnPlayer!=null) Map.RecalcAviable(OwnPlayer.transform.position);
        }

        if (Diablo == null || OwnPlayer == null)
        {
            gameIsEnd = true;
        }
        else
        {
            gameIsEnd = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.LoadLevel(0);
        }

	}

    public bool IsTargetAviable(GameObject player, Vector3 target)
    {      

        if (!Map.IsCellAviable(target.x,target.z) || OwnPlayer.transform.position == target)
        {
            return false;
        }

        return true;
    }

    #region Events
    public void OnSingleClick(GameObject target, Vector3 hitPoint)
    {
        
        
        var res = target.GetComponent<ResourceObject>();
        if (res != null)
        {
            //все действия можно производить, только если выше нет ничего и ресур активный.
            if (!IsTargetAviable(OwnPlayer.gameObject, target.transform.position))
            {
                return;
            }

            switch (res.type)
            {
                case ResourceType.Avatar:
                    //переедем
                    //OwnPlayer.MoveTo(res.transform.position);
                    //OwnPlayer.CurrentRobot = target;
                    Map.RecalcAviable(res.transform.position);
                    break;
                case ResourceType.Tree:
                    //соберем
                    //OwnPlayer.AbsorbResource(res);
                    break;
                case ResourceType.Rock:
                    //поставим
                    //OwnPlayer.SetRes(SetRobot,target.transform.position+Vector3.up);
                    break;
            }
        }
        else
        {
            var tile = target.GetComponent<Tile>();
            if (tile == null) return;
            var pos = Map.GetCoord(hitPoint);

            if (!IsTargetAviable(OwnPlayer.gameObject, pos))
            {
                return;
            }

            //OwnPlayer.SetRes(SetRobot, pos);

        }



    }

    public void OnDoubleClick(GameObject target)
    {
        //взять
        var res = target.GetComponent<ResourceObject>();
        if (res != null)
        {
            if (!IsTargetAviable(OwnPlayer.gameObject, target.transform.position))
            {
                return;
            }

            //OwnPlayer.AbsorbResource(res);
        }
    }

    public void OnMouseHold(GameObject target)
    {
        //?
    }

    #endregion
}
