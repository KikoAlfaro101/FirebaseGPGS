# GPGS Sign in with Firebase
This demo covers the basic steps in order to integrate the Google Play Games Services offered by Google with Firebase on a Unity project. Whereas it is not posted the configuration of a Firebase Project on detail, there are mentioned the main steps to do so, in order to check it all.

## 1.  Unity Project
Configuration of the Player.

#### Other Settings
- Take into account the ID Project a.k.a. Bundle Identifier (e.g. com.yourcompany.unity-project-name). It will be useful in order to register your app when creating the Firebase project.
- Set the *Api Compatibility Level* to **.NET 4.x**.
- Check that the Package Name has the correct Company Name and Product Name.

#### Publishing Settings


## 2. Firebase Project

### Creating the project and importing the packages




**NOTICE:** When creating the Firebase project on the [Firebase console](https://console.firebase.google.com/?hl=es-419), it's crucial to download both the last version of the [Firebase SDK packages](https://firebase.google.com/download/unity?hl=es-419) and the JSON configuration file of the project. The JSON file must be on the Assets root folder.

For more information: https://firebase.google.com/docs/unity/setup?hl=es-419.

## 3. Firebase and GPGS Methods
