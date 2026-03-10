---
title: "Quickstart: XAML"
---

# Quickstart: XAML

Use direct XAML composition when the ribbon layout is mostly static.

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

Key takeaways:

- `RibbonTab`, `RibbonGroup`, and `RibbonItem` can all be authored inline.
- `RibbonBackstage` can be supplied through `Ribbon.Backstage`.
- Top-bar and header content areas are normal content slots and can host arbitrary Avalonia controls.
