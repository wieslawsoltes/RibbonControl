// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Globalization;
using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;

namespace RibbonControl.Core.Controls;

public class RibbonIconPresenter : TemplatedControl
{
    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<RibbonIconPresenter, object?>(nameof(Icon));

    public static readonly StyledProperty<object?> IconResourceKeyProperty =
        AvaloniaProperty.Register<RibbonIconPresenter, object?>(nameof(IconResourceKey));

    public static readonly StyledProperty<string?> IconPathDataProperty =
        AvaloniaProperty.Register<RibbonIconPresenter, string?>(nameof(IconPathData));

    public static readonly StyledProperty<string?> IconEmojiProperty =
        AvaloniaProperty.Register<RibbonIconPresenter, string?>(nameof(IconEmoji));

    public static readonly StyledProperty<Stretch> IconStretchProperty =
        AvaloniaProperty.Register<RibbonIconPresenter, Stretch>(nameof(IconStretch), Stretch.Uniform);

    public static readonly StyledProperty<StretchDirection> IconStretchDirectionProperty =
        AvaloniaProperty.Register<RibbonIconPresenter, StretchDirection>(nameof(IconStretchDirection), StretchDirection.Both);

    public static readonly StyledProperty<double> IconWidthProperty =
        AvaloniaProperty.Register<RibbonIconPresenter, double>(nameof(IconWidth), double.NaN);

    public static readonly StyledProperty<double> IconHeightProperty =
        AvaloniaProperty.Register<RibbonIconPresenter, double>(nameof(IconHeight), double.NaN);

    public static readonly StyledProperty<double> IconMinWidthProperty =
        AvaloniaProperty.Register<RibbonIconPresenter, double>(nameof(IconMinWidth));

    public static readonly StyledProperty<double> IconMinHeightProperty =
        AvaloniaProperty.Register<RibbonIconPresenter, double>(nameof(IconMinHeight));

    public static readonly StyledProperty<double> IconMaxWidthProperty =
        AvaloniaProperty.Register<RibbonIconPresenter, double>(nameof(IconMaxWidth), double.PositiveInfinity);

    public static readonly StyledProperty<double> IconMaxHeightProperty =
        AvaloniaProperty.Register<RibbonIconPresenter, double>(nameof(IconMaxHeight), double.PositiveInfinity);

    public static readonly StyledProperty<object?> OverlayProperty =
        AvaloniaProperty.Register<RibbonIconPresenter, object?>(nameof(Overlay));

    public static readonly StyledProperty<object?> OverlayResourceKeyProperty =
        AvaloniaProperty.Register<RibbonIconPresenter, object?>(nameof(OverlayResourceKey));

    public static readonly StyledProperty<string?> OverlayPathDataProperty =
        AvaloniaProperty.Register<RibbonIconPresenter, string?>(nameof(OverlayPathData));

    public static readonly StyledProperty<string?> OverlayEmojiProperty =
        AvaloniaProperty.Register<RibbonIconPresenter, string?>(nameof(OverlayEmoji));

    public static readonly StyledProperty<Stretch> OverlayStretchProperty =
        AvaloniaProperty.Register<RibbonIconPresenter, Stretch>(nameof(OverlayStretch), Stretch.Uniform);

    public static readonly StyledProperty<StretchDirection> OverlayStretchDirectionProperty =
        AvaloniaProperty.Register<RibbonIconPresenter, StretchDirection>(nameof(OverlayStretchDirection), StretchDirection.Both);

    public static readonly StyledProperty<double> OverlayWidthProperty =
        AvaloniaProperty.Register<RibbonIconPresenter, double>(nameof(OverlayWidth), double.NaN);

    public static readonly StyledProperty<double> OverlayHeightProperty =
        AvaloniaProperty.Register<RibbonIconPresenter, double>(nameof(OverlayHeight), double.NaN);

    public static readonly StyledProperty<double> OverlayMinWidthProperty =
        AvaloniaProperty.Register<RibbonIconPresenter, double>(nameof(OverlayMinWidth));

    public static readonly StyledProperty<double> OverlayMinHeightProperty =
        AvaloniaProperty.Register<RibbonIconPresenter, double>(nameof(OverlayMinHeight));

    public static readonly StyledProperty<double> OverlayMaxWidthProperty =
        AvaloniaProperty.Register<RibbonIconPresenter, double>(nameof(OverlayMaxWidth), double.PositiveInfinity);

    public static readonly StyledProperty<double> OverlayMaxHeightProperty =
        AvaloniaProperty.Register<RibbonIconPresenter, double>(nameof(OverlayMaxHeight), double.PositiveInfinity);

    public static readonly StyledProperty<HorizontalAlignment> OverlayHorizontalAlignmentProperty =
        AvaloniaProperty.Register<RibbonIconPresenter, HorizontalAlignment>(nameof(OverlayHorizontalAlignment), HorizontalAlignment.Right);

    public static readonly StyledProperty<VerticalAlignment> OverlayVerticalAlignmentProperty =
        AvaloniaProperty.Register<RibbonIconPresenter, VerticalAlignment>(nameof(OverlayVerticalAlignment), VerticalAlignment.Bottom);

    public static readonly StyledProperty<Thickness> OverlayMarginProperty =
        AvaloniaProperty.Register<RibbonIconPresenter, Thickness>(nameof(OverlayMargin));

    public static readonly StyledProperty<int?> OverlayCountProperty =
        AvaloniaProperty.Register<RibbonIconPresenter, int?>(nameof(OverlayCount));

    public static readonly StyledProperty<string?> OverlayCountTextProperty =
        AvaloniaProperty.Register<RibbonIconPresenter, string?>(nameof(OverlayCountText));

    public static readonly StyledProperty<bool> ShowOverlayCountWhenZeroProperty =
        AvaloniaProperty.Register<RibbonIconPresenter, bool>(nameof(ShowOverlayCountWhenZero));

    public static readonly StyledProperty<HorizontalAlignment> OverlayCountHorizontalAlignmentProperty =
        AvaloniaProperty.Register<RibbonIconPresenter, HorizontalAlignment>(nameof(OverlayCountHorizontalAlignment), HorizontalAlignment.Right);

    public static readonly StyledProperty<VerticalAlignment> OverlayCountVerticalAlignmentProperty =
        AvaloniaProperty.Register<RibbonIconPresenter, VerticalAlignment>(nameof(OverlayCountVerticalAlignment), VerticalAlignment.Bottom);

    public static readonly StyledProperty<Thickness> OverlayCountMarginProperty =
        AvaloniaProperty.Register<RibbonIconPresenter, Thickness>(nameof(OverlayCountMargin));

    public static readonly DirectProperty<RibbonIconPresenter, object?> ResolvedIconContentProperty =
        AvaloniaProperty.RegisterDirect<RibbonIconPresenter, object?>(
            nameof(ResolvedIconContent),
            owner => owner.ResolvedIconContent);

    public static readonly DirectProperty<RibbonIconPresenter, object?> ResolvedOverlayContentProperty =
        AvaloniaProperty.RegisterDirect<RibbonIconPresenter, object?>(
            nameof(ResolvedOverlayContent),
            owner => owner.ResolvedOverlayContent);

    public static readonly DirectProperty<RibbonIconPresenter, string?> ResolvedOverlayCountTextProperty =
        AvaloniaProperty.RegisterDirect<RibbonIconPresenter, string?>(
            nameof(ResolvedOverlayCountText),
            owner => owner.ResolvedOverlayCountText);

    public static readonly DirectProperty<RibbonIconPresenter, bool> HasOverlayProperty =
        AvaloniaProperty.RegisterDirect<RibbonIconPresenter, bool>(
            nameof(HasOverlay),
            owner => owner.HasOverlay);

    public static readonly DirectProperty<RibbonIconPresenter, bool> HasOverlayCountProperty =
        AvaloniaProperty.RegisterDirect<RibbonIconPresenter, bool>(
            nameof(HasOverlayCount),
            owner => owner.HasOverlayCount);

    private object? _resolvedIconContent;
    private object? _resolvedOverlayContent;
    private string? _resolvedOverlayCountText;
    private bool _hasOverlay;
    private bool _hasOverlayCount;

    static RibbonIconPresenter()
    {
        IconProperty.Changed.AddClassHandler<RibbonIconPresenter>((owner, _) => owner.UpdateResolvedIconContent());
        IconPathDataProperty.Changed.AddClassHandler<RibbonIconPresenter>((owner, _) => owner.UpdateResolvedIconContent());
        IconEmojiProperty.Changed.AddClassHandler<RibbonIconPresenter>((owner, _) => owner.UpdateResolvedIconContent());
        IconResourceKeyProperty.Changed.AddClassHandler<RibbonIconPresenter>((owner, _) => owner.UpdateResolvedIconContent());

        OverlayProperty.Changed.AddClassHandler<RibbonIconPresenter>((owner, _) => owner.UpdateResolvedOverlayContent());
        OverlayPathDataProperty.Changed.AddClassHandler<RibbonIconPresenter>((owner, _) => owner.UpdateResolvedOverlayContent());
        OverlayEmojiProperty.Changed.AddClassHandler<RibbonIconPresenter>((owner, _) => owner.UpdateResolvedOverlayContent());
        OverlayResourceKeyProperty.Changed.AddClassHandler<RibbonIconPresenter>((owner, _) => owner.UpdateResolvedOverlayContent());

        OverlayCountProperty.Changed.AddClassHandler<RibbonIconPresenter>((owner, _) => owner.UpdateResolvedOverlayCountText());
        OverlayCountTextProperty.Changed.AddClassHandler<RibbonIconPresenter>((owner, _) => owner.UpdateResolvedOverlayCountText());
        ShowOverlayCountWhenZeroProperty.Changed.AddClassHandler<RibbonIconPresenter>((owner, _) => owner.UpdateResolvedOverlayCountText());
    }

    public RibbonIconPresenter()
    {
        ResourcesChanged += OnResourcesChanged;
        ActualThemeVariantChanged += OnActualThemeVariantChanged;
        UpdateResolvedIconContent();
        UpdateResolvedOverlayContent();
        UpdateResolvedOverlayCountText();
    }

    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public object? IconResourceKey
    {
        get => GetValue(IconResourceKeyProperty);
        set => SetValue(IconResourceKeyProperty, value);
    }

    public string? IconPathData
    {
        get => GetValue(IconPathDataProperty);
        set => SetValue(IconPathDataProperty, value);
    }

    public string? IconEmoji
    {
        get => GetValue(IconEmojiProperty);
        set => SetValue(IconEmojiProperty, value);
    }

    public Stretch IconStretch
    {
        get => GetValue(IconStretchProperty);
        set => SetValue(IconStretchProperty, value);
    }

    public StretchDirection IconStretchDirection
    {
        get => GetValue(IconStretchDirectionProperty);
        set => SetValue(IconStretchDirectionProperty, value);
    }

    public double IconWidth
    {
        get => GetValue(IconWidthProperty);
        set => SetValue(IconWidthProperty, NormalizeLength(value));
    }

    public double IconHeight
    {
        get => GetValue(IconHeightProperty);
        set => SetValue(IconHeightProperty, NormalizeLength(value));
    }

    public double IconMinWidth
    {
        get => GetValue(IconMinWidthProperty);
        set => SetValue(IconMinWidthProperty, NormalizeMinLength(value));
    }

    public double IconMinHeight
    {
        get => GetValue(IconMinHeightProperty);
        set => SetValue(IconMinHeightProperty, NormalizeMinLength(value));
    }

    public double IconMaxWidth
    {
        get => GetValue(IconMaxWidthProperty);
        set => SetValue(IconMaxWidthProperty, NormalizeMaxLength(value));
    }

    public double IconMaxHeight
    {
        get => GetValue(IconMaxHeightProperty);
        set => SetValue(IconMaxHeightProperty, NormalizeMaxLength(value));
    }

    public object? Overlay
    {
        get => GetValue(OverlayProperty);
        set => SetValue(OverlayProperty, value);
    }

    public object? OverlayResourceKey
    {
        get => GetValue(OverlayResourceKeyProperty);
        set => SetValue(OverlayResourceKeyProperty, value);
    }

    public string? OverlayPathData
    {
        get => GetValue(OverlayPathDataProperty);
        set => SetValue(OverlayPathDataProperty, value);
    }

    public string? OverlayEmoji
    {
        get => GetValue(OverlayEmojiProperty);
        set => SetValue(OverlayEmojiProperty, value);
    }

    public Stretch OverlayStretch
    {
        get => GetValue(OverlayStretchProperty);
        set => SetValue(OverlayStretchProperty, value);
    }

    public StretchDirection OverlayStretchDirection
    {
        get => GetValue(OverlayStretchDirectionProperty);
        set => SetValue(OverlayStretchDirectionProperty, value);
    }

    public double OverlayWidth
    {
        get => GetValue(OverlayWidthProperty);
        set => SetValue(OverlayWidthProperty, NormalizeLength(value));
    }

    public double OverlayHeight
    {
        get => GetValue(OverlayHeightProperty);
        set => SetValue(OverlayHeightProperty, NormalizeLength(value));
    }

    public double OverlayMinWidth
    {
        get => GetValue(OverlayMinWidthProperty);
        set => SetValue(OverlayMinWidthProperty, NormalizeMinLength(value));
    }

    public double OverlayMinHeight
    {
        get => GetValue(OverlayMinHeightProperty);
        set => SetValue(OverlayMinHeightProperty, NormalizeMinLength(value));
    }

    public double OverlayMaxWidth
    {
        get => GetValue(OverlayMaxWidthProperty);
        set => SetValue(OverlayMaxWidthProperty, NormalizeMaxLength(value));
    }

    public double OverlayMaxHeight
    {
        get => GetValue(OverlayMaxHeightProperty);
        set => SetValue(OverlayMaxHeightProperty, NormalizeMaxLength(value));
    }

    public HorizontalAlignment OverlayHorizontalAlignment
    {
        get => GetValue(OverlayHorizontalAlignmentProperty);
        set => SetValue(OverlayHorizontalAlignmentProperty, value);
    }

    public VerticalAlignment OverlayVerticalAlignment
    {
        get => GetValue(OverlayVerticalAlignmentProperty);
        set => SetValue(OverlayVerticalAlignmentProperty, value);
    }

    public Thickness OverlayMargin
    {
        get => GetValue(OverlayMarginProperty);
        set => SetValue(OverlayMarginProperty, value);
    }

    public int? OverlayCount
    {
        get => GetValue(OverlayCountProperty);
        set => SetValue(OverlayCountProperty, value);
    }

    public string? OverlayCountText
    {
        get => GetValue(OverlayCountTextProperty);
        set => SetValue(OverlayCountTextProperty, value);
    }

    public bool ShowOverlayCountWhenZero
    {
        get => GetValue(ShowOverlayCountWhenZeroProperty);
        set => SetValue(ShowOverlayCountWhenZeroProperty, value);
    }

    public HorizontalAlignment OverlayCountHorizontalAlignment
    {
        get => GetValue(OverlayCountHorizontalAlignmentProperty);
        set => SetValue(OverlayCountHorizontalAlignmentProperty, value);
    }

    public VerticalAlignment OverlayCountVerticalAlignment
    {
        get => GetValue(OverlayCountVerticalAlignmentProperty);
        set => SetValue(OverlayCountVerticalAlignmentProperty, value);
    }

    public Thickness OverlayCountMargin
    {
        get => GetValue(OverlayCountMarginProperty);
        set => SetValue(OverlayCountMarginProperty, value);
    }

    public object? ResolvedIconContent
    {
        get => _resolvedIconContent;
        private set => SetAndRaise(ResolvedIconContentProperty, ref _resolvedIconContent, value);
    }

    public object? ResolvedOverlayContent
    {
        get => _resolvedOverlayContent;
        private set => SetAndRaise(ResolvedOverlayContentProperty, ref _resolvedOverlayContent, value);
    }

    public string? ResolvedOverlayCountText
    {
        get => _resolvedOverlayCountText;
        private set => SetAndRaise(ResolvedOverlayCountTextProperty, ref _resolvedOverlayCountText, value);
    }

    public bool HasOverlay
    {
        get => _hasOverlay;
        private set => SetAndRaise(HasOverlayProperty, ref _hasOverlay, value);
    }

    public bool HasOverlayCount
    {
        get => _hasOverlayCount;
        private set => SetAndRaise(HasOverlayCountProperty, ref _hasOverlayCount, value);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        UpdateResolvedIconContent();
        UpdateResolvedOverlayContent();
        UpdateResolvedOverlayCountText();
    }

    private void OnResourcesChanged(object? sender, ResourcesChangedEventArgs e)
    {
        if (IconResourceKey is not null)
        {
            UpdateResolvedIconContent();
        }

        if (OverlayResourceKey is not null)
        {
            UpdateResolvedOverlayContent();
        }
    }

    private void OnActualThemeVariantChanged(object? sender, EventArgs e)
    {
        if (IconResourceKey is not null)
        {
            UpdateResolvedIconContent();
        }

        if (OverlayResourceKey is not null)
        {
            UpdateResolvedOverlayContent();
        }
    }

    private void UpdateResolvedIconContent()
    {
        object? source = ResolveContentSource(Icon, IconResourceKey, IconPathData, IconEmoji);
        ResolvedIconContent = CreateIconContent(source);
    }

    private void UpdateResolvedOverlayContent()
    {
        object? source = ResolveContentSource(Overlay, OverlayResourceKey, OverlayPathData, OverlayEmoji);
        ResolvedOverlayContent = CreateIconContent(source);
        HasOverlay = ResolvedOverlayContent is not null;
    }

    private void UpdateResolvedOverlayCountText()
    {
        var countText = NormalizeText(OverlayCountText);
        if (countText is null && OverlayCount.HasValue)
        {
            var count = OverlayCount.Value;
            if (count != 0 || ShowOverlayCountWhenZero)
            {
                countText = count.ToString(CultureInfo.InvariantCulture);
            }
        }

        ResolvedOverlayCountText = countText;
        HasOverlayCount = !string.IsNullOrWhiteSpace(countText);
    }

    private object? ResolveContentSource(object? directSource, object? resourceKey, string? pathData, string? emoji)
    {
        object? source = NormalizeUnsetValue(directSource);

        if (source is null && resourceKey is not null)
        {
            _ = this.TryFindResource(resourceKey, ActualThemeVariant, out source);
        }

        if (source is null && !string.IsNullOrWhiteSpace(pathData))
        {
            source = pathData;
        }

        if (source is null && !string.IsNullOrWhiteSpace(emoji))
        {
            source = emoji;
        }

        return source;
    }

    private static object? CreateIconContent(object? source)
    {
        source = NormalizeUnsetValue(source);
        if (source is null)
        {
            return null;
        }

        if (source is Control control)
        {
            return CloneControlForPresentation(control);
        }

        if (source is Geometry)
        {
            return source is Geometry geometry
                ? new PathIcon { Data = geometry.Clone() }
                : source;
        }

        if (source is IImage image)
        {
            return new Image
            {
                Source = image,
                Stretch = Stretch.Uniform,
            };
        }

        if (source is string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            if (TryParseGeometry(text, out var geometry))
            {
                return new PathIcon { Data = geometry };
            }

            return new TextBlock
            {
                Text = text,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center,
            };
        }

        return source;
    }

    private static Control CloneControlForPresentation(Control source)
    {
        var cloned = CloneControl(source, new Dictionary<Control, Control>(ReferenceEqualityComparer.Instance));
        if (cloned is not null)
        {
            return cloned;
        }

        return new TextBlock
        {
            Text = source is TextBlock textBlock
                ? textBlock.Text
                : source.ToString(),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            TextAlignment = TextAlignment.Center,
        };
    }

    private static Control? CloneControl(Control source, IDictionary<Control, Control> map)
    {
        if (map.TryGetValue(source, out var existing))
        {
            return existing;
        }

        if (Activator.CreateInstance(source.GetType()) is not Control clone)
        {
            return null;
        }

        map[source] = clone;

        foreach (var cssClass in source.Classes)
        {
            clone.Classes.Add(cssClass);
        }

        CloneAvaloniaProperties(source, clone, map);
        ClonePanelChildren(source, clone, map);

        return clone;
    }

    private static void CloneAvaloniaProperties(Control source, Control clone, IDictionary<Control, Control> map)
    {
        var registry = AvaloniaPropertyRegistry.Instance;
        var registered = registry.GetRegistered(source.GetType());
        var attached = registry.GetRegisteredAttached(source.GetType());

        foreach (var property in registered.Concat(attached))
        {
            if (property.IsReadOnly || ShouldSkipClonedProperty(property))
            {
                continue;
            }

            if (!source.IsSet(property))
            {
                continue;
            }

            object? value;
            try
            {
                value = source.GetValue(property);
            }
            catch
            {
                continue;
            }

            if (ReferenceEquals(value, AvaloniaProperty.UnsetValue))
            {
                continue;
            }

            if (value is IList && value is not string)
            {
                continue;
            }

            object? clonedValue;
            try
            {
                clonedValue = ClonePropertyValue(value, map);
            }
            catch
            {
                continue;
            }

            try
            {
                clone.SetValue(property, clonedValue);
            }
            catch
            {
                // Best-effort cloning: ignore properties that cannot be transferred.
            }
        }
    }

    private static bool ShouldSkipClonedProperty(AvaloniaProperty property)
    {
        return property == StyledElement.DataContextProperty;
    }

    private static object? ClonePropertyValue(object? value, IDictionary<Control, Control> map)
    {
        value = NormalizeUnsetValue(value);
        if (value is null)
        {
            return null;
        }

        if (value is Geometry geometry)
        {
            return geometry.Clone();
        }

        if (value is Control nestedControl)
        {
            return CloneControl(nestedControl, map);
        }

        return value;
    }

    private static void ClonePanelChildren(Control source, Control clone, IDictionary<Control, Control> map)
    {
        if (source is not Panel sourcePanel || clone is not Panel targetPanel)
        {
            return;
        }

        foreach (var child in sourcePanel.Children.OfType<Control>())
        {
            var clonedChild = CloneControl(child, map);
            if (clonedChild is null)
            {
                continue;
            }

            CopyAttachedLayoutProperties(child, clonedChild);
            targetPanel.Children.Add(clonedChild);
        }
    }

    private static void CopyAttachedLayoutProperties(Control source, Control target)
    {
        Grid.SetColumn(target, Grid.GetColumn(source));
        Grid.SetColumnSpan(target, Grid.GetColumnSpan(source));
        Grid.SetRow(target, Grid.GetRow(source));
        Grid.SetRowSpan(target, Grid.GetRowSpan(source));
        DockPanel.SetDock(target, DockPanel.GetDock(source));
        Canvas.SetLeft(target, Canvas.GetLeft(source));
        Canvas.SetTop(target, Canvas.GetTop(source));
        Canvas.SetRight(target, Canvas.GetRight(source));
        Canvas.SetBottom(target, Canvas.GetBottom(source));
    }

    private static string? NormalizeText(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value;
    }

    private static object? NormalizeUnsetValue(object? value)
    {
        return ReferenceEquals(value, AvaloniaProperty.UnsetValue)
            ? null
            : value;
    }

    private static bool TryParseGeometry(string pathData, out Geometry? geometry)
    {
        try
        {
            geometry = StreamGeometry.Parse(pathData);
            return true;
        }
        catch
        {
            geometry = null;
            return false;
        }
    }

    private static double NormalizeLength(double value)
    {
        return double.IsNaN(value) || value >= 0
            ? value
            : double.NaN;
    }

    private static double NormalizeMinLength(double value)
    {
        return value <= 0
            ? 0
            : value;
    }

    private static double NormalizeMaxLength(double value)
    {
        return double.IsNaN(value) || value <= 0 || double.IsPositiveInfinity(value)
            ? double.PositiveInfinity
            : value;
    }
}
