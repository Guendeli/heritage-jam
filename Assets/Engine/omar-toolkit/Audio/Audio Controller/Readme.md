### Oryx Audio Controller

The approach of this Audio controller is to mimic the functionality of Audio Middleware like WWise or FMod while still being inside the unity editor.
Its goal is to pretty much separate concerns and avoid to everyone the use of manually editing AudioSources.

a sound designer should not get mad at looking at all the scene hierarchy, prefabs looking at which audiosource to edit.
a game programmer should not care about what to do with an audio source, they should only care about "playing a sound event with an id of X when Y happens"

Audio Controller offers basically that 
