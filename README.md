# NetworkAudioSync
Keep your audio in sync across clients in your multiplayer game!

# How to get started?
- Download NetworkAudioSync library with chosen networking integration (more about them later)
- Set up your AudioSources on networked objects
- Create new NetworkAudioClips ScriptableObject and give it unique name (this is VERY important)
- Add NetworkAudioSource component to your GameObjects that contain AudioSources
- Assign your AudioSource and NetworkAudioClips instance to NetworkAudioSource
- Assign networking solution integration component to the GameObject where NetworkAudioClips component is
- If you have trouble with that, usually those components are named in following format: NetworkAudioSync(networking solution name). If you use Assembly definitions, then remember to reference NetworkAudioSync & your integration's assembly. If you still have problems, join our Discord for support (link below)
- From now on, don't touch AudioSource component in your code - use NetworkAudioSource.

# Main features
- Asset covers most AudioSource features
- NetworkAudioSource state changes are server-authoritative
- Can be used with many networking solutions
- Support Discord server, usually I respond in less than day
- Audio synchronization is memory-friendly

# Supported networking solutions
To be honest, all networking solutions are supported! All you have to do is write custom integration component (remember to implement INetworkAudioSyncIntegration interface) & you are done! But here are listed networking solutions that are officially supported:
- Mirror: https://github.com/vis2k/Mirror
- Fish-Networking: https://github.com/FirstGearGames/FishNet

# Asset support
If you have any problem with my asset, feel free to contact me on Discord: https://discord.gg/z9CakMWnT6

# NetworkFootsteps
This little script I made is to... efficiently synchronize footsteps sound. Here, sound accuracy does not matter, but performance. To configure this thing, you have to set footstep threshold, and footstep sounds. If you want to disable footsteps (for example when your character is in vehicle or is falling), then just disable NetworkFootsteps component and re-enable it when you will need it.

# Planned features
- Automatic late-joiners synchronization