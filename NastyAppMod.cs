/**
 *      _   _           _            _                
 *     | \ | | __ _ ___| |_ _   _   / \   _ __  _ __  
 *     |  \| |/ _` / __| __| | | | / _ \ | '_ \| '_ \ 
 *     | |\  | (_| \__ \ |_| |_| |/ ___ \| |_) | |_) |
 *     |_| \_|\__,_|___/\__|\__, /_/   \_\ .__/| .__/ 
 *                          |___/        |_|   |_|    
 *     
 *     a MelonLoader mod for the game Schedule I
 */

using System.Collections;

using MelonLoader;

using UnityEngine;

using Il2CppScheduleOne.UI.Shop;
using Il2CppScheduleOne.UI;

namespace NastyApp
{
    public class NastyAppMod : MelonMod
    {
        #region Fields
        private HarmonyLib.Harmony harmony;

        private bool isIngame = false;
        private bool isInterfacesFound = false;

        private GameObject hardwareStoreObject;
        private GameObject gasStationObject;
        private GameObject darkMarketObject;
        private GameObject weedSupplierObject;
        private GameObject methSupplierObject;
        private GameObject cokeSupplierObject;
        private GameObject pawnShopObject;

        private ShopInterface hardwareStoreInterface;
        private ShopInterface gasStationInterface;
        private ShopInterface darkMarketInterface;
        private ShopInterface weedSupplierInterface;
        private ShopInterface methSupplierInterface;
        private ShopInterface cokeSupplierInterface;
        private PawnShopInterface pawnShopInterface;
        #endregion

        #region MelonLoader
        public override void OnInitializeMelon()
        {
            SendLoggerMsg("Initializing...");

            harmony = new HarmonyLib.Harmony("com.nastyapp.schedulei");
            harmony.PatchAll();

            SendLoggerMsg("Initialized!");
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (sceneName == "Main")
            {
                MelonCoroutines.Start(WaitForCamera());
            }
        }

        public override void OnUpdate()
        {
            if (!isIngame || !isInterfacesFound)
                return;

            if (Input.GetKeyDown(KeyCode.PageUp))
                ToggleInterface(hardwareStoreInterface, "HardwareStoreInterface");

            if (Input.GetKeyDown(KeyCode.PageDown))
                ToggleInterface(gasStationInterface, "GasStationInterface");

            if (Input.GetKeyDown(KeyCode.Home))
                ToggleInterface(darkMarketInterface, "DarkMarketInterface");

            if (Input.GetKeyDown(KeyCode.UpArrow))
                ToggleInterface(weedSupplierInterface, "WeedSupplierInterface");

            if (Input.GetKeyDown(KeyCode.LeftArrow))
                ToggleInterface(methSupplierInterface, "MethSupplierInterface");

            if (Input.GetKeyDown(KeyCode.RightArrow))
                ToggleInterface(cokeSupplierInterface, "CokeSupplierInterface");

            if (Input.GetKeyDown(KeyCode.DownArrow))
                TogglePawnShopInterface(pawnShopInterface, "PawnShopInterface");
        }
        #endregion

        #region IEnumerators
        private IEnumerator WaitForCamera()
        {
            yield return new WaitUntil(new System.Func<bool>(CheckCamera));

            isIngame = true;
            MelonCoroutines.Start(WaitForInterfaces());

            yield break;
        }

        private IEnumerator WaitForInterfaces()
        {
            // Wait for all interfaces to be found
            yield return new WaitUntil(new System.Func<bool>(CheckInterfaces));

            isInterfacesFound = true;
            SendLoggerMsg("All interfaces found!");

            yield break;
        }
        #endregion

        #region Custom Functions
        public void SendLoggerMsg(string msg)
        {
            MelonLogger.Msg(msg);
        }

        private bool CheckCamera()
        {
            return Camera.main != null;
        }

        private bool CheckInterfaces()
        {
            // Define paths for all interfaces
            var paths = new object[]
            {
                new InterfacePath<ShopInterface>
                {
                    Path = "UI/HardwareStoreInterface",
                    Name = "HardwareStoreInterface",
                    Hotkey = KeyCode.PageUp.ToString(),
                    ObjectSetter = obj => hardwareStoreObject = obj,
                    InterfaceSetter = iface => hardwareStoreInterface = iface
                },
                new InterfacePath<ShopInterface>
                {
                    Path = "UI/GasStationInterface",
                    Name = "GasStationInterface",
                    Hotkey = KeyCode.PageDown.ToString(),
                    ObjectSetter = obj => gasStationObject = obj,
                    InterfaceSetter = iface => gasStationInterface = iface
                },
                new InterfacePath<ShopInterface>
                {
                    Path = "UI/DarkMarketInterface",
                    Name = "DarkMarketInterface",
                    Hotkey = KeyCode.Home.ToString(),
                    ObjectSetter = obj => darkMarketObject = obj,
                    InterfaceSetter = iface => darkMarketInterface = iface
                },
                new InterfacePath<ShopInterface>
                {
                    Path = "UI/Supplier Stores/WeedSupplierInterface",
                    Name = "WeedSupplierInterface",
                    Hotkey = KeyCode.UpArrow.ToString(),
                    ObjectSetter = obj => weedSupplierObject = obj,
                    InterfaceSetter = iface => weedSupplierInterface = iface
                },
                new InterfacePath<ShopInterface>
                {
                    Path = "UI/Supplier Stores/MethSupplierInterface",
                    Name = "MethSupplierInterface",
                    Hotkey = KeyCode.LeftArrow.ToString(),
                    ObjectSetter = obj => methSupplierObject = obj,
                    InterfaceSetter = iface => methSupplierInterface = iface
                },
                new InterfacePath<ShopInterface>
                {
                    Path = "UI/Supplier Stores/CokeSupplierInterface",
                    Name = "CokeSupplierInterface",
                    Hotkey = KeyCode.RightArrow.ToString(),
                    ObjectSetter = obj => cokeSupplierObject = obj,
                    InterfaceSetter = iface => cokeSupplierInterface = iface
                },
                new InterfacePath<PawnShopInterface>
                {
                    Path = "UI/PawnShopInterface",
                    Name = "PawnShopInterface",
                    Hotkey = KeyCode.DownArrow.ToString(),
                    ObjectSetter = obj => pawnShopObject = obj,
                    InterfaceSetter = iface => pawnShopInterface = iface
                }
            };

            // Check if all interfaces can be initialized
            foreach (var entry in paths)
            {
                if (entry is InterfacePath<ShopInterface> shopEntry)
                {
                    if (!TryInitializeInterface(shopEntry.Path, shopEntry.ObjectSetter, shopEntry.InterfaceSetter))
                        return false;

                    // Log the interface found
                    SendLoggerMsg($"Found {shopEntry.Name}! Hotkey: {shopEntry.Hotkey}");
                }
                else if (entry is InterfacePath<PawnShopInterface> pawnEntry)
                {
                    if (!TryInitializeInterface(pawnEntry.Path, pawnEntry.ObjectSetter, pawnEntry.InterfaceSetter))
                        return false;

                    // Log the interface found
                    SendLoggerMsg($"Found {pawnEntry.Name}! Hotkey: {pawnEntry.Hotkey}");
                }
            }

            // All interfaces found!
            return true;
        }

        public void ToggleInterface(ShopInterface shopInterface, string interfaceName)
        {
            // Check if the shop interface is open
            if (!shopInterface._IsOpen_k__BackingField)
            {
                // If not, open it
                shopInterface.Open();
                SendLoggerMsg($"Opened {interfaceName}!");
            }
        }

        public void TogglePawnShopInterface(PawnShopInterface pawnShopInterface, string interfaceName)
        {
            // Check if the pawn shop interface is open
            if (!pawnShopInterface._IsOpen_k__BackingField)
            {
                // If not, open it
                pawnShopInterface.Open();
                SendLoggerMsg($"Opened {interfaceName}!");
            }
        }

        private bool TryInitializeInterface<T>(string path, System.Action<GameObject> objectSetter, System.Action<T> interfaceSetter) where T : Component
        {
            // Find the GameObject by path
            var obj = FindGameObjectByPath(path);
            if (obj == null) return false;

            // Check if the GameObject has the required component
            var component = obj.GetComponent<T>();
            if (component == null) return false;

            // Set the object and interface
            objectSetter(obj);
            interfaceSetter(component);

            // Return true if the interface is found
            return true;
        }
        #endregion

        #region Unity Functions
        private GameObject FindGameObjectByPath(string path)
        {
            Transform transform = null;

            string[] array = path.Split('/');
            if (array.Length == 0)
            {
                return null;
            }

            foreach (string text in array)
            {
                if (transform == null)
                {
                    GameObject gameObject = GameObject.Find(text);
                    if (gameObject == null)
                    {
                        return null;
                    }

                    transform = gameObject.transform;
                }
                else
                {
                    transform = transform.Find(text);
                    if (transform == null)
                    {
                        return null;
                    }
                }
            }

            return transform?.gameObject;
        }
        #endregion

        #region Interfaces
        private class InterfacePath<T> where T : Component
        {
            public string Path { get; set; }
            public string Name { get; set; }
            public string Hotkey { get; set; }
            public System.Action<GameObject> ObjectSetter { get; set; }
            public System.Action<T> InterfaceSetter { get; set; }
        }
        #endregion
    }
}
