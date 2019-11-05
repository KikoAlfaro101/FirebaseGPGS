using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class AuthManager : MonoBehaviour
{
    // Singleton instance
    private static AuthManager _instance;
    public static AuthManager Instance { get { return _instance; } }


    [SerializeField] private Image anonymousStateImg;
    [SerializeField] private Image emailStateImg;

    [SerializeField] private Sprite successImg, failImg;
    [SerializeField] private Text anonymousDebugTest, emailDebugTest;

    // References
    [SerializeField] private InputField userInputfield;
    [SerializeField] private InputField emailInputfield;
    [SerializeField] private InputField passwordInputfield;

    // Auth instance
    FirebaseAuth auth;

    #region UNITY METHODS

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private async void Start()
    {
        bool dependencesChecked = await CheckFirebaseDependences();
        if (dependencesChecked) DatabaseManager.Instance.SetDatabase();
        // DON'T DO ANYTHING BEFORE DATABASE IS LOADED
    }

    void Update()
    {
        //auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        //Firebase.Auth.FirebaseUser user = auth.CurrentUser;
        //if (user != null)
        //{
        //    anonymousStateImg.sprite = successImg;
        //    emailStateImg.sprite = successImg;
        //    emailDebugTest.text = user.DisplayName;
        //}
        //else
        //{
        //    anonymousStateImg.sprite = failImg;
        //    emailStateImg.sprite = failImg;
        //}
    }

    #endregion


    #region SIGN IN

    public void OnAnonymousSignInPressed()
    {
        AnonymousSignIn();
    }
    private void AnonymousSignIn()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

        auth.SignInAnonymouslyAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAnonymouslyAsync was canceled.");
                anonymousStateImg.sprite = failImg;
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                anonymousStateImg.sprite = failImg;
                return;
            }

            FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);

            anonymousDebugTest.text = newUser.UserId;
            anonymousStateImg.sprite = successImg;

        });
    }

    public async void OnEmailSignInSignUpPressed()
    {
        if (userInputfield.text == "" || emailInputfield.text == "" || passwordInputfield.text == "") return; // DEBUG

        //User userInput = new User(userInputfield.text,
        //                        emailInputfield.text,
        //                        passwordInputfield.text,
        //                        DatabaseManager.Instance.itemsAmount);

        // Tries to get the user from DB
        User user = await DatabaseManager.Instance.GetUserFromDB(userInput.username);
        if (userExists)
        {
            Debug.Log("Sign in user");
            EmailSignIn(userInput);
            DatabaseManager.Instance.InitItemsCanvas();
        }
        else // If not exists, register user
        {
            Debug.Log("Register user");
            EmailSignUp(userInput);
            DatabaseManager.Instance.InitItemsCanvas();
        }
    }

    private void EmailSignUp(User user)
    {
        auth = FirebaseAuth.DefaultInstance;

        auth.CreateUserWithEmailAndPasswordAsync(user.email, user.password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                emailStateImg.sprite = failImg;
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                emailStateImg.sprite = failImg;
                return;
            }

            DatabaseManager.Instance.StoreUser(user);


            // Firebase user has been created.
            FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);

            emailStateImg.sprite = successImg;
            emailDebugTest.text = newUser.DisplayName;
        });
    }

    private void EmailSignIn(User user)
    {
        Credential credential =
    EmailAuthProvider.GetCredential(user.email, user.password);
        auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });
    }

    #endregion

    public void OnSignOutPressed()
    {
        auth.SignOut();
    }

    private async Task<bool> CheckFirebaseDependences()
    {
        bool check = false;
        await FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                Debug.Log("Dependency OK");
                check = true;
            }
            else
            {
                Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });

        return check;
    }

}