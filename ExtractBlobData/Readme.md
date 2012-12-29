### What's going on here?

To support Steam integration, Sledge needs to look through all the Steam dependencies to find the correct game files. Unfortunately, Steam stores this data in the binary ClientRegistry.blob file. 

Fortunately however, [Darien Hager][1] has created a very handy Java library called [hl2parse][2] (used in [PackBSP][3]), which, among other things, can parse the ClientRegistry file.

Because I don't (currently) see the need to support dynamically reading the ClientRegistry file, the included Java app will use hl2parse to generate the static SteamGames class.

A list of Steam application IDs is maintained [on the VDC][4]. Regenerate the file when games are modified or when new games are added.

[1]: http://technofovea.com/
[2]: https://github.com/DHager/hl2parse
[3]: https://github.com/DHager/packbsp
[4]: http://developer.valvesoftware.com/wiki/Steam_Application_IDs