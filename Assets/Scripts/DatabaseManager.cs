using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class User
{
    public string username;
    public string email;

    public User()
    {
    }

    public User(string username, string email)
    {
        this.username = username;
        this.email = email;
    }
}

public class DatabaseManager : MonoBehaviour
{
    // References
    public InputField userInputfield;
    public InputField emailInputfield;

    public int lastUserID = 0; // Allowed ID

    // Firebase
    DatabaseReference dbReference;

    public void SetUpDatabase()
    {
        // Set up the Editor before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://signuptest-kikoalfaro.firebaseio.com/");

        // Get the root reference location of the database.
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void OnStoreUserPressed()
    {
        if(dbReference == null) SetUpDatabase(); // Must to research a callback-based way to do this...

        writeNewUser(lastUserID.ToString(), userInputfield.text, emailInputfield.text);
        lastUserID++;
    }

    private void writeNewUser(string userId, string name, string email)
    {
        User user = new User(name, email);
        string json = JsonUtility.ToJson(user);

        dbReference.Child("users").Child(userId).SetRawJsonValueAsync(json);

        // If user exists but it wants to update its data (e.g. "username"), it's possible to access by:
        // mDatabaseRef.Child("users").Child(userId).Child("username").SetValueAsync(name);

    }
}