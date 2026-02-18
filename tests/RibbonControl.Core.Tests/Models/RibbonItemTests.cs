// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using RibbonControl.Core.Models;
using RibbonControl.Core.Enums;
using RibbonControl.Core.ViewModels;
using Avalonia.Media;

namespace RibbonControl.Core.Tests.Models;

public class RibbonItemTests
{
    [Fact]
    public void MenuItemFlags_ProduceFilteredPreviewAndPopupCollections()
    {
        var item = new RibbonItem
        {
            Id = "styles",
            Label = "Styles",
        };

        item.MenuItems.Add(new RibbonMenuItem
        {
            Id = "preview-only",
            Label = "Normal",
            ShowInRibbonPreview = true,
            ShowInPopup = false,
        });
        item.MenuItems.Add(new RibbonMenuItem
        {
            Id = "popup-only",
            Label = "Heading 2",
            ShowInRibbonPreview = false,
            ShowInPopup = true,
        });
        item.MenuItems.Add(new RibbonMenuItem
        {
            Id = "both",
            Label = "No Spacing",
            ShowInRibbonPreview = true,
            ShowInPopup = true,
        });

        Assert.Equal(2, item.RibbonPreviewMenuItems.Count);
        Assert.Equal(2, item.PopupMenuItems.Count);
        Assert.True(item.HasRibbonPreviewMenuItems);
        Assert.True(item.HasPopupMenuItems);
        Assert.Contains(item.RibbonPreviewMenuItems, x => x.Id == "preview-only");
        Assert.DoesNotContain(item.PopupMenuItems, x => x.Id == "preview-only");
        Assert.Contains(item.PopupMenuItems, x => x.Id == "popup-only");
        Assert.DoesNotContain(item.RibbonPreviewMenuItems, x => x.Id == "popup-only");
    }

    [Fact]
    public void GalleryPreviewAndPopupCategoryFlags_RespectConfiguration()
    {
        var item = new RibbonItem
        {
            Id = "table-styles",
            Label = "Table Styles",
            Primitive = RibbonItemPrimitive.Gallery,
            GalleryPreviewMaxItems = 2,
        };

        item.MenuItems.Add(new RibbonMenuItem
        {
            Id = "plain-1",
            Label = "Plain 1",
            Category = "Plain Tables",
            ShowInRibbonPreview = true,
            ShowInPopup = true,
        });
        item.MenuItems.Add(new RibbonMenuItem
        {
            Id = "plain-2",
            Label = "Plain 2",
            Category = "Plain Tables",
            ShowInRibbonPreview = true,
            ShowInPopup = true,
        });
        item.MenuItems.Add(new RibbonMenuItem
        {
            Id = "grid-1",
            Label = "Grid 1",
            Category = "Grid Tables",
            ShowInRibbonPreview = true,
            ShowInPopup = true,
        });
        item.MenuItems.Add(new RibbonMenuItem
        {
            Id = "clear",
            Label = "Clear",
            ShowInRibbonPreview = false,
            ShowInPopup = true,
        });

        Assert.Equal(2, item.RibbonPreviewMenuItems.Count);
        Assert.True(item.HasPopupMenuCategories);
        Assert.True(item.ShowPopupMenuCategories);
        Assert.False(item.ShowFlatCategorizedPopupMenuItems);
        Assert.True(item.ShowUncategorizedPopupMenuItems);
        Assert.False(item.ShowLegacyPopupMenuItems);
        Assert.Equal(2, item.PopupMenuCategories.Count);
        Assert.Equal("Plain Tables", item.PopupMenuCategories[0].Header);
        Assert.Equal("Grid Tables", item.PopupMenuCategories[1].Header);

        item.GalleryShowCategoryHeaders = false;

        Assert.False(item.ShowPopupMenuCategories);
        Assert.True(item.ShowFlatCategorizedPopupMenuItems);
    }

    [Fact]
    public void StructuredPopupSections_GroupAndOrderMenuItems()
    {
        var item = new RibbonItem
        {
            Id = "styles",
            Label = "Styles",
            Primitive = RibbonItemPrimitive.Gallery,
        };

        item.MenuItems.Add(new RibbonMenuItem
        {
            Id = "style-normal",
            Label = "Normal",
            ShowInRibbonPreview = false,
            ShowInPopup = true,
            ShowChevron = true,
            Order = 0,
            PopupSectionId = "style-presets",
            PopupSectionHeader = "Quick Styles",
            PopupSectionOrder = 0,
            PopupSectionLayout = RibbonPopupSectionLayout.GalleryWrap,
        });
        item.MenuItems.Add(new RibbonMenuItem
        {
            Id = "style-heading",
            Label = "Heading 1",
            ShowInRibbonPreview = false,
            ShowInPopup = true,
            ShowChevron = true,
            Order = 1,
            PopupSectionId = "style-presets",
            PopupSectionOrder = 0,
            PopupSectionLayout = RibbonPopupSectionLayout.GalleryWrap,
        });
        item.MenuItems.Add(new RibbonMenuItem
        {
            Id = "style-more",
            Label = "See More Styles",
            ShowInRibbonPreview = false,
            ShowInPopup = true,
            Order = 10,
            PopupSectionId = "style-actions",
            PopupSectionOrder = 1,
        });
        item.MenuItems.Add(new RibbonMenuItem
        {
            Id = "style-create",
            Label = "Create New Style",
            ShowInRibbonPreview = false,
            ShowInPopup = true,
            Order = 11,
            PopupSectionId = "style-actions",
            PopupSectionOrder = 1,
        });

        Assert.True(item.HasExplicitPopupMenuSections);
        Assert.True(item.ShowStructuredPopupMenuSections);
        Assert.False(item.ShowPopupMenuCategories);
        Assert.False(item.ShowFlatCategorizedPopupMenuItems);
        Assert.False(item.ShowUncategorizedPopupMenuItems);
        Assert.False(item.ShowLegacyPopupMenuItems);
        Assert.Equal(2, item.PopupMenuSections.Count);

        var presets = item.PopupMenuSections[0];
        Assert.Equal("style-presets", presets.Id);
        Assert.Equal("Quick Styles", presets.Header);
        Assert.True(presets.IsGalleryLayout);
        Assert.False(presets.ShowSeparator);
        Assert.Equal(2, presets.Items.Count);

        var actions = item.PopupMenuSections[1];
        Assert.Equal("style-actions", actions.Id);
        Assert.True(actions.IsCommandListLayout);
        Assert.True(actions.ShowSeparator);
        Assert.Equal(2, actions.Items.Count);
    }

    [Fact]
    public void PopupSections_DefaultToSingleCommandSectionWithoutMetadata()
    {
        var item = new RibbonItem
        {
            Id = "table",
            Label = "Table",
            Primitive = RibbonItemPrimitive.MenuButton,
        };

        item.MenuItems.Add(new RibbonMenuItem
        {
            Id = "table-2x2",
            Label = "2 x 2",
            ShowInPopup = true,
            ShowInRibbonPreview = false,
            Order = 0,
        });
        item.MenuItems.Add(new RibbonMenuItem
        {
            Id = "table-3x3",
            Label = "3 x 3",
            ShowInPopup = true,
            ShowInRibbonPreview = false,
            Order = 1,
        });

        Assert.False(item.HasExplicitPopupMenuSections);
        Assert.False(item.ShowStructuredPopupMenuSections);

        var section = Assert.Single(item.PopupMenuSections);
        Assert.Equal("default", section.Id);
        Assert.True(section.IsCommandListLayout);
        Assert.False(section.ShowSeparator);
        Assert.Equal(2, section.Items.Count);
    }

    [Fact]
    public void SelectingGalleryMenuItem_DeselectsSiblingSelection()
    {
        var item = new RibbonItem
        {
            Id = "styles",
            Label = "Styles",
            Primitive = RibbonItemPrimitive.Gallery,
        };

        item.MenuItems.Add(new RibbonMenuItem
        {
            Id = "style-normal",
            Label = "Normal",
        });
        item.MenuItems.Add(new RibbonMenuItem
        {
            Id = "style-heading",
            Label = "Heading 1",
        });

        item.MenuItems[0].IsSelected = true;
        Assert.True(item.MenuItems[0].IsSelected);
        Assert.False(item.MenuItems[1].IsSelected);

        item.MenuItems[1].IsSelected = true;
        Assert.False(item.MenuItems[0].IsSelected);
        Assert.True(item.MenuItems[1].IsSelected);
        Assert.Equal("style-heading", item.SelectedGalleryMenuItem?.Id);
    }

    [Fact]
    public void PrimitiveSpecificDropDownFlags_EnableOnlyActivePrimitivePopup()
    {
        var item = new RibbonItem
        {
            Id = "styles",
            Label = "Styles",
            Primitive = RibbonItemPrimitive.Gallery,
            IsDropDownOpen = true,
        };

        Assert.False(item.IsSplitDropDownOpen);
        Assert.False(item.IsMenuDropDownOpen);
        Assert.True(item.IsGalleryDropDownOpen);

        item.Primitive = RibbonItemPrimitive.MenuButton;
        Assert.False(item.IsSplitDropDownOpen);
        Assert.True(item.IsMenuDropDownOpen);
        Assert.False(item.IsGalleryDropDownOpen);

        item.Primitive = RibbonItemPrimitive.PasteSplitButton;
        Assert.True(item.IsSplitDropDownOpen);
        Assert.False(item.IsMenuDropDownOpen);
        Assert.False(item.IsGalleryDropDownOpen);
    }

    [Fact]
    public void PrimitiveSpecificDropDownFlags_TwoWaySetters_OnlyAffectMatchingPrimitive()
    {
        var item = new RibbonItem
        {
            Id = "table",
            Label = "Table",
            Primitive = RibbonItemPrimitive.MenuButton,
        };

        item.IsSplitDropDownOpen = true;
        Assert.False(item.IsDropDownOpen);

        item.IsMenuDropDownOpen = true;
        Assert.True(item.IsDropDownOpen);
        Assert.False(item.IsSplitDropDownOpen);
        Assert.True(item.IsMenuDropDownOpen);
        Assert.False(item.IsGalleryDropDownOpen);

        item.IsMenuDropDownOpen = false;
        Assert.False(item.IsDropDownOpen);

        item.Primitive = RibbonItemPrimitive.SplitButton;
        item.IsSplitDropDownOpen = true;
        Assert.True(item.IsDropDownOpen);
        Assert.True(item.IsSplitDropDownOpen);
        Assert.False(item.IsMenuDropDownOpen);

        item.IsDropDownOpen = false;
        item.Primitive = RibbonItemPrimitive.PasteSplitButton;
        item.IsSplitDropDownOpen = true;
        Assert.True(item.IsDropDownOpen);
        Assert.True(item.IsSplitDropDownOpen);
        Assert.False(item.IsMenuDropDownOpen);
    }

    [Fact]
    public void AdaptiveEffectiveSize_OverridesAndResetsButtonVisibilityFlags()
    {
        var item = new RibbonItem
        {
            Id = "paste",
            Label = "Paste",
            Primitive = RibbonItemPrimitive.Button,
            Size = RibbonItemSize.Large,
        };

        Assert.True(item.IsEffectiveLargeButtonPrimitive);
        Assert.False(item.IsEffectiveSmallButtonPrimitive);

        item.SetAdaptiveEffectiveSize(RibbonItemSize.Small);
        Assert.False(item.IsEffectiveLargeButtonPrimitive);
        Assert.True(item.IsEffectiveSmallButtonPrimitive);

        item.SetAdaptiveEffectiveSize(null);
        Assert.True(item.IsEffectiveLargeButtonPrimitive);
        Assert.False(item.IsEffectiveSmallButtonPrimitive);
    }

    [Fact]
    public void DisplayMode_IconOnly_SwitchesButtonPresentationFlags()
    {
        var item = new RibbonItem
        {
            Id = "undo",
            Label = "Undo",
            Primitive = RibbonItemPrimitive.Button,
            Size = RibbonItemSize.Small,
            DisplayMode = RibbonItemDisplayMode.IconOnly,
        };

        Assert.True(item.IsIconOnlyDisplayMode);
        Assert.False(item.IsAutoDisplayMode);
        Assert.False(item.IsEffectiveSmallButtonPrimitive);
        Assert.True(item.IsEffectiveSmallIconOnlyButtonPrimitive);

        item.SetAdaptiveEffectiveSize(RibbonItemSize.Large);
        Assert.True(item.IsEffectiveLargeIconOnlyButtonPrimitive);
        Assert.False(item.IsEffectiveLargeButtonPrimitive);
    }

    [Fact]
    public void SplitSecondaryModeFlags_TrackPopupAndSecondaryCommandSources()
    {
        var item = new RibbonItem
        {
            Id = "equation",
            Label = "Equation",
            Primitive = RibbonItemPrimitive.SplitButton,
        };

        Assert.False(item.HasSplitPopup);
        Assert.False(item.HasSecondaryCommand);
        Assert.False(item.IsSplitDropDownToggleVisible);
        Assert.False(item.IsSplitSecondaryCommandVisible);
        Assert.True(item.IsSplitFallbackToggleVisible);

        item.SecondaryCommandId = "symbol-more";

        Assert.True(item.HasSecondaryCommand);
        Assert.False(item.HasSplitPopup);
        Assert.False(item.IsSplitDropDownToggleVisible);
        Assert.True(item.IsSplitSecondaryCommandVisible);
        Assert.False(item.IsSplitFallbackToggleVisible);

        item.MenuItems.Add(new RibbonMenuItem
        {
            Id = "equation-insert",
            Label = "Insert New Equation",
            ShowInPopup = true,
            ShowInRibbonPreview = false,
        });

        Assert.True(item.HasSplitPopup);
        Assert.True(item.IsSplitDropDownToggleVisible);
        Assert.False(item.IsSplitSecondaryCommandVisible);
        Assert.False(item.IsSplitFallbackToggleVisible);

        item.MenuItems[0].ShowInPopup = false;

        Assert.False(item.HasSplitPopup);
        Assert.False(item.IsSplitDropDownToggleVisible);
        Assert.True(item.IsSplitSecondaryCommandVisible);
        Assert.False(item.IsSplitFallbackToggleVisible);
    }

    [Fact]
    public void SideBySideSplitFlags_TrackSmallAndIconOnlyPresentations()
    {
        var item = new RibbonItem
        {
            Id = "bullets",
            Label = "Bullets",
            Primitive = RibbonItemPrimitive.SplitButton,
            SplitButtonMode = RibbonSplitButtonMode.SideBySide,
            Size = RibbonItemSize.Small,
            DisplayMode = RibbonItemDisplayMode.IconOnly,
        };

        Assert.True(item.IsSideBySideSplitButtonPrimitive);
        Assert.False(item.IsSideBySideLargeSplitButtonPrimitive);
        Assert.False(item.IsSideBySideSmallSplitButtonPrimitive);
        Assert.False(item.IsSideBySideLargeIconOnlySplitButtonPrimitive);
        Assert.True(item.IsSideBySideSmallIconOnlySplitButtonPrimitive);

        item.DisplayMode = RibbonItemDisplayMode.Auto;

        Assert.False(item.IsSideBySideSmallIconOnlySplitButtonPrimitive);
        Assert.True(item.IsSideBySideSmallSplitButtonPrimitive);

        item.SetAdaptiveEffectiveSize(RibbonItemSize.Large);

        Assert.True(item.IsSideBySideLargeSplitButtonPrimitive);
        Assert.False(item.IsSideBySideSmallSplitButtonPrimitive);
    }

    [Fact]
    public void StackedSplitModeFlags_ShowTopAndBottomSplitSegments()
    {
        var item = new RibbonItem
        {
            Id = "cell-shading",
            Label = "Cell Shading",
            Primitive = RibbonItemPrimitive.SplitButton,
            SplitButtonMode = RibbonSplitButtonMode.Stacked,
            SecondaryCommandId = "page-color",
        };

        Assert.True(item.IsStackedSplitButtonPrimitive);
        Assert.False(item.IsSideBySideSplitButtonPrimitive);
        Assert.False(item.IsStackedSplitBottomSplitVisible);
        Assert.True(item.IsStackedSplitBottomCommandOnlyVisible);
        Assert.False(item.IsStackedSplitBottomDropDownOnlyVisible);
        Assert.False(item.IsStackedSplitBottomFallbackVisible);

        item.MenuItems.Add(new RibbonMenuItem
        {
            Id = "page-color-no-color",
            Label = "No Color",
            ShowInPopup = true,
            ShowInRibbonPreview = false,
        });

        Assert.True(item.HasSplitPopup);
        Assert.True(item.IsStackedSplitBottomSplitVisible);
        Assert.False(item.IsStackedSplitBottomCommandOnlyVisible);
        Assert.False(item.IsStackedSplitBottomDropDownOnlyVisible);
        Assert.False(item.IsStackedSplitBottomFallbackVisible);
        Assert.False(item.IsSplitSecondaryCommandVisible);
    }

    [Fact]
    public void PasteSplitPrimitive_UsesTopCommandAndBottomDropDownPattern()
    {
        var item = new RibbonItem
        {
            Id = "paste",
            Label = "Paste",
            Primitive = RibbonItemPrimitive.PasteSplitButton,
            SecondaryCommandId = "paste-secondary",
        };

        Assert.True(item.IsPasteSplitButtonPrimitive);
        Assert.True(item.IsStackedSplitButtonPrimitive);
        Assert.False(item.IsSideBySideSplitButtonPrimitive);
        Assert.False(item.IsStackedSplitBottomSplitVisible);
        Assert.False(item.IsStackedSplitBottomCommandOnlyVisible);
        Assert.False(item.IsStackedSplitBottomDropDownOnlyVisible);
        Assert.True(item.IsStackedSplitBottomFallbackVisible);

        item.MenuItems.Add(new RibbonMenuItem
        {
            Id = "paste-default",
            Label = "Paste",
            ShowInPopup = true,
            ShowInRibbonPreview = false,
        });

        Assert.True(item.HasSplitPopup);
        Assert.False(item.IsStackedSplitBottomSplitVisible);
        Assert.False(item.IsStackedSplitBottomCommandOnlyVisible);
        Assert.True(item.IsStackedSplitBottomDropDownOnlyVisible);
        Assert.False(item.IsStackedSplitBottomFallbackVisible);
    }

    [Fact]
    public void TogglePrimitiveFlags_RespectEffectiveSizing()
    {
        var item = new RibbonItem
        {
            Id = "bold",
            Label = "B",
            Primitive = RibbonItemPrimitive.ToggleButton,
            Size = RibbonItemSize.Large,
        };

        Assert.True(item.IsToggleButtonPrimitive);
        Assert.True(item.IsEffectiveLargeToggleButtonPrimitive);
        Assert.False(item.IsEffectiveSmallToggleButtonPrimitive);

        item.SetAdaptiveEffectiveSize(RibbonItemSize.Small);

        Assert.False(item.IsEffectiveLargeToggleButtonPrimitive);
        Assert.True(item.IsEffectiveSmallToggleButtonPrimitive);
    }

    [Fact]
    public void ToggleEnabledButton_UsesTogglePresentationAcrossSizes()
    {
        var item = new RibbonItem
        {
            Id = "ruler",
            Label = "Ruler",
            Primitive = RibbonItemPrimitive.Button,
            Size = RibbonItemSize.Large,
            IsToggle = true,
        };

        Assert.True(item.IsToggleButtonPrimitive);
        Assert.False(item.IsEffectiveLargeButtonPrimitive);
        Assert.True(item.IsEffectiveLargeToggleButtonPrimitive);
        Assert.False(item.IsEffectiveSmallToggleButtonPrimitive);

        item.SetAdaptiveEffectiveSize(RibbonItemSize.Small);
        Assert.False(item.IsEffectiveLargeToggleButtonPrimitive);
        Assert.True(item.IsEffectiveSmallToggleButtonPrimitive);

        item.DisplayMode = RibbonItemDisplayMode.IconOnly;
        Assert.False(item.IsEffectiveSmallToggleButtonPrimitive);
        Assert.True(item.IsEffectiveSmallIconOnlyToggleButtonPrimitive);
    }

    [Fact]
    public void Clone_PreservesToggleStateSyncWithSourceItem()
    {
        var source = new RibbonItem
        {
            Id = "ruler",
            Label = "Ruler",
            Primitive = RibbonItemPrimitive.Button,
            IsToggle = true,
            IsChecked = false,
        };

        var clone = RibbonModelConverter.Clone(source);
        Assert.False(clone.IsChecked);
        Assert.True(clone.IsToggle);

        source.IsChecked = true;
        Assert.True(clone.IsChecked);

        clone.IsChecked = false;
        Assert.False(source.IsChecked);
    }

    [Fact]
    public void Clone_PreservesNestedItemsTree()
    {
        var source = new RibbonItem
        {
            Id = "font-group",
            Label = "Font Group",
            Primitive = RibbonItemPrimitive.Group,
        };
        var row = new RibbonItem
        {
            Id = "font-row",
            Label = "Font Row",
            Primitive = RibbonItemPrimitive.Row,
        };
        row.Items.Add(new RibbonItem
        {
            Id = "bold",
            Label = "Bold",
            Primitive = RibbonItemPrimitive.ToggleButton,
            IsToggle = true,
            IsChecked = true,
        });
        source.Items.Add(row);

        var clone = RibbonModelConverter.Clone(source);

        var clonedRow = Assert.Single(clone.Items);
        var clonedBold = Assert.Single(clonedRow.Items);
        Assert.Equal("font-row", clonedRow.Id);
        Assert.Equal("bold", clonedBold.Id);
        Assert.NotSame(row, clonedRow);
        Assert.NotSame(row.Items[0], clonedBold);

        row.Items.Add(new RibbonItem { Id = "italic", Label = "Italic", Primitive = RibbonItemPrimitive.ToggleButton });
        Assert.Single(clonedRow.Items);
    }

    [Fact]
    public void ConvertedItem_SynchronizesToggleStateWithViewModelNode()
    {
        var source = new RibbonItemViewModel
        {
            Id = "navigation-pane",
            Label = "Navigation Pane",
            Primitive = RibbonItemPrimitive.Button,
            IsToggle = true,
            IsChecked = false,
        };

        var runtimeItem = RibbonModelConverter.ToRibbonItem(source);
        Assert.True(runtimeItem.IsToggle);
        Assert.False(runtimeItem.IsChecked);

        source.IsChecked = true;
        Assert.True(runtimeItem.IsChecked);

        runtimeItem.IsChecked = false;
        Assert.False(source.IsChecked);

        runtimeItem.IsToggle = false;
        Assert.False(source.IsToggle);
    }

    [Fact]
    public void ComboBoxPrimitive_TracksSelectedMenuItemIdAndSelectionFlags()
    {
        var item = new RibbonItem
        {
            Id = "font-size",
            Label = "Size",
            Primitive = RibbonItemPrimitive.ComboBox,
            Size = RibbonItemSize.Small,
            SelectedMenuItemId = "font-size-12",
        };

        item.MenuItems.Add(new RibbonMenuItem
        {
            Id = "font-size-10",
            Label = "10",
            CommandId = "font-size-10",
            Order = 0,
        });
        item.MenuItems.Add(new RibbonMenuItem
        {
            Id = "font-size-12",
            Label = "12",
            CommandId = "font-size-12",
            Order = 1,
        });
        item.MenuItems.Add(new RibbonMenuItem
        {
            Id = "font-size-14",
            Label = "14",
            CommandId = "font-size-14",
            Order = 2,
        });

        Assert.True(item.IsComboBoxPrimitive);
        Assert.True(item.HasComboBoxMenuItems);
        Assert.Equal("font-size-12", item.SelectedMenuItemId);
        Assert.Equal("font-size-12", item.SelectedComboBoxMenuItem?.Id);
        Assert.True(item.MenuItems[1].IsSelected);
        Assert.False(item.MenuItems[0].IsSelected);

        item.SelectedMenuItemId = "font-size-14";

        Assert.Equal("font-size-14", item.SelectedComboBoxMenuItem?.Id);
        Assert.True(item.MenuItems[2].IsSelected);
        Assert.False(item.MenuItems[1].IsSelected);
    }

    [Fact]
    public void ToggleGroupMultipleSelection_AllowsMultipleSelections()
    {
        var item = new RibbonItem
        {
            Id = "table-style-options",
            Label = "Table Style Options",
            Primitive = RibbonItemPrimitive.ToggleGroup,
            ToggleGroupSelectionMode = RibbonToggleGroupSelectionMode.Multiple,
            ToggleGroupColumns = 2,
        };

        item.MenuItems.Add(new RibbonMenuItem { Id = "header-row", Label = "Header Row", Order = 0 });
        item.MenuItems.Add(new RibbonMenuItem { Id = "first-column", Label = "First Column", Order = 1 });
        item.MenuItems.Add(new RibbonMenuItem { Id = "banded-rows", Label = "Banded Rows", Order = 2 });

        item.MenuItems[0].IsSelected = true;
        item.MenuItems[1].IsSelected = true;

        Assert.True(item.IsToggleGroupPrimitive);
        Assert.True(item.IsToggleGroupMultipleSelectionMode);
        Assert.False(item.IsToggleGroupSingleSelectionMode);
        Assert.True(item.MenuItems[0].IsSelected);
        Assert.True(item.MenuItems[1].IsSelected);
        Assert.Null(item.SelectedMenuItemId);
        Assert.Null(item.SelectedComboBoxMenuItem);
        Assert.Equal(3, item.ToggleGroupMenuItems.Count);
    }

    [Fact]
    public void ToggleGroupSingleSelection_DeselectsSiblingsAndTracksSelectedItem()
    {
        var item = new RibbonItem
        {
            Id = "table-border-scope",
            Label = "Border Scope",
            Primitive = RibbonItemPrimitive.ToggleGroup,
            ToggleGroupSelectionMode = RibbonToggleGroupSelectionMode.Single,
            ToggleGroupColumns = 1,
        };

        item.MenuItems.Add(new RibbonMenuItem { Id = "scope-all", Label = "All Borders", IsSelected = true, Order = 0 });
        item.MenuItems.Add(new RibbonMenuItem { Id = "scope-inside", Label = "Inside Borders", Order = 1 });
        item.MenuItems.Add(new RibbonMenuItem { Id = "scope-outside", Label = "Outside Borders", Order = 2 });

        item.MenuItems[1].IsSelected = true;

        Assert.True(item.IsToggleGroupPrimitive);
        Assert.True(item.IsToggleGroupSingleSelectionMode);
        Assert.False(item.IsToggleGroupMultipleSelectionMode);
        Assert.False(item.MenuItems[0].IsSelected);
        Assert.True(item.MenuItems[1].IsSelected);
        Assert.Equal("scope-inside", item.SelectedMenuItemId);
        Assert.Equal("scope-inside", item.SelectedComboBoxMenuItem?.Id);
    }

    [Fact]
    public void ToggleGroupColumns_AreClampedToAtLeastOne()
    {
        var item = new RibbonItem
        {
            Id = "table-style-options",
            Label = "Table Style Options",
            Primitive = RibbonItemPrimitive.ToggleGroup,
            ToggleGroupColumns = 0,
        };

        Assert.Equal(1, item.ToggleGroupColumns);

        item.ToggleGroupColumns = -10;
        Assert.Equal(1, item.ToggleGroupColumns);
    }

    [Fact]
    public void MenuItemContentFlags_TrackContentPresence()
    {
        var item = new RibbonMenuItem
        {
            Id = "style-normal",
            Label = "Normal",
        };

        Assert.False(item.HasContent);
        Assert.True(item.HasNoContent);

        item.Content = "Normal | Aptos 12";
        Assert.True(item.HasContent);
        Assert.False(item.HasNoContent);

        item.Content = null;
        Assert.False(item.HasContent);
        Assert.True(item.HasNoContent);
    }

    [Fact]
    public void IconMetadata_HasIconTracksPathResourceEmojiAndCustomIcon()
    {
        var item = new RibbonItem
        {
            Id = "icon-source",
            Label = "Icon Source",
        };

        Assert.False(item.HasIcon);

        item.IconPathData = "M0 0";
        Assert.True(item.HasIcon);
        Assert.True(item.HasIconPathData);

        item.IconPathData = null;
        item.IconResourceKey = "Ribbon.Sample.Icon";
        Assert.True(item.HasIcon);

        item.IconResourceKey = null;
        item.IconEmoji = "📌";
        Assert.True(item.HasIcon);

        item.IconEmoji = null;
        item.Icon = new object();
        Assert.True(item.HasIcon);

        item.Icon = null;
        Assert.False(item.HasIcon);
    }

    [Fact]
    public void ConvertedItem_PreservesIconMetadataFromViewModel()
    {
        var viewModel = new RibbonItemViewModel
        {
            Id = "emoji-item",
            Label = "Emoji",
            IconResourceKey = "Ribbon.Sample.IconKey",
            IconPathData = "M1 1 L2 2",
            IconEmoji = "🎯",
            IconStretch = Stretch.Fill,
            IconStretchDirection = StretchDirection.DownOnly,
            IconWidth = 18,
            IconHeight = 18,
            IconMinWidth = 14,
            IconMinHeight = 14,
            IconMaxWidth = 24,
            IconMaxHeight = 24,
        };

        var runtimeItem = RibbonModelConverter.ToRibbonItem(viewModel);

        Assert.Equal("Ribbon.Sample.IconKey", runtimeItem.IconResourceKey);
        Assert.Equal("M1 1 L2 2", runtimeItem.IconPathData);
        Assert.Equal("🎯", runtimeItem.IconEmoji);
        Assert.Equal(Stretch.Fill, runtimeItem.IconStretch);
        Assert.Equal(StretchDirection.DownOnly, runtimeItem.IconStretchDirection);
        Assert.Equal(18, runtimeItem.IconWidth);
        Assert.Equal(18, runtimeItem.IconHeight);
        Assert.Equal(14, runtimeItem.IconMinWidth);
        Assert.Equal(14, runtimeItem.IconMinHeight);
        Assert.Equal(24, runtimeItem.IconMaxWidth);
        Assert.Equal(24, runtimeItem.IconMaxHeight);
    }

    [Fact]
    public void ConvertedItem_PreservesPopupSectionMetadataFromViewModel()
    {
        var viewModel = new RibbonItemViewModel
        {
            Id = "style-gallery",
            Label = "Styles",
            Primitive = RibbonItemPrimitive.Gallery,
        };

        viewModel.MenuItemsViewModel.Add(new RibbonMenuItemViewModel
        {
            Id = "style-normal",
            Label = "Normal",
            ShowInRibbonPreview = false,
            ShowInPopup = true,
            PopupSectionId = "style-presets",
            PopupSectionHeader = "Quick Styles",
            PopupSectionOrder = 0,
            PopupSectionLayout = RibbonPopupSectionLayout.GalleryWrap,
        });

        var runtimeItem = RibbonModelConverter.ToRibbonItem(viewModel);
        var menuItem = Assert.Single(runtimeItem.MenuItems);

        Assert.Equal("style-presets", menuItem.PopupSectionId);
        Assert.Equal("Quick Styles", menuItem.PopupSectionHeader);
        Assert.Equal(0, menuItem.PopupSectionOrder);
        Assert.Equal(RibbonPopupSectionLayout.GalleryWrap, menuItem.PopupSectionLayout);
    }

    [Fact]
    public void ClosingOwnerDropDown_ClosesNestedSubMenus()
    {
        var item = new RibbonItem
        {
            Id = "styles",
            Label = "Styles",
            Primitive = RibbonItemPrimitive.Gallery,
        };

        var styles = new RibbonMenuItem
        {
            Id = "style-normal",
            Label = "Normal",
            ShowChevron = true,
        };
        var manage = new RibbonMenuItem
        {
            Id = "style-manage",
            Label = "Manage",
            ShowChevron = true,
        };
        manage.SubMenuItems.Add(new RibbonMenuItem { Id = "style-manage-document", Label = "Document" });
        styles.SubMenuItems.Add(manage);
        item.MenuItems.Add(styles);

        item.IsDropDownOpen = true;
        styles.IsSubMenuOpen = true;
        manage.IsSubMenuOpen = true;

        Assert.True(styles.IsSubMenuOpen);
        Assert.True(manage.IsSubMenuOpen);

        item.IsDropDownOpen = false;

        Assert.False(styles.IsSubMenuOpen);
        Assert.False(manage.IsSubMenuOpen);
    }

    [Fact]
    public void OpeningSubMenu_ClosesSiblingSubMenus()
    {
        var root = new RibbonMenuItem
        {
            Id = "root",
            Label = "Root",
            ShowChevron = true,
        };

        var first = new RibbonMenuItem
        {
            Id = "first",
            Label = "First",
            ShowChevron = true,
        };
        first.SubMenuItems.Add(new RibbonMenuItem { Id = "first-child", Label = "First Child" });

        var second = new RibbonMenuItem
        {
            Id = "second",
            Label = "Second",
            ShowChevron = true,
        };
        second.SubMenuItems.Add(new RibbonMenuItem { Id = "second-child", Label = "Second Child" });

        root.SubMenuItems.Add(first);
        root.SubMenuItems.Add(second);

        first.IsSubMenuOpen = true;
        Assert.True(first.IsSubMenuOpen);
        Assert.False(second.IsSubMenuOpen);

        second.IsSubMenuOpen = true;
        Assert.False(first.IsSubMenuOpen);
        Assert.True(second.IsSubMenuOpen);
    }
}
