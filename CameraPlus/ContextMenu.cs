using System;
using UnityEngine;
using LogLevel = IPA.Logging.Logger.Level;

namespace CameraPlus
{
    public class ContextMenu : MonoBehaviour
    {
        internal Vector2 menuPos
        {
            get
            {
                return new Vector2(
                   Mathf.Min(mousePosition.x / (Screen.width / 1600f), (Screen.width * ( 0.806249998f / (Screen.width / 1600f)))),
                   Mathf.Min((Screen.height - mousePosition.y) / (Screen.height / 900f), (Screen.height * (0.555555556f / (Screen.height / 900f))))
                    );
            }
        }
        internal Vector2 mousePosition;
        internal bool showMenu;
        internal int MenuMode = 0;
        internal float amountMove = 0.1f;
        internal float amountRot = 0.1f;
        internal CameraPlusBehaviour parentBehaviour;
        internal string[] scriptName;
        internal int scriptPage;
        internal Texture2D texture = null;
        internal GUIStyle CustomEnableStyle = null;
        internal GUIStyle CustomDisableStyle = null;

        public void Awake()
        {
        }
        public void EnableMenu(Vector2 mousePos, CameraPlusBehaviour parentBehaviour)
        {
            this.enabled = true;
            mousePosition = mousePos;
            showMenu = true;
            this.parentBehaviour = parentBehaviour;
            MenuMode = 0;
            scriptName = null;
            scriptPage = 0;

            if (this.parentBehaviour.Config.LockScreen)
                texture = Utils.LoadTextureFromResources("CameraPlus.Resources.Lock.png");
            else
                texture = Utils.LoadTextureFromResources("CameraPlus.Resources.UnLock.png");
        }
        public void DisableMenu()
        {
            if (!this) return;
            this.enabled = false;
            showMenu = false;
        }
        void OnGUI()
        {

            if (showMenu)
            {
                Vector3 scale;
                float originalWidth = 1600f;
                float originalHeight = 900f;

                scale.x = Screen.width / originalWidth;
                scale.y = Screen.height / originalHeight;
                scale.z = 1;
                Matrix4x4 originalMatrix = GUI.matrix;
                GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, scale);
                //Layer boxes for Opacity
                GUI.Box(new Rect(menuPos.x - 5, menuPos.y, 310, 470), "CameraPlus" + parentBehaviour.name);
                GUI.Box(new Rect(menuPos.x - 5, menuPos.y, 310, 470), "CameraPlus" + parentBehaviour.name);
                GUI.Box(new Rect(menuPos.x - 5, menuPos.y, 310, 470), "CameraPlus" + parentBehaviour.name);

                CustomEnableStyle = new GUIStyle(GUI.skin.button);
                CustomEnableStyle.normal.background = CustomEnableStyle.active.background;
                CustomEnableStyle.hover.background = CustomEnableStyle.active.background;
                CustomDisableStyle = new GUIStyle(GUI.skin.button);

                if (MenuMode == 0)
                {
                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 25, 30, 30), texture))
                    {
                        parentBehaviour.Config.LockScreen = !parentBehaviour.Config.LockScreen;
                        parentBehaviour.Config.Save();
                        if (this.parentBehaviour.Config.LockScreen)
                            texture = Utils.LoadTextureFromResources("CameraPlus.Resources.Lock.png");
                        else
                            texture = Utils.LoadTextureFromResources("CameraPlus.Resources.UnLock.png");
                    }
                    GUI.Box(new Rect(menuPos.x + 35, menuPos.y + 25, 115, 30), new GUIContent(parentBehaviour.Config.LockScreen ? "Locked Screen" : "Unlocked Screen"));

                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 60, 145, 60), new GUIContent("Add New Camera")))
                    {
                        lock (Plugin.Instance.Cameras)
                        {
                            string cameraName = CameraUtilities.GetNextCameraName();
                            Logger.Log($"Adding new config with name {cameraName}.cfg");
                            CameraUtilities.AddNewCamera(cameraName);
                            CameraUtilities.ReloadCameras();
                            parentBehaviour.CloseContextMenu();
                        }
                    }
                    if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 60, 145, 60), new GUIContent("Duplicate\nSelected Camera")))
                    {
                        lock (Plugin.Instance.Cameras)
                        {
                            string cameraName = CameraUtilities.GetNextCameraName();
                            Logger.Log($"Adding {cameraName}", LogLevel.Notice);
                            CameraUtilities.AddNewCamera(cameraName, parentBehaviour.Config);
                            CameraUtilities.ReloadCameras();
                            parentBehaviour.CloseContextMenu();
                        }
                    }
                    if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 130, 145, 50), new GUIContent("Remove\nSelected Camera")))
                    {
                        lock (Plugin.Instance.Cameras)
                        {
                            if (CameraUtilities.RemoveCamera(parentBehaviour))
                            {
                                parentBehaviour._isCameraDestroyed = true;
                                parentBehaviour.CreateScreenRenderTexture();
                                parentBehaviour.CloseContextMenu();
                                Logger.Log("Camera removed!", LogLevel.Notice);
                            }
                        }
                    }

                    //First Person, Third Person, 360degree
                    GUI.Box(new Rect(menuPos.x, menuPos.y + 190, 300, 55), "Camera Mode");
                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 210, 95, 30), new GUIContent("First Person"), !parentBehaviour.Config.thirdPerson ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.thirdPerson = false;
                        parentBehaviour.Config.use360Camera = false;
                        parentBehaviour.ThirdPerson = parentBehaviour.Config.thirdPerson;
                        parentBehaviour.ThirdPersonPos = parentBehaviour.Config.Position;
                        parentBehaviour.ThirdPersonRot = parentBehaviour.Config.Rotation;

                        parentBehaviour.CreateScreenRenderTexture();
                        parentBehaviour.Config.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 105, menuPos.y + 210, 95, 30), new GUIContent("Third Person"), (parentBehaviour.Config.thirdPerson && !parentBehaviour.Config.use360Camera) ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.thirdPerson = true;
                        parentBehaviour.Config.use360Camera = false;
                        parentBehaviour.ThirdPersonPos = parentBehaviour.Config.Position;
                        parentBehaviour.ThirdPersonRot = parentBehaviour.Config.Rotation;

                        parentBehaviour.CreateScreenRenderTexture();
                        parentBehaviour.Config.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 205, menuPos.y + 210, 95, 30), new GUIContent("360 degree"), (parentBehaviour.Config.thirdPerson && parentBehaviour.Config.use360Camera) ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.thirdPerson = true;
                        parentBehaviour.Config.use360Camera = true;
                        parentBehaviour.ThirdPersonPos = parentBehaviour.Config.Position;
                        parentBehaviour.ThirdPersonRot = parentBehaviour.Config.Rotation;

                        parentBehaviour.CreateScreenRenderTexture();
                        parentBehaviour.Config.Save();
                    }

                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 250, 145, 40), new GUIContent("Display Object")))
                    {
                        MenuMode = 1;
                    }
                    if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 250, 145, 40), new GUIContent("Layout")))
                    {
                        MenuMode = 2;
                    }
                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 295, 145, 40), new GUIContent("Multiplayer")))
                    {
                        MenuMode = 3;
                    }
                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 340, 145, 40), new GUIContent("Profile Saver")))
                    {
                        MenuMode = 4;
                    }
                    if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 340, 145, 40), new GUIContent("MovementScript")))
                    {
                        MenuMode = 5;
                        scriptName = CameraUtilities.MovementScriptList();
                    }
                    /*
                    if (GUI.Button(new Rect(menuPos.x, menuPos.y + 345, 300, 30), new GUIContent(parentBehaviour.Config.Orthographics ? "Perspective" : "Orthographics")))
                    {
                        parentBehaviour.Config.Orthographics = !parentBehaviour.Config.Orthographics;
                        parentBehaviour.Config.Save();
                        parentBehaviour.CloseContextMenu();
                        parentBehaviour.CreateScreenRenderTexture();
                    }
                    */
                    /*
                    if (GUI.Button(new Rect(menuPos.x, menuPos.y + 385, 300, 30), new GUIContent("Spawn 38 Cameras")))
                    {
                        parentBehaviour.StartCoroutine(CameraUtilities.Spawn38Cameras());
                        parentBehaviour.CloseContextMenu();
                    }*/
                    if (GUI.Button(new Rect(menuPos.x, menuPos.y + 430, 300, 30), new GUIContent("Close Menu")))
                    {
                        parentBehaviour.CloseContextMenu();
                    }
                }
                else if (MenuMode == 1)
                {
                    //Display Preview QUad
                    GUI.Box(new Rect(menuPos.x, menuPos.y + 25, 300, 55), "Display ThirdPersonCamera PrevewQuad");

                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 45, 145, 30), new GUIContent("Hide CameraQuad"), !parentBehaviour.Config.showThirdPersonCamera ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.showThirdPersonCamera = false;
                        parentBehaviour.Config.Save();
                        parentBehaviour.CreateScreenRenderTexture();
                    }

                    if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 45, 145, 30), new GUIContent("Display CameraQuad"), parentBehaviour.Config.showThirdPersonCamera ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.showThirdPersonCamera = true;
                        parentBehaviour.Config.Save();
                        parentBehaviour.CreateScreenRenderTexture();
                    }

                    //FirstPerson Camera Upright
                    GUI.Box(new Rect(menuPos.x, menuPos.y + 80, 300, 55), "Force FirstPerson Camera Upright");
                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 100, 145, 30), new GUIContent("Disable Upright"), !parentBehaviour.Config.forceFirstPersonUpRight ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.forceFirstPersonUpRight = false;
                        parentBehaviour.Config.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 100, 145, 30), new GUIContent("Enable Upright"), parentBehaviour.Config.forceFirstPersonUpRight ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.forceFirstPersonUpRight = true;
                        parentBehaviour.Config.Save();
                    }

                    //TransportWall
                    GUI.Box(new Rect(menuPos.x, menuPos.y + 135, 300, 55), "Transport Wall");
                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 155, 145, 30), new GUIContent("Disable Transportwall"), !parentBehaviour.Config.transparentWalls ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.transparentWalls = false;
                        parentBehaviour.SetCullingMask();
                        parentBehaviour.Config.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 155, 145, 30), new GUIContent("Enable Transportwall"), parentBehaviour.Config.transparentWalls ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.transparentWalls = true;
                        parentBehaviour.SetCullingMask();
                        parentBehaviour.Config.Save();
                    }

                    //Debri
                    GUI.Box(new Rect(menuPos.x, menuPos.y + 190, 300, 55), "Display Debri");
                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 210, 95, 30), new GUIContent("Link InGame"), parentBehaviour.Config.debri == "link" ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.debri = "link";
                        parentBehaviour.SetCullingMask();
                        parentBehaviour.Config.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 105, menuPos.y + 210, 95, 30), new GUIContent("Forced Display"), parentBehaviour.Config.debri == "show" ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.debri = "show";
                        parentBehaviour.SetCullingMask();
                        parentBehaviour.Config.Save();
                    }

                    if (GUI.Button(new Rect(menuPos.x + 205, menuPos.y + 210, 95, 30), new GUIContent("Forced Hide"), parentBehaviour.Config.debri == "hide" ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.debri = "hide";
                        parentBehaviour.SetCullingMask();
                        parentBehaviour.Config.Save();
                    }

                    //Display UI
                    GUI.Box(new Rect(menuPos.x, menuPos.y + 245, 300, 55), "Display Official UI");
                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 265, 145, 30), new GUIContent("Show UI"), !parentBehaviour.Config.HideUI ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.HideUI = false;
                        parentBehaviour.SetCullingMask();
                        parentBehaviour.Config.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 265, 145, 30), new GUIContent("Hide UI"), parentBehaviour.Config.HideUI ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.HideUI = true;
                        parentBehaviour.SetCullingMask();
                        parentBehaviour.Config.Save();
                    }

                    //Display Custom and VMC Avatar
                    GUI.Box(new Rect(menuPos.x, menuPos.y + 300, 300, 55), "Display CustomAvatar/VMCAvatar");
                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 320, 95, 30), new GUIContent("Show Avatar"), parentBehaviour.Config.avatar == 1 ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.avatar = 1;
                        parentBehaviour.SetCullingMask();
                        parentBehaviour.Config.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 105, menuPos.y + 320, 95, 30), new GUIContent("Hide Avatar"), parentBehaviour.Config.avatar == 0 ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.avatar = 0;
                        parentBehaviour.SetCullingMask();
                        parentBehaviour.Config.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 205, menuPos.y + 320, 95, 30), new GUIContent("Avatar Only"), parentBehaviour.Config.avatar == 2 ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.avatar = 2;
                        parentBehaviour.SetCullingMask();
                        parentBehaviour.Config.Save();
                    }


                    //Camera Tracking to NoodleExtensions AssignPlayerToTrack
                    GUI.Box(new Rect(menuPos.x, menuPos.y + 355, 300, 55), "Tracking NoodleExtension PlayerToTrack");
                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 375, 145, 30), new GUIContent("Tracking On"), parentBehaviour.Config.NoodleTrack ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.NoodleTrack = true;
                        parentBehaviour.Config.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 375, 145, 30), new GUIContent("Tracking Off"), !parentBehaviour.Config.NoodleTrack ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.NoodleTrack = false;
                        parentBehaviour.Config.Save();
                    }

                    if (GUI.Button(new Rect(menuPos.x, menuPos.y + 430, 300, 30), new GUIContent("Close CameraMode Menu")))
                    {
                        MenuMode = 0;
                    }
                }
                else if (MenuMode == 2)
                {
                    if (GUI.Button(new Rect(menuPos.x, menuPos.y + 25, 290, 30), new GUIContent("Reset Camera Position and Rotation")))
                    {

                        parentBehaviour.Config.Position = parentBehaviour.Config.DefaultPosition;
                        parentBehaviour.Config.Rotation = parentBehaviour.Config.DefaultRotation;
                        parentBehaviour.Config.FirstPersonPositionOffset = parentBehaviour.Config.DefaultFirstPersonPositionOffset;
                        parentBehaviour.Config.FirstPersonRotationOffset = parentBehaviour.Config.DefaultFirstPersonRotationOffset;
                        parentBehaviour.ThirdPersonPos = parentBehaviour.Config.DefaultPosition;
                        parentBehaviour.ThirdPersonRot = parentBehaviour.Config.DefaultRotation;
                        parentBehaviour.Config.Save();
                        parentBehaviour.CloseContextMenu();
                    }
                    //Layer
                    GUI.Box(new Rect(menuPos.x, menuPos.y + 60, 140, 55), "Layer: " + parentBehaviour.Config.layer);
                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 80, 60, 30), new GUIContent("-")))
                    {
                        parentBehaviour.Config.layer--;
                        parentBehaviour.CreateScreenRenderTexture();
                        parentBehaviour.Config.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 75, menuPos.y + 80, 60, 30), new GUIContent("+")))
                    {
                        parentBehaviour.Config.layer++;
                        parentBehaviour.CreateScreenRenderTexture();
                        parentBehaviour.Config.Save();
                    }
                    //FOV
                    GUI.Box(new Rect(menuPos.x + 155, menuPos.y + 60, 140, 55), "FOV: " + parentBehaviour.Config.fov);
                    if (GUI.Button(new Rect(menuPos.x + 160, menuPos.y + 80, 60, 30), new GUIContent("-")))
                    {
                        parentBehaviour.Config.fov--;
                        parentBehaviour.SetFOV();
                        parentBehaviour.CreateScreenRenderTexture();
                        parentBehaviour.Config.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 230, menuPos.y + 80, 60, 30), new GUIContent("+")))
                    {
                        parentBehaviour.Config.fov++;
                        parentBehaviour.SetFOV();
                        parentBehaviour.CreateScreenRenderTexture();
                        parentBehaviour.Config.Save();
                    }
                    //Render Scale
                    GUI.Box(new Rect(menuPos.x, menuPos.y + 120, 140, 55), "Render Scale: " + parentBehaviour.Config.renderScale.ToString("F1"));
                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 140, 60, 30), new GUIContent("-")))
                    {
                        parentBehaviour.Config.renderScale -= 0.1f;
                        parentBehaviour.CreateScreenRenderTexture();
                        parentBehaviour.Config.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 75, menuPos.y + 140, 60, 30), new GUIContent("+")))
                    {
                        parentBehaviour.Config.renderScale += 0.1f;
                        parentBehaviour.CreateScreenRenderTexture();
                        parentBehaviour.Config.Save();
                    }
                    //Fit Canvas
                    if (GUI.Button(new Rect(menuPos.x + 155, menuPos.y + 140, 140, 30), new GUIContent(parentBehaviour.Config.fitToCanvas ? " Don't Fit To Canvas" : "Fit To Canvas")))
                    {
                        parentBehaviour.Config.fitToCanvas = !parentBehaviour.Config.fitToCanvas;
                        parentBehaviour.CreateScreenRenderTexture();
                        parentBehaviour.Config.Save();
                    }
                    //Amount of Movemnet
                    GUI.Box(new Rect(menuPos.x, menuPos.y + 180, 210, 55), "Amount movement : " + amountMove.ToString("F2"));
                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 200, 60, 30), new GUIContent("0.01")))
                    {
                        amountMove = 0.01f;
                        parentBehaviour.CreateScreenRenderTexture();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 75, menuPos.y + 200, 60, 30), new GUIContent("0.10")))
                    {
                        amountMove = 0.1f;
                        parentBehaviour.CreateScreenRenderTexture();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 145, menuPos.y + 200, 60, 30), new GUIContent("1.00")))
                    {
                        amountMove = 1.0f;
                        parentBehaviour.CreateScreenRenderTexture();
                    }
                    //X Position
                    GUI.Box(new Rect(menuPos.x, menuPos.y + 240, 95, 55), "X Pos :" +
                        (parentBehaviour.Config.use360Camera ? parentBehaviour.Config.cam360RightOffset.ToString("F2") : (parentBehaviour.Config.thirdPerson ? parentBehaviour.Config.posx.ToString("F2") : parentBehaviour.Config.firstPersonPosOffsetX.ToString("F2"))));
                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 260, 40, 30), new GUIContent("-")))
                    {
                        if (parentBehaviour.Config.use360Camera)
                            parentBehaviour.Config.cam360RightOffset -= amountMove;
                        else if (parentBehaviour.Config.thirdPerson)
                            parentBehaviour.Config.posx -= amountMove;
                        else
                            parentBehaviour.Config.firstPersonPosOffsetX -= amountMove;
                        parentBehaviour.CreateScreenRenderTexture();
                        parentBehaviour.Config.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 50, menuPos.y + 260, 40, 30), new GUIContent("+")))
                    {
                        if (parentBehaviour.Config.use360Camera)
                            parentBehaviour.Config.cam360RightOffset += amountMove;
                        else if (parentBehaviour.Config.thirdPerson)
                            parentBehaviour.Config.posx += amountMove;
                        else
                            parentBehaviour.Config.firstPersonPosOffsetX += amountMove;
                        parentBehaviour.CreateScreenRenderTexture();
                        parentBehaviour.Config.Save();
                    }
                    //Y Position
                    GUI.Box(new Rect(menuPos.x + 100, menuPos.y + 240, 95, 55), "Y Pos :" +
                        (parentBehaviour.Config.use360Camera ? parentBehaviour.Config.cam360UpOffset.ToString("F2") : (parentBehaviour.Config.thirdPerson ? parentBehaviour.Config.posy.ToString("F2") : parentBehaviour.Config.firstPersonPosOffsetY.ToString("F2"))));
                    if (GUI.Button(new Rect(menuPos.x + 105, menuPos.y + 260, 40, 30), new GUIContent("-")))
                    {
                        if (parentBehaviour.Config.use360Camera)
                            parentBehaviour.Config.cam360UpOffset -= amountMove;
                        else if (parentBehaviour.Config.thirdPerson)
                            parentBehaviour.Config.posy -= amountMove;
                        else
                            parentBehaviour.Config.firstPersonPosOffsetY -= amountMove;
                        parentBehaviour.CreateScreenRenderTexture();
                        parentBehaviour.Config.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 260, 40, 30), new GUIContent("+")))
                    {
                        if (parentBehaviour.Config.use360Camera)
                            parentBehaviour.Config.cam360UpOffset += amountMove;
                        else if (parentBehaviour.Config.thirdPerson)
                            parentBehaviour.Config.posy += amountMove;
                        else
                            parentBehaviour.Config.firstPersonPosOffsetY += amountMove;
                        parentBehaviour.CreateScreenRenderTexture();
                        parentBehaviour.Config.Save();
                    }
                    //Z Position
                    GUI.Box(new Rect(menuPos.x + 205, menuPos.y + 240, 95, 55), "Z Pos :" +
                        (parentBehaviour.Config.use360Camera ? parentBehaviour.Config.cam360ForwardOffset.ToString("F2") : (parentBehaviour.Config.thirdPerson ? parentBehaviour.Config.posz.ToString("F2") : parentBehaviour.Config.firstPersonPosOffsetZ.ToString("F2"))));
                    if (GUI.Button(new Rect(menuPos.x + 210, menuPos.y + 260, 40, 30), new GUIContent("-")))
                    {
                        if (parentBehaviour.Config.use360Camera)
                            parentBehaviour.Config.cam360ForwardOffset -= amountMove;
                        else if (parentBehaviour.Config.thirdPerson)
                            parentBehaviour.Config.posz -= amountMove;
                        else
                            parentBehaviour.Config.firstPersonPosOffsetZ -= amountMove;
                        parentBehaviour.CreateScreenRenderTexture();
                        parentBehaviour.Config.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 255, menuPos.y + 260, 40, 30), new GUIContent("+")))
                    {
                        if (parentBehaviour.Config.use360Camera)
                            parentBehaviour.Config.cam360ForwardOffset += amountMove;
                        else if (parentBehaviour.Config.thirdPerson)
                            parentBehaviour.Config.posz += amountMove;
                        else
                            parentBehaviour.Config.firstPersonPosOffsetZ += amountMove;
                        parentBehaviour.CreateScreenRenderTexture();
                        parentBehaviour.Config.Save();
                    }
                    //Amount of Rotation
                    GUI.Box(new Rect(menuPos.x, menuPos.y + 300, 290, 55), "Amount rotation : " + amountRot.ToString("F2"));
                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 320, 50, 30), new GUIContent("0.01")))
                    {
                        amountRot = 0.01f;
                        parentBehaviour.CreateScreenRenderTexture();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 60, menuPos.y + 320, 50, 30), new GUIContent("0.10")))
                    {
                        amountRot = 0.1f;
                        parentBehaviour.CreateScreenRenderTexture();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 115, menuPos.y + 320, 50, 30), new GUIContent("1.00")))
                    {
                        amountRot = 1.0f;
                        parentBehaviour.CreateScreenRenderTexture();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 170, menuPos.y + 320, 50, 30), new GUIContent("10")))
                    {
                        amountRot = 10.0f;
                        parentBehaviour.CreateScreenRenderTexture();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 225, menuPos.y + 320, 50, 30), new GUIContent("45")))
                    {
                        amountRot = 45.0f;
                        parentBehaviour.CreateScreenRenderTexture();
                    }
                    //X Rotation
                    GUI.Box(new Rect(menuPos.x, menuPos.y + 360, 95, 55), "X Rot :" +
                        (parentBehaviour.Config.use360Camera ? parentBehaviour.Config.cam360XTilt.ToString("F2") : (parentBehaviour.Config.thirdPerson ? parentBehaviour.Config.angx.ToString("F2") : parentBehaviour.Config.firstPersonRotOffsetX.ToString("F2"))));
                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 380, 40, 30), new GUIContent("-")))
                    {
                        if (parentBehaviour.Config.use360Camera)
                            parentBehaviour.Config.cam360XTilt -= amountRot;
                        else if (parentBehaviour.Config.thirdPerson)
                            parentBehaviour.Config.angx -= amountRot;
                        else
                            parentBehaviour.Config.firstPersonRotOffsetX -= amountRot;
                        parentBehaviour.CreateScreenRenderTexture();
                        parentBehaviour.Config.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 50, menuPos.y + 380, 40, 30), new GUIContent("+")))
                    {
                        if (parentBehaviour.Config.use360Camera)
                            parentBehaviour.Config.cam360XTilt += amountRot;
                        else if (parentBehaviour.Config.thirdPerson)
                            parentBehaviour.Config.angx += amountRot;
                        else
                            parentBehaviour.Config.firstPersonRotOffsetX += amountRot;
                        parentBehaviour.CreateScreenRenderTexture();
                        parentBehaviour.Config.Save();
                    }
                    //Y Rotation
                    GUI.Box(new Rect(menuPos.x + 100, menuPos.y + 360, 95, 55), "Y Rot :" +
                        (parentBehaviour.Config.use360Camera ? parentBehaviour.Config.cam360YTilt.ToString("F2") : (parentBehaviour.Config.thirdPerson ? parentBehaviour.Config.angy.ToString("F2") : parentBehaviour.Config.firstPersonRotOffsetY.ToString("F2"))));
                    if (GUI.Button(new Rect(menuPos.x + 105, menuPos.y + 380, 40, 30), new GUIContent("-")))
                    {
                        if (parentBehaviour.Config.use360Camera)
                            parentBehaviour.Config.cam360YTilt -= amountRot;
                        else if (parentBehaviour.Config.thirdPerson)
                            parentBehaviour.Config.angy -= amountRot;
                        else
                            parentBehaviour.Config.firstPersonRotOffsetY -= amountRot;
                        parentBehaviour.CreateScreenRenderTexture();
                        parentBehaviour.Config.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 380, 40, 30), new GUIContent("+")))
                    {
                        if (parentBehaviour.Config.use360Camera)
                            parentBehaviour.Config.cam360YTilt += amountRot;
                        else if (parentBehaviour.Config.thirdPerson)
                            parentBehaviour.Config.angy += amountRot;
                        else
                            parentBehaviour.Config.firstPersonRotOffsetY += amountRot;
                        parentBehaviour.CreateScreenRenderTexture();
                        parentBehaviour.Config.Save();
                    }
                    //Z Rotation
                    GUI.Box(new Rect(menuPos.x + 205, menuPos.y + 360, 95, 55), "Z Rot :" +
                        (parentBehaviour.Config.use360Camera ? parentBehaviour.Config.cam360ZTilt.ToString("F2") : (parentBehaviour.Config.thirdPerson ? parentBehaviour.Config.angz.ToString("F2") : parentBehaviour.Config.firstPersonRotOffsetZ.ToString("F2"))));
                    if (GUI.Button(new Rect(menuPos.x + 210, menuPos.y + 380, 40, 30), new GUIContent("-")))
                    {
                        if (parentBehaviour.Config.use360Camera)
                            parentBehaviour.Config.cam360ZTilt -= amountRot;
                        else if (parentBehaviour.Config.thirdPerson)
                            parentBehaviour.Config.angz -= amountRot;
                        else
                            parentBehaviour.Config.firstPersonRotOffsetZ -= amountRot;
                        parentBehaviour.CreateScreenRenderTexture();
                        parentBehaviour.Config.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 255, menuPos.y + 380, 40, 30), new GUIContent("+")))
                    {
                        if (parentBehaviour.Config.use360Camera)
                            parentBehaviour.Config.cam360ZTilt += amountRot;
                        else if (parentBehaviour.Config.thirdPerson)
                            parentBehaviour.Config.angz += amountRot;
                        else
                            parentBehaviour.Config.firstPersonRotOffsetZ += amountRot;
                        parentBehaviour.CreateScreenRenderTexture();
                        parentBehaviour.Config.Save();
                    }
                    //Close
                    if (GUI.Button(new Rect(menuPos.x, menuPos.y + 430, 300, 30), new GUIContent("Close Layout Menu")))
                    {
                        MenuMode = 0;
                    }
                }
                else if (MenuMode == 3)
                {
                    //MultiPlayerOffset
                    GUI.Box(new Rect(menuPos.x, menuPos.y + 25, 300, 90), "Multiplayer tracking camera");
                    if (GUI.Button(new Rect(menuPos.x +5, menuPos.y + 45, 55, 30), new GUIContent("Player1"), parentBehaviour.Config.MultiPlayerNumber == 1 ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.MultiPlayerNumber=1;
                        parentBehaviour.Config.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 65, menuPos.y + 45, 55, 30), new GUIContent("Player2"), parentBehaviour.Config.MultiPlayerNumber == 2 ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.MultiPlayerNumber = 2;
                        parentBehaviour.Config.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 125, menuPos.y + 45, 55, 30), new GUIContent("Player3"), parentBehaviour.Config.MultiPlayerNumber == 3 ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.MultiPlayerNumber = 3;
                        parentBehaviour.Config.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 185, menuPos.y + 45, 55, 30), new GUIContent("Player4"), parentBehaviour.Config.MultiPlayerNumber == 4 ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.MultiPlayerNumber = 4;
                        parentBehaviour.Config.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 245, menuPos.y + 45, 55, 30), new GUIContent("Player5"), parentBehaviour.Config.MultiPlayerNumber == 5 ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.MultiPlayerNumber = 5;
                        parentBehaviour.Config.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 75, menuPos.y + 80, 150, 30), new GUIContent("Tracking Camera Off"), parentBehaviour.Config.MultiPlayerNumber == 0 ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.MultiPlayerNumber = 0;
                        parentBehaviour.Config.Save();
                    }

                    //Display Name, Rand and Score
                    GUI.Box(new Rect(menuPos.x, menuPos.y + 115, 300, 55), "Display Multiplayer Name, Rank and Score");
                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 135, 145, 30), new GUIContent("Show Info"), parentBehaviour.Config.DisplayMultiPlayerNameInfo ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.DisplayMultiPlayerNameInfo = true;
                        parentBehaviour.Config.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 135, 145, 30), new GUIContent("Hide Info"), !parentBehaviour.Config.DisplayMultiPlayerNameInfo ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.DisplayMultiPlayerNameInfo = false;
                        parentBehaviour.Config.Save();
                    }

                    //Close
                    if (GUI.Button(new Rect(menuPos.x, menuPos.y + 430, 300, 30), new GUIContent("Close Multiplayer Menu")))
                    {
                        MenuMode = 0;
                    }
                }
                else if (MenuMode == 4)
                {
                    if (GUI.Button(new Rect(menuPos.x, menuPos.y + 25, 140, 30), new GUIContent("<")))
                        CameraProfiles.TrySetLast(CameraProfiles.currentlySelected);
                    if (GUI.Button(new Rect(menuPos.x + 155, menuPos.y + 25, 140, 30), new GUIContent(">")))
                        CameraProfiles.SetNext(CameraProfiles.currentlySelected);
                    if (GUI.Button(new Rect(menuPos.x + 30, menuPos.y + 65, 230, 80), new GUIContent("Currently Selected:\n" + CameraProfiles.currentlySelected)))
                        CameraProfiles.SetNext(CameraProfiles.currentlySelected);
                    if (GUI.Button(new Rect(menuPos.x, menuPos.y + 155, 140, 30), new GUIContent("Save")))
                        CameraProfiles.SaveCurrent();
                    if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 155, 140, 30), new GUIContent("Delete")))
                    {
                        if (!Plugin.Instance._rootConfig.ProfileLoadCopyMethod)
                            Plugin.Instance._profileChanger.ProfileChange(null);
                        CameraProfiles.DeleteProfile(CameraProfiles.currentlySelected);
                        CameraProfiles.TrySetLast();
                    }
                    if (GUI.Button(new Rect(menuPos.x, menuPos.y + 195, 290, 30), new GUIContent("Load Selected")))
                        Plugin.Instance._profileChanger.ProfileChange(CameraProfiles.currentlySelected);
                    if (GUI.Button(new Rect(menuPos.x, menuPos.y + 245, 290, 30), new GUIContent(Plugin.Instance._rootConfig.ProfileSceneChange ? "To SceneChange Off" : "To SceneChange On")))
                    {
                        Plugin.Instance._rootConfig.ProfileSceneChange = !Plugin.Instance._rootConfig.ProfileSceneChange;
                        Plugin.Instance._rootConfig.Save();
                    }
                    if (Plugin.Instance._rootConfig.ProfileSceneChange)
                    {
                        GUI.Box(new Rect(menuPos.x + 35, menuPos.y + 275, 260, 30), "Menu Scene Profile : " + (Plugin.Instance._rootConfig.MenuProfile));
                        GUI.Box(new Rect(menuPos.x + 35, menuPos.y + 305, 260, 30), "Game Scene Profile : " + (Plugin.Instance._rootConfig.GameProfile));
                        GUI.Box(new Rect(menuPos.x + 35, menuPos.y + 335, 260, 30), "Multiplay Profile : " + (Plugin.Instance._rootConfig.MultiplayerProfile));
                        if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 275, 30, 30), "X"))
                        {
                            if (Plugin.Instance._rootConfig.MenuProfile!=string.Empty)
                                Plugin.Instance._rootConfig.MenuProfile = string.Empty;
                            Plugin.Instance._rootConfig.Save();
                        }
                        if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 305, 30, 30), "X"))
                        {
                            if (Plugin.Instance._rootConfig.GameProfile != string.Empty)
                                Plugin.Instance._rootConfig.GameProfile = string.Empty;
                            Plugin.Instance._rootConfig.Save();
                        }
                        if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 335, 30, 30), "X"))
                        {
                            if (Plugin.Instance._rootConfig.MultiplayerProfile != string.Empty)
                                Plugin.Instance._rootConfig.MultiplayerProfile = string.Empty;
                            Plugin.Instance._rootConfig.Save();
                        }
                        if (GUI.Button(new Rect(menuPos.x, menuPos.y + 365, 140, 25), new GUIContent("Set Menu Selected")))
                        {
                            Plugin.Instance._rootConfig.MenuProfile = CameraProfiles.currentlySelected;
                            Plugin.Instance._rootConfig.Save();
                        }
                        if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 365, 140, 25), new GUIContent("Set Game Selected")))
                        {
                            Plugin.Instance._rootConfig.GameProfile = CameraProfiles.currentlySelected;
                            Plugin.Instance._rootConfig.Save();
                        }
                        if (GUI.Button(new Rect(menuPos.x , menuPos.y + 395, 290, 25), new GUIContent("Set Multiplayer Selected")))
                        {
                            Plugin.Instance._rootConfig.MultiplayerProfile = CameraProfiles.currentlySelected;
                            Plugin.Instance._rootConfig.Save();
                        }
                    }
                    /*
                    if (GUI.Button(new Rect(menuPos.x, menuPos.y + 390, 290, 30), new GUIContent(Plugin.Instance._rootConfig.ProfileLoadCopyMethod ? "To Folder Reference Method" : "To File Copy Method")))
                    {
                        Plugin.Instance._rootConfig.ProfileLoadCopyMethod = !Plugin.Instance._rootConfig.ProfileLoadCopyMethod;
                        Plugin.Instance._rootConfig.Save();
                        Plugin.Instance._profileChanger.ProfileChange(null);
                    }
                    */
                    if (GUI.Button(new Rect(menuPos.x, menuPos.y + 430, 300, 30), new GUIContent("Close Profile Menu")))
                        MenuMode = 0;
                }
                else if (MenuMode == 5)
                {
                    GUI.Box(new Rect(menuPos.x, menuPos.y + 25, 300, 55), new GUIContent("MovementScript Method"));
                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 45, 145, 30), new GUIContent("Tick UnityTimer"), !parentBehaviour.Config.movementAudioSync ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.movementAudioSync = false;
                        parentBehaviour.SetCullingMask();
                        parentBehaviour.Config.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 45, 145, 30), new GUIContent("Tick AudioTimer"), parentBehaviour.Config.movementAudioSync ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.movementAudioSync = true;
                        parentBehaviour.SetCullingMask();
                        parentBehaviour.Config.Save();
                    }

                    if (GUI.Button(new Rect(menuPos.x, menuPos.y + 105, 80, 30), new GUIContent("<")))
                    {
                        if (scriptPage > 0) scriptPage--;
                    }
                    GUI.Box(new Rect(menuPos.x + 80, menuPos.y + 105, 140, 30), new GUIContent($"{scriptPage + 1} / {Math.Ceiling(Decimal.Parse(scriptName.Length.ToString()) / 5)}"));
                    if (GUI.Button(new Rect(menuPos.x + 220, menuPos.y + 105, 80, 30), new GUIContent(">")))
                    {
                        if (scriptPage < Math.Ceiling(Decimal.Parse(scriptName.Length.ToString()) / 5) - 1) scriptPage++;
                    }
                    for (int i = scriptPage * 5; i < scriptPage * 5 + 5; i++)
                    {
                        if (i < scriptName.Length)
                        {
                            if (GUI.Button(new Rect(menuPos.x, menuPos.y + (i - scriptPage * 5) * 35 + 145, 300, 30), new GUIContent(scriptName[i]), CameraUtilities.CurrentMovementScript(parentBehaviour.Config.movementScriptPath) == scriptName[i] ? CustomEnableStyle : CustomDisableStyle))
                            {
                                parentBehaviour.Config.movementScriptPath = scriptName[i];
                                parentBehaviour.Config.Save();
                                parentBehaviour.AddMovementScript();
                            }
                        }
                    }
                    if (GUI.Button(new Rect(menuPos.x + 50, menuPos.y + 330, 200, 40), new GUIContent("Movement Off")))
                    {
                        if (parentBehaviour.Config.movementScriptPath != string.Empty)
                        {
                            parentBehaviour.Config.movementScriptPath = String.Empty;
                            parentBehaviour.Config.Save();
                            parentBehaviour.ClearMovementScript();
                        }
                    }
                    //Close
                    if (GUI.Button(new Rect(menuPos.x, menuPos.y + 430, 300, 30), new GUIContent("Close MovementScript Menu")))
                    {
                        MenuMode = 0;
                    }
                }

                GUI.matrix = originalMatrix;
            }
        }
    }
}
