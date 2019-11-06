using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Threading.Tasks;
using System;


public class DatabaseManager : MonoBehaviour
{
    [SerializeField] private readonly int itemsAmount = 9;
    [SerializeField] private GameObject itemsCanvas;
    [SerializeField] private Transform buttonsGridParent;
    [SerializeField] private ItemButton unlockableButtonPrefab;

    private ItemButton[] allButtons;
    private User currentUser; // DATABASE

    // Properties
    public int ItemsAmount => itemsAmount;

    // Singleton instance
    private static DatabaseManager _instance;
    public static DatabaseManager Instance { get { return _instance; } }


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

    public void SetDatabase()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://signuptest-kikoalfaro.firebaseio.com/");
    }

    #region DATABASE GETTERS AND SETTERS

    /// <summary>
    /// Stores a new entry of an user on the database
    /// </summary>
    /// <param name="username"></param>
    public void StoreUserOnDB(string username)
    {
        User user = new User(username, itemsAmount); // Only place where a new User is created
        string json = JsonUtility.ToJson(user);
        DatabaseReference dbRootRef = FirebaseDatabase.DefaultInstance.RootReference;

        dbRootRef
            .Child("Users")
            .Child(user.username) // ---> Mail contains @ and it's not allowed
            .SetRawJsonValueAsync(json);

        SetCurrentUser(user);
        // If user exists but it wants to update its data (e.g. "username"), it's possible to access by:
        // mDatabaseRef.Child("users").Child(userId).Child("username").SetValueAsync(name);
    }

    /// <summary>
    /// Gets an entry of an user from the database based on its username
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public async Task<User> GetUserFromDB(string username)
    {
        DataSnapshot snapshot;
        User user = null;

         await FirebaseDatabase.DefaultInstance
        .GetReference("Users").Child(username)
        .GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("ERROR GETTING THE USER");
            }
            else if (task.IsCompleted)
            {
                snapshot = task.Result;
                // Key == username (Both if exists or not)
                Debug.Log(string.Format("Snapshot: {0} - {1}",  snapshot.Key, snapshot.Value));

                if(snapshot.Value != null) // Exists on the database
                {
                    user = User.CreateFromJSON(snapshot.GetRawJsonValue());
                }

                Debug.Log("User from JSON: " + user);
            }
        });
                return user; // DEBUG
    }

    #endregion

    public void SetCurrentUser(User user)
    {
        currentUser = user;
        Debug.Log("New user set: " + currentUser);
    }

    #region ITEMS MANAGEMENT

    public void InitItemsCanvas()
    {
        itemsCanvas.SetActive(true);
        InstantiateItemButtons();
    }

    public void InstantiateItemButtons()
    {
        allButtons = new ItemButton[ItemsAmount];

        for (int i = 0; i < allButtons.Length; i++)
        {
            ItemButton newButton = Instantiate(unlockableButtonPrefab, buttonsGridParent);
            newButton.Init(i + 1);
            newButton.GetComponentInChildren<Text>().text = newButton.itemID.ToString();
            allButtons[i] = newButton;
        }
    }

    public bool SwitchItemState(int id)
    {
        bool unlocked = false;
        if (currentUser.IsItemUnlocked(id))
        {
            currentUser.LockItem(id);
            unlocked = false;
        }
        else
        {
            currentUser.UnlockItem(id);
            unlocked = true;
        }

        return unlocked;
    }

    #endregion

    public async Task<bool> UserExists(string username)
    {
        bool exists = false;
        await FirebaseDatabase.DefaultInstance
  .GetReference("Users").Child(username)
  .GetValueAsync().ContinueWith(task =>
  {
      if (task.IsFaulted)
      {
          // Handle the error...
          Debug.LogError("ERROR GETTING THE USER");
      }
      else if (task.IsCompleted)
      {
          DataSnapshot snapshot = task.Result;
          exists = snapshot.Exists;
          Debug.Log("Snapshot: " + exists);
      }
  });

        return exists;
    }
}