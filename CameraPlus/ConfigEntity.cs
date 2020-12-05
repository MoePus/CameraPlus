namespace CameraPlus
{
    class ConfigEntity
    {
		public string version = "1.6.0";
		public int windowResolutionWidth = 1280;
		public int windowResolutionHeight = 720;
		public int windowMode = (int)MainSettingsModelSO.WindowMode.Fullscreen;
		public float vrResolutionScale = 1.0f;
		public float menuVRResolutionScaleMultiplier = 1.0f;
		public bool useFixedFoveatedRenderingDuringGameplay = false;
		public int antiAliasingLevel = 2;
		public int mirrorGraphicsSettings = 2;
		public int mainEffectGraphicsSettings = 1;
		public int bloomGraphicsSettings = 0;
		public int smokeGraphicsSettings = 1;
		public bool burnMarkTrailsEnabled = true;
		public bool screenDisplacementEffectsEnabled = true;
		public float roomCenterX = 0.0f;
		public float roomCenterY = 0.0f;
		public float roomCenterZ = 0.0f;
		public float roomRotation = 0.0f;
		public float controllerPositionX = 0.0f;
		public float controllerPositionY = 0.0f;
		public float controllerPositionZ = 0.0f;
		public float controllerRotationX = 0.0f;
		public float controllerRotationY = 0.0f;
		public float controllerRotationZ = 0.0f;
		public int smoothCameraEnabled = 0;
		public float smoothCameraFieldOfView = 70f;
		public float smoothCameraThirdPersonPositionX = 0.0f;
		public float smoothCameraThirdPersonPositionY = 1.5f;
		public float smoothCameraThirdPersonPositionZ = -1.5f;
		public float smoothCameraThirdPersonEulerAnglesX = 0.0f;
		public float smoothCameraThirdPersonEulerAnglesY = 0.0f;
		public float smoothCameraThirdPersonEulerAnglesZ = 0.0f;
		public int smoothCameraThirdPersonEnabled = 0;
		public float smoothCameraRotationSmooth = 4f;
		public float smoothCameraPositionSmooth = 4f;
		public float volume = 1f;
		public bool controllersRumbleEnabled = true;
		public int enableAlphaFeatures = 0;
		public int pauseButtonPressDurationLevel = 0;
		public int maxShockwaveParticles = 1;
		public bool overrideAudioLatency = false;
		public float audioLatency = 0.0f;
		public int maxNumberOfCutSoundEffects = 24;
		public bool onlineServicesEnabled = false;
		public bool oculusMRCEnabled = false;
		public bool openVrThreadedHaptics = false;
		public int languageSettings = 0;
	}
}
