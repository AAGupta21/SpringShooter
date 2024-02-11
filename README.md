# SpringShooter

Game made as an Assignment for Ronin Labs.

Game can be played by opening the scene "Main" in "Assets>Scene" folder.
It has a how to play section (denoted as ?) which can be referred to understand the gameplay mechanics.

All files are named and placed in seperate folders. For instance, all scripts can be found in "Assets>Script" folder or all materials are located in "Assets>Art>Material" folder.

To navigate the code, simply find the main script called 'Manager' in Assets>Script folder, there are also 'helper' class variables that have been delegated specific tasks and they take care of that, for instance "ScoreKeeper" takes track of points scored and storing/retrieving the highest score from prefs.
Simply start at Manager class and you can navigate to all the other classes from there or find all helper classes in "Assets>Script>Helper" folder.

No Main Menu UI or Music was added (since it was not a requirement given in assignment)

Cartoon Fx particle system (free ver) was used to add particle effects in gameplay.
https://assetstore.unity.com/packages/vfx/particles/cartoon-fx-remaster-free-109565

PS

I was limited to one monobehavior, which made wrapping all target tiles into an interface and dealing with them in a singular manner not possible since they will need to have monobehavior themselves and this will break the rule. So instead, I went with another route and detected them using the tags assigned to them and let a handler deal with them.
