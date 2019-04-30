using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {
	Ready,
	Playing,
	GameOver
}

public class GameManager : Photon.PunBehaviour {
	public static GameManager instance;
	public bool spawnZombies = true;
	[HideInInspector] public GameState gameState = GameState.Ready;
	public ViewBase startView;
	public Transform playerSpawnPoint;
	public Transform[] enemySpawnPoints;
	public EnemySpawner enemySpawner;
	public float spawnDuration = 5f;
	public int maxZombies = 20;
	public float upgradeDuration = 20f;
	public int zombieSpawned = 0;
	public int baseZombieHP = 100;
	public int baseKillReward = 10;
	public float baseZombieSpeed = 1.0f;
	public float maxZombieSpeed = 5.0f;
	[SerializeField] private int zombieHP;
	[SerializeField] private float zombieSpeed;
	[SerializeField] private int killReward;
	private IEnumerator coSpawnEnemies;
	private IEnumerator coEnhanceZombieStatus;
	[HideInInspector] public Player[] players;
	private int playerAlive = 0;
	[SerializeField] private Animator deathAnimator;
	public List<GameObject> deadBodies = new List<GameObject>();
	[HideInInspector] public int zombieKilled = 0;

	void Awake() {
		instance = this;
	}

	public void NotifyPlayerSpawn() {
		photonView.RPC("RPCPlayerSpawned", PhotonTargets.MasterClient);
	}

	public void NotifyPlayerDead() {
		photonView.RPC("RPCPlayerDead", PhotonTargets.MasterClient);
	}

	[PunRPC]
	void RPCPlayerSpawned() {
		playerAlive++;
	}

	[PunRPC]
	void RPCPlayerDead() {
		playerAlive--;

		// Check Game Over
		if(playerAlive <= 0) {
			photonView.RPC("RPCGameOver", PhotonTargets.All);
		}
	}

	public void StartGame() {
		zombieKilled = 0;

		zombieHP = baseZombieHP;
		zombieSpeed = baseZombieSpeed;
		killReward = baseKillReward;

		PhotonNetwork.Instantiate("Player", playerSpawnPoint.position, playerSpawnPoint.rotation, 0);
		GameManager.instance.NotifyPlayerSpawn();

		gameState = GameState.Playing;

		if(spawnZombies == false) return;

		if(!PhotonNetwork.isMasterClient) return;

		coSpawnEnemies = CoSpawnEnemies();
		coEnhanceZombieStatus = CoEnhanceZombieStatus();

		StartCoroutine(coSpawnEnemies);
		StartCoroutine(coEnhanceZombieStatus);
	}

	[PunRPC]
	void RPCGameOver() {
		deathAnimator.SetTrigger("Show");
		gameState = GameState.GameOver;

		if(PhotonNetwork.isMasterClient) {
			StopCoroutine(coSpawnEnemies);
			StopCoroutine(coEnhanceZombieStatus);
		}

		NetworkManager.instance.UpdateUserStatus(zombieKilled);
		Invoke("CleanupGame", 5f);
	}

	void CleanupGame() {
		//Master Client Stuffs
		if(PhotonNetwork.isMasterClient) {
			// Remove Zombies
			Enemy[] zombies = GameObject.FindObjectsOfType<Enemy>();

			for(int i = 0; i < zombies.Length; i++) {
				PhotonNetwork.Destroy(zombies[i].gameObject);
			}
		}
		
		// Remove Inspector
		Inspector inspector = GameObject.FindObjectOfType<Inspector>();
		Destroy(inspector.gameObject);

		// Remove Dead Bodies
		for(int i = 0; i < deadBodies.Count; i++) {
			Destroy(deadBodies[i]);
		}

		deadBodies.Clear();

		gameState = GameState.Ready;

		if(PhotonNetwork.isMasterClient) {
			photonView.RPC("RPCRestartGame", PhotonTargets.All);
		}
	}

	[PunRPC]
	void RPCRestartGame() {
		deathAnimator.SetTrigger("Reset");
		Invoke("NotifyGameOver", 0.1f);
	}

	void NotifyGameOver() {
		NetworkManager.instance.onGameOver.Invoke();
	}

	public void RefreshCurrentPlayers() {
		players = GameObject.FindObjectsOfType<Player>();
	}

	IEnumerator CoSpawnEnemies() {
		yield return new WaitForSeconds(5);

		RefreshCurrentPlayers();

		while(true) {
			for(int i = 0; i < enemySpawnPoints.Length; i++) {
				if(zombieSpawned >= maxZombies) continue;

				GameObject enemyObj = enemySpawner.SpawnAt(enemySpawnPoints[i].position, enemySpawnPoints[i].rotation);
				Enemy enemy = enemyObj.GetComponent<Enemy>();
				Health enemyHealth = enemyObj.GetComponent<Health>();
				KillReward enemyKillReward = enemyObj.GetComponent<KillReward>();

				enemy.speed = zombieSpeed;
				enemyHealth.value = zombieHP;
				enemyKillReward.amount = killReward;

				enemy.onDead.AddListener(() => {
					zombieSpawned--;
				});

				zombieSpawned++;
			}

			yield return new WaitForSeconds(spawnDuration);
		}
	}

	IEnumerator CoEnhanceZombieStatus() {
		yield return new WaitForSeconds(5);

		while(true) {
			yield return new WaitForSeconds(upgradeDuration);

			zombieHP += 20;
			zombieSpeed += 0.25f;
			killReward++;

			if(zombieSpeed > maxZombieSpeed) {
				zombieSpeed = maxZombieSpeed;
			}
		}
	}
}
