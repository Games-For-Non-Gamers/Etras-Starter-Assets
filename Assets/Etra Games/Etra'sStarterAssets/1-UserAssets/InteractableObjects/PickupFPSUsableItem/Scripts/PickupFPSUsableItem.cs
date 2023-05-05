using Etra.StarterAssets.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Etra.StarterAssets.Interactables
{

  public class PickupFPSUsableItem : MonoBehaviour, ISerializationCallbackReceiver
  {
    public static List<string> TMPList;
    [HideInInspector] public List<string> itemShortenedNames;
    [ListToPopup(typeof(PickupFPSUsableItem), "TMPList")]
    public string Item_To_Add;
    private List<Item> fpsItems;
    Item itemType;

    //Set the correct selected item
    private void Start()
    {
      GetAllItems();
      itemType = fpsItems.ElementAt(itemShortenedNames.IndexOf(Item_To_Add));
      if (EtraCharacterMainController.Instance.etraFPSUsableItemManager == null)
      {
        Debug.LogWarning("PickupFPSUsableItem.cs cannot function without the etraFPSUsableItemManager object.");
      }
    }

    //If player runs into pickup...
    private void OnTriggerEnter(Collider other)
    {
      if (other.gameObject.CompareTag("Player"))
      {
        //Add the script to the item manager
        EtraCharacterMainController.Instance.etraFPSUsableItemManager.gameObject.AddComponent(itemType.type);
        //Update the items array
        EtraCharacterMainController.Instance.etraFPSUsableItemManager.updateUsableItemsArray();
        //Equip the new item
        EtraCharacterMainController.Instance.etraFPSUsableItemManager.equipLastItem();
        //Destory this pickup
        Destroy(gameObject);
      }

    }

    #region ItemListDisplay
    public List<string> GetAllItems()
    {
      //Get all EtraFPSUsableItemBaseClass
      fpsItems = new List<Item>();
      fpsItems = FindAllTypes<EtraFPSUsableItemBaseClass>().Select(x => new Item(x)).ToList();

      List<string> temp = new List<string>();
      foreach (var ability in fpsItems)
      {
        temp.Add(ability.shortenedName.ToString());
      }
      return temp;
    }

    //Update the list every frame on editor selection "functionally"
    public void OnBeforeSerialize()
    {
      itemShortenedNames = GetAllItems();
      TMPList = itemShortenedNames;
    }

    public void OnAfterDeserialize()
    {

    }

    //Helper function to find all EtraFPSUsableItemBaseClass scripts
    public static IEnumerable<Type> FindAllTypes<T>()
    {
      var type = typeof(T);
      return AppDomain.CurrentDomain.GetAssemblies()
          .SelectMany(x => x.GetTypes())
          .Where(t => t != type && type.IsAssignableFrom(t));
    }

    //Helper class to find all EtraFPSUsableItemBaseClass scripts
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
