using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace FunctionalNetworkUI
{
    /// <summary>
    /// Base class for connection dragging event args.
    /// </summary>
    public class ExecutionConnectionDragEventArgs : RoutedEventArgs
    {
        #region Private Data Members

        /// <summary>
        /// The NodeItem or it's DataContext (when non-NULL).
        /// </summary>
        private object node = null;

        /// <summary>
        /// The ConnectorItem or it's DataContext (when non-NULL).
        /// </summary>
        private object draggedOutExecutionConnector = null;

        /// <summary>
        /// The connector that will be dragged out.
        /// </summary>
        protected object executionConnection = null;

        #endregion Private Data Members

        /// <summary>
        /// The NodeItem or it's DataContext (when non-NULL).
        /// </summary>
        public object Node
        {
            get
            {
                return node;
            }
        }

        /// <summary>
        /// The ConnectorItem or it's DataContext (when non-NULL).
        /// </summary>
        public object ExecutionConnectorDraggedOut
        {
            get
            {
                return draggedOutExecutionConnector;
            }
        }

        #region Private Methods

        protected ExecutionConnectionDragEventArgs(RoutedEvent routedEvent, object source, object node, object executionConnection, object executionConnector) :
            base(routedEvent, source)
        {
            this.node = node;
            this.draggedOutExecutionConnector = executionConnector;
            this.executionConnection = executionConnection;
        }

        #endregion Private Methods
    }

    /// <summary>
    /// Arguments for event raised when the user starts to drag a connection out from a node.
    /// </summary>
    public class ExecutionConnectionDragStartedEventArgs : ExecutionConnectionDragEventArgs
    {
        /// <summary>
        /// The connection that will be dragged out.
        /// </summary>
        public object ExecutionConnection
        {
            get
            {
                return executionConnection;
            }
            set
            {
                executionConnection = value;
            }
        }

        #region Private Methods

        internal ExecutionConnectionDragStartedEventArgs(RoutedEvent routedEvent, object source, object node, object executionConnector) :
            base(routedEvent, source, node, null, executionConnector)
        {
        }

        #endregion Private Methods
    }

    /// <summary>
    /// Defines the event handler for the ConnectionDragStarted event.
    /// </summary>
    public delegate void ExecutionConnectionDragStartedEventHandler(object sender, ExecutionConnectionDragStartedEventArgs e);

    /// <summary>
    /// Arguments for event raised while user is dragging a node in the network.
    /// </summary>
    public class QueryExecutionConnectionFeedbackEventArgs : ExecutionConnectionDragEventArgs
    {
        #region Private Data Members

        /// <summary>
        /// The ConnectorItem or it's DataContext (when non-NULL).
        /// </summary>
        private object draggedOverExecutionConnector = null;

        /// <summary>
        /// Set to 'true' / 'false' to indicate that the connection from the dragged out connection to the dragged over connector is valid.
        /// </summary>
        private bool executionConnectionOk = true;

        /// <summary>
        /// The indicator to display.
        /// </summary>
        private object feedbackIndicator = null;

        #endregion Private Data Members

        /// <summary>
        /// The ConnectorItem or it's DataContext (when non-NULL).
        /// </summary>
        public object DraggedOverExecutionConnector
        {
            get
            {
                return draggedOverExecutionConnector;
            }
        }

        /// <summary>
        /// The connection that will be dragged out.
        /// </summary>
        public object ExecutionConnection
        {
            get
            {
                return executionConnection;
            }
        }

        /// <summary>
        /// Set to 'true' / 'false' to indicate that the connection from the dragged out connection to the dragged over connector is valid.
        /// </summary>
        public bool ExecutionConnectionOk
        {
            get
            {
                return executionConnectionOk;
            }
            set
            {
                executionConnectionOk = value;
            }
        }

        /// <summary>
        /// The indicator to display.
        /// </summary>
        public object FeedbackIndicator
        {
            get
            {
                return feedbackIndicator;
            }
            set
            {
                feedbackIndicator = value;
            }
        }

        #region Private Methods

        internal QueryExecutionConnectionFeedbackEventArgs(RoutedEvent routedEvent, object source,
            object node, object executionConnection, object executionConnector, object draggedOverExecutionConnector) :
            base(routedEvent, source, node, executionConnection, executionConnector)
        {
            this.draggedOverExecutionConnector = draggedOverExecutionConnector;
        }

        #endregion Private Methods
    }

    /// <summary>
    /// Defines the event handler for the QueryConnectionFeedback event.
    /// </summary>
    public delegate void QueryExecutionConnectionFeedbackEventHandler(object sender, QueryExecutionConnectionFeedbackEventArgs e);

    /// <summary>
    /// Arguments for event raised while user is dragging a node in the network.
    /// </summary>
    public class ExecutionConnectionDraggingEventArgs : ExecutionConnectionDragEventArgs
    {
        /// <summary>
        /// The connection being dragged out.
        /// </summary>
        public object ExecutionConnection
        {
            get
            {
                return executionConnection;
            }
        }

        #region Private Methods

        internal ExecutionConnectionDraggingEventArgs(RoutedEvent routedEvent, object source,
                object node, object executionConnection, object executionConnector) :
            base(routedEvent, source, node, executionConnection, executionConnector)
        {
        }

        #endregion Private Methods
    }

    /// <summary>
    /// Defines the event handler for the ConnectionDragging event.
    /// </summary>
    public delegate void ExecutionConnectionDraggingEventHandler(object sender, ExecutionConnectionDraggingEventArgs e);

    /// <summary>
    /// Arguments for event raised when the user has completed dragging a connector.
    /// </summary>
    public class ExecutionConnectionDragCompletedEventArgs : ExecutionConnectionDragEventArgs
    {
        #region Private Data Members

        /// <summary>
        /// The ConnectorItem or it's DataContext (when non-NULL).
        /// </summary>
        private object executionConnectorDraggedOver = null;

        #endregion Private Data Members

        /// <summary>
        /// The ConnectorItem or it's DataContext (when non-NULL).
        /// </summary>
        public object ExecutionConnectorDraggedOver
        {
            get
            {
                return executionConnectorDraggedOver;
            }
        }

        /// <summary>
        /// The connection that will be dragged out.
        /// </summary>
        public object ExecutionConnection
        {
            get
            {
                return executionConnection;
            }
        }

        #region Private Methods

        internal ExecutionConnectionDragCompletedEventArgs(RoutedEvent routedEvent, object source, object node, object executionConnection, object executionConnector, object executionConnectorDraggedOver) :
            base(routedEvent, source, node, executionConnection, executionConnector)
        {
            this.executionConnectorDraggedOver = executionConnectorDraggedOver;
        }

        #endregion Private Methods
    }

    /// <summary>
    /// Defines the event handler for the ConnectionDragCompleted event.
    /// </summary>
    public delegate void ExecutionConnectionDragCompletedEventHandler(object sender, ExecutionConnectionDragCompletedEventArgs e);
}
