using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyTracker : MonoBehaviour
{

	#region
	
	public List<EnemySpawned> currentEnemies = new List<EnemySpawned>();
	//public EnemySpawned[] currentEnemies;
	
	
	#endregion
	
	#region TempVars
	//Used for temp storage each time new enemy is spawned
	EnemySpawned enemyToAdd;
	EnemySpawned currentlySelected; //when called to find target, this is used
	GameObject towerCalling; //tower calling the function
    float towerRange;
	#endregion
	
	
	public void Start()
	{
    	currentEnemies = new currentEnemies();
        
	}
	
	
	//call function when an enemy spawns 
	//possibly best to do this with a unity event for disentanglement.
	public void OnEnemySpawn(var arg0)
	{	
		EnemyList(arg0 /*TODO correct later*/);
	}
	
	private void EnemyListAdd(GameObject enemyToSpawn)
	{ //TODO change passed GameObject to a property.
		//possibly means more effort is needed to implement for external use 
		enemyToAdd.spawnedEnemy = enemyToSpawn;
		enemyToAdd.timeSpawned = Time.timeSinceLevelLoad;
		
		//Add more information here to give more detail about what kinda enemy it is for targeting
		currentEnemies.Add(enemyToAdd);
	}
	
	
	//called via tower to request enemy
	public OnCallEnemyCheck(var arg0)
	{
		towerCalling = arg0;
		return FindEnemy(); 
	}

	public void FindEnemy()
	{
		currentlySelected = null;
		
        
		foreach(EnemySpawned e in currentEnemies)
		{
			if(currentlySelected.GameObject == null)
			{
				currentlySelected = e;
				break;	 
			}
			
			else if(e.spawnedEnemy == null)
			{
				currentEnemies.Remove(e);
				break;
			}
			
            //Targeting mode is changed here
			else
			{
                switch (targetMode)
                {
                    case targetMode.closest:
                        if (e.DistToTower < currentlySelected.DistToTower && e.DistToTower < towerRange)
                        {
                            currentlySelected = e;
                        }
                        break;

                    case targetMode.lowestHealth:
                            currentlySelected = currentEnemies.Max();
                        break;

                    case targetMode.highestHealth:
                            currentlySelected = currentEnemies.Max();
                        break;

                    case targetMode.furthest:
                            if (e.CurrentAge > currentlySelected.CurrentAge && e.DistToTower < towerRange)
                            {
                                currentlySelected = e;
                            }
                        break;

                }

            }
		}
		return currentlySelected.spawnedEnemy;


	}

	
	//changes targeting credentials for switch in TargetingType()
	public enum	targetMode
	{
		closest,
		lowestHealth,
		highestHealth,
		furthest
	}	
}

//Stores data for enemies spawned and when etc 
public class EnemySpawned : IComparable<EnemySpawned>
{
    public GameObject spawnedEnemy;
    //TODo can I set this to only be set once?
    public float timeSpawned;
    
    private float _currentAge;
    public float CurrentAge
    {
		get
		{
			_currentAge = Time.timeSinceLevelLoad - timeSpawned;
			return _currentAge;
		}
	}

    private float _currentHealth;
    public float CurrentHealth
    {
        get
        {
            
            _currentHealth = spawnedEnemy.Health;
            return _currentHealth;
        }
    }

    private float _distToTower;
    public float DistToTower
    {
        get
        {
            _distToTower = Vector3.Distance(towerCalling.transform.position, spawnedEnemy.transform.postition);
            return _distToTower;
        }
    }

    int IComparable<EnemySpawned>.CompareTo(EnemySpawned other)
    {
        if (other.CurrentHealth > this.CurrentHealth)
            return -1;
        else if (other.CurrentHealth == this.CurrentHealth)
            return 0;
        else
            return 1;
    }
}
