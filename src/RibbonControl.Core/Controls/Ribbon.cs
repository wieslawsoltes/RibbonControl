// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using Avalonia;
using Avalonia.Automation.Peers;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Metadata;
using Avalonia.Threading;
using Avalonia.VisualTree;
using RibbonControl.Core.Automation.Peers;
using RibbonControl.Core.Collections;
using RibbonControl.Core.Contracts;
using RibbonControl.Core.Enums;
using RibbonControl.Core.Models;
using RibbonControl.Core.Services;

namespace RibbonControl.Core.Controls;

[TemplatePart("PART_TabControl", typeof(TabControl), IsRequired = true)]
[TemplatePart("PART_MinimizedDropDownHost", typeof(Border))]
[TemplatePart("PART_MinimizedDropDownContentHost", typeof(ContentPresenter))]
public class Ribbon : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable<IRibbonTabNode>?> TabsSourceProperty =
        AvaloniaProperty.Register<Ribbon, IEnumerable<IRibbonTabNode>?>(nameof(TabsSource));

    public static readonly StyledProperty<RibbonMergeMode> TabMergeModeProperty =
        AvaloniaProperty.Register<Ribbon, RibbonMergeMode>(nameof(TabMergeMode), RibbonMergeMode.Merge);

    public static readonly StyledProperty<string?> SelectedTabIdProperty =
        AvaloniaProperty.Register<Ribbon, string?>(
            nameof(SelectedTabId),
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<bool> IsMinimizedProperty =
        AvaloniaProperty.Register<Ribbon, bool>(
            nameof(IsMinimized),
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<bool> IsKeyTipModeProperty =
        AvaloniaProperty.Register<Ribbon, bool>(
            nameof(IsKeyTipMode),
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<IReadOnlyList<string>> ActiveContextGroupIdsProperty =
        AvaloniaProperty.Register<Ribbon, IReadOnlyList<string>>(
            nameof(ActiveContextGroupIds),
            []);

    public static readonly StyledProperty<IEnumerable<RibbonItem>?> QuickAccessItemsProperty =
        AvaloniaProperty.Register<Ribbon, IEnumerable<RibbonItem>?>(nameof(QuickAccessItems));

    public static readonly StyledProperty<RibbonQuickAccessPlacement> QuickAccessPlacementProperty =
        AvaloniaProperty.Register<Ribbon, RibbonQuickAccessPlacement>(nameof(QuickAccessPlacement), RibbonQuickAccessPlacement.Above);

    public static readonly StyledProperty<RibbonBackstage?> BackstageProperty =
        AvaloniaProperty.Register<Ribbon, RibbonBackstage?>(nameof(Backstage));

    public static readonly StyledProperty<object?> HeaderStartContentProperty =
        AvaloniaProperty.Register<Ribbon, object?>(nameof(HeaderStartContent));

    public static readonly StyledProperty<IDataTemplate?> HeaderStartContentTemplateProperty =
        AvaloniaProperty.Register<Ribbon, IDataTemplate?>(nameof(HeaderStartContentTemplate));

    public static readonly StyledProperty<object?> HeaderEndContentProperty =
        AvaloniaProperty.Register<Ribbon, object?>(nameof(HeaderEndContent));

    public static readonly StyledProperty<IDataTemplate?> HeaderEndContentTemplateProperty =
        AvaloniaProperty.Register<Ribbon, IDataTemplate?>(nameof(HeaderEndContentTemplate));

    public static readonly StyledProperty<object?> TopBarStartContentProperty =
        AvaloniaProperty.Register<Ribbon, object?>(nameof(TopBarStartContent));

    public static readonly StyledProperty<IDataTemplate?> TopBarStartContentTemplateProperty =
        AvaloniaProperty.Register<Ribbon, IDataTemplate?>(nameof(TopBarStartContentTemplate));

    public static readonly StyledProperty<object?> TopBarCenterContentProperty =
        AvaloniaProperty.Register<Ribbon, object?>(nameof(TopBarCenterContent));

    public static readonly StyledProperty<IDataTemplate?> TopBarCenterContentTemplateProperty =
        AvaloniaProperty.Register<Ribbon, IDataTemplate?>(nameof(TopBarCenterContentTemplate));

    public static readonly StyledProperty<object?> TopBarEndContentProperty =
        AvaloniaProperty.Register<Ribbon, object?>(nameof(TopBarEndContent));

    public static readonly StyledProperty<IDataTemplate?> TopBarEndContentTemplateProperty =
        AvaloniaProperty.Register<Ribbon, IDataTemplate?>(nameof(TopBarEndContentTemplate));

    public static readonly StyledProperty<RibbonStateOwnershipMode> StateOwnershipModeProperty =
        AvaloniaProperty.Register<Ribbon, RibbonStateOwnershipMode>(
            nameof(StateOwnershipMode),
            RibbonStateOwnershipMode.Synchronized);

    public static readonly StyledProperty<IRibbonCommandCatalog?> CommandCatalogProperty =
        AvaloniaProperty.Register<Ribbon, IRibbonCommandCatalog?>(nameof(CommandCatalog));

    public static readonly StyledProperty<IRibbonMergePolicy> MergePolicyProperty =
        AvaloniaProperty.Register<Ribbon, IRibbonMergePolicy>(
            nameof(MergePolicy),
            RibbonMergePolicy.StaticThenDynamic);

    public static readonly StyledProperty<IRibbonCustomizationService> CustomizationServiceProperty =
        AvaloniaProperty.Register<Ribbon, IRibbonCustomizationService>(
            nameof(CustomizationService),
            new RibbonCustomizationService());

    public static readonly StyledProperty<IRibbonStateStore?> StateStoreProperty =
        AvaloniaProperty.Register<Ribbon, IRibbonStateStore?>(nameof(StateStore));

    public static readonly StyledProperty<IRibbonAdaptiveLayoutEngine> AdaptiveLayoutEngineProperty =
        AvaloniaProperty.Register<Ribbon, IRibbonAdaptiveLayoutEngine>(
            nameof(AdaptiveLayoutEngine),
            new RibbonAdaptiveLayoutEngine());

    public static readonly StyledProperty<bool> EnableAdaptiveLayoutProperty =
        AvaloniaProperty.Register<Ribbon, bool>(nameof(EnableAdaptiveLayout), true);

    public static readonly StyledProperty<double> AdaptiveLayoutHorizontalPaddingProperty =
        AvaloniaProperty.Register<Ribbon, double>(nameof(AdaptiveLayoutHorizontalPadding), 24);

    public static readonly StyledProperty<bool> MaintainStableRibbonHeightProperty =
        AvaloniaProperty.Register<Ribbon, bool>(nameof(MaintainStableRibbonHeight), true);

    public static readonly StyledProperty<double> StableRibbonMinHeightProperty =
        AvaloniaProperty.Register<Ribbon, double>(nameof(StableRibbonMinHeight), 120);

    public static readonly StyledProperty<bool> SynchronizeCommandHeightsProperty =
        AvaloniaProperty.Register<Ribbon, bool>(nameof(SynchronizeCommandHeights), true);

    public static readonly StyledProperty<bool> AutoSynchronizeCommandHeightsProperty =
        AvaloniaProperty.Register<Ribbon, bool>(nameof(AutoSynchronizeCommandHeights), true);

    public static readonly StyledProperty<double> SynchronizedLargeCommandHeightProperty =
        AvaloniaProperty.Register<Ribbon, double>(nameof(SynchronizedLargeCommandHeight), 66);

    public static readonly StyledProperty<double> SynchronizedSmallCommandHeightProperty =
        AvaloniaProperty.Register<Ribbon, double>(nameof(SynchronizedSmallCommandHeight), 30);

    public static readonly DirectProperty<Ribbon, RibbonTab?> SelectedTabProperty =
        AvaloniaProperty.RegisterDirect<Ribbon, RibbonTab?>(
            nameof(SelectedTab),
            owner => owner.SelectedTab,
            (owner, value) => owner.SelectedTab = value,
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly DirectProperty<Ribbon, IReadOnlyDictionary<string, string>> ActiveKeyTipsProperty =
        AvaloniaProperty.RegisterDirect<Ribbon, IReadOnlyDictionary<string, string>>(
            nameof(ActiveKeyTips),
            owner => owner.ActiveKeyTips);

    public static readonly DirectProperty<Ribbon, string> KeyTipSequenceProperty =
        AvaloniaProperty.RegisterDirect<Ribbon, string>(
            nameof(KeyTipSequence),
            owner => owner.KeyTipSequence);

    public static readonly DirectProperty<Ribbon, ICommand> ToggleBackstageCommandProperty =
        AvaloniaProperty.RegisterDirect<Ribbon, ICommand>(
            nameof(ToggleBackstageCommand),
            owner => owner.ToggleBackstageCommand);

    public static readonly DirectProperty<Ribbon, ICommand> LoadStateCommandProperty =
        AvaloniaProperty.RegisterDirect<Ribbon, ICommand>(
            nameof(LoadStateCommand),
            owner => owner.LoadStateCommand);

    public static readonly DirectProperty<Ribbon, ICommand> SaveStateCommandProperty =
        AvaloniaProperty.RegisterDirect<Ribbon, ICommand>(
            nameof(SaveStateCommand),
            owner => owner.SaveStateCommand);

    public static readonly DirectProperty<Ribbon, ICommand> ResetStateCommandProperty =
        AvaloniaProperty.RegisterDirect<Ribbon, ICommand>(
            nameof(ResetStateCommand),
            owner => owner.ResetStateCommand);

    public static readonly DirectProperty<Ribbon, ICommand> ResetCustomizationCommandProperty =
        AvaloniaProperty.RegisterDirect<Ribbon, ICommand>(
            nameof(ResetCustomizationCommand),
            owner => owner.ResetCustomizationCommand);

    public static readonly DirectProperty<Ribbon, RibbonRuntimeState?> RuntimeStateProperty =
        AvaloniaProperty.RegisterDirect<Ribbon, RibbonRuntimeState?>(
            nameof(RuntimeState),
            owner => owner.RuntimeState,
            (owner, value) => owner.RuntimeState = value);

    public static readonly DirectProperty<Ribbon, bool> IsMinimizedDropDownOpenProperty =
        AvaloniaProperty.RegisterDirect<Ribbon, bool>(
            nameof(IsMinimizedDropDownOpen),
            owner => owner.IsMinimizedDropDownOpen);

    private static readonly IReadOnlyDictionary<string, string> EmptyKeyTips =
        new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());

    private readonly ObservableCollection<RibbonTab> _mergedTabs = [];
    private readonly ReadOnlyObservableCollection<RibbonTab> _readonlyMergedTabs;
    private readonly ObservableCollection<RibbonItem> _resolvedQuickAccessItems = [];
    private readonly Dictionary<Control, object?> _originalToolTips = [];
    private readonly Dictionary<RibbonItem, PropertyChangedEventHandler> _dropDownHandlers = [];
    private readonly Dictionary<RibbonGroup, PropertyChangedEventHandler> _groupDropDownHandlers = [];

    private TabControl? _tabControl;
    private RibbonContextualTabBand? _contextualBand;
    private DockPanel? _quickAccessTopRow;
    private DockPanel? _quickAccessBottomRow;
    private RibbonQuickAccessToolBar? _quickAccessTopToolbar;
    private RibbonQuickAccessToolBar? _quickAccessBottomToolbar;
    private Button? _backstageTopButton;
    private Button? _backstageBottomButton;
    private Border? _backstageHost;
    private ContentPresenter? _backstageContentHost;
    private RibbonBackstage? _attachedBackstage;
    private INotifyCollectionChanged? _quickAccessItemsNotifier;
    private Border? _minimizedDropDownHost;
    private ContentPresenter? _minimizedDropDownContentHost;

    private bool _isSynchronizingSelection;
    private bool _isApplyingRuntimeState;
    private bool _isApplyingAdaptiveLayout;
    private bool _isTabStage;
    private bool _handlingKeyTipProperty;
    private bool _isMinimizedDropDownOpen;
    private double _stableRibbonContentHeight;
    private double _stableLargeCommandHeight;
    private double _stableSmallCommandHeight;
    private RibbonTab? _selectedTab;
    private RibbonRuntimeState? _runtimeState;
    private IKeyTipService _keyTipService;
    private IReadOnlyDictionary<string, string> _activeKeyTips = EmptyKeyTips;
    private string _keyTipSequence = string.Empty;
    private readonly ICommand _toggleBackstageCommand;
    private readonly ICommand _loadStateCommand;
    private readonly ICommand _saveStateCommand;
    private readonly ICommand _resetStateCommand;
    private readonly ICommand _resetCustomizationCommand;

    public Ribbon()
    {
        _readonlyMergedTabs = new ReadOnlyObservableCollection<RibbonTab>(_mergedTabs);
        _keyTipService = new KeyTipService();
        _toggleBackstageCommand = new DelegateCommand(_ => ToggleBackstage());
        _loadStateCommand = new AsyncDelegateCommand(_ => LoadStateAsync());
        _saveStateCommand = new AsyncDelegateCommand(_ => SaveStateAsync());
        _resetStateCommand = new AsyncDelegateCommand(_ => ResetStateAsync());
        _resetCustomizationCommand = new DelegateCommand(_ => ResetCustomization());

        Tabs.CollectionChanged += OnStaticTabsCollectionChanged;
        UpdateQuickAccessItemsSubscription();
    }

    [Content]
    public AvaloniaList<RibbonTab> Tabs { get; } = [];

    public ReadOnlyObservableCollection<RibbonTab> MergedTabs => _readonlyMergedTabs;

    public IEnumerable<IRibbonTabNode>? TabsSource
    {
        get => GetValue(TabsSourceProperty);
        set => SetValue(TabsSourceProperty, value);
    }

    public RibbonMergeMode TabMergeMode
    {
        get => GetValue(TabMergeModeProperty);
        set => SetValue(TabMergeModeProperty, value);
    }

    public string? SelectedTabId
    {
        get => GetValue(SelectedTabIdProperty);
        set => SetValue(SelectedTabIdProperty, value);
    }

    public bool IsMinimized
    {
        get => GetValue(IsMinimizedProperty);
        set => SetValue(IsMinimizedProperty, value);
    }

    public bool IsKeyTipMode
    {
        get => GetValue(IsKeyTipModeProperty);
        set => SetValue(IsKeyTipModeProperty, value);
    }

    public IReadOnlyList<string> ActiveContextGroupIds
    {
        get => GetValue(ActiveContextGroupIdsProperty);
        set => SetValue(ActiveContextGroupIdsProperty, value);
    }

    public RibbonStateOwnershipMode StateOwnershipMode
    {
        get => GetValue(StateOwnershipModeProperty);
        set => SetValue(StateOwnershipModeProperty, value);
    }

    public IEnumerable<RibbonItem>? QuickAccessItems
    {
        get => GetValue(QuickAccessItemsProperty);
        set => SetValue(QuickAccessItemsProperty, value);
    }

    public RibbonQuickAccessPlacement QuickAccessPlacement
    {
        get => GetValue(QuickAccessPlacementProperty);
        set => SetValue(QuickAccessPlacementProperty, value);
    }

    public RibbonBackstage? Backstage
    {
        get => GetValue(BackstageProperty);
        set => SetValue(BackstageProperty, value);
    }

    public object? HeaderStartContent
    {
        get => GetValue(HeaderStartContentProperty);
        set => SetValue(HeaderStartContentProperty, value);
    }

    public IDataTemplate? HeaderStartContentTemplate
    {
        get => GetValue(HeaderStartContentTemplateProperty);
        set => SetValue(HeaderStartContentTemplateProperty, value);
    }

    public object? HeaderEndContent
    {
        get => GetValue(HeaderEndContentProperty);
        set => SetValue(HeaderEndContentProperty, value);
    }

    public IDataTemplate? HeaderEndContentTemplate
    {
        get => GetValue(HeaderEndContentTemplateProperty);
        set => SetValue(HeaderEndContentTemplateProperty, value);
    }

    public object? TopBarStartContent
    {
        get => GetValue(TopBarStartContentProperty);
        set => SetValue(TopBarStartContentProperty, value);
    }

    public IDataTemplate? TopBarStartContentTemplate
    {
        get => GetValue(TopBarStartContentTemplateProperty);
        set => SetValue(TopBarStartContentTemplateProperty, value);
    }

    public object? TopBarCenterContent
    {
        get => GetValue(TopBarCenterContentProperty);
        set => SetValue(TopBarCenterContentProperty, value);
    }

    public IDataTemplate? TopBarCenterContentTemplate
    {
        get => GetValue(TopBarCenterContentTemplateProperty);
        set => SetValue(TopBarCenterContentTemplateProperty, value);
    }

    public object? TopBarEndContent
    {
        get => GetValue(TopBarEndContentProperty);
        set => SetValue(TopBarEndContentProperty, value);
    }

    public IDataTemplate? TopBarEndContentTemplate
    {
        get => GetValue(TopBarEndContentTemplateProperty);
        set => SetValue(TopBarEndContentTemplateProperty, value);
    }

    public IRibbonCommandCatalog? CommandCatalog
    {
        get => GetValue(CommandCatalogProperty);
        set => SetValue(CommandCatalogProperty, value);
    }

    public IRibbonMergePolicy MergePolicy
    {
        get => GetValue(MergePolicyProperty);
        set => SetValue(MergePolicyProperty, value);
    }

    public IRibbonCustomizationService CustomizationService
    {
        get => GetValue(CustomizationServiceProperty);
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            SetValue(CustomizationServiceProperty, value);
        }
    }

    public IRibbonStateStore? StateStore
    {
        get => GetValue(StateStoreProperty);
        set => SetValue(StateStoreProperty, value);
    }

    public IRibbonAdaptiveLayoutEngine AdaptiveLayoutEngine
    {
        get => GetValue(AdaptiveLayoutEngineProperty);
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            SetValue(AdaptiveLayoutEngineProperty, value);
        }
    }

    public bool EnableAdaptiveLayout
    {
        get => GetValue(EnableAdaptiveLayoutProperty);
        set => SetValue(EnableAdaptiveLayoutProperty, value);
    }

    public double AdaptiveLayoutHorizontalPadding
    {
        get => GetValue(AdaptiveLayoutHorizontalPaddingProperty);
        set => SetValue(AdaptiveLayoutHorizontalPaddingProperty, Math.Max(0, value));
    }

    public bool MaintainStableRibbonHeight
    {
        get => GetValue(MaintainStableRibbonHeightProperty);
        set => SetValue(MaintainStableRibbonHeightProperty, value);
    }

    public double StableRibbonMinHeight
    {
        get => GetValue(StableRibbonMinHeightProperty);
        set => SetValue(StableRibbonMinHeightProperty, Math.Max(0, value));
    }

    public bool SynchronizeCommandHeights
    {
        get => GetValue(SynchronizeCommandHeightsProperty);
        set => SetValue(SynchronizeCommandHeightsProperty, value);
    }

    public bool AutoSynchronizeCommandHeights
    {
        get => GetValue(AutoSynchronizeCommandHeightsProperty);
        set => SetValue(AutoSynchronizeCommandHeightsProperty, value);
    }

    public double SynchronizedLargeCommandHeight
    {
        get => GetValue(SynchronizedLargeCommandHeightProperty);
        set => SetValue(SynchronizedLargeCommandHeightProperty, Math.Max(0, value));
    }

    public double SynchronizedSmallCommandHeight
    {
        get => GetValue(SynchronizedSmallCommandHeightProperty);
        set => SetValue(SynchronizedSmallCommandHeightProperty, Math.Max(0, value));
    }

    internal double EffectiveSynchronizedLargeCommandHeight =>
        AutoSynchronizeCommandHeights
            ? Math.Max(SynchronizedLargeCommandHeight, _stableLargeCommandHeight)
            : SynchronizedLargeCommandHeight;

    internal double EffectiveSynchronizedSmallCommandHeight =>
        AutoSynchronizeCommandHeights
            ? Math.Max(SynchronizedSmallCommandHeight, _stableSmallCommandHeight)
            : SynchronizedSmallCommandHeight;

    public RibbonTab? SelectedTab
    {
        get => _selectedTab;
        set => SetAndRaise(SelectedTabProperty, ref _selectedTab, value);
    }

    public IReadOnlyDictionary<string, string> ActiveKeyTips
    {
        get => _activeKeyTips;
        private set => SetAndRaise(ActiveKeyTipsProperty, ref _activeKeyTips, value);
    }

    public string KeyTipSequence
    {
        get => _keyTipSequence;
        private set => SetAndRaise(KeyTipSequenceProperty, ref _keyTipSequence, value);
    }

    public RibbonRuntimeState? RuntimeState
    {
        get => _runtimeState;
        private set => SetAndRaise(RuntimeStateProperty, ref _runtimeState, value);
    }

    public bool IsMinimizedDropDownOpen
    {
        get => _isMinimizedDropDownOpen;
        private set => SetAndRaise(IsMinimizedDropDownOpenProperty, ref _isMinimizedDropDownOpen, value);
    }

    public ICommand ToggleBackstageCommand => _toggleBackstageCommand;

    public ICommand LoadStateCommand => _loadStateCommand;

    public ICommand SaveStateCommand => _saveStateCommand;

    public ICommand ResetStateCommand => _resetStateCommand;

    public ICommand ResetCustomizationCommand => _resetCustomizationCommand;

    public IKeyTipService KeyTipService
    {
        get => _keyTipService;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            _keyTipService = value;
            if (IsKeyTipMode)
            {
                EnterKeyTipMode();
            }
        }
    }

    public void ApplyRuntimeState(RibbonRuntimeState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        RuntimeState = CloneRuntimeState(state);
        ApplyControlStateFromRuntimeState(RuntimeState);
        RebuildTabs();
        UpdateQuickAccessRows();

        if (IsKeyTipMode)
        {
            EnterKeyTipMode();
        }
        else
        {
            ExitKeyTipMode();
        }
    }

    public RibbonRuntimeState ExportRuntimeState()
    {
        var snapshot = BuildRuntimeStateSnapshot();
        RuntimeState = CloneRuntimeState(snapshot);
        return snapshot;
    }

    public async Task<bool> LoadStateAsync(CancellationToken cancellationToken = default)
    {
        var store = StateStore;
        if (store is null)
        {
            return false;
        }

        var loaded = await store.LoadAsync(cancellationToken).ConfigureAwait(false);
        if (loaded is null)
        {
            return false;
        }

        if (Dispatcher.UIThread.CheckAccess())
        {
            ApplyRuntimeState(loaded);
        }
        else
        {
            await Dispatcher.UIThread.InvokeAsync(() => ApplyRuntimeState(loaded));
        }

        return true;
    }

    public async Task SaveStateAsync(CancellationToken cancellationToken = default)
    {
        var store = StateStore;
        if (store is null)
        {
            return;
        }

        RibbonRuntimeState snapshot;
        if (Dispatcher.UIThread.CheckAccess())
        {
            snapshot = ExportRuntimeState();
        }
        else
        {
            snapshot = await Dispatcher.UIThread.InvokeAsync(ExportRuntimeState);
        }

        await store.SaveAsync(snapshot, cancellationToken).ConfigureAwait(false);
    }

    public async Task ResetStateAsync(CancellationToken cancellationToken = default)
    {
        var store = StateStore;
        if (store is not null)
        {
            await store.ResetAsync(cancellationToken).ConfigureAwait(false);
        }

        if (Dispatcher.UIThread.CheckAccess())
        {
            ResetRuntimeStateCore();
        }
        else
        {
            await Dispatcher.UIThread.InvokeAsync(ResetRuntimeStateCore);
        }
    }

    public void ResetCustomization()
    {
        RuntimeState = null;
        RebuildTabs();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _tabControl = e.NameScope.Find<TabControl>("PART_TabControl");
        _contextualBand = e.NameScope.Find<RibbonContextualTabBand>("PART_ContextualTabBand");
        _quickAccessTopRow = e.NameScope.Find<DockPanel>("PART_QuickAccessTopRow");
        _quickAccessBottomRow = e.NameScope.Find<DockPanel>("PART_QuickAccessBottomRow");
        _quickAccessTopToolbar = e.NameScope.Find<RibbonQuickAccessToolBar>("PART_QuickAccessTop");
        _quickAccessBottomToolbar = e.NameScope.Find<RibbonQuickAccessToolBar>("PART_QuickAccessBottom");
        _backstageTopButton = e.NameScope.Find<Button>("PART_BackstageButtonTop");
        _backstageBottomButton = e.NameScope.Find<Button>("PART_BackstageButtonBottom");
        _backstageHost = e.NameScope.Find<Border>("PART_BackstageHost");
        _backstageContentHost = e.NameScope.Find<ContentPresenter>("PART_BackstageContentHost");
        _minimizedDropDownHost = e.NameScope.Find<Border>("PART_MinimizedDropDownHost");
        _minimizedDropDownContentHost = e.NameScope.Find<ContentPresenter>("PART_MinimizedDropDownContentHost");

        if (_tabControl is not null)
        {
            _isSynchronizingSelection = true;
            try
            {
                _tabControl.ItemsSource = MergedTabs;
                _tabControl.Bind(
                    SelectingItemsControl.SelectedItemProperty,
                    new Binding(nameof(SelectedTab))
                    {
                        Source = this,
                        Mode = BindingMode.TwoWay,
                    });
            }
            finally
            {
                _isSynchronizingSelection = false;
            }
        }

        UpdateQuickAccessItemsSubscription();
        UpdateQuickAccessRows();
        UpdateContextualBand();
        AttachBackstage(Backstage);
        UpdateBackstageHost();
        SyncSelectedTabFromState();
        UpdateMinimizedVisualState();
        RewireGroupDropDownHandlers();
        UpdateAdaptiveLayout();
        ApplySynchronizedCommandHeights();
        UpdateStableRibbonHeight();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);

        ExitKeyTipMode();
        CloseMinimizedDropDown();
        AttachBackstage(null);
        DetachQuickAccessItemsSubscription();
        DetachDropDownHandlers();
        DetachGroupDropDownHandlers();
        ReleaseMergedTabBindings();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (IsMinimizedDropDownOpen && !IsKeyTipMode && e.Key == Key.Escape)
        {
            CloseMinimizedDropDown();
            e.Handled = true;
            return;
        }

        if (!IsKeyTipMode &&
            e.Key == Key.Tab &&
            IsKeyboardFocusWithin &&
            TryMoveRibbonFocus(e.KeyModifiers))
        {
            e.Handled = true;
            return;
        }

        HandleKeyTipInput(e);

        if (!e.Handled)
        {
            base.OnKeyDown(e);
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        var source = e.Source as Visual;

        if (IsMinimized &&
            source is not null &&
            _tabControl is not null &&
            IsVisualOrDescendantOf(source, _tabControl))
        {
            var tabIdResolved = TryGetTabIdFromPointerSource(source, out var resolvedTabId);
            var tabId = tabIdResolved ? resolvedTabId : SelectedTabId ?? SelectedTab?.Id;
            if (string.IsNullOrWhiteSpace(tabId))
            {
                base.OnPointerPressed(e);
                return;
            }

            var sameTab = string.Equals(SelectedTabId, tabId, StringComparison.Ordinal);
            if (!sameTab)
            {
                SelectedTabId = tabId;
                OpenMinimizedDropDown();
            }
            else if (IsMinimizedDropDownOpen)
            {
                CloseMinimizedDropDown();
            }
            else
            {
                OpenMinimizedDropDown();
            }
        }

        if (IsMinimizedDropDownOpen &&
            source is not null &&
            _minimizedDropDownHost is not null &&
            _tabControl is not null &&
            !IsVisualOrDescendantOf(source, _minimizedDropDownHost) &&
            !IsVisualOrDescendantOf(source, _tabControl))
        {
            CloseMinimizedDropDown();
        }

        if (Backstage?.IsOpen == true &&
            source is not null &&
            _backstageHost is not null &&
            _backstageContentHost is not null &&
            IsVisualOrDescendantOf(source, _backstageHost) &&
            !IsVisualOrDescendantOf(source, _backstageContentHost) &&
            (_backstageTopButton is null || !IsVisualOrDescendantOf(source, _backstageTopButton)) &&
            (_backstageBottomButton is null || !IsVisualOrDescendantOf(source, _backstageBottomButton)))
        {
            Backstage.IsOpen = false;
            UpdateBackstageHost();
        }

        base.OnPointerPressed(e);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var arranged = base.ArrangeOverride(finalSize);
        UpdateAdaptiveLayout();
        ApplySynchronizedCommandHeights();
        UpdateStableRibbonHeight();
        return arranged;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        SynchronizeRuntimeStateFromLive(change.Property);

        if (_isApplyingRuntimeState)
        {
            return;
        }

        if (change.Property == CustomizationServiceProperty)
        {
            RebuildTabs();
            return;
        }

        if (change.Property == StateOwnershipModeProperty)
        {
            if (StateOwnershipMode == RibbonStateOwnershipMode.Synchronized && RuntimeState is not null)
            {
                SynchronizeRuntimeStateFromControl();
            }

            return;
        }

        if (change.Property == TabsSourceProperty ||
            change.Property == TabMergeModeProperty ||
            change.Property == MergePolicyProperty ||
            change.Property == ActiveContextGroupIdsProperty)
        {
            RebuildTabs();
            return;
        }

        if (change.Property == AdaptiveLayoutEngineProperty ||
            change.Property == EnableAdaptiveLayoutProperty ||
            change.Property == AdaptiveLayoutHorizontalPaddingProperty)
        {
            UpdateAdaptiveLayout();
            return;
        }

        if (change.Property == SynchronizeCommandHeightsProperty ||
            change.Property == AutoSynchronizeCommandHeightsProperty ||
            change.Property == SynchronizedLargeCommandHeightProperty ||
            change.Property == SynchronizedSmallCommandHeightProperty)
        {
            if (change.Property == SynchronizeCommandHeightsProperty && !SynchronizeCommandHeights)
            {
                ResetSynchronizedCommandHeightCache();
            }
            else if (change.Property == AutoSynchronizeCommandHeightsProperty)
            {
                ResetSynchronizedCommandHeightCache();
            }

            ApplySynchronizedCommandHeights();
            return;
        }

        if (change.Property == MaintainStableRibbonHeightProperty ||
            change.Property == StableRibbonMinHeightProperty)
        {
            if (change.Property == MaintainStableRibbonHeightProperty && !MaintainStableRibbonHeight)
            {
                _stableRibbonContentHeight = 0;
            }

            UpdateStableRibbonHeight();
            return;
        }

        if (change.Property == CommandCatalogProperty)
        {
            RebuildTabs();
            RebuildResolvedQuickAccessItems();

            if (IsKeyTipMode && _isTabStage)
            {
                BuildTabStageKeyTips();
            }

            return;
        }

        if (change.Property == SelectedTabIdProperty)
        {
            if (!_isSynchronizingSelection)
            {
                SyncSelectedTabFromState();
            }

            if (IsKeyTipMode && !_isTabStage)
            {
                BuildCommandStageKeyTips();
            }

            UpdateAdaptiveLayout();

            return;
        }

        if (change.Property == IsMinimizedProperty)
        {
            UpdateMinimizedVisualState();
            UpdateStableRibbonHeight();
            return;
        }

        if (change.Property == SelectedTabProperty)
        {
            if (!_isSynchronizingSelection)
            {
                SyncSelectedTabIdFromSelection();
            }

            if (IsMinimized &&
                change.OldValue is RibbonTab &&
                change.NewValue is RibbonTab &&
                IsKeyboardFocusWithin)
            {
                OpenMinimizedDropDown();
            }

            if (IsKeyTipMode && !_isTabStage)
            {
                BuildCommandStageKeyTips();
            }

            UpdateAdaptiveLayout();

            return;
        }

        if (change.Property == QuickAccessItemsProperty)
        {
            UpdateQuickAccessItemsSubscription();
            UpdateQuickAccessRows();

            if (IsKeyTipMode && _isTabStage)
            {
                BuildTabStageKeyTips();
            }

            return;
        }

        if (change.Property == QuickAccessPlacementProperty)
        {
            UpdateQuickAccessRows();

            if (IsKeyTipMode && _isTabStage)
            {
                BuildTabStageKeyTips();
            }

            return;
        }

        if (change.Property == BackstageProperty)
        {
            AttachBackstage(change.NewValue as RibbonBackstage);
            UpdateBackstageHost();

            if (Backstage?.IsOpen == true)
            {
                CloseMinimizedDropDown();
            }

            if (IsKeyTipMode && _isTabStage)
            {
                BuildTabStageKeyTips();
            }

            return;
        }

        if (change.Property == IsKeyTipModeProperty && !_handlingKeyTipProperty)
        {
            if (IsKeyTipMode)
            {
                EnterKeyTipMode();
            }
            else
            {
                ExitKeyTipMode();
            }
        }
    }

    protected override AutomationPeer OnCreateAutomationPeer()
        => new RibbonAutomationPeer(this);

    private void OnStaticTabsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RebuildTabs();
    }

    private void RebuildTabs()
    {
        var staticTabs = Tabs.Select(RibbonModelConverter.Clone).ToList();
        var merged = MergePolicy.MergeTabs(staticTabs, TabsSource, TabMergeMode)
            .Where(tab => !tab.IsContextual ||
                          ActiveContextGroupIds.Contains(ResolveContextGroupId(tab), StringComparer.Ordinal))
            .ToList();

        ResolveMergedTabCommands(merged);

        if (RuntimeState is not null)
        {
            merged = CustomizationService.ApplyState(merged, RuntimeState).ToList();
            ResolveMergedTabCommands(merged);
        }

        ReleaseMergedTabBindings();
        _mergedTabs.ReplaceWith(merged);
        RewireDropDownHandlers();
        RewireGroupDropDownHandlers();

        if (_tabControl is not null)
        {
            _tabControl.ItemsSource = MergedTabs;
        }

        UpdateContextualBand();
        SyncSelectedTabFromState();

        if (IsKeyTipMode)
        {
            if (_isTabStage)
            {
                BuildTabStageKeyTips();
            }
            else
            {
                BuildCommandStageKeyTips();
            }
        }

        UpdateAdaptiveLayout();
    }

    private void ReleaseMergedTabBindings()
    {
        foreach (var tab in _mergedTabs)
        {
            foreach (var group in tab.MergedGroups)
            {
                group.ReleaseMergedItemBindings();
            }
        }
    }

    private void SyncSelectedTabFromState()
    {
        if (_isSynchronizingSelection)
        {
            return;
        }

        _isSynchronizingSelection = true;
        try
        {
            if (string.IsNullOrWhiteSpace(SelectedTabId))
            {
                var fallback = MergedTabs.FirstOrDefault();
                SelectedTab = fallback;

                if (fallback is not null)
                {
                    SelectedTabId = fallback.Id;
                }

                return;
            }

            var selected = MergedTabs.FirstOrDefault(tab => string.Equals(tab.Id, SelectedTabId, StringComparison.Ordinal));
            if (selected is not null)
            {
                SelectedTab = selected;
                return;
            }

            if (MergedTabs.Count == 0)
            {
                SelectedTab = null;
                return;
            }

            if (SelectedTab is null || !MergedTabs.Contains(SelectedTab))
            {
                SelectedTab = MergedTabs[0];
            }
        }
        finally
        {
            _isSynchronizingSelection = false;

            if (_minimizedDropDownContentHost is not null)
            {
                _minimizedDropDownContentHost.Content = SelectedTab;
            }

            if (IsMinimizedDropDownOpen && SelectedTab is null)
            {
                CloseMinimizedDropDown();
            }
        }

        UpdateAdaptiveLayout();
    }

    private void SyncSelectedTabIdFromSelection()
    {
        if (_isSynchronizingSelection)
        {
            return;
        }

        _isSynchronizingSelection = true;
        try
        {
            if (!string.IsNullOrWhiteSpace(SelectedTabId) &&
                MergedTabs.All(tab => !string.Equals(tab.Id, SelectedTabId, StringComparison.Ordinal)))
            {
                return;
            }

            var selectedId = SelectedTab?.Id;
            if (!string.Equals(SelectedTabId, selectedId, StringComparison.Ordinal))
            {
                SelectedTabId = selectedId;
            }
        }
        finally
        {
            _isSynchronizingSelection = false;
        }
    }

    private void ResolveMergedTabCommands(IEnumerable<RibbonTab> tabs)
    {
        foreach (var tab in tabs)
        {
            foreach (var group in tab.MergedGroups)
            {
                foreach (var item in group.MergedItems)
                {
                    ResolveCommand(item);
                }
            }
        }
    }

    private void ResolveCommand(RibbonItem item)
    {
        var commandCatalog = CommandCatalog;

        if (commandCatalog is not null &&
            item.Command is null &&
            !string.IsNullOrWhiteSpace(item.CommandId) &&
            commandCatalog.TryResolve(item.CommandId, out var resolvedCommand, out var resolvedParameter))
        {
            item.Command = resolvedCommand;
            item.CommandParameter = resolvedParameter;
        }

        if (commandCatalog is not null &&
            item.SecondaryCommand is null &&
            !string.IsNullOrWhiteSpace(item.SecondaryCommandId) &&
            commandCatalog.TryResolve(item.SecondaryCommandId, out var resolvedSecondaryCommand, out var resolvedSecondaryParameter))
        {
            item.SecondaryCommand = resolvedSecondaryCommand;
            item.SecondaryCommandParameter = resolvedSecondaryParameter;
        }

        ResolveMenuItemCommands(item, item.MenuItems, commandCatalog);

        foreach (var child in item.Items)
        {
            ResolveCommand(child);
        }
    }

    private static void ResolveMenuItemCommands(
        RibbonItem ownerItem,
        IEnumerable<RibbonMenuItem> menuItems,
        IRibbonCommandCatalog? commandCatalog)
    {
        foreach (var menuItem in menuItems)
        {
            if (commandCatalog is not null &&
                menuItem.Command is null &&
                !string.IsNullOrWhiteSpace(menuItem.CommandId) &&
                commandCatalog.TryResolve(menuItem.CommandId, out var resolvedMenuCommand, out var resolvedMenuParameter))
            {
                menuItem.Command = resolvedMenuCommand;
                menuItem.CommandParameter = resolvedMenuParameter;
            }

            if (menuItem.Command is not null && menuItem.Command is not PopupClosingCommand)
            {
                menuItem.Command = new PopupClosingCommand(menuItem.Command, () => ownerItem.IsDropDownOpen = false);
            }

            if (menuItem.SubMenuItems.Count > 0)
            {
                ResolveMenuItemCommands(ownerItem, menuItem.SubMenuItems, commandCatalog);
            }
        }
    }

    private static IEnumerable<RibbonItem> EnumerateNestedItems(IEnumerable<RibbonItem> items)
    {
        foreach (var item in items)
        {
            yield return item;

            foreach (var child in EnumerateNestedItems(item.Items))
            {
                yield return child;
            }
        }
    }

    private void RewireDropDownHandlers()
    {
        DetachDropDownHandlers();

        foreach (var tab in MergedTabs)
        {
            foreach (var group in tab.MergedGroups)
            {
                foreach (var item in EnumerateNestedItems(group.MergedItems))
                {
                    var capturedItem = item;
                    PropertyChangedEventHandler handler = (_, args) =>
                    {
                        if (args.PropertyName == nameof(RibbonItem.IsDropDownOpen) && capturedItem.IsDropDownOpen)
                        {
                            CloseOtherDropDowns(capturedItem);
                        }
                    };

                    capturedItem.PropertyChanged += handler;
                    _dropDownHandlers[capturedItem] = handler;
                }
            }
        }
    }

    private void DetachDropDownHandlers()
    {
        foreach (var pair in _dropDownHandlers)
        {
            pair.Key.PropertyChanged -= pair.Value;
        }

        _dropDownHandlers.Clear();
    }

    private void RewireGroupDropDownHandlers()
    {
        DetachGroupDropDownHandlers();

        foreach (var tab in MergedTabs)
        {
            foreach (var group in tab.MergedGroups)
            {
                var capturedGroup = group;
                PropertyChangedEventHandler handler = (_, args) =>
                {
                    if (args.PropertyName == nameof(RibbonGroup.IsDropDownOpen) && capturedGroup.IsDropDownOpen)
                    {
                        CloseOtherGroupDropDowns(capturedGroup);
                    }

                    if (args.PropertyName == nameof(RibbonGroup.DisplayMode) && !capturedGroup.IsCollapsedMode && capturedGroup.IsDropDownOpen)
                    {
                        capturedGroup.IsDropDownOpen = false;
                    }
                };

                capturedGroup.PropertyChanged += handler;
                _groupDropDownHandlers[capturedGroup] = handler;
            }
        }
    }

    private void DetachGroupDropDownHandlers()
    {
        foreach (var pair in _groupDropDownHandlers)
        {
            pair.Key.PropertyChanged -= pair.Value;
        }

        _groupDropDownHandlers.Clear();
    }

    private void CloseOtherGroupDropDowns(RibbonGroup openedGroup)
    {
        foreach (var candidate in _groupDropDownHandlers.Keys)
        {
            if (ReferenceEquals(candidate, openedGroup) || !candidate.IsDropDownOpen)
            {
                continue;
            }

            candidate.IsDropDownOpen = false;
        }
    }

    private void CloseOtherDropDowns(RibbonItem openedItem)
    {
        foreach (var candidate in _dropDownHandlers.Keys)
        {
            if (ReferenceEquals(candidate, openedItem) || !candidate.IsDropDownOpen)
            {
                continue;
            }

            candidate.IsDropDownOpen = false;
        }
    }

    private void UpdateAdaptiveLayout()
    {
        if (_isApplyingAdaptiveLayout)
        {
            return;
        }

        var selectedTab = SelectedTab;
        if (selectedTab is null)
        {
            return;
        }

        if (!EnableAdaptiveLayout)
        {
            ResetAdaptiveLayout(selectedTab);
            return;
        }

        var availableWidth = _tabControl?.Bounds.Width > 0
            ? _tabControl!.Bounds.Width
            : Bounds.Width;

        availableWidth = Math.Max(0, availableWidth - AdaptiveLayoutHorizontalPadding);

        _isApplyingAdaptiveLayout = true;
        try
        {
            var changed = AdaptiveLayoutEngine.ApplyLayout(selectedTab.MergedGroups, availableWidth);
            if (!changed)
            {
                return;
            }

            if (IsKeyTipMode && !_isTabStage)
            {
                BuildCommandStageKeyTips();
            }
        }
        finally
        {
            _isApplyingAdaptiveLayout = false;
        }
    }

    private static void ResetAdaptiveLayout(RibbonTab tab)
    {
        foreach (var group in tab.MergedGroups)
        {
            if (group.DisplayMode != RibbonGroupDisplayMode.Expanded)
            {
                group.DisplayMode = RibbonGroupDisplayMode.Expanded;
            }

            if (group.IsDropDownOpen)
            {
                group.IsDropDownOpen = false;
            }
        }
    }

    private void ApplySynchronizedCommandHeights()
    {
        List<(Control Control, bool IsLarge)>? targets = null;
        var observedLargeHeight = 0d;
        var observedSmallHeight = 0d;

        foreach (var control in this.GetVisualDescendants().OfType<Control>())
        {
            if (!TryResolveSynchronizedHeightTarget(control, out var largeTarget))
            {
                continue;
            }

            (targets ??= []).Add((control, largeTarget));

            if (!SynchronizeCommandHeights)
            {
                control.ClearValue(Layoutable.HeightProperty);
                control.ClearValue(Layoutable.MinHeightProperty);
                continue;
            }

            if (!AutoSynchronizeCommandHeights)
            {
                continue;
            }

            var observedHeight = ResolveObservedHeight(control);
            if (observedHeight <= 0)
            {
                continue;
            }

            if (largeTarget)
            {
                observedLargeHeight = Math.Max(observedLargeHeight, observedHeight);
            }
            else
            {
                observedSmallHeight = Math.Max(observedSmallHeight, observedHeight);
            }
        }

        if (!SynchronizeCommandHeights || targets is null || targets.Count == 0)
        {
            return;
        }

        if (AutoSynchronizeCommandHeights)
        {
            if (observedLargeHeight > _stableLargeCommandHeight)
            {
                _stableLargeCommandHeight = observedLargeHeight;
            }

            if (observedSmallHeight > _stableSmallCommandHeight)
            {
                _stableSmallCommandHeight = observedSmallHeight;
            }
        }

        var largeTargetHeight = EffectiveSynchronizedLargeCommandHeight;
        var smallTargetHeight = EffectiveSynchronizedSmallCommandHeight;

        foreach (var (control, isLarge) in targets)
        {
            var targetHeight = isLarge ? largeTargetHeight : smallTargetHeight;
            if (targetHeight <= 0)
            {
                control.ClearValue(Layoutable.HeightProperty);
                control.ClearValue(Layoutable.MinHeightProperty);
                continue;
            }

            if (!AreClose(control.Height, targetHeight))
            {
                control.Height = targetHeight;
            }

            if (!AreClose(control.MinHeight, targetHeight))
            {
                control.MinHeight = targetHeight;
            }
        }
    }

    private void ResetSynchronizedCommandHeightCache()
    {
        _stableLargeCommandHeight = 0;
        _stableSmallCommandHeight = 0;
    }

    private void UpdateStableRibbonHeight()
    {
        if (_tabControl is null)
        {
            return;
        }

        if (!MaintainStableRibbonHeight)
        {
            _tabControl.ClearValue(Layoutable.MinHeightProperty);
            _stableRibbonContentHeight = 0;
            return;
        }

        if (IsMinimized)
        {
            _tabControl.ClearValue(Layoutable.MinHeightProperty);
            return;
        }

        var measured = _tabControl.Bounds.Height;
        if (measured <= 0)
        {
            var floor = StableRibbonMinHeight;
            if (_stableRibbonContentHeight < floor)
            {
                _stableRibbonContentHeight = floor;
                _tabControl.MinHeight = _stableRibbonContentHeight;
            }

            return;
        }

        var candidate = Math.Max(StableRibbonMinHeight, measured);
        if (candidate > _stableRibbonContentHeight)
        {
            _stableRibbonContentHeight = candidate;
        }

        if (!AreClose(_tabControl.MinHeight, _stableRibbonContentHeight))
        {
            _tabControl.MinHeight = _stableRibbonContentHeight;
        }
    }

    private static bool TryResolveSynchronizedHeightTarget(Control control, out bool largeTarget)
    {
        if (control.Classes.Contains("ribbon-command-small") ||
            control.Classes.Contains("ribbon-small") ||
            control.Classes.Contains("ribbon-split-main-small") ||
            control.Classes.Contains("ribbon-icon-only-small") ||
            control.Classes.Contains("ribbon-toggle-small") ||
            control.Classes.Contains("ribbon-toggle-icon-only-small"))
        {
            largeTarget = false;
            return true;
        }

        if (control.Classes.Contains("ribbon-command-large") ||
            control.Classes.Contains("ribbon-large") ||
            control.Classes.Contains("ribbon-menu") ||
            control.Classes.Contains("ribbon-split-main"))
        {
            largeTarget = true;
            return true;
        }

        largeTarget = false;
        return false;
    }

    private static bool AreClose(double left, double right)
    {
        return Math.Abs(left - right) < 0.01;
    }

    private static double ResolveObservedHeight(Control control)
    {
        if (control.Bounds.Height > 0)
        {
            return control.Bounds.Height;
        }

        if (control.DesiredSize.Height > 0)
        {
            return control.DesiredSize.Height;
        }

        return control.MinHeight > 0 ? control.MinHeight : 0;
    }

    private void HandleKeyTipInput(KeyEventArgs e)
    {
        if (e.Key == Key.LeftAlt || e.Key == Key.RightAlt)
        {
            _handlingKeyTipProperty = true;
            try
            {
                IsKeyTipMode = !IsKeyTipMode;
            }
            finally
            {
                _handlingKeyTipProperty = false;
            }

            if (IsKeyTipMode)
            {
                EnterKeyTipMode();
            }
            else
            {
                ExitKeyTipMode();
            }

            e.Handled = true;
            return;
        }

        if (!IsKeyTipMode)
        {
            return;
        }

        if (e.Key == Key.Escape)
        {
            _handlingKeyTipProperty = true;
            try
            {
                IsKeyTipMode = false;
            }
            finally
            {
                _handlingKeyTipProperty = false;
            }

            ExitKeyTipMode();
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Back)
        {
            if (KeyTipSequence.Length > 0)
            {
                KeyTipSequence = KeyTipSequence[..^1];
                ApplyKeyTipHints(_keyTipService.GetMatches(KeyTipSequence));
            }
            else if (!_isTabStage)
            {
                BuildTabStageKeyTips();
            }
            else
            {
                _handlingKeyTipProperty = true;
                try
                {
                    IsKeyTipMode = false;
                }
                finally
                {
                    _handlingKeyTipProperty = false;
                }

                ExitKeyTipMode();
            }

            e.Handled = true;
            return;
        }

        var input = GetKeyTipCharacter(e.Key);
        if (input is null)
        {
            return;
        }

        var nextSequence = KeyTipSequence + input.Value;
        if (_keyTipService.TryResolve(nextSequence, out var resolvedItem) && resolvedItem is not null)
        {
            KeyTipSequence = string.Empty;
            HandleResolvedKeyTip(resolvedItem);
            e.Handled = true;
            return;
        }

        if (_keyTipService.HasMatches(nextSequence))
        {
            KeyTipSequence = nextSequence;
            ApplyKeyTipHints(_keyTipService.GetMatches(KeyTipSequence));
            e.Handled = true;
            return;
        }

        var singleSequence = input.Value.ToString();
        if (_keyTipService.HasMatches(singleSequence))
        {
            KeyTipSequence = singleSequence;
            ApplyKeyTipHints(_keyTipService.GetMatches(KeyTipSequence));
            e.Handled = true;
            return;
        }

        KeyTipSequence = string.Empty;
        ApplyKeyTipHints(_keyTipService.GetMatches(KeyTipSequence));
        e.Handled = true;
    }

    private void EnterKeyTipMode()
    {
        BuildTabStageKeyTips();
    }

    private void ExitKeyTipMode()
    {
        _keyTipService.ExitMode();
        _isTabStage = true;
        ActiveKeyTips = EmptyKeyTips;
        KeyTipSequence = string.Empty;
        RestoreToolTips();
    }

    private void BuildTabStageKeyTips()
    {
        _isTabStage = true;
        KeyTipSequence = string.Empty;

        var nodes = new List<IRibbonItemNode>();
        var order = 0;

        if (Backstage is not null)
        {
            nodes.Add(new RibbonItem
            {
                Id = "__backstage",
                Label = "File",
                Order = order++,
                KeyTip = "F",
            });
        }

        foreach (var tab in MergedTabs)
        {
            nodes.Add(new RibbonItem
            {
                Id = $"tab:{tab.Id}",
                Label = tab.Header,
                Order = order++,
            });
        }

        foreach (var item in _resolvedQuickAccessItems)
        {
            nodes.Add(new RibbonItem
            {
                Id = $"qat:{item.Id}",
                Label = item.Label,
                Order = order++,
                Command = item.Command,
                CommandId = item.CommandId,
                CommandParameter = item.CommandParameter,
                KeyTip = item.KeyTip,
                ScreenTip = item.ScreenTip,
            });
        }

        _keyTipService.EnterMode(nodes);
        ActiveKeyTips = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(_keyTipService.ActiveTips));
        ApplyKeyTipHints(_keyTipService.GetMatches(string.Empty));
    }

    private void BuildCommandStageKeyTips()
    {
        var selected = MergedTabs.FirstOrDefault(tab => string.Equals(tab.Id, SelectedTabId, StringComparison.Ordinal))
            ?? MergedTabs.FirstOrDefault();

        if (selected is null)
        {
            BuildTabStageKeyTips();
            return;
        }

        var items = selected.MergedGroups
            .Where(group => group.IsVisible)
            .SelectMany(group => EnumerateNestedItems(group.MergedItems))
            .Where(item =>
                item.IsVisible &&
                !item.IsGroupPrimitive &&
                !item.IsRowPrimitive &&
                !string.IsNullOrWhiteSpace(item.Id))
            .Cast<IRibbonItemNode>()
            .ToList();

        if (items.Count == 0)
        {
            BuildTabStageKeyTips();
            return;
        }

        _isTabStage = false;
        KeyTipSequence = string.Empty;
        _keyTipService.EnterMode(items);
        ActiveKeyTips = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(_keyTipService.ActiveTips));
        ApplyKeyTipHints(_keyTipService.GetMatches(string.Empty));
    }

    private void HandleResolvedKeyTip(IRibbonItemNode resolvedItem)
    {
        if (string.Equals(resolvedItem.Id, "__backstage", StringComparison.Ordinal))
        {
            ToggleBackstage();
            _handlingKeyTipProperty = true;
            try
            {
                IsKeyTipMode = false;
            }
            finally
            {
                _handlingKeyTipProperty = false;
            }

            ExitKeyTipMode();
            return;
        }

        if (resolvedItem.Id.StartsWith("tab:", StringComparison.Ordinal))
        {
            SelectedTabId = resolvedItem.Id[4..];
            BuildCommandStageKeyTips();
            return;
        }

        if (resolvedItem.Id.StartsWith("qat:", StringComparison.Ordinal))
        {
            ExecuteItem(resolvedItem);
            _handlingKeyTipProperty = true;
            try
            {
                IsKeyTipMode = false;
            }
            finally
            {
                _handlingKeyTipProperty = false;
            }

            ExitKeyTipMode();
            return;
        }

        ExecuteItem(resolvedItem);
        _handlingKeyTipProperty = true;
        try
        {
            IsKeyTipMode = false;
        }
        finally
        {
            _handlingKeyTipProperty = false;
        }

        ExitKeyTipMode();
    }

    private void ExecuteItem(IRibbonItemNode item)
    {
        var command = item.Command;
        var parameter = item.CommandParameter;

        if (command is null &&
            CommandCatalog is not null &&
            !string.IsNullOrWhiteSpace(item.CommandId) &&
            CommandCatalog.TryResolve(item.CommandId, out var resolvedCommand, out var resolvedParameter))
        {
            command = resolvedCommand;
            parameter = resolvedParameter;
        }

        if (command is not null && command.CanExecute(parameter))
        {
            command.Execute(parameter);
        }
    }

    private void ApplyKeyTipHints(IReadOnlyDictionary<string, string> keyTipMatches)
    {
        if (!IsKeyTipMode)
        {
            RestoreToolTips();
            return;
        }

        foreach (var control in this.GetVisualDescendants().OfType<Control>())
        {
            if (control.Tag is not string id || string.IsNullOrWhiteSpace(id))
            {
                continue;
            }

            if (keyTipMatches.TryGetValue(id, out var tip))
            {
                if (!_originalToolTips.ContainsKey(control))
                {
                    _originalToolTips[control] = ToolTip.GetTip(control);
                }

                var existing = _originalToolTips[control]?.ToString();
                var tooltip = string.IsNullOrWhiteSpace(existing)
                    ? $"KeyTip: {tip}"
                    : $"{existing}\nKeyTip: {tip}";

                ToolTip.SetTip(control, tooltip);
                control.Classes.Add("ribbon-keytip-active");
            }
            else if (_originalToolTips.TryGetValue(control, out var original))
            {
                ToolTip.SetTip(control, original);
                control.Classes.Remove("ribbon-keytip-active");
            }
        }
    }

    private void RestoreToolTips()
    {
        foreach (var pair in _originalToolTips)
        {
            ToolTip.SetTip(pair.Key, pair.Value);
            pair.Key.Classes.Remove("ribbon-keytip-active");
        }

        _originalToolTips.Clear();
    }

    private void UpdateQuickAccessRows()
    {
        RebuildResolvedQuickAccessItems();

        if (_quickAccessTopRow is null || _quickAccessBottomRow is null)
        {
            return;
        }

        var showTop = QuickAccessPlacement == RibbonQuickAccessPlacement.Above;
        _quickAccessTopRow.IsVisible = showTop;
        _quickAccessBottomRow.IsVisible = !showTop;

        if (_quickAccessTopToolbar is not null)
        {
            _quickAccessTopToolbar.ItemsSource = _resolvedQuickAccessItems;
            _quickAccessTopToolbar.Placement = RibbonQuickAccessPlacement.Above;
        }

        if (_quickAccessBottomToolbar is not null)
        {
            _quickAccessBottomToolbar.ItemsSource = _resolvedQuickAccessItems;
            _quickAccessBottomToolbar.Placement = RibbonQuickAccessPlacement.Below;
        }
    }

    private void RebuildResolvedQuickAccessItems()
    {
        var resolved = new List<RibbonItem>();
        foreach (var item in QuickAccessItems ?? Enumerable.Empty<RibbonItem>())
        {
            var clone = RibbonModelConverter.Clone(item);
            ResolveCommand(clone);
            resolved.Add(clone);
        }

        _resolvedQuickAccessItems.ReplaceWith(resolved);
    }

    private void UpdateQuickAccessItemsSubscription()
    {
        DetachQuickAccessItemsSubscription();

        _quickAccessItemsNotifier = QuickAccessItems as INotifyCollectionChanged;
        if (_quickAccessItemsNotifier is not null)
        {
            _quickAccessItemsNotifier.CollectionChanged += OnQuickAccessItemsCollectionChanged;
        }
    }

    private void DetachQuickAccessItemsSubscription()
    {
        if (_quickAccessItemsNotifier is not null)
        {
            _quickAccessItemsNotifier.CollectionChanged -= OnQuickAccessItemsCollectionChanged;
            _quickAccessItemsNotifier = null;
        }
    }

    private void OnQuickAccessItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RebuildResolvedQuickAccessItems();

        if (IsKeyTipMode && _isTabStage)
        {
            BuildTabStageKeyTips();
        }
    }

    private void UpdateContextualBand()
    {
        if (_contextualBand is null)
        {
            return;
        }

        var contextualGroups = MergedTabs
            .Where(tab => tab.IsContextual)
            .GroupBy(tab => ResolveContextGroupId(tab), StringComparer.Ordinal)
            .Select(group =>
            {
                var orderedTabs = group
                    .OrderBy(tab => tab.Order)
                    .ThenBy(tab => tab.Id, StringComparer.Ordinal)
                    .ToList();
                var firstTab = orderedTabs[0];
                var header = orderedTabs
                    .Select(tab => tab.ContextGroupHeader)
                    .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value))
                    ?? firstTab.ContextGroupId
                    ?? firstTab.Header;
                var accentColor = orderedTabs
                    .Select(tab => tab.ContextGroupAccentColor)
                    .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));
                var order = orderedTabs
                    .Select(tab => tab.ContextGroupOrder)
                    .FirstOrDefault(value => value.HasValue)
                    ?? firstTab.Order;

                return new RibbonContextualTabGroup(
                    id: group.Key,
                    header: header,
                    order: order,
                    accentColor: accentColor);
            })
            .OrderBy(group => group.Order)
            .ThenBy(group => group.Id, StringComparer.Ordinal)
            .ToList();

        _contextualBand.ContextGroups = contextualGroups;
        _contextualBand.IsVisible = contextualGroups.Count > 0;
    }

    private static string ResolveContextGroupId(RibbonTab tab)
    {
        if (!string.IsNullOrWhiteSpace(tab.ContextGroupId))
        {
            return tab.ContextGroupId;
        }

        return $"tab:{tab.Id}";
    }

    private void ToggleBackstage()
    {
        if (Backstage is null)
        {
            return;
        }

        Backstage.IsOpen = !Backstage.IsOpen;
        UpdateBackstageHost();
    }

    private void AttachBackstage(RibbonBackstage? backstage)
    {
        if (_attachedBackstage is not null)
        {
            _attachedBackstage.PropertyChanged -= OnBackstagePropertyChanged;
        }

        _attachedBackstage = backstage;

        if (_attachedBackstage is not null)
        {
            _attachedBackstage.PropertyChanged += OnBackstagePropertyChanged;
        }
    }

    private void OnBackstagePropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == RibbonBackstage.IsOpenProperty ||
            e.Property == ContentControl.ContentProperty ||
            e.Property == RibbonBackstage.ItemsSourceProperty ||
            e.Property == RibbonBackstage.ItemTemplateProperty ||
            e.Property == RibbonBackstage.SelectedItemProperty)
        {
            UpdateBackstageHost();
        }
    }

    private void UpdateBackstageHost()
    {
        if (_backstageHost is null || _backstageContentHost is null)
        {
            return;
        }

        if (Backstage is null)
        {
            _backstageHost.IsVisible = false;
            _backstageContentHost.Content = null;

            if (_backstageTopButton is not null)
            {
                _backstageTopButton.IsVisible = false;
            }

            if (_backstageBottomButton is not null)
            {
                _backstageBottomButton.IsVisible = false;
            }

            return;
        }

        if (_backstageTopButton is not null)
        {
            _backstageTopButton.IsVisible = true;
        }

        if (_backstageBottomButton is not null)
        {
            _backstageBottomButton.IsVisible = true;
        }

        _backstageHost.IsVisible = Backstage.IsOpen;
        _backstageContentHost.Content = Backstage;

        if (Backstage.IsOpen)
        {
            CloseMinimizedDropDown();
        }
    }

    private void UpdateMinimizedVisualState()
    {
        if (_tabControl is not null)
        {
            if (IsMinimized)
            {
                _tabControl.Classes.Add("ribbon-minimized");
            }
            else
            {
                _tabControl.Classes.Remove("ribbon-minimized");
            }
        }

        if (_minimizedDropDownContentHost is not null)
        {
            _minimizedDropDownContentHost.Content = SelectedTab;
        }

        if (!IsMinimized)
        {
            CloseMinimizedDropDown();
        }
    }

    private void OpenMinimizedDropDown()
    {
        if (!IsMinimized ||
            Backstage?.IsOpen == true ||
            SelectedTab is null)
        {
            return;
        }

        if (_minimizedDropDownContentHost is not null)
        {
            _minimizedDropDownContentHost.Content = SelectedTab;
        }

        if (_minimizedDropDownHost is not null)
        {
            _minimizedDropDownHost.IsVisible = true;
        }

        IsMinimizedDropDownOpen = true;
    }

    private void CloseMinimizedDropDown()
    {
        if (_minimizedDropDownHost is not null)
        {
            _minimizedDropDownHost.IsVisible = false;
        }

        IsMinimizedDropDownOpen = false;
    }

    private void ApplyControlStateFromRuntimeState(RibbonRuntimeState state)
    {
        if (StateOwnershipMode == RibbonStateOwnershipMode.Internal)
        {
            return;
        }

        _isApplyingRuntimeState = true;
        try
        {
            SelectedTabId = state.SelectedTabId;
            IsMinimized = state.IsMinimized;
            IsKeyTipMode = state.IsKeyTipMode;
            QuickAccessPlacement = state.QuickAccessPlacement;
            ActiveContextGroupIds = [.. state.ActiveContextGroupIds];
        }
        finally
        {
            _isApplyingRuntimeState = false;
        }
    }

    private RibbonRuntimeState BuildRuntimeStateSnapshot()
    {
        var seed = RuntimeState;
        var snapshot = CustomizationService.ExportState(MergedTabs, seed);

        if (StateOwnershipMode != RibbonStateOwnershipMode.External || seed is null)
        {
            snapshot.SelectedTabId = SelectedTabId;
            snapshot.IsMinimized = IsMinimized;
            snapshot.IsKeyTipMode = IsKeyTipMode;
            snapshot.QuickAccessPlacement = QuickAccessPlacement;
            snapshot.ActiveContextGroupIds = [.. ActiveContextGroupIds];
        }

        return snapshot;
    }

    private void ResetRuntimeStateCore()
    {
        RuntimeState = null;

        if (StateOwnershipMode != RibbonStateOwnershipMode.External)
        {
            _isApplyingRuntimeState = true;
            try
            {
                SelectedTabId = null;
                IsMinimized = false;
                IsKeyTipMode = false;
                QuickAccessPlacement = RibbonQuickAccessPlacement.Above;
                ActiveContextGroupIds = [];
            }
            finally
            {
                _isApplyingRuntimeState = false;
            }
        }

        RebuildTabs();
        UpdateQuickAccessRows();

        if (IsKeyTipMode)
        {
            EnterKeyTipMode();
        }
        else
        {
            ExitKeyTipMode();
        }
    }

    private void SynchronizeRuntimeStateFromLive(AvaloniaProperty property)
    {
        if (RuntimeState is null ||
            _isApplyingRuntimeState ||
            StateOwnershipMode != RibbonStateOwnershipMode.Synchronized)
        {
            return;
        }

        if (property == SelectedTabIdProperty)
        {
            RuntimeState.SelectedTabId = SelectedTabId;
            return;
        }

        if (property == IsMinimizedProperty)
        {
            RuntimeState.IsMinimized = IsMinimized;
            return;
        }

        if (property == IsKeyTipModeProperty)
        {
            RuntimeState.IsKeyTipMode = IsKeyTipMode;
            return;
        }

        if (property == QuickAccessPlacementProperty)
        {
            RuntimeState.QuickAccessPlacement = QuickAccessPlacement;
            return;
        }

        if (property == ActiveContextGroupIdsProperty)
        {
            RuntimeState.ActiveContextGroupIds = [.. ActiveContextGroupIds];
        }
    }

    private void SynchronizeRuntimeStateFromControl()
    {
        if (RuntimeState is null)
        {
            return;
        }

        RuntimeState.SelectedTabId = SelectedTabId;
        RuntimeState.IsMinimized = IsMinimized;
        RuntimeState.IsKeyTipMode = IsKeyTipMode;
        RuntimeState.QuickAccessPlacement = QuickAccessPlacement;
        RuntimeState.ActiveContextGroupIds = [.. ActiveContextGroupIds];
    }

    private static RibbonRuntimeState CloneRuntimeState(RibbonRuntimeState source)
    {
        return new RibbonRuntimeState
        {
            SchemaVersion = source.SchemaVersion,
            SelectedTabId = source.SelectedTabId,
            IsMinimized = source.IsMinimized,
            IsKeyTipMode = source.IsKeyTipMode,
            QuickAccessPlacement = source.QuickAccessPlacement,
            ActiveContextGroupIds = [.. source.ActiveContextGroupIds],
            NodeCustomizations = source.NodeCustomizations.Select(CloneNodeCustomization).ToList(),
        };
    }

    private static RibbonNodeCustomization CloneNodeCustomization(RibbonNodeCustomization source)
    {
        return new RibbonNodeCustomization
        {
            Id = source.Id,
            ParentId = source.ParentId,
            IsHidden = source.IsHidden,
            Order = source.Order,
        };
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

    private bool TryMoveRibbonFocus(KeyModifiers keyModifiers)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is not IInputRoot inputRoot)
        {
            return false;
        }

        var focusManager = inputRoot.FocusManager;
        var navigationHandler = inputRoot.KeyboardNavigationHandler;
        var focused = focusManager?.GetFocusedElement();
        if (focusManager is null ||
            navigationHandler is null ||
            focused is null)
        {
            return false;
        }

        var direction = keyModifiers.HasFlag(KeyModifiers.Shift)
            ? NavigationDirection.Previous
            : NavigationDirection.Next;

        navigationHandler.Move(focused, direction, keyModifiers);

        var updated = focusManager.GetFocusedElement();
        return updated is not null && !ReferenceEquals(updated, focused);
    }

    private static bool TryGetTabIdFromPointerSource(Visual source, out string tabId)
    {
        for (var current = source; current is not null; current = current.GetVisualParent())
        {
            if (current is TabItem { DataContext: RibbonTab { Id: { Length: > 0 } ribbonTabId } })
            {
                tabId = ribbonTabId;
                return true;
            }

            if (current is not Control { Tag: string taggedId } ||
                !taggedId.StartsWith("tab:", StringComparison.Ordinal))
            {
                continue;
            }

            var candidate = taggedId[4..];
            if (!string.IsNullOrWhiteSpace(candidate))
            {
                tabId = candidate;
                return true;
            }
        }

        tabId = string.Empty;
        return false;
    }

    private static char? GetKeyTipCharacter(Key key)
    {
        if (key >= Key.A && key <= Key.Z)
        {
            return (char)('A' + (key - Key.A));
        }

        if (key >= Key.D0 && key <= Key.D9)
        {
            return (char)('0' + (key - Key.D0));
        }

        if (key >= Key.NumPad0 && key <= Key.NumPad9)
        {
            return (char)('0' + (key - Key.NumPad0));
        }

        return null;
    }

    private sealed class PopupClosingCommand : ICommand
    {
        private readonly ICommand _inner;
        private readonly Action _onExecuted;

        public PopupClosingCommand(ICommand inner, Action onExecuted)
        {
            _inner = inner;
            _onExecuted = onExecuted;
        }

        public event EventHandler? CanExecuteChanged
        {
            add => _inner.CanExecuteChanged += value;
            remove => _inner.CanExecuteChanged -= value;
        }

        public bool CanExecute(object? parameter) => _inner.CanExecute(parameter);

        public void Execute(object? parameter)
        {
            if (!_inner.CanExecute(parameter))
            {
                return;
            }

            try
            {
                _inner.Execute(parameter);
            }
            finally
            {
                _onExecuted();
            }
        }
    }

    private sealed class DelegateCommand : ICommand
    {
        private readonly Action<object?> _execute;

        public DelegateCommand(Action<object?> execute)
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

    private sealed class AsyncDelegateCommand : ICommand
    {
        private readonly Func<object?, Task> _execute;

        public AsyncDelegateCommand(Func<object?, Task> execute)
        {
            _execute = execute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { }
            remove { }
        }

        public bool CanExecute(object? parameter) => true;

        public async void Execute(object? parameter)
        {
            await _execute(parameter).ConfigureAwait(false);
        }
    }
}
