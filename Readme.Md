# Pngify.Me
The goal of this to build a functional software for any pngtuber, 
meaning a vtuber who just has a png/gif as their avatar.

Feel free to suggest features and submit code yourself

[Download on Steam](https://store.steampowered.com/app/3337800/Pngifyme)

![Logo](./PngifyMe/Assets/logo_full.png?raw=true)

## Todo
- [x] stick on any png (redeem)
	- [ ] move with model through states (figure out nearest outline in both states and move by difference)
	- [ ] visualize location
- [x] git build chain triggered by tags 
- [ ] web view -> just display current image on web, need ws connection of some sort probably
- [x] stream deck websocket to trigger stuff
- [ ] splash screen or load json in bg
- [ ] change audio engine to this: https://github.com/LSXPrime/SoundFlow
	- this also allows fancy effects like audio spectrums
	- likely just more compatible
- [ ] layers
	- [ ] cpu/gpu temp layer (might need admin access for this though... https://github.com/LibreHardwareMonitor/LibreHardwareMonitor) 

## Roadmap (future stuff)
- [x] sprite sheet to avoid more advanced stuff, like more mouth movement when talking
- [ ] collab feature like VTS has, invite your friends and see their PNG too
- [ ] virtual webcam integration
	- [ ] https://github.com/tedd/Tedd.VirtualNetworkCam
	- [ ] https://www.codeproject.com/Articles/437617/DirectShow-Virtual-Video-Capture-Source-Filter-in
- [ ] controller tracking
- [x] mouse tracking
- [x] drawing tablets
- [x] LCOLONQ mode

## Support
The goal is to have this be usuable for free, downloadable from git, itch.io and steam.
Also it should integrate a ton of features whilst remaining lightweight.
