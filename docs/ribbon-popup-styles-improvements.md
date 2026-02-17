# Ribbon Popup Styles: Complex Model and Control Improvements

## Problem analysis
The existing popup menu model supported flat popup lists and gallery category grouping, but complex Word-like style menus needed stronger structure:

1. Mixed layout regions in one popup (gallery-style presets + command list actions).
2. Section-level headers and ordering.
3. Deterministic rendering support across XAML-only, MVVM-only, and hybrid modes.

## Design

### Model extensions
1. Added `RibbonPopupSectionLayout` enum with `CommandList` and `GalleryWrap`.
2. Added popup section metadata to each menu item:
- `PopupSectionId`
- `PopupSectionHeader`
- `PopupSectionOrder`
- `PopupSectionLayout`
3. Added runtime projection `RibbonPopupMenuSection` and `RibbonItem.PopupMenuSections`.

### Runtime behavior
1. `RibbonItem` now groups popup menu items into section objects for rendering.
2. If explicit section metadata is not present, behavior remains backward compatible.
3. Gallery legacy category flags are suppressed when explicit popup sections are used.

### Template/styling updates
1. Added reusable popup section data templates for Generic and Office themes.
2. Updated split/menu/gallery popup surfaces to render structured sections.
3. Added section header/separator style hooks in both themes.

## Sample updates
The Styles gallery popup in all sample modes now demonstrates:

1. `Quick Styles` section rendered as a gallery-wrap region.
2. `style-actions` section rendered as command-list actions.
3. Nested submenu under `Manage Default Styles` still supported.

## Validation
1. Extended core tests for section grouping/order and fallback behavior.
2. Extended conversion/merge tests for new popup section metadata.
3. Full solution build passes on `net10.0` and `net8.0` targets.

## Next enhancements
1. Section-scoped max-height and pin-to-bottom action regions.
2. Optional section template key per section for specialized visuals.
3. Section-level keyboard navigation scopes for very large popups.
