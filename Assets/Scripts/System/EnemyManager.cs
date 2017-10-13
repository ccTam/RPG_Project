using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

	#region Singleton

	public static EnemyManager instance;

	void Awake()
	{
		if (instance != null)
		{
			Debug.LogWarning("More than one instance of EnemyManager found!");
			return;
		}
		instance = this;
	}
	#endregion

	private List<GameObject> enemyGroup;
	[SerializeField]
	private GameObject g1, g2;

	public List<GameObject> EnemyGroup { get { return enemyGroup; } /*set { enemyGroup = value; }*/ }

	void Start()
	{
		enemyGroup = new List<GameObject>();
		AddEnemyToGroup(g1);
		AddEnemyToGroup(g2);
	}

	public void AddEnemyToGroup(GameObject gObj)
	{
		enemyGroup.Add(gObj);
	}
}
