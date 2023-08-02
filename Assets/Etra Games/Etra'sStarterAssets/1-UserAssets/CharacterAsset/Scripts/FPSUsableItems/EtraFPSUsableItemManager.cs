using UnityEngine;
using System.Collections;
using UnityEditor;
using Etra.StarterAssets.Input;
using Etra.StarterAssets.Source;
using EtrasStarterAssets;

namespace Etra.StarterAssets.Items
{
    public class EtraFPSUsableItemManager : MonoBehaviour
    {
        //Visible in Inspector
        public bool playEquipAnims = true;
        public bool playUnequipAnims = true;
        [HideInInspector]
        public bool weaponInitHandledElsewhere = false;
        [Header("The Items Will Be Selected In This Order:")]
        public usableItemScriptAndPrefab[] usableItems;

        #region Functions to update The usableItems Array
        //Run this function whenever an item is added
        public void updateUsableItemsArray()
        {
            removeNullItemSlots();
            //I understand this is Big O^2 however, it only runs on validate. What's more important is navigation of the final structure (an array) is as fast as possible.
            EtraFPSUsableItemBaseClass[] grabbedUsableItems;

            if (this == null)
            {
                return;
            }

            if (GetComponent<EtraFPSUsableItemBaseClass>() != null)
            {
                grabbedUsableItems = GetComponents<EtraFPSUsableItemBaseClass>();
            }
            else
            {
                grabbedUsableItems = new EtraFPSUsableItemBaseClass[0];
            }

            foreach (var item in grabbedUsableItems)
            {
                bool itemFound = false;

                foreach (var setItem in usableItems)
                {
                    if (item.Equals(setItem.script))
                    {
                        itemFound = true;
                    }
                }

                if (!itemFound)
                {
                    usableItemScriptAndPrefab newItem = new usableItemScriptAndPrefab(item);
                    increaseAbilityArrayWithNewElement(newItem);
                }
            }

        }

        private void removeNullItemSlots()
        {

            if (usableItems == null)
            {
                return;
            }

            if (usableItems.Length == 0)
            {
                return;
            }

            bool slotsNeedRemoved = true;
            while (slotsNeedRemoved)
            {
                slotsNeedRemoved = false;
                int elementToPass = 0;
                for (int i = 0; i < usableItems.Length; i++)
                {
                    if (usableItems[i].script == null)
                    {
                        slotsNeedRemoved = true;
                        elementToPass = i;
                        i = usableItems.Length;
                    }
                }

                if (slotsNeedRemoved)
                {
                    usableItems = removeElementFromArray(elementToPass);
                }

            }


        }

        private usableItemScriptAndPrefab[] removeElementFromArray(int elementToSkip)
        {
            usableItemScriptAndPrefab[] shortenedArray = new usableItemScriptAndPrefab[usableItems.Length - 1];

            int iterator = 0;
            for (int i = 0; i < usableItems.Length; i++)
            {
                if (i != elementToSkip)
                {
                    shortenedArray[iterator] = usableItems[i];
                    iterator++;
                }
            }

            return shortenedArray;
        }

        private void increaseAbilityArrayWithNewElement(usableItemScriptAndPrefab abilityToAdd)
        {
            usableItemScriptAndPrefab[] temp = new usableItemScriptAndPrefab[usableItems.Length + 1];

            for (int i = 0; i < usableItems.Length; i++)
            {
                temp[i] = usableItems[i];
            }

            temp[usableItems.Length] = abilityToAdd;


            usableItems = temp;
        }


        //Editor exclusive functions
#if UNITY_EDITOR
        EtraFPSUsableItemManager()
        {
            ObjectFactory.componentWasAdded -= HandleComponentAdded;
            ObjectFactory.componentWasAdded += HandleComponentAdded;

            EditorApplication.quitting -= OnEditorQuiting;
            EditorApplication.quitting += OnEditorQuiting;
        }
        private void HandleComponentAdded(Component obj)
        {
            updateUsableItemsArray();
            if (Application.isPlaying)
            {
                Debug.LogWarning("In the EDITOR PLAY MODE, you can add items by adding components to the FPS Usable Item Manager.\n" +
                    "However, this will not work in a built game witout an additional step. Everytime you add a component to\n" +
                    "the item manager, run the updateUsableItemsArray() function to add the new item to the correct array.\n");
            }
        }

        private void OnEditorQuiting()
        {
            ObjectFactory.componentWasAdded -= HandleComponentAdded;
            EditorApplication.quitting -= OnEditorQuiting;
        }

#endif

        #endregion

        //Not in Inspector
        private int activeItemNum = 0;
        [HideInInspector] public GameObject activeItemPrefab;
        [HideInInspector] bool isEquipping = false;

        //References
        StarterAssetsInputs starterAssetsInputs;
        GameObject cameraRoot;
        private bool inputsLocked = false;
        AudioManager fpsItemAudioManager;

        [System.Serializable]
        public class usableItemScriptAndPrefab
        {
            public EtraFPSUsableItemBaseClass script;
            [HideInInspector] public GameObject prefab;

            public usableItemScriptAndPrefab(EtraFPSUsableItemBaseClass passedScript)
            {
                script = passedScript;
                prefab = EtrasResourceGrabbingFunctions.getPrefabFromResourcesByName(script.getNameOfPrefabToLoad());
            }
        }


#if UNITY_EDITOR
        //Reset is ran by the character creator adding this component
        public void Reset()
        {
            updateUsableItemsArray();
            //Add usable FPS item camera if it does not exist
            var FPSUsableItemsCamera = EtrasResourceGrabbingFunctions.addPrefabFromAssetsByName("FPSUsableItemsCamera", GameObject.Find("EtraPlayerCameraRoot").transform, false, Vector3.zero);
            //Add usable FPS item camera script to the camera to check for the FPSUsableItem Layer
            if (FPSUsableItemsCamera != null && FPSUsableItemsCamera.GetComponent<FPS_Item_Cam_Checks>() == null) { FPSUsableItemsCamera.AddComponent<FPS_Item_Cam_Checks>(); }
        }


        private void OnValidate() //ON COMPONENT ADD
        {
            updateUsableItemsArray();
            //go through usable items.
            //load all items in game object prefabs so searching does not have to be done.
        }
#endif

        private void Awake()
        {
#if UNITY_EDITOR
            removeNullItemSlots();
#endif
            cameraRoot = GameObject.Find("EtraPlayerCameraRoot");
            starterAssetsInputs = GetComponentInParent<StarterAssetsInputs>();

            for (int i = 0; i < usableItems.Length; i++)
            {
                usableItems[i].script.enabled = false;
            }

        }


        private void Start()
        {
            fpsItemAudioManager = GameObject.FindGameObjectWithTag("MainCamera").transform.Find("FPSItemSfx").GetComponent<AudioManager>();
            if (!weaponInitHandledElsewhere)
            {
                instatiateItemAtStart();
            }

        }


        public void instatiateItemAtStart()
        {

            if (usableItems.Length > 0)
            {
                var newItem = Instantiate(usableItems[activeItemNum].prefab);
                newItem.transform.SetParent(cameraRoot.transform);
                newItem.transform.localPosition = Vector3.zero;
                newItem.transform.localRotation = Quaternion.identity;
                activeItemPrefab = newItem;
                usableItems[activeItemNum].script.enabled = true;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (inputsLocked)
            {
                setInputsToDefault();
                return;
            }

            //Some sort of check that were not moving to same item
            if (isEquipping)
            {
                return;
            }

            //Number Keys select
            #region Number Key Item Selection
            if (starterAssetsInputs.item0Select)
            {
                if (activeItemNum != 0 && usableItems.Length > 0)
                {
                    StartCoroutine(equipItemCoroutine(0));
                }
            }

            if (starterAssetsInputs.item1Select)
            {
                if (activeItemNum != 1 && usableItems.Length > 1)
                {
                    StartCoroutine(equipItemCoroutine(1));
                }
            }

            if (starterAssetsInputs.item2Select)
            {
                if (activeItemNum != 2 && usableItems.Length > 2)
                {
                    StartCoroutine(equipItemCoroutine(2));
                }
            }

            if (starterAssetsInputs.item3Select)
            {
                if (activeItemNum != 3 && usableItems.Length > 3)
                {
                    StartCoroutine(equipItemCoroutine(3));
                }
            }

            if (starterAssetsInputs.item4Select)
            {
                if (activeItemNum != 4 && usableItems.Length > 4)
                {
                    StartCoroutine(equipItemCoroutine(4));
                }
            }

            if (starterAssetsInputs.item5Select)
            {
                if (activeItemNum != 5 && usableItems.Length > 5)
                {
                    StartCoroutine(equipItemCoroutine(5));
                }
            }

            if (starterAssetsInputs.item6Select)
            {
                if (activeItemNum != 6 && usableItems.Length > 6)
                {
                    StartCoroutine(equipItemCoroutine(6));
                }
            }

            if (starterAssetsInputs.item7Select)
            {
                if (activeItemNum != 7 && usableItems.Length > 7)
                {
                    StartCoroutine(equipItemCoroutine(7));
                }
            }

            if (starterAssetsInputs.item8Select)
            {
                if (activeItemNum != 8 && usableItems.Length > 8)
                {
                    StartCoroutine(equipItemCoroutine(8));
                }
            }

            if (starterAssetsInputs.item9Select)
            {
                if (activeItemNum != 9 && usableItems.Length > 9)
                {
                    StartCoroutine(equipItemCoroutine(9));
                }
            }
            #endregion

            if (usableItems.Length <= 1)
            {
                return;
            }

            //Mouse wheel and shoulder button scroll
            if (starterAssetsInputs.usableItemInventoryScroll == 1)
            {
                int itemToMoveTo = activeItemNum + 1;

                if (itemToMoveTo >= usableItems.Length)
                {
                    itemToMoveTo = 0;
                }
                StartCoroutine(equipItemCoroutine(itemToMoveTo));

            }

            if (starterAssetsInputs.usableItemInventoryScroll == -1)
            {
                int itemToMoveTo = activeItemNum - 1;

                if (itemToMoveTo < 0)
                {
                    itemToMoveTo = usableItems.Length - 1;
                }
                StartCoroutine(equipItemCoroutine(itemToMoveTo));

            }

        }

        public void equipItem(int num)
        {
            if (usableItems.Length > num)
            {
                StartCoroutine(equipItemCoroutine(num));
            }
        }

        public void equipLastItem()
        {
            if (usableItems.Length>0)
            {
                StartCoroutine(equipItemCoroutine(usableItems.Length - 1));
            }
        }

        IEnumerator equipItemCoroutine(int newItemNum)
        {

            isEquipping = true;


            if (playUnequipAnims && usableItems.Length > 1)
            {
                usableItems[activeItemNum].script.runUnequipAnimation();
                yield return new WaitForSeconds(usableItems[activeItemNum].script.getItemUnequipSpeed());
            }

            usableItems[activeItemNum].script.enabled = false;
            Destroy(activeItemPrefab);

            activeItemNum = newItemNum;
            var newItem = Instantiate(usableItems[activeItemNum].prefab);
            newItem.transform.SetParent(cameraRoot.transform);
            newItem.transform.localPosition = Vector3.zero;
            newItem.transform.localRotation = Quaternion.identity;
            activeItemPrefab = newItem;

            if (playEquipAnims)
            {
                if (usableItems[activeItemNum].script.equipSfxName == "")
                {
                    fpsItemAudioManager.Play("DefaultEquip");
                }
                else
                {
                    fpsItemAudioManager.Play(usableItems[activeItemNum].script.equipSfxName);
                }

                
                activeItemPrefab.transform.localRotation = Quaternion.Euler(usableItems[activeItemNum].script.getItemUnequipRotation());
                usableItems[activeItemNum].script.runEquipAnimation();
                yield return new WaitForSeconds(usableItems[activeItemNum].script.getItemEquipSpeed());
            }

            usableItems[activeItemNum].script.enabled = true;
            setInputsToDefault();
            isEquipping = false;


        }

        void setInputsToDefault()
        {
            starterAssetsInputs.usableItemInventoryScroll = 0;

            starterAssetsInputs.item0Select = false;
            starterAssetsInputs.item1Select = false;
            starterAssetsInputs.item2Select = false;
            starterAssetsInputs.item3Select = false;
            starterAssetsInputs.item4Select = false;
            starterAssetsInputs.item5Select = false;
            starterAssetsInputs.item6Select = false;
            starterAssetsInputs.item7Select = false;
            starterAssetsInputs.item8Select = false;
            starterAssetsInputs.item9Select = false;
        }

        public void disableFPSItemInputs()
        {

            if (usableItems.Length > 0)
            {
                inputsLocked = true;
                usableItems[activeItemNum].script.inputsLocked = true;
                if (EtraCharacterMainController.Instance.GetComponentInChildren<FPSUsableItemSwayAndBobAnimations>())
                {
                    EtraCharacterMainController.Instance.GetComponentInChildren<FPSUsableItemSwayAndBobAnimations>().lockInput = true;
                }
            }

        }

        public void enableFPSItemInputs()
        {

            if (usableItems.Length > 0)
            {
                inputsLocked = false;
                usableItems[activeItemNum].script.inputsLocked = false;
                if (EtraCharacterMainController.Instance.GetComponentInChildren<FPSUsableItemSwayAndBobAnimations>())
                {
                    EtraCharacterMainController.Instance.GetComponentInChildren<FPSUsableItemSwayAndBobAnimations>().lockInput = false;
                }
            }


        }

    }
}
