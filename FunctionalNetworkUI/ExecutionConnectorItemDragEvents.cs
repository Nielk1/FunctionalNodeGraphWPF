﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace FunctionalNetworkUI
{
    /// <summary>
    /// Arguments for event raised when the user starts to drag a connector out from a node.
    /// </summary>
    internal class ExecutionConnectorItemDragStartedEventArgs : RoutedEventArgs
    {
        internal ExecutionConnectorItemDragStartedEventArgs(RoutedEvent routedEvent, object source) :
            base(routedEvent, source)
        {
        }

        /// <summary>
        /// Cancel dragging out of the connector.
        /// </summary>
        public bool Cancel
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Defines the event handler for ConnectorDragStarted events.
    /// </summary>
    internal delegate void ExecutionConnectorItemDragStartedEventHandler(object sender, ExecutionConnectorItemDragStartedEventArgs e);

    /// <summary>
    /// Arguments for event raised while user is dragging a node in the network.
    /// </summary>
    internal class ExecutionConnectorItemDraggingEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// The amount the connector has been dragged horizontally.
        /// </summary>
        private double horizontalChange = 0;

        /// <summary>
        /// The amount the connector has been dragged vertically.
        /// </summary>
        private double verticalChange = 0;

        public ExecutionConnectorItemDraggingEventArgs(RoutedEvent routedEvent, object source, double horizontalChange, double verticalChange) :
            base(routedEvent, source)
        {
            this.horizontalChange = horizontalChange;
            this.verticalChange = verticalChange;
        }

        /// <summary>
        /// The amount the node has been dragged horizontally.
        /// </summary>
        public double HorizontalChange
        {
            get
            {
                return horizontalChange;
            }
        }

        /// <summary>
        /// The amount the node has been dragged vertically.
        /// </summary>
        public double VerticalChange
        {
            get
            {
                return verticalChange;
            }
        }
    }

    /// <summary>
    /// Defines the event handler for ConnectorDragStarted events.
    /// </summary>
    internal delegate void ExecutionConnectorItemDraggingEventHandler(object sender, ExecutionConnectorItemDraggingEventArgs e);

    /// <summary>
    /// Arguments for event raised when the user has completed dragging a connector.
    /// </summary>
    internal class ExecutionConnectorItemDragCompletedEventArgs : RoutedEventArgs
    {
        public ExecutionConnectorItemDragCompletedEventArgs(RoutedEvent routedEvent, object source) :
            base(routedEvent, source)
        {
        }
    }

    /// <summary>
    /// Defines the event handler for ConnectorDragCompleted events.
    /// </summary>
    internal delegate void ExecutionConnectorItemDragCompletedEventHandler(object sender, ExecutionConnectorItemDragCompletedEventArgs e);

}
