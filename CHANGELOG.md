# The Main Changelog File

## sisbase `Himenokōji Akiko` 1.0.0 

+ Feature parity with [`old lolibase`](https://github.com/lolidevs/lolibase) project

+ Added SMC (System Management Controller) for disabling and enabling of systems

+ Project works as a standalone library

+ + [sisbase.1.0.0.nupkg](/uploads/190416627da1376cd9629196248f042b/sisbase.1.0.0.nupkg)

### `Patch 1.0.1`

+ Adds documentation to nupkg.

+ + [sisbase.1.0.1.nupkg](/uploads/7759e3270ecd828989df41463456a385/sisbase.1.0.1.nupkg)

#### What broke : 

- `Nothing` This version is fully backwards compatible 
## sisbase `Kasugano Sora` 1.1.0 `Now Available on nuget!`

+ Added mutation to DiscordEmbeds via the `Mutate` extension method.
+ Added `PrefixAttribute` for commands to only be executed with a specific prefix
+ Added `Behaviour System`
+ Help command now `hides` commands with the `HiddenAttribute` **(Unless -h is added to the end of the command)**
+ Fixed help command showing commands to unauthorized users

### What broke :
- `Nothing` This version is fully backwards compatible with 1.0.1

## sisbase `Hashima Chihiro` 1.2.0

+ Added `Custom Settings` to `Sisbase`

Ever wanted to add your own settings to the config.json file? Now you **can!**

Using the new Extesion Methods from `sisbase.Utils` you too can add anything onto the configuration file and request at will.

+ Added `RequireSystemAttribute`

IF you need that a system be active for a command to execute, you now can just add this simple attribute to it (providing the  `Type` of said system of course) and it
will only be executed **if** said system exists.

+ Simplified way to register a bot.

Now you need to register a bot with `#SisbaseBot.RegisterBot()` isntead of manually registering the commands and systems.

### What broke :

- If you used the token value anywhere in your bot (which of course you shouldn't do anyways), you now can't since that property is `internal`.
- Registering all systems from an assembly is now inaccessible since its `internal` , just replace it with the new unified way.
