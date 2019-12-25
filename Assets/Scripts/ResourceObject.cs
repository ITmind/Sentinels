using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class ResourceObject : MonoBehaviour, IPointerClickHandler
{
	public int score;
    public ResourceType type;    
    public bool IsCursor = false;
    private GameObject enterCursor;
    private GameObject canvas;
    private Collider col;

    Player player;
    Transform tr;

    public bool IsVisble
    {
        get
        {
            return !IsCursor && IsVisible();
        }
    }

    public bool IsAviable
    {
        get
        {
            return col.bounds.min.y < player.transform.position.y;
            //return GetAtop() == this;
        }
    }

    public void Start()
    {
        //Debug.Log(this);
        var goPlayer = GameObject.FindGameObjectWithTag("Player");
        if (goPlayer != null)
        {
            player = goPlayer.GetComponent<Player>();
        }
        tr = transform;
        col = GetComponent<Collider>();

        //CreateCursors();
    }

    private void CreateCursors()
    {
        canvas = GameObject.FindGameObjectWithTag("Canvas");
        if (canvas != null)
        {
            if (type == ResourceType.Avatar)
            {
                enterCursor = Instantiate(ObjectPool.instance.EnterCursorPerfab);
                enterCursor.transform.SetParent(canvas.transform);
                enterCursor.SetActive(false);
            }
        }
    }

    private void ShowCursor()
    {

        //Physics.Linecast(transform.position, player.transform.position);
        if (enterCursor != null)
        {
            enterCursor.SetActive(IsAviable && IsVisble);
        }
    }

    

    void Update()
    {
        
        //ShowCursor();
        

        //if (enterCursor!=null && enterCursor.activeSelf)
        //{
        //    enterCursor.transform.position = Camera.main.WorldToScreenPoint(transform.position+Vector3.up*1f);
        //}
    }

    //void OnDisable()
    //{
    //    DestroyObject(enterCursor);
    //}

    void OnDestroy()
    {
        DestroyObject(enterCursor);
    }


    private bool IsVisible()
    {
        var screenPoint = Camera.main.WorldToViewportPoint(transform.position);
        return screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Click(eventData.pointerPressRaycast.worldPosition);
    }

    public void Click(Vector3 point)
    {
        if (CursorAim.instance.CursorType == CursorAim.CursorTypeEnum.Collect)
        {
            Collect();
        }
        else 
        {
            if (type == ResourceType.Avatar)
            {
                //вселяемся
                player.GoToAvatar(gameObject);                
            }
            else if (type == ResourceType.Rock && CursorAim.instance.CursorType == CursorAim.CursorTypeEnum.Rock)
            {
                Create(CursorAim.instance.RockPerfab, point);

            }  
        }

    }
    /// <summary>
    /// удаляет самый верхний ресурс. Если нет верхнего то текущий.
    /// </summary>
    public void Collect()
    {
        if (player == null) return;
        var hit = GetAtop();
        if (hit.transform == null)
        {
            CollectRes(this);
            return;
        }

        var res = hit.transform.GetComponent<ResourceObject>();
        if (res == null) res = this;
        CollectRes(res);

    }

    private void CollectRes(ResourceObject res)
    {
        player.Score += res.score;
        DestroyObject(res.gameObject);
    }


    /// <summary>
    /// сверху в центр текущего объекта
    /// </summary>
    /// <returns></returns>
    public ResourceObject GetAtop()
    {
        var temp = GetComponentsInChildren<ResourceObject>();
        for (int i = 0; i < temp.Length; i++)
        {
            if(temp[i] != this)
            {
                return temp[i].GetAtop();
            }
        }

        return this;
      
    }

    /// <summary>
    /// ставим сверху текущего ресурса
    /// </summary>
    /// <param name="newRes"></param>
    public void PutAtop(GameObject newRes, bool isCursor)
    {
        var upHit = GetAtop();
        //получим верх по центру для найденного ресурса
        var thisCollider = upHit.GetComponent<Collider>();
        var bounds = thisCollider.bounds;
        Vector3 upPoint = new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
        //найдем половину высоты нового ресурая
        var newCollider = newRes.GetComponent<Collider>();
        //Debug.Log("pivot " + upHit.transform.position + " bounds center " + bounds.center+ " max "+bounds.max+" min "+bounds.min);

        //вычислим разницу между пивот и центром боундс
        var dy = newRes.transform.position.y - newCollider.bounds.center.y;
        if (!isCursor)
        {            
            newRes.transform.SetParent(upHit.transform);         
        }

        newRes.transform.position = upPoint + Vector3.up * (newCollider.bounds.extents.y + dy);
    }

    /// <summary>
    /// ставим в точке
    /// </summary>
    /// <param name="point">точка в которой нажали</param>
    public static void PutInPoint(GameObject gameObject, Vector3 point, bool isCursor)
    {
        int layerMask = 1 << 2;
        layerMask = ~layerMask;
        //для начала проверим в точке пересечение со сферой, т.к. куда нажали пальцем точно не определить.

        var colliders = Physics.OverlapSphere(point, ObjectPool.instance.TouchRadius, layerMask);
        ResourceObject res = null;
        //найдем, есть ли ресуры в ограничевающей сфере
        for (int i = 0; i < colliders.Length; i++)
        {
            res = colliders[i].GetComponent<ResourceObject>();
            if(res!=null) break;
        }

        //если ресурс
        if (res != null)
        {
            //print("res");
            //и этот ресурс камень
            if (res.type == ResourceType.Rock)
            {
                //то ставим над ним
                res.PutAtop(gameObject,isCursor);
            }
        }
        else
        {
            //иначе ставим на земле
            var newCollider = gameObject.GetComponent<Collider>();

            //вычислим разницу между пивот и центром боундс
            var dy = gameObject.transform.position.y - newCollider.bounds.center.y;

            gameObject.transform.position = point + Vector3.up * (newCollider.bounds.extents.y + dy);
        }

    }

    public static void Create(ResourceObject perfab, Vector3 point)
    {
        Player player = null;

        var goPlayer = GameObject.FindGameObjectWithTag("Player");
        if (goPlayer != null)
        {
            player = goPlayer.GetComponent<Player>();
        }
        if (player == null) return;

        //нет ресурсов
        if (player.Score - perfab.score < 0)
        {
            return;
        }

        var go = Instantiate(perfab.gameObject);
        var res = go.GetComponent<ResourceObject>();
        if (res == null) return;
        res.Start();

        PutInPoint(res.gameObject,point, false);
        player.Score -= res.score;       

    }
}

