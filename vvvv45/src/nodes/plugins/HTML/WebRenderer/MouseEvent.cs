﻿/*
 * Created by SharpDevelop.
 * User: elias
 * Date: 26.02.2012
 * Time: 03:13
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using VVVV.Core;

namespace VVVV.Nodes.HTML
{
    public enum MouseButton
    {
        None,
        Left,
        Middle,
        Right
    }
    /// <summary>
    /// Description of MouseEvent.
    /// </summary>
    public struct MouseEvent : IEquatable<MouseEvent>
    {
        public double X;
        public double Y;
        public MouseButton Button;
        public bool MouseUp;
        public int ClickCount;
        
        public MouseEvent(double x, double y, MouseButton button, bool mouseUp, int clickCount)
        {
            X = x;
            Y = y;
            Button = button;
            MouseUp = mouseUp;
            ClickCount = clickCount;
        }
        
        [Node(Name = "MouseEvent", Category = "Join")]
        public static MouseEvent Join(
            double x,
            double y,
            bool leftButton,
            bool middleButton,
            bool rightButton,
            bool mouseUp,
            int clickCount)
        {
            var button = MouseButton.None;
            if (leftButton)
                button = MouseButton.Left;
            else if (middleButton)
                button = MouseButton.Middle;
            else if (rightButton)
                button = MouseButton.Right;
            return new MouseEvent(x, y, button, mouseUp, clickCount);
        }
        
        [Node(Name = "MouseEvent", Category = "Split")]
        public static void Split(
            MouseEvent mouseEvent,
            out double x,
            out double y,
            out bool leftButton,
            out bool middleButton,
            out bool rightButton,
            out bool mouseUp,
            out int clickCount
           )
        {
            x = mouseEvent.X;
            y = mouseEvent.Y;
            leftButton = mouseEvent.Button == MouseButton.Left;
            middleButton = mouseEvent.Button == MouseButton.Middle;
            rightButton = mouseEvent.Button == MouseButton.Right;
            mouseUp = mouseEvent.MouseUp;
            clickCount = mouseEvent.ClickCount;
        }
        
        #region Equals and GetHashCode implementation
        // The code in this region is useful if you want to use this structure in collections.
        // If you don't need it, you can just remove the region and the ": IEquatable<MouseEvent>" declaration.
        
        public override bool Equals(object obj)
        {
            if (obj is MouseEvent)
                return Equals((MouseEvent)obj); // use Equals method below
            else
                return false;
        }
        
        public bool Equals(MouseEvent other)
        {
            // add comparisions for all members here
            return this.X == other.X && this.Y == other.Y && this.Button == other.Button && this.ClickCount == other.ClickCount && this.MouseUp == other.MouseUp;
        }
        
        public override int GetHashCode()
        {
            // combine the hash codes of all members here (e.g. with XOR operator ^)
            return X.GetHashCode() ^ Y.GetHashCode() ^ Button.GetHashCode() ^ ClickCount.GetHashCode() ^ MouseUp.GetHashCode();
        }
        
        public static bool operator ==(MouseEvent left, MouseEvent right)
        {
            return left.Equals(right);
        }
        
        public static bool operator !=(MouseEvent left, MouseEvent right)
        {
            return !left.Equals(right);
        }
        #endregion
    }
}
