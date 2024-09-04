# PngifyMe
The goal of this to build a funtional software for any pngtuber, 
meaning a vtuber who just has a png/gif as their avatar.

Feel free to suggest features and submit code yourself

## Todo Basics
- [x] Basic functionality, showing two different Pngs depending if speaking or not
- [x] Save config
- [x] Blinking images in both states
	- [ ] smooth transitions (show old state at 50% opacity)
- [x] State transition images (mouth opening/mouth closing) ?
- [x] Switch between different png setups (states) using hotkeys
- [x] Multiple profiles
	- [ ] save all needed files into the profile folder instead of keeping a global reference
	- [ ] show icon?
	- [ ] import/export as zipped archive
- [x] Effect editor, allow layering of different effects and custom setup
- [ ] Windows, Linux and MacOs build
	- [ ] use dx11 for global hotkeys on linux (possibly mac too)
- [x] stick on any png (redeem)
	- [ ] move with model (figure out nearest outline in both states and move by difference)
	- [ ] visualize location

## Todo Advanced
- [ ] Release aside from git
	- [ ] Ko-Fi
	- [ ] Steam
- [x] Spout integration
- [x] Twitch integration
- [x] TITS (Twitch Integrated Throwing System) like system, throw pngs at your vtuber
	- [ ] custom assets
	- [x] sound
	- [ ] custom triggers
	- [ ] throw emotes
- [ ] sprite sheet to avoid more advanced stuff, like more mouth movement when talking
- [ ] collab feature like VTS has, invite your friends and see their PNG too

## Todo TTSPet
Allow integration of Chatgpt/TTS system to use this as a reaction pet software aswell.
- [x] full Twitch integration
- [ ] Chatgpt/other llm 
- [ ] TTS like Elevenlabs/Openai/local
- [ ] STT to talk with your TTS pet

## Todo Refactors
- [ ] change to proper DI instead of static
- [ ] use views and view models properly
- [ ] more usercontrols to avoid overnesting

## Todo better ui/ux
- [ ] [UI Toolkit for more styled elements](https://github.com/kikipoulet/SukiUI)
- [ ] fix number inputs, sorta wacky right now

## Support
The goal is to have this be usuable for free, downloadable from git, ko-fi and steam (if it sees some support as steam isn't free)
