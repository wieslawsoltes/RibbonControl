// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia.Media;
using RibbonControl.Core.Contracts;
using RibbonControl.Core.Enums;
using RibbonControl.Core.Icons;
using RibbonControl.Core.Models;
using RibbonControl.Core.Services;
using RibbonControl.Core.ViewModels;

namespace RibbonControl.Samples.Hybrid.ViewModels;

public class MainWindowViewModel : RibbonObservableObject
{
    private static readonly IReadOnlyList<string> WordCommandIds =
    [
        "paste",
        "copy",
        "bold",
        "italic",
        "underline",
        "strikethrough",
        "superscript",
        "font-grow",
        "font-shrink",
        "font-family-aptos",
        "font-family-calibri",
        "font-family-segoe",
        "font-size-10",
        "font-size-11",
        "font-size-12",
        "font-size-14",
        "para-bullets",
        "para-align-left",
        "style-normal",
        "style-heading1",
        "find",
        "replace",
        "cover-page",
        "blank-page",
        "table",
        "quick-tables",
        "picture",
        "shapes",
        "chart",
        "hyperlink",
        "bookmark",
        "header",
        "footer",
        "margins",
        "orientation",
        "size",
        "indent-left",
        "spacing",
        "position",
        "wrap-text",
        "table-contents",
        "insert-footnote",
        "insert-caption",
        "mark-entry",
        "envelopes",
        "labels",
        "start-merge",
        "select-recipients",
        "finish-merge",
        "spelling",
        "thesaurus",
        "translate",
        "new-comment",
        "track-changes",
        "read-mode",
        "print-layout",
        "web-layout",
        "ruler",
        "navigation-pane",
        "zoom",
        "new-window",
        "side-by-side",
        "help",
        "feedback",
        "remove-background",
        "corrections",
        "color",
        "picture-border",
        "picture-effects",
        "bring-forward",
        "send-backward",
        "addin-sync",
        "addin-validate",
        "save",
        "undo",
        "redo",
        "open",
        "save-as",
        "export",
        "cut",
        "format-painter",
        "paste-text-only",
        "paste-merge-formatting",
        "font-color",
        "font-color-more",
        "styles-more",
        "style-create-new",
        "style-clear-formatting",
        "style-manage-default",
        "symbol",
        "symbol-more",
        "emoji",
        "dictate",
        "select",
        "replace-advanced",
        "quick-template-todo",
        "quick-template-notes",
        "quick-template-weekly",
        "page-borders",
        "page-color",
        "border-all",
        "border-top",
        "border-bottom",
        "border-left",
        "border-right",
        "border-weight-050",
        "border-weight-100",
        "border-style-solid",
        "border-style-dashed",
        "border-color-black",
        "border-color-blue",
        "border-apply-document",
        "border-apply-section",
        "border-remove-page",
        "table-style-option-header-row",
        "table-style-option-first-column",
        "table-style-option-total-row",
        "table-style-option-last-column",
        "table-style-option-banded-rows",
        "table-style-option-banded-columns",
        "table-border-scope-all",
        "table-border-scope-inside",
        "table-border-scope-outside",
        "style-manage-default-document",
        "style-manage-default-all",
        "hybrid-static-template",
        "hybrid-static-template-letter",
        "hybrid-static-template-report",
        "hybrid-static-template-notes",
    ];

    private string _status = "Ready.";
    private IReadOnlyList<string> _activeContextGroupIds = [];
    private string? _selectedTabId = "home";
    private bool _isMinimized;
    private bool _isKeyTipMode;
    private bool _isBackstageOpen;
    private RibbonQuickAccessPlacement _quickAccessPlacement = RibbonQuickAccessPlacement.Above;
    private RibbonStateOwnershipMode _stateOwnershipMode = RibbonStateOwnershipMode.Synchronized;

    public MainWindowViewModel()
    {
        var catalog = new DictionaryRibbonCommandCatalog();
        foreach (var commandId in WordCommandIds)
        {
            catalog.Register(commandId, new RelayCommand(_ => Status = $"{ToDisplay(commandId)} executed"));
        }

        CommandCatalog = catalog;
        StateStore = new InMemoryRibbonStateStore();
        StateOwnershipModes = Enum.GetValues<RibbonStateOwnershipMode>();
        QuickAccessPlacements = Enum.GetValues<RibbonQuickAccessPlacement>();

        QuickAccessItems =
        [
            new RibbonItem
            {
                Id = "qat-save",
                Label = "Save",
                IconPathData = FluentIconData.DocumentSave20Regular,
                CommandId = "save",
                KeyTip = "S",
                ScreenTip = "Save document",
                Order = 0,
            },
            new RibbonItem
            {
                Id = "qat-undo",
                Label = "Undo",
                IconPathData = FluentIconData.ArrowUndo20Regular,
                CommandId = "undo",
                KeyTip = "U",
                ScreenTip = "Undo last action",
                Order = 1,
            },
            new RibbonItem
            {
                Id = "qat-open",
                Label = "Open",
                IconPathData = FluentIconData.FolderOpen20Regular,
                CommandId = "open",
                KeyTip = "O",
                ScreenTip = "Open document",
                Order = 2,
            },
        ];

        DynamicTabs =
        [
            BuildHomeContribution(),
            BuildInsertContribution(),
            BuildLayoutContribution(),
            BuildReferencesTab(),
            BuildMailingsTab(),
            BuildReviewTab(),
            BuildViewTab(),
            BuildHelpTab(),
            BuildPictureTab(),
        ];
        ApplyIconCustomizationSamples(DynamicTabs);

        TogglePictureToolsCommand = new RelayCommand(_ => TogglePictureTools());
        ToggleAddInsTabCommand = new RelayCommand(_ => ToggleAddInsTab());
        ToggleMinimizedCommand = new RelayCommand(_ => IsMinimized = !IsMinimized);
        ToggleKeyTipModeCommand = new RelayCommand(_ => IsKeyTipMode = !IsKeyTipMode);
        ToggleQuickAccessPlacementCommand = new RelayCommand(_ => ToggleQuickAccessPlacement());
        SeedStateStoreCommand = new RelayCommand(_ => _ = SeedStateStoreAsync());

        OpenCommand = new RelayCommand(_ => Status = "Open from Backstage executed");
        NewCommand = new RelayCommand(_ => Status = "New from Backstage executed");
        ShareCommand = new RelayCommand(_ => Status = "Share from Backstage executed");
        CreateCopyCommand = new RelayCommand(_ => Status = "Create a Copy from Backstage executed");
        SaveAsCommand = new RelayCommand(_ => Status = "Save As from Backstage executed");
        ExportCommand = new RelayCommand(_ => Status = "Export PDF from Backstage executed");
        PrintCommand = new RelayCommand(_ => Status = "Print from Backstage executed");
        RenameCommand = new RelayCommand(_ => Status = "Rename from Backstage executed");
        VersionHistoryCommand = new RelayCommand(_ => Status = "Version History from Backstage executed");
        DeleteCommand = new RelayCommand(_ => Status = "Delete from Backstage executed");
        InfoCommand = new RelayCommand(_ => Status = "Info from Backstage executed");
        CloseBackstageCommand = new RelayCommand(_ => IsBackstageOpen = false);

        BackstageItems = BuildBackstageItems();
    }

    public ObservableCollection<RibbonTabViewModel> DynamicTabs { get; }

    public IRibbonCommandCatalog CommandCatalog { get; }

    public IRibbonStateStore StateStore { get; }

    public ObservableCollection<RibbonItem> QuickAccessItems { get; }

    public ObservableCollection<RibbonBackstageItem> BackstageItems { get; }

    public IReadOnlyList<RibbonStateOwnershipMode> StateOwnershipModes { get; }

    public IReadOnlyList<RibbonQuickAccessPlacement> QuickAccessPlacements { get; }

    public ICommand TogglePictureToolsCommand { get; }

    public ICommand ToggleAddInsTabCommand { get; }

    public ICommand ToggleMinimizedCommand { get; }

    public ICommand ToggleKeyTipModeCommand { get; }

    public ICommand ToggleQuickAccessPlacementCommand { get; }

    public ICommand SeedStateStoreCommand { get; }

    public ICommand OpenCommand { get; }

    public ICommand NewCommand { get; }

    public ICommand ShareCommand { get; }

    public ICommand CreateCopyCommand { get; }

    public ICommand SaveAsCommand { get; }

    public ICommand ExportCommand { get; }

    public ICommand PrintCommand { get; }

    public ICommand RenameCommand { get; }

    public ICommand VersionHistoryCommand { get; }

    public ICommand DeleteCommand { get; }

    public ICommand InfoCommand { get; }

    public ICommand CloseBackstageCommand { get; }

    public IReadOnlyList<string> ActiveContextGroupIds
    {
        get => _activeContextGroupIds;
        private set => SetProperty(ref _activeContextGroupIds, value);
    }

    public string? SelectedTabId
    {
        get => _selectedTabId;
        set => SetProperty(ref _selectedTabId, value);
    }

    public bool IsMinimized
    {
        get => _isMinimized;
        set => SetProperty(ref _isMinimized, value);
    }

    public bool IsKeyTipMode
    {
        get => _isKeyTipMode;
        set => SetProperty(ref _isKeyTipMode, value);
    }

    public RibbonQuickAccessPlacement QuickAccessPlacement
    {
        get => _quickAccessPlacement;
        set => SetProperty(ref _quickAccessPlacement, value);
    }

    public RibbonStateOwnershipMode StateOwnershipMode
    {
        get => _stateOwnershipMode;
        set => SetProperty(ref _stateOwnershipMode, value);
    }

    public bool IsBackstageOpen
    {
        get => _isBackstageOpen;
        set => SetProperty(ref _isBackstageOpen, value);
    }

    public string Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    private ObservableCollection<RibbonBackstageItem> BuildBackstageItems()
    {
        return
        [
            new RibbonBackstageItem
            {
                Id = "backstage-new",
                Label = "New",
                IconPathData = FluentIconData.DocumentSave20Regular,
                ShowChevron = true,
                Order = 0,
                Command = NewCommand,
                ExecuteCommandOnSelect = true,
                Content = "Create a new document from templates.",
            },
            new RibbonBackstageItem
            {
                Id = "backstage-open",
                Label = "Open",
                IconPathData = FluentIconData.FolderOpen20Regular,
                ShowChevron = true,
                Order = 1,
                Command = OpenCommand,
                ExecuteCommandOnSelect = true,
                Content = "Open recent files, OneDrive files, or local documents.",
            },
            new RibbonBackstageItem
            {
                Id = "backstage-share",
                Label = "Share",
                IconPathData = FluentIconData.Share20Regular,
                ShowChevron = true,
                Order = 2,
                Command = ShareCommand,
                ExecuteCommandOnSelect = true,
                Content = "Invite people and manage sharing permissions.",
            },
            new RibbonBackstageItem
            {
                Id = "backstage-copy",
                Label = "Create a Copy",
                IconPathData = FluentIconData.Copy20Regular,
                ShowChevron = true,
                Order = 3,
                Command = CreateCopyCommand,
                ExecuteCommandOnSelect = true,
                Content = "Create a copy in another location or cloud account.",
            },
            new RibbonBackstageItem
            {
                Id = "backstage-export",
                Label = "Export",
                IconPathData = FluentIconData.ArrowSync20Regular,
                ShowChevron = true,
                Order = 4,
                Command = ExportCommand,
                ExecuteCommandOnSelect = true,
                Content = "Export to PDF, web page, or other formats.",
            },
            new RibbonBackstageItem
            {
                Id = "backstage-print",
                Label = "Print",
                IconPathData = FluentIconData.Document20Regular,
                Order = 5,
                Command = PrintCommand,
                ExecuteCommandOnSelect = true,
                Content = "Open print preview and printer settings.",
            },
            new RibbonBackstageItem
            {
                Id = "backstage-separator-main",
                IsSeparator = true,
                Order = 6,
            },
            new RibbonBackstageItem
            {
                Id = "backstage-rename",
                Label = "Rename",
                IconPathData = FluentIconData.Wrench20Regular,
                Order = 7,
                Command = RenameCommand,
                ExecuteCommandOnSelect = true,
                Content = "Rename this file and update cloud references.",
            },
            new RibbonBackstageItem
            {
                Id = "backstage-move",
                Label = "Move File",
                IconPathData = FluentIconData.FolderOpen20Regular,
                Order = 8,
                IsEnabled = false,
                Content = "File movement is unavailable in this sample.",
            },
            new RibbonBackstageItem
            {
                Id = "backstage-history",
                Label = "Version History",
                IconPathData = FluentIconData.ArrowUndo20Regular,
                Order = 9,
                Command = VersionHistoryCommand,
                ExecuteCommandOnSelect = true,
                Content = "Review and restore previous document versions.",
            },
            new RibbonBackstageItem
            {
                Id = "backstage-delete",
                Label = "Delete",
                IconPathData = FluentIconData.Settings20Regular,
                Order = 10,
                Command = DeleteCommand,
                ExecuteCommandOnSelect = true,
                Content = "Move this file to the recycle bin.",
            },
            new RibbonBackstageItem
            {
                Id = "backstage-info",
                Label = "Info",
                IconPathData = FluentIconData.Comment20Regular,
                ShowChevron = true,
                Order = 11,
                Command = InfoCommand,
                ExecuteCommandOnSelect = true,
                Content = "Inspect document metadata, protection, and permissions.",
            },
            new RibbonBackstageItem
            {
                Id = "backstage-separator-close",
                IsSeparator = true,
                Order = 12,
            },
            new RibbonBackstageItem
            {
                Id = "backstage-close",
                Label = "Close Backstage",
                IconPathData = FluentIconData.ArrowSync20Regular,
                Order = 13,
                Command = CloseBackstageCommand,
                ExecuteCommandOnSelect = true,
                CloseBackstageOnExecute = true,
            },
        ];
    }

    private void TogglePictureTools()
    {
        if (ActiveContextGroupIds.Contains("picture-tools", StringComparer.Ordinal))
        {
            ActiveContextGroupIds = [];
            Status = "Picture Tools hidden";
            return;
        }

        ActiveContextGroupIds = ["picture-tools"];
        Status = "Picture Tools shown";
    }

    private void ToggleAddInsTab()
    {
        var existing = DynamicTabs.FirstOrDefault(x => string.Equals(x.Id, "add-ins", StringComparison.Ordinal));
        if (existing is not null)
        {
            DynamicTabs.Remove(existing);
            Status = "Add-ins dynamic tab removed";
            return;
        }

        DynamicTabs.Add(BuildAddInsTab());
        Status = "Add-ins dynamic tab added";
    }

    private void ToggleQuickAccessPlacement()
    {
        QuickAccessPlacement = QuickAccessPlacement == RibbonQuickAccessPlacement.Above
            ? RibbonQuickAccessPlacement.Below
            : RibbonQuickAccessPlacement.Above;
    }

    private async Task SeedStateStoreAsync()
    {
        await StateStore.SaveAsync(new RibbonRuntimeState
        {
            SelectedTabId = "review",
            IsMinimized = true,
            QuickAccessPlacement = RibbonQuickAccessPlacement.Below,
            ActiveContextGroupIds = ["picture-tools"],
            NodeCustomizations =
            {
                new RibbonNodeCustomization { Id = "style-heading1", ParentId = "styles", IsHidden = true },
            },
        });

        Status = "Seeded hybrid Word-like state profile. Run Load State to apply it.";
    }

    private static RibbonTabViewModel BuildHomeContribution()
        => Tab(
            "home",
            "Home",
            0,
            Group(
                "history",
                "Undo",
                -1,
                RibbonGroupHeaderPlacement.Bottom,
                RibbonGroupItemsLayoutMode.Stacked,
                stackedRows: 2,
                Item(
                    "undo",
                    "Undo",
                    FluentIconData.ArrowUndo20Regular,
                    0,
                    "undo",
                    "Z",
                    "Undo last action",
                    size: RibbonItemSize.Small,
                    displayMode: RibbonItemDisplayMode.IconOnly),
                Item(
                    "redo",
                    "Redo",
                    FluentIconData.ArrowSync20Regular,
                    1,
                    "redo",
                    "Y",
                    "Redo last action",
                    size: RibbonItemSize.Small,
                    displayMode: RibbonItemDisplayMode.IconOnly)),
            Group("clipboard", "Clipboard", 0,
                PasteSplitItem(
                    "paste",
                    "Paste",
                    FluentIconData.ClipboardPaste20Regular,
                    0,
                    "paste",
                    "V",
                    "Paste from clipboard (dynamic metadata path)",
                    MenuEntryDetailed(
                        "paste-default",
                        "Paste",
                        0,
                        "paste",
                        FluentIconData.ClipboardPaste20Regular,
                        "Keep source formatting",
                        "Cmd+V"),
                    MenuEntryDetailed(
                        "paste-text-only",
                        "Paste Text Only",
                        1,
                        "paste-text-only",
                        FluentIconData.Copy20Regular,
                        "Keep text and discard formatting",
                        "Cmd+Shift+V"),
                    MenuEntryDetailed(
                        "paste-merge",
                        "Paste With Merge Formatting",
                        2,
                        "paste-merge-formatting",
                        FluentIconData.ArrowSync20Regular,
                        "Match destination formatting",
                        "Option+V")),
                Item("cut", "Cut", FluentIconData.Settings20Regular, 1, "cut", "X", "Cut selection", size: RibbonItemSize.Small),
                Item("format-painter", "Format Painter", FluentIconData.Wrench20Regular, 2, "format-painter", "FP", "Copy formatting", size: RibbonItemSize.Small)),
            Group("paragraph", "Paragraph", 2, RibbonGroupHeaderPlacement.Bottom, RibbonGroupItemsLayoutMode.Stacked, 2,
                CompactSplitItemWithPopup(
                    "para-bullets",
                    "Bullets",
                    FluentIconData.TextBulletList20Regular,
                    0,
                    "para-bullets",
                    "para-bullets",
                    "BU",
                    "Apply bullet list",
                    "Bullet Library",
                    320,
                    360,
                    MenuSeparator("para-bullets-separator", 80, showInPopup: true),
                    MenuEntryDetailed("para-bullets-define", "Define New Bullet", 81, "para-bullets", showInRibbonPreview: false),
                    MenuEntryDetailed("para-bullets-none", "None", 82, "para-bullets", showInRibbonPreview: false)),
                Item("para-align-left", "Align Left", FluentIconData.Copy20Regular, 1, "para-align-left", "AL", "Align paragraph left", size: RibbonItemSize.Small, displayMode: RibbonItemDisplayMode.IconOnly, isToggle: true),
                CompactSplitItem(
                    "para-numbering",
                    "Numbering",
                    FluentIconData.TextNumberList20Regular,
                    2,
                    "spacing",
                    "spacing",
                    "NU",
                    "Apply numbering",
                    MenuEntryDetailed("para-numbering-decimal", "1. 2. 3.", 0, "spacing", showInRibbonPreview: false),
                    MenuEntryDetailed("para-numbering-alpha", "a. b. c.", 1, "spacing", showInRibbonPreview: false),
                    MenuEntryDetailed("para-numbering-roman", "i. ii. iii.", 2, "spacing", showInRibbonPreview: false)),
                Item("para-align-center", "Align Center", FluentIconData.TextBold20Regular, 3, "spacing", "AC", "Center align paragraph", size: RibbonItemSize.Small, displayMode: RibbonItemDisplayMode.IconOnly, isToggle: true),
                CompactSplitItem(
                    "para-multilevel",
                    "Multilevel List",
                    FluentIconData.TextNumberList20Regular,
                    4,
                    "spacing",
                    "spacing",
                    "ML",
                    "Apply multilevel list",
                    MenuEntryDetailed("para-multilevel-1", "1. 1.1. 1.1.1", 0, "spacing", showInRibbonPreview: false),
                    MenuEntryDetailed("para-multilevel-a", "A. I. a.", 1, "spacing", showInRibbonPreview: false),
                    MenuEntryDetailed("para-multilevel-custom", "Define New Multilevel List", 2, "spacing", showInRibbonPreview: false)),
                Item("para-align-right", "Align Right", FluentIconData.TextItalic20Regular, 5, "spacing", "AR", "Right align paragraph", size: RibbonItemSize.Small, displayMode: RibbonItemDisplayMode.IconOnly, isToggle: true),
                CompactSplitItem(
                    "para-indent-decrease",
                    "Decrease Indent",
                    FluentIconData.TextIndentDecrease20Regular,
                    6,
                    "indent-left",
                    "indent-left",
                    "DI",
                    "Decrease indent",
                    MenuEntryDetailed("para-indent-decrease-options", "Indent Options", 0, "indent-left", showInRibbonPreview: false)),
                Item("para-align-justify", "Justify", FluentIconData.ChartMultiple20Regular, 7, "spacing", "AJ", "Justify paragraph", size: RibbonItemSize.Small, displayMode: RibbonItemDisplayMode.IconOnly, isToggle: true),
                CompactSplitItem(
                    "para-indent-increase",
                    "Increase Indent",
                    FluentIconData.TextIndentIncrease20Regular,
                    8,
                    "spacing",
                    "spacing",
                    "II",
                    "Increase indent",
                    MenuEntryDetailed("para-indent-increase-options", "Indent Options", 0, "spacing", showInRibbonPreview: false)),
                CompactSplitItem(
                    "para-line-spacing",
                    "Line Spacing",
                    FluentIconData.Document20Regular,
                    9,
                    "spacing",
                    "spacing",
                    "LS",
                    "Set line and paragraph spacing",
                    MenuEntryDetailed("para-line-spacing-100", "1.0", 0, "spacing", showInRibbonPreview: false),
                    MenuEntryDetailed("para-line-spacing-115", "1.15", 1, "spacing", showInRibbonPreview: false),
                    MenuEntryDetailed("para-line-spacing-150", "1.5", 2, "spacing", showInRibbonPreview: false),
                    MenuEntryDetailed("para-line-spacing-200", "2.0", 3, "spacing", showInRibbonPreview: false)),
                Item("para-show-marks", "Show Marks", FluentIconData.TextItalic20Regular, 10, "spacing", "PM", "Show or hide paragraph marks", size: RibbonItemSize.Small, displayMode: RibbonItemDisplayMode.IconOnly, isToggle: true),
                CompactSplitItemWithPopup(
                    "para-shading",
                    "Shading",
                    FluentIconData.Wrench20Regular,
                    11,
                    "spacing",
                    "spacing",
                    "SD",
                    "Apply paragraph shading",
                    "Shading",
                    260,
                    320,
                    MenuEntryDetailed("para-shading-none", "No Color", 0, "spacing", showInRibbonPreview: false),
                    MenuEntryDetailed("para-shading-theme", "Theme Colors", 1, "spacing", showInRibbonPreview: false),
                    MenuEntryDetailed("para-shading-more", "More Colors", 2, "spacing", showInRibbonPreview: false)),
                Item("para-line-numbers", "Line Numbers", FluentIconData.TextNumberList20Regular, 12, "spacing", "LN", "Adjust line numbering", size: RibbonItemSize.Small, displayMode: RibbonItemDisplayMode.IconOnly, isToggle: true),
                CompactSplitItem(
                    "para-borders",
                    "Borders",
                    FluentIconData.Grid20Regular,
                    13,
                    "page-borders",
                    "page-borders",
                    "BR",
                    "Configure borders",
                    MenuEntryDetailed("para-borders-all", "All Borders", 0, "border-all", showInRibbonPreview: false),
                    MenuEntryDetailed("para-borders-top", "Top Border", 1, "border-top", showInRibbonPreview: false),
                    MenuEntryDetailed("para-borders-bottom", "Bottom Border", 2, "border-bottom", showInRibbonPreview: false),
                    MenuEntryDetailed("para-borders-left", "Left Border", 3, "border-left", showInRibbonPreview: false),
                    MenuEntryDetailed("para-borders-right", "Right Border", 4, "border-right", showInRibbonPreview: false))),
            Group("styles", "Styles", 3, RibbonGroupHeaderPlacement.Right,
                GalleryItem("style-gallery", "Styles", FluentIconData.DocumentSave20Regular, 0, "SG", "Pick a style preset", "Quick style presets",
                    MenuEntryDetailed("style-preview-normal", "Normal", 0, "style-normal", content: "Normal\nAptos, 12", showInPopup: false, isSelected: true),
                    MenuEntryDetailed("style-preview-no-spacing", "No Spacing", 1, "styles-more", content: "No Spacing\nAptos, 12", showInPopup: false),
                    MenuEntryDetailed("style-preview-heading1", "Heading 1", 2, "style-heading1", content: "Heading 1\nAptos Display, 20", showInPopup: false),
                    MenuEntryDetailed("style-popup-normal", "Normal", 20, "style-normal", showChevron: true, showInRibbonPreview: false, category: "Quick Styles", isSelected: true),
                    MenuEntryDetailed("style-popup-no-spacing", "No Spacing", 21, "styles-more", showChevron: true, showInRibbonPreview: false, category: "Quick Styles"),
                    MenuEntryDetailed("style-popup-heading1", "Heading 1", 22, "style-heading1", showChevron: true, showInRibbonPreview: false, category: "Quick Styles"),
                    MenuEntryDetailed("style-popup-heading2", "Heading 2", 23, "styles-more", showChevron: true, showInRibbonPreview: false, category: "Quick Styles"),
                    MenuEntryDetailed("style-popup-title", "Title", 24, "styles-more", showChevron: true, showInRibbonPreview: false, category: "Quick Styles"),
                    MenuEntryDetailed("style-popup-subtitle", "Subtitle", 25, "styles-more", showChevron: true, showInRibbonPreview: false, category: "Quick Styles"),
                    MenuSeparator("style-popup-separator", 26),
                    MenuEntryDetailed("style-popup-more", "See More Styles", 27, "styles-more", FluentIconData.ArrowSync20Regular, showInRibbonPreview: false),
                    MenuEntryDetailed("style-popup-create", "Create New Style from Selection", 28, "style-create-new", FluentIconData.CheckmarkCircle20Regular, showInRibbonPreview: false),
                    MenuEntryDetailed("style-popup-clear", "Clear Formatting of Selection", 29, "style-clear-formatting", FluentIconData.Settings20Regular, showInRibbonPreview: false),
                    MenuEntryWithSubMenu("style-popup-manage", "Manage Default Styles", FluentIconData.Wrench20Regular, 30,
                        MenuEntryDetailed("style-popup-manage-document", "Set as Default in this Document", 0, "style-manage-default-document"),
                        MenuEntryDetailed("style-popup-manage-all", "Set as Default for All Documents", 1, "style-manage-default-all")))),
            Group("editing", "Editing", 4, RibbonGroupHeaderPlacement.Bottom, RibbonGroupItemsLayoutMode.Stacked, 3,
                Item("find", "Find", FluentIconData.Search20Regular, 0, "find", "FD", "Find text", size: RibbonItemSize.Small),
                Item("replace", "Replace", FluentIconData.ArrowSync20Regular, 1, "replace", "RP", "Find and replace", size: RibbonItemSize.Small),
                SmallSplitItem(
                    "select",
                    "Select",
                    FluentIconData.Person20Regular,
                    2,
                    "select",
                    "select",
                    "SL",
                    "Select content",
                    MenuEntryDetailed("select-all", "Select All", 0, "select", showInRibbonPreview: false),
                    MenuEntryDetailed("select-objects", "Select Objects", 1, "select", showInRibbonPreview: false),
                    MenuEntryDetailed("select-selection-pane", "Selection Pane", 2, "select", showInRibbonPreview: false))));

    private static RibbonTabViewModel BuildInsertContribution()
        => Tab(
            "insert",
            "Insert",
            1,
            Group("pages", "Pages", 0,
                Item("cover-page", "Cover Page", FluentIconData.DocumentSave20Regular, 0, "cover-page", "CV", "Insert a cover page"),
                Item("blank-page", "Blank Page", FluentIconData.Copy20Regular, 1, "blank-page", "BP", "Insert a blank page")),
            Group("tables", "Tables", 1,
                MenuButtonItem("table", "Table", FluentIconData.Settings20Regular, 0, "table", "TB", "Insert table",
                    MenuEntry("table-grid-2x2", "2 x 2 Table", FluentIconData.Settings20Regular, 0, "table"),
                    MenuEntry("table-grid-3x3", "3 x 3 Table", FluentIconData.Settings20Regular, 1, "quick-tables")),
                MenuButtonItemWithPopup("quick-tables", "Quick Templates", FluentIconData.ChartMultiple20Regular, 1, "quick-tables", "QT", "Insert a quick template",
                    "Quick Templates", 260, 340,
                    MenuEntryDetailed("quick-template-todo", "To-do List", 0, "quick-template-todo", FluentIconData.Document20Regular),
                    MenuEntryWithSubMenu("quick-template-notes", "Notes", FluentIconData.Wrench20Regular, 1,
                        MenuEntryDetailed("quick-template-meeting-notes", "Meeting Notes", 0, "quick-template-notes", FluentIconData.Document20Regular),
                        MenuEntryDetailed("quick-template-project-notes", "Project Notes", 1, "quick-template-notes", FluentIconData.Copy20Regular)),
                    MenuEntryDetailed("quick-template-weekly", "Weekly Plan", 2, "quick-template-weekly", FluentIconData.Settings20Regular))),
            Group("illustrations", "Illustrations", 2,
                Item("shapes", "Shapes", FluentIconData.Settings20Regular, 0, "shapes", "SP", "Insert a shape")),
            Group("links", "Links", 3,
                Item("hyperlink", "Link", FluentIconData.Share20Regular, 0, "hyperlink", "LK", "Insert hyperlink"),
                Item("bookmark", "Bookmark", FluentIconData.FolderOpen20Regular, 1, "bookmark", "BM", "Insert bookmark")),
            Group("header-footer", "Header & Footer", 4,
                Item("header", "Header", FluentIconData.Copy20Regular, 0, "header", "HD", "Insert header"),
                Item("footer", "Footer", FluentIconData.Copy20Regular, 1, "footer", "FT", "Insert footer")),
            Group("symbols", "Symbols", 5,
                SplitItem("equation", "Equation", FluentIconData.Wrench20Regular, 0, "symbol", "symbol-more", "EQ", "Insert equation",
                    MenuEntryDetailed("equation-insert", "Insert New Equation", 0, "symbol", FluentIconData.Wrench20Regular),
                    MenuEntryDetailed("equation-ink", "Ink Equation", 1, "symbol-more", FluentIconData.Document20Regular)),
                MenuButtonItem("symbol", "Symbol", FluentIconData.Settings20Regular, 1, "symbol", "SY", "Insert symbol",
                    MenuEntryDetailed("symbol-more", "More Symbols", 0, "symbol-more", FluentIconData.Settings20Regular)),
                MenuButtonItem("emoji", "Emoji", FluentIconData.Comment20Regular, 2, "emoji", "EM", "Insert emoji",
                    MenuEntryDetailed("emoji-picker", "Open Emoji Panel", 0, "emoji", inputGestureText: "Ctrl+Cmd+Space"))));

    private static RibbonTabViewModel BuildLayoutContribution()
        => Tab(
            "layout",
            "Layout",
            2,
            Group("paragraph-layout", "Paragraph", 1,
                Item("indent-left", "Indent Left", FluentIconData.Copy20Regular, 0, "indent-left", "IL", "Decrease left indent"),
                Item("spacing", "Spacing", FluentIconData.Settings20Regular, 1, "spacing", "SG", "Adjust paragraph spacing")),
            Group("arrange", "Arrange", 2,
                Item("position", "Position", FluentIconData.Wrench20Regular, 0, "position", "PS", "Adjust object position"),
                Item("wrap-text", "Wrap Text", FluentIconData.Share20Regular, 1, "wrap-text", "WT", "Change text wrapping")),
            Group("page-background", "Page Background", 3,
                MenuButtonItemWithPopup("page-borders", "Page Borders", FluentIconData.Copy20Regular, 0, "page-borders", "PB", "Configure border options",
                    "Border Options", 320, 380, null, "Esc to close",
                    MenuEntryWithSubMenu("page-border-weight", "Weight", FluentIconData.Settings20Regular, 0,
                        MenuEntryDetailed("page-border-weight-050", "0.5 pt", 0, "border-weight-050"),
                        MenuEntryDetailed("page-border-weight-100", "1.0 pt", 1, "border-weight-100")),
                    MenuEntryWithSubMenu("page-border-style", "Style", FluentIconData.Wrench20Regular, 1,
                        MenuEntryDetailed("page-border-style-solid", "Solid", 0, "border-style-solid"),
                        MenuEntryDetailed("page-border-style-dashed", "Dashed", 1, "border-style-dashed")),
                    MenuEntryWithSubMenu("page-border-color", "Color", FluentIconData.ChartMultiple20Regular, 2,
                        MenuEntryDetailed("page-border-color-black", "Black", 0, "border-color-black"),
                        MenuEntryDetailed("page-border-color-blue", "Blue", 1, "border-color-blue")),
                    MenuEntryWithSubMenu("page-border-apply-to", "Apply to", FluentIconData.Document20Regular, 3,
                        MenuEntryDetailed("page-border-apply-document", "Whole document", 0, "border-apply-document"),
                        MenuEntryDetailed("page-border-apply-section", "This section", 1, "border-apply-section")),
                    MenuSeparator("page-border-separator", 4),
                    MenuEntryDetailed("page-border-remove", "Remove Page Border", 5, "border-remove-page", FluentIconData.ArrowUndo20Regular, showInRibbonPreview: false)),
                SplitItemWithPopup("cell-shading", "Cell Shading", FluentIconData.ChartMultiple20Regular, 1, "page-color", "page-color", "CS", "Apply or pick a shading color",
                    "Cell Shading", 320, 420, null, null, RibbonSplitButtonMode.Stacked,
                    MenuEntryDetailed("page-color-no-color", "No Color", 0, "page-color"),
                    MenuEntryDetailed("page-color-blue", "Blue", 1, "border-color-blue"),
                    MenuEntryDetailed("page-color-black", "Black", 2, "border-color-black")),
                ToggleGroupItem("table-style-options", "Table Style Options", 2, RibbonItemSize.Large, RibbonToggleGroupSelectionMode.Multiple, 2, "TS", "Toggle table style options",
                    MenuEntryDetailed("table-style-option-header-row", "Header Row", 0, "table-style-option-header-row", FluentIconData.Grid20Regular, isSelected: true),
                    MenuEntryDetailed("table-style-option-first-column", "First Column", 1, "table-style-option-first-column", FluentIconData.Grid20Regular, isSelected: true),
                    MenuEntryDetailed("table-style-option-total-row", "Total Row", 2, "table-style-option-total-row", FluentIconData.Grid20Regular),
                    MenuEntryDetailed("table-style-option-last-column", "Last Column", 3, "table-style-option-last-column", FluentIconData.Grid20Regular),
                    MenuEntryDetailed("table-style-option-banded-rows", "Banded Rows", 4, "table-style-option-banded-rows", FluentIconData.Grid20Regular, isSelected: true),
                    MenuEntryDetailed("table-style-option-banded-columns", "Banded Columns", 5, "table-style-option-banded-columns", FluentIconData.Grid20Regular)),
                ToggleGroupItem("table-border-scope", "Border Scope", 3, RibbonItemSize.Small, RibbonToggleGroupSelectionMode.Single, 1, "TB", "Choose one border scope",
                    MenuEntryDetailed("table-border-scope-all", "All Borders", 0, "table-border-scope-all", FluentIconData.Settings20Regular, isSelected: true),
                    MenuEntryDetailed("table-border-scope-inside", "Inside Borders", 1, "table-border-scope-inside", FluentIconData.Settings20Regular),
                    MenuEntryDetailed("table-border-scope-outside", "Outside Borders", 2, "table-border-scope-outside", FluentIconData.Settings20Regular))));

    private static RibbonTabViewModel BuildReferencesTab()
        => Tab(
            "references",
            "References",
            3,
            Group("table-of-contents", "Table of Contents", 0,
                Item("table-contents", "Table of Contents", FluentIconData.Copy20Regular, 0, "table-contents", "TC", "Insert table of contents")),
            Group(
                "footnotes",
                "Footnotes",
                1,
                RibbonGroupHeaderPlacement.Bottom,
                RibbonGroupItemsLayoutMode.Docked,
                2,
                RibbonGroupDockedCenterLayoutMode.Stacked,
                Item("insert-footnote", "Insert Footnote", FluentIconData.TextNumberList20Regular, 0, "insert-footnote", "FN", "Insert footnote", layoutDock: RibbonItemLayoutDock.Left),
                Item("insert-endnote", "Insert Endnote", FluentIconData.Document20Regular, 1, "insert-endnote", "EN", "Insert endnote", size: RibbonItemSize.Small, layoutDock: RibbonItemLayoutDock.Center),
                Item("show-footnotes", "Show Footnotes", FluentIconData.ArrowSync20Regular, 2, "show-footnotes", "SF", "Jump to footnote reference", size: RibbonItemSize.Small, layoutDock: RibbonItemLayoutDock.Center),
                Item("show-endnotes", "Show Endnotes", FluentIconData.ArrowSync20Regular, 3, "show-endnotes", "SE", "Jump to endnote reference", size: RibbonItemSize.Small, layoutDock: RibbonItemLayoutDock.Center)),
            Group("captions", "Captions", 2,
                Item("insert-caption", "Insert Caption", FluentIconData.Image20Regular, 0, "insert-caption", "IC", "Insert caption")),
            Group("index", "Index", 3,
                Item("mark-entry", "Mark Entry", FluentIconData.Wrench20Regular, 0, "mark-entry", "ME", "Mark index entry")));

    private static RibbonTabViewModel BuildMailingsTab()
        => Tab(
            "mailings",
            "Mailings",
            4,
            Group("create", "Create", 0,
                Item("envelopes", "Envelopes", FluentIconData.Copy20Regular, 0, "envelopes", "EV", "Create envelopes"),
                Item("labels", "Labels", FluentIconData.Copy20Regular, 1, "labels", "LB", "Create labels")),
            Group("start-mail-merge", "Start Mail Merge", 1,
                Item("start-merge", "Start Merge", FluentIconData.Share20Regular, 0, "start-merge", "SM", "Start mail merge"),
                Item("select-recipients", "Recipients", FluentIconData.FolderOpen20Regular, 1, "select-recipients", "SR", "Select recipients")),
            Group("finish-mailings", "Finish", 2,
                Item("finish-merge", "Finish Merge", FluentIconData.DocumentSave20Regular, 0, "finish-merge", "FM", "Finish and merge")));

    private static RibbonTabViewModel BuildReviewTab()
        => Tab(
            "review",
            "Review",
            5,
            Group("proofing", "Proofing", 0,
                Item("spelling", "Spelling", FluentIconData.Settings20Regular, 0, "spelling", "SP", "Run spelling and grammar"),
                Item("thesaurus", "Thesaurus", FluentIconData.FolderOpen20Regular, 1, "thesaurus", "TH", "Open thesaurus")),
            Group("language", "Language", 1,
                Item("translate", "Translate", FluentIconData.Share20Regular, 0, "translate", "TR", "Translate selection")),
            Group("comments", "Comments", 2,
                Item("new-comment", "New Comment", FluentIconData.Copy20Regular, 0, "new-comment", "NC", "Insert comment")),
            Group("tracking", "Tracking", 3,
                Item("track-changes", "Track Changes", FluentIconData.Wrench20Regular, 0, "track-changes", "TC", "Track document changes")));

    private static RibbonTabViewModel BuildViewTab()
        => Tab(
            "view",
            "View",
            6,
            Group("views", "Views", 0,
                Item("read-mode", "Read Mode", FluentIconData.Copy20Regular, 0, "read-mode", "RM", "Switch to read mode"),
                Item("print-layout", "Print Layout", FluentIconData.ChartMultiple20Regular, 1, "print-layout", "PL", "Switch to print layout"),
                Item("web-layout", "Web Layout", FluentIconData.Share20Regular, 2, "web-layout", "WL", "Switch to web layout")),
            Group("show", "Show", 1,
                Item("ruler", "Ruler", FluentIconData.Settings20Regular, 0, "ruler", "RU", "Show or hide ruler", isToggle: true),
                Item("navigation-pane", "Navigation Pane", FluentIconData.FolderOpen20Regular, 1, "navigation-pane", "NP", "Toggle navigation pane", isToggle: true),
                Item("header-footer", "Header & Footer", FluentIconData.Copy20Regular, 2, "header", "HF", "Toggle header and footer markers", isToggle: true)),
            Group("zoom", "Zoom", 2,
                Item("zoom", "Zoom", FluentIconData.ChartMultiple20Regular, 0, "zoom", "ZM", "Adjust zoom")),
            Group("window", "Window", 3,
                Item("new-window", "New Window", FluentIconData.Copy20Regular, 0, "new-window", "NW", "Open in new window"),
                Item("side-by-side", "Side by Side", FluentIconData.Share20Regular, 1, "side-by-side", "SB", "View side by side")));

    private static RibbonTabViewModel BuildHelpTab()
        => Tab(
            "help",
            "Help",
            7,
            Group("help-group", "Help", 0,
                Item("help", "Help", FluentIconData.FolderOpen20Regular, 0, "help", "HP", "Open help"),
                Item("feedback", "Feedback", FluentIconData.Share20Regular, 1, "feedback", "FB", "Send feedback")));

    private static RibbonTabViewModel BuildPictureTab()
    {
        var tab = Tab(
            "picture-format",
            "Picture Format",
            20,
            Group("picture-adjust", "Adjust", 0,
                Item("remove-background", "Remove Background", FluentIconData.Settings20Regular, 0, "remove-background", "RB", "Remove image background"),
                Item("corrections", "Corrections", FluentIconData.Image20Regular, 1, "corrections", "CR", "Adjust picture corrections"),
                Item("color", "Color", FluentIconData.ChartMultiple20Regular, 2, "color", "CL", "Adjust picture color")),
            Group("picture-styles", "Picture Styles", 1,
                Item("picture-border", "Picture Border", FluentIconData.Copy20Regular, 0, "picture-border", "PB", "Add a picture border"),
                Item("picture-effects", "Picture Effects", FluentIconData.Wrench20Regular, 1, "picture-effects", "PE", "Apply picture effects")),
            Group("picture-arrange", "Arrange", 2,
                Item("bring-forward", "Bring Forward", FluentIconData.Share20Regular, 0, "bring-forward", "BF", "Bring picture forward"),
                Item("send-backward", "Send Backward", FluentIconData.Share20Regular, 1, "send-backward", "BA", "Send picture backward")));

        tab.IsContextual = true;
        tab.ContextGroupId = "picture-tools";
        tab.ContextGroupHeader = "Picture Tools";
        tab.ContextGroupAccentColor = "#0F6CBD";
        tab.ContextGroupOrder = 30;
        return tab;
    }

    private static RibbonTabViewModel BuildAddInsTab()
        => Tab(
            "add-ins",
            "Add-ins",
            8,
            Group("addin-tools", "Add-in Tools", 0,
                Item("addin-sync", "Sync", FluentIconData.Wrench20Regular, 0, "addin-sync", "AS", "Run add-in sync"),
                Item("addin-validate", "Validate", FluentIconData.Settings20Regular, 1, "addin-validate", "AV", "Validate add-in data")));

    private static RibbonTabViewModel Tab(string id, string header, int order, params RibbonGroupViewModel[] groups)
    {
        var tab = new RibbonTabViewModel
        {
            Id = id,
            Header = header,
            Order = order,
        };

        foreach (var group in groups)
        {
            tab.GroupsViewModel.Add(group);
        }

        return tab;
    }

    private static RibbonGroupViewModel Group(string id, string header, int order, params RibbonItemViewModel[] items)
        => Group(
            id,
            header,
            order,
            RibbonGroupHeaderPlacement.Bottom,
            RibbonGroupItemsLayoutMode.Auto,
            3,
            RibbonGroupDockedCenterLayoutMode.Auto,
            items);

    private static RibbonGroupViewModel Group(
        string id,
        string header,
        int order,
        RibbonGroupHeaderPlacement headerPlacement,
        params RibbonItemViewModel[] items)
        => Group(
            id,
            header,
            order,
            headerPlacement,
            RibbonGroupItemsLayoutMode.Auto,
            3,
            RibbonGroupDockedCenterLayoutMode.Auto,
            items);

    private static RibbonGroupViewModel Group(
        string id,
        string header,
        int order,
        RibbonGroupHeaderPlacement headerPlacement,
        RibbonGroupItemsLayoutMode itemsLayoutMode,
        int stackedRows,
        params RibbonItemViewModel[] items)
        => Group(
            id,
            header,
            order,
            headerPlacement,
            itemsLayoutMode,
            stackedRows,
            RibbonGroupDockedCenterLayoutMode.Auto,
            items);

    private static RibbonGroupViewModel Group(
        string id,
        string header,
        int order,
        RibbonGroupHeaderPlacement headerPlacement,
        RibbonGroupItemsLayoutMode itemsLayoutMode,
        int stackedRows,
        RibbonGroupDockedCenterLayoutMode dockedCenterLayoutMode,
        params RibbonItemViewModel[] items)
    {
        var group = new RibbonGroupViewModel
        {
            Id = id,
            Header = header,
            Order = order,
            HeaderPlacement = headerPlacement,
            ItemsLayoutMode = itemsLayoutMode,
            DockedCenterLayoutMode = dockedCenterLayoutMode,
            StackedRows = stackedRows,
        };

        foreach (var item in items)
        {
            group.ItemsViewModel.Add(item);
        }

        return group;
    }

    private static RibbonItemViewModel Item(
        string id,
        string label,
        string iconPathData,
        int order,
        string commandId,
        string keyTip,
        string screenTip,
        RibbonItemSize size = RibbonItemSize.Large,
        RibbonItemDisplayMode displayMode = RibbonItemDisplayMode.Auto,
        RibbonItemLayoutDock layoutDock = RibbonItemLayoutDock.Auto,
        bool isToggle = false)
        => new()
        {
            Id = id,
            Label = label,
            IconPathData = iconPathData,
            Order = order,
            CommandId = commandId,
            KeyTip = keyTip,
            ScreenTip = screenTip,
            Size = size,
            DisplayMode = displayMode,
            LayoutDock = layoutDock,
            IsToggle = isToggle,
        };

    private static RibbonItemViewModel PasteSplitItem(
        string id,
        string label,
        string iconPathData,
        int order,
        string commandId,
        string keyTip,
        string screenTip,
        params RibbonMenuItemViewModel[] menuItems)
    {
        var item = new RibbonItemViewModel
        {
            Id = id,
            Label = label,
            Primitive = RibbonItemPrimitive.PasteSplitButton,
            IconPathData = iconPathData,
            Order = order,
            CommandId = commandId,
            KeyTip = keyTip,
            ScreenTip = screenTip,
        };

        foreach (var menuItem in menuItems)
        {
            item.MenuItemsViewModel.Add(menuItem);
        }

        return item;
    }

    private static RibbonItemViewModel SplitItem(
        string id,
        string label,
        string iconPathData,
        int order,
        string commandId,
        string secondaryCommandId,
        string keyTip,
        string screenTip)
        => SplitItem(
            id,
            label,
            iconPathData,
            order,
            commandId,
            secondaryCommandId,
            keyTip,
            screenTip,
            RibbonSplitButtonMode.SideBySide);

    private static RibbonItemViewModel SplitItem(
        string id,
        string label,
        string iconPathData,
        int order,
        string commandId,
        string secondaryCommandId,
        string keyTip,
        string screenTip,
        RibbonSplitButtonMode splitButtonMode)
        => new()
        {
            Id = id,
            Label = label,
            Primitive = RibbonItemPrimitive.SplitButton,
            SplitButtonMode = splitButtonMode,
            IconPathData = iconPathData,
            Order = order,
            CommandId = commandId,
            SecondaryCommandId = secondaryCommandId,
            KeyTip = keyTip,
            ScreenTip = screenTip,
        };

    private static RibbonItemViewModel SplitItem(
        string id,
        string label,
        string iconPathData,
        int order,
        string commandId,
        string secondaryCommandId,
        string keyTip,
        string screenTip,
        params RibbonMenuItemViewModel[] menuItems)
        => SplitItem(
            id,
            label,
            iconPathData,
            order,
            commandId,
            secondaryCommandId,
            keyTip,
            screenTip,
            RibbonSplitButtonMode.SideBySide,
            menuItems);

    private static RibbonItemViewModel SplitItem(
        string id,
        string label,
        string iconPathData,
        int order,
        string commandId,
        string secondaryCommandId,
        string keyTip,
        string screenTip,
        RibbonSplitButtonMode splitButtonMode,
        params RibbonMenuItemViewModel[] menuItems)
    {
        var item = SplitItem(id, label, iconPathData, order, commandId, secondaryCommandId, keyTip, screenTip, splitButtonMode);

        foreach (var menuItem in menuItems)
        {
            item.MenuItemsViewModel.Add(menuItem);
        }

        return item;
    }

    private static RibbonItemViewModel SplitItemWithPopup(
        string id,
        string label,
        string iconPathData,
        int order,
        string commandId,
        string secondaryCommandId,
        string keyTip,
        string screenTip,
        string popupTitle,
        double popupMinWidth,
        double popupMaxHeight,
        object? popupContent = null,
        object? popupFooterContent = null,
        RibbonSplitButtonMode splitButtonMode = RibbonSplitButtonMode.SideBySide,
        params RibbonMenuItemViewModel[] menuItems)
    {
        var item = SplitItem(
            id,
            label,
            iconPathData,
            order,
            commandId,
            secondaryCommandId,
            keyTip,
            screenTip,
            splitButtonMode,
            menuItems);

        item.PopupTitle = popupTitle;
        item.PopupMinWidth = popupMinWidth;
        item.PopupMaxHeight = popupMaxHeight;
        item.PopupContent = popupContent;
        item.PopupFooterContent = popupFooterContent;
        return item;
    }

    private static RibbonItemViewModel CompactSplitItem(
        string id,
        string label,
        string iconPathData,
        int order,
        string commandId,
        string secondaryCommandId,
        string keyTip,
        string screenTip,
        params RibbonMenuItemViewModel[] menuItems)
    {
        var item = SplitItem(
            id,
            label,
            iconPathData,
            order,
            commandId,
            secondaryCommandId,
            keyTip,
            screenTip,
            RibbonSplitButtonMode.SideBySide,
            menuItems);

        item.Size = RibbonItemSize.Small;
        item.DisplayMode = RibbonItemDisplayMode.IconOnly;
        return item;
    }

    private static RibbonItemViewModel SmallSplitItem(
        string id,
        string label,
        string iconPathData,
        int order,
        string commandId,
        string secondaryCommandId,
        string keyTip,
        string screenTip,
        params RibbonMenuItemViewModel[] menuItems)
    {
        var item = SplitItem(
            id,
            label,
            iconPathData,
            order,
            commandId,
            secondaryCommandId,
            keyTip,
            screenTip,
            RibbonSplitButtonMode.SideBySide,
            menuItems);

        item.Size = RibbonItemSize.Small;
        return item;
    }

    private static RibbonItemViewModel CompactSplitItemWithPopup(
        string id,
        string label,
        string iconPathData,
        int order,
        string commandId,
        string secondaryCommandId,
        string keyTip,
        string screenTip,
        string popupTitle,
        double popupMinWidth,
        double popupMaxHeight,
        params RibbonMenuItemViewModel[] menuItems)
    {
        var item = CompactSplitItem(
            id,
            label,
            iconPathData,
            order,
            commandId,
            secondaryCommandId,
            keyTip,
            screenTip,
            menuItems);

        item.PopupTitle = popupTitle;
        item.PopupMinWidth = popupMinWidth;
        item.PopupMaxHeight = popupMaxHeight;
        return item;
    }

    private static RibbonItemViewModel MenuButtonItem(
        string id,
        string label,
        string iconPathData,
        int order,
        string commandId,
        string keyTip,
        string screenTip,
        params RibbonMenuItemViewModel[] menuItems)
    {
        var item = new RibbonItemViewModel
        {
            Id = id,
            Label = label,
            Primitive = RibbonItemPrimitive.MenuButton,
            IconPathData = iconPathData,
            Order = order,
            CommandId = commandId,
            KeyTip = keyTip,
            ScreenTip = screenTip,
        };

        foreach (var menuItem in menuItems)
        {
            item.MenuItemsViewModel.Add(menuItem);
        }

        return item;
    }

    private static RibbonItemViewModel MenuButtonItemWithPopup(
        string id,
        string label,
        string iconPathData,
        int order,
        string commandId,
        string keyTip,
        string screenTip,
        string popupTitle,
        double popupMinWidth,
        double popupMaxHeight,
        object? popupContent = null,
        object? popupFooterContent = null,
        params RibbonMenuItemViewModel[] menuItems)
    {
        var item = MenuButtonItem(id, label, iconPathData, order, commandId, keyTip, screenTip, menuItems);
        item.PopupTitle = popupTitle;
        item.PopupMinWidth = popupMinWidth;
        item.PopupMaxHeight = popupMaxHeight;
        item.PopupContent = popupContent;
        item.PopupFooterContent = popupFooterContent;
        return item;
    }

    private static RibbonItemViewModel GalleryItem(
        string id,
        string label,
        string iconPathData,
        int order,
        string keyTip,
        string screenTip,
        string description,
        params RibbonMenuItemViewModel[] menuItems)
    {
        var item = new RibbonItemViewModel
        {
            Id = id,
            Label = label,
            Primitive = RibbonItemPrimitive.Gallery,
            Description = description,
            IconPathData = iconPathData,
            Order = order,
            KeyTip = keyTip,
            ScreenTip = screenTip,
            GalleryPreviewMaxItems = 3,
            GalleryShowCategoryHeaders = true,
        };

        foreach (var menuItem in menuItems)
        {
            item.MenuItemsViewModel.Add(menuItem);
        }

        return item;
    }

    private static RibbonItemViewModel ToggleGroupItem(
        string id,
        string label,
        int order,
        RibbonItemSize size,
        RibbonToggleGroupSelectionMode selectionMode,
        int columns,
        string keyTip,
        string screenTip,
        params RibbonMenuItemViewModel[] menuItems)
    {
        var item = new RibbonItemViewModel
        {
            Id = id,
            Label = label,
            Primitive = RibbonItemPrimitive.ToggleGroup,
            Size = size,
            Order = order,
            ToggleGroupSelectionMode = selectionMode,
            ToggleGroupColumns = columns,
            KeyTip = keyTip,
            ScreenTip = screenTip,
        };

        foreach (var menuItem in menuItems)
        {
            item.MenuItemsViewModel.Add(menuItem);
        }

        return item;
    }

    private static RibbonMenuItemViewModel MenuEntry(
        string id,
        string label,
        string iconPathData,
        int order,
        string commandId)
        => new()
        {
            Id = id,
            Label = label,
            IconPathData = iconPathData,
            Order = order,
            CommandId = commandId,
        };

    private static RibbonMenuItemViewModel MenuEntryDetailed(
        string id,
        string label,
        int order,
        string? commandId,
        string? iconPathData = null,
        string? description = null,
        string? inputGestureText = null,
        bool showChevron = false,
        bool showInRibbonPreview = true,
        bool showInPopup = true,
        object? content = null,
        string? category = null,
        bool isSelected = false)
        => new()
        {
            Id = id,
            Label = label,
            IconPathData = iconPathData,
            Order = order,
            CommandId = commandId,
            Description = description,
            InputGestureText = inputGestureText,
            ShowChevron = showChevron,
            ShowInRibbonPreview = showInRibbonPreview,
            ShowInPopup = showInPopup,
            Content = content,
            Category = category,
            IsSelected = isSelected,
        };

    private static RibbonMenuItemViewModel MenuEntryWithSubMenu(
        string id,
        string label,
        string? iconPathData,
        int order,
        params RibbonMenuItemViewModel[] subMenuItems)
    {
        var item = MenuEntryDetailed(
            id,
            label,
            order,
            commandId: null,
            iconPathData: iconPathData,
            showChevron: true);

        foreach (var subMenuItem in subMenuItems)
        {
            item.SubMenuItemsViewModel.Add(subMenuItem);
        }

        return item;
    }

    private static RibbonMenuItemViewModel MenuSeparator(string id, int order, bool showInRibbonPreview = false, bool showInPopup = true)
        => new()
        {
            Id = id,
            Label = string.Empty,
            Order = order,
            IsSeparator = true,
            ShowInRibbonPreview = showInRibbonPreview,
            ShowInPopup = showInPopup,
        };

    private static void ApplyIconCustomizationSamples(IEnumerable<RibbonTabViewModel> tabs)
    {
        if (TryFindItem(tabs, "paste", out var paste))
        {
            paste.IconStretch = Stretch.Uniform;
            paste.IconStretchDirection = StretchDirection.Both;
            paste.IconMinWidth = 20;
            paste.IconMinHeight = 20;
            paste.IconMaxWidth = 28;
            paste.IconMaxHeight = 28;
        }

        if (TryFindItem(tabs, "cut", out var cut))
        {
            cut.IconResourceKey = "Ribbon.Sample.Icon.Cut";
            cut.IconWidth = 16;
            cut.IconHeight = 16;
            cut.IconStretch = Stretch.Uniform;
            cut.IconStretchDirection = StretchDirection.DownOnly;
        }

        if (TryFindItem(tabs, "format-painter", out var formatPainter))
        {
            formatPainter.IconResourceKey = "Ribbon.Sample.Icon.Emoji";
            formatPainter.IconEmoji = "🖌️";
            formatPainter.IconPathData = FluentIconData.Wrench20Regular;
        }

        if (TryFindItem(tabs, "quick-tables", out var quickTemplates))
        {
            quickTemplates.IconResourceKey = "Ribbon.Sample.Icon.QuickTemplates";
            quickTemplates.IconWidth = 18;
            quickTemplates.IconHeight = 18;
            quickTemplates.IconMinWidth = 16;
            quickTemplates.IconMinHeight = 16;
            quickTemplates.IconMaxWidth = 20;
            quickTemplates.IconMaxHeight = 20;
        }

        if (TryFindItem(tabs, "style-gallery", out var styleGallery))
        {
            styleGallery.IconResourceKey = "Ribbon.Sample.Icon.QuickTemplates";
            styleGallery.IconStretch = Stretch.Uniform;
            styleGallery.IconStretchDirection = StretchDirection.Both;
            styleGallery.IconMinWidth = 16;
            styleGallery.IconMinHeight = 16;
            styleGallery.IconMaxWidth = 24;
            styleGallery.IconMaxHeight = 24;
        }

        if (TryFindMenuItem(tabs, "quick-template-weekly", out var weeklyTemplate))
        {
            weeklyTemplate.IconEmoji = "🗓️";
            weeklyTemplate.IconResourceKey = "Ribbon.Sample.Icon.Emoji";
        }

        if (TryFindItem(tabs, "spelling", out var spelling))
        {
            spelling.OverlayPathData = FluentIconData.CheckmarkCircle20Regular;
            spelling.OverlayCount = 2;
        }

        if (TryFindItem(tabs, "new-comment", out var newComment))
        {
            newComment.OverlayCountText = "4";
        }

        if (TryFindItem(tabs, "track-changes", out var trackChanges))
        {
            trackChanges.OverlayEmoji = "✨";
        }
    }

    private static bool TryFindItem(IEnumerable<RibbonTabViewModel> tabs, string id, out RibbonItemViewModel item)
    {
        foreach (var tab in tabs)
        {
            foreach (var group in tab.GroupsViewModel)
            {
                foreach (var candidate in group.ItemsViewModel)
                {
                    if (string.Equals(candidate.Id, id, StringComparison.Ordinal))
                    {
                        item = candidate;
                        return true;
                    }
                }
            }
        }

        item = null!;
        return false;
    }

    private static bool TryFindMenuItem(IEnumerable<RibbonTabViewModel> tabs, string id, out RibbonMenuItemViewModel menuItem)
    {
        foreach (var tab in tabs)
        {
            foreach (var group in tab.GroupsViewModel)
            {
                if (TryFindMenuItem(group.ItemsViewModel, id, out menuItem))
                {
                    return true;
                }
            }
        }

        menuItem = null!;
        return false;
    }

    private static bool TryFindMenuItem(IEnumerable<RibbonItemViewModel> items, string id, out RibbonMenuItemViewModel menuItem)
    {
        foreach (var item in items)
        {
            if (TryFindMenuItem(item.MenuItemsViewModel, id, out menuItem))
            {
                return true;
            }
        }

        menuItem = null!;
        return false;
    }

    private static bool TryFindMenuItem(IEnumerable<RibbonMenuItemViewModel> menuItems, string id, out RibbonMenuItemViewModel menuItem)
    {
        foreach (var candidate in menuItems)
        {
            if (string.Equals(candidate.Id, id, StringComparison.Ordinal))
            {
                menuItem = candidate;
                return true;
            }

            if (TryFindMenuItem(candidate.SubMenuItemsViewModel, id, out menuItem))
            {
                return true;
            }
        }

        menuItem = null!;
        return false;
    }

    private static string ToDisplay(string commandId)
        => commandId.Replace("-", " ", StringComparison.Ordinal);
}
