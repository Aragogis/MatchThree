using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using TMPro;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    public TextMeshProUGUI email, password;
    FirebaseAuth auth;
    DatabaseReference dbReference;

    [SerializeField] private GameObject logInMessage;
    [SerializeField] private GameObject emailRegistrationPanel;

    [SerializeField] private GameObject logInUI;
    [SerializeField] private GameObject startUI;

    public User user;
    private void Awake()
    {
        AppOptions options = new AppOptions
        {
            DatabaseUrl = new System.Uri(FirebaseParams.FIREBASE_DATABASE_URL)
        };
        // Create a new FirebaseApp instance with options
        FirebaseApp app = FirebaseApp.Create(options);
        // Get the FirebaseDatabase instance using the FirebaseApp
        FirebaseDatabase firebaseDatabase = FirebaseDatabase.GetInstance(app, FirebaseParams.FIREBASE_DATABASE_URL);

        auth = FirebaseAuth.DefaultInstance;
        dbReference = firebaseDatabase.RootReference;
    }

    public async void CreateUserWithEmail()
    {
        try
        {
            AuthResult result = await auth.CreateUserWithEmailAndPasswordAsync(email.text, password.text);
           
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);
            user = new User(result.User.UserId, result.User.Email);
            SaveUserData(user);

            logInUI.SetActive(false);
            startUI.SetActive(true);
        }
        catch(Exception ex)
        {
            // Handle exceptions (inspect the message for Firebase-specific issues)
            Debug.LogError("Sign-in failed: " + ex.Message);

            // Show error message on UI
            emailRegistrationPanel.SetActive(false);
            logInMessage.GetComponentInChildren<TMP_Text>().text = "Please try again";
            logInMessage.SetActive(true);
        }
    }

    public async void SignInWithEmail()
    {
        try
        {
            // Await the asynchronous sign-in method
            AuthResult result = await auth.SignInWithEmailAndPasswordAsync(email.text, password.text);

            // Handle successful sign-in
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);

            LoadPlayerData(result.User.UserId);
            logInUI.SetActive(false);
            startUI.SetActive(true);
        }
        catch (Exception ex)
        {
            // Handle exceptions (inspect the message for Firebase-specific issues)
            Debug.LogError("Sign-in failed: " + ex.Message);

            // Show error message on UI
            emailRegistrationPanel.SetActive(false);
            logInMessage.GetComponentInChildren<TMP_Text>().text = "Please try again";
            logInMessage.SetActive(true);
        }
    }

    public void SaveUserData(User userData)
    {
        string json = JsonUtility.ToJson(userData);
        Debug.Log(json);
        dbReference.Child("players").Child(userData.uid).SetRawJsonValueAsync(json);
    }

    public void LoadPlayerData(string playerId)
    {
        dbReference.Child("players").Child(playerId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    user = JsonUtility.FromJson<User>(snapshot.GetRawJsonValue());
                    Debug.Log("Player data loaded successfully!");
                }
                else
                {
                    Debug.Log("No data found for this player!");
                }
            }
            else
            {
                Debug.LogError("Failed to load player data: " + task.Exception);
            }
        });
    }

    public void UpdateNewScore(int levelId, int score)
    {
        dbReference.Child("players").Child(user.uid).Child("scores").Child(levelId.ToString()).SetValueAsync(score);
    }

    private void OnEnable()
    {
        GameEvents.OnNewScore += UpdateNewScore;
    }

    private void OnDisable()
    {
        GameEvents.OnNewScore -= UpdateNewScore;
        SaveUserData(user);
    }
}
