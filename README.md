# NetworkAudioSync
Keep your audio in sync across clients in your multiplayer Unity game!

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
- Most AudioSource features are covered
- Audio lag compensation included
- Asset is server-authoritative
- Can be used with all networking solutions
- Support Discord server
- Memory & GC friendly

# Supported networking solutions
To be honest, all networking solutions are supported! All you have to do is write custom integration component (remember to implement INetworkAudioSyncIntegration interface) & you are done! But here are listed networking solutions that are officially supported:
- Mirror: https://github.com/vis2k/Mirror
- Fish-Networking: https://github.com/FirstGearGames/FishNet

# Asset support
If you have any problem with my asset, feel free to contact me on Discord: https://discord.gg/z9CakMWnT6

# NetworkFootsteps
This little script plays random sound when GameObject moves by some fixed distance. This is a perfect thing to use while implementing footsteps sounds - it uses much less network overhead. You can change at runtime footstep threshold, volume, and set if footsteps should be played or not (for example you don't wanna hear footsteps while driving a car or falling).

# Planned features
- Automatic late-joiners synchronization
- Ambient sound synchronization (even for late-joiners

# Projects that use NetworkAudioSync
_If your project uses NetworkAudioSync & you want to promote it here, contact me on Discord!_