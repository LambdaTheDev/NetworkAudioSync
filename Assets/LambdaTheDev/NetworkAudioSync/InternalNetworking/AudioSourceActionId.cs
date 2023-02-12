namespace LambdaTheDev.NetworkAudioSync.InternalNetworking
{
    // Static class with AudioSource Action IDs used in packet sending & reading
    internal static class AudioSourceActionId
    {
        // General NetworkAudioSync IDs
        public const byte BypassEffects = 1;
        public const byte BypassListenerEffects = 2;
        public const byte BypassReverbZones = 3;
        public const byte DopplerLevel = 4;
        public const byte GamepadSpeakerOutputType = 5;
        public const byte IgnoreListenerPause = 6;
        public const byte IgnoreListenerVolume = 7;
        public const byte Loop = 8;
        public const byte MaxDistance = 9;
        public const byte MinDistance = 10;
        public const byte Mute = 11;
        public const byte OutputAudioMixerGroup = 12;
        public const byte PanStereo = 13;
        public const byte Pitch = 14;
        public const byte Priority = 15;
        public const byte ReverbZoneMix = 16;
        public const byte RolloffMode = 17;
        public const byte SpatialBlend = 18;
        public const byte Spatialize = 19;
        public const byte SpatializePostEffects = 20;
        public const byte Spread = 21;
        public const byte Time = 22;
        public const byte TimeSamples = 23;
        public const byte VelocityUpdateMode = 24;
        public const byte Volume = 25;

        public const byte Play = 26;
        public const byte Pause = 27;
        public const byte Stop = 28;
        public const byte SetClip = 29;

        // NetworkFootsteps IDs
        public const byte FootstepsEnabled = 75;
        public const byte FootstepsVolume = 76;
        public const byte FootstepThreshold = 77;
        
        public static class PlayModes
        {
            public const byte Normal = 0;
            public const byte Delayed = 1;
            public const byte OneShot = 2;
            public const byte AtPoint = 3;
        }

        public static void InvalidPacketThrow()
        {
            // throw new InvalidDataException("Invalid packet arrived!");
            // This may be a bad idea... maybe just disregard invalid packets?
        }
    }
}