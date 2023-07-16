~~~~Etra's Standard Menus~~~~
This is just a basic starting point for standard menus using Unity's New Input System

Features:
-Working Pause, Graphics, and Audio menus.
-Automatic controller and keyboard switching on the ui menus.
-Auto selection of graphics quality
-Saving and Loading default Settings thgrough player prefs.
-Cool sliders

~~~DEPENDENCIES:~~~~
The following dependencies must be installed in order for this asset to function.
-TextMeshPro
-Unity Input System

~~~SETUP:~~~~
1) Add the LoadSavedEtraStandardMenuSettings.cs script to an object on your opening scene. 
It's at: Assets/EtraStandardMenus/Menus/ADD_ME_TO_TITLE_SCENE

2) Drag the EtraStandardMenusManager.prefab into whatever scene you want the menus in.
At: Assets/EtraStandardMenus/Menus

3) Edit that prefab to your heart's content. If you want to add new menu scripts (like a working one for gameplay)
make a new script with a parent of the EtraStandardMenu and attach it to your added menu. 

~~~MORE INFO:~~~~
-Try following the template of the EtraAudioMenu.cs script to make a menu that saves and loads settings values in PlayerPrefs.
Assets/EtraStandardMenus/Menus/Scripts/EtraAudioMenu.cs

-Set the benchmarks for automatically selecting the graphics quality at EtraStandardMenuSettingsFunctions.cs.
Assets/EtraStandardMenus/Menus/Scripts/EtraStandardMenuSettingsFunctions.cs

~Thanks for checking this asset out! :]

---The End---
If you like this, check out my others assets, as well as my...
Youtube:https://www.youtube.com/@Games4NonGamers
and...
Patreon: https://patreon.com/Games4NonGamers