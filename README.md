# GPGS Sign in with Firebase
This demo covers the basic steps in order to integrate the Google Play Games Services offered by Google with Firebase on a Unity project. Whereas it is not posted the configuration of a Firebase Project on detail, there are mentioned the main steps to do so, in order to check it all.

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

To create the Firebase project on the [Firebase console](https://console.firebase.google.com/), [follow the steps showed on the official documentation.] (https://firebase.google.com/docs/unity/setup)

Important steps:
- Download **the last version** of the [Firebase SDK packages](https://firebase.google.com/download/unity).
- When creating the project, download the JSON configuration file of the project. The JSON file must be on the Assets root folder.


## 3. Google Play Games Services


## 4. Putting all together
