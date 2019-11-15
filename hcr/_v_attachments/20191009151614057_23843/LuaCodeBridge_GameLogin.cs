using System.Collections.Generic;
using UnityEngine;
using Skin;

public partial class LuaCodeBridge
{
    public static GameObject GetAttackTargetHead()
    {
        if (SceneGameLogin.isInGameLogin && SceneGameLogin.ins != null)
        {
            return SceneGameLogin.ins.GetAttackTargetHead();
        }
        return null;
    }

    public static void GameLoginStartSpawnWeapons()
    {
        if (SceneGameLogin.isInGameLogin && SceneGameLogin.ins != null)
        {
            SceneGameLogin.ins.StartSpawnWeapons();
        }
    }

    public static void GameLoginRemoveSpawnWeapons()
    {
        if (SceneGameLogin.isInGameLogin && SceneGameLogin.ins != null)
        {
            SceneGameLogin.ins.RemoveSpawnWeapons();
        }
    }

    public static void GameLoginSetSpawnWeapons(string json)
    {
        if (SceneGameLogin.isInGameLogin && SceneGameLogin.ins != null && json != null)
        {
            HashTable weapons = Json.Decode(json);
            if (weapons != null)
            {
                List<int> weaponList = new List<int>();
                foreach (string value in weapons.GetHashtable().Values)
                {
                    int weaponId = int.Parse(value);
                    weaponList.Add(weaponId);
                }
                SceneGameLogin.ins.SetSpawnWeapons(weaponList);
            }
        }
    }

    [System.Obsolete("使用GameLoginSetTargetPlayerJointRendererVisible")]
    public static void GameLoginSetTargetPlayerLineRendererVisible(bool isVisible)
    {
        if (SceneGameLogin.isInGameLogin && SceneGameLogin.ins != null)
        {
            SceneGameLogin.ins.SetTargetPlayerLineRendererVisible(isVisible);
        }
    }

    public static void GameLoginSetTargetPlayerJointRendererVisible(short visibleValue)
    {
        if (SceneGameLogin.isInGameLogin && SceneGameLogin.ins != null)
        {
            SceneGameLogin.ins.SetTargetPlayerJointRendererVisible(visibleValue);
        }
    }

    public static void GameLoginTrySetTargetPlayerJointRendererVisible()
    {
        if (SceneGameLogin.isInGameLogin && SceneGameLogin.ins != null)
        {
            SceneGameLogin.ins.TrySetTargetPlayerJointRendererVisible();
        }
    }

    public static void GameLoginLoadTargetPlayerSkinPart(int partId, string skinId)
    {
        if (SceneGameLogin.isInGameLogin && SceneGameLogin.ins != null)
        {
            if (partId > 0 && !string.IsNullOrEmpty(skinId))
                SceneGameLogin.ins.LoadTargetPlayerSkinPart(partId, skinId);
        }
    }

    public static void GameLoginRemoveTargetSkinComponent()
    {
        if (SceneGameLogin.isInGameLogin && SceneGameLogin.ins != null)
        {
            SceneGameLogin.ins.RemoveTargetSkinComponent();
        }
    }
    public static void InitSensitiveWord(string path, string name)
    {
        WordFilter.WordFilter.InitLoadWord(path, name);
    }
    public static void GameLoginSetSignPostVisible(bool isVisible)
    {
        if (HideChangeSceneObject.ins)
        {
            HideChangeSceneObject.ins.SetSignPostVisible(isVisible);
        }
    }
    public static void GameLoginHideNotify()
    {
        if (UINotifyLogin.ins)
        {
            UINotifyLogin.ins.HideNotify();
        }
    }
    public static void GameLoginResumeNotify()
    {
        if (UINotifyLogin.ins)
        {
            UINotifyLogin.ins.ResumeNotify();
        }
    }

    public static void SetMainCityCameraDepth(int depth)
    {
        if(SceneGameLogin.ins != null)
        {
            SceneGameLogin.ins.MainCityCamera.depth = depth;
            {
                if( SceneGameLogin.ins.LoginLight != null)
                {
                    SceneGameLogin.ins.LoginLight.gameObject.SetActive(depth > 19);
                }

                 UIMovementUI.TrySetActive(depth < 19);
            }
        }
    }


    public static void SetMainCityCameraEulerAngleY(float angle)
    {
        if (SceneGameLogin.ins != null)
        {
            
            Vector3 eulerAngles = SceneGameLogin.ins.MainCityCamera.transform.localEulerAngles;
            eulerAngles.y = angle;
            SceneGameLogin.ins.MainCityCamera.transform.localEulerAngles = eulerAngles;
        }
    }

       public static void SetPlayerPos(int pos, long playerId, bool isMainCity = true)
    {
        if (isMainCity)
            SceneGameLogin.ins.LoadGameLoginSceneObject();
        else
            SceneGameLogin.ins.LoadShoppedBagSceneObject();

        PlayerBase player = HPlayerMgr.GetPlayerBase(playerId);
        if(SceneGameLogin.ins != null && player != null)
        {
            List<Transform> poslist = SceneGameLogin.ins.PosList;
            if(poslist != null && poslist.Count > pos)
            {
                Transform parent = poslist[pos];
                if(parent != null)
                {
                    var position = player.transform.localPosition;
                    var rotation = player.transform.localRotation;
                    var scale = player.transform.localScale;
                    player.transform.parent = parent;
                    player.transform.localPosition = position;
                    player.transform.localRotation = rotation;
                    player.transform.localScale = scale;
                }
            }
        }
    }


    public static void CreateLoginSceneLocalPlayer()
    {
        if (SceneGameLogin.isInGameLogin && SceneGameLogin.ins != null)
        {
            SceneGameLogin.ins.Revive();
        }
    }

    public static void CreateLoginSceneAIPlayer()
    {
        if (SceneGameLogin.isInGameLogin && SceneGameLogin.ins != null)
        {
            SceneGameLogin.ins.CreateTargetPlayer();
        }
    }

    public static void RemoveLoginSceneLocalPlayer()
    {
        if (SceneGameLogin.isInGameLogin && SceneGameLogin.ins != null)
        {
            SceneGameLogin.ins.RemoveLocalPlayer();
        }
    }

    public static void RemoveLoginSceneAIPlayer()
    {
        if (SceneGameLogin.isInGameLogin && SceneGameLogin.ins != null)
        {
            SceneGameLogin.ins.RemoveTargetAI();
        }
    }

    public static void SetActiveAllCameras(bool activeState)
    {
        if (SceneGameLogin.isInGameLogin && SceneGameLogin.ins != null)
        {
            SceneGameLogin.ins.SetActiveAllCameras(activeState);
        }
    }

    public static void SetOnTrainStatus(bool status)
    {
        if (SceneGameLogin.isInGameLogin && SceneGameLogin.ins != null)
        {
            SceneGameLogin.ins.isOnTrainScene = status;
        }
    }
}