using System.Collections;
using System.Collections.Generic;
using OfflineMode;
using UnityEngine;
using Skin;

//进入game login 需要清理的 全局静态变量 
public class SceneGameLogin : MonoBehaviour
{
    //for weapon
    public Vector3 spawnPoint = Vector3.zero;
    private bool hasStartSpawnWeapons = false;
    private List<int> spawnWeaponIds = null;
    private GameObject spawnWeapon;
    private Coroutine createWeaponCoroutne;

    //for current player
    private GameObject currentPlayer;
    private float dieTime;
    private PlayerAggress fighting;
    private PlayerInfo info;
    public Vector3 revivePos = Vector3.zero;


    //for target player
    private GameObject targetPlayer;
    public Vector3 targetPlayerPos;
    private TagPlayerTorso targetTorso;
    private CharacterSkin targetCharacterSkin;

    //single instance
    public static SceneGameLogin ins;
    public static bool isInGameLogin = false;
    public bool WeaponRevive = false;
    //detect weapon not picked
    private float weaponSurroundTime = 0;

    //检测点击的间隔时间
    private float checkPressTime = 0;

    private bool isShowGameLoginSceneObject = false;
    private bool isShowShoppedBagSceneObject = false;
    private GameObject gameLoginSceneObject = null;
    private GameObject shoppedbagSceneObject = null;

    //是否需要播放引导1：需要 其他数字：不需要
    //private int needLoginGuide = 0;

    // MainCity Camera
    [SerializeField]
    Camera m_MainCityCamera = null;
    [SerializeField]
    List<Transform> m_PosList = null;
    [SerializeField]
    Light m_LoginLight = null;

    public Camera MainCityCamera { get { return m_MainCityCamera; } }

    public List<Transform> PosList { get { return m_PosList; } }

    public Light LoginLight { get { return m_LoginLight; } }

    [SerializeField]
    private Camera[] LoginScene_Cameras;

    void Awake()
    {
        isInGameLogin = true;
        ins = this;
        revivePos = new Vector3(0, 10, 6);
        targetPlayerPos = new Vector3(0, 0, -3);

        if (MainThread.ins != null)
            MainThread.ins.ClearPerFrameTask();
    }
    void Start()
    {
        OnEnterClearDirtyData();
        //Revive();
        //CreateTargetPlayer();
        //清理一下内存
        if (ResourcesMgr.ins != null)
        {
            ResourcesMgr.ins.UnloadUnusedAssets();
        }

        if (DevConfig.IsEnableRecordShare)
        {
            if (!GASDK.RecordScreenButtJoint.Instance.IsAverageFPSCheck)
            {
                StartCoroutine(CheckFPSAverage(2.0f));
            }
        }

        //this.gameLoginSceneObject = GameObject.Find("MainCity");
        //if (gameLoginSceneObject != null)
        //{
        //    gameLoginSceneObject.SetActive(true);
        //}

        //this.shoppedbagSceneObject = GameObject.Find("ShopCity");
        //if (shoppedbagSceneObject != null)
        //{
        //    shoppedbagSceneObject.SetActive(false);
        //}

        this.LoadGameLoginSceneObject();
    }

    private bool isChecked = false;
    private bool hasCheckFPS = false;

    private int raw_targetFrameRate = 30;
    private int raw_vSyncCount = 2;

    //是否主界面内是否打开靶场
    public bool isOnTrainScene { get; set; }

    IEnumerator CheckFPSAverage(float duration)
    {
        //进入场景后3秒开始检测
        yield return new WaitForSeconds(3f);
        
        raw_targetFrameRate = Application.targetFrameRate;
        raw_vSyncCount = QualitySettings.vSyncCount;

        //60帧模式
        hasCheckFPS = true;
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 1;

        float time = 0f;
        int count = 0;
        float fps = 0f;
        while (time < duration)
        {
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
            ++count;
            fps += 1f / Time.deltaTime;
        }

        fps = fps / (count == 0 ? 1f : count);
        bool isPass = fps > DevConfig.MinFpsForLowQuality;
        GASDK.RecordScreenButtJoint.Instance.IsAverageFPSPass = isPass;
        Base.Events.fire2Lua("CheckSupport");
        isChecked = true;

        //bak framerate
        yield return new WaitForSeconds(1f);
        if (hasCheckFPS && !isOnTrainScene)
        {
            Application.targetFrameRate = raw_targetFrameRate;
            QualitySettings.vSyncCount = raw_vSyncCount;
            hasCheckFPS = false;
        }
    }


    public static void OnEnterClearDirtyData()
    {
        //-------------地图编辑器
        MapEditor.MapEditorStroageData.Clear();
        MapEditor.MapObjectRoot.record_json = "";
        //清理一下 截图任务
        MapEditor.MapRuntimeCapture.ClearTasks();
    }
    public static void OnLeaveClearDirtyData()
    {

    }
    void OnDestroy()
    {
        if (ins == this)
        {
            isInGameLogin = false;
            ins = null;
        }
        OnLeaveClearDirtyData();
        //清理一下内存
        if (ResourcesMgr.ins != null)
        {
            ResourcesMgr.ins.UnloadUnusedAssets();
        }

        if (MainThread.ins != null)
            MainThread.ins.ClearPerFrameTask();

        StopCoroutine("CheckFPSAverage");
        //bak framerate
        if (hasCheckFPS && !isOnTrainScene)
        {
            Application.targetFrameRate = raw_targetFrameRate;
            QualitySettings.vSyncCount = raw_vSyncCount;
            hasCheckFPS = false;
        }
        if (!isChecked)
        {
            Base.Events.fire2Lua("CheckSupport");
        }

    }

    public void StartSpawnWeapons()
    {
        WeaponRevive = true;
        hasStartSpawnWeapons = true;
        spawnWeaponIds = new List<int>(ConfigLoader._ConfigGeneral.loginSceneSpawnWeaponIds);
        RandomCreateOneWeapon();
    }

    public void RemoveSpawnWeapons()
    {
        WeaponRevive = false;
        hasStartSpawnWeapons = false;
        if (spawnWeapon != null)
        {
            GameObject.Destroy(spawnWeapon);
            spawnWeapon = null;
        }
    }

    public void SetSpawnWeapons(List<int> weaponIds)
    {
        //这里直接走configGeneral配置表
        spawnWeaponIds = new List<int>(ConfigLoader._ConfigGeneral.loginSceneSpawnWeaponIds);
        //限制刷新的武器列表配置
        //List<int> lockWeaponIds = new List<int>(ConfigLoader._ConfigGeneral.loginSceneSpawnWeaponIds);
        //spawnWeaponIds = weaponIds.FindAll((int item)=> lockWeaponIds.IndexOf(item) == -1);
        if (hasStartSpawnWeapons)
            RandomCreateOneWeapon();
    }

    private void RandomCreateOneWeapon()
    {
        if (!hasStartSpawnWeapons) return;
        if (spawnWeaponIds == null || spawnWeaponIds.Count == 0) return;
        if (spawnWeapon != null) return; //场景里已经有武器就不创建
        if (currentPlayer != null && fighting != null)
        {
            if (fighting.attachedArmed != null)
            {
                return; //玩家手里有武器，不重新创建
            }
        }
        int randomkey = Random.Range(0, spawnWeaponIds.Count);
        int weaponIndex = spawnWeaponIds[randomkey] + 1;
        //     Debug.Log("create weapon " + weaponIndex);
        OnSpawnWeapon(weaponIndex);
        if (spawnWeapon)
        {
            spawnWeapon.transform.position = spawnPoint;
        }
    }

    public GameObject OnSpawnWeapon(int weaponIndex)
    {
        //https://bugly.qq.com/v2/crash-reporting/errors/f4a2f0d471/162239?pid=1
        if (weaponIndex <= 0) return null;

        if (spawnWeapon != null)
        {
            WeaponRevive = false;
            Destroy(spawnWeapon);
            WeaponRevive = true;
        }

        GameObject ret1 = WeaponsLoader.ins.LoadSpawn(weaponIndex - 1);
        if (ret1 == null)
        {
            Debug.LogError("Can not find weapon index =" + weaponIndex);
            return null;
        }

        GameObject ret = GameObject.Instantiate<GameObject>(ret1);
        ret.SetActive(true);
        var pick = ret.GetComponent<WeaponArmer>();
        //ret.FetchComponent<DestroyAfterTime>().time = 10f; //取消10秒自动消失逻辑
        ret.FetchComponent<WeaponDestroyInLogin>();//此脚本在OnDestroy的时候回调SceneGameLogin的OneWeaponDestroyed
        pick.Init(1, weaponIndex);
        ret.layer = ConstLayers.Terrain;
        var rig = ret.GetComponent<Rigidbody>();
        if (rig != null)
        {
            rig.isKinematic = false;
        }
        spawnWeapon = ret;
        return ret;
    }

    //武器被销毁的回调
    public void OneWeaponDestroyed()
    {
        if (createWeaponCoroutne != null)
        {
            StopCoroutine(createWeaponCoroutne);
        }
        createWeaponCoroutne = StartCoroutine(DelayAndCreateWeapon());
    }

    IEnumerator DelayAndCreateWeapon()
    {
        yield return new WaitForSeconds(5.0f);
        RandomCreateOneWeapon();
    }

    public void Revive()
    {
        if (MiscLoader.ins == null) return;

        if (currentPlayer == null)
        {
            currentPlayer = MiscLoader.ins.LoadAndInstantiate<GameObject>("Game/Character", "Game/Character/Character.prefab");
            currentPlayer.gameObject.name += "Local";
        }

        if (revivePos != Vector3.zero)
        {
            currentPlayer.transform.position = revivePos;
        }
        currentPlayer.SetActive(true);
        currentPlayer.layer = 20;
        foreach (var p in currentPlayer.GetComponentsInChildren<Collider>(true))
        {
            p.gameObject.layer = 20;
        }
        info = currentPlayer.GetComponent<PlayerInfo>();
        info.layer = 20;
        info.CanTakeDamage = false;
        //     HealthHandler healthHandler = currentPlayer.GetComponent<HealthHandler>();
        var self = currentPlayer.GetComponent<PlayerRootScript>();
        self.SetHasControl();
        //StartCoroutine(SelfTurnRight());
        fighting = currentPlayer.GetComponent<PlayerAggress>();

        //LoadCurrentPlayerSkin();
        //SetCurrentPlayerLineRendererVisible(StaticData.LineRendererVisible);

        //SetCurrentPlayerJointRendererVisible(StaticData.JointRenderVisibleVue);

        SetCurrentPlayerJointRendererVisible(LoadCurrentPlayerSkin());

        // set local color
        PlayerRootScript.SetColor(PlayerRootScript.self, StaticData.colorId, 20);
    }
    //等待初始化完成之后向右转一下
    IEnumerator SelfTurnRight()
    {
        yield return new WaitForEndOfFrame();
        if (PlayerRootScript.self != null)
        {
            PlayerRootScript.self.Right();
        }
    }

    public void Update()
    {
        if (info != null && info.isNotAlive)
        {

            Time.timeScale = 0.3f;

            dieTime += Time.deltaTime;

            if (dieTime > 0.5f)
            {
                Time.timeScale = 1;
                dieTime = 0;
                Destroy(currentPlayer);
                currentPlayer = null;
                Revive();
            }
        }

        if (IsPointerUI())
        {
            Base.Events.ins.FireLua("ui", "GameLogin_PlayerOperation");
        }
        CheckWeaponAroundTargetPlayer();
    }

#if (UNITY_ANDROID || UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
    // Check if finger is over a UI element
    public bool IsPointerUI()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        }
        //检测按住,避免每一帧都检测，防止每一帧都给lua派发事件造成效率下降
        checkPressTime = checkPressTime + Time.deltaTime;
        if(checkPressTime > 1)
        {
            checkPressTime = 0;
            if (Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Stationary || Input.GetTouch(0).phase == TouchPhase.Moved))
            {
                return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
            }
        }
        return false;
    }
#else
    public bool IsPointerUI()
    {
        //检测点下
        if (Input.GetMouseButtonDown(0))
        {
            return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
        }
        //检测按住,避免每一帧都检测，防止每一帧都给lua派发事件造成效率下降
        checkPressTime = checkPressTime + Time.deltaTime;
        if (checkPressTime > 1)
        {
            checkPressTime = 0;
            if (Input.GetMouseButton(0))
            {
                return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
            }
        }
        return false;
    }
#endif

    public void RemoveLocalPlayer()
    {
        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
            currentPlayer = null;
        }
    }


    public void RemoveTargetAI()
    {
        if (targetPlayer != null)
        {
            Destroy(targetPlayer);
            targetPlayer = null;
        }
    }

    public short LoadCurrentPlayerSkin()
    {
        short visibleValue = 0;
        if (currentPlayer != null)
        {
            CharacterSkin currentCharacterSkin = currentPlayer.GetComponent<CharacterSkin>();
            currentCharacterSkin.cskinunload_skin_parts();
            if (currentCharacterSkin != null && StaticData.CanLoadSkin())
            {
                var typetoskin = StaticData.GetSelfSkins();
                for (int i = 1; i < (int)Skin.SkinPartType.Max; i++)
                {
                    if (typetoskin[i] != null)
                    {
                        foreach (var id in typetoskin[i])
                        {
                            currentCharacterSkin.LoadSkinPart(i, id);
                        }
                    }
                }

                visibleValue = currentCharacterSkin.GetJointVisibleValue();
            }
        }
        return visibleValue;
    }

    [System.Obsolete("接口废弃，SetCurrentPlayerJointRendererVisible")]
    public void SetCurrentPlayerLineRendererVisible(bool visible)
    {
        if (currentPlayer != null)
        {
            PlayerRootScript controller = currentPlayer.GetComponent<PlayerRootScript>();
            if (controller != null)
            {
                controller.SetLineRendererVisible(visible);
            }
        }
    }


    public void SetCurrentPlayerJointRendererVisible(short visibleValue)
    {
        if (currentPlayer != null)
        {
            PlayerRootScript controller = currentPlayer.GetComponent<PlayerRootScript>();
            if (controller != null)
            {
                controller.SetJointRendererVisible(visibleValue);
            }
        }
    }

    public void CreateTargetPlayer()
    {
        if (targetPlayer == null)
        {
            targetPlayer = MiscLoader.ins.LoadAndInstantiate<GameObject>("Game/Character", "Game/Character/Character.prefab");
            targetPlayer.gameObject.name += "AI";
        }

        if (targetPlayerPos != Vector3.zero)
        {
            targetPlayer.transform.position = targetPlayerPos;
        }
        targetPlayer.SetActive(true);
        targetPlayer.layer = 21;
        foreach (var p in targetPlayer.GetComponentsInChildren<Collider>(true))
        {
            p.gameObject.layer = 21;
        }
        var targetInfo = targetPlayer.GetComponent<PlayerInfo>();
        targetInfo.myColor = new Color(35f / 255f, 235f / 255f, 235 / 255f);
        targetInfo.layer = 21;
        targetInfo.myEffectColor = Color.cyan;
        targetInfo.CanTakeDamage = false;
        //SetTargetPlayerLineRendererVisible(false);//创建的时候不显示稻草人，否则会在给他换上牛仔套装的瞬间看到皮肤切换
        SetTargetPlayerJointRendererVisible(CONST.Config.JOINT_ALL_UNVISIBLE);
        targetPlayer.FetchComponent<JackstrawAI>();
        //稻草人 没有需要移除
        targetPlayer.DestroyComponent<SyncablePlayer>();
    }

    public GameObject GetAttackTargetHead()
    {
        if (targetPlayer != null)
        {
            Transform targetPlayerHeadTransform = targetPlayer.transform.Find("Rigidbodies/Head");
            if (targetPlayerHeadTransform != null)
            {
                return targetPlayerHeadTransform.gameObject;
            }
            Debug.LogError("can not find target head in game login scene");
        }
        return null;
    }

    public void CheckWeaponAroundTargetPlayer()
    {
        if (targetPlayer != null && spawnWeapon != null)
        {
            if (targetTorso == null)
            {
                targetTorso = targetPlayer.GetComponentInChildren<TagPlayerTorso>();
            }
            if (targetTorso != null)
            {
                if (Vector3.Distance(targetTorso.transform.position, spawnWeapon.transform.position) > 4)
                {
                    weaponSurroundTime = 0;
                }
                else
                {
                    weaponSurroundTime = weaponSurroundTime + Time.deltaTime;
                    if (weaponSurroundTime >= 5)
                    {
                        Base.Events.ins.FireLua("ui", "GameLogin_WeaponSurround");
                        weaponSurroundTime = 0;
                    }
                }
            }
            else
            {
                weaponSurroundTime = 0;
            }
        }
        else
        {
            weaponSurroundTime = 0;
        }
    }

    public void BroadCastPlayerTakeDamage(GameObject target)
    {
        if (target != null && target == targetPlayer)
        {
            Base.Events.ins.FireLua("ui", "GameLogin_AttackTarget");
        }
    }

    public GameObject GetAttackTarget()
    {
        if (targetPlayer != null)
            return targetPlayer;
        else
            return null;
    }


    public void LoadTargetPlayerSkinPart(int partId, string skinId)
    {
        if (targetPlayer != null)
        {
            if (targetCharacterSkin == null)
            {
                targetCharacterSkin = targetPlayer.GetComponent<CharacterSkin>();
            }
            if (targetCharacterSkin != null)
            {
                if (partId > 0 && !string.IsNullOrEmpty(skinId))
                {
                    targetCharacterSkin.LoadSkinPart(partId, skinId);
                }
            }
        }
        else
        {
            //https://bugly.qq.com/v2/crash-reporting/errors/f4a2f0d471/195538?pid=1
            //    Debug.LogError("can not find target player");
        }
    }

    public void RemoveTargetSkinComponent()
    {
        if (targetPlayer != null)
        {
            if (targetCharacterSkin == null)
            {
                targetCharacterSkin = targetPlayer.GetComponent<CharacterSkin>();
            }
            if (targetCharacterSkin != null)
            {
                Destroy(targetCharacterSkin);
                targetCharacterSkin = null;
            }
        }
        else
        {
            Debug.LogError("can not find target player");
        }
    }

    [System.Obsolete("接口废弃，SetTargetPlayerJointRendererVisible")]
    public void SetTargetPlayerLineRendererVisible(bool visible)
    {
        if (targetPlayer != null)
        {
            var controller = targetPlayer.GetComponent<PlayerRootScript>();
            if (controller != null)
            {
                controller.SetLineRendererVisible(visible);
            }
        }
        else
        {
            Debug.LogError("can not find target player");
        }
    }

    public void SetTargetPlayerJointRendererVisible(short visibleValue)
    {
        if (targetPlayer != null)
        {
            PlayerRootScript controller = targetPlayer.GetComponent<PlayerRootScript>();
            if (controller != null)
            {
                controller.SetJointRendererVisible(visibleValue);
            }
        }
        else
        {
            Debug.LogError("can not find target player");
        }
    }

    public void TrySetTargetPlayerJointRendererVisible()
    {
        if (targetPlayer != null)
        {
            if (targetCharacterSkin == null)
            {
                targetCharacterSkin = targetPlayer.GetComponent<CharacterSkin>();
            }

            if (targetCharacterSkin != null)
            {
                SetTargetPlayerJointRendererVisible(targetCharacterSkin.GetJointVisibleValue());
            }
        }
        else
        {
            Debug.LogError("TrySetTargetPlayerJointRendererVisible can not find target player");
        }
    }

    public void DownloadOfflineMap()
    {

    }
    public IEnumerator DownloadMapOffLine(string name)
    {

        yield return MapHttpTask.Download(name, (string json1) =>
        {
            if (Serializable.Map.IsMapJson(json1))
            {
                GameOfflineMode.LoadMapJson = json1;
                Base.Events.ins.FireLua("offlinemode", "downmap", name);
            }
            else
            {
                Base.Events.ins.FireLua("offlinemode", "downmap", 0);
            }
        }, () =>
        {
            Base.Events.ins.FireLua("offlinemode", "downmap", 0);
        }, true);
    }

    public void SetActiveAllCameras(bool activeState)
    {
        if (LoginScene_Cameras != null && LoginScene_Cameras.Length > 0)
        {
            for(int i = 0,length = LoginScene_Cameras.Length;i < length;i++)
            {
                Camera _curCamera = LoginScene_Cameras[i];
                if(_curCamera != null)
                {
            		_curCamera.enabled = activeState;
                }
            }
        }
    }

    public void LoadGameLoginSceneObject()
    {
        
        if (!isShowGameLoginSceneObject)
        {
            Debug.LogError("================LoadGameLoginSceneObject");

            this.gameLoginSceneObject = MiscLoader.ins.LoadAndInstantiate<GameObject>("Game/Scene", "Game/Scene/MainCity.prefab");

            if (this.gameLoginSceneObject != null)
            {
                this.m_MainCityCamera = this.gameLoginSceneObject.transform.Find("Map Camera").GetComponent<Camera>();
                this.PosList.Clear();
                for (int i = 1; i <= 2; i++)
                {
                    var pos = this.gameLoginSceneObject.transform.Find("Pos" + i.ToString());
                    this.PosList.Add(pos);
                }
                
                this.gameLoginSceneObject.SetActive(true);
            }

            if (this.shoppedbagSceneObject != null)
            {
                GameObject.Destroy(this.shoppedbagSceneObject);
                this.shoppedbagSceneObject = null;
                this.isShowShoppedBagSceneObject = false;
                //this.shoppedbagSceneObject.SetActive(false);
            }

            isShowGameLoginSceneObject = true;
        }

    }

    public void LoadShoppedBagSceneObject()
    {
        if (!isShowShoppedBagSceneObject)
        {
            Debug.LogError("================LoadShoppedBagSceneObject");
            this.shoppedbagSceneObject = MiscLoader.ins.LoadAndInstantiate<GameObject>("Game/Scene", "Game/Scene/ShopCity.prefab");

            if (this.gameLoginSceneObject != null)
            {
                GameObject.Destroy(this.gameLoginSceneObject);
                this.gameLoginSceneObject = null;
                this.isShowGameLoginSceneObject = false;
            }

            if (this.shoppedbagSceneObject != null)
            {
                this.m_MainCityCamera = this.shoppedbagSceneObject.transform.Find("Map Camera").GetComponent<Camera>();
                this.PosList.Clear();
                for (int i = 1; i <= 2; i++)
                {
                    var pos = this.shoppedbagSceneObject.transform.Find("Pos" + i.ToString());
                    this.PosList.Add(pos);
                }

                this.shoppedbagSceneObject.SetActive(true);
            }


            isShowShoppedBagSceneObject = true;
        }

    }
}