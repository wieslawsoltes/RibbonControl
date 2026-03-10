# RibbonControl

Avalonia ribbon toolkit for desktop applications, with XAML-first composition, MVVM-friendly state flow, and optional JSON persistence for user customization state.

[![CI](https://github.com/wieslawsoltes/RibbonControl/actions/workflows/ci.yml/badge.svg)](https://github.com/wieslawsoltes/RibbonControl/actions/workflows/ci.yml)
[![Docs](https://img.shields.io/badge/docs-lunet-0b7285.svg)](https://wieslawsoltes.github.io/RibbonControl/)
[![RibbonControl.Core](https://img.shields.io/nuget/v/RibbonControl.Core.svg)](https://www.nuget.org/packages/RibbonControl.Core)
[![RibbonControl.Persistence.Json](https://img.shields.io/nuget/v/RibbonControl.Persistence.Json.svg)](https://www.nuget.org/packages/RibbonControl.Persistence.Json)
[![.NET](https://img.shields.io/badge/.NET-8%20%7C%2010-512BD4)](https://dotnet.microsoft.com/)
[![Avalonia](https://img.shields.io/badge/Avalonia-11.3.11-8B44AC)](https://avaloniaui.net/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/wieslawsoltes/RibbonControl/blob/main/LICENSE)

## NuGet Packages

| Package | NuGet | Description |
|---------|-------|-------------|
| **RibbonControl.Core** | [![NuGet](https://img.shields.io/nuget/v/RibbonControl.Core.svg)](https://www.nuget.org/packages/RibbonControl.Core) | Core Avalonia ribbon controls, themes, models, view models, automation peers, merge policies, and command/state services |
| **RibbonControl.Persistence.Json** | [![NuGet](https://img.shields.io/nuget/v/RibbonControl.Persistence.Json.svg)](https://www.nuget.org/packages/RibbonControl.Persistence.Json) | JSON-backed `IRibbonStateStore` implementation with schema versioning and migration hooks |

## Documentation

Full project documentation is published with Lunet at [wieslawsoltes.github.io/RibbonControl](https://wieslawsoltes.github.io/RibbonControl/).

## Features

- XAML-first, MVVM-first, and hybrid composition models.
- Rich ribbon primitives including tabs, groups, split buttons, combo boxes, toggle groups, galleries, backstage, and quick access toolbar.
- Data-driven composition with `RibbonDefinition`, `RibbonTab`, `RibbonGroup`, `RibbonItem`, and matching definition/view-model types.
- Merge-aware runtime composition through `TabsSource`, `GroupsSource`, `ItemsSource`, and `IRibbonMergePolicy`.
- Built-in key tip routing, adaptive layout, stable ribbon height support, and command-height synchronization.
- Runtime customization export/reset/load flows through `IRibbonCustomizationService` and `IRibbonStateStore`.
- JSON persistence package with schema versioning support and pluggable migrations.
- Automation peers, headless tests, visual regression coverage, and performance tests in the repository.

## Installation

```bash
dotnet add package RibbonControl.Core
dotnet add package RibbonControl.Persistence.Json
```

Add the ribbon theme resources to your Avalonia app:

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

Use the RibbonControl XML namespace in views:

```xml
xmlns:ribbon="https://github.com/wieslawsoltes/ribboncontrol"
```

## Usage

### XAML-First Composition

```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:ribbon="https://github.com/wieslawsoltes/ribboncontrol">
  <DockPanel>
    <ribbon:Ribbon DockPanel.Dock="Top">
      <ribbon:RibbonTab Id="home" Header="Home" Order="0">
        <ribbon:RibbonGroup Id="clipboard" Header="Clipboard" Order="0">
          <ribbon:RibbonItem Id="paste"
                             Label="Paste"
                             Primitive="PasteSplitButton"
                             IconPathData="{x:Static ribbon:FluentIconData.ClipboardPaste20Regular}"
                             ScreenTip="Paste from clipboard" />
          <ribbon:RibbonItem Id="copy"
                             Label="Copy"
                             Primitive="Button"
                             IconPathData="{x:Static ribbon:FluentIconData.Copy20Regular}" />
        </ribbon:RibbonGroup>
      </ribbon:RibbonTab>
    </ribbon:Ribbon>
  </DockPanel>
</Window>
```

### MVVM / Data-Driven Composition

Bind the control to view-model state and command services:

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

Typical service setup:

```csharp
using RibbonControl.Core.Models;
using RibbonControl.Core.Services;
using RibbonControl.Persistence.Json.Storage;
using RibbonControl.Core.ViewModels;

var commandCatalog = new DictionaryRibbonCommandCatalog();
var customizationService = new RibbonCustomizationService();
var stateStore = new JsonRibbonStateStore(
    Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "RibbonControl",
        "ribbon-state.json"));

var ribbon = new RibbonViewModel();
var homeTab = new RibbonTabViewModel
{
    Id = "home",
    Header = "Home"
};

var clipboardGroup = new RibbonGroupViewModel
{
    Id = "clipboard",
    Header = "Clipboard"
};

clipboardGroup.ItemsViewModel.Add(new RibbonItemViewModel
{
    Id = "paste",
    Label = "Paste",
    CommandId = "paste"
});

homeTab.GroupsViewModel.Add(clipboardGroup);
ribbon.Tabs.Add(homeTab);
```

## Persistence

`RibbonControl.Persistence.Json` ships a file-based `IRibbonStateStore` implementation for saving and restoring `RibbonRuntimeState`.

```csharp
using RibbonControl.Persistence.Json.Storage;
using RibbonControl.Persistence.Json.Storage.SampleMigrations;

var stateStore = new JsonRibbonStateStore(
    "ribbon-state.json",
    new JsonRibbonStateStoreOptions
    {
        CurrentSchemaVersion = 2,
        Migrations =
        {
            new Schema1To2QuickAccessPlacementMigration()
        }
    });
```

Bind the store to `Ribbon.StateStore`, then use the built-in `LoadStateCommand`, `SaveStateCommand`, `ResetStateCommand`, and `ResetCustomizationCommand` commands exposed by the control.

## Samples

| Project | Focus |
|---------|-------|
| `RibbonControl.Samples.XamlOnly` | Entire ribbon declared in XAML, including backstage and command presentation |
| `RibbonControl.Samples.MvvmOnly` | Ribbon generated from view-model state with two-way runtime synchronization |
| `RibbonControl.Samples.Hybrid` | Static shell in XAML with dynamic tabs, groups, and backstage content |

## Architecture

The package split is intentionally small:

- `RibbonControl.Core`: controls, themes, automation peers, runtime models, MVVM view models, merge logic, key tips, and state/customization services.
- `RibbonControl.Persistence.Json`: a file-based `IRibbonStateStore` implementation with schema versioning and migration support.

Application code references `RibbonControl.Core` directly and adds `RibbonControl.Persistence.Json` when persistent ribbon state is required.

## Build, Test, and Pack

```bash
dotnet restore RibbonControl.sln
dotnet build RibbonControl.sln -c Release
dotnet test RibbonControl.sln -c Release
dotnet pack src/RibbonControl.Core/RibbonControl.Core.csproj -c Release
dotnet pack src/RibbonControl.Persistence.Json/RibbonControl.Persistence.Json.csproj -c Release
```

The GitHub Actions setup includes:

- `CI`: matrix build and test on macOS, Linux, and Windows, plus NuGet package artifact generation.
- `Docs`: Lunet build and GitHub Pages deployment from the default branch.
- `Release`: tag-driven package build, NuGet.org publish, symbol package upload, and GitHub release creation.

Create a tag such as `v1.0.0` to publish a versioned release through the release workflow.
