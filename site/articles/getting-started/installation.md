---
title: "Installation"
---

# Installation

Install the packages you need:

```bash
dotnet add package RibbonControl.Core
dotnet add package RibbonControl.Persistence.Json
```

Add the theme resources in your Avalonia application:

```xml
<Application xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="MyApp.App">
  <Application.Styles>
    <FluentTheme />
    <StyleInclude Source="avares://RibbonControl.Core/Themes/Generic.axaml" />
  </Application.Styles>
</Application>
```

Use the ribbon namespace in views:

```xml
xmlns:ribbon="https://github.com/wieslawsoltes/ribboncontrol"
```

If your application persists runtime ribbon state, register or construct an `IRibbonStateStore` implementation and bind it to `Ribbon.StateStore`.
