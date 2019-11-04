using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using System.Threading.Tasks;
using UnityEngine.UI;
using System;

public class GPGSManager : MonoBehaviour
{

    public Sprite successImg, failImg;
    public Image debugGPGSImg;
    public FirebaseManager firebaseManager;


    private string authCode; // Auth code from Play Games services --> It will be used as a Firebase credential in order to authenticate the player!
    //public Text authStatus;

    void Start()
    {
        InitGPGS();
    }

    private void InitGPGS()
    {
        Debug.Log("INIT GPGS...");

        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
    .RequestServerAuthCode(false /* Don't force refresh */)
    .Build();

        PlayGamesPlatform.DebugLogEnabled = true; // Enable debugging output

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        //PlayGamesPlatform.Instance.Authenticate(SignInCallback, true);

        GoogleSignin();
    }

    private void GoogleSignin()
    {
        if (!Social.localUser.authenticated)
        {
            Social.localUser.Authenticate(success => {
                if (success)
                {
                    Debug.Log("success");
                }
                else
                {
                    Debug.Log("fail...");
                }
            });
        }
    }

    public void OnSignInButtonPressed()
    {
        GoogleSignin();
    }

    public void OnSignOutButtonPressed()
    {
        PlayGamesPlatform.Instance.SignOut();
    }

    public void OnFirebaseSignInButtonPressed()
    {
        authCode = PlayGamesPlatform.Instance.GetServerAuthCode();
        firebaseManager.SignIn(authCode);
    }

    public void OnFirebaseSignOutButtonPressed()
    {
        firebaseManager.SignOut();
    }

    private void Update()
    {
        if (Social.localUser.authenticated)
        {
            debugGPGSImg.sprite = successImg;
        }
        else
        {
            debugGPGSImg.sprite = failImg;
        }
    }
}
