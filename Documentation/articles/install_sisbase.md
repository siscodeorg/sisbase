# Installing sisbase

>[!WARNING]  
>This tutorial **requires** the [`D#+ nuget source`](prerequisites.md) to be alredy configured.  
>Only proceed if you are certain the [previous steps](prerequisites.md) were concluded.

## Stable Releases
> [!WARNING]  
> The API documentation provided under `siscodeorg.github.io/sisbase` does **not** apply to the **stable releases**. It applies to the **latest development release.**

Stable releases are distributed through the `nuget.org` repository. Therefore, no extra setup is required. Simply install the <a href="https://www.nuget.org/packages/sisbase/" target="_blank">**`sisbase package`**</a>.

## Development Releases
Develoment releases are demarked with the `-preview` tag after the version number.
They are available at the `github` nuget repository and built from the `stable` branch of the repository.  
Said versions are stable but may contain breaking changes with the prior develoment release or incompatibility with the stable release if under a different **MAJOR** version.  
Use this version if you want to test the latest features as soon as they are considered stable and rolled out to the public.  
> [!WARNING]  
>Since the distribution method for them currently is `Github Packages` there are minor inconviniences in order to get automatic updates to work. Namely the need of login with an personal access token due to a change on github itself.  
>You can install the `.nupkg` maually although thats not suggested since you will end up with an unsupported old development release. Please keep in mind that we only offer support for the **latest** development release.

### 1. Create a new PAT
- <a href="https://github.com/settings/tokens/new" target="_blank"><kbd>Click here</kbd></a> in order to enter the personal acess token creation screen.  
- Create a new token with the scope of `read:packages`.
- Save that token because you'll need to login with that later.

### 2. Add the siscode nuget repository
#### Nuget Repo URL : `https://nuget.pkg.github.com/siscodeorg/index.json`

- Follow the instructions under [`Installing D#+`](prerequisites.md) but use the following url instead:
`https://nuget.pkg.github.com/siscodeorg/index.json`

- It will ask for authentication, log in using your github username as the username and your newly created personal acess token as the password.

### Installing the package should now work just fine, including future updates.

