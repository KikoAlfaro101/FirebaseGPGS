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
    [Header("Graphics references")]
    [SerializeField] private Sprite successImg, failImg;

    [SerializeField] private Image anonymousStateImg, emailStateImg;
    //[SerializeField] private Text anonymousDebugText, emailDebugText;

    // References
    [Header("Inputfield references")]
    [SerializeField] private InputField userInputfield;
    [SerializeField] private InputField emailInputfield;
    [SerializeField] private InputField passwordInputfield;

    // Auth instance
    FirebaseAuth auth;

    // Singleton instance
    private static AuthManager _instance;
    public static AuthManager Instance { get { return _instance; } }

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
        // First of all, Firebase dependences must be checked.
        bool dependencesChecked = await CheckFirebaseDependences();
        if (dependencesChecked) DatabaseManager.Instance.SetDatabase();
        auth = FirebaseAuth.DefaultInstance;

        auth.SignOut();
    }

    #endregion


    #region ANONYMOUS

    public void OnAnonymousSignInPressed()
    {
        AnonymousSignIn();
    }
    private void AnonymousSignIn()
    {
        auth = FirebaseAuth.DefaultInstance;

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
        });
    }

    #endregion

    #region EMAIL

    public async void OnEmailSignInSignUpPressed()
    {
        // Get data from GUI
        string username = userInputfield.text;
        string email = emailInputfield.text;
        string password = passwordInputfield.text;

        // Check if any field is empty
        if (username == "" || email == "" || password == "") return; // DEBUG

        // Try to get the user from DB --- SEARCH ANOTHER CHECKING METHOD (BASED ON AUTH)
        User user = await DatabaseManager.Instance.GetUserFromDB(userInputfield.text);

        if (user != null) // If exists, Sign in with that user
        {
            Debug.Log("Sign in user");
            EmailSignIn(email, password);
            DatabaseManager.Instance.SetCurrentUser(user); // (Already found)
        }
        else // If not exists, register user (Sign up)
        {
            Debug.Log("Register user");
            EmailSignUp(email, password); // Create a new User with the input data
            DatabaseManager.Instance.StoreUserOnDB(username);
        }

        DatabaseManager.Instance.InitItemsCanvas();
    }

    private void EmailSignIn(string email, string password)
    {
        Credential credential = EmailAuthProvider.GetCredential(email, password);

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

            // Firebase user Sign in successful.
            FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });
    }

    private void EmailSignUp(string email, string password)
    {
        auth = FirebaseAuth.DefaultInstance;

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            // Firebase user has been created.
            FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });
    }

    #endregion

    public void OnSignOutPressed()
    {
        auth.SignOut();
        Debug.Log("Signed Out");
    }

    private async Task<bool> CheckFirebaseDependences()
    {
        bool checkingDone = false;
        await FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                Debug.Log("Dependency OK");
                checkingDone = true;
            }
            else
            {
                Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });

        return checkingDone;
    }

}