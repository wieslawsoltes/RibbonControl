// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Layout;
using Avalonia.Media;
using RibbonControl.Core.Controls;
using RibbonControl.Core.Icons;

namespace RibbonControl.Headless.Tests;

public class RibbonIconPresenterHeadlessTests
{
    [AvaloniaFact]
    public void Icon_TakesPrecedence_OverResourcePathAndEmoji()
    {
        var customIcon = new TextBlock { Text = "custom" };
        var presenter = new RibbonIconPresenter
        {
            Icon = customIcon,
            IconResourceKey = "Ribbon.Sample.Icon",
            IconPathData = FluentIconData.Copy20Regular,
            IconEmoji = "🙂",
        };
        presenter.Resources["Ribbon.Sample.Icon"] = FluentIconData.Document20Regular;

        var resolved = Assert.IsType<TextBlock>(presenter.ResolvedIconContent);
        Assert.Equal("custom", resolved.Text);
        Assert.NotSame(customIcon, resolved);
    }

    [AvaloniaFact]
    public void ResourceKey_ResolvesPathData_WhenDirectIconIsMissing()
    {
        var presenter = new RibbonIconPresenter();
        presenter.Resources["Ribbon.Sample.Icon"] = FluentIconData.Copy20Regular;
        presenter.IconResourceKey = "Ribbon.Sample.Icon";

        var resolved = Assert.IsType<PathIcon>(presenter.ResolvedIconContent);
        Assert.NotNull(resolved.Data);
    }

    [AvaloniaFact]
    public void PathData_IsUsed_WhenResourceKeyIsMissing()
    {
        var presenter = new RibbonIconPresenter
        {
            IconResourceKey = "Ribbon.Sample.Missing",
            IconPathData = FluentIconData.Copy20Regular,
        };

        var resolved = Assert.IsType<PathIcon>(presenter.ResolvedIconContent);
        Assert.NotNull(resolved.Data);
    }

    [AvaloniaFact]
    public void Emoji_IsUsed_WhenNoOtherIconSourceExists()
    {
        var presenter = new RibbonIconPresenter
        {
            IconEmoji = "🖌️",
        };

        var resolved = Assert.IsType<TextBlock>(presenter.ResolvedIconContent);
        Assert.Equal("🖌️", resolved.Text);
    }

    [AvaloniaFact]
    public void IconSizeProperties_NormalizeInvalidValues()
    {
        var presenter = new RibbonIconPresenter
        {
            IconWidth = -10,
            IconHeight = -5,
            IconMinWidth = -1,
            IconMinHeight = -2,
            IconMaxWidth = -3,
            IconMaxHeight = double.NaN,
            IconStretch = Stretch.UniformToFill,
            IconStretchDirection = StretchDirection.DownOnly,
        };

        Assert.True(double.IsNaN(presenter.IconWidth));
        Assert.True(double.IsNaN(presenter.IconHeight));
        Assert.Equal(0, presenter.IconMinWidth);
        Assert.Equal(0, presenter.IconMinHeight);
        Assert.True(double.IsPositiveInfinity(presenter.IconMaxWidth));
        Assert.True(double.IsPositiveInfinity(presenter.IconMaxHeight));
        Assert.Equal(Stretch.UniformToFill, presenter.IconStretch);
        Assert.Equal(StretchDirection.DownOnly, presenter.IconStretchDirection);
    }

    [AvaloniaFact]
    public void Overlay_TakesPrecedence_OverResourcePathAndEmoji()
    {
        var overlay = new TextBlock { Text = "!" };
        var presenter = new RibbonIconPresenter
        {
            Overlay = overlay,
            OverlayResourceKey = "Ribbon.Sample.Overlay",
            OverlayPathData = FluentIconData.CheckmarkCircle20Regular,
            OverlayEmoji = "🟢",
        };
        presenter.Resources["Ribbon.Sample.Overlay"] = FluentIconData.Copy20Regular;

        var resolved = Assert.IsType<TextBlock>(presenter.ResolvedOverlayContent);
        Assert.Equal("!", resolved.Text);
        Assert.NotSame(overlay, resolved);
        Assert.True(presenter.HasOverlay);
    }

    [AvaloniaFact]
    public void SharedControlIconSource_IsClonedPerPresenter()
    {
        var sharedIcon = new TextBlock { Text = "📄" };
        var firstPresenter = new RibbonIconPresenter
        {
            Icon = sharedIcon,
        };
        var secondPresenter = new RibbonIconPresenter
        {
            Icon = sharedIcon,
        };

        var firstResolved = Assert.IsType<TextBlock>(firstPresenter.ResolvedIconContent);
        var secondResolved = Assert.IsType<TextBlock>(secondPresenter.ResolvedIconContent);

        Assert.Equal("📄", firstResolved.Text);
        Assert.Equal("📄", secondResolved.Text);
        Assert.NotSame(sharedIcon, firstResolved);
        Assert.NotSame(sharedIcon, secondResolved);
        Assert.NotSame(firstResolved, secondResolved);
    }

    [AvaloniaFact]
    public void OverlayCountText_TakesPrecedence_OverNumericCount()
    {
        var presenter = new RibbonIconPresenter
        {
            OverlayCount = 12,
            OverlayCountText = "NEW",
        };

        Assert.Equal("NEW", presenter.ResolvedOverlayCountText);
        Assert.True(presenter.HasOverlayCount);
    }

    [AvaloniaFact]
    public void OverlayCount_HidesZeroByDefault_AndCanBeEnabled()
    {
        var presenter = new RibbonIconPresenter
        {
            OverlayCount = 0,
        };

        Assert.Null(presenter.ResolvedOverlayCountText);
        Assert.False(presenter.HasOverlayCount);

        presenter.ShowOverlayCountWhenZero = true;

        Assert.Equal("0", presenter.ResolvedOverlayCountText);
        Assert.True(presenter.HasOverlayCount);
    }

    [AvaloniaFact]
    public void OverlayDefaults_AreBottomRight()
    {
        var presenter = new RibbonIconPresenter();

        Assert.Equal(HorizontalAlignment.Right, presenter.OverlayHorizontalAlignment);
        Assert.Equal(VerticalAlignment.Bottom, presenter.OverlayVerticalAlignment);
        Assert.Equal(HorizontalAlignment.Right, presenter.OverlayCountHorizontalAlignment);
        Assert.Equal(VerticalAlignment.Bottom, presenter.OverlayCountVerticalAlignment);
    }
}
