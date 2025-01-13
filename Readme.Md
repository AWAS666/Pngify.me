# Pngify.Me
The goal of this to build a functional software for any pngtuber, 
meaning a vtuber who just has a png/gif as their avatar.

Feel free to suggest features and submit code yourself

[Download on Steam](https://store.steampowered.com/app/3337800/Pngifyme)

## Todo
- [x] stick on any png (redeem)
	- [ ] move with model through states (figure out nearest outline in both states and move by difference)
	- [ ] visualize location
- [ ] git build chain triggered by tags 
- [ ] web view -> just display current image on web, need ws connection of some sort probably
- [ ] stream deck websocket to trigger stuff
- [ ] splash screen or load json in bg

## Roadmap (future stuff)
- [ ] sprite sheet to avoid more advanced stuff, like more mouth movement when talking
- [ ] collab feature like VTS has, invite your friends and see their PNG too
- [ ] virtual webcam integration
	- [ ] https://github.com/tedd/Tedd.VirtualNetworkCam
	- [ ] https://www.codeproject.com/Articles/437617/DirectShow-Virtual-Video-Capture-Source-Filter-in
- [ ] controller tracking
- [ ] mouse tracking
- [ ] LCOLONQ mode

## Todo Refactors
- [ ] change to proper DI instead of static
- [ ] use views and view models better
- [ ] more usercontrols to avoid overnesting

## Support
The goal is to have this be usuable for free, downloadable from git, itch.io and steam.
Also it should integrate a ton of features whilst remaining lightweight.