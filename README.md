# GPGS Sign in with Firebase
This demo covers the basic steps in order to integrate the Google Play Games Services offered by Google with Firebase on a Unity project. Whereas it is not posted the configuration of a Firebase Project on detail, there are mentioned the main steps to do so, in order to check it all.

**All the script snippets showed below have been extracted of the official documentation of Google, which is the owner of all the rights associated to them.**

Moreover, the steps covered are based on the [official documentation guide](https://firebase.google.com/docs/auth/unity/play-games#set_up_your_firebase_project). Please, check it out while reading this.

## 1.  Unity Project
Configuration of the Player.

#### Other Settings
- Take into account the ID Project a.k.a. Bundle Identifier (e.g. com.yourcompany.unity-project-name). It will be useful in order to register your app when creating the Firebase project.
- Set the *Api Compatibility Level* to **.NET 4.x**.
- Check that the Package Name has the correct Company Name and Product Name.

#### Publishing Settings
The keystore file:
- For the demo, it was used the debug keystore offered by Android SDK. Its default directory is usually: ***C:/Users/USER_NAME/.android/debug.keystore*** and the password is ***android***. Please, check that it's properly set and signed with this password.

(Future) EDIT: How to test the services locally with the debug keystore. (metalink)

## 2. Firebase Project

### Creating the project and importing the packages

It is time to create the Firebase project on the [Firebase console](https://console.firebase.google.com/). See details of [how to create a Firebase Project](https://firebase.google.com/docs/unity/setup).

Important steps:
- Download **the last version** of the [Firebase SDK packages](https://firebase.google.com/download/unity).
- When creating the project, download the JSON configuration file of the project. The JSON file must be on the Assets root folder.

Once the project is created, it must be configurated on the Settings tab and **the Play Games provider must be enabled**. Check the complete steps on the [official guide](https://firebase.google.com/docs/auth/unity/play-games#set_up_your_firebase_project).


## 3. Google Play Games Services

- Download the official [Google Play Games plugin for Unity github repository](https://github.com/playgameservices/play-games-plugin-for-unity).
- Import the package.

#### Configure your app on the Google Play Console (REVIEW)
Games Services:
- It must be linked 2 applications: The Android one (com.yourcompany.unity-project-name) and the Firebase one (https://firebaseproject-yourcompany.firebaseapp.com/).

- To local tests, the public certificate must be used. On the [Google Play Console](https://play.google.com/apps/publish), go to *All applications > YOUR_APP > Release management > Apps signing* and copy the SHA-1 certificate fingerprint Upload certificate (the last one) to the [Google Developer Console](https://console.cloud.google.com/apis): *Credentials > Client IDs of OAuth 2.0 > Android client for...*

    **TIP:** This way, it is possible test the GPGS locally, even by selecting a *Development Build* in order to see the adb log on the Editor's console. ([adb](https://developer.android.com/studio/command-line/adb.html) must be installed)
    
    Thanks to the Stackoverflow user [Naren Neelamegam](https://stackoverflow.com/users/3241481/naren-neelamegam) for providing the method. [Check it out to detailed information.](https://stackoverflow.com/questions/47284852/google-play-sign-in-for-unity)

[More information about Google App signing.](https://developer.android.com/studio/publish/app-signing)

## 4. Putting all together

Once the cloud services are configurated, let's call the API services from the Unity project.


### GPGS management

#### Initialization

```csharp
    private void InitGPGS()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
    .RequestServerAuthCode(false /* Don't force refresh */)
    .Build();

        PlayGamesPlatform.DebugLogEnabled = true; // Enable debugging output
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();
        
        GoogleSignin(); // Optional after initializate services
    }
```


#### Signing in

```csharp
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
```

#### Linking GPGS id token with Firebase
Here the authentication token of GPGS is used as a identification parameter to Sign In on Firebase.
After this step, the user will be authenticated on Firebase and its identifier will be showed on the *Authentication > Users* tab of the Firebase project's console.

```csharp
        authCode = PlayGamesPlatform.Instance.GetServerAuthCode();
        firebaseManager.SignIn(authCode); // Call to the Firebase script
```



### Firebase services management

#### Checking dependences
First of all, it is important have to check that Firebase is going to work well in the Unity project.

```csharp
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                //   app = Firebase.FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
                
                // ...
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
```

#### Signing in
In this case, the Sign In method is called once the user has been logged in with the GPGS, as the *authCode* variable is needed to link it with the Firebase project.

```csharp
    public void SignIn(string authCode)
    {
        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        Firebase.Auth.Credential credential =
            Firebase.Auth.PlayGamesAuthProvider.GetCredential(authCode);
        auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
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

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });
    }
```

#### Signing Out

```csharp
    public void SignOut()
    {
        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;

        auth.SignOut();
        user.DeleteAsync();
    }
```




