# Overview
This is a project I made back in the fall of 2026.
It was submitted for the WSU Game Dev Club's Club Projects 4, which can be viewed here: https://itch.io/jam/wsu-club-projects-4.

The goal for this project was to expand on my previous project (Local Bad) and re-work a lot of the codebase.
For this project, I ended up learning a few things:
* Maybe don't just casually rewrite things all the time.
* Prioritize more component based modular systems instead of focusing on large and sometimes complex class hiercharies.
* A few scuffed implementations here and there is better than no implementations.
* Keep it simple for god sakes.
* The Type class in C# (used mostly for the inventory system).
* Do not use MemberWiseClone() on a C# class inheriting from RefCounted or else bad things will happen. From what I can tell, the Godot's reference counter system for RefCounted doesn't work with the function and thus causes some memory issues.
* Godot internally is not a very strongly typed engine/codebase. The Variant structure/class is used extensively within it, which can represent a wide variety of values only defined by the engine. The documentation for it can be found here: https://docs.godotengine.org/en/stable/classes/class_variant.html
* Godot and C# don't mix too well in a few ways...

# How to Run
One should be able to open this project by installing version 1.6.0 of the LimboAI version of Godot: https://github.com/limbonaut/limboai/releases#release-v1.6.0.
This will allow one to play the game alongside easily view the assets and such.
If one simply wants to play the game without the need of installing Godot,
they can install the built/compiled version here: https://vinterwanter.itch.io/local-bad-survival
