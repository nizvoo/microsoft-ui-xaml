// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

// DO NOT EDIT! This file was generated by CustomTasks.DependencyPropertyCodeGen
#pragma once

class ScrollingViewProperties
{
public:
    ScrollingViewProperties();

    void ComputedHorizontalScrollBarVisibility(winrt::Visibility const& value);
    winrt::Visibility ComputedHorizontalScrollBarVisibility();

    void ComputedVerticalScrollBarVisibility(winrt::Visibility const& value);
    winrt::Visibility ComputedVerticalScrollBarVisibility();

    void Content(winrt::UIElement const& value);
    winrt::UIElement Content();

    void ContentOrientation(winrt::ContentOrientation const& value);
    winrt::ContentOrientation ContentOrientation();

    void HorizontalAnchorRatio(double value);
    double HorizontalAnchorRatio();

    void HorizontalScrollBarVisibility(winrt::ScrollBarVisibility const& value);
    winrt::ScrollBarVisibility HorizontalScrollBarVisibility();

    void HorizontalScrollChainingMode(winrt::ChainingMode const& value);
    winrt::ChainingMode HorizontalScrollChainingMode();

    void HorizontalScrollController(winrt::IScrollController const& value);
    winrt::IScrollController HorizontalScrollController();

    void HorizontalScrollMode(winrt::ScrollMode const& value);
    winrt::ScrollMode HorizontalScrollMode();

    void HorizontalScrollRailingMode(winrt::RailingMode const& value);
    winrt::RailingMode HorizontalScrollRailingMode();

    void IgnoredInputKind(winrt::InputKind const& value);
    winrt::InputKind IgnoredInputKind();

    void MaxZoomFactor(double value);
    double MaxZoomFactor();

    void MinZoomFactor(double value);
    double MinZoomFactor();

    void VerticalAnchorRatio(double value);
    double VerticalAnchorRatio();

    void VerticalScrollBarVisibility(winrt::ScrollBarVisibility const& value);
    winrt::ScrollBarVisibility VerticalScrollBarVisibility();

    void VerticalScrollChainingMode(winrt::ChainingMode const& value);
    winrt::ChainingMode VerticalScrollChainingMode();

    void VerticalScrollController(winrt::IScrollController const& value);
    winrt::IScrollController VerticalScrollController();

    void VerticalScrollMode(winrt::ScrollMode const& value);
    winrt::ScrollMode VerticalScrollMode();

    void VerticalScrollRailingMode(winrt::RailingMode const& value);
    winrt::RailingMode VerticalScrollRailingMode();

    void ZoomChainingMode(winrt::ChainingMode const& value);
    winrt::ChainingMode ZoomChainingMode();

    void ZoomMode(winrt::ZoomMode const& value);
    winrt::ZoomMode ZoomMode();

    static winrt::DependencyProperty ComputedHorizontalScrollBarVisibilityProperty() { return s_ComputedHorizontalScrollBarVisibilityProperty; }
    static winrt::DependencyProperty ComputedVerticalScrollBarVisibilityProperty() { return s_ComputedVerticalScrollBarVisibilityProperty; }
    static winrt::DependencyProperty ContentProperty() { return s_ContentProperty; }
    static winrt::DependencyProperty ContentOrientationProperty() { return s_ContentOrientationProperty; }
    static winrt::DependencyProperty HorizontalAnchorRatioProperty() { return s_HorizontalAnchorRatioProperty; }
    static winrt::DependencyProperty HorizontalScrollBarVisibilityProperty() { return s_HorizontalScrollBarVisibilityProperty; }
    static winrt::DependencyProperty HorizontalScrollChainingModeProperty() { return s_HorizontalScrollChainingModeProperty; }
    static winrt::DependencyProperty HorizontalScrollControllerProperty() { return s_HorizontalScrollControllerProperty; }
    static winrt::DependencyProperty HorizontalScrollModeProperty() { return s_HorizontalScrollModeProperty; }
    static winrt::DependencyProperty HorizontalScrollRailingModeProperty() { return s_HorizontalScrollRailingModeProperty; }
    static winrt::DependencyProperty IgnoredInputKindProperty() { return s_IgnoredInputKindProperty; }
    static winrt::DependencyProperty MaxZoomFactorProperty() { return s_MaxZoomFactorProperty; }
    static winrt::DependencyProperty MinZoomFactorProperty() { return s_MinZoomFactorProperty; }
    static winrt::DependencyProperty ScrollingPresenterProperty() { return s_ScrollingPresenterProperty; }
    static winrt::DependencyProperty VerticalAnchorRatioProperty() { return s_VerticalAnchorRatioProperty; }
    static winrt::DependencyProperty VerticalScrollBarVisibilityProperty() { return s_VerticalScrollBarVisibilityProperty; }
    static winrt::DependencyProperty VerticalScrollChainingModeProperty() { return s_VerticalScrollChainingModeProperty; }
    static winrt::DependencyProperty VerticalScrollControllerProperty() { return s_VerticalScrollControllerProperty; }
    static winrt::DependencyProperty VerticalScrollModeProperty() { return s_VerticalScrollModeProperty; }
    static winrt::DependencyProperty VerticalScrollRailingModeProperty() { return s_VerticalScrollRailingModeProperty; }
    static winrt::DependencyProperty ZoomChainingModeProperty() { return s_ZoomChainingModeProperty; }
    static winrt::DependencyProperty ZoomModeProperty() { return s_ZoomModeProperty; }

    static GlobalDependencyProperty s_ComputedHorizontalScrollBarVisibilityProperty;
    static GlobalDependencyProperty s_ComputedVerticalScrollBarVisibilityProperty;
    static GlobalDependencyProperty s_ContentProperty;
    static GlobalDependencyProperty s_ContentOrientationProperty;
    static GlobalDependencyProperty s_HorizontalAnchorRatioProperty;
    static GlobalDependencyProperty s_HorizontalScrollBarVisibilityProperty;
    static GlobalDependencyProperty s_HorizontalScrollChainingModeProperty;
    static GlobalDependencyProperty s_HorizontalScrollControllerProperty;
    static GlobalDependencyProperty s_HorizontalScrollModeProperty;
    static GlobalDependencyProperty s_HorizontalScrollRailingModeProperty;
    static GlobalDependencyProperty s_IgnoredInputKindProperty;
    static GlobalDependencyProperty s_MaxZoomFactorProperty;
    static GlobalDependencyProperty s_MinZoomFactorProperty;
    static GlobalDependencyProperty s_ScrollingPresenterProperty;
    static GlobalDependencyProperty s_VerticalAnchorRatioProperty;
    static GlobalDependencyProperty s_VerticalScrollBarVisibilityProperty;
    static GlobalDependencyProperty s_VerticalScrollChainingModeProperty;
    static GlobalDependencyProperty s_VerticalScrollControllerProperty;
    static GlobalDependencyProperty s_VerticalScrollModeProperty;
    static GlobalDependencyProperty s_VerticalScrollRailingModeProperty;
    static GlobalDependencyProperty s_ZoomChainingModeProperty;
    static GlobalDependencyProperty s_ZoomModeProperty;

    winrt::event_token AnchorRequested(winrt::TypedEventHandler<winrt::ScrollingView, winrt::ScrollingPresenterAnchorRequestedEventArgs> const& value);
    void AnchorRequested(winrt::event_token const& token);
    winrt::event_token BringingIntoView(winrt::TypedEventHandler<winrt::ScrollingView, winrt::ScrollingPresenterBringingIntoViewEventArgs> const& value);
    void BringingIntoView(winrt::event_token const& token);
    winrt::event_token ExtentChanged(winrt::TypedEventHandler<winrt::ScrollingView, winrt::IInspectable> const& value);
    void ExtentChanged(winrt::event_token const& token);
    winrt::event_token ScrollAnimationStarting(winrt::TypedEventHandler<winrt::ScrollingView, winrt::ScrollAnimationStartingEventArgs> const& value);
    void ScrollAnimationStarting(winrt::event_token const& token);
    winrt::event_token ScrollCompleted(winrt::TypedEventHandler<winrt::ScrollingView, winrt::ScrollCompletedEventArgs> const& value);
    void ScrollCompleted(winrt::event_token const& token);
    winrt::event_token StateChanged(winrt::TypedEventHandler<winrt::ScrollingView, winrt::IInspectable> const& value);
    void StateChanged(winrt::event_token const& token);
    winrt::event_token ViewChanged(winrt::TypedEventHandler<winrt::ScrollingView, winrt::IInspectable> const& value);
    void ViewChanged(winrt::event_token const& token);
    winrt::event_token ZoomAnimationStarting(winrt::TypedEventHandler<winrt::ScrollingView, winrt::ZoomAnimationStartingEventArgs> const& value);
    void ZoomAnimationStarting(winrt::event_token const& token);
    winrt::event_token ZoomCompleted(winrt::TypedEventHandler<winrt::ScrollingView, winrt::ZoomCompletedEventArgs> const& value);
    void ZoomCompleted(winrt::event_token const& token);

    event_source<winrt::TypedEventHandler<winrt::ScrollingView, winrt::ScrollingPresenterAnchorRequestedEventArgs>> m_anchorRequestedEventSource;
    event_source<winrt::TypedEventHandler<winrt::ScrollingView, winrt::ScrollingPresenterBringingIntoViewEventArgs>> m_bringingIntoViewEventSource;
    event_source<winrt::TypedEventHandler<winrt::ScrollingView, winrt::IInspectable>> m_extentChangedEventSource;
    event_source<winrt::TypedEventHandler<winrt::ScrollingView, winrt::ScrollAnimationStartingEventArgs>> m_scrollAnimationStartingEventSource;
    event_source<winrt::TypedEventHandler<winrt::ScrollingView, winrt::ScrollCompletedEventArgs>> m_scrollCompletedEventSource;
    event_source<winrt::TypedEventHandler<winrt::ScrollingView, winrt::IInspectable>> m_stateChangedEventSource;
    event_source<winrt::TypedEventHandler<winrt::ScrollingView, winrt::IInspectable>> m_viewChangedEventSource;
    event_source<winrt::TypedEventHandler<winrt::ScrollingView, winrt::ZoomAnimationStartingEventArgs>> m_zoomAnimationStartingEventSource;
    event_source<winrt::TypedEventHandler<winrt::ScrollingView, winrt::ZoomCompletedEventArgs>> m_zoomCompletedEventSource;

    static void EnsureProperties();
    static void ClearProperties();

    static void OnComputedHorizontalScrollBarVisibilityPropertyChanged(
        winrt::DependencyObject const& sender,
        winrt::DependencyPropertyChangedEventArgs const& args);

    static void OnComputedVerticalScrollBarVisibilityPropertyChanged(
        winrt::DependencyObject const& sender,
        winrt::DependencyPropertyChangedEventArgs const& args);

    static void OnContentPropertyChanged(
        winrt::DependencyObject const& sender,
        winrt::DependencyPropertyChangedEventArgs const& args);

    static void OnContentOrientationPropertyChanged(
        winrt::DependencyObject const& sender,
        winrt::DependencyPropertyChangedEventArgs const& args);

    static void OnHorizontalAnchorRatioPropertyChanged(
        winrt::DependencyObject const& sender,
        winrt::DependencyPropertyChangedEventArgs const& args);

    static void OnHorizontalScrollBarVisibilityPropertyChanged(
        winrt::DependencyObject const& sender,
        winrt::DependencyPropertyChangedEventArgs const& args);

    static void OnHorizontalScrollChainingModePropertyChanged(
        winrt::DependencyObject const& sender,
        winrt::DependencyPropertyChangedEventArgs const& args);

    static void OnHorizontalScrollControllerPropertyChanged(
        winrt::DependencyObject const& sender,
        winrt::DependencyPropertyChangedEventArgs const& args);

    static void OnHorizontalScrollModePropertyChanged(
        winrt::DependencyObject const& sender,
        winrt::DependencyPropertyChangedEventArgs const& args);

    static void OnHorizontalScrollRailingModePropertyChanged(
        winrt::DependencyObject const& sender,
        winrt::DependencyPropertyChangedEventArgs const& args);

    static void OnIgnoredInputKindPropertyChanged(
        winrt::DependencyObject const& sender,
        winrt::DependencyPropertyChangedEventArgs const& args);

    static void OnMaxZoomFactorPropertyChanged(
        winrt::DependencyObject const& sender,
        winrt::DependencyPropertyChangedEventArgs const& args);

    static void OnMinZoomFactorPropertyChanged(
        winrt::DependencyObject const& sender,
        winrt::DependencyPropertyChangedEventArgs const& args);

    static void OnScrollingPresenterPropertyChanged(
        winrt::DependencyObject const& sender,
        winrt::DependencyPropertyChangedEventArgs const& args);

    static void OnVerticalAnchorRatioPropertyChanged(
        winrt::DependencyObject const& sender,
        winrt::DependencyPropertyChangedEventArgs const& args);

    static void OnVerticalScrollBarVisibilityPropertyChanged(
        winrt::DependencyObject const& sender,
        winrt::DependencyPropertyChangedEventArgs const& args);

    static void OnVerticalScrollChainingModePropertyChanged(
        winrt::DependencyObject const& sender,
        winrt::DependencyPropertyChangedEventArgs const& args);

    static void OnVerticalScrollControllerPropertyChanged(
        winrt::DependencyObject const& sender,
        winrt::DependencyPropertyChangedEventArgs const& args);

    static void OnVerticalScrollModePropertyChanged(
        winrt::DependencyObject const& sender,
        winrt::DependencyPropertyChangedEventArgs const& args);

    static void OnVerticalScrollRailingModePropertyChanged(
        winrt::DependencyObject const& sender,
        winrt::DependencyPropertyChangedEventArgs const& args);

    static void OnZoomChainingModePropertyChanged(
        winrt::DependencyObject const& sender,
        winrt::DependencyPropertyChangedEventArgs const& args);

    static void OnZoomModePropertyChanged(
        winrt::DependencyObject const& sender,
        winrt::DependencyPropertyChangedEventArgs const& args);
};
