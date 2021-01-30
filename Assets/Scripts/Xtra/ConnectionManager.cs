using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

using CotcSdk;
using UnityEngine.Networking;
using System.Threading;
using System.Threading.Tasks;

using static Tools;

public class ProfileData
{
    public string displayName;
    public string email;
    public string lang;
}
public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager instance;

    [Header("UI Objects")]
    public Button socialConnect;
    public Button socialTrophy;

    public GameObject loginPanel;
    public TMP_InputField mail;
    public TMP_InputField pass;

    public GameObject loggedPanel;
    public TextMeshProUGUI pseudoInf;
    public TextMeshProUGUI mailInf;

    public bool connected;

    private Cloud Cloud;
    private Gamer Gamer;
    private ProfileData profileData;

    string mailUsed;

    bool isSet = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        InitConnection();
    }

    bool isResumed = false;
    void Update()
    {
        if (!isResumed)
        {
            Resume();
        }

        if (!connected)
        {
            socialConnect.interactable = false;
            socialTrophy.interactable = false;
        }
        else
        {
            socialConnect.interactable = true;
            socialTrophy.interactable = true;
        }
    }

    void InitConnection()
    {
        var cb = FindObjectOfType<CotcGameObject>();
        if (cb == null)
        {
            Debug.LogError("Place prefab Cotc !");
            return;
        }

        // Log unhandled exceptions (.Done block without .Catch -- not called if there is any .Then)
        Promise.UnhandledException += (object sender, ExceptionEventArgs e) => {
            Debug.LogError("Unhandled exception: " + e.Exception.ToString());
        };
        cb.GetCloud().Done(cloud => {
            Cloud = cloud;
            Cloud.HttpRequestFailedHandler = (HttpRequestFailedEventArgs e) => {
                if (e.UserData == null)
                {
                    e.UserData = new object();
                    e.RetryIn(1000);
                }
                else
                {
                    e.Abort();
                }
            };
            Debug.Log("--- Setup done ---");
            connected = true;
        });
    }

    
    /* Resume profile if not connected
     */
    public void Resume()
    {
        if (!connected) return;

        string idKey = PlayerPrefs.GetString("login_idkey");
        string secretKey = PlayerPrefs.GetString("login_secretkey");
        
        Cloud.ResumeSession(
            gamerId: PlayerPrefs.GetString("login_idkey"),
            gamerSecret: PlayerPrefs.GetString("login_secretkey"))
            .Done(gamer => {
                Gamer = gamer;
                DidLogin();
            }, ex => {
                LoginAnonymous();
            });
        isResumed = true;
    }

    /* If no logs, connect in anonymous
     */
    public void LoginAnonymous()
    {
        if (Gamer != null) return;
        
       
        Cloud.LoginAnonymously()
            .Done(gamer => {
                Gamer = gamer;
                DidLogin();
            }, ex => {
                CotcException error = (CotcException)ex;
            });
    }

    /* Actions after been logged
     */
    void DidLogin(Gamer newGamer = null)
    {
        if(newGamer != null)
        {
            Gamer = newGamer;
        }

        mail.text = "";
        pass.text = "";

        if (isSet && Gamer.Network == "network")
        {
            LoginNotification();
        }
        else if (isSet && Gamer.Network == "anonymous")
        {
            LogoutNotification();
        }
        else
        {
            isSet = true;
        }
        GetProfileData();
    }

    void RefreshData()
    {

        PlayerPrefs.SetString("login_idkey", Gamer.GamerId);
        PlayerPrefs.SetString("login_secretkey", Gamer.GamerSecret);
        
        if(profileData.displayName != "Guest" || profileData.email != null)
        {
            loginPanel.SetActive(false);
            loggedPanel.SetActive(true);
            pseudoInf.text = profileData.displayName;
            mailInf.text = profileData.email;
        }
        else
        {
            loginPanel.SetActive(true);
            loggedPanel.SetActive(false);
            mail.text = "";
            pass.text = "";
        }

        GetUserPlanes();
        GetUserCharacters();
        GetUserCoins();
    }

    /* Logout Gamer AND delete profile data
     */
    public void Logout()
    {
        if (Gamer == null) return;

        Cloud.Logout(Gamer)
        .Done(result => {
            Gamer = null;
            profileData = new ProfileData();
            CollectionCosmetic.instance.DeleteAll();
            GameMaster.instance.SetCoin(0);

            loginPanel.SetActive(true);
            loggedPanel.SetActive(false);

            PlayerPrefs.SetString("login_idkey", "");
            PlayerPrefs.SetString("login_secretkey", "");

            LoginAnonymous();
        }, ex => {
            CotcException error = (CotcException)ex;
        });
    }


    #region -- Profile Data --

    /* Get Gamer Profile Data
     */
    void GetProfileData()
    {
        if (!GamerIsCreated()) return;

        Gamer.Profile.Get()
        .Done(profileRes => {
            profileData = GetProfileClass(profileRes.ToString());
            if(profileData.email == "")
                SetProfileMail(mailUsed);
            RefreshData();
        }, ex => {
            CotcException error = (CotcException)ex;
        });
    }
    
    void SetProfileMail(string email)
    {
        Bundle profileUpdates = Bundle.CreateObject();
        profileUpdates["email"] = new Bundle(email);

        Gamer.Profile.Set(profileUpdates)
        .Done(profileRes => {
        }, ex => {
            CotcException error = (CotcException)ex;
        });
    }
    private void SetProfilePseudo(string pseudo)
    {
        Bundle profileUpdates = Bundle.CreateObject();
        profileUpdates["displayName"] = new Bundle(pseudo);

        Gamer.Profile.Set(profileUpdates)
        .Done(profileRes => {
        }, ex => {
            CotcException error = (CotcException)ex;
        });
    }
    #endregion

    #region -- User data --

    public async Task<string> GetUserValues(string keyName)
    {
        if (!connected) return "";

        await Task.Delay(0);
        string result = "";

        Gamer.GamerVfs.Domain("private").GetValue(keyName)
        .Done(getUserValueRes => {
            result = getUserValueRes["result"].ToString();
        }, ex => {
            CotcException error = (CotcException)ex;
            if (GetJsonError(error.ServerData.ToString()).name == "KeyNotFound")
            {
                result = "KeyNotFound";
            }
        });

        return result;
    }
    public void SetUserValues(string keyName, string keyValue)
    {
        if (!connected) return;

        Bundle value = new Bundle(keyValue);
        Gamer.GamerVfs.Domain("private").SetValue(keyName, value)
        .Done(setUserValueRes => {
            //  Done
        }, ex => {
            CotcException error = (CotcException)ex;
        });
    }

    void DeleteUserValue(string keyName)
    {
        if (!connected) return;

        Gamer.GamerVfs.Domain("private").DeleteValue(keyName)
        .Done(deleteUserValueRes => {
            //  Done
        }, ex => {
            CotcException error = (CotcException)ex;

        });
    }


    public void GetUserPlanes()
    {
        Gamer.GamerVfs.Domain("private").GetValue("deltaplanes")
        .Done(getUserValueRes => {
            string result = getUserValueRes["result"].ToString();
            CollectionCosmetic.instance.SetPlanes(GetJsonPlane(result).deltaplanes);
        }, ex => {
            CotcException error = (CotcException)ex;
            if (GetJsonError(error.ServerData.ToString()).name == "KeyNotFound")
            {
                SetUserPlanes();
            }
        });
    }
    public void GetUserCharacters()
    {
        Gamer.GamerVfs.Domain("private").GetValue("characters")
        .Done(getUserValueRes => {
            string result = getUserValueRes["result"].ToString();
            CollectionCosmetic.instance.SetCharacters(GetJsonCharacters(result).characters);
        }, ex => {
            CotcException error = (CotcException)ex;
            if (GetJsonError(error.ServerData.ToString()).name == "KeyNotFound")
            {
                SetUserCharacters();
            }
        });
    }
    public void GetUserCoins()
    {
        Gamer.GamerVfs.Domain("private").GetValue("coins")
        .Done(getUserValueRes => {
            string result = getUserValueRes["result"].ToString();

            GameMaster.instance.SetCoin(int.Parse(GetJsonCoin(result).coins));
        }, ex => {
            CotcException error = (CotcException)ex;
            if (GetJsonError(error.ServerData.ToString()).name == "KeyNotFound")
            {
                SetUserCoins(0);
            }
        });
    }

    void SetUserPlanes(string values = "grey")
    {
        Bundle value = new Bundle(values);
        Gamer.GamerVfs.Domain("private").SetValue("deltaplanes", value)
        .Done(setUserValueRes => {
            GetUserPlanes();
        }, ex => {
            CotcException error = (CotcException)ex;
            Debug.LogError("Could not set user data due to error: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
        });
    }
    void SetUserCharacters(string values = "grey")
    {
        Bundle value = new Bundle(values);
        Gamer.GamerVfs.Domain("private").SetValue("characters", value)
        .Done(setUserValueRes => {
            GetUserCharacters();
        }, ex => {
            CotcException error = (CotcException)ex;
            Debug.LogError("Could not set user data due to error: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
        });
    }
    void SetUserCoins(int values = 0)
    {
        if(values == 0)
        {
            values = GameMaster.instance.coins;
        }

        Bundle value = new Bundle(values);
        Gamer.GamerVfs.Domain("private").SetValue("coins", value)
        .Done(setUserValueRes => {
            GetUserCoins();
        }, ex => {
            CotcException error = (CotcException)ex;
            Debug.LogError("Can't create coins Data.");
        });
    }
    #endregion

    #region Xtralife Network
    /* Try to login to Xtralife
     */
    public void LoginNetwork()
    {
        Cloud.Login(
                network: "email",
                networkId: mail.text,
                networkSecret: pass.text)
            .Done(gamer => {
                mailUsed = mail.text;
                Gamer = gamer;
                DidLogin();
            }, ex => {
                // The exception should always be CotcException
                CotcException error = (CotcException)ex;
                WrongNotification();
            });
    }

    /* Return to Xtralife to register
     */
    public void RegisterNetwork()
    {
        Application.OpenURL("https://account.clanofthecloud.com/");
    }
    #endregion

    #region Notifications

    /* Return notification
     */
    public void WrongNotification(string msg = "Failed to log")
    {
        UINotificationManager.instance.CreateNotification("wrong", msg);
    }

    /* Return notification
     */
    public void LoginNotification()
    {
        UINotificationManager.instance.CreateNotification("alert", "Logged");
    }

    /* Return notification
     */
    public void LogoutNotification()
    {
        UINotificationManager.instance.CreateNotification("alert", "Logged out");
    }

    /* Return notification
     */
    public void SaveNotification()
    {
        UINotificationManager.instance.CreateNotification("alert", "Saved");
    }

    #endregion


    private bool GamerIsCreated()
    {
        if (Gamer == null)
            Debug.LogError("Gamer not created.");
        return Gamer != null;
    }

}
