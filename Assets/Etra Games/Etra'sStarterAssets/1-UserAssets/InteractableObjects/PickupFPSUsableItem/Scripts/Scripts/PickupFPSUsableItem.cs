using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace EtrasStarterAssets
{

    public class PickupFPSUsableItem : MonoBehaviour, ISerializationCallbackReceiver
    {
        public static List<string> TMPList;
        [HideInInspector] public List<string> itemShortenedNames;
        [ListToPopup(typeof(PickupFPSUsableItem), "TMPList")]
        public string Item_To_Add;
        private List<Item> fpsItems;
        Item itemType;
        private void Start()
        {
            GetAllItems();
            itemType = fpsItems.ElementAt(itemShortenedNames.IndexOf(Item_To_Add));
            if (EtraCharacterMainController.Instance.etraFPSUsableItemManager == null)
            {
                Debug.LogWarning("PickupFPSUsableItem.cs cannot function without the etraFPSUsableItemManager object.");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                EtraCharacterMainController.Instance.etraFPSUsableItemManager.gameObject.AddComponent(itemType.type);
                EtraCharacterMainController.Instance.etraFPSUsableItemManager.updateUsableItemsArray();
                EtraCharacterMainController.Instance.etraFPSUsableItemManager.equipLastItem();
                Destroy(this.gameObject);
            }
            
        }


        #region ItemListDisplay
        public List<String> GetAllItems()
        {
            fpsItems = new List<Item>();
            fpsItems = FindAllTypes<EtraFPSUsableItemBaseClass>().Select(x => new Item(x)).ToList();

            List<string> temp = new List<string>();
            foreach (Item ability in fpsItems)
            {
                temp.Add(ability.shortenedName.ToString());
            }

            // temp.Sort();
            //generalAbilities.Sort();
            return temp;
        }

        public void OnBeforeSerialize()
        {
            itemShortenedNames = GetAllItems();
            TMPList = itemShortenedNames;
        }

        public void OnAfterDeserialize()
        {

        }

        public static IEnumerable<Type> FindAllTypes<T>()
        {
            var type = typeof(T);
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(t => t != type && type.IsAssignableFrom(t));
        }


        class Item
        {
            public Item(Type type)
            {
                this.type = type;
                state = false;
                name = type.Name;
                GenerateName();
            }

            public Type type;
            public string name;
            public string shortenedName;
            public bool state;

            public void GenerateName()
            {
                shortenedName = type.Name.Split('_').Last();


                shortenedName = Regex.Replace(shortenedName, "([a-z])([A-Z])", "$1 $2");
            }
        }
        #endregion
    }
}
