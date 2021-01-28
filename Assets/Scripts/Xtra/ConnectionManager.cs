using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using CotcSdk;
using UnityEngine.Networking;
using static Tools;

public struct ProfileData
{
    public string displayName;
    public string email;
    public string lang;
}
public class ConnectionManager : MonoBehaviour
{
    [Header("UI Objects")]
    public GameObject loginPanel;
    public TMP_InputField mail;
    public TMP_InputField pass;

    public GameObject loggedPanel;
    public TextMeshProUGUI pseudoInf;
    public TextMeshProUGUI mailInf;

    private Cloud Cloud;

    private Gamer Gamer;

    private ProfileData profileData;

    string mailUsed;

    void Start()
    {
        Init();
    }

    bool triedToResume = false;
    bool isSet = false;
    private void Update()
    {
        if (!triedToResume)
        {
            Resume();
        }
    }

    private void Init()
    {
        // Link with the CotC Game Object
        var cb = FindObjectOfType<CotcGameObject>();
        if (cb == null)
        {
            Debug.LogError("Cotc object needed !");
            return;
        }

        // Initiate getting the main Cloud object
        cb.GetCloud().Done(cloud => {
            Cloud = cloud;
            // Retry failed HTTP requests once
            Cloud.HttpRequestFailedHandler = (HttpRequestFailedEventArgs e) => {
                if (e.UserData == null)
                {
                    e.UserData = new object();
                    e.RetryIn(1000);
                }
                else
                    e.Abort();
            };
            Debug.Log("Setup done");
        });
    }


    /* If no logs, connect in anonymous
     */
    public void LoginAnonymous()
    {
        if (Gamer != null) return;

        Cloud.LoginAnonymously()
            .Done(gamer => {
                DidLogin(gamer);
            }, ex => {
                CotcException error = (CotcException)ex;
                Debug.LogError("Failed to login: " + error.ErrorCode + " (" + error.HttpStatusCode + ")");
            });
    }

    /* Resume profile if not connected
     */
    public void Resume()
    {
        string idKey = PlayerPrefs.GetString("login_idkey");
        string secretKey = PlayerPrefs.GetString("login_secretkey");
        if (idKey != "" && secretKey != "")
        {
            Cloud.ResumeSession(
                gamerId: PlayerPrefs.GetString("login_idkey"),
                gamerSecret: PlayerPrefs.GetString("login_secretkey"))
                .Done(gamer => {
                    DidLogin(gamer);
                }, ex => {
                    CotcException error = (CotcException)ex;
                    //Debug.LogError("Failed to login: " + error.ErrorCode + " (" + error.HttpStatusCode + ")");
                    LoginAnonymous();
                });
            triedToResume = true;
        }
        else
        {
            Debug.Log("No Accounts");
        }
    }
    
    /* Actions after been logged
     */
    void DidLogin(Gamer newGamer)
    {
        if (Gamer != null)
        {
            Debug.LogWarning("Current gamer " + Gamer.GamerId + " has been dismissed");
        }
        Gamer = newGamer;
        mail.text = "";
        pass.text = "";

        GetProfileData();

        if (isSet)
        {
            LoginNotification();
        }
        else
        {
            isSet = true;
        }
    }

    void RefreshData()
    {
        PlayerPrefs.SetString("login_idkey", Gamer.GamerId);
        PlayerPrefs.SetString("login_secretkey", Gamer.GamerSecret);
        loginPanel.SetActive(false);
        loggedPanel.SetActive(true);
        pseudoInf.text = profileData.displayName;
        mailInf.text = profileData.email;
        print(profileData.email);
    }

    /* Logout Gamer AND delete profile data
     */
    public void Logout()
    {
        Cloud.Logout(Gamer)
        .Done(result => {
            profileData = new ProfileData();
            loginPanel.SetActive(true);
            loggedPanel.SetActive(false);
        }, ex => {
            // The exception should always be CotcException
            CotcException error = (CotcException)ex;
            Debug.LogError("Failed to logout: " + error.ErrorCode + " (" + error.HttpStatusCode + ")");
        });
    }

    /* Get Gamer Profile Data
     */
    private void GetProfileData()
    {
        Gamer.Profile.Get()
        .Done(profileRes => {
            profileData = GetProfileClass(profileRes.ToString());
            if(profileData.email == "")
            {
                SetProfileMail(mailUsed);
            }
            RefreshData();
        }, ex => {
            CotcException error = (CotcException)ex;
            Debug.LogError("Could not get profile data due to error: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
        });
    }
    
    private void SetProfileMail(string email)
    {
        Bundle profileUpdates = Bundle.CreateObject();
        profileUpdates["email"] = new Bundle(email);

        Gamer.Profile.Set(profileUpdates)
        .Done(profileRes => {
            GetProfileData();
            Debug.Log("Profile data set: " + profileRes.ToString());
        }, ex => {
            // The exception should always be CotcException
            CotcException error = (CotcException)ex;
            Debug.LogError("Could not set profile data due to error: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
        });
    }
    private void SetProfilePseudo(string pseudo)
    {
        Bundle profileUpdates = Bundle.CreateObject();
        profileUpdates["displayName"] = new Bundle(pseudo);

        Gamer.Profile.Set(profileUpdates)
        .Done(profileRes => {
            Debug.Log("Profile data set: " + profileRes.ToString());
        }, ex => {
            // The exception should always be CotcException
            CotcException error = (CotcException)ex;
            Debug.LogError("Could not set profile data due to error: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
        });
    }

    public void LoginNetwork()
    {
        Cloud.Login(
                network: "email",
                networkId: mail.text,
                networkSecret: pass.text)
            .Done(gamer => {
                DidLogin(gamer);
                mailUsed = mail.text;
            }, ex => {
                // The exception should always be CotcException
                CotcException error = (CotcException)ex;
                Debug.LogError("Failed to login: " + error.ErrorCode + " (" + error.HttpStatusCode + ")");
            });
    }

    public void RegisterNetwork()
    {
        Application.OpenURL("https://account.clanofthecloud.com/");
    }

    public void CheckUser(string type, string value)
    {
        // cloud is an object retrieved at the beginning of the game through the CotcGameObject object.

        Cloud.UserExists("email", "myEmail@gmail.com")
        .Done(userExistsRes => {
            foreach (var userInfo in userExistsRes)
            {
                Debug.Log("User: " + userExistsRes.ToString());
            }
        }, ex => {
            // The exception should always be CotcException
            CotcException error = (CotcException)ex;
            Debug.LogError("Failed to check user: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
        });
    }

    public void LoginNotification()
    {
        UINotificationManager.instance.CreateNotification("alert", "Logged");
    }

    public void SaveNotification()
    {
        UINotificationManager.instance.CreateNotification("alert", "Saved");
    }

    private bool RequireGamer()
    {
        if (Gamer == null)
            Debug.LogError("Gamer not created.");
        return Gamer != null;
    }

}
