// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.ObjectModel;
using System.Windows.Input;
using RibbonControl.Core.Contracts;
using RibbonControl.Core.Enums;
using RibbonControl.Core.Icons;
using RibbonControl.Core.Models;
using RibbonControl.Core.Services;

namespace RibbonControl.Samples.XamlOnly.ViewModels;

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
        "insert-endnote",
        "citations",
        "citations-insert",
        "citations-manage-sources",
        "citations-style-apa",
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
        "separate-pages",
        "immersive-reader",
        "print-layout",
        "web-layout",
        "ruler",
        "navigation-pane",
        "zoom",
        "zoom-100",
        "zoom-level-50",
        "zoom-level-75",
        "zoom-level-100",
        "zoom-level-125",
        "zoom-level-150",
        "view-footnotes",
        "view-endnotes",
        "dark-mode",
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
        "style-manage-default-document",
        "style-manage-default-all",
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
    ];

    private string _status = "Ready.";
    private string? _selectedTabId = "home";
    private bool _isBackstageOpen;
    private bool _isMinimized;
    private bool _isKeyTipMode;
    private IReadOnlyList<string> _activeContextGroupIds = [];
    private RibbonQuickAccessPlacement _quickAccessPlacement = RibbonQuickAccessPlacement.Above;
    private RibbonStateOwnershipMode _stateOwnershipMode = RibbonStateOwnershipMode.Synchronized;

    public MainWindowViewModel()
    {
        var catalog = new DictionaryRibbonCommandCatalog();

        foreach (var commandId in WordCommandIds)
        {
            if (IsViewCommandDisabled(commandId))
            {
                catalog.Register(commandId, new RelayCommand(_ => { }, _ => false));
                continue;
            }

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

        TogglePictureToolsCommand = new RelayCommand(_ => TogglePictureTools());
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
    }

    public IRibbonCommandCatalog CommandCatalog { get; }

    public IRibbonStateStore StateStore { get; }

    public ObservableCollection<RibbonItem> QuickAccessItems { get; }

    public IReadOnlyList<RibbonStateOwnershipMode> StateOwnershipModes { get; }

    public IReadOnlyList<RibbonQuickAccessPlacement> QuickAccessPlacements { get; }

    public ICommand TogglePictureToolsCommand { get; }

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

    public string Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    public string? SelectedTabId
    {
        get => _selectedTabId;
        set => SetProperty(ref _selectedTabId, value);
    }

    public bool IsBackstageOpen
    {
        get => _isBackstageOpen;
        set => SetProperty(ref _isBackstageOpen, value);
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

    public IReadOnlyList<string> ActiveContextGroupIds
    {
        get => _activeContextGroupIds;
        private set => SetProperty(ref _activeContextGroupIds, value);
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
            IsKeyTipMode = false,
            QuickAccessPlacement = RibbonQuickAccessPlacement.Below,
            ActiveContextGroupIds = ["picture-tools"],
            NodeCustomizations =
            {
                new RibbonNodeCustomization { Id = "translate", ParentId = "language", IsHidden = true },
            },
        });

        Status = "Seeded Word-like state profile. Run Load State to apply it.";
    }

    private static bool IsViewCommandDisabled(string commandId)
        => commandId is "view-footnotes" or "view-endnotes";

    private static string ToDisplay(string commandId)
        => commandId.Replace("-", " ", StringComparison.Ordinal);
}
