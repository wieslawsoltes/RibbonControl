// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Avalonia;
using Avalonia.Automation;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.VisualTree;
using RibbonControl.Core.Controls;
using RibbonControl.Core.Enums;
using RibbonControl.Core.Models;
using RibbonControl.Core.ViewModels;
using System.Windows.Input;

namespace RibbonControl.Headless.Tests;

public class RibbonHeadlessTests
{
    [AvaloniaFact]
    public void HybridRibbon_RendersWithMergedTabs()
    {
        var ribbon = new Ribbon
        {
            SelectedTabId = "plugin",
        };

        ribbon.Tabs.Add(new RibbonTab
        {
            Id = "home",
            Header = "Home",
            Groups =
            {
                new RibbonGroup
                {
                    Id = "clipboard",
                    Header = "Clipboard",
                    Items =
                    {
                        new RibbonItem { Id = "copy", Label = "Copy" },
                    },
                },
            },
        });

        ribbon.TabsSource =
        [
            new RibbonTabViewModel
            {
                Id = "plugin",
                Header = "Plugin",
                Order = 10,
            },
        ];

        var window = new Window
        {
            Width = 1000,
            Height = 700,
            Content = ribbon,
        };

        window.Show();

        Assert.Equal(2, ribbon.MergedTabs.Count);
        Assert.Equal("plugin", ribbon.SelectedTabId);
        Assert.NotNull(TopLevel.GetTopLevel(ribbon));
    }

    [AvaloniaFact]
    public void TemplateParts_ExposeAutomationMetadata()
    {
        EnsureCoreThemeLoaded();

        var ribbon = new Ribbon();
        ribbon.Tabs.Add(new RibbonTab { Id = "home", Header = "Home" });

        var window = new Window
        {
            Width = 1000,
            Height = 700,
            Content = ribbon,
        };

        window.Show();

        var tabControl = ribbon.GetVisualDescendants().OfType<TabControl>().Single();
        Assert.Equal("Ribbon Tabs", AutomationProperties.GetName(tabControl));
        Assert.Equal("Ribbon.Tabs", AutomationProperties.GetAutomationId(tabControl));

        var backstageButtons = ribbon.GetVisualDescendants()
            .OfType<Button>()
            .Where(button => Equals(button.Tag, "__backstage"))
            .ToList();

        Assert.NotEmpty(backstageButtons);
        Assert.All(backstageButtons, button => Assert.Equal("File", AutomationProperties.GetName(button)));
    }

    [AvaloniaFact]
    public void HeaderEndContent_RendersInTemplateHost()
    {
        EnsureCoreThemeLoaded();

        var ribbon = new Ribbon
        {
            HeaderEndContent = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Children =
                {
                    new Button
                    {
                        Content = "Comments",
                    },
                    new Button
                    {
                        Content = "Share",
                    },
                },
            },
        };

        ribbon.Tabs.Add(new RibbonTab { Id = "home", Header = "Home" });

        var window = new Window
        {
            Width = 1000,
            Height = 700,
            Content = ribbon,
        };

        window.Show();
        window.UpdateLayout();

        var shareButton = ribbon.GetVisualDescendants()
            .OfType<Button>()
            .FirstOrDefault(button => string.Equals(button.Content as string, "Share", StringComparison.Ordinal));

        Assert.NotNull(shareButton);
    }

    [AvaloniaFact]
    public void TopBarContent_RendersInTemplateHost()
    {
        EnsureCoreThemeLoaded();

        var ribbon = new Ribbon
        {
            TopBarStartContent = new StackPanel
            {
                Children =
                {
                    new TextBlock
                    {
                        Text = "Document 6",
                    },
                },
            },
            TopBarCenterContent = new TextBox
            {
                Text = "Search",
            },
        };

        ribbon.Tabs.Add(new RibbonTab { Id = "home", Header = "Home" });

        var window = new Window
        {
            Width = 1000,
            Height = 700,
            Content = ribbon,
        };

        window.Show();
        window.UpdateLayout();

        var documentTitle = ribbon.GetVisualDescendants()
            .OfType<TextBlock>()
            .FirstOrDefault(textBlock => string.Equals(textBlock.Text, "Document 6", StringComparison.Ordinal));
        var searchBox = ribbon.GetVisualDescendants()
            .OfType<TextBox>()
            .FirstOrDefault(textBox => string.Equals(textBox.Text, "Search", StringComparison.Ordinal));

        Assert.NotNull(documentTitle);
        Assert.NotNull(searchBox);
    }

    [AvaloniaFact]
    public void MinimizedRibbon_TabSelection_OpensDropDownAndEscapeClosesIt()
    {
        EnsureCoreThemeLoaded();

        var ribbon = new Ribbon
        {
            IsMinimized = true,
            SelectedTabId = "home",
            Backstage = new RibbonBackstage
            {
                Content = new TextBlock { Text = "Backstage" },
            },
        };

        ribbon.Tabs.Add(new RibbonTab
        {
            Id = "home",
            Header = "Home",
            Groups =
            {
                new RibbonGroup
                {
                    Id = "clipboard",
                    Header = "Clipboard",
                    Items =
                    {
                        new RibbonItem { Id = "copy", Label = "Copy" },
                    },
                },
            },
        });

        ribbon.Tabs.Add(new RibbonTab
        {
            Id = "insert",
            Header = "Insert",
            Groups =
            {
                new RibbonGroup
                {
                    Id = "illustrations",
                    Header = "Illustrations",
                    Items =
                    {
                        new RibbonItem { Id = "picture", Label = "Picture" },
                        new RibbonItem { Id = "chart", Label = "Chart" },
                    },
                },
            },
        });

        var window = new Window
        {
            Width = 1000,
            Height = 700,
            Content = ribbon,
        };

        window.Show();
        ribbon.SelectedTabId = "home";

        Assert.False(ribbon.IsMinimizedDropDownOpen);

        var backstageButton = ribbon.GetVisualDescendants()
            .OfType<Button>()
            .First(x => Equals(x.Tag, "__backstage"));
        Assert.True(backstageButton.Focus());
        ribbon.SelectedTabId = "insert";
        window.UpdateLayout();

        Assert.True(ribbon.IsMinimizedDropDownOpen);
        var minimizedHost = ribbon.GetVisualDescendants()
            .OfType<Border>()
            .Single(x => AutomationProperties.GetAutomationId(x) == "Ribbon.Minimized.DropDown");
        Assert.True(minimizedHost.IsVisible);

        ribbon.Focus();
        window.KeyPressQwerty(PhysicalKey.Escape, RawInputModifiers.None);

        Assert.False(ribbon.IsMinimizedDropDownOpen);
        Assert.False(minimizedHost.IsVisible);
    }

    [AvaloniaFact]
    public void KeyboardTraversal_MovesBetweenTopRowAndTabs()
    {
        EnsureCoreThemeLoaded();

        var ribbon = new Ribbon
        {
            Backstage = new RibbonBackstage
            {
                Content = new TextBlock { Text = "Backstage" },
            },
        };

        ribbon.Tabs.Add(new RibbonTab
        {
            Id = "home",
            Header = "Home",
            Groups =
            {
                new RibbonGroup
                {
                    Id = "clipboard",
                    Header = "Clipboard",
                    Items =
                    {
                        new RibbonItem { Id = "copy", Label = "Copy" },
                    },
                },
            },
        });

        var window = new Window
        {
            Width = 1000,
            Height = 700,
            Content = ribbon,
        };

        window.Show();
        ribbon.SelectedTabId = "home";
        window.UpdateLayout();

        var backstageButton = ribbon.GetVisualDescendants()
            .OfType<Button>()
            .First(x => Equals(x.Tag, "__backstage"));
        var tabControl = ribbon.GetVisualDescendants().OfType<TabControl>().Single();
        var topLevel = TopLevel.GetTopLevel(ribbon);

        Assert.NotNull(topLevel);
        Assert.True(topLevel is IInputRoot);

        var inputRoot = (IInputRoot)topLevel;
        Assert.NotNull(inputRoot.FocusManager);
        Assert.NotNull(inputRoot.KeyboardNavigationHandler);

        Assert.True(backstageButton.Focus());
        Assert.True(backstageButton.IsFocused);
        Assert.Same(backstageButton, inputRoot.FocusManager!.GetFocusedElement());

        var focusedBeforeMove = inputRoot.FocusManager.GetFocusedElement();
        Assert.NotNull(focusedBeforeMove);

        inputRoot.KeyboardNavigationHandler!.Move(
            focusedBeforeMove!,
            NavigationDirection.Next,
            KeyModifiers.None);

        var focusedAfterTab = inputRoot.FocusManager.GetFocusedElement();
        Assert.NotNull(focusedAfterTab);
        Assert.False(ReferenceEquals(backstageButton, focusedAfterTab));
        Assert.True(
            focusedAfterTab is Visual focusedAfterTabVisual &&
            IsVisualOrDescendantOf(focusedAfterTabVisual, tabControl));

        inputRoot.KeyboardNavigationHandler.Move(
            focusedAfterTab!,
            NavigationDirection.Previous,
            KeyModifiers.Shift);
        Assert.True(backstageButton.IsFocused);
    }

    [AvaloniaFact]
    public void OpeningSecondDropDown_ClosesFirstDropDown()
    {
        EnsureCoreThemeLoaded();

        var ribbon = new Ribbon
        {
            SelectedTabId = "home",
        };

        ribbon.Tabs.Add(new RibbonTab
        {
            Id = "home",
            Header = "Home",
            Groups =
            {
                new RibbonGroup
                {
                    Id = "insert",
                    Header = "Insert",
                    Items =
                    {
                        new RibbonItem
                        {
                            Id = "table",
                            Label = "Table",
                            Primitive = RibbonItemPrimitive.MenuButton,
                            MenuItems =
                            {
                                new RibbonMenuItem
                                {
                                    Id = "table-2x2",
                                    Label = "2 x 2 Table",
                                    Command = new RelayCommand(_ => { }),
                                },
                            },
                        },
                        new RibbonItem
                        {
                            Id = "symbol",
                            Label = "Symbol",
                            Primitive = RibbonItemPrimitive.MenuButton,
                            MenuItems =
                            {
                                new RibbonMenuItem
                                {
                                    Id = "symbol-more",
                                    Label = "More Symbols",
                                    Command = new RelayCommand(_ => { }),
                                },
                            },
                        },
                    },
                },
            },
        });

        var window = new Window
        {
            Width = 1000,
            Height = 700,
            Content = ribbon,
        };

        window.Show();
        ribbon.SelectedTabId = "home";
        window.UpdateLayout();

        var group = Assert.Single(ribbon.MergedTabs.Single().MergedGroups);
        var first = group.MergedItems.Single(x => x.Id == "table");
        var second = group.MergedItems.Single(x => x.Id == "symbol");

        first.IsDropDownOpen = true;
        second.IsDropDownOpen = true;

        Assert.False(first.IsDropDownOpen);
        Assert.True(second.IsDropDownOpen);
    }

    [AvaloniaFact]
    public void GalleryPopupAndSubMenu_OpenSinglePrimaryPopupAndOneNestedPopup()
    {
        EnsureCoreThemeLoaded();

        var ribbon = new Ribbon
        {
            SelectedTabId = "home",
        };

        ribbon.Tabs.Add(new RibbonTab
        {
            Id = "home",
            Header = "Home",
            Groups =
            {
                new RibbonGroup
                {
                    Id = "styles",
                    Header = "Styles",
                    Items =
                    {
                        new RibbonItem
                        {
                            Id = "style-gallery",
                            Label = "Style Gallery",
                            Primitive = RibbonItemPrimitive.Gallery,
                            MenuItems =
                            {
                                new RibbonMenuItem
                                {
                                    Id = "style-normal",
                                    Label = "Normal",
                                    ShowChevron = true,
                                    SubMenuItems =
                                    {
                                        new RibbonMenuItem { Id = "style-normal-document", Label = "This Document", Command = new RelayCommand(_ => { }) },
                                    },
                                },
                                new RibbonMenuItem
                                {
                                    Id = "style-heading",
                                    Label = "Heading",
                                    ShowChevron = true,
                                    SubMenuItems =
                                    {
                                        new RibbonMenuItem { Id = "style-heading-document", Label = "This Document", Command = new RelayCommand(_ => { }) },
                                    },
                                },
                            },
                        },
                    },
                },
            },
        });

        var window = new Window
        {
            Width = 1000,
            Height = 700,
            Content = ribbon,
        };

        window.Show();
        ribbon.SelectedTabId = "home";
        window.UpdateLayout();

        var gallery = ribbon.MergedTabs
            .Single()
            .MergedGroups
            .Single()
            .MergedItems
            .Single();

        gallery.IsDropDownOpen = true;
        window.UpdateLayout();
        Assert.True(gallery.IsGalleryDropDownOpen);
        Assert.False(gallery.IsMenuDropDownOpen);
        Assert.False(gallery.IsSplitDropDownOpen);

        var normal = gallery.MenuItems.Single(menuItem => menuItem.Id == "style-normal");
        var heading = gallery.MenuItems.Single(menuItem => menuItem.Id == "style-heading");

        normal.IsSubMenuOpen = true;
        window.UpdateLayout();
        Assert.True(normal.IsSubMenuOpen);
        Assert.False(heading.IsSubMenuOpen);

        heading.IsSubMenuOpen = true;
        window.UpdateLayout();
        Assert.False(normal.IsSubMenuOpen);
        Assert.True(heading.IsSubMenuOpen);

        gallery.IsDropDownOpen = false;
        window.UpdateLayout();
        Assert.False(gallery.IsGalleryDropDownOpen);
        Assert.False(normal.IsSubMenuOpen);
        Assert.False(heading.IsSubMenuOpen);
    }

    [AvaloniaFact]
    public void PrimitiveItems_RenderSplitMenuAndGalleryTemplates()
    {
        EnsureCoreThemeLoaded();

        var ribbon = new Ribbon
        {
            SelectedTabId = "home",
        };

        ribbon.Tabs.Add(new RibbonTab
        {
            Id = "home",
            Header = "Home",
            Groups =
            {
                new RibbonGroup
                {
                    Id = "clipboard",
                    Header = "Clipboard",
                    Items =
                    {
                        new RibbonItem
                        {
                            Id = "paste",
                            Label = "Paste",
                            Primitive = RibbonItemPrimitive.SplitButton,
                            Command = new RelayCommand(_ => { }),
                            SecondaryCommand = new RelayCommand(_ => { }),
                        },
                        new RibbonItem
                        {
                            Id = "cell-shading",
                            Label = "Cell Shading",
                            Primitive = RibbonItemPrimitive.SplitButton,
                            SplitButtonMode = RibbonSplitButtonMode.Stacked,
                            Command = new RelayCommand(_ => { }),
                            SecondaryCommand = new RelayCommand(_ => { }),
                            MenuItems =
                            {
                                new RibbonMenuItem
                                {
                                    Id = "shade-no-color",
                                    Label = "No Color",
                                    Command = new RelayCommand(_ => { }),
                                    ShowInRibbonPreview = false,
                                    ShowInPopup = true,
                                },
                            },
                        },
                        new RibbonItem
                        {
                            Id = "table",
                            Label = "Table",
                            Primitive = RibbonItemPrimitive.MenuButton,
                            MenuItems =
                            {
                                new RibbonMenuItem
                                {
                                    Id = "table-2x2",
                                    Label = "2 x 2 Table",
                                    Command = new RelayCommand(_ => { }),
                                },
                            },
                        },
                        new RibbonItem
                        {
                            Id = "font-bold",
                            Label = "Bold",
                            Primitive = RibbonItemPrimitive.Button,
                            Size = RibbonItemSize.Small,
                            Command = new RelayCommand(_ => { }),
                        },
                        new RibbonItem
                        {
                            Id = "style-gallery",
                            Label = "Style Gallery",
                            Primitive = RibbonItemPrimitive.Gallery,
                            MenuItems =
                            {
                                new RibbonMenuItem
                                {
                                    Id = "style-normal",
                                    Label = "Normal",
                                    Command = new RelayCommand(_ => { }),
                                },
                            },
                        },
                        new RibbonItem
                        {
                            Id = "font-controls",
                            Label = "Font Controls",
                            Primitive = RibbonItemPrimitive.Custom,
                            Content = new TextBlock
                            {
                                Text = "Font controls host",
                            },
                        },
                    },
                },
            },
        });

        var window = new Window
        {
            Width = 1000,
            Height = 700,
            Content = ribbon,
        };

        window.Show();
        ribbon.SelectedTabId = "home";
        window.UpdateLayout();

        var tab = Assert.Single(ribbon.MergedTabs);
        var group = Assert.Single(tab.MergedGroups);
        Assert.Equal(6, group.MergedItems.Count);

        var splitItem = group.MergedItems.Single(item => item.Id == "paste");
        Assert.Equal(RibbonItemPrimitive.SplitButton, splitItem.Primitive);
        Assert.NotNull(splitItem.Command);
        Assert.NotNull(splitItem.SecondaryCommand);

        var stackedSplitItem = group.MergedItems.Single(item => item.Id == "cell-shading");
        Assert.Equal(RibbonItemPrimitive.SplitButton, stackedSplitItem.Primitive);
        Assert.Equal(RibbonSplitButtonMode.Stacked, stackedSplitItem.SplitButtonMode);
        Assert.True(stackedSplitItem.IsStackedSplitButtonPrimitive);
        Assert.True(stackedSplitItem.IsStackedSplitBottomSplitVisible);

        var menuItem = group.MergedItems.Single(item => item.Id == "table");
        Assert.Equal(RibbonItemPrimitive.MenuButton, menuItem.Primitive);
        Assert.Single(menuItem.MenuItems);
        Assert.Equal("table-2x2", menuItem.MenuItems[0].Id);

        var galleryItem = group.MergedItems.Single(item => item.Id == "style-gallery");
        Assert.Equal(RibbonItemPrimitive.Gallery, galleryItem.Primitive);
        Assert.Single(galleryItem.MenuItems);
        Assert.Equal("style-normal", galleryItem.MenuItems[0].Id);

        var smallItem = group.MergedItems.Single(item => item.Id == "font-bold");
        Assert.Equal(RibbonItemPrimitive.Button, smallItem.Primitive);
        Assert.Equal(RibbonItemSize.Small, smallItem.Size);

        var customItem = group.MergedItems.Single(item => item.Id == "font-controls");
        Assert.Equal(RibbonItemPrimitive.Custom, customItem.Primitive);
        Assert.NotNull(customItem.Content);
    }

    [AvaloniaFact]
    public void AdaptiveLayout_ResizingRibbon_CompactsAndRestoresGroups()
    {
        EnsureCoreThemeLoaded();

        var ribbon = new Ribbon
        {
            SelectedTabId = "home",
        };

        ribbon.Tabs.Add(new RibbonTab
        {
            Id = "home",
            Header = "Home",
            Groups =
            {
                new RibbonGroup
                {
                    Id = "clipboard",
                    Header = "Clipboard",
                    Order = 0,
                    Items =
                    {
                        new RibbonItem { Id = "paste", Label = "Paste", Primitive = RibbonItemPrimitive.Button, Size = RibbonItemSize.Large },
                    },
                },
                new RibbonGroup
                {
                    Id = "font",
                    Header = "Font",
                    Order = 1,
                    Items =
                    {
                        new RibbonItem { Id = "bold", Label = "Bold", Primitive = RibbonItemPrimitive.Button, Size = RibbonItemSize.Large },
                    },
                },
                new RibbonGroup
                {
                    Id = "styles",
                    Header = "Styles",
                    Order = 2,
                    Items =
                    {
                        new RibbonItem { Id = "normal", Label = "Normal", Primitive = RibbonItemPrimitive.Button, Size = RibbonItemSize.Large },
                    },
                },
            },
        });

        var window = new Window
        {
            Width = 1200,
            Height = 700,
            Content = ribbon,
        };

        window.Show();
        window.UpdateLayout();

        var groups = ribbon.MergedTabs.Single().MergedGroups;
        Assert.All(groups, group => Assert.Equal(RibbonGroupDisplayMode.Expanded, group.DisplayMode));

        ribbon.Width = 300;
        window.UpdateLayout();

        Assert.Contains(groups, group => group.DisplayMode != RibbonGroupDisplayMode.Expanded);

        ribbon.Width = 1200;
        window.UpdateLayout();

        Assert.All(groups, group => Assert.Equal(RibbonGroupDisplayMode.Expanded, group.DisplayMode));
    }

    [AvaloniaFact]
    public void StableHeightAndSynchronizedCommandHeights_PreventRibbonHeightJump()
    {
        EnsureCoreThemeLoaded();

        var syncLargeProbe = new Button { Content = "SyncLargeProbe" };
        syncLargeProbe.Classes.Add("ribbon-command-large");
        var syncSmallProbe = new Button { Content = "SyncSmallProbe" };
        syncSmallProbe.Classes.Add("ribbon-command-small");

        var ribbon = new Ribbon
        {
            SelectedTabId = "home",
            MaintainStableRibbonHeight = true,
            StableRibbonMinHeight = 120,
            SynchronizeCommandHeights = true,
            AutoSynchronizeCommandHeights = true,
            SynchronizedLargeCommandHeight = 64,
            SynchronizedSmallCommandHeight = 30,
            EnableAdaptiveLayout = false,
            HeaderEndContent = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 4,
                Children =
                {
                    syncLargeProbe,
                    syncSmallProbe,
                },
            },
        };

        ribbon.Tabs.Add(new RibbonTab
        {
            Id = "home",
            Header = "Home",
            Order = 0,
            Groups =
            {
                new RibbonGroup
                {
                    Id = "clipboard",
                    Header = "Clipboard",
                    Order = 0,
                    Items =
                    {
                        new RibbonItem { Id = "paste", Label = "Paste", Primitive = RibbonItemPrimitive.Button, Size = RibbonItemSize.Large },
                        new RibbonItem { Id = "cut", Label = "Cut", Primitive = RibbonItemPrimitive.Button, Size = RibbonItemSize.Small },
                    },
                },
            },
        });

        ribbon.Tabs.Add(new RibbonTab
        {
            Id = "insert",
            Header = "Insert",
            Order = 1,
            Groups =
            {
                new RibbonGroup
                {
                    Id = "gallery",
                    Header = "Gallery",
                    Order = 0,
                    Items =
                    {
                        new RibbonItem { Id = "styles", Label = "Styles", Primitive = RibbonItemPrimitive.Gallery },
                    },
                },
            },
        });

        var window = new Window
        {
            Width = 1200,
            Height = 700,
            Content = ribbon,
        };

        window.Show();
        window.UpdateLayout();

        var tabControl = ribbon.GetVisualDescendants().OfType<TabControl>().Single();
        var initialHeight = tabControl.Bounds.Height;
        Assert.True(initialHeight >= 120);
        Assert.True(ribbon.EffectiveSynchronizedLargeCommandHeight >= 64);
        Assert.True(ribbon.EffectiveSynchronizedSmallCommandHeight >= 30);
        Assert.True(syncLargeProbe.MinHeight >= 63);
        Assert.True(syncSmallProbe.MinHeight >= 29);

        ribbon.SelectedTabId = "insert";
        window.UpdateLayout();
        var switchedHeight = tabControl.Bounds.Height;

        ribbon.SelectedTabId = "home";
        window.UpdateLayout();
        var restoredHeight = tabControl.Bounds.Height;

        Assert.True(restoredHeight >= switchedHeight - 1);
    }

    [AvaloniaFact]
    public void AutoSynchronizedCommandHeights_PropagateMeasuredLargeHeight()
    {
        EnsureCoreThemeLoaded();

        var seedLargeProbe = new Button
        {
            Content = "SeedLargeProbe",
            Height = 88,
            MinHeight = 88,
        };
        seedLargeProbe.Classes.Add("ribbon-command-large");

        var followerLargeProbe = new Button { Content = "FollowerLargeProbe" };
        followerLargeProbe.Classes.Add("ribbon-command-large");

        var ribbon = new Ribbon
        {
            SelectedTabId = "home",
            SynchronizeCommandHeights = true,
            AutoSynchronizeCommandHeights = true,
            SynchronizedLargeCommandHeight = 0,
            SynchronizedSmallCommandHeight = 0,
            EnableAdaptiveLayout = false,
            HeaderEndContent = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 4,
                Children =
                {
                    seedLargeProbe,
                    followerLargeProbe,
                },
            },
        };

        ribbon.Tabs.Add(new RibbonTab
        {
            Id = "home",
            Header = "Home",
            Groups =
            {
                new RibbonGroup
                {
                    Id = "clipboard",
                    Header = "Clipboard",
                    Items =
                    {
                        new RibbonItem { Id = "paste", Label = "Paste", Primitive = RibbonItemPrimitive.Button, Size = RibbonItemSize.Large },
                        new RibbonItem { Id = "copy", Label = "Copy", Primitive = RibbonItemPrimitive.Button, Size = RibbonItemSize.Large },
                        new RibbonItem { Id = "cut", Label = "Cut", Primitive = RibbonItemPrimitive.Button, Size = RibbonItemSize.Small },
                    },
                },
            },
        });

        var window = new Window
        {
            Width = 1200,
            Height = 700,
            Content = ribbon,
        };

        window.Show();
        window.UpdateLayout();
        Assert.True(followerLargeProbe.MinHeight >= 87);
        Assert.True(ribbon.EffectiveSynchronizedLargeCommandHeight >= 87);
    }

    [AvaloniaFact]
    public void SynchronizedCommandHeights_SplitMainSmallTargetsSmallHeight()
    {
        EnsureCoreThemeLoaded();

        var splitMainSmallProbe = new Button { Content = "SplitMainSmallProbe" };
        splitMainSmallProbe.Classes.Add("ribbon-split-main");
        splitMainSmallProbe.Classes.Add("ribbon-split-main-small");

        var ribbon = new Ribbon
        {
            SelectedTabId = "home",
            SynchronizeCommandHeights = true,
            AutoSynchronizeCommandHeights = true,
            SynchronizedLargeCommandHeight = 66,
            SynchronizedSmallCommandHeight = 30,
            EnableAdaptiveLayout = false,
            HeaderEndContent = splitMainSmallProbe,
        };

        ribbon.Tabs.Add(new RibbonTab
        {
            Id = "home",
            Header = "Home",
            Groups =
            {
                new RibbonGroup
                {
                    Id = "paragraph",
                    Header = "Paragraph",
                    Items =
                    {
                        new RibbonItem { Id = "align-left", Label = "Align Left", Primitive = RibbonItemPrimitive.Button, Size = RibbonItemSize.Small },
                    },
                },
            },
        });

        var window = new Window
        {
            Width = 1200,
            Height = 700,
            Content = ribbon,
        };

        window.Show();
        window.UpdateLayout();

        Assert.True(splitMainSmallProbe.MinHeight <= ribbon.EffectiveSynchronizedSmallCommandHeight + 1);
        Assert.True(splitMainSmallProbe.MinHeight < ribbon.EffectiveSynchronizedLargeCommandHeight - 10);
    }

    [AvaloniaFact]
    public void SplitButtonDropdown_ModeSpecificOpenFlags_AreMutuallyExclusive()
    {
        EnsureCoreThemeLoaded();

        var ribbon = new Ribbon
        {
            SelectedTabId = "home",
            EnableAdaptiveLayout = false,
        };

        ribbon.Tabs.Add(new RibbonTab
        {
            Id = "home",
            Header = "Home",
            Groups =
            {
                new RibbonGroup
                {
                    Id = "paragraph",
                    Header = "Paragraph",
                    Items =
                    {
                        new RibbonItem
                        {
                            Id = "select",
                            Label = "Select",
                            Primitive = RibbonItemPrimitive.SplitButton,
                            SplitButtonMode = RibbonSplitButtonMode.SideBySide,
                            Size = RibbonItemSize.Small,
                            DisplayMode = RibbonItemDisplayMode.IconOnly,
                            CommandId = "select",
                            SecondaryCommandId = "select",
                            PopupTitle = "Select",
                            MenuItems =
                            {
                                new RibbonMenuItem
                                {
                                    Id = "select-all",
                                    Label = "Select All",
                                    CommandId = "select-all",
                                    Order = 0,
                                },
                            },
                        },
                    },
                },
            },
        });

        var window = new Window
        {
            Width = 1200,
            Height = 700,
            Content = ribbon,
        };

        window.Show();
        window.UpdateLayout();

        var mergedSplitItem = ribbon.MergedTabs.Single()
            .MergedGroups.Single()
            .MergedItems.Single(item => item.Id == "select");

        mergedSplitItem.IsSplitDropDownOpen = true;
        Assert.True(mergedSplitItem.IsSideBySideSplitDropDownOpen);
        Assert.False(mergedSplitItem.IsStackedSplitDropDownOpen);

        mergedSplitItem.SplitButtonMode = RibbonSplitButtonMode.Stacked;
        Assert.False(mergedSplitItem.IsSideBySideSplitDropDownOpen);
        Assert.True(mergedSplitItem.IsStackedSplitDropDownOpen);
    }

    private static void EnsureCoreThemeLoaded()
    {
        if (Application.Current is null)
        {
            return;
        }

        var uri = new Uri("avares://RibbonControl.Core/Themes/Generic.axaml");
        var alreadyLoaded = Application.Current.Styles
            .OfType<StyleInclude>()
            .Any(x => x.Source == uri);

        if (alreadyLoaded)
        {
            return;
        }

        Application.Current.Styles.Add(new StyleInclude(uri)
        {
            Source = uri,
        });
    }

    private static bool IsVisualOrDescendantOf(Visual source, Visual ancestor)
    {
        for (var current = source; current is not null; current = current.GetVisualParent())
        {
            if (ReferenceEquals(current, ancestor))
            {
                return true;
            }
        }

        return false;
    }

    private sealed class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;

        public RelayCommand(Action<object?> execute)
        {
            _execute = execute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { }
            remove { }
        }

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            _execute(parameter);
        }
    }
}
