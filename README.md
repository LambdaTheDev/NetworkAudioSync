# NetworkAudioSync
NetworkAudioSync is a small addon to Mirror Networking that lets you synchronize your audio effects across clients.

# How to use it?
If you want basic audio synchronization, then add NetworkAudioSync component to your networked object. With it, one more component will appear - NetworkAudioClips. This is responsible for getting IDs of each AudioClip used in the NetworkAudioSync.

After you are done with setup, reference NetworkAudioSync in your code, and execute SyncAudio(AudioClip) method (remember - audio clip that you place in SyncAudio method has to be present in the NetworkAudioClips!)

# What about other modules?
I have thought about some specific situations when audio sync is needed. Here are listed all modules and explanation how to use them:

**NetworkFootsteps:**
- I have made really simple way to sync player footsteps. In this case, it's not important to send the same audio clip on every step.
- To use NetworkFootsteps, just add it to your object, place all your footstep AudioClips in the *footstepSounds* list, set sync threshold (if threshold is 1f, then player needs to walk by 1 unit to trigger footstep sync), and that's it!
- You may also want to not hear footsteps in specific situations, for example if your player is crouching. Then, __on server__, set the SyncFootsteps property to false, and when player stops crouching - set it back to true.

**PreciseAudioSync**
- This was made more for music. When you are using PreciseAudioSync, it is guaranteed that AudioClip will end in the same time for all players.
- Note: If AudioClip you are syncing is shorter than latency between client-server, then clip will not be played.
- Setup and usage is the same as in the regular NetworkAudioSync.

**NetworkAnnouncer**
- NetworkAnnouncer is something like the intercom.
- NetworkAnnouncer setup is quite different than in previous modules. First 2 options are announcement prefix ans suffix. You can add there AudioClips that will play right before and right after specific announcement. Those aren't mandatory, so you don't have to assign them. Then, you have next 2 variables - *delayBetweenAnnouncements* and *delayAfterPrefix*. First one is amount of seconds that NetworkAnnouncer will wait before starting next announcement. Second float is amount of seconds that NA will wait before finishing playing *announcementPrefix* AudioClip.
- To send an announcement, you only need to call Announce(AudioClip) method. Remember - AudioClip needs to be registered in the NetworkAudioClips!

# Is there a way to have NetworkAudioClips in a different GO?
Not now, but soon I will implement such feature. Only thing I need to do is to add idiot-proof checks to make sure that someone referenced NAC in AudioSync module.

# Projects that use NetworkAudioSync:
*If you use NetworkAudioSync, and want to promote your project, contact me on Discord - LambdaTheDev#6559*


Enjoy & report all issues. If you want to contribute, DM me on Discord, or just make a pull request (remember to explain what you add to NetworkAudioSync)!