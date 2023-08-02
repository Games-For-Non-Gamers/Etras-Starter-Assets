using Etra.StarterAssets.Abilities;
using Etra.StarterAssets.Items;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine;
using UObject = UnityEngine.Object;
using static Etra.StarterAssets.EtraCharacterMainController;
using static Etra.StarterAssets.Items.EtraFPSUsableItemManager;
using static Etra.StarterAssets.Source.Editor.EtraCharacterCreator;
using System;
using UnityEditor.Experimental.GraphView;

namespace Etra.StarterAssets.Source.Editor
{
    public class EtraCharacterCreatorCreateOrModify : MonoBehaviour
    {

        public static EtraCharacterMainController CreateOrModify(GameplayType _gameplayType, Model _fpModel, Model _tpModel, List<Type> abilitiesPlayerHas,  List<Type>abilitiesPlayerIsTaught)
        {

            //Combine the Ability lists and get their names
            List<string> abilitiesToAdd = new List<string>();
            foreach (Type abil in abilitiesPlayerHas)
            {
                abilitiesToAdd.Add(abil.Name);
            }
            foreach (Type abil in abilitiesPlayerIsTaught)
            {
                abilitiesToAdd.Add(abil.Name);
            }




            //just the generateAbilitiesAndItems() function from char controller
            //**************************************************************
                List<Ability> generalAbilities = new List<Ability>();
                List<Ability> fpAbilities = new List<Ability>();
                List<Ability> tpAbilities = new List<Ability>();
                List<Ability> fpsItems = new List<Ability>();
                //Initialize abilities
                generalAbilities = EtraGUIUtility.FindAllTypes<EtraAbilityBaseClass>()
                    .Select(x => new Ability(x))
                .ToList();

                fpAbilities = generalAbilities
                    .Where(x => EtraGUIUtility.CheckForUsage(x.type, GameplayTypeFlags.FirstPerson))
                .ToList();

                tpAbilities = generalAbilities
                    .Where(x => EtraGUIUtility.CheckForUsage(x.type, GameplayTypeFlags.ThirdPerson))
                    .ToList();

                generalAbilities = generalAbilities
                .Except(fpAbilities)
                    .Except(tpAbilities)
                    .ToList();
                //Initialize items
                fpsItems = EtraGUIUtility.FindAllTypes<EtraFPSUsableItemBaseClass>()
                    .Select(x => new Ability(x))
                    .ToList();
            //**************************************************************

            //Go through each list and enable the abilities if they are in abilitiesToAdd
            foreach (Ability a in fpAbilities)
            { if (abilitiesToAdd.Contains(a.type.Name)) { a.state = true; } else { a.state = false; } }
            foreach (Ability a in tpAbilities)
            { if (abilitiesToAdd.Contains(a.type.Name)) { a.state = true; } else { a.state = false; } }
            foreach (Ability a in fpsItems)
            { if (abilitiesToAdd.Contains(a.type.Name)) { a.state = true; } else { a.state = false; } }
            foreach (Ability a in generalAbilities)
            { if (abilitiesToAdd.Contains(a.type.Name)) { a.state = true; } else { a.state = false; } }

            return CreateOrModify(null, _gameplayType, _fpModel, _tpModel, generalAbilities, fpAbilities, tpAbilities, fpsItems);
        }




        public static EtraCharacterMainController CreateOrModify(EtraCharacterMainController _target, GameplayType _gameplayType, Model _fpModel, Model _tpModel, List<EtraCharacterCreator.Ability> generalAbilities, List<EtraCharacterCreator.Ability> fpAbilities, List<EtraCharacterCreator.Ability> tpAbilities, List<EtraCharacterCreator.Ability> fpsItems)
        {

            if (_target == null && GameObject.FindGameObjectWithTag("Player"))
            {
                Debug.LogWarning("Please use the Etra Character Creator (Window->StarterAssets-->CharacterCreator) to modify the character or delete the EtraCharacterAssetGroup before making a new non-gamer tutorial/character.");
                return GameObject.FindGameObjectWithTag("Player").GetComponent<EtraCharacterMainController>();
            }

            var group = _target == null ?
                EtrasResourceGrabbingFunctions.addPrefabFromResourcesByName("EtraCharacterAssetGroup") :
                GetRootParent(_target.transform).gameObject;

            if (_target == null)
                _target = group.GetComponentInChildren<EtraCharacterMainController>();

            _target.setChildObjects();

            var abilityManager = _target.etraAbilityManager;

            var selectAbilityScriptTypes = generalAbilities
                .Concat(_gameplayType switch
                {
                    GameplayType.FirstPerson => fpAbilities,
                    GameplayType.ThirdPerson => tpAbilities,
                    _ => new List<Ability>(),
                })
                .Select(x => x.type)
                .ToList();

            foreach (var item in abilityManager.characterAbilityUpdateOrder)
                if (item != null && !selectAbilityScriptTypes.Contains(item.GetType()))
                    DestroyImmediate(item, true);

            abilityManager.characterAbilityUpdateOrder = new EtraAbilityBaseClass[0];

            //Add base abilities
            AddAbilities(abilityManager, generalAbilities, false);

            switch (_gameplayType)
            {
                case GameplayType.FirstPerson:
                    AddAbilities(abilityManager, fpAbilities, log: "Adding first person ability");

                    //Add FPS Items
                    bool requiresItems = fpsItems
                        .Where(x => x.state)
                        .Count() > 0;

                    switch (requiresItems)
                    {
                        case true:
                            //TODO: don't
                            var itemManager = FindObjectOfType<EtraFPSUsableItemManager>() ??
                                EtrasResourceGrabbingFunctions.addPrefabFromAssetsByName("EtraFPSUsableItemManagerPrefab", _target.transform)
                                .GetComponent<EtraFPSUsableItemManager>();

                            var itemsToDelete = itemManager.usableItems;

                            foreach (var item in itemsToDelete)
                                DestroyImmediate(item.script, true);

                            itemManager.Reset();

                            foreach (var item in fpsItems)
                            {
                                if (!item.state) continue;
                                var component = (EtraFPSUsableItemBaseClass)itemManager.gameObject.AddComponent(item.type);
                                itemManager.usableItems = itemManager.usableItems
                                    .Concat(new usableItemScriptAndPrefab[] { new usableItemScriptAndPrefab(component) })
                                    .ToArray();

                              //  DebugLog($"Adding item '{item.name}'");
                            }

                            break;
                        case false:
                            var manager = FindObjectOfType<EtraFPSUsableItemManager>();

                            if (manager != null)
                                DestroyImmediate(manager.gameObject, true);
                            break;
                    }

                    var aimCamera = GameObject.Find("Etra'sStarterAssetsThirdPersonAimCamera");
                    if (aimCamera != null)
                        DestroyImmediate(aimCamera.gameObject, true);

                    _target.applyGameplayChanges(_gameplayType, _fpModel);
                    break;

                case GameplayType.ThirdPerson:
                    var usableItemManager = FindObjectOfType<EtraFPSUsableItemManager>();
                    if (usableItemManager != null)
                        DestroyImmediate(usableItemManager.gameObject, true);

                    var itemCamera = GameObject.Find("FPSUsableItemsCamera");
                    if (itemCamera != null)
                        DestroyImmediate(itemCamera.gameObject, true);


                    AddAbilities(abilityManager, tpAbilities, log: "Adding third person ability");
                    _target.applyGameplayChanges(_gameplayType, _tpModel);
                    break;
            }

            EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            Selection.objects = new UObject[] { group };
            return _target;
        }

        static void AddAbilities(EtraAbilityManager abilityManager, List<Ability> abilities, bool assignScripts = true, string log = "Adding generic ability")
        {
            foreach (var item in abilities)
            {
                if (!item.state || abilityManager.gameObject.GetComponent(item.type)) continue;

                abilityManager.gameObject.AddComponent(item.type);

                //DebugLog($"{log} '{item.name}'");
            }

            if (assignScripts)
                abilityManager.characterAbilityUpdateOrder = abilityManager.gameObject.GetComponents<EtraAbilityBaseClass>();
        }

        static Transform GetRootParent(Transform trans)
        {
            if (trans == null)
                return trans;

            while (true)
            {
                if (trans.parent == null)
                    return trans;

                trans = trans.parent;
            }
        }
    }
}
