---
title: "Theme Usage and Resources"
---

# Theme Usage and Resources

RibbonControl ships its visual resources in `avares://RibbonControl.Core/Themes/Generic.axaml`.

Recommended application setup:

```xml
<Application.Styles>
  <FluentTheme />
  <StyleInclude Source="avares://RibbonControl.Core/Themes/Generic.axaml" />
</Application.Styles>
```

Resource guidance:

- keep the ribbon namespace mapped with `xmlns:ribbon="https://github.com/wieslawsoltes/ribboncontrol"`,
- prefer reusable icon path constants from `FluentIconData`,
- keep custom control themes and shared resources in resource dictionaries rather than code.

The sample apps also show how to override `RibbonIconPresenter` themes with a `ControlTheme` scoped to the application resources.
