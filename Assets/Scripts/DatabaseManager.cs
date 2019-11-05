using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Threading.Tasks;
using System;

public class User
{
    public string username;
    public string email;
    public string password;
    public string items = "";

    public User()
    {
    }

    public User(string username, string email, string password, int itemsAmount)
    {
        this.username = username;
        this.email = email;
        this.password = password;

        CreateItems(itemsAmount);
    }

    private void CreateItems(int amount)
    {
        string temp = "";

        for (int i = 0; i < amount; i++)
        {
            temp += "0";
        }

        items = temp;
    }

    public override string ToString()
    {
        return string.Format("Username: {0} - Email: {1}", username, email);
    }

    public void UnlockItem(int id)
    {
        string newItems = "";

        for (int i = 0; i < items.Length; i++)
        {
            if (i == id - 1) newItems += "1"; // unlocked
            else newItems += items[i]; // not changes
        }

        items = newItems; // update data
    }

    public void LockItem(int id)
    {
        string newItems = "";

        for (int i = 0; i < items.Length; i++)
        {
            if (i == id - 1) newItems += "0"; // unlocked
            else newItems += items[i]; // not changes
        }

        items = newItems; // update data
    }

    public bool IsItemUnlocked(int id)
    {
        return items[id - 1] == '1'; // If equals 1, the item is unlocked
    }
}

public class DatabaseManager : MonoBehaviour
{
    public GameObject itemsCanvas;
    [SerializeField] private Transform buttonsGridParent;

    [Space]
    [SerializeField] private UnlockableButton unlockableButtonPrefab;
    private UnlockableButton[] allButtons;
    public int itemsAmount = 9;

    private User currentUser; // DATABASE
    public User CurrentUser { get => currentUser; set => currentUser = value; }

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

    public void StoreUserOnDB(User user)
    {
        string json = JsonUtility.ToJson(user);
        DatabaseReference dbRootRef = FirebaseDatabase.DefaultInstance.RootReference;

        Debug.Log(json);
        dbRootRef
            .Child("Users")
            .Child(user.username) // ---> Mail contains @ and it's not allowed
            .SetRawJsonValueAsync(json);

        Debug.Log("WRITE NEW USER CALLED");

        // If user exists but it wants to update its data (e.g. "username"), it's possible to access by:
        // mDatabaseRef.Child("users").Child(userId).Child("username").SetValueAsync(name);
    }

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


    public async User GetUserFromDB(string username)
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



    public void InitItemsCanvas()
    {
        itemsCanvas.SetActive(true);
        InstantiateUnlockableButtons();
    }

    public void InstantiateUnlockableButtons()
    {
        allButtons = new UnlockableButton[itemsAmount];

        for (int i = 0; i < allButtons.Length; i++)
        {
            UnlockableButton newButton = Instantiate(unlockableButtonPrefab, buttonsGridParent);
            newButton.Init(i + 1);
            newButton.GetComponentInChildren<Text>().text = newButton.unlockableID.ToString();
            allButtons[i] = newButton;
        }
    }

    public bool SwitchItemState(int id)
    {
        bool unlocked = false;
        User currentUser = AuthManager.Instance.CurrentUser;

        Debug.Log(currentUser);
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
}