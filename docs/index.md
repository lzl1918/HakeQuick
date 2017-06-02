# Hake Quick
Welcome to developement guide.


## Framework
`Host` contains almost all program logic of application framework.

`HostBuilder` is used to config different settings and build instances of `Host`

`Host` uses pipeline to handle user input and retrive actions.

## Components
Application pipeline contains several components, which return actions according to user input.
Components in pipeline can be both methods or classes (actually only methods are supported, classes are supported by anonymous methods).

### Methods
Generally the supported methods have the signature shown below.
```CSharp
Task MethodName(IQuickContext context, Func<Task> next)
```

### Classes
Classes used as components must have method called `Invoke`, and the return type must be `Task`. `Invoke` methods are call using **dependency injection** so configured services can be accessed via parameters.

Instances are created each time when going through the application pipeline.

## Actions
Actions can be executable, or unexecutable. If one action is executable, user can invoke the action by pressing `enter` key when main window is being shown. `Host` will invoke the method `Invoke` of action instance via **dependency injection**, which means developers can use configured services as parameters of `Invoke`.

## Services
Services are divided into three scopes
- `Singleton` Instances are created the first time they are required, and get disposed when application ends
- `Scoped` Instances are created each time specific hotkey is pressed. When main window hides, instances are disposed
- `Transient` Instances are created each time they are required, and the created instances are disposed next time instances with same type get required.

Internally supported services can be references [here](https://github.com/lzl1918/HakeQuick/tree/master/docs/Services/index.md)

Service pool can be extended by adding custom services.

## Plugins
