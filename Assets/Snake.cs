using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Snake : MonoBehaviour
{
    // Start is called before the first frame update
    
    public GameObject snakeBody;
    
    List<GameObject> objectPool;
    
    LinkedList<int[]> snake;
    
    public long gameSpeed;
    int length;
    DateTime lastTime;
    
    static readonly int[] UP={0,-1};
    static readonly int[] DOWN={0,1};
    static readonly int[] LEFT={-1,0};
    static readonly int[] RIGHT={1,0};
    
    public int ntiles;
    public float boardSize;
    
    
    private float tileSize;
    static Vector2 topLeft;
    
    int[] direction;
    
    int[] lastDir;
    
    int[] food;
    public GameObject foodObject;
    
    public GameObject scoreObject;
    TMP_Text scoreText;
    
    private float y;
    void Start()
    {
    	lastTime=DateTime.Now;
		snake=new LinkedList<int[]>();
		
		direction=RIGHT;
		
		snake.AddLast(new int[2]{ntiles/2,ntiles/2});
		
		
        objectPool=new List<GameObject>();
		//Instantiate(snakeBody,new Vector3(gameObject.transform.position.x-5,gameObject.transform.position.y,gameObject.transform.position.z-5),Quaternion.identity);
		
		topLeft=new Vector2(gameObject.transform.position.x-5,gameObject.transform.position.z+5);
		
		tileSize=10.0f/ntiles;
		
		GameObject t=Instantiate(snakeBody,gridToWorld(snake.First.Value),Quaternion.identity);
			
		t.transform.localScale=new Vector3(tileSize,tileSize,tileSize);
		objectPool.Add(t);
		
		
		y=gameObject.transform.position.y+tileSize/2;
		
		
		newFood();
		
		scoreText=scoreObject.GetComponent<TMP_Text>();
		scoreText.text="Score: 1";
    }
	
	public void newFood()
	{
		food=new int[2];
		food[0]=UnityEngine.Random.Range(0,ntiles);
		food[1]=UnityEngine.Random.Range(0,ntiles);
		
		foodObject.transform.position=gridToWorld(food);
		foodObject.transform.localScale=new Vector3(0.5f*tileSize,0.5f*tileSize,0.5f*tileSize);
	}	
	public void extendSnake()
	{
		if(objectPool.Count<snake.Count+1)
		{
			GameObject t=Instantiate(snakeBody,gridToWorld(lastDir),Quaternion.identity);
			t.transform.localScale=new Vector3(tileSize,tileSize,tileSize);
			objectPool.Add(t);
		}
		
		snake.AddLast(lastDir);
		
		Debug.Log("Extended");
		
		newFood();
		scoreText.text="Score: "+snake.Count;
	}
	Vector3 gridToWorld(int[] pos)
	{
		return new Vector3(topLeft.x+tileSize*(pos[0]+0.5f),y,topLeft.y-tileSize*(pos[1]+0.5f));
	}
	void updateSnake()
	{
		//Move in new direction
		int[] head=snake.First.Value;
		
		
		if(head[0]==food[0] && head[1]==food[1])
			extendSnake();
		
		snake.AddFirst(new int[2]{head[0]+direction[0],head[1]+direction[1]});
		
		lastDir=snake.Last.Value;
		snake.RemoveLast();
		
		head=snake.First.Value;
		
		if(head[0]<0 || head[0]>=ntiles || head[1]<0 || head[1]>=ntiles)
			die();
		
		Debug.Log(head[0]+" "+head[1]);
		
		int i=0;
		foreach(int[] s in snake)
		{
			if( s!=head && s[0]==head[0] && s[1]==head[1] )
			{
				die();
				break;
			}
			objectPool[i].transform.position=gridToWorld(s);
			objectPool[i++].SetActive(true);
		}
	}
	void die()
	{
		Debug.Log("Death");
		foreach(GameObject b in objectPool)
			b.SetActive(false);
			
		lastTime=DateTime.Now;
		snake=new LinkedList<int[]>();
		
		direction=RIGHT;
		
		snake.AddLast(new int[2]{ntiles/2,ntiles/2});
		
		newFood();
		
		
		scoreText.text="Score: 1";
	}
    void Update()
    {
    	DateTime currentTime=DateTime.Now;
    	
    	if(Input.GetKeyDown(KeyCode.A))
    		direction=LEFT;
    	else if(Input.GetKeyDown(KeyCode.D))
    		direction=RIGHT;
    	else if(Input.GetKeyDown(KeyCode.W))
    		direction=UP;
    	else if(Input.GetKeyDown(KeyCode.S))
    		direction=DOWN;
 
 
    	if((currentTime-lastTime).TotalMilliseconds>=gameSpeed)
    	{	
    		lastTime=currentTime;
    		updateSnake();
    	}
    }
}
