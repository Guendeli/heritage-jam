# EZState
An Alternative approach to Animator and single frame animation files.

### The Problem

![](./EZStateProblem.jpg)

A Lot of UI/Art designers make uses of single frame animation files to achieve stuff, like disabling/enabling a/some gameobject(s), changing a sprite or a material etc...
On Mobile games mostly that are UI heavy, this is a bad practice since Animator components have an update of their own, so they run even if nothing is happening.
also after multiple tests, running an action through code is much cheaper than running it through an Animation Clip.

Goal of this component is to get rid of Animator and single frame animation clips altogether by providing States and Actions built after years of working with artists and UI designers, compacting all their needs in one single component.
a lite version of Animator, relegating Mecanim/Animator only if you are using skinned mesh renderers.
