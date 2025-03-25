using SimpleJSON;
using System;

[Serializable]
public class GameSettings : Settings
{
    public int playerNumber = 0;
    public string prefabItemImageUrl;
    public int objectMovingSpeed = 0;
}

public static class SetParams
{
    public static void setCustomParameters(GameSettings settings = null, JSONNode jsonNode = null)
    {
        if (settings != null && jsonNode != null)
        {
            ////////Game Customization params/////////
            string prefabItemUrl = jsonNode["setting"]["flying_item_image"] != null ?
                                jsonNode["setting"]["flying_item_image"].ToString().Replace("\"", "") : null;

            settings.playerNumber = jsonNode["setting"]["player_number"] != null ? jsonNode["setting"]["player_number"] : 3;
            settings.objectMovingSpeed = jsonNode["setting"]["object_moving_speed"] != null ? jsonNode["setting"]["object_moving_speed"] : 1;

            LoaderConfig.Instance.gameSetup.playerNumber = settings.playerNumber;
            LoaderConfig.Instance.gameSetup.objectAverageSpeed = settings.objectMovingSpeed;

            if (prefabItemUrl != null)
            {
                if (!prefabItemUrl.StartsWith("https://") || !prefabItemUrl.StartsWith(APIConstant.blobServerRelativePath))
                    settings.prefabItemImageUrl = APIConstant.blobServerRelativePath + prefabItemUrl;
            }
        }
    }
}