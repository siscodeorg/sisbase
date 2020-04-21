# Hello World!
Lets walktrough on the creation of a simple discord bot that only connects to discord.

## Requirements
* Already working sisbase installation
* A file called `Program.cs`, this for all the next articles will be called the "Main File".

`Note : The main file is usually automatically generated once you create a new C# project. If you already have one **don't** delete it.`
### First part : Importing sisbase.
To create an sisbase bot you need to have the sisbase library available.

Add the following line to the top of your main file.
```csharp
using sisbase;
```

### Second part : Creating a bot
Now its time to make a brand new sisbase bot. Don't worry it will be easy.

Lets start by converting our main loop to be `async`. Just be sure that your code matches this example and
you'll be all set for the next step.
```csharp
class Program
{
	private static async Task Main()
	{

	}
}
```

After that is done we need to create a new instance of the bot.
```csharp
var sisbase = new SisbaseBot();
```

Now onto the last part. Registering the current project on the instance.

**This is important since without it, any new commands and systems won't be loaded**

```csharp
sisbase.RegisterBot(typeof(Program).Assembly);
```

### Last Part : Running the bot.
To run the bot just use the `Start()` function as its shown below.
```csharp
await sisbase.Start();
```

## Results
If you have anything similar like this the bot will now run and ask for a token.
To learn more about tokens [Click Here]("")

```csharp
using sisbase;
class Program
{
	private static async Task Main()
	{
		var sisbase = new SisbaseBot();
		sisbase.RegisterBot(typeof(Program).Assembly);
		await sisbase.Start();
	}
}
```