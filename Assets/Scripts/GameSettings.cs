using SimpleJSON;
using System;

[Serializable]
public class GameSettings : Settings
{
    public string prefabItemImageUrl;
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

            if (prefabItemUrl != null)
            {
                if (!prefabItemUrl.StartsWith("https://") || !prefabItemUrl.StartsWith(APIConstant.blobServerRelativePath))
                    settings.prefabItemImageUrl = APIConstant.blobServerRelativePath + prefabItemUrl;
            }
        }
    }
}