# Status Effect Manager

Can be used to easily add new status effects to Valheim.

## How to add status effects

Copy the asset bundle into your project and make sure to set it as an EmbeddedResource in the properties of the asset bundle.
Default path for the asset bundle is an `assets` directory, but you can override this.
This way, you don't have to distribute your assets with your mod. They will be embedded into your mods DLL.

### Merging the DLLs into your mod

Download the StatusEffectManager.dll from the release section to the right.
Including the DLLs is best done via ILRepack (https://github.com/ravibpatel/ILRepack.Lib.MSBuild.Task). You can load this package (ILRepack.Lib.MSBuild.Task) from NuGet.

If you have installed ILRepack via NuGet, simply create a file named `ILRepack.targets` in your project and copy the following content into the file

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
    <Target Name="ILRepacker" AfterTargets="Build">
        <ItemGroup>
            <InputAssemblies Include="$(TargetPath)" />
            <InputAssemblies Include="$(OutputPath)\StatusEffectManager.dll" />
        </ItemGroup>
        <ILRepack Parallel="true" DebugInfo="true" Internalize="true" InputAssemblies="@(InputAssemblies)" OutputFile="$(TargetPath)" TargetKind="SameAsPrimaryAssembly" LibraryPath="$(OutputPath)" />
    </Target>
</Project>
```

Make sure to set the StatusEffectManager.dll in your project to "Copy to output directory" in the properties of the DLLs and to add a reference to it.
After that, simply add `using StatusEffectManager;` to your mod and use the `CustomSE` class, to add your effects.

## Example project

This adds a status effect from a bundle ,with and without a custom folder, and creates one in code. The `se_toxicity` asset bundle is in a directory called `StatusEffects`, while the `se_drunk` asset bundle is in a directory called `assets`. The folder `assets` is the default fallback if a custom folder is not defined.

```csharp
using BepInEx;
using StatusEffectManager;

namespace MyEffects
{
	[BepInPlugin(ModGUID, ModName, ModVersion)]
	public class MyEffectsPlugin : BaseUnityPlugin
	{
		private const string ModName = "MyEffects";
		private const string ModVersion = "1.0.0";
		private const string ModGUID = "org.bepinex.plugins.myeffects";
		
		public void Awake()
		{
			CustomSE mycooleffect = new CustomSE("se_toxicity", "se_toxicity_effect", "StatusEffects");
			mycooleffect.Name.English("Toxicity"); // You can use this to fix the display name in code
			mycooleffect.Type = EffectType.Equip;
			mycooleffect.Icon = "MyCoolIcon.png"; // Use this to add an icon (64x64) for the status effect. Put your icon in an "icons" folder
			mycooleffect.Name.German("Toxizität"); // Or add translations for other languages
			mycooleffect.Effect.m_startMessageType = MessageHud.MessageType.Center; // Specify where the start effect message shows
			mycooleffect.Effect.m_startMessage = "My Cool Status Effect Started"; // What the start message says
			mycooleffect.Effect.m_stopMessageType = MessageHud.MessageType.Center; // Specify where the stop effect message shows
			mycooleffect.Effect.m_stopMessage = "Not cool anymore, ending effect."; // What the stop message says
			mycooleffect.Effect.m_tooltip = "<color=orange>Toxic damage over time</color>"; // Tooltip that will describe the effect applied to the player
			mycooleffect.AddSEToPrefab(test, "SwordIron"); // Adds the status effect to Iron swords. Applies when equipped.
			
			CustomSE drunkeffect = new CustomSE("se_drunk", "se_drunk_effect");
			drunkeffect.Name.English("Drunk"); // You can use this to fix the display name in code
			drunkeffect.Icon = "DrunkIcon.png"; // Use this to add an icon (64x64) for the status effect. Put your icon in an "icons" folder
			drunkeffect.Name.German("Betrunken"); // Or add translations for other languages
			drunkeffect.Effect.m_startMessageType = MessageHud.MessageType.Center; // Specify where the start effect message shows
			drunkeffect.Effect.m_startMessage = "I'm drunk!"; // What the start message says
			drunkeffect.Effect.m_stopMessageType = MessageHud.MessageType.Center; // Specify where the stop effect message shows
			drunkeffect.Effect.m_stopMessage = "Sober...again."; // What the stop message says
			drunkeffect.Effect.m_tooltip = "<color=red>Your vision is blurry</color>"; // Tooltip that will describe the effect applied to the player
			drunkeffect.AddSEToPrefab(test, "TankardAnniversary"); // Adds the status effect to the Anniversary Tankard. Applies when equipped.
			
			// Create a new status effect in code and apply it to a prefab.
			CustomSE codeSE = new CustomSE("CodeStatusEffect");
			codeSE.Name.English("New Effect");
			codeSE.Type = EffectType.Consume;
			codeSE.Icon = "ModDevPower.png";
			codeSE.Name.German("Betrunken"); // Or add translations for other languages
			codeSE.Effect.m_startMessageType = MessageHud.MessageType.Center; // Specify where the start effect message shows
			codeSE.Effect.m_startMessage = "Mod Dev power, granted."; // What the start message says
			codeSE.Effect.m_stopMessageType = MessageHud.MessageType.Center; // Specify where the stop effect message shows
			codeSE.Effect.m_stopMessage = "Mod Dev power, removed."; // What the stop message says
			codeSE.Effect.m_tooltip = "<color=green>You now have Mod Dev POWER!</color>"; // Tooltip that will describe the effect applied to the player
			codeSE.AddSEToPrefab(test, "SwordCheat"); // Adds the status effect to the Cheat Sword. Applies when equipped.
		}
	}
}
```