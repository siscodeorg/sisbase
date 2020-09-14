# Installing D#+
In order to install sisbase you need to use a **nightly** version of
dsharpplus which is currently available through a separate nuget
repository.

If you already have the right nuget source skip to [Installing sisbase](install_sisbase.md)

>[!Warning]
>Accidentally installing the wrong version of D#+ will cancel
>the attempt of adding sisbase to your project due to a version conflict.
>[!Tip]
>**Don't** install D#+ manually / remove installed version to avoid this
>issue completely.

## How to add the nuget source
>[!Warning]
>The nuget source for D#+ nightly builds in some of the D#+ documentation is
>**OUTDATED**.  
>It will not work unless you use the following source:
>`https://nuget.emzi0767.com/api/v3/index.json`

Nuget Source : https://nuget.emzi0767.com/api/v3/index.json

### Nuget CLI
Run `nuget sources add -name D#+ -source  https://nuget.emzi0767.com/api/v3/index.json`

### Dotnet CLI
Run `dotnet nuget add source https://nuget.emzi0767.com/api/v3/index.json --name D#+`

### Visual Studio
- Enter the Nuget Manager  
<kbd> Project > Manage Nuget Packages...</kbd>  or <kbd>Alt</kbd> + <kbd>P</kbd> + <kbd>N</kbd> + <kbd>N</kbd> + <kbd>Enter</kbd>

- Click on the settings ⚙️

![](https://i.imgur.com/xxdfd5J.png)
- Click on <kbd>Add package source</kbd> 

![](https://i.imgur.com/SYCvDgU.png)

- Under <kbd>name</kbd> and <kbd>source</kbd> add the following values

Name : `DSharpPlus`  
Source : `https://nuget.emzi0767.com/api/v3/index.json`  

- Click on <kbd>Ok</kbd>
