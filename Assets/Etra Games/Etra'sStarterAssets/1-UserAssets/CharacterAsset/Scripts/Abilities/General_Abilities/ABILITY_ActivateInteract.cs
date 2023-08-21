using Etra.StarterAssets.Input;
using Etra.StarterAssets.Source.Interactions;
using UnityEngine;
using EtrasStarterAssets;
using static Etra.StarterAssets.Abilities.ABILITY_Sprint;
using Etra.StarterAssets.Source;
using UnityEditor;

namespace Etra.StarterAssets.Abilities
{
    [AbilityUsageAttribute(EtraCharacterMainController.GameplayTypeFlags.All)]
    public class ABILITY_ActivateInteract : EtraAbilityBaseClass
    {

        //Ability by: asour

        //Public variables
        [Header("Basics")]
        public float interactDistance = 2.0f; // The distance you can interact with an object

        public enum InteractUiType
        {
            None,
            MidBottomScreenCircle
        }
        [Header("UI")]
        public InteractUiType interactUiType = InteractUiType.MidBottomScreenCircle; // The distance you can interact with an object
        public bool hideUiInEditor = true;

        //Private variables
        private ObjectInteraction previousObject; // Previous object gotten in the raycast
        bool holdingInteract = false;
        float heldTime =0;
        bool buttonPressed = false;

        //References
        StarterAssetsInputs starterAssetsInputs;
        ABILITY_CameraMovement camMoveScript;
        InteractCircleUi interactCircleUi;

        public override void abilityStart()
        {
            starterAssetsInputs = GetComponentInParent<StarterAssetsInputs>();
            camMoveScript = GameObject.Find("EtraAbilityManager").GetComponent<ABILITY_CameraMovement>();

            switch (interactUiType)
            {
                case InteractUiType.MidBottomScreenCircle:
                    interactCircleUi = FindObjectOfType<InteractCircleUi>();
                    interactCircleUi.sliderValue = 0;
                    interactCircleUi.hideUi();
                    break;
            }
        }
        //need to do own raycast which checks for triggers
        LayerMask allLayersMask = ~0;
        public override void abilityLateUpdate()
        {
            if (!enabled)
            {
                return;
            }
            RaycastHit raycastHit;
            Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
            var ray = Camera.main.ScreenPointToRay(screenCenterPoint);
  
            //If the object is in range
            if (Physics.Raycast(ray, out raycastHit, interactDistance, allLayersMask, QueryTriggerInteraction.Collide) && interactDistance > Vector3.Distance(camMoveScript.playerCameraRoot.transform.position, raycastHit.point))
            {
                if (raycastHit.transform.GetComponent<ObjectInteraction>()) //Check if the object has the ObjectInteraction script
                {
                    //interactDistance
                    var interaction = raycastHit.transform.GetComponent<ObjectInteraction>(); // Get the object's interaction script

                    var objectThatIsLookedAt = raycastHit.transform.gameObject;

                    if (interaction.isInteractable)
                    {
                        if (previousObject != null)
                        {
                            if (previousObject != interaction)
                            {
                                interaction.onHover.Invoke(); // Call the previous object's onHover event
                                previousObject.onEndHover.Invoke(); // Call the previous object's onEndHover event
                            }
                        }
                        else
                        {
                            interaction.onHover.Invoke(); // Call the object's onHover event

                            switch (interactUiType)
                            {
                                case InteractUiType.MidBottomScreenCircle:
                                    interactCircleUi.showUi();
                                    if (objectThatIsLookedAt.transform.gameObject.GetComponent<ObjectInteraction>().timeToInteract > 0)
                                    {
                                        interactCircleUi.sliderVisibility(true);
                                    }
                                    else
                                    {
                                        interactCircleUi.sliderVisibility(false);
                                    }
                                    break;
                            }
                        }

                        previousObject = interaction; // Set the previous object to the current object


                        if (starterAssetsInputs.interact)
                        {
                            holdingInteract = true;
                            // Interact with the object

                            heldTime += Time.deltaTime;

                            if (objectThatIsLookedAt.transform.gameObject.GetComponent<ObjectInteraction>().timeToInteract > 0)
                            {
                                //Holding E and it has hold E timer
                                switch (interactUiType)
                                {
                                    case InteractUiType.MidBottomScreenCircle:
                                        interactCircleUi.sliderValue = heldTime / objectThatIsLookedAt.transform.gameObject.GetComponent<ObjectInteraction>().timeToInteract;
                                        break;
                                }


                                if (heldTime >= objectThatIsLookedAt.transform.gameObject.GetComponent<ObjectInteraction>().timeToInteract)
                                { //Holding E
                                    heldTime = 0;
                                    starterAssetsInputs.interact = false;
                                    objectThatIsLookedAt.transform.gameObject.GetComponent<ObjectInteraction>().Interact();
                                    buttonPressed = true;
                                    //Nice fade out, and set slider val to 0, ignore new held. reset after holdingInteract = false
                                    switch (interactUiType)
                                    {
                                        case InteractUiType.MidBottomScreenCircle:
                                            interactCircleUi.sliderValue = 0;
                                            interactCircleUi.fadeOutUi(0.3f);
                                            break;
                                    }

                                }
                            }
                            else
                            {
                                buttonPressed = true;
                                objectThatIsLookedAt.transform.gameObject.GetComponent<ObjectInteraction>().Interact();
                            }
                        }
                        else
                        {
                            holdingInteract = false;
                            heldTime = 0;

                            switch (interactUiType)
                            {
                                case InteractUiType.MidBottomScreenCircle:
                                    interactCircleUi.sliderValue = 0;
                                    break;
                            }

                        }

                        if (buttonPressed == true && holdingInteract == false)
                        {
                            buttonPressed = false;
                            // End the interaction with the object
                            objectThatIsLookedAt.transform.gameObject.GetComponent<ObjectInteraction>().onEndInteract.Invoke();
                            objectThatIsLookedAt.transform.gameObject.GetComponent<ObjectInteraction>().onPress.Invoke();
                        }

                    }
                    else
                    {

                        if (previousObject != null)
                        {
                            previousObject.onEndHover.Invoke(); // Call the previous object's onEndHover event
                            previousObject.onEndInteract.Invoke(); // Call the previous object's onEndInteract event
                            switch (interactUiType)
                            {
                                case InteractUiType.MidBottomScreenCircle:
                                    interactCircleUi.hideUi();
                                    interactCircleUi.sliderValue = 0;
                                    break;
                            }

                        }

                        previousObject = null; // Set the previous object to null
                    }
                }
                else
                {
                    switch (interactUiType)
                    {
                        case InteractUiType.MidBottomScreenCircle:
                            interactCircleUi.hideUi();
                            interactCircleUi.sliderValue = 0;
                            break;
                    }
                    if (previousObject != null)
                    {
                        previousObject.onEndHover.Invoke(); // Call the previous object's onEndHover event
                        previousObject.onEndInteract.Invoke(); // Call the previous object's onEndInteract event
                    }
                    previousObject = null;
                }
            }
            else
            {
                switch (interactUiType)
                {
                    case InteractUiType.MidBottomScreenCircle:
                        interactCircleUi.hideUi();
                        interactCircleUi.sliderValue = 0;
                        break;
                }
                if (previousObject != null)
                {
                    previousObject.onEndHover.Invoke(); // Call the previous object's onEndHover event
                    previousObject.onEndInteract.Invoke(); // Call the previous object's onEndInteract event
                }
                previousObject = null;
            }
        }

        private void OnValidate()
        {
            if (hideUiInEditor)
            {
                switch (interactUiType)
                {
                    case InteractUiType.MidBottomScreenCircle:
                        if (FindObjectOfType<InteractCircleUi>())
                        {
                            FindObjectOfType<InteractCircleUi>().hideUi();
                        }
                        break;
                }
            }
            else
            {
                switch (interactUiType)
                {
                    case InteractUiType.MidBottomScreenCircle:
                        if (FindObjectOfType<InteractCircleUi>())
                        {
                            FindObjectOfType<InteractCircleUi>().showUi();
                        }
                        break;
                }
            }
        }

        private void Reset()
        {
            updateUi();
        }
        public void updateUi()
        {
            //Destroy all Ui's
            if (FindObjectOfType<InteractCircleUi>())
            {
                DestroyImmediate(FindObjectOfType<InteractCircleUi>().gameObject);
            }

            //Ad new ones if needed
            switch (interactUiType)
            {
                case InteractUiType.MidBottomScreenCircle:
                    if (this.gameObject.name == "Tempcube") { return; }
                    transform.parent.GetComponent<EtraCharacterMainController>().setChildObjects();
                    EtrasResourceGrabbingFunctions.addPrefabFromAssetsByName("InteractCircleUi", gameObject.transform.parent.GetComponent<EtraCharacterMainController>().starterAssetCanvas.transform, false, Vector3.zero, Quaternion.identity, Vector3.one);
                    interactCircleUi = FindObjectOfType<InteractCircleUi>();
                    break;
            }
        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(ABILITY_ActivateInteract))]
    public class ABILITY_ActivateInteractEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            ABILITY_ActivateInteract script = (ABILITY_ActivateInteract)target;

            if (GUILayout.Button("Update/Reset Ui Type"))
            {
                script.updateUi();
            }
        }
    }
#endif

}