using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using System.Linq;

public class Cell
{
    private int _height;

    public Vector2 Coord;
    public GameObject AviablePlane;

    public int Height
    {
        get
        {
            return _height;
        }
        set
        {
            if (value > 9) _height = 9;
            else _height = value;
        }
    }

    public Vector3 CellPos
    {
        get
        {
            return new Vector3(Coord.x, Height, Coord.y);
        }
    }

    public Rect CellRect
    {
        get
        {
            return new Rect(Coord.x - 0.5f, Coord.y - 0.5f, 1f, 1f);
        }
    }


    public Vector3 LeftUp
    {
        get
        {
            return new Vector3(Coord.x - 0.5f, Height, Coord.y - 0.5f);
        }
    }

    public Vector3 LeftBottom
    {
        get
        {
            return new Vector3(Coord.x - 0.5f, Height, Coord.y + 0.5f);
        }
    }

    public Vector3 RightUp
    {
        get
        {
            return new Vector3(Coord.x + 0.5f, Height, Coord.y - 0.5f);
        }
    }

    public Vector3 RightBottom
    {
        get
        {
            return new Vector3(Coord.x + 0.5f, Height, Coord.y + 0.5f);
        }
    }

    public void SetAvaible(bool aviable)
    {
        if (AviablePlane != null)
        {
            AviablePlane.SetActive(aviable);
        }
    }

    public bool IsPointInCell(float x, float y)
    {
        return CellRect.Contains(new Vector2(x,y));
    }
}

public class TileMapGenerator : MonoBehaviour
{

	public GameObject Brick = null;
	public GameObject Resource1 = null;
	public GameObject Resource2 = null;
	public GameObject Emeny = null;
	public GameObject Player = null;
	public GameObject RobotPerfab = null;
	public GameObject AviableBrickPerfab = null;
    List<Cell> CellMap;

	public int numResource = 10;

	public int MapWidth = 15;
	public int MapHeight = 15;

	int[,] resmap;

    void Start()
    {
        int size = PlayerPrefs.GetInt("size");
        MapHeight = 10 * size;
        MapWidth = 10 * size;
        numResource = 10 * size * size;
        CellMap = new List<Cell>();

        Generate();        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Generate()
    {
        Clear();
        InitCellMap();
        GenerateHeightMap();

        BuildMesh();
        //CombineMesh("Brick");
        SetPlayer();
        SetEnemy();
        GenerateResource();
        GenerateAviablePlane();
    }

	#region MapGenerator
	
	

    void BuildMesh()
    {
        List<Vector3> vertices = new List<Vector3>(MapHeight * MapWidth * 4);
        List<Vector3> normals = new List<Vector3>(MapHeight * MapWidth * 4);
        List<Vector2> uv = new List<Vector2>(MapHeight * MapWidth * 4);
        List<int> triangles = new List<int>(MapHeight * MapWidth * 6);

        int faceNum = 0;
        int colIndex = 0;
        for (int i = 0; i < CellMap.Count; i++)
        {
            //РїСЂРѕРІРµСЂРёРј СЃРїСЂР°РІР°, РµСЃР»Рё РµСЃС‚СЊ Рё РІС‹С€Рµ, С‚Рѕ РїСЂРёСЃРѕРµРґРёРЅРёРј
            int rightCell = i - 1;
            if (rightCell >= 0 && colIndex != 0)
            {
                if (CellMap[rightCell].Height != CellMap[i].Height)
                {
                    AddFace(vertices, normals, uv, triangles, faceNum, CellMap[rightCell].RightUp, CellMap[i].LeftUp,
                                                                        CellMap[i].LeftBottom, CellMap[rightCell].RightBottom);
                    faceNum++;
                }
            }
            else if (rightCell < 0 || colIndex == 0)
            {
                //РєСЂР°Р№РЅСЏСЏ СЏС‡РµР№РєР°
                if (CellMap[i].Height > 0)
                {
                    //РµСЃР»Рё РІС‹С€Рµ 0, С‚Рѕ СЃРґРµР»Р°РµРј СЃС‚РµРЅРєСѓ
                    AddFace(vertices, normals, uv, triangles, faceNum, CellMap[i].LeftUp - Vector3.up * CellMap[i].Height, CellMap[i].LeftUp,
                                                                        CellMap[i].LeftBottom, CellMap[i].LeftBottom- Vector3.up * CellMap[i].Height);
                    faceNum++;
                }
            }

            ////РїСЂРѕРІРµСЂРёРј СЃРЅРёР·Сѓ, РµСЃР»Рё РµСЃС‚СЊ Рё РІС‹С€Рµ, С‚Рѕ РїСЂРёСЃРѕРµРґРёРЅСЏРµРј
            int bottomCell = i - MapHeight;
            if(bottomCell >= 0)
            {                
                if (CellMap[bottomCell].Height != CellMap[i].Height)
                {
                    //Debug.Log("bottom = " + bottomCell + " orig=" + i);
                    AddFace(vertices, normals, uv, triangles, faceNum, CellMap[bottomCell].LeftBottom, CellMap[bottomCell].RightBottom,
                                                                        CellMap[i].RightUp, CellMap[i].LeftUp);
                    faceNum++;
                }
            }
            else if(bottomCell < 0)
            {
                //РєСЂР°Р№РЅСЏСЏ СЏС‡РµР№РєР°
                if (CellMap[i].Height > 0)
                {
                    //РµСЃР»Рё РІС‹С€Рµ 0, С‚Рѕ СЃРґРµР»Р°РµРј СЃС‚РµРЅРєСѓ
                    AddFace(vertices, normals, uv, triangles, faceNum, CellMap[i].LeftUp - Vector3.up * CellMap[i].Height, CellMap[i].RightUp - Vector3.up * CellMap[i].Height,
                                                                        CellMap[i].RightUp, CellMap[i].LeftUp);
                    faceNum++;
                }
            }

            //Рё РЅР°СЂРёСЃСѓРµРј РїСЂРѕСЃС‚Рѕ РїР»РѕСЃРєРѕСЃС‚СЊ.
            AddFace(vertices, normals,uv, triangles,faceNum, CellMap[i].LeftUp,CellMap[i].RightUp, CellMap[i].RightBottom, CellMap[i].LeftBottom);
            faceNum++;

            colIndex++;
            if (colIndex == MapHeight)
            {
                colIndex = 0;
            }
        }

        for (int i = 0; i < faceNum*4; i++)
        {
            normals.Add(Vector3.up);
            //normals[i] = Vector3.up;
        }

        Mesh mesh = new Mesh();
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();
        mesh.Optimize();
        

        MeshFilter mf = GetComponent<MeshFilter>();
        mf.mesh = mesh;
        MeshCollider mc = GetComponent<MeshCollider>();
        if (mc==null)
        {
            mc = gameObject.AddComponent<MeshCollider>();
        }

        mc.sharedMesh = mesh;
        

    }

    void AddFace(List<Vector3> vertices, List<Vector3> normals, List<Vector2> uv, List<int> triangles, int faceNum, Vector3 upLeft, Vector3 upRight, Vector3 bottomRight, Vector3 bottomLeft)
    {
        int tl = faceNum * 4;
        vertices.Add(upLeft);
        vertices.Add(upRight);
        vertices.Add(bottomRight);
        vertices.Add(bottomLeft);
        //Debug.Log("face num=" + faceNum);
        //Debug.Log(upLeft);
        //Debug.Log(upRight);
        //Debug.Log(bottomRight);
        //Debug.Log(bottomLeft);


        uv.Add(new Vector2(0f, 1f));
        uv.Add(new Vector2(1f, 1f));
        uv.Add(new Vector2(1f, 0f));
        uv.Add(new Vector2(0f, 0f));


        triangles.Add(tl + 0);
        triangles.Add(tl + 2);
        triangles.Add(tl + 1);

        triangles.Add(tl + 0);
        triangles.Add(tl + 3);
        triangles.Add(tl + 2);

    }
    void InitCellMap()
    {
        CellMap.Clear();
        resmap = new int[MapHeight, MapWidth];
        for (int y = 0; y < MapHeight; y++)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                //СѓРєР°Р·С‹РІР°РµРј СЃРµСЂРµРґРёРЅСѓ СЏС‡РµР№РєРё
                CellMap.Add(new Cell { Coord = new Vector2(x+0.5f, y+0.5f) }); 
                resmap[x, y] = 0;
            }
        }
    }
    void GenerateHeightMap()
    {
        //РІРѕР·РІС‹С€РµРЅРѕСЃС‚Рё
        int num = Mathf.RoundToInt(MapWidth * MapHeight * 0.5f);

        for (int i = 0; i < num; i++)
        {
            int x = UnityEngine.Random.Range(0, MapWidth - 1);
            int y = UnityEngine.Random.Range(0, MapHeight - 1);
            CellMap[x + y * MapWidth].Height += UnityEngine.Random.Range(0, 3);
        }

        num = Mathf.RoundToInt(MapWidth*MapHeight*0.05f); //5% РіРѕСЂ

        for (int i = 0; i < num; i++)
        {
            int x = UnityEngine.Random.Range(0, MapWidth - 1);
            int y = UnityEngine.Random.Range(0, MapHeight - 1);
            CellMap[x + y * MapWidth].Height += 4;          
        }

        if (CellMap[0].Height > 2)
        {
            CellMap[0].Height = 2;
        }

        CellMap[1].Height = 0;
        CellMap[MapWidth].Height = 0;
    }

    public void GenerateAviablePlane()
    {
        foreach (var item in CellMap)
        {
            GameObject aviableObject = (GameObject)Instantiate(AviableBrickPerfab);
            aviableObject.transform.parent = gameObject.transform;
            aviableObject.transform.position = new Vector3(item.Coord.x, item.Height + 0.05f, item.Coord.y);
            aviableObject.SetActive(false);
            item.AviablePlane = aviableObject;
        }
    }

    void SetPlayer()
    {
        //РїРѕСЃС‚Р°РІРёРј РёРіСЂРѕРєР°

        if (Player != null)
        {
            //Cell randomCell = CellMap.FirstOrDefault(c => c.Height == CellMap.Min(h => h.Height));
            //РІСЃРµРіРґР° Р±СѓРґРµРј СЃС‚Р°РІРёС‚СЊ РІ 0,0. С‡С‚Рѕ Р±С‹ Сѓ РёРіСЂРѕРєР° Р±С‹Р»Рѕ РјРµСЃС‚Рѕ РґР»СЏ РґРµР№СЃС‚РІРёР№
            Cell randomCell = CellMap[0];
            
            GameObject robot = (GameObject)Instantiate (RobotPerfab);
            GameObject player = (GameObject)Instantiate (Player);

            robot.transform.parent = transform;
            player.transform.parent = transform;

            robot.transform.position = randomCell.CellPos + Vector3.up * 0.5f;
            player.transform.position = randomCell.CellPos + Vector3.up * 0.5f;
            resmap[(int)randomCell.Coord.x, (int)randomCell.Coord.y] = 3;

        }
    }

    void SetEnemy()
    {        
        if (Emeny != null)
        {
            Cell randomCell = CellMap.FirstOrDefault(c => c.Height == CellMap.Max(h => h.Height));
            GameObject enemy = (GameObject)Instantiate (Emeny);
            enemy.transform.position = randomCell.CellPos+Vector3.up*0.5f;
            enemy.transform.parent = gameObject.transform;
        }

    }

	public Vector3 GetFreeRandomCoord ()
	{
		while (true) {
			int x = UnityEngine.Random.Range (0, MapWidth);
			int y = UnityEngine.Random.Range (0, MapHeight);
			if (resmap [x, y] > 0) {
				continue;
			}
			return CellMap[x+y*MapWidth].CellPos;
		}
	}

	public void SetObjectInCoord (Vector3 coordAbsolute, TypeObject type)
	{
		float delta = 0;
		Vector3 coordReal = new Vector3 (coordAbsolute.x, coordAbsolute.y, coordAbsolute.z);
		
		//СЃС‚Р°РІРёРј
		GameObject res = null;
		switch (type) {
		case TypeObject.Resource1:
			res = (GameObject)Instantiate (Resource1);
			delta = -0.5f;
			break;
		case TypeObject.Resource2:
			res = (GameObject)Instantiate (Resource2);
			delta = 1f;
			break;
		default:
			break;
		}
		

		res.transform.position = coordReal;
		res.transform.Translate (Vector3.up * (1 + delta), Space.World);
		res.transform.parent = gameObject.transform;
		resmap [(int)coordAbsolute.x, (int)coordAbsolute.y] = 1;
	}
	
	public void SetObjectInRandomCoord (TypeObject type)
	{
		SetObjectInCoord (GetFreeRandomCoord (), type);
	}
	
	void GenerateResource ()
	{
		//РЅР° РєР°Р¶РґС‹Р№ СЃР»РѕРµ Р±СѓРґРµРј СЂР°Р·РјРµС‰Р°С‚СЊ РѕРїСЂРµРґРµР»РµРЅРЅРѕРµ РєРѕР»РёС‡РµСЃС‚РІРѕ СЂРµСЃСѓСЂСЃРѕРІ
		//РЅР° РїСЂРµРІРѕРј СЃР°РјРѕРµ Р±РѕР»СЊС€Рµ РєРѕР»РёС‡РµСЃС‚РІРѕ, РЅР° РІС‚РѕСЂРѕРј РїРѕРјРµРЅСЊС€Рµ Рё С‚.Рґ.
		//РІСЃРµРіРѕ СЃР»РѕРµРІ 10.
		//int x, y;
		float delta = 0;
		//РїРѕР»СѓС‡РёРј СЃР»СѓС‡Р°Р№РЅС‹Рµ РєРѕРѕСЂРґРёРЅР°С‚С‹
		//var randomGenerator = new System.Random ();
		
		for (int i = numResource; i > -1; i--) {
			Vector3 coordAbsolute = GetFreeRandomCoord ();
			Vector3 coordReal = new Vector3 (coordAbsolute.x, coordAbsolute.y, coordAbsolute.z);
			if ((i / (coordAbsolute.y + 1)) >= 1) {
				//СЃС‚Р°РІРёРј
				GameObject res = null;
				if (UnityEngine.Random.Range (0, 2) == 0) {
					res = (GameObject)Instantiate (Resource1);
					delta = -0.5f;
				} else {
					res = (GameObject)Instantiate (Resource2);
					delta = 1f;
				}
					
				res.transform.position = coordReal;
				res.transform.Translate (Vector3.up * (1 + delta), Space.World);
				res.transform.parent = gameObject.transform;
				resmap [(int)coordAbsolute.x, (int)coordAbsolute.z] = 1;
				//Debug.Log ("set to layer " + layer.ToString () + " at " + x.ToString () + "," + y.ToString ());
				//break;
			}
		}
	}
	

	
    private void CombineMesh(string Tag)
    {
        var groundObj = GameObject.FindGameObjectsWithTag(Tag);
        if (groundObj.Length < 2) return;

        var plane = new GameObject("All" + Tag) { isStatic = true };
        Material[] mat = null;

        plane.AddComponent<MeshFilter>();
        plane.AddComponent<MeshRenderer>();
        //if (isEarth)
        //    plane.AddComponent<Earth>();



        var combine = new CombineInstance[groundObj.Length];
        for (int i = 0; i < combine.Length; i++)
        {
            var meshFilter = groundObj[i].GetComponent<MeshFilter>();
            combine[i].mesh = meshFilter.sharedMesh;
            combine[i].transform = meshFilter.transform.localToWorldMatrix;
            mat = groundObj[i].GetComponent<Renderer>().sharedMaterials;
            DestroyImmediate(groundObj[i]);
        }

        plane.transform.position = new Vector3(0, 0, 0);
        plane.transform.parent = transform;
        plane.GetComponent<MeshFilter>().sharedMesh = new Mesh();
        plane.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine, true);
        plane.GetComponent<MeshRenderer>().materials = mat;
        plane.AddComponent<MeshCollider>();
        plane.AddComponent<Tile>();
        //plane.tag = Tag;

    }

    public void Clear()
    {
        List<GameObject> del = new List<GameObject>();
        foreach (Transform child in transform)
        {
            del.Add(child.gameObject);
            
        }

        foreach (var item in del)
        {
            DestroyImmediate(item);
        }
        
    }
	#endregion
	
	#region AviableObject
    public void RecalcAviable(Vector3 forPoint)
    {
        foreach (var cell in CellMap)
        {
            cell.SetAvaible(false);
        }

        int mask = ~(1 << 8 | 1 << 2);

        int playerLayer = Mathf.RoundToInt(forPoint.y);
        var aviableCells = CellMap.Where(c => c.Height <= playerLayer);
        foreach (var cell in aviableCells)
        {
            if (!Physics.Linecast(forPoint, cell.CellPos + Vector3.up * 1.2f, mask))
            {
                cell.SetAvaible(true);
            }
        }
    }

	
	#endregion

    public Vector3 GetCoord(Vector3 hit)
    {
        int x = Mathf.FloorToInt(hit.x);
        int y = Mathf.FloorToInt(hit.z);

        var cell = CellMap.FirstOrDefault(c => c.IsPointInCell(x, y));
        if (cell != null)
        {
            return cell.CellPos+Vector3.up;
        }

        return Vector3.zero;
    }

    public bool IsCellAviable(float x, float y)
    {
        var cell = CellMap.FirstOrDefault(c => c.IsPointInCell(x, y));
        if (cell != null)
        {
            return cell.AviablePlane.activeSelf;
        }
        else
        {
            return false;
        }
    }
}
