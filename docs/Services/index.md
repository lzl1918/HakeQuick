# HakeQuick-Services

## `ICurrentEnvironment`
Records program environments.
- MainDirectory
- PluginDirectory
- ConfigDirectory

## `IQuickContext`
- Command
- CancellationProvider
- AddAsyncAction
- AddAction

## `IProgramContext`
Records informations of foreground application just before hotkey pressed.
- WindowPosition
- CurrentProcess
- WindowHandler
- DesktopHandler

## `ITray`
- SendNotification

## `ITerminationNotifier`
To send termination message to main program.
- NotifyTerminate