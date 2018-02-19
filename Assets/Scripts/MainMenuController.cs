﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

    public GameObject mainMenuCanvas;

    InputField gameIDinputField;
    Text errorText;

    void Start()
    {
        gameIDinputField = mainMenuCanvas.transform.Find("GameIDInputField").GetComponent<InputField>();
        errorText = mainMenuCanvas.transform.Find("ErrorText").GetComponent<Text>();
        errorText.text = "";
    }

    public void OnButtonCreateGame()
    {
        StartCoroutine(CreateGame());
    }

    public void OnButtonJoinGame()
    {
        string gameID = gameIDinputField.text;

        if (gameID == "")
        {
            errorText.text = "Error: no game ID entered!";
            return;
        }
        else
        {
            errorText.text = "";
            StartCoroutine(JoinGame(gameID));
        }
    }

    IEnumerator CreateGame()
    {
        GlobalData.instance.playerID = 0;

        WWW newGameRequest = new WWW("http://localhost/sb/createNewGame.php");

        yield return newGameRequest;

        if (newGameRequest.error == null)
        {
            Debug.Log("New game created! ");
            int convertedInt;
            int.TryParse(newGameRequest.text, out convertedInt);
            GlobalData.instance.gameID = convertedInt;
            Debug.Log("GameID is: " + convertedInt.ToString());
        }
        else
        {
            Debug.LogError("Error: could not create new game.\n" + newGameRequest.error);
        }
        SceneManager.LoadScene("Scene2");
    }

    IEnumerator JoinGame(string gameID)
    {
        GlobalData.instance.playerID = 1;
        int convertedInt;
        int.TryParse(gameIDinputField.text, out convertedInt);
        GlobalData.instance.gameID = convertedInt;
        WWWForm gameJoinID = new WWWForm();
        gameJoinID.AddField("gID", gameID);

        WWW attemptGameJoin = new WWW("http://localhost/sb/joinGame.php", gameJoinID);

        yield return attemptGameJoin;

        if (attemptGameJoin.error == null)
        {
            Debug.Log("Joining game with ID: " + gameID);
            Debug.Log("From Server: " + attemptGameJoin.text);
        }
        else
        {
            Debug.LogError("Error: could not join game with ID: " + gameID + "\nError from PHP script: " + attemptGameJoin.error);
        }

        SceneManager.LoadScene("Scene2");
    }

}
