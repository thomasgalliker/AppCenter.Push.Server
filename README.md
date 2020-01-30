# AppCenter.Push.Server
[![Version](https://img.shields.io/nuget/v/AppCenter.Push.Server.svg)](https://www.nuget.org/packages/AppCenter.Push.Server)  [![Downloads](https://img.shields.io/nuget/dt/AppCenter.Push.Server.svg)](https://www.nuget.org/packages/AppCenter.Push.Server)

<img src="https://raw.githubusercontent.com/thomasgalliker/AppCenter.Push.Server/develop/logo.png" height="100" alt="AppCenter.Push.Server" align="right">
AppCenter.Push.Server provides tools which are commonly used for handling .Net enums.

### Download and Install AppCenter.Push.Server
This library is available on NuGet: https://www.nuget.org/packages/AppCenter.Push.Server/
Use the following command to install AppCenter.Push.Server using NuGet package manager console:

    PM> Install-Package AppCenter.Push.Server

You can use this library in any .Net project which is compatible to .Net Framework 4.0+ and .Net Standard 1.0+.

### API Usage
The unit tests shipped along with this library show nicely all the helper methods you can use.
To give an impression, here a selection of provided methods:

#### Check if type is an enum
```C#
bool isEnum = EnumHelper.IsEnum<Weekday>();
```

#### Enumerating all values of an enum
```C#
IEnumerable<Weekday> weekdays = EnumHelper.GetValues<Weekday>();
```
#### Get string name from enum
```C#
string weekday = EnumHelper.GetName(Weekday.Tue);
```

#### Parse string to enum value
```C#
Weekday weekday = EnumHelper.Parse<Weekday>("Thu");
```

#### TryParse string to enum value (single line statement)
```C#
Weekday weekday = EnumHelper.TryParse<Weekday>("Thu");
```

#### Safely cast integer to enum
```C#
Weekday weekday = EnumHelper.Cast(value: 1, defaultValue: Weekday.Mon);
```
### Contribution
Contributors welcome! If you find a bug or you want to propose a new feature, feel free to do so by opening a new issue on github.com.

### License
This project is Copyright &copy; 2018 [Thomas Galliker](https://ch.linkedin.com/in/thomasgalliker). Free for non-commercial use. For commercial use please contact the author.
