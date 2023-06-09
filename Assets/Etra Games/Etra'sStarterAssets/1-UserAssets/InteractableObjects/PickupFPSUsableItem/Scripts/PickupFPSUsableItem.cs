using Etra.StarterAssets.Items;
using Etra.StarterAssets.Source;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Etra.StarterAssets.Interactables
{

    public class PickupFPSUsableItem : MonoBehaviour, ISerializationCallbackReceiver
    {
        public static List<string> TMPList;
        [HideInInspector] public List<string> itemShortenedNames;
        [ListToPopup(typeof(PickupFPSUsableItem), "TMPList")]
        public string Item_To_Add;
        private List<ItemScriptAndNameHolder> fpsItems;
        ItemScriptAndNameHolder selectedItem;

        //Set the correct selected item
        private void Start()
        {
            updateItems();

            foreach (ItemScriptAndNameHolder item in fpsItems)
            {
                if (item.shortenedName == Item_To_Add)
                {
                    selectedItem = item;
                }
            }

            if (EtraCharacterMainController.Instance.etraFPSUsableItemManager == null)
            {
                Debug.LogWarning("PickupFPSUsableItem.cs cannot function without the etraFPSUsableItemManager object.");
            }
        }


        private void Reset()
        {
            updateItems();
        }

        //If player runs into pickup...
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                //Add the script to the item manager
                EtraCharacterMainController.Instance.etraFPSUsableItemManager.gameObject.AddComponent(selectedItem.script.GetType());
                //Update the items array
                EtraCharacterMainController.Instance.etraFPSUsableItemManager.updateUsableItemsArray();
                //Equip the new item
                EtraCharacterMainController.Instance.etraFPSUsableItemManager.equipLastItem();
                //Destory this pickup
                Destroy(gameObject);
            }

        }


        //Update the list every frame on editor selection "functionally"
        [ContextMenu("Update Items")]
        public void updateItems()
        {
            fpsItems = EtrasResourceGrabbingFunctions.GetAllItems();
            itemShortenedNames = new List<string>();
            foreach (ItemScriptAndNameHolder item in fpsItems)
            {
                itemShortenedNames.Add(item.shortenedName);
            }

        }

        //Update the list every frame on editor selection "functionally"
        public void OnBeforeSerialize()
        {
            //itemShortenedNames = GetAllItems();
            TMPList = itemShortenedNames;
        }

        public void OnAfterDeserialize()
        {

        }


    }
}
