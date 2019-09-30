using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
public class Configuration : MonoBehaviour {

	public enum ApiKey{
		REGISTRATION,
		LOGIN,
        GUEST_LOGIN,
        FORGOT_PASSWORD,
        CHECK_OTP,
        RESET_PASSWORD,
        LOGOUT,
        GAME_LIST,
        LEADERBOARD,
        PROFILE_DETAILS,
        UPDATE_EMAIL,
        AMOUNT_LIST,
        UPDATE_PROFILE,
        PASSWORD_UPDATE,
        CURRENT_BALANCE,
        SOCIAL_LOGIN,
        SEND_GIFT,
        REDEEM,
        AVATAR_LIST,
        IMAGE_UPDATE,
        BONUS_TIME,
        BONUS_COLLECT,
        BUY_CHIP,
        CREATE_TABLE,
        JOIN_TABLE,
        JOINED_TABLE_LIST,
        DELETE_TABLE,
        UPDATEAVATAR
    }

    bool isLoggedIn;

	JSONNode configuration;
    JSONNode countryCodes;
    Dictionary<string, string> CountryAndCodes = new Dictionary<string, string>();
	public static Configuration Instance;
	void Awake(){
		Instance = this;
		configuration = JSONNode.Parse ((Resources.Load("Configuration/Configuration")as TextAsset).text);
        countryCodes  = JSONNode.Parse((Resources.Load("Configuration/countries") as TextAsset).text);
        //  GLog.Log("The Registration APi........."+GetApi(ApiKey.SIGN_UP));
        isLoggedIn = GetLoginStatus();
        animationType = (AnimationType)Database.GetInteger(Database.Key.ANIMATION);
        Debug.Log("Country Count: "+ countryCodes["countries"].Count);
    }


	public string GetApi(ApiKey apiKey){
        return configuration["API"]["DOMAIN"].Value + configuration["API"][apiKey.ToString()].Value;
	}
    public string GetDomainUrl()
    {
        return configuration["API"]["DOMAIN"].Value;
    }

    public bool GetLoginStatus()
    {
        return isLoggedIn;
    }

    public void SetLoginStatus(bool s)
    {
        isLoggedIn = s;
    }

    private AnimationType animationType;

    public AnimationType AnimationType { get => animationType; set => animationType = value; }

    public string[] GetAllCountryNames()
    {
        List<string> c = new List<string>();

        for (int i = 0; i < countryCodes["countries"].Count; i++)
        {
            c.Add(countryCodes["countries"][i]["name"].Value);
            CountryAndCodes.Add(countryCodes["countries"][i]["name"].Value, countryCodes["countries"][i]["dial_code"].Value);
        }

        return c.ToArray();
    }

    public string GetCountryCode(string key)
    {
        return CountryAndCodes[key];
    }
}
