# CameraPlus
CameraPlus is a Beat Saber mod that allows for multiple wide FOV cameras with smoothed movement, which makes for a much more pleasant overall spectator experience.

# stable version Installing
1. Use the Mod Assistant : https://github.com/Assistant/ModAssistant/releases
		It is the easiest method, it will do all these steps below in 1 click.
# Latest version Download
The version here may be a version that has not yet been registered with ModAssistant, or a version that will not be registered.
[Release Page](https://github.com/Snow1226/CameraPlus/releases)
### To install manually:
	1b. Make sure that Beat Saber is not running.
	2b. Extract the contents of the zip into Beat Saber's installation folder.
		For Oculus Home: \Oculus Apps\Software\hyperbolic-magnetism-beat-saber\
		For Steam: \steamapps\common\Beat Saber\
		(The folder that contains Beat Saber.exe)
	3b. Done! You've installed the CameraPlus Plugin.

### Make sure to disable the base game's "Smooth Camera" If you wish to use CameraPlus
The latest version will automatically force SmoothCamera to turn off. If you want to turn it on, please change the setting in UserData/CameraPlus.ini. (See RootConfig below).

# Usage
To edit the settings of any camera in real time, right click on the Beat Saber game window! A context menu will appear with options specific to the camera that you right clicked on!

Press <kbd>F1</kbd> to toggle the main camera between first and third person.

## RootConfig
After you run the game once, a `CameraPlus.ini` file is created within the folder Beat Saber\UserData. This file controls profile switching between menu scenes and game scenes.
Edit that file to configure Profile change:

| Parameter                   | Description                                                                                  |
|-----------------------------|----------------------------------------------------------------------------------------------|
| **ProfileSceneChange**      | When set to True, profile switching is enabled in the Menu scene and Game scene.             |
| **MenuProfile**             | Enter the profile name to be read in the Menu scene.                                         |
| **GameProfile**             | Enter the profile name to be read in the Game scene.                                         |
| **MultiplayerProfile**      | Enter the profile name to be read in the Multiplay scene.                                    |
|                             | If value here, the profile will not be switched between the multiplayer lobby and the game.  |
| **CurrentProfile**          | It is currently not working. This is for the next version.                                   |
| **CameraQuadLayer**         | Changes the display layer of the third person camera. Basically, do not move it from zero.   |
| **ForceDisableSmoothCamera**| Automatically turn off SmoothCamera at game launch.                                          |
### Profile Name
![ProfileName](https://github.com/Snow1226/CameraPlus/blob/master/img/Menu_08_ProfileMenu.png)

Open the Profile menu from Profile Saver in the right-click menu.
The profile name is displayed under Cullently Selected.
Please describe this in ini above.
Example) MenuProfile = "CameraPlusProfile"

## CameraConfig
After you run the game once, a `cameraplus.cfg` file is created within the folder Beat Saber\UserData\CameraPlus. Any cfg files located in this folder will be used to render additional cameras.
Edit that file to configure CameraPlus:

| Parameter                     | Description                                                                                     |
|-------------------------------|-------------------------------------------------------------------------------------------------|
| **LockScreen**                | Locks the window from being resized or moved.                                                   |
| **fov**                       | Horizontal field of view of the camera                                                          |
| **antiAliasing**              | Anti-aliasing setting for the camera (1, 2, 4 or 8 only)                                        |
| **renderScale**               | The resolution scale of the camera relative to game window (similar to supersampling for VR)    |
| **positionSmooth**            | How much position should smooth **(SMALLER NUMBER = SMOOTHER)**                                 |
| **rotationSmooth**            | How much rotation should smooth **(SMALLER NUMBER = SMOOTHER)**                                 |
| **cam360Smoothness**          | How smoothly the 360 cam snaps to lane changes **(SMALLER NUMBER = SMOOTHER)**                  |
| **cam360RotateControlNew**    | Set to False if you don't like the new 360 degree control.                                      |
| **thirdPerson**               | Whether third person camera is enabled                                                          |
| **showThirdPersonCamera**     | Whether or not the third person camera is visible                                               |
| **use360Camera**              | Whether the 360 camera is used                                                                  |
| **posx**                      | X position of third person camera                                                               |
| **posy**                      | Y position of third person camera                                                               |
| **posz**                      | Z position of third person camera                                                               |
| **angx**                      | X rotation of third person camera                                                               |
| **angy**                      | Y rotation of third person camera                                                               |
| **angz**                      | Z rotation of third person camera                                                               |
| **firstPersonPosOffsetX**     | X position offset of first person camera                                                        |
| **firstPersonPosOffsetY**     | Y position offset of first person camera                                                        |
| **firstPersonPosOffsetZ**     | Z position offset of first person camera                                                        |
| **firstPersonRotOffsetX**     | X rotation offset of first person camera                                                        |
| **firstPersonRotOffsetY**     | Y rotation offset of first person camera                                                        |
| **firstPersonRotOffsetZ**     | Z rotation offset of first person camera                                                        |
| **cam360ForwardOffset**       | 360 camera position offset forwards                                                             |
| **cam360UpOffset**            | 360 camera position offset upwards                                                              |
| **cam360RightOffset**         | 360 camera position offset to the right                                                         |
| **cam360XTilt**               | X tilt rotation relative to the forward of 360 camera                                           |
| **cam360ZTilt**               | Z tilt rotation relative to the forward of 360 camera                                           |
| **cam360YTilt**               | The Y tilt rotation OFFSET on the 360 camera                                                    |
| **screenWidth**               | Width of the camera render area                                                                 |
| **screenHeight**              | Height of the camera render area                                                                |
| **screenPosX**                | X position of the camera in the Beat Saber window                                               |
| **screenPosY**                | Y position of the camera in the Beat Saber window                                               |
| **MultiplayerNumber**         | Shift the central axis of the camera to the participants in the official mulch.                 |
|                               |  (0=Off, 1=Host, 2~=Participants)                                                               |
| **DisplayMultiPlayerNameInfo**| Displays player information in the upper left corner of the screen during official multiplayer. |
| **layer**                     | Layer to render the camera on **(HIGHER NUMBER = top)**                                         |
| **fitToCanvas**               | Force camera to stretch to fit window                                                           |
| **transparentWalls**          | Make Walls appear transparent on the camera                                                     |
| **forceFirstPersonUpRight**   | Lock rotation of first person camera on Z axis to keep the camera upright                       |
| **Avatar**                    | Switch avatar display layer                                                                     |
| **debri**                     | Use "link" to follow the game settings, and "show" and "hide" to force display/hide.            |
| **HideUI**                    | Hide the official UI in the cam+ screen.                                                        |
| **movementScriptPath**        | Enter the script file name, including the json extension.                                       |
| **movementAudioSync**         | Syncs the transitional process of the move script to the game audio.                            |

## Movement Script
The sample script described in MovementScriptPath is located in the "Beat Saber\UserData\CameraPlus\Scripts" folder.

movementScriptPath should contain either a "fullpath including the file name + extension" or "file name + extension json".

ex) 
1. movementScriptpath = "C:\Program Files(x86)\Steam\steamapps\common\Beat Saber\UserData\CameraPlus\Scripts\ExampleMovementScript.json"
2. movementScriptpath = "ExampleMovementScript.json"

**for Users of locales that use commas for decimal points**

If the move script doesn't work, enclose the number in double quotation marks. Also, use commas for decimal points.


```xml
{
    "ActiveInPauseMenu": true,     "ActiveInPauseMenu": Determines whether the camera pauses when pausing the game.
    "Movements": [                 "Movements"        : Position description section of the moving camera.
    {
        "StartPos": {              "StartPos"         : Camera start position (the center of the play area is 0,0,0).
            "x": 2,
            "y": 1.75,
            "z": -2,
            "FOV": 90              "FOV"              : StartFOV (Optional)
        },
        "StartRot": {              "StartRot"         : The rotation at which the camera starts (0,0,0 looks straight at the main menu).
            "x": 15,
            "y": -15,
            "z": 0
        },
        "EndPos": {                "EndPos"           : The position where the camera ends.
            "x": 2,
            "y": 1,
            "z": 9,
            "FOV": 40              "FOV"              : EndFOV (Optional)
        },
        "EndRot": {                "EndRot"           : The rotation at which the camera ends.
            "x": 15,
            "y": -40,
            "z": 0
        },
        "Duration": 4,             "Duration"         : The time it takes for the transition to start / end / rotate.
        "Delay": 0,                "Delay"            : the time to wait before proceeding to the next move.
        "EaseTransition": true     "EaseTransition"   : If false, the transition between start / end will be linear. Otherwise, the transition is slower from the beginning to the end and faster.

    }
}
```

If you need help, ask us at the Beat Saber Mod Group Discord Server:  
https://discord.gg/BeatSaberMods

## For developers

### Contributing to CameraPlus
In order to build this project, please create the file `CameraPlus.csproj.user` and add your Beat Saber directory path to it in the project directory.
This file should not be uploaded to GitHub and is in the .gitignore.

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <!-- Set "YOUR OWN" Beat Saber folder here to resolve most of the dependency paths! -->
    <BeatSaberDir>E:\Program Files (x86)\Steam\steamapps\common\Beat Saber</BeatSaberDir>
  </PropertyGroup>
</Project>
```

If you plan on adding any new dependencies which are located in the Beat Saber directory, it would be nice if you edited the paths to use `$(BeatSaberDir)` in `CameraPlus.csproj`

```xml
...
<Reference Include="BS_Utils">
  <HintPath>$(BeatSaberDir)\Plugins\BS_Utils.dll</HintPath>
</Reference>
<Reference Include="IPA.Loader">
  <HintPath>$(BeatSaberDir)\Beat Saber_Data\Managed\IPA.Loader.dll</HintPath>
</Reference>
...
```
