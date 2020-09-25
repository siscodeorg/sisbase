# Installing sisbase

>[!WARNING]  
>This tutorial **requires** the [`D#+ nuget source`](prerequisites.md) to be already configured.
>Only proceed if you are certain the [previous steps](prerequisites.md) were concluded.

## Stable Releases
> [!WARNING]  
> The API documentation provided under `siscodeorg.github.io/sisbase` does **not** apply to the **stable releases**. It applies to the **latest development release.**

Stable releases are distributed through the `nuget.org` repository. Therefore, no extra setup is required. Simply install the <a href="https://www.nuget.org/packages/sisbase/" target="_blank">**`sisbase package`**</a>.

## Development Releases
Development releases are demarcated with the `-preview` tag after the version number.
They are available at the `github` nuget repository and are built from the `stable` branch of the repository.
These versions are stable, but may contain breaking changes from the prior development release or be incompatible with the current official release.
Use this version if you want to test the latest features as soon as they are considered stable and rolled out to the public.  
> [!WARNING]  
>Since the distribution method for development releases currently is `Github Packages`, there are minor inconveniences in order to get automatic updates to work: you will need to login with an personal access token due to a change on github itself.
>You can install the `.nupkg` manually, but will have to manually update to new stable versions when they are released, as well. Please note that we only offer support for the **latest** development release.

### 1. Create a new Personal Access Token
- <a href="https://github.com/settings/tokens/new" target="_blank"><kbd>Click here</kbd></a> in order to enter the personal access token creation screen.
- Create a new token with the scope of `read:packages`.
- Save that token to login with later.

### 2. Add the siscode nuget repository
#### Nuget Repo URL : `https://nuget.pkg.github.com/siscodeorg/index.json`

- Follow the instructions under [`Installing D#+`](prerequisites.md) but use the following url instead:
`https://nuget.pkg.github.com/siscodeorg/index.json`

- When asked for authentication, log in using your github username as the username and your newly created personal access token as the password.

### Installing the package should now work correctly, including future updates.

