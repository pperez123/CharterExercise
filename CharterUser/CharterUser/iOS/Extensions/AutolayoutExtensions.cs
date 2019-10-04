// C# Port of UIView+SDCAutoLayout.m - https://github.com/sberrevoets/SDCAutoLayout
// Convenience methods to simplify NSLayoutConstraint creation.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using CoreGraphics;
using UIKit;

namespace CharterUser.iOS.Extensions
{
    public struct UIOffset
    {
        public float X, Y;

        public UIOffset(float horizontal = 0, float vertical = 0)
        {
            X = horizontal;
            Y = vertical;
        }
    }

    [Flags]
    public enum RectEdge
    {
        None = 0,
        Top = 1 << 0,
        Left = 1 << 1,
        Right = 1 << 2,
        Bottom = 1 << 3,
        All = Top | Left | Right | Bottom,
        Leading = Left,
        Trailing = Right
    }

    public static class AutolayoutExtensions
    {
        /// <summary>
        /// Helper method that returns the first common ancestor of self and view.
        /// Not needed for OSX >= 10.10 since constraintObject.Active = true will add the 
        /// constraint to the common ancestor.
        /// </summary>
        /// <returns>The ancestor UIView</returns>
        /// <param name="view">View.</param>
        /// <param name="targetView">Target view.</param>
        public static UIView CommonAncestor(this UIView view, UIView targetView)
        {
            if (targetView.IsDescendantOfView(view))
            {
                return view;
            }

            if (view.IsDescendantOfView(targetView))
            {
                return targetView;
            }

            var superView = view.Superview;

            while (!targetView.IsDescendantOfView(superView))
            {
                superView = superView.Superview;

                if (superView == null) break;
            }

            return superView;
        }

        // Edge Alignment

        public static NSLayoutAttribute LayoutAttributeForEdge(RectEdge edge)
        {
            NSLayoutAttribute attribute;

            switch (edge)
            {
                case RectEdge.Top:
                    attribute = NSLayoutAttribute.Top;
                    break;
                case RectEdge.Right:
                    attribute = NSLayoutAttribute.Trailing;
                    break;
                case RectEdge.Bottom:
                    attribute = NSLayoutAttribute.Bottom;
                    break;
                case RectEdge.Left:
                    attribute = NSLayoutAttribute.Leading;
                    break;
                case RectEdge.None:
                    attribute = NSLayoutAttribute.NoAttribute;
                    break;
                case RectEdge.All:
                    attribute = NSLayoutAttribute.NoAttribute;
                    break;
                default:
                    attribute = NSLayoutAttribute.NoAttribute;
                    break;
            }

            return attribute;
        }

        public static NSLayoutConstraint AlignEdge(this UIView view, RectEdge edge, UIView targetView, float inset = 0)
        {
            return view.AlignEdge(edge, targetView, edge, inset);
        }

        public static NSLayoutConstraint AlignEdge(this UIView view, RectEdge edge, UIView targetView, RectEdge targetEdge, float inset = 0)
        {
            var attribute = LayoutAttributeForEdge(edge);
            var targetAttribute = LayoutAttributeForEdge(targetEdge);

            if (attribute == NSLayoutAttribute.NoAttribute || targetAttribute == NSLayoutAttribute.NoAttribute)
                return null;

            var constraint = NSLayoutConstraint.Create(view, attribute, NSLayoutRelation.Equal, targetView, targetAttribute, 1, inset);
            constraint.Active = true;

            return constraint;
        }

        /// <summary>
        /// Aligning a view's edges with another view
        /// </summary>
        /// <returns>NSLayoutConstraints</returns>
        /// <param name="view">View.</param>
        /// <param name="edges">Edges.</param>
        /// <param name="targetView">Target view.</param>
        /// <param name="insets">Insets.</param>
        public static NSLayoutConstraint[] AlignEdges(this UIView view, RectEdge edges, UIView targetView, UIEdgeInsets insets = default(UIEdgeInsets))
        {
            var constraints = new List<NSLayoutConstraint>();

            if (edges.HasFlag(RectEdge.Top))
            {
                constraints.Add(view.AlignEdge(RectEdge.Top, targetView, (float)insets.Top));
            }

            if (edges.HasFlag(RectEdge.Right))
            {
                constraints.Add(view.AlignEdge(RectEdge.Right, targetView, (float)insets.Right));
            }

            if (edges.HasFlag(RectEdge.Bottom))
            {
                constraints.Add(view.AlignEdge(RectEdge.Bottom, targetView, (float)insets.Bottom));
            }

            if (edges.HasFlag(RectEdge.Left))
            {
                constraints.Add(view.AlignEdge(RectEdge.Left, targetView, (float)insets.Left));
            }

            return constraints.ToArray();
        }

        /// <summary>
        /// Aligning a view's edges with its superview
        /// This method aligns the superview's trailing and bottom edges with child view's trailing and bottom edges so that
        /// the superview resizes according to child dimensions
        /// </summary>
        /// <returns>NSLayoutConstraint's</returns>
        /// <param name="view">View.</param>
        /// <param name="edges">Edges.</param>
        /// <param name="insets">Insets.</param>
        public static NSLayoutConstraint[] AlignEdgesWithSuperview(this UIView view, RectEdge edges = RectEdge.All, UIEdgeInsets insets = default(UIEdgeInsets))
        {
            Debug.Assert(view.Superview != null, "View must have a super view.");

            var constraints = new List<NSLayoutConstraint>();

            if (edges.HasFlag(RectEdge.Top))
            {
                constraints.Add(view.AlignEdge(RectEdge.Top, view.Superview, (float)insets.Top));
            }

            if (edges.HasFlag(RectEdge.Right))
            {
                constraints.Add(view.Superview.AlignEdge(RectEdge.Right, view, (float)insets.Right));
            }

            if (edges.HasFlag(RectEdge.Bottom))
            {
                constraints.Add(view.Superview.AlignEdge(RectEdge.Bottom, view, (float)insets.Bottom));
            }

            if (edges.HasFlag(RectEdge.Left))
            {
                constraints.Add(view.AlignEdge(RectEdge.Left, view.Superview, (float)insets.Left));
            }

            return constraints.ToArray();
        }

        public static NSLayoutConstraint AlignEdgeWithSuperview(this UIView view, RectEdge edge, float inset = 0)
        {
            return view.AlignEdge(edge, view.Superview, inset);
        }

        // Aligning a view's center with another view

        public static NSLayoutConstraint AlignVerticalCenter(this UIView view, UIView targetView, float offset = 0)
        {
            var constraint = NSLayoutConstraint.Create(view, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, targetView, NSLayoutAttribute.CenterY, 1, offset);
            constraint.Active = true;

            return constraint;
        }

        public static NSLayoutConstraint AlignHorizontalCenter(this UIView view, UIView targetView, float offset = 0)
        {
            var constraint = NSLayoutConstraint.Create(view, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, targetView, NSLayoutAttribute.CenterX, 1, offset);
            constraint.Active = true;

            return constraint;
        }

        public static NSLayoutConstraint[] AlignCenters(this UIView view, UIView targetView, UIOffset offset = default(UIOffset))
        {
            return new[] { view.AlignHorizontalCenter(targetView, offset.X), view.AlignVerticalCenter(targetView, offset.Y) };
        }

        // Centering a view in its superview

        public static NSLayoutConstraint VerticallyCenterInSuperview(this UIView view, float offset = 0)
        {
            NSLayoutConstraint constraint = null;

            if (view.Superview != null)
            {
                constraint = NSLayoutConstraint.Create(view, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, view.Superview, NSLayoutAttribute.CenterY, 1, offset);
                constraint.Active = true;
            }

            return constraint;
        }

        public static NSLayoutConstraint HorizontallyCenterInSuperview(this UIView view, float offset = 0)
        {
            NSLayoutConstraint constraint = null;

            if (view.Superview != null)
            {
                constraint = NSLayoutConstraint.Create(view, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, view.Superview, NSLayoutAttribute.CenterX, 1, offset);
                constraint.Active = true;
            }

            return constraint;
        }

        public static NSLayoutConstraint[] CenterInSuperview(this UIView view, UIOffset offset = default(UIOffset))
        {
            return new [] { view.HorizontallyCenterInSuperview(offset.X), view.VerticallyCenterInSuperview(offset.Y) };
        }

        /// <summary>
        /// Align a view's baseline with another view
        /// </summary>
        /// <returns>NSLayoutConstraint</returns>
        /// <param name="view">View.</param>
        /// <param name="targetView">Target view.</param>
        /// <param name="offset">Offset.</param>
        public static NSLayoutConstraint AlignBaseline(this UIView view, UIView targetView, float offset = 0)
        {
            var constraint = NSLayoutConstraint.Create(view, NSLayoutAttribute.Baseline, NSLayoutRelation.Equal, targetView, NSLayoutAttribute.Baseline, 1, offset);
            constraint.Active = true;

            return constraint;
        }

        // Pinning a view's dimensions with constants

        // Pinning Width Constants

        public static NSLayoutConstraint PinWidth(this UIView view, float width)
        {
            var constraint = NSLayoutConstraint.Create(view, NSLayoutAttribute.Width, NSLayoutRelation.Equal, 1, width);
            constraint.Active = true;
            return constraint;
        }

        public static NSLayoutConstraint SetMinimumWidth(this UIView view, float width)
        {
            var constraint = NSLayoutConstraint.Create(view, NSLayoutAttribute.Width, NSLayoutRelation.GreaterThanOrEqual, 1, width);
            constraint.Active = true;
            return constraint;
        }

        public static NSLayoutConstraint SetMaximumWidth(this UIView view, float width)
        {
            var constraint = NSLayoutConstraint.Create(view, NSLayoutAttribute.Width, NSLayoutRelation.LessThanOrEqual, 1, width);
            constraint.Active = true;
            return constraint;
        }

        public static NSLayoutConstraint SetMaximumWidthToSuperviewWidth(this UIView view, float offset = 0)
        {
            var constraint = NSLayoutConstraint.Create(view, NSLayoutAttribute.Width, NSLayoutRelation.LessThanOrEqual, view.Superview, NSLayoutAttribute.Width, 1, offset);
            constraint.Active = true;
            return constraint;
        }

        // Pinning Height Constants

        public static NSLayoutConstraint PinHeight(this UIView view, float height)
        {
            var constraint = NSLayoutConstraint.Create(view, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1, height);
            constraint.Active = true;
            return constraint;
        }

        public static NSLayoutConstraint SetMinimumHeight(this UIView view, float width)
        {
            var constraint = NSLayoutConstraint.Create(view, NSLayoutAttribute.Height, NSLayoutRelation.GreaterThanOrEqual, 1, width);
            constraint.Active = true;
            return constraint;
        }

        public static NSLayoutConstraint SetMaximumHeight(this UIView view, float width)
        {
            var constraint = NSLayoutConstraint.Create(view, NSLayoutAttribute.Height, NSLayoutRelation.LessThanOrEqual, 1, width);
            constraint.Active = true;
            return constraint;
        }

        public static NSLayoutConstraint SetMaximumHeightToSuperviewHeight(this UIView view, float offset = 0)
        {
            var constraint = NSLayoutConstraint.Create(view, NSLayoutAttribute.Height, NSLayoutRelation.LessThanOrEqual, view.Superview, NSLayoutAttribute.Height, 1, offset);
            constraint.Active = true;
            return constraint;
        }

        public static NSLayoutConstraint[] PinSize(this UIView view, CGSize size)
        {
            return new [] { view.PinWidth((float)size.Width), view.PinHeight((float)size.Height) };
        }

        public static NSLayoutConstraint[] PinSize(this UIView view, float width, float height)
        {
            return new[] { view.PinWidth(width), view.PinHeight(height) };
        }

        // Pinning Views

        public static NSLayoutConstraint PinWidthToViewWidth(this UIView view, UIView targetView, float offset = 0, float multiplier = 1)
        {
            var constraint = NSLayoutConstraint.Create(view, NSLayoutAttribute.Width, NSLayoutRelation.Equal, targetView, NSLayoutAttribute.Width, multiplier, offset);
            constraint.Active = true;
            return constraint;
        }

        public static NSLayoutConstraint PinHeightToViewHeight(this UIView view, UIView targetView, float offset = 0)
        {
            var constraint = NSLayoutConstraint.Create(view, NSLayoutAttribute.Height, NSLayoutRelation.Equal, targetView, NSLayoutAttribute.Height, 1, offset);
            constraint.Active = true;
            return constraint;
        }

        public static NSLayoutConstraint[] PinSizeToViewSize(this UIView view, UIView targetView, UIOffset offset = default(UIOffset))
        {
            return new [] { view.PinWidthToViewWidth(targetView, offset.X), view.PinHeightToViewHeight(targetView, offset.Y) };
        }

        /// <summary>
        /// Setting the spacing between a view and other view
        /// A positive spacing (or 0) means view will be placed to the right of the targetView.
        /// A negative spacing means view will be placed to the left of targetView.
        /// </summary>
        /// <returns>NSLayoutConstraint</returns>
        /// <param name="view">View.</param>
        /// <param name="targetView">Target view.</param>
        /// <param name="spacing">Spacing.</param>
        public static NSLayoutConstraint PinHorizontalSpacing(this UIView view, UIView targetView, float spacing = 0)
        {
            var constraint = spacing >= 0 ? NSLayoutConstraint.Create(view, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, targetView, NSLayoutAttribute.Trailing, 1, spacing) : 
                NSLayoutConstraint.Create(view, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, targetView, NSLayoutAttribute.Leading, 1, spacing);
            
            constraint.Active = true;

            return constraint;
        }

        /// <summary>
        /// A positive spacing (or 0) means view will be placed below the targetView.
        /// A negative spacing means view will be placed above the targetView.
        /// </summary>
        /// <returns>NSLayoutConstraint</returns>
        /// <param name="view">View.</param>
        /// <param name="targetView">Target view.</param>
        /// <param name="spacing">Spacing.</param>
        public static NSLayoutConstraint PinVerticalSpacing(this UIView view, UIView targetView, float spacing = 0)
        {
            var constraint = spacing >= 0 ? NSLayoutConstraint.Create(view, NSLayoutAttribute.Top, NSLayoutRelation.Equal, targetView, NSLayoutAttribute.Bottom, 1, spacing) : 
                NSLayoutConstraint.Create(view, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, targetView, NSLayoutAttribute.Top, 1, spacing);

            constraint.Active = true;

            return constraint;
        }

        public static NSLayoutConstraint[] PinSpacing(this UIView view, UIView targetView, UIOffset offset = default(UIOffset))
        {
            return new [] { view.PinHorizontalSpacing(targetView, offset.X), view.PinVerticalSpacing(targetView, offset.Y) };
        }
    }
}
