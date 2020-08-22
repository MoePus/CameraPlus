using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        internal bool layoutMode = false;
        internal bool profileMode = false;
        internal float amountMove = 0.1f;
        internal float amountRot = 0.1f;
        internal CameraPlusBehaviour parentBehaviour;
        public void Awake()
        {
        }
        public void EnableMenu(Vector2 mousePos, CameraPlusBehaviour parentBehaviour)
        {
            this.enabled = true;
     //       Console.WriteLine("Enable Menu");
            mousePosition = mousePos;
            showMenu = true;
            this.parentBehaviour = parentBehaviour;
            layoutMode = false;
            profileMode = false;
        }
        public void DisableMenu()
        {
            if (!this) return;
            this.enabled = false;
     //       Console.WriteLine("Disable Menu");
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
                if (!layoutMode && !profileMode)
                {
                    if (GUI.Button(new Rect(menuPos.x, menuPos.y + 25, 120, 30), new GUIContent("Add New Camera")))
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
                    if (GUI.Button(new Rect(menuPos.x + 130, menuPos.y + 25, 170, 30), new GUIContent("Remove Selected Camera")))
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
                    if (GUI.Button(new Rect(menuPos.x, menuPos.y + 65, 170, 30), new GUIContent("Duplicate Selected Camera")))
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
                    if (GUI.Button(new Rect(menuPos.x + 180, menuPos.y + 65, 120, 30), new GUIContent("Layout")))
                    {
                        layoutMode = true;
                    }
                    if (GUI.Button(new Rect(menuPos.x, menuPos.y + 105, 120, 30), 
                        new GUIContent(parentBehaviour.Config.use360Camera? "First Person" : parentBehaviour.Config.thirdPerson ? " 360 Third Person" : "Third Person" )))
                    {
                        if(parentBehaviour.Config.use360Camera)
                        {
                            parentBehaviour.Config.thirdPerson = !parentBehaviour.Config.thirdPerson;
                            parentBehaviour.ThirdPerson = parentBehaviour.Config.thirdPerson;
                            parentBehaviour.ThirdPersonPos = parentBehaviour.Config.Position;
                            parentBehaviour.ThirdPersonRot = parentBehaviour.Config.Rotation;
                            parentBehaviour.Config.use360Camera = false;
                        }
                        else if(parentBehaviour.Config.thirdPerson)
                        {
                            parentBehaviour.Config.use360Camera = true;
                        }
                        else
                        {
                            parentBehaviour.Config.thirdPerson = !parentBehaviour.Config.thirdPerson;
                            parentBehaviour.ThirdPerson = parentBehaviour.Config.thirdPerson;
                            parentBehaviour.ThirdPersonPos = parentBehaviour.Config.Position;
                            parentBehaviour.ThirdPersonRot = parentBehaviour.Config.Rotation;
                        }
                        //      FirstPersonOffset = Config.FirstPersonPositionOffset;
                        //     FirstPersonRotationOffset = Config.FirstPersonRotationOffset;
                        parentBehaviour.CreateScreenRenderTexture();
                        parentBehaviour.CloseContextMenu();
                        parentBehaviour.Config.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 130, menuPos.y + 105, 170, 30), new GUIContent(parentBehaviour.Config.showThirdPersonCamera ? "Hide Third Person Camera" : "Show Third Person Camera")))
                    {

                        parentBehaviour.Config.showThirdPersonCamera = !parentBehaviour.Config.showThirdPersonCamera;
                        parentBehaviour.Config.Save();
                        parentBehaviour.CreateScreenRenderTexture();
                        parentBehaviour.CloseContextMenu();
                    }
                    if (GUI.Button(new Rect(menuPos.x, menuPos.y + 145, 170, 30), new GUIContent(parentBehaviour.Config.forceFirstPersonUpRight ? "Don't Force Camera Upright" : "Force Camera Upright")))
                    {

                        parentBehaviour.Config.forceFirstPersonUpRight = !parentBehaviour.Config.forceFirstPersonUpRight;
                        parentBehaviour.Config.Save();
                        parentBehaviour.CloseContextMenu();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 180, menuPos.y + 145, 120, 30), new GUIContent(parentBehaviour.Config.transparentWalls ? "Solid Walls" : "Transparent Walls")))
                    {
                        parentBehaviour.Config.transparentWalls = !parentBehaviour.Config.transparentWalls;
                        parentBehaviour.SetCullingMask();
                        parentBehaviour.CloseContextMenu();
                        parentBehaviour.Config.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x, menuPos.y + 185, 120, 30), new GUIContent(parentBehaviour.Config.avatar == 2 ? "Hide Avatar" : parentBehaviour.Config.avatar == 0 ? "Show Avatar" : "Avatar Only")))
                    {
                        parentBehaviour.Config.avatar = (parentBehaviour.Config.avatar + 1) % 3;
                        parentBehaviour.SetCullingMask();
                        parentBehaviour.CloseContextMenu();
                        parentBehaviour.Config.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 130, menuPos.y + 185, 170, 30), new GUIContent(parentBehaviour.Config.debri=="link" ? "Forced display Debri" : parentBehaviour.Config.debri == "show" ? "Forced non-display Debri" : "Debri Linked In-Game")))
                    {
                        if (parentBehaviour.Config.debri == "link")
                            parentBehaviour.Config.debri = "show";
                        else if (parentBehaviour.Config.debri=="show")
                            parentBehaviour.Config.debri = "hide";
                        else
                            parentBehaviour.Config.debri = "link";
                        parentBehaviour.SetCullingMask();
                        parentBehaviour.CloseContextMenu();
                        parentBehaviour.Config.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x, menuPos.y + 225, 120, 30), new GUIContent(parentBehaviour.Config.displayUI ? "Show UI" : "Hide UI")))
                    {
                        parentBehaviour.Config.displayUI = !parentBehaviour.Config.displayUI;
                        parentBehaviour.SetCullingMask();
                        parentBehaviour.CloseContextMenu();
                        parentBehaviour.Config.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x+130, menuPos.y + 225, 170, 30), new GUIContent(parentBehaviour.Config.movementAudioSync ? "ScriptSyncUnity" : "ScriptSyncAudio")))
                    {
                        parentBehaviour.Config.movementAudioSync = !parentBehaviour.Config.movementAudioSync;
                        parentBehaviour.SetCullingMask();
                        parentBehaviour.CloseContextMenu();
                        parentBehaviour.Config.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x, menuPos.y + 265, 300, 30), new GUIContent("Profile Saver")))
                    {
                        profileMode = true;
                    }
                    if (GUI.Button(new Rect(menuPos.x, menuPos.y + 385, 300, 30), new GUIContent("Spawn 38 Cameras")))
                    {
                        parentBehaviour.StartCoroutine(CameraUtilities.Spawn38Cameras());
                        parentBehaviour.CloseContextMenu();
                    }
                    if (GUI.Button(new Rect(menuPos.x, menuPos.y + 430, 300, 30), new GUIContent("Close Menu")))
                    {
                        parentBehaviour.CloseContextMenu();
                    }
                }
                else if (layoutMode)
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
                    if (GUI.Button(new Rect(menuPos.x+5, menuPos.y + 80, 60, 30), new GUIContent("-")))
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
                    GUI.Box(new Rect(menuPos.x+155, menuPos.y + 60, 140, 55), "FOV: " + parentBehaviour.Config.fov);
                    if (GUI.Button(new Rect(menuPos.x+160, menuPos.y + 80, 60, 30), new GUIContent("-")))
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
                        if(parentBehaviour.Config.use360Camera)
                            parentBehaviour.Config.cam360RightOffset -= amountMove;
                        else if(parentBehaviour.Config.thirdPerson)
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
                    GUI.Box(new Rect(menuPos.x +100 , menuPos.y + 240, 95, 55), "Y Pos :" +
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
                    if (GUI.Button(new Rect(menuPos.x, menuPos.y + 430, 290, 30), new GUIContent("Close Layout Menu")))
                    {
                        layoutMode = false;
                    }


                }
                else if (profileMode)
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
                        CameraProfiles.DeleteProfile(CameraProfiles.currentlySelected);
                    if (GUI.Button(new Rect(menuPos.x, menuPos.y + 195, 290, 30), new GUIContent("Load Selected")))
                    {
                        var cs = Resources.FindObjectsOfTypeAll<CameraPlusBehaviour>();
                        foreach (var c in cs)
                            CameraUtilities.RemoveCamera(c);
                        foreach (var csi in Plugin.Instance.Cameras.Values)
                            Destroy(csi.Instance.gameObject);
                        Plugin.Instance.Cameras.Clear();
                        CameraProfiles.SetProfile(CameraProfiles.currentlySelected);
                        CameraUtilities.ReloadCameras();
                    }
                    if (GUI.Button(new Rect(menuPos.x, menuPos.y + 245, 290, 30), new GUIContent(Plugin.Instance._rootConfig.ProfileSceneChange ? "To SceneChange Off" : "To SceneChange On")))
                    {
                        Plugin.Instance._rootConfig.ProfileSceneChange = !Plugin.Instance._rootConfig.ProfileSceneChange;
                        Plugin.Instance._rootConfig.Save();
                    }
                    if (Plugin.Instance._rootConfig.ProfileSceneChange)
                    {
                        GUI.Box(new Rect(menuPos.x, menuPos.y + 285, 290, 30), "Menu Scene Profile : " + (Plugin.Instance._rootConfig.MenuProfile));
                        GUI.Box(new Rect(menuPos.x, menuPos.y + 315, 290, 30), "Game Scene Profile : " + (Plugin.Instance._rootConfig.GameProfile));
                        if (GUI.Button(new Rect(menuPos.x, menuPos.y + 345, 140, 30), new GUIContent("Set Menu Selected")))
                            Plugin.Instance._rootConfig.MenuProfile = CameraProfiles.currentlySelected;
                        if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 345, 140, 30), new GUIContent("Set Game Selected")))
                            Plugin.Instance._rootConfig.GameProfile = CameraProfiles.currentlySelected;
                    }


                    if (GUI.Button(new Rect(menuPos.x, menuPos.y + 430, 290, 30), new GUIContent("Close Profile Menu")))
                        profileMode = false;
                }

                GUI.matrix = originalMatrix;
            }
        }
    }
}
