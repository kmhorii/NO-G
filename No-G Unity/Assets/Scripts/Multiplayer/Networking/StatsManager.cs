using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;

public class StatsManager : MonoBehaviour
{
	public GameObject Wins;
	public GameObject GamesPlayed;
	public GameObject Deaths;
	public GameObject Kills;

	public bool StatsPage = false;

	public void Start()
	{
		if(StatsPage)
		{
			GetStatistics();
		}
	}

	public void Update() { }

	public void CloseScene()
	{
		SceneManager.UnloadSceneAsync("Stats");
	}

	public void GetStatistics()
	{
		PlayFabClientAPI.GetPlayerStatistics(
			new GetPlayerStatisticsRequest(),
			OnGetStatistics,
			error => Debug.LogError(error.GenerateErrorReport())
		);
	}

	private void OnGetStatistics(GetPlayerStatisticsResult result)
	{
		foreach(var eachStat in result.Statistics)
		{
			if (eachStat.StatisticName == "Kills") Kills.GetComponent<Text>().text = eachStat.Value.ToString();
			else if (eachStat.StatisticName == "Games") GamesPlayed.GetComponent<Text>().text = eachStat.Value.ToString();
			else if (eachStat.StatisticName == "Deaths") Deaths.GetComponent<Text>().text = eachStat.Value.ToString();
			else if (eachStat.StatisticName == "Wins") Wins.GetComponent<Text>().text = eachStat.Value.ToString();
		}
	}

	public void incrementGames()
	{
		PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
			{
				Statistics = new List<StatisticUpdate> {
					new StatisticUpdate { StatisticName = "Games", Value = 1 },
				}
			},
			result => { Debug.Log("User statistics updated"); },
			error => { Debug.LogError(error.GenerateErrorReport()); }
		);

	}

	public void incrementKills()
	{
		PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
		{
			Statistics = new List<StatisticUpdate> {
					new StatisticUpdate { StatisticName = "Kills", Value = 1 },
				}
		},
			result => { Debug.Log("User statistics updated"); },
			error => { Debug.LogError(error.GenerateErrorReport()); }
		);
	}

	public void incrementDeaths()
	{
		PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
		{
			Statistics = new List<StatisticUpdate> {
					new StatisticUpdate { StatisticName = "Deaths", Value = 1 },
				}
		},
			result => { Debug.Log("User statistics updated"); },
			error => { Debug.LogError(error.GenerateErrorReport()); }
		);
	}

	public void incrementWins()
	{
		PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
		{
			Statistics = new List<StatisticUpdate> {
					new StatisticUpdate { StatisticName = "Wins", Value = 1 },
				}
		},
			result => { Debug.Log("User statistics updated"); },
			error => { Debug.LogError(error.GenerateErrorReport()); }
		);
	}
}
