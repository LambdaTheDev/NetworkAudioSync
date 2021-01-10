# NetworkAudioSync
I have made simple audio sync "addon" for Mirror Networking. Just apply NetworkAudioSync to your AudioSource, add additional AudioClips and enjoy!

# How to use
- Fist, place NetworkAudioSync somewhere in your project;
- Then, apply NetworkAudioSync to the object with the AudioSource;
- Add all additional AudioClips if you want;
- Finally, to sync the audio, get the NetworkAudioSync instance and execute SyncAudio() function (you can pass AudioClip argument, but AudioClip you are trying to send has to be registered).
- Enjoy
