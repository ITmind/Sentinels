using UnityEngine;
using System.Collections;

enum EnemyState{
	Seek,
	Fire
}

public class EnemyGun : MonoBehaviour
{
    Player TargetPlayer;
	public float RotateSpeed = 0.5f;
	public float distance = 100f;
	TileMapGenerator map = null;
	private EnemyState state;
	Coroutine absorbCoroutine;
    //Tweener _rotateKorpusAnim;

	public void Start ()
	{
		GameObject temp = GameObject.FindGameObjectWithTag ("Map");
		if(temp!=null){
			map = temp.GetComponent(typeof(TileMapGenerator)) as TileMapGenerator;
		}

		state = EnemyState.Seek;
	}

	public void Update()
	{
		//вращаемся по кругу
		//если видим игрока то останавливаем вращение и начинаем слежение с уничтожение
        if (GameManager.OwnPlayer == null) return;

		SeekPlayer();
		
		switch(state){
		case EnemyState.Seek:
			RotateAround();	
			if(absorbCoroutine != null){
				StopCoroutine("absorbPlayerEnergy");
				absorbCoroutine = null;
			}
			break;
		case EnemyState.Fire:
			RotateKorpus();
			if(absorbCoroutine == null){
				absorbCoroutine = StartCoroutine("absorbPlayerEnergy");
			}
			
			break;
		}
	}
	
		
	void RotateKorpus(){
		//направление на игрока
        var normalPlayerPos = TargetPlayer.transform.position;
        normalPlayerPos.y = transform.position.y;
        Vector3 direction = normalPlayerPos - transform.position;
        
		Quaternion newRotate = Quaternion.LookRotation(direction);

        //Debug.Log(Quaternion.Angle(transform.rotation, newRotate));
        //if (_rotateKorpusAnim == null || _rotateKorpusAnim.isComplete)
        //{
        //    //Debug.Log("start rotate");
        //   _rotateKorpusAnim = HOTween.To(transform, 5f, "rotation", newRotate);
        //}
        //transform.rotation = newRotate;//Quaternion.Slerp(transform.rotation, newRotate, 0.1f * Time.fixedDeltaTime);        
        
	}
	
	void RotateAround(){
		if(Time.timeScale == 0) return;
		
		Vector3 oldRotate = gameObject.transform.eulerAngles;
		gameObject.transform.rotation = Quaternion.Euler(oldRotate.x,oldRotate.y+RotateSpeed,oldRotate.z);
	}
	
	void SeekPlayer(){

        Ray eyeRay = new Ray(transform.position, transform.forward);
        Vector3 eyeDist = eyeRay.GetPoint(2);
        Debug.DrawLine(transform.position, eyeDist, Color.blue);
        Vector3 playerPos = GameManager.OwnPlayer.transform.position;
        if (Vector3.Distance(playerPos, transform.position) <= distance)
        {

            Vector3 direction = playerPos - gameObject.transform.position;

            //определим угол между направлением на игрока и направлением взгляда
            Vector3 directionXZ = direction;
            directionXZ.y = 0;
            float angle = Vector3.Angle(transform.forward, directionXZ);
            int mask = ~(1 << 8 | 1 << 2);

            //если нет ни одного препятствия
            if (!Physics.Linecast(transform.position+Vector3.up, playerPos, mask))
            {
                Debug.DrawLine(transform.position, playerPos, Color.red);
                if (angle < 45)
                {
                    TargetPlayer = GameManager.OwnPlayer;
                    //TargetPlayer.EnableKillEfect();
                    state = EnemyState.Fire;                   
                }
                else
                {
                    if (TargetPlayer != null)
                    {
                        //TargetPlayer.DisableKillEfect();
                    }
                    TargetPlayer = null;
                    state = EnemyState.Seek;                    
                }

            }
            else
            {
                if (TargetPlayer != null)
                {
                    //TargetPlayer.DisableKillEfect();
                }
                TargetPlayer = null;
                state = EnemyState.Seek; 
            }
        }
	}
	
	IEnumerator absorbPlayerEnergy(){
        //ставим в случайном месте ресурс
  //      if (TargetPlayer != null)
  //      {
  //          if (TargetPlayer.Scores >= 0)
  //          {
  //              TargetPlayer.Scores--;
  //              map.SetObjectInRandomCoord(TypeObject.Resource1);
  //          }

  //          if (TargetPlayer.Scores < 0)
  //          {
  //              Destroy(TargetPlayer.gameObject);
  //          }
  //      }
		yield return new WaitForSeconds(3.0f);
		//StartCoroutine("absorbPlayerEnergy");
	}
}
