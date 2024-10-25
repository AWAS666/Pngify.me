# Pngify.Me
The goal of this to build a funtional software for any pngtuber, 
meaning a vtuber who just has a png/gif as their avatar.

Feel free to suggest features and submit code yourself

## Todo Basics
- [x] Basic functionality, showing two different Pngs depending if speaking or not
- [x] Save config
- [x] Blinking images in both states
	- [x] smooth transitions (show old state at 50% opacity)
- [x] State transition images (mouth opening/mouth closing) ?
- [x] Switch between different png setups (states) using hotkeys
	- [x] Hotkeys currently override the input for all applications
	- [x] https://github.com/TolikPylypchuk/SharpHook
- [x] Multiple profiles
	- [ ] save all needed files into the profile folder instead of keeping a global reference
	- [ ] show icon?
	- [ ] import/export as zipped archive, custom extension
	- [x] delete profiles
- [x] Effect editor, allow layering of different effects and custom setup
- [ ] Windows, Linux and MacOs build
	- [x] use dx11 for global hotkeys on linux (possibly mac too)
	- [x] cross plattform audio lib, naudio wont do
		- [ ] https://opentk.net/
		- [ ] https://www.ambiera.com/irrklang/
		- [ ] libvlcsharp
		- [ ] this might also fix crashes in tits
		- [x] portaudio
	- [x] disable spout on non windows
- [x] stick on any png (redeem)
	- [x] apply same effects as model 
	- [ ] move with model through states (figure out nearest outline in both states and move by difference)
	- [ ] visualize location
- [x] make audio smoothing changable
- [x] layers
	- [x] panic emote
- [x] size to 80-90% of canvas (settings)
- [ ] transition layers
	- [x] entry and exit for each micstate with setable time for both directions
	- [x] order is: current -> current exist -> next entry -> next
	- [ ] blending is working a little weird
- [x] remember window position
- [x] add hints/layer descriptions
	- [x] rework this into a proper menu anyway, with different categories and stuff
- [x] new trigger variant: sub
- [ ] web view -> just display current image on web, need ws connection of some sort prolly

## Todo Advanced
- [ ] Release aside from git
	- [x] Ko-Fi
	- [x] itch.io
	- [ ] Steam
- [x] Spout integration
- [x] Twitch integration
	- [x] show twitch status globally 
- [x] TITS (Twitch Integrated Throwing System) like system, throw pngs at your vtuber
	- [ ] custom assets
	- [x] sound
	- [x] custom triggers
	- [x] twitch redeems
		- [ ] add text and parse amount?
	- [x] throw emotes
		- [ ] make these load from cache always and refresh once twitch is connected?
	- [x] settings to change how loud collission sound plays
	- [x] bits shower
	- [x] spawn throw similar to rain, not all at once but one after another
- [ ] sprite sheet to avoid more advanced stuff, like more mouth movement when talking
- [ ] collab feature like VTS has, invite your friends and see their PNG too
- [ ] stream deck websocket to trigger stuff
- [ ] optimize performance
	- [x] avoid .Clear() when creating a new frame anyway
	- [x] remove .tolist of layer all the time, use one copy
	- [ ] reuse canvases
	- [ ] avoid drawing of unchanged objects
	- [ ] switch out skia view component, triggers gc to much
- [ ] plugio integration
	- [ ] https://github.com/buttplugio/buttplug-csharp
- [ ] virtual webcam integration
	- [ ] https://github.com/tedd/Tedd.VirtualNetworkCam
	- [ ] https://www.codeproject.com/Articles/437617/DirectShow-Virtual-Video-Capture-Source-Filter-in
- [ ] complex layers 
	- [x] explode into a variety of pixels
		- [x] add sound effects	
	- [ ] shaders
	- [ ] https://x.com/MuRo_CG/status/1839337596549505274 <- similarish effects, maybe no drag/drop
	- [x] yeet away
	- [x] play sound layer -> use this instead of adding sounds to specific layers
		- [x] random sound from folder (.wav only ofc)
	- [ ] emotes explosion like streamelements?
- [ ] controller tracking
- [ ] mouse tracking
## Todo TTSPet
Allow integration of Chatgpt/TTS system to use this as a reaction pet software aswell.
- [x] full Twitch integration
- [x] Chatgpt/other llm 
- [ ] TTS like Elevenlabs/Openai/local
	- [x] openai
	- [ ] elevenlabs
	- [x] stream elements
		- [ ] double check
	- [x] tik tok
- [x] max tokens, limit yapping 
- [x] react to chat
- [ ] STT to talk with your TTS pet
- [x] change to use profile parameter, enabled has to switch current profile from human -> ttspet
- [x] list of filtered words to be replaced by a filtered phrase, possibly also dictionary
	- [ ] possibly use openai moderation, but always need an openai api key for that... 
- [ ] fix audio levels -> make this changable with a slider?

## Todo Refactors
- [ ] change to proper DI instead of static
- [ ] use views and view models properly
- [ ] more usercontrols to avoid overnesting
- [x] remove behaviours

## Todo better ui/ux
- [ ] [UI Toolkit for more styled elements](https://github.com/kikipoulet/SukiUI) -> still in beta
- [x] fix number inputs, sorta wacky right now
- [x] test the interface on a smaller scale, 1920x1080 monitor
- [ ] use icons instead of text
	- [ ] icon library or custom made?
	- [x] https://github.com/MarwanFr/LucideAvaloniaUI
	- [x] https://lucide.dev/icons/save
- [x] https://github.com/irihitech/Ursa.Avalonia
- [x] https://github.com/irihitech/Semi.Avalonia/
- [ ] splash screen or load json in bg
- [x] button to close settings
- [x] more tooltip
- [x] add notifications
 
## Ideas
- [ ] The timer (gets modified by twitch subs, follows, bits etc.) -> ursa timebox
- [x] colourpicker in background select -> Semi

## Support
The goal is to have this be usuable for free, downloadable from git, ko-fi and steam (if it sees some support as steam isn't free)