# RibbonControl v1+ Spec and Plan (Office/Word Parity Track)

## Scope and intent
RibbonControl must deliver feature-equivalent behavior for modern Office-style ribbons while preserving three first-class authoring modes in a single library:
1. XAML-only composition.
2. MVVM/data-driven composition.
3. Hybrid composition (static XAML + dynamic contributions in one ribbon).

Target frameworks remain `net10.0` (primary) and `net8.0` (secondary) on Avalonia `11.3.x`.

## Current status summary
Implemented and shipping in-repo today:
1. Core controls and merge model (`Ribbon`, `RibbonTab`, `RibbonGroup`, `RibbonItem`) with deterministic ID-based merge policy.
2. XAML, MVVM, and hybrid binding surfaces (`TabsSource`, merge modes, state ownership, runtime state import/export/load/save/reset).
3. Backstage overlay, contextual tab band, quick access toolbar, key-tip generation with collision handling.
4. Core automation peers and headless parity tests (cross-mode contextual/backstage/key-tip parity).
5. JSON persistence and migration sample for runtime/customization state.
6. Office theme package and three sample apps.
7. Primitive command families (button/split/menu/gallery/custom-host) with popup content support and rich menu metadata (description, shortcuts, separators, chevrons).
8. Gallery preview-vs-popup menu filtering and dedicated ribbon top-bar slots (`TopBarStart/Center/EndContent`) for Office-like chrome composition.
9. Ribbon surface stability controls (`MaintainStableRibbonHeight`) and command-height synchronization (`SynchronizeCommandHeights`) with auto-measured height floors for large/small commands.
10. Hierarchical popup ribbon menus (menu button/split/gallery) with nested submenu model contracts (`SubMenuItems`) and popup metadata (`PopupTitle`, `PopupFooterContent`, `PopupMinWidth`, `PopupMaxHeight`).
11. Cross-mode popup parity coverage in tests, including deterministic submenu merge behavior and submenu close rules (owner close cascade + sibling exclusivity).
12. Theme foundation refactor for Office simplification:
- Generic control themes now consume shared `RibbonTheme*` design tokens (brushes, metrics, template slots).
- Office overrides those tokens in `OfficeTokens.axaml` instead of duplicating Ribbon/Tab/QAT control themes.
- Office control-theme overrides are reduced to the unique Backstage layout only.

## Review findings (implementation + themes)

### High priority gaps (parity blockers)
1. Office theme fidelity is currently minimal and does not emulate Word-level tab, top-row chrome, and command-surface visual language strongly enough.
2. Samples are functionally rich but still demo-centric; they do not present a full Word-like information architecture (tab/group taxonomy and realistic command map).
3. Feature surface does not yet include several Word-parity controls (split buttons, galleries, in-ribbon editors such as font/size selectors, launcher buttons, overflow galleries).

### Medium priority gaps
1. Keyboard model covers key tips and traversal basics, but parity matrix lacks exhaustive Office-like chord sequences and full tab-cycle scenarios with minimized state and backstage transitions.
2. Customization UX is service-level and persistence-ready, but end-user UI for command reorder/add/remove and QAT customization is not yet exposed as a full Word-like dialog workflow.
3. Accessibility validation exists for key surfaces but not yet complete against a formal Office parity matrix (automation patterns per control family).

### Low priority gaps
1. Visual regression coverage is currently smoke-level and should expand to per-tab and per-theme snapshots.
2. Performance baselines exist for merge throughput; command-rich rendering scenarios still need expanded thresholds with virtualized galleries.

## Office parity target definition

### Ribbon feature parity scope
1. Top chrome: QAT above/below, File (Backstage), tab strip, contextual tab groups.
2. Command primitives:
- standard button,
- toggle button,
- split button,
- menu button,
- gallery item,
- launcher action per group,
- in-ribbon editor hosts (combo/text/number).
3. Layout and scaling:
- large/medium/small presentation states,
- group collapse and overflow,
- deterministic scaling priorities.
4. Keyboard:
- Alt activation,
- key-tip map for tabs/QAT/backstage/commands,
- sequence disambiguation,
- Tab/Shift+Tab traversal and arrow navigation parity.
5. Backstage:
- navigation pane,
- page content host,
- command-centric file workflow.
6. Customization:
- reorder, hide/show, add/remove commands,
- QAT customization,
- import/export/reset profile.
7. Accessibility:
- automation peers, names/ids/help text,
- predictable focus order and control patterns.

### Mode parity guarantee
All capabilities above must be equivalent in:
1. XAML-only mode.
2. MVVM-only mode.
3. Hybrid mode.

## Architecture constraints (unchanged)
1. Core remains framework-agnostic where possible (`INotifyPropertyChanged`, `ICommand`, contract interfaces).
2. Themes/templates authored in XAML.
3. Template overrides must be authored through `ControlTheme` resources.
4. ReactiveUI remains samples-only unless explicitly requested elsewhere.
5. Stable string IDs are mandatory for all tab/group/item nodes.

## Theme simplification plan (control-theme override reduction)

### Goals
1. Keep Generic as the structural source of truth for Ribbon control templates.
2. Use token overrides for Office visual/layout differences where feasible.
3. Limit Office to minimal `ControlTheme` overrides only for structurally different surfaces.

### Implemented in this refactor
1. Added shared `RibbonTheme*` token contract in Generic tokens for:
- chrome/surface/tab brushes,
- tab metrics,
- top-row and drop-down margins,
- backstage host sizing,
- template indirection slots used by Ribbon/QAT/context-band/tab content.
2. Updated `GenericControlThemes.axaml` to consume `RibbonTheme*` tokens via `DynamicResource`.
3. Updated `GenericStyles.axaml` minimized-content template to use `RibbonThemeCollapsedTabContentTemplate`.
4. Added Office token overrides and template-slot mapping resources:
- `/Users/wieslawsoltes/GitHub/RibbonControl/src/RibbonControl.Themes.Office/Themes/Office/OfficeTokens.axaml`
- `/Users/wieslawsoltes/GitHub/RibbonControl/src/RibbonControl.Themes.Office/Themes/Office/OfficeTemplateSlots.axaml`
5. Reduced Office control themes to Backstage-only:
- `/Users/wieslawsoltes/GitHub/RibbonControl/src/RibbonControl.Themes.Office/Themes/Office/OfficeControlThemes.axaml`
6. Removed the explicit `OfficeRibbonTheme` dependency in Office styles and bound File-button styling to generic Ribbon template parts.

### Remaining follow-up (optional next pass)
1. Reduce `OfficeTemplates.axaml` duplication further by extracting shared menu/gallery item sub-templates into Generic tokenized partials.
2. Add a formal `ThemeContract.md` key table listing every `RibbonTheme*` token and its semantic role.
3. Add headless theme-resolution tests asserting Generic vs Office token bindings for key parts (tab headers, minimized host, backstage panel).

## Implementation roadmap (Office parity)

### Phase A: command surface completion
Deliverables:
1. New control models for split/menu/gallery/custom-host items and command-size metadata (`Large`/`Small`).
2. Templated rendering pipeline for mixed command primitives in groups, including custom-host content presenters.
3. Group launcher support and command routing.
4. Ribbon header extension slots (`HeaderStartContent`/`HeaderEndContent`) for top-row command surfaces (for example Comments/Share/search).

Exit criteria:
1. Home/Insert/Review-style groups can be represented, including dense small-button rows and hosted editor controls in XAML.
2. Headless tests validate command invocation and keyboard access for each primitive.

### Phase B: layout/scaling/overflow parity
Deliverables:
1. Group scaling metadata and deterministic scale rules.
2. Overflow/dropdown conversion path for constrained width.
3. Minimized-ribbon interaction parity with contextual transitions.

Exit criteria:
1. Large Word-like ribbons preserve command discoverability under narrow widths.
2. Perf and allocation thresholds documented and enforced.

### Phase C: backstage and customization UX parity
Deliverables:
1. Backstage page model (nav + page host + commands).
2. Customization UI flows (tabs/QAT reorder and add/remove).
3. Persistence schema versioning for new command primitive types.

Exit criteria:
1. End-to-end customization scenario passes in XAML/MVVM/hybrid modes.
2. Save/load/reset/migrate tests pass across schema versions.

### Phase D: keyboard and accessibility hardening
Deliverables:
1. Full key-tip parity matrix with conflict/collision tests.
2. Focus and traversal matrix per mode and per primitive.
3. Expanded automation peer coverage and pattern verification.

Exit criteria:
1. Headless keyboard matrix passes.
2. Accessibility matrix passes with deterministic automation metadata.

### Phase E: Office-grade visual system and docs
Deliverables:
1. Office theme pass (light, high contrast, optional dark variant).
2. Expanded visual regression baselines per tab and per mode.
3. Documentation updates with migration and parity tracking table.

Exit criteria:
1. Visual regression suite passes for all supported theme variants.
2. Documentation includes explicit "implemented vs planned" parity table.

## Sample strategy (Word-like showcase)

### XAML-only sample target
1. Static Word-like tabs and groups authored in XAML.
2. Backstage, contextual picture tab, QAT, key tips, minimized and state workflows.
3. Command catalog resolution for all command ids.

### MVVM-only sample target
1. Entire Word-like ribbon generated from `RibbonViewModel` graph.
2. Runtime tab mutation (for example Add-ins/plugin tab).
3. Full state synchronization and persistence commands.

### Hybrid sample target
1. Static XAML baseline for canonical tabs.
2. Dynamic MVVM contributions that merge into same tab/group/item ids.
3. Explicit demonstrations of merge conflict resolution and contextual activation.

## Test expansion plan
1. Headless keyboard traversal matrix:
- top-row to tabs,
- tabs to command surface,
- minimized open/close + escape,
- contextual activation transitions.
2. Cross-mode scenario matrix:
- same user workflow must produce equivalent observable state and command results.
3. Accessibility matrix:
- automation names/ids/pattern expectations for tabs, groups, commands, backstage, QAT.
4. Visual matrix:
- snapshots per major tab in Word-like samples.
5. Performance matrix:
- command-rich ribbons (250+ items with mixed primitives) with thresholds.

## Immediate next tasks (post-update)
1. Add group launcher model + templates.
2. Add scaling metadata and overflow behavior.
3. Add customization UI package on top of existing service contracts.
4. Extend visual/headless tests to parity matrix above (including popup dismissal and gallery-preview filtering).
5. Continue Office-theme fidelity pass for spacing, borders, and command-surface parity.
