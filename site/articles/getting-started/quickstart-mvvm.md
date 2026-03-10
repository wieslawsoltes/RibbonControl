---
title: "Quickstart: MVVM"
---

# Quickstart: MVVM

Use the MVVM path when your ribbon needs to react to application state or dynamic feature flags.

```xml
<ribbon:Ribbon TabsSource="{CompiledBinding Ribbon.Tabs}"
               CommandCatalog="{CompiledBinding CommandCatalog}"
               StateStore="{CompiledBinding StateStore}"
               SelectedTabId="{CompiledBinding Ribbon.SelectedTabId, Mode=TwoWay}"
               IsMinimized="{CompiledBinding Ribbon.IsMinimized, Mode=TwoWay}"
               IsKeyTipMode="{CompiledBinding Ribbon.IsKeyTipMode, Mode=TwoWay}"
               QuickAccessItems="{CompiledBinding QuickAccessItems}"
               QuickAccessPlacement="{CompiledBinding QuickAccessPlacement, Mode=TwoWay}" />
```

Typical runtime setup:

```csharp
var commandCatalog = new DictionaryRibbonCommandCatalog();
var stateStore = new JsonRibbonStateStore("ribbon-state.json");

var ribbon = new RibbonViewModel();
var homeTab = new RibbonTabViewModel { Id = "home", Header = "Home" };
var clipboardGroup = new RibbonGroupViewModel { Id = "clipboard", Header = "Clipboard" };

clipboardGroup.ItemsViewModel.Add(new RibbonItemViewModel
{
    Id = "paste",
    Label = "Paste",
    CommandId = "paste",
});

homeTab.GroupsViewModel.Add(clipboardGroup);
ribbon.Tabs.Add(homeTab);
```

This mode keeps the view passive while letting you unit-test the ribbon composition logic independently of Avalonia visuals.
