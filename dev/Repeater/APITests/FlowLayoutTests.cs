﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Windows.UI.Xaml.Tests.MUXControls.ApiTests.RepeaterTests.Common;
using Windows.UI.Xaml.Tests.MUXControls.ApiTests.RepeaterTests.Common.Mocks;
using MUXControlsTestApp.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Markup;
using Common;

#if USING_TAEF
using WEX.TestExecution;
using WEX.TestExecution.Markup;
using WEX.Logging.Interop;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
#endif

#if !BUILD_WINDOWS
using UniformGridLayoutItemsJustification = Microsoft.UI.Xaml.Controls.UniformGridLayoutItemsJustification;
using FlowLayoutLineAlignment = Microsoft.UI.Xaml.Controls.FlowLayoutLineAlignment;
using VirtualizingLayout = Microsoft.UI.Xaml.Controls.VirtualizingLayout;
using ItemsRepeater = Microsoft.UI.Xaml.Controls.ItemsRepeater;
using ElementFactory = Microsoft.UI.Xaml.Controls.ElementFactory;
using RecyclePool = Microsoft.UI.Xaml.Controls.RecyclePool;
using StackLayout = Microsoft.UI.Xaml.Controls.StackLayout;
using FlowLayout = Microsoft.UI.Xaml.Controls.FlowLayout;
using UniformGridLayout = Microsoft.UI.Xaml.Controls.UniformGridLayout;
using ScrollAnchorProvider = Microsoft.UI.Xaml.Controls.ScrollAnchorProvider;
using VirtualizingLayoutContext = Microsoft.UI.Xaml.Controls.VirtualizingLayoutContext;
using LayoutContext = Microsoft.UI.Xaml.Controls.LayoutContext;
using LayoutPanel = Microsoft.UI.Xaml.Controls.LayoutPanel;
#endif

namespace Windows.UI.Xaml.Tests.MUXControls.ApiTests.RepeaterTests
{
    [TestClass]
    public class FlowLayoutTests : TestsBase
    {      
        [TestMethod]
        public void ValidateStackLayout()
        {
            RunOnUIThread.Execute(() =>
            {
                foreach (ScrollOrientation scrollOrientation in Enum.GetValues(typeof(ScrollOrientation)))
                {
                    Log.Comment(string.Format("ScrollOrientation: {0}", scrollOrientation));
                    var om = new OrientationBasedMeasures(scrollOrientation);
                    const int panelMinorSize = 400;
                    const int numItems = 5;
                    const int itemMajorSize = 50;
                    // We test for when the item is smaller than the panel and when it's bigger than the panel: 
                    int[] itemMinorSizeChoices = new int[] { 200, 800 };

                    foreach (int itemMinorSize in itemMinorSizeChoices)
                    {

                        LayoutPanel panel = new LayoutPanel()
                        {
                            Layout = new StackLayout() { Orientation = scrollOrientation.ToLayoutOrientation() }
                        };

                    SetPanelMinorSize(panel, om, panelMinorSize);
                    for (int i = 0; i < numItems; i++)
                    {
                        panel.Children.Add(
                            new Button()
                            {
                                Content = i,
                                Width = om.IsVerical ? itemMinorSize : itemMajorSize,
                                Height = om.IsVerical ? itemMajorSize : itemMinorSize
                            });
                    }

                        Content = panel;
                        Content.UpdateLayout();
                        int itemSpacing = 0;
                        ValidateStackLayoutChildrenLayoutBounds(om, (i) => panel.Children[i], panelMinorSize, numItems, itemMajorSize, itemSpacing, panel.DesiredSize);

                        itemSpacing = 10;
                        ((StackLayout)panel.Layout).Spacing = itemSpacing;
                        panel.UpdateLayout();
                        ValidateStackLayoutChildrenLayoutBounds(om, (i) => panel.Children[i], panelMinorSize, numItems, itemMajorSize, itemSpacing, panel.DesiredSize);
                    }
                }
            });
        }

        [TestMethod]
        public void ValidateGridLayout()
        {
            RunOnUIThread.Execute(() =>
            {
                foreach (ScrollOrientation scrollOrientation in Enum.GetValues(typeof(ScrollOrientation)))
                {
                    Log.Comment(string.Format("ScrollOrientation: {0}", scrollOrientation));
                    var om = new OrientationBasedMeasures(scrollOrientation);
                    const int panelMinorSize = 400;
                    const int numItems = 10;
                    const int itemMajorSize = 50;
                    int[] itemMinorSizeChoices = new int[] { 100, 800 };

                    foreach (int itemMinorSize in itemMinorSizeChoices)
                    {
                        Log.Comment(string.Format("MinorSize: {0}", itemMinorSize));
                        LayoutPanel panel = new LayoutPanel()
                        {
                            Layout = new UniformGridLayout()
                            {
                                Orientation = scrollOrientation.ToOrthogonalLayoutOrientation(),
                                ItemsJustification = UniformGridLayoutItemsJustification.Start,
                                MinItemWidth = om.IsVerical ? itemMinorSize : itemMajorSize,
                                MinItemHeight = om.IsVerical ? itemMajorSize : itemMinorSize
                            },
                            VerticalAlignment = VerticalAlignment.Top,
                            HorizontalAlignment = HorizontalAlignment.Left
                        };

                        SetPanelMinorSize(panel, om, panelMinorSize);
                        for (int i = 0; i < numItems; i++)
                        {
                            panel.Children.Add(new Button() { Content = i });
                        }

                        // Put the panel inside a canvas to give it all the space it needs
                        // and avoid any clipping of desired size by the framework.
                        var canvas = new Canvas();
                        canvas.Children.Add(panel);
                        Content = canvas;
                        Content.UpdateLayout();
                        int minItemSpacing = 0;
                        int lineSpacing = 0;
                        ValidateGridLayoutChildrenLayoutBounds(om, (i) => panel.Children[i], itemMinorSize, itemMajorSize, minItemSpacing, lineSpacing, panel.Children.Count, panel.DesiredSize);

                        minItemSpacing = 10;
                        lineSpacing = 10;
                        ((UniformGridLayout)panel.Layout).MinRowSpacing = minItemSpacing;
                        ((UniformGridLayout)panel.Layout).MinColumnSpacing = lineSpacing;
                        Content.UpdateLayout();
                        ValidateGridLayoutChildrenLayoutBounds(om, (i) => panel.Children[i], itemMinorSize, itemMajorSize, minItemSpacing, lineSpacing, panel.Children.Count, panel.DesiredSize);
                    }
                }
            });
        }
        
        [TestMethod]
        public void ValidateResizingFirstItemResizesOtherItemsInGridLayout()
        {
            if (PlatformConfiguration.IsOsVersionGreaterThan(OSVersion.Redstone4))
            {
                //BUGBUG Bug 19277320: MUX Repeater tests fail on RS5_Release
                return;
            }

            RunOnUIThread.Execute(() =>
            {
                const int numItems = 10;
                foreach (ScrollOrientation scrollOrientation in Enum.GetValues(typeof(ScrollOrientation)))
                {
                    Log.Comment(string.Format("ScrollOrientation: {0}", scrollOrientation));
                    foreach (DimensionChoice dimension in Enum.GetValues(typeof(DimensionChoice)))
                    {
                        LayoutPanel panel = new LayoutPanel()
                        {
                            Layout = new UniformGridLayout() { }
                        };

                        for (int i = 0; i < numItems; i++)
                        {
                            panel.Children.Add(new Button() { Content = i, HorizontalAlignment = HorizontalAlignment.Stretch });
                        }

                        Content = panel;
                        Content.UpdateLayout();

                        // verify the defaults before our modification 
                        for (int i = 0; i < panel.Children.Count; i++)
                        {
                            var child = (FrameworkElement)panel.Children.ElementAt(i);
                            var layoutBounds = LayoutInformation.GetLayoutSlot(child);

                            // Ensure width is not 100 before updating.
                            Verify.AreNotEqual(100, layoutBounds.Width);
                            Verify.AreEqual(32, layoutBounds.Height);
                        }

                        // modify the first item's width, hight, or both
                        var firstItem = panel.Children[0] as Button;

                        switch (dimension)
                        {
                            case DimensionChoice.Width:
                                firstItem.Width = 100;
                                break;
                            case DimensionChoice.Height:
                                firstItem.Height = 100;
                                break;
                            case DimensionChoice.Size:
                                firstItem.Width = 100;
                                firstItem.Height = 100;
                                break;
                            default:
                                throw new InvalidOperationException("Unhanlded dimension choice.");
                        }

                        Content.UpdateLayout();

                        // validate that our dimension's size has propagated to all other buttons
                        for (int i = 0; i < panel.Children.Count; i++)
                        {
                            var child = (FrameworkElement)panel.Children.ElementAt(i);
                            var layoutBounds = LayoutInformation.GetLayoutSlot(child);

                            switch (dimension)
                            {
                                case DimensionChoice.Width:
                                    Verify.AreEqual(100, layoutBounds.Width);
                                    break;
                                case DimensionChoice.Height:
                                    Verify.AreEqual(100, layoutBounds.Height);
                                    break;
                                case DimensionChoice.Size:
                                    Verify.AreEqual(100, layoutBounds.Width);
                                    Verify.AreEqual(100, layoutBounds.Height);
                                    break;
                                default:
                                    throw new InvalidOperationException("Unhanlded dimension choice.");

                            }
                        }
                    }
                }
            });
        }

        [TestMethod]
        public void ValidateDefaultWidthForGridLayoutItemsIsBasedOnFirstItem()
        {
            if (PlatformConfiguration.IsOsVersionGreaterThan(OSVersion.Redstone4))
            {
                //BUGBUG Bug 19277320: MUX Repeater tests fail on RS5_Release
                return;
            }

            ValidateDimensionForGridLayoutItemsIsBasedOnFirstItem(DimensionChoice.Width);
        }

        [TestMethod]
        public void ValidateDefaultHeightForGridLayoutItemsIsBasedOnFirstItem()
        {
            if (PlatformConfiguration.IsOsVersionGreaterThan(OSVersion.Redstone4))
            {
                //BUGBUG Bug 19277320: MUX Repeater tests fail on RS5_Release
                return;
            }

            ValidateDimensionForGridLayoutItemsIsBasedOnFirstItem(DimensionChoice.Height);
        }

        [TestMethod]
        public void ValidateDefaultSizeForGridLayoutItemsIsBasedOnFirstItem()
        {
            if (PlatformConfiguration.IsOsVersionGreaterThan(OSVersion.Redstone4))
            {
                //BUGBUG Bug 19277320: MUX Repeater tests fail on RS5_Release
                return;
            }

            ValidateDimensionForGridLayoutItemsIsBasedOnFirstItem(DimensionChoice.Size);
        }

        [TestMethod]
        public void ValidateDefaultSizeForFirstItemDoesntOverridePresetOne()
        {
            RunOnUIThread.Execute(() =>
            {
                const int itemWidth = 50;
                const int itemHeight = 50;

                foreach (ScrollOrientation scrollOrientation in Enum.GetValues(typeof(ScrollOrientation)))
                {
                    LayoutPanel panel = new LayoutPanel()
                    {
                        Layout = new UniformGridLayout()
                        {
                            MinItemWidth = itemWidth,
                            MinItemHeight = itemHeight,
                            Orientation = scrollOrientation.ToOrthogonalLayoutOrientation(),
                        }
                    };

                    PopulatePanelWithButtons(panel, 10 /*numItems*/);
                    Content = panel;
                    Content.UpdateLayout();

                    ModifyElementSize(panel, 0 /*index*/, 100 /*itemWidth*/, 100 /*itemHeight*/);
                    Content = panel;
                    Content.UpdateLayout();
                    ValidateElementsSizeInGridLayout(panel, itemWidth, itemHeight);
                }
            });
        }

        [TestMethod]
        public void ValidateAdaptabilityWhenChangingFirstElementWidthForGridLayout()
        {
            if (PlatformConfiguration.IsOsVersionGreaterThan(OSVersion.Redstone4))
            {
                //BUGBUG Bug 19277320: MUX Repeater tests fail on RS5_Release
                return;
            }

            ValidateAdaptabilityWhenChangingTheFirstElementForGridLayout(DimensionChoice.Width);
        }

        [TestMethod]
        public void ValidateAdaptabilityWhenChangingFirstElementHeightForGridLayout()
        {
            if (PlatformConfiguration.IsOsVersionGreaterThan(OSVersion.Redstone4))
            {
                //BUGBUG Bug 19277320: MUX Repeater tests fail on RS5_Release
                return;
            }

            ValidateAdaptabilityWhenChangingTheFirstElementForGridLayout(DimensionChoice.Height);
        }

        [TestMethod]
        public void ValidateAdaptabilityWhenChangingFirstElementSizeForGridLayout()
        {
            if (PlatformConfiguration.IsOsVersionGreaterThan(OSVersion.Redstone4))
            {
                //BUGBUG Bug 19277320: MUX Repeater tests fail on RS5_Release
                return;
            }

            ValidateAdaptabilityWhenChangingTheFirstElementForGridLayout(DimensionChoice.Size);
        }

        [TestMethod]
        public void ValidateGridLayoutLineAlignment()
        {
            RunOnUIThread.Execute(() =>
            {
                foreach (ScrollOrientation scrollOrientation in Enum.GetValues(typeof(ScrollOrientation)))
                {
                    Log.Comment(string.Format("ScrollOrientation: {0}", scrollOrientation));
                    var om = new OrientationBasedMeasures(scrollOrientation);
                    const int panelMinorSize = 500;
                    const int numItems = 2;
                    const int itemMinorSize = 200;
                    const int itemMajorSize = 100;
                    LayoutPanel panel = new LayoutPanel()
                    {
                        Layout = new UniformGridLayout()
                        {
                            Orientation = scrollOrientation.ToOrthogonalLayoutOrientation(),
                            ItemsJustification = UniformGridLayoutItemsJustification.Start,
                            MinItemWidth = om.IsVerical ? itemMinorSize : itemMajorSize,
                            MinItemHeight = om.IsVerical ? itemMajorSize : itemMinorSize
                        }
                    };

                    SetPanelMinorSize(panel, om, panelMinorSize);
                    for (int i = 0; i < numItems; i++)
                    {
                        panel.Children.Add(new Button() { Content = i });
                    }

                    Content = panel;
                    Content.UpdateLayout();
                    var layout = (UniformGridLayout)panel.Layout;
                    layout.ItemsJustification = UniformGridLayoutItemsJustification.Center;
                    panel.UpdateLayout();
                    ValidateChildBounds(
                        panel,
                        new List<Rect>()
                        {
                            om.MinorMajorRect(50, 0, itemMinorSize, itemMajorSize),
                            om.MinorMajorRect(250, 0, itemMinorSize, itemMajorSize)
                        });

                    layout.ItemsJustification = UniformGridLayoutItemsJustification.End;
                    panel.UpdateLayout();
                    ValidateChildBounds(
                        panel,
                        new List<Rect>()
                        {
                            om.MinorMajorRect(100, 0, itemMinorSize, itemMajorSize),
                            om.MinorMajorRect(300, 0, itemMinorSize, itemMajorSize)
                        });

                    layout.ItemsJustification = UniformGridLayoutItemsJustification.SpaceBetween;
                    panel.UpdateLayout();
                    ValidateChildBounds(
                        panel,
                        new List<Rect>()
                        {
                            om.MinorMajorRect(0, 0, itemMinorSize, itemMajorSize),
                            om.MinorMajorRect(300, 0, itemMinorSize, itemMajorSize)
                        });

                    layout.ItemsJustification = UniformGridLayoutItemsJustification.SpaceAround;
                    panel.UpdateLayout();
                    ValidateChildBounds(
                        panel,
                        new List<Rect>()
                        {
                            om.MinorMajorRect(33, 0, itemMinorSize, itemMajorSize),
                            om.MinorMajorRect(267, 0, itemMinorSize, itemMajorSize)
                        });
                }
            });
        }

        [TestMethod]
        public void ValidateFlowLayout()
        {
            RunOnUIThread.Execute(() =>
            {
                foreach (ScrollOrientation scrollOrientation in Enum.GetValues(typeof(ScrollOrientation)))
                {
                    Log.Comment(string.Format("ScrollOrientation: {0}", scrollOrientation));
                    var om = new OrientationBasedMeasures(scrollOrientation);
                    int [] panelMinorSizeChoices = new int[] { 10, 50 };
                    foreach(int panelMinorSize in panelMinorSizeChoices)
                    {
                        const int numItems = 10;
                        LayoutPanel panel = new LayoutPanel()
                        {
                            Layout = new FlowLayout()
                            {
                                Orientation = scrollOrientation.ToOrthogonalLayoutOrientation(),
                                LineAlignment = FlowLayoutLineAlignment.Start
                            }
                        };

                    SetPanelMinorSize(panel, om, panelMinorSize);
                    for (int i = 0; i < numItems; i++)
                    {
                        panel.Children.Add(new Button() { Content = i });
                    }

                        Content = panel;

                        Content.UpdateLayout();
                        int minItemSpacing = 0;
                        int lineSpacing = 0;
                        ValidateFlowLayoutChildrenLayoutBounds(om, (i) => panel.Children[i], minItemSpacing, lineSpacing, panel.Children.Count, panel.DesiredSize);

                        minItemSpacing = 10;
                        lineSpacing = 10;
                        ((FlowLayout)panel.Layout).MinRowSpacing = minItemSpacing;
                        ((FlowLayout)panel.Layout).MinColumnSpacing = lineSpacing;
                        Content.UpdateLayout();
                        ValidateFlowLayoutChildrenLayoutBounds(om, (i) => panel.Children[i], minItemSpacing, lineSpacing, panel.Children.Count, panel.DesiredSize);
                    }
                }         
            });
        }

        [TestMethod]
        public void ValidateFlowLayoutLineAlignment()
        {
            RunOnUIThread.Execute(() =>
            {
                foreach (ScrollOrientation scrollOrientation in Enum.GetValues(typeof(ScrollOrientation)))
                {
                    Log.Comment(string.Format("ScrollOrientation: {0}", scrollOrientation));
                    var om = new OrientationBasedMeasures(scrollOrientation);
                    const int panelMinorSize = 500;
                    const int itemMajorSize = 100;
                    LayoutPanel panel = new LayoutPanel()
                    {
                        Layout = new FlowLayout()
                        {
                            Orientation = scrollOrientation.ToOrthogonalLayoutOrientation(),
                            LineAlignment = FlowLayoutLineAlignment.Start
                        }
                    };

                    SetPanelMinorSize(panel, om, panelMinorSize);
                    panel.Children.Add(new Button()
                    {
                        Content = 0,
                        Width = om.IsVerical ? 100 : itemMajorSize,
                        Height = om.IsVerical ? itemMajorSize : 100,
                    });

                    panel.Children.Add(new Button()
                    {
                        Content = 1,
                        Width = om.IsVerical ? 200 : itemMajorSize,
                        Height = om.IsVerical ? itemMajorSize : 200,
                    });

                    Content = panel;
                    Content.UpdateLayout();
                    var layout = (FlowLayout)panel.Layout;
                    layout.LineAlignment = FlowLayoutLineAlignment.Center;
                    panel.UpdateLayout();
                    ValidateChildBounds(
                        panel,
                        new List<Rect>()
                        {
                            om.MinorMajorRect(100, 0, 100, itemMajorSize),
                            om.MinorMajorRect(200, 0, 200, itemMajorSize)
                        });

                    layout.LineAlignment = FlowLayoutLineAlignment.End;
                    Content.UpdateLayout();
                    ValidateChildBounds(
                        panel,
                        new List<Rect>()
                        {
                            om.MinorMajorRect(200, 0, 100, itemMajorSize),
                            om.MinorMajorRect(300, 0, 200, itemMajorSize)
                        });

                    layout.LineAlignment = FlowLayoutLineAlignment.SpaceBetween;
                    Content.UpdateLayout();
                    ValidateChildBounds(
                        panel,
                        new List<Rect>()
                        {
                            om.MinorMajorRect(0, 0, 100, itemMajorSize),
                            om.MinorMajorRect(300, 0, 200, itemMajorSize)
                        });
                }
            });
        }

        [TestMethod]
        public void ValidateMakeAnchor()
        {
            RunOnUIThread.Execute(() =>
            {
                foreach (ScrollOrientation scrollOrientation in Enum.GetValues(typeof(ScrollOrientation)))
                {
                    Log.Comment(string.Format("Orientation: {0}", scrollOrientation));
                    var om = new OrientationBasedMeasures(scrollOrientation);

                    const int numItems = 1000;
                    int getElementCallCount = 0;
                    int recycleElementCallCount = 0;
                    var elementFactory = new RecyclingElementFactoryDerived()
                    {
                        Templates = { { "key", GetDataTemplate("<Button Content='{Binding}' />") } },
                        RecyclePool = new RecyclePool(),
                        GetElementFunc = (int index, UIElement owner, UIElement elementFromBase) =>
                        {
                            getElementCallCount++;
                            return elementFromBase;
                        },
                        ClearElementFunc = (UIElement element, UIElement owner) => recycleElementCallCount++,
                    };

                    ItemsRepeater repeater = null;
                    ScrollViewer scrollViewer = null;
                    Content = CreateAndInitializeRepeater
                    (
                       om,
                       itemsSource: Enumerable.Range(0, numItems),
                       elementFactory: elementFactory,
                       layout: new FlowLayoutDerived()
                       {
                           Orientation = scrollOrientation.ToOrthogonalLayoutOrientation(),
                       },
                       repeater: ref repeater,
                       scrollViewer: ref scrollViewer
                    );

                    Content.UpdateLayout();
                    // Validate that we are indeed virtualizing
                    Verify.IsGreaterThan(numItems, elementFactory.RealizedElementIndices.Count);

                    // GetAnchor on an element in view.
                    getElementCallCount = 0;
                    var anchor = repeater.GetOrCreateElement(1);
                    Verify.AreEqual(1, repeater.GetElementIndex(anchor));
                    Verify.AreEqual(0, getElementCallCount);

                    // GetAnchor on element outside the view.
                    recycleElementCallCount = 0;
                    int realizedElementsBeforeMakeAnchor = elementFactory.RealizedElementIndices.Count();
                    anchor = repeater.GetOrCreateElement(900);
                    Verify.AreEqual(900, repeater.GetElementIndex(anchor));
                    Verify.AreEqual(1, getElementCallCount);

                    Content.UpdateLayout();
                    Verify.AreEqual(realizedElementsBeforeMakeAnchor, recycleElementCallCount);
                    Verify.IsLessThan(1, elementFactory.RealizedElementIndices.Count());
                    Verify.IsGreaterThan(150, elementFactory.RealizedElementIndices.Count());

                    Verify.Throws<ArgumentException>(() => repeater.GetOrCreateElement(numItems + 10));
                }
            });
        }

        // TODO: Bug 11136935 - Add test coverage for GetAnchorElement once we have a programatic way to 
        // provide the suggested anchor.
        // Tests should include 
        //- connected, 
        //- disconnected
        //- suggest an anchor that is valid
        //- suggest an anchor that is not valid(some random uielement)
        //- suggest an anchor that on gridlayout that gives a different target index.
        //- !valid anchor index
        //  - change size, itemspacing - getanchorforrealizationrect
        //  - window not connected  - anchorforvisiblerect
        //  - connected window - first in realized range should be picked.

        // Validate that the extent and realized items look fine with connected and disconnected
        // jumps for stack/grid and flow layouts.

        [TestMethod]
        public void ValidateEstimations_Stack_Horizontal()
        {
            ValidateLayoutEstimations(ScrollOrientation.Horizontal, LayoutChoice.Stack);
        }

        [TestMethod]
        public void ValidateEstimations_Stack_Vertical()
        {
            ValidateLayoutEstimations(ScrollOrientation.Vertical, LayoutChoice.Stack);
        }

        [TestMethod]
        public void ValidateEstimations_Grid_Horizontal()
        {
            ValidateLayoutEstimations(ScrollOrientation.Horizontal, LayoutChoice.Grid);
        }

        [TestMethod]
        public void ValidateEstimations_Grid_Vertical()
        {
            ValidateLayoutEstimations(ScrollOrientation.Vertical, LayoutChoice.Grid);
        }

        [TestMethod]
        public void ValidateEstimations_Flow_Horizontal()
        {
            ValidateLayoutEstimations(ScrollOrientation.Horizontal, LayoutChoice.Flow);
        }

        [TestMethod]
        public void ValidateEstimations_Flow_Vertical()
        {
            ValidateLayoutEstimations(ScrollOrientation.Vertical, LayoutChoice.Flow);
        }

        [TestMethod]
        public void VerifyFlowLayoutOnLineArranged()
        {
            RunOnUIThread.Execute(() =>
            {
                foreach (ScrollOrientation scrollOrientation in Enum.GetValues(typeof(ScrollOrientation)))
                {
                    Log.Comment(string.Format("Orientation: {0}", scrollOrientation));
                    var om = new OrientationBasedMeasures(scrollOrientation);
                    const int panelMinorSize = 100;
                    const double itemSize = 50;
                    const int numItems = 5;
                    int onLineArrangedCallCount = 0;
                    LayoutPanel panel = new LayoutPanel()
                    {
                        Layout = new FlowLayoutDerived()
                        {
                            Orientation = scrollOrientation.ToOrthogonalLayoutOrientation(),
                            OnLineArrangedFunc = (int startIndex, int countInLine, double lineSize, VirtualizingLayoutContext context) =>
                            {
                                Verify.AreEqual(0, startIndex % 2);
                                Verify.AreEqual(lineSize, itemSize);
                                // Last line has only one item in it
                                Verify.AreEqual(startIndex == 4 ? 1 : 2, countInLine);
                                onLineArrangedCallCount++;
                            }
                        }
                    };

                    SetPanelMinorSize(panel, om, panelMinorSize);
                    for (int i = 0; i < numItems; i++)
                    {
                        panel.Children.Add(new Button() { Content = i, Width = itemSize, Height = itemSize });
                    }

                    Content = panel;

                    Content.UpdateLayout();

                    Verify.AreEqual(3, onLineArrangedCallCount);
                }
            });
        }

        [TestMethod]
        public void ValidateLayoutSharing()
        {
            RunOnUIThread.Execute(() =>
            {
                foreach (ScrollOrientation scrollOrientation in Enum.GetValues(typeof(ScrollOrientation)))
                {
                    Log.Comment(string.Format("Orientation: {0}", scrollOrientation));
                    var om = new OrientationBasedMeasures(scrollOrientation);
                    const int itemSize = 20;
                    var layouts = new List<VirtualizingLayout>()
                    {
                        new StackLayout(),
                        new UniformGridLayout(){ MinItemWidth = itemSize, MinItemHeight = itemSize },
                        new FlowLayoutDerived()
                    };

                    for (int layoutIndex = 0; layoutIndex < layouts.Count; layoutIndex++)
                    {
                        var repeater1Layout = layouts[layoutIndex];
                        var repeater2Layout = layouts[(layoutIndex + 1) % layouts.Count];
                        Log.Comment(string.Format(" Layouts: {0} and {1}", repeater1Layout.GetType(), repeater2Layout.GetType()));
                        repeater1Layout.SetOrientation(scrollOrientation);
                        repeater2Layout.SetOrientation(scrollOrientation);

                        const int repeater1NumItems = 5;
                        const int repeater2NumItems = 10;
                        var elementFactory = new RecyclingElementFactoryDerived()
                        {
                            Templates = { { "key", GetDataTemplate(@"<Button Content='{Binding}' Width='20' Height='20'/>") } },
                            RecyclePool = new RecyclePool(),
                            ValidateElementIndices = false, // can get same indicies for different repeaters
                        };

                        var repeater1 = new ItemsRepeater()
                        {
                            ItemsSource = Enumerable.Range(0, repeater1NumItems),
                            Layout = repeater1Layout,
#if BUILD_WINDOWS
                            ItemTemplate = (Windows.UI.Xaml.IElementFactory)elementFactory,
#else
                            ItemTemplate = elementFactory,
#endif
                            HorizontalCacheLength = 0,
                            VerticalCacheLength = 0,
                        };

                        var repeater2 = new ItemsRepeater()
                        {
                            ItemsSource = Enumerable.Range(0, repeater2NumItems),
                            Layout = repeater2Layout,
#if BUILD_WINDOWS
                            ItemTemplate = (Windows.UI.Xaml.IElementFactory)elementFactory,
#else
                            ItemTemplate = elementFactory,
#endif
                            HorizontalCacheLength = 0,
                            VerticalCacheLength = 0,
                        };

                        var stack = new StackPanel() { Orientation = scrollOrientation.ToLayoutOrientation() };
                        stack.Children.Add(repeater1);
                        stack.Children.Add(repeater2);

                        var scrollViewer = new ScrollViewer()
                        {
                            Content = stack
                        };

                        SetUpScrollViewerOrientation(scrollViewer, scrollOrientation);
                        Content = new ScrollAnchorProvider()
                        {
                            Width = 400,
                            Height = 400,
                            Content = scrollViewer
                        };

                        Content.UpdateLayout();

                        if (repeater1Layout is StackLayout)
                        {
                            ValidateStackLayoutChildrenLayoutBounds(om, (i) => repeater1.TryGetElement(i), 400, repeater1NumItems, itemSize, 0, repeater1.DesiredSize);
                        }
                        else if (repeater1Layout is UniformGridLayout)
                        {
                            ValidateGridLayoutChildrenLayoutBounds(om, (i) => repeater1.TryGetElement(i), itemSize, itemSize, 0, 0, repeater1NumItems, repeater1.DesiredSize);
                        }
                        else
                        {
                            ValidateFlowLayoutChildrenLayoutBounds(om, (i) => repeater1.TryGetElement(i), 0, 0, repeater1NumItems, repeater1.DesiredSize);
                        }

                        if (repeater2Layout is StackLayout)
                        {
                            ValidateStackLayoutChildrenLayoutBounds(om, (i) => repeater2.TryGetElement(i), 400, repeater2NumItems, itemSize, 0, repeater2.DesiredSize);
                        }
                        else if (repeater2Layout is UniformGridLayout)
                        {
                            ValidateGridLayoutChildrenLayoutBounds(om, (i) => repeater2.TryGetElement(i), itemSize, itemSize, 0, 0, repeater2NumItems, repeater2.DesiredSize);
                        }
                        else
                        {
                            ValidateFlowLayoutChildrenLayoutBounds(om, (i) => repeater2.TryGetElement(i), 0, 0, repeater2NumItems, repeater2.DesiredSize);
                        }
                    }
                }
            });
        }

        [TestMethod]
        [TestProperty("Bug", "12042780")]
        public void ValidateLayoutWithInfiniteMeasureSizeInNonVirtualizingDirection()
        {
            RunOnUIThread.Execute(() =>
            {
                foreach (ScrollOrientation scrollOrientation in Enum.GetValues(typeof(ScrollOrientation)))
                {
                    Log.Comment("ScrollOrientation is " + scrollOrientation);

                    var data = new ObservableCollection<int>(Enumerable.Range(0, 4));
                    var dataSource = MockItemsSource.CreateDataSource(data, supportsUniqueIds: false);
                    var elementFactory = MockElementFactory.CreateElementFactory(
                        Enumerable.Range(0, data.Count).Select(i => new ContentControl
                        {
                            Width = (i + 1) * 40,
                            Height = (i + 1) * 40,
                        }).ToList());

                    var repeater = new ItemsRepeater();
                    repeater.ItemsSource = dataSource;
#if BUILD_WINDOWS
                    repeater.ItemTemplate = (Windows.UI.Xaml.IElementFactory)elementFactory;
#else
                    repeater.ItemTemplate = (Microsoft.UI.Xaml.Controls.IElementFactoryShim)elementFactory;
#endif

                    var scrollViewer = new ScrollViewer();
                    scrollViewer.HorizontalScrollMode = ScrollMode.Enabled;
                    scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                    scrollViewer.Content = repeater;
                    scrollViewer.Width = scrollViewer.Height = 600;

                    var tracker = new ScrollAnchorProvider();
                    tracker.Content = scrollViewer;

                    Content = tracker;
                    Content.UpdateLayout();

                    var layouts = new VirtualizingLayout[]
                    {
                        new StackLayout { Spacing = 10, Orientation = scrollOrientation.ToLayoutOrientation() },
                        new UniformGridLayout { MinItemHeight = 40, MinItemWidth = 40, MinColumnSpacing = 10, MinRowSpacing = 10, Orientation = scrollOrientation.ToOrthogonalLayoutOrientation() },
                        new FlowLayout { MinColumnSpacing = 10, MinRowSpacing = 10, Orientation = scrollOrientation.ToOrthogonalLayoutOrientation() }
                    };

                    var verifyDesiredSize = new Action<Size, Size>((expected, actual) =>
                    {
                        Verify.AreEqual(expected.Width, scrollOrientation == ScrollOrientation.Vertical ? actual.Width : actual.Height);
                        Verify.AreEqual(expected.Height, scrollOrientation == ScrollOrientation.Vertical ? actual.Height : actual.Width);
                    });

                    foreach(var layout in layouts)
                    {
                        Log.Comment("Layout is " + layout.GetType().Name);
                        repeater.Layout = layout;
                        Content.UpdateLayout();
                        
                        if (layout is StackLayout)  verifyDesiredSize(new Size(160, 430), repeater.DesiredSize);
                        if (layout is UniformGridLayout)   verifyDesiredSize(new Size(190, 40), repeater.DesiredSize);
                        if (layout is FlowLayout)   verifyDesiredSize(new Size(430, 160), repeater.DesiredSize);
                    }
                }
            });
        }

        // Bug 17411124: StackLayout: measuring multiple times causes extent to sometimes be 0 in the non-virtualizing direction.
        [TestMethod]
        [TestProperty("Bug", "12052938")]
        public void ValidateIntersectionWithRealizationWindow()
        {
            // In this test, we have a ScrollViewer with a viewport of 200x200.
            // The content is a stack panel with a 300x300 border followed by a ItemsRepeater.
            // At the beginning, the ItemsRepeater doesn't intersect the visible or realization window.
            // The test will wait for the realization window to expand and will validate that we
            // realize elements when it begins to intersect the repeater.

            var createLayouts = new Func<ScrollOrientation, VirtualizingLayout[]>((scrollOrientation) =>
            {
                VirtualizingLayout[] layouts = null;
                RunOnUIThread.Execute(() =>
                {
                    layouts = new VirtualizingLayout[]
                    {
                        new StackLayout { Spacing = 10, Orientation = scrollOrientation.ToLayoutOrientation() },
                        new UniformGridLayout { MinItemHeight = 40, MinItemWidth = 40, MinColumnSpacing = 10, MinRowSpacing = 10, Orientation = scrollOrientation.ToOrthogonalLayoutOrientation() },
                        new FlowLayout { MinColumnSpacing = 10, MinRowSpacing = 10, Orientation = scrollOrientation.ToOrthogonalLayoutOrientation() }
                    };
                });
                return layouts;
            });

            foreach (ScrollOrientation scrollOrientation in Enum.GetValues(typeof(ScrollOrientation)))
            {
                foreach (var layout in createLayouts(scrollOrientation))
                {
                    Log.Comment(" Layout: " + layout.GetType().Name + " ScrollOrientation:" + scrollOrientation.ToString());
                    var repeater = (ItemsRepeater)null;
                    var rangeRealizedEvent = new AutoResetEvent(initialState: false);
                    var data = new ObservableCollection<string>(Enumerable.Range(0, 4).Select(i => "Item #" + i));
                    var mapping = (List<ContentControl>)null;

                    RunOnUIThread.Execute(() =>
                    {
                        mapping = Enumerable.Range(0, data.Count).Select(i => new ContentControl { Width = 40, Height = 40 }).ToList();

                        var dataSource = MockItemsSource.CreateDataSource(data, supportsUniqueIds: false);
                        var elementFactory = MockElementFactory.CreateElementFactory(mapping);

                        repeater = new ItemsRepeater();
                        // Explicitly set a cache length so we dont depend on the defaults.
                        repeater.VerticalCacheLength = 3;
                        repeater.HorizontalCacheLength = 3;
                        repeater.ItemsSource = dataSource;
#if BUILD_WINDOWS
                        repeater.ItemTemplate = (Windows.UI.Xaml.IElementFactory)elementFactory;
#else
                        repeater.ItemTemplate = (Microsoft.UI.Xaml.Controls.IElementFactoryShim)elementFactory;
#endif
                        repeater.Layout = layout;

                        var stack = new StackPanel();
                        stack.Orientation = scrollOrientation.ToLayoutOrientation();
                        stack.Children.Add(new Border { Width = 300, Height = 300 });
                        stack.Children.Add(repeater);

                        var scrollViewer = new ScrollViewer();
                        scrollViewer.Content = stack;
                        scrollViewer.Width = scrollViewer.Height = 200;
                        SetUpScrollViewerOrientation(scrollViewer, scrollOrientation);

                        var tracker = new ScrollAnchorProvider();
                        tracker.Content = scrollViewer;
                        Content = tracker;

                        repeater.ElementPrepared += (o, e) =>
                        {
                            if (e.Index == data.Count - 1)
                            {
                                for (int i = 0; i < data.Count; ++i)
                                {
                                    Verify.AreEqual(mapping[i], repeater.TryGetElement(i));
                                }

                                rangeRealizedEvent.Set();
                            }
                        };

                        Content.UpdateLayout();
                    });

                    Verify.IsTrue(rangeRealizedEvent.WaitOne(DefaultWaitTimeInMS), "Waiting for all items to be realized as the realization window expands.");
                }
            }
        }

        [TestMethod]
        public void ValidateFlowLayoutWithOneItemHasNonZeroExtent()
        {
            RunOnUIThread.Execute(() =>
            {
                foreach (ScrollOrientation scrollOrientation in Enum.GetValues(typeof(ScrollOrientation)))
                {
                    var repeater = new ItemsRepeater() {
                        ItemsSource = new List<string>() { "single item" },
                        Layout = new FlowLayout() { Orientation = scrollOrientation.ToLayoutOrientation() },
                    };

                    Content = repeater;
                    Content.UpdateLayout();

                    Verify.IsTrue(repeater.ActualWidth > 10);
                    Verify.IsTrue(repeater.ActualHeight > 10);
                }
            });
        }

        #region Private Helpers

        private enum LayoutChoice
        {
            Stack,
            Grid,
            Flow
        }

        private enum DimensionChoice
        {
            Width,
            Height,
            Size
        }

        private void ValidateStackLayoutChildrenLayoutBounds(
            OrientationBasedMeasures om,
            Func<int, UIElement> elementAtIndexFunc,
            int panelMinorSize,
            int numItems,
            int itemMajorSize,
            int itemSpacing,
            Size desiredSize)
        {
            Rect expectedRect = om.MinorMajorRect(0, 0, panelMinorSize, itemMajorSize);
            for (int i = 0; i < numItems; i++)
            {
                var child = (FrameworkElement)elementAtIndexFunc(i);
                var actualRect = LayoutInformation.GetLayoutSlot(child);
                Verify.AreEqual(expectedRect, actualRect);

                om.SetMajorStart(ref expectedRect, om.MajorStart(expectedRect) + itemMajorSize + itemSpacing);
            }

            Verify.AreEqual(numItems * itemMajorSize + (numItems - 1) * itemSpacing, om.Major(desiredSize));
        }

        private void ValidateGridLayoutChildrenLayoutBounds(
            OrientationBasedMeasures om,
            Func<int, UIElement> elementAtIndexFunc,
            double itemMinorSize,
            double itemMajorSize,
            double minItemSpacing,
            double lineSpacing,
            int childCount,
            Size desiredSize)
        {
            var expectedRect = om.MinorMajorRect(0, 0, itemMinorSize, itemMajorSize);
            double extentMajor = 0;
            for (int i = 0; i < childCount; i++)
            {
                var child = (FrameworkElement)elementAtIndexFunc(i);
                var layoutBounds = LayoutInformation.GetLayoutSlot(child);

                Verify.AreEqual(expectedRect, layoutBounds);

                extentMajor = om.MajorStart(expectedRect) + itemMajorSize;
                om.SetMinorStart(ref expectedRect, om.MinorStart(expectedRect) + itemMinorSize + minItemSpacing);
                if (om.MinorStart(expectedRect) + itemMinorSize + minItemSpacing > om.Minor(desiredSize))
                {
                    om.SetMinorStart(ref expectedRect, 0);
                    om.SetMajorStart(ref expectedRect, om.MajorStart(expectedRect) + itemMajorSize + lineSpacing);
                }
            }

            Verify.AreEqual(extentMajor, om.Major(desiredSize));
        }

        private void ValidateFlowLayoutChildrenLayoutBounds(
            OrientationBasedMeasures om,
            Func<int, UIElement> elementAtIndexFunc,
            double minItemSpacing,
            double lineSpacing,
            int childCount,
            Size desiredSize)
        {
            var expectedRect = om.MinorMajorRect(0, 0, 0, 0);
            double extentMajor = 0;
            double lineSize = 0;
            for (int i = 0; i < childCount; i++)
            {
                var child = (FrameworkElement)elementAtIndexFunc(i);

                var layoutBounds = LayoutInformation.GetLayoutSlot(child);
                expectedRect.Width = child.DesiredSize.Width;
                expectedRect.Height = child.DesiredSize.Height;
                lineSize = Math.Max(lineSize, om.Major(child.DesiredSize));

                Verify.AreEqual(expectedRect, layoutBounds);

                extentMajor = om.MajorStart(expectedRect) + lineSize;
                om.SetMinorStart(ref expectedRect, om.MinorStart(expectedRect) + om.Minor(child.DesiredSize) + minItemSpacing);
                if (om.MinorStart(expectedRect) + om.Minor(child.DesiredSize) + minItemSpacing > om.Minor(desiredSize))
                {
                    om.SetMinorStart(ref expectedRect, 0);
                    om.SetMajorStart(ref expectedRect, om.MajorStart(expectedRect) + lineSize + lineSpacing);
                }
            }

            Verify.AreEqual(extentMajor, om.Major(desiredSize));
        }

        private void ValidateChildBounds(LayoutPanel panel, List<Rect> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var child = (FrameworkElement)panel.Children[i];
                Verify.AreEqual(LayoutInformation.GetLayoutSlot(child), list[i]);
            }
        }

        private DataTemplate GetDataTemplate(string content)
        {
            return (DataTemplate)XamlReader.Load(
                       string.Format(@"<DataTemplate  
                            xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
                           {0}
                        </DataTemplate>", content));
        }

        private ScrollAnchorProvider CreateAndInitializeRepeater(
           OrientationBasedMeasures om,
           object itemsSource,
           VirtualizingLayout layout,
           ElementFactory elementFactory,
           ref ItemsRepeater repeater,
           ref ScrollViewer scrollViewer)
        {
            repeater = new ItemsRepeater()
            {
                ItemsSource = itemsSource,
                Layout = layout,
#if BUILD_WINDOWS
                ItemTemplate = (Windows.UI.Xaml.IElementFactory)elementFactory,
#else
                ItemTemplate = elementFactory,
#endif
                HorizontalCacheLength = 0,
                VerticalCacheLength = 0,
            };

            scrollViewer = new ScrollViewer()
            {
                Content = repeater
            };

            SetUpScrollViewerOrientation(scrollViewer, om.ScrollOrientation);
            return new ScrollAnchorProvider()
            {
                Width = 400,
                Height = 400,
                Content = scrollViewer
            };
        }

        private void ValidateLayoutEstimations(ScrollOrientation scrollOrientation, LayoutChoice layoutChoice)
        {
            Log.Comment(string.Format("ScrollOrientation: {0}", scrollOrientation));
            var om = new OrientationBasedMeasures(scrollOrientation);
            List<VirtualizingLayout> layouts = new List<VirtualizingLayout>();
            const double itemSize = 90;
            const double lineSpacing = 10;
            double itemSizeWithLineSpacing = itemSize + lineSpacing;
            VirtualizingLayout layout = null;
            var viewChangedEvent = new ManualResetEvent(false);

            RunOnUIThread.Execute(() =>
            {
                switch (layoutChoice)
                {
                    case LayoutChoice.Stack:
                        layout = new StackLayout() { Spacing = lineSpacing };
                        break;

                    case LayoutChoice.Grid:

                        layout = new UniformGridLayout() { MinItemWidth = itemSize, MinItemHeight = itemSize, MinColumnSpacing = lineSpacing };
                        break;

                    case LayoutChoice.Flow:
                        layout = new FlowLayoutDerived()
                        {
                            MinColumnSpacing = lineSpacing,
                            OnLineArrangedFunc = (int startIndex, int countInLine, double lineSize, VirtualizingLayoutContext context) =>
                            {
                                Verify.AreEqual(0, startIndex % 4);
                                Verify.AreEqual(4, countInLine);
                                Verify.AreEqual(lineSize, itemSize);
                            }
                        };
                        break;

                    default:
                        throw new InvalidOperationException("Unhanlded layout choice.");
                }
            });

            ItemsRepeater repeater = null;
            RecyclingElementFactoryDerived elementFactory = null;
            ScrollViewer scrollViewer = null;
            const int numItems = 1000;
            var numColumns = (int)(400 / itemSize);
            var numRows = numItems / numColumns;

            RunOnUIThread.Execute(() =>
            {
                Log.Comment(string.Format(" Layout: {0}", layout.GetType()));
                layout.SetOrientation(scrollOrientation);

                elementFactory = new RecyclingElementFactoryDerived()
                {
                    Templates = { { "key", GetDataTemplate(@"<Button Content='{Binding}' Width='90' Height='90'/>") } },
                    RecyclePool = new RecyclePool(),
                };

                Content = CreateAndInitializeRepeater
                (
                   om,
                   itemsSource: Enumerable.Range(0, numItems),
                   elementFactory: elementFactory,
                   layout: layout,
                   repeater: ref repeater,
                   scrollViewer: ref scrollViewer
                );

                scrollViewer.ViewChanged += (sender, args) =>
                {
                    if (!args.IsIntermediate)
                    {
                        viewChangedEvent.Set();
                    }
                };

                Content.UpdateLayout();
                if (layout is StackLayout)
                {
                    Verify.AreEqual(numItems * itemSizeWithLineSpacing - lineSpacing, om.Major(repeater.DesiredSize));
                }
                else if (layout is UniformGridLayout || layout is FlowLayout)
                {
                    Verify.AreEqual(numRows * itemSizeWithLineSpacing - lineSpacing, om.Major(repeater.DesiredSize));
                }
            });

            // Move window - connected
            IdleSynchronizer.Wait();

            RunOnUIThread.Execute(() =>
            {
                var success = om.IsVerical ?
                    scrollViewer.ChangeView(null, 200, null, true) :
                    scrollViewer.ChangeView(200, null, null, true);
            });

            Verify.IsTrue(viewChangedEvent.WaitOne(DefaultWaitTime), "Waiting for ViewChanged.");
            IdleSynchronizer.Wait();

            RunOnUIThread.Execute(() =>
            {
                if (layout is StackLayout)
                {
                    Verify.AreEqual(numItems * itemSizeWithLineSpacing - lineSpacing, om.Major(repeater.DesiredSize));
                }
                else if (layout is UniformGridLayout || layout is FlowLayout)
                {
                    Verify.AreEqual(numRows * itemSizeWithLineSpacing - lineSpacing, om.Major(repeater.DesiredSize));
                }
            });

            // Move window - disconnected
            RunOnUIThread.Execute(() =>
            {
                viewChangedEvent.Reset();
                var success = om.IsVerical ?
                   scrollViewer.ChangeView(null, 20000, null, true) :
                   scrollViewer.ChangeView(20000, null, null, true);
            });

            Verify.IsTrue(viewChangedEvent.WaitOne(DefaultWaitTime), "Waiting for ViewChanged.");
            IdleSynchronizer.Wait();

            RunOnUIThread.Execute(() =>
            {
                if (layout is StackLayout)
                {
                    VerifyRealizedRange(expectedStartindex: 199, expectedCount: 6, actualIndices: elementFactory.RealizedElementIndices);
                    Verify.AreEqual(numItems * itemSizeWithLineSpacing - lineSpacing, om.Major(repeater.DesiredSize));
                }
                else if (layout is UniformGridLayout)
                {
                    // We have to copy the list we're passing, as any modification in the realizedElementIndices will result in 
                    // altering the behavior and preventing the recycling of the containers.
                    List<int> observedIndices = new List<int>(elementFactory.RealizedElementIndices);
                    observedIndices.Remove(0);
                    VerifyRealizedRange(expectedStartindex: 799, expectedCount: 18, actualIndices: observedIndices);
                    Verify.AreEqual(numRows * itemSizeWithLineSpacing - lineSpacing, om.Major(repeater.DesiredSize));
                }
                else if (layout is FlowLayout)
                {
                    VerifyRealizedRange(expectedStartindex: 799, expectedCount: 18, actualIndices: elementFactory.RealizedElementIndices);
                    Verify.AreEqual(numRows * itemSizeWithLineSpacing - lineSpacing, om.Major(repeater.DesiredSize));
                }

            });

            // Validate Ability to override Extent using a disconnected move
            RunOnUIThread.Execute(() =>
            {
                if (layout is FlowLayoutDerived)
                {
                    var flowLayout = (FlowLayoutDerived)layout;
                    flowLayout.GetExtentFunc = (
                       Size availableSize,
                       VirtualizingLayoutContext context,
                       UIElement firstRealized,
                       int firstRealizedItemIndex,
                       Rect firstRealizedLayoutBounds,
                       UIElement lastRealized,
                       int lastRealizedItemIndex,
                       Rect lastRealizedLayoutBounds,
                       Rect fromBase) =>
                    {
                        return om.MinorMajorRect(0, 0, 400, numRows * itemSize);
                    };
                }

                viewChangedEvent.Reset();
                var success = om.IsVerical ?
                   scrollViewer.ChangeView(null, 10000, null, true) :
                   scrollViewer.ChangeView(10000, null, null, true);
            });

            Verify.IsTrue(viewChangedEvent.WaitOne(DefaultWaitTime), "Waiting for ViewChanged.");
            IdleSynchronizer.Wait();

            RunOnUIThread.Execute(() =>
            {
                if (layout is StackLayout)
                {
                    Verify.AreEqual(numItems * itemSizeWithLineSpacing - lineSpacing, om.Major(repeater.DesiredSize));
                }
                else if (layout is UniformGridLayout)
                {
                    Verify.AreEqual(numRows * itemSizeWithLineSpacing - lineSpacing, om.Major(repeater.DesiredSize));
                }
                else if (layout is FlowLayout)
                {
                    // Extent overriden using GetExtent override.
                    Verify.AreEqual(numRows * itemSize, om.Major(repeater.DesiredSize));
                }
            });
        }

        private void ValidateDimensionForGridLayoutItemsIsBasedOnFirstItem(DimensionChoice dimension)
        {
            LayoutPanel panel = null;
            RunOnUIThread.Execute(() =>
            {
                foreach (ScrollOrientation scrollOrientation in Enum.GetValues(typeof(ScrollOrientation)))
                {
                    panel = new LayoutPanel()
                    {
                        Layout = new UniformGridLayout()
                        {
                            Orientation = scrollOrientation.ToOrthogonalLayoutOrientation(),
                        }
                    };

                    PopulatePanelWithButtons(panel, 10 /*numItems*/);
                    Content = panel;
                    Content.UpdateLayout();

                    double widthBeforeUpdate = double.NaN;
                    for (int i = 0; i < panel.Children.Count; i++)
                    {
                        var child = (FrameworkElement)panel.Children.ElementAt(i);
                        var layoutBounds = LayoutInformation.GetLayoutSlot(child);

                        // Ensure bounds before updating.
                        Verify.AreNotEqual(100, layoutBounds.Width);
                        widthBeforeUpdate = layoutBounds.Width;
                        Verify.AreEqual(32 /* itemHeight */, layoutBounds.Height);
                    }

                    switch (dimension)
                    {
                        case DimensionChoice.Width:
                            ModifyElementSize(panel, 0 /*index*/, 100 /*itemWidth*/, Double.NaN /*itemHeight*/);
                            Content.InvalidateMeasure(); // Bug 16644056
                            Content.UpdateLayout();
                            ValidateElementsSizeInGridLayout(panel, 100 /*itemWidth*/, 32 /*itemHeight*/);
                            break;

                        case DimensionChoice.Height:

                            ModifyElementSize(panel, 0 /*index*/, Double.NaN /*itemWidth*/, 100 /*itemHeight*/);
                            Content.InvalidateMeasure(); // Bug 16644056
                            Content.UpdateLayout();
                            ValidateElementsSizeInGridLayout(panel, widthBeforeUpdate /*itemWidth*/, 100 /*itemHeight*/);
                            break;

                        case DimensionChoice.Size:
                            ModifyElementSize(panel, 0 /*index*/, 100 /*itemWidth*/, 100 /*itemHeight*/);
                            Content.InvalidateMeasure(); // Bug 16644056
                            Content.UpdateLayout();
                            ValidateElementsSizeInGridLayout(panel, 100 /*itemWidth*/, 100 /*itemHeight*/);
                            break;

                        default:
                            throw new InvalidOperationException("Unhanlded dimension choice.");
                    }
                }
            });
        }

        private void ValidateAdaptabilityWhenChangingTheFirstElementForGridLayout(DimensionChoice dimension)
        {
            RunOnUIThread.Execute(() =>
            {
                foreach (ScrollOrientation scrollOrientation in Enum.GetValues(typeof(ScrollOrientation)))
                {
                    LayoutPanel panel = new LayoutPanel()
                    {
                        Layout = new UniformGridLayout()
                        {
                            Orientation = scrollOrientation.ToOrthogonalLayoutOrientation(),
                        }
                    };

                    PopulatePanelWithButtons(panel, 10);
                    Content = panel;
                    Content.UpdateLayout();

                    double widthBeforeUpdate = double.NaN;
                    for (int i = 0; i < panel.Children.Count; i++)
                    {
                        var child = (FrameworkElement)panel.Children.ElementAt(i);
                        var layoutBounds = LayoutInformation.GetLayoutSlot(child);

                        // Ensure bounds before updating.
                        Verify.AreNotEqual(100, layoutBounds.Width);
                        widthBeforeUpdate = layoutBounds.Width;
                        Verify.AreEqual(32 /* itemHeight */, layoutBounds.Height);
                    }

                    switch (dimension)
                    {
                        case DimensionChoice.Width:
                            ModifyElementSize(panel, 1 /*index*/, 100 /*itemWidth*/, Double.NaN /*itemHeight*/);
                            break;

                        case DimensionChoice.Height:

                            ModifyElementSize(panel, 1 /*index*/, Double.NaN /*itemWidth*/, 100 /*itemHeight*/);
                            break;

                        case DimensionChoice.Size:
                            ModifyElementSize(panel, 1 /*index*/, 100 /*itemWidth*/, 100 /*itemHeight*/);
                            break;

                        default:
                            throw new InvalidOperationException("Unhanlded dimension choice.");
                    }

                    panel.Children.RemoveAt(0);
                    Content.UpdateLayout();

                    switch (dimension)
                    {
                        case DimensionChoice.Width:
                            ValidateElementsSizeInGridLayout(panel, 100 /*itemWidth*/, 32 /*itemHeight*/);
                            break;

                        case DimensionChoice.Height:
                            ValidateElementsSizeInGridLayout(panel, widthBeforeUpdate /*itemWidth*/, 100 /*itemHeight*/);
                            break;

                        case DimensionChoice.Size:
                            ValidateElementsSizeInGridLayout(panel, 100 /*itemWidth*/, 100 /*itemHeight*/);
                            break;

                        default:
                            throw new InvalidOperationException("Unhanlded dimension choice.");
                    }

                }
            });
        }

        private static void VerifyWithinTolerance(double expected, double actual, double tolerance)
        {
            Log.Comment(string.Format("Verify: WithinTolerance({0}, {1}, {2})", expected, actual, tolerance));
            if (Math.Abs(expected - actual) > tolerance)
            {
                Verify.Fail(string.Format("AreEqual failed to be within tolerance: {0}:{1}:{2}", expected, actual, tolerance));
            }
        }

        private static void SetUpScrollViewerOrientation(ScrollViewer scrollViewer, ScrollOrientation scrollOrientation)
        {
            if (scrollOrientation == ScrollOrientation.Vertical)
            {
                scrollViewer.SetValue(ScrollViewer.HorizontalScrollModeProperty, ScrollMode.Disabled);
                scrollViewer.SetValue(ScrollViewer.VerticalScrollModeProperty, ScrollMode.Enabled);
                scrollViewer.SetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled);
                scrollViewer.SetValue(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Visible);
            }
            else
            {
                scrollViewer.SetValue(ScrollViewer.HorizontalScrollModeProperty, ScrollMode.Enabled);
                scrollViewer.SetValue(ScrollViewer.VerticalScrollModeProperty, ScrollMode.Disabled);
                scrollViewer.SetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Visible);
                scrollViewer.SetValue(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled);
            }
        }

        private static void VerifyRealizedRange(int expectedStartindex, int expectedCount, List<int> actualIndices)
        {
            Verify.AreEqual(expectedCount, actualIndices.Count);
            for (int i = expectedStartindex; i < expectedStartindex + expectedCount; i++)
            {
                Verify.IsTrue(actualIndices.Contains(i));
            }
        }
        

        private void SetPanelMinorSize(LayoutPanel panel , OrientationBasedMeasures om, double value)
        {
            if (om.IsVerical)
            {
                panel.Width = value;
            }
            else
            {
                panel.Height = value;
            }

        }

        private void PopulatePanelWithButtons(LayoutPanel panel, int numItems)
        {
            for (int i = 0; i < numItems; i++)
            {
                panel.Children.Add(new Button() { Content = i.ToString() });
            }
        }

        private static void ValidateElementsSizeInGridLayout(LayoutPanel panel, double itemWidth, double itemHeight)
        {
            for (int i = 0; i < panel.Children.Count; i++)
            {
                var child = (FrameworkElement) panel.Children.ElementAt(i);
                var layoutBounds = LayoutInformation.GetLayoutSlot(child);

                Verify.AreEqual(itemWidth, layoutBounds.Width);
                Verify.AreEqual(itemHeight, layoutBounds.Height);
            }
        }

        private static void ModifyElementSize(LayoutPanel panel, int index, double itemWidth, double itemHeight)
        {
            var item = (FrameworkElement) panel.Children.ElementAt(index);

            if (!Double.IsNaN(itemWidth))
            {
                item.Width = itemWidth;
            }
            if (!Double.IsNaN(itemHeight))
            {
                item.Height = itemHeight;
            }
        }

        private int DefaultWaitTime = 2000;

        #endregion
    }
}
