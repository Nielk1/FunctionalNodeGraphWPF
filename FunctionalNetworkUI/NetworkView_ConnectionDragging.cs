using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Specialized;
using Utils;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Input;

namespace FunctionalNetworkUI
{
    /// <summary>
    /// Partial definition of the NetworkView class.
    /// This file only contains private members related to dragging connections.
    /// </summary>
    public partial class NetworkView
    {
        #region Private Data Members

        /// <summary>
        /// When dragging a connection, this is set to the ConnectorItem that was initially dragged out.
        /// </summary>
        private ExecutionConnectorItem draggedOutExecutionConnectorItem = null;

        /// <summary>
        /// When dragging a connection, this is set to the ConnectorItem that was initially dragged out.
        /// </summary>
        private ConnectorItem draggedOutConnectorItem = null;

        /// <summary>
        /// The view-model object for the connector that has been dragged out.
        /// </summary>
        private object draggedOutConnectorDataContext = null;

        /// <summary>
        /// The view-model object for the connector that has been dragged out.
        /// </summary>
        private object draggedOutExecutionConnectorDataContext = null;

        /// <summary>
        /// The view-model object for the node whose connector was dragged out.
        /// </summary>
        private object draggedOutNodeDataContext = null;

        /// <summary>
        /// The view-model object for the connection that is currently being dragged, or null if none being dragged.
        /// </summary>
        private object draggingExecutionConnectionDataContext = null;

        /// <summary>
        /// The view-model object for the connection that is currently being dragged, or null if none being dragged.
        /// </summary>
        private object draggingConnectionDataContext = null;

        /// <summary>
        /// A reference to the feedback adorner that is currently in the adorner layer, or null otherwise.
        /// It is used for feedback when dragging a connection over a prospective connector.
        /// </summary>
        private FrameworkElementAdorner feedbackAdorner = null;

        #endregion Private Data Members

        #region Private Methods

        /// <summary>
        /// Event raised when the user starts to drag a connector.
        /// </summary>
        private void ExecutionConnectorItem_DragStarted(object source, ExecutionConnectorItemDragStartedEventArgs e)
        {
            this.Focus();

            e.Handled = true;

            this.IsDragging = true;
            this.IsNotDragging = false;
            this.IsDraggingExecutionConnection = true;
            this.IsNotDraggingExecutionConnection = false;

            this.draggedOutExecutionConnectorItem = (ExecutionConnectorItem)e.OriginalSource;
            var nodeItem = this.draggedOutExecutionConnectorItem.ParentNodeItem;
            this.draggedOutNodeDataContext = nodeItem.DataContext != null ? nodeItem.DataContext : nodeItem;
            this.draggedOutExecutionConnectorDataContext = this.draggedOutExecutionConnectorItem.DataContext != null ? this.draggedOutExecutionConnectorItem.DataContext : this.draggedOutExecutionConnectorItem;

            //
            // Raise an event so that application code can create a connection and
            // add it to the view-model.
            //
            ExecutionConnectionDragStartedEventArgs eventArgs = new ExecutionConnectionDragStartedEventArgs(ExecutionConnectionDragStartedEvent, this, this.draggedOutNodeDataContext, this.draggedOutExecutionConnectorDataContext);
            RaiseEvent(eventArgs);

            //
            // Retrieve the the view-model object for the connection was created by application code.
            //
            this.draggingExecutionConnectionDataContext = eventArgs.ExecutionConnection;

            if (draggingExecutionConnectionDataContext == null)
            {
                //
                // Application code didn't create any connection.
                //
                e.Cancel = true;
                return;
            }
        }

        /// <summary>
        /// Event raised while the user is dragging a connector.
        /// </summary>
        private void ExecutionConnectorItem_Dragging(object source, ExecutionConnectorItemDraggingEventArgs e)
        {
            e.Handled = true;

            Trace.Assert((ExecutionConnectorItem)e.OriginalSource == this.draggedOutExecutionConnectorItem);

            Point mousePoint = Mouse.GetPosition(this);
            //
            // Raise an event so that application code can compute intermediate connection points.
            //
            var executionConnectionDraggingEventArgs =
                new ExecutionConnectionDraggingEventArgs(ExecutionConnectionDraggingEvent, this,
                        this.draggedOutNodeDataContext, this.draggingExecutionConnectionDataContext,
                        this.draggedOutExecutionConnectorDataContext);

            RaiseEvent(executionConnectionDraggingEventArgs);

            //
            // Figure out if the connection has been dragged over a connector.
            //

            ExecutionConnectorItem executionConnectorDraggedOver = null;
            object executionConnectorDataContextDraggedOver = null;
            bool dragOverSuccess = DetermineExecutionConnectorItemDraggedOver(mousePoint, out executionConnectorDraggedOver, out executionConnectorDataContextDraggedOver);
            if (executionConnectorDraggedOver != null)
            {
                //
                // Raise an event so that application code can specify if the connector
                // that was dragged over is valid or not.
                //
                var queryFeedbackEventArgs =
                    new QueryExecutionConnectionFeedbackEventArgs(QueryExecutionConnectionFeedbackEvent, this, this.draggedOutNodeDataContext, this.draggingExecutionConnectionDataContext,
                            this.draggedOutExecutionConnectorDataContext, executionConnectorDataContextDraggedOver);

                RaiseEvent(queryFeedbackEventArgs);

                if (queryFeedbackEventArgs.FeedbackIndicator != null)
                {
                    //
                    // A feedback indicator was specified by the event handler.
                    // This is used to indicate whether the connection is good or bad!
                    //
                    AddFeedbackAdorner(executionConnectorDraggedOver, queryFeedbackEventArgs.FeedbackIndicator);
                }
                else
                {
                    //
                    // No feedback indicator specified by the event handler.
                    // Clear any existing feedback indicator.
                    //
                    ClearFeedbackAdorner();
                }
            }
            else
            {
                //
                // Didn't drag over any valid connector.
                // Clear any existing feedback indicator.
                //
                ClearFeedbackAdorner();
            }
        }

        /// <summary>
        /// Event raised when the user has finished dragging a connector.
        /// </summary>
        private void ExecutionConnectorItem_DragCompleted(object source, ExecutionConnectorItemDragCompletedEventArgs e)
        {
            e.Handled = true;

            Trace.Assert((ExecutionConnectorItem)e.OriginalSource == this.draggedOutExecutionConnectorItem);

            Point mousePoint = Mouse.GetPosition(this);

            //
            // Figure out if the end of the connection was dropped on a connector.
            //
            ExecutionConnectorItem connectorDraggedOver = null;
            object connectorDataContextDraggedOver = null;
            DetermineExecutionConnectorItemDraggedOver(mousePoint, out connectorDraggedOver, out connectorDataContextDraggedOver);

            //
            // Now that connection dragging has completed, don't any feedback adorner.
            //
            ClearFeedbackAdorner();

            //
            // Raise an event to inform application code that connection dragging is complete.
            // The application code can determine if the connection between the two connectors
            // is valid and if so it is free to make the appropriate connection in the view-model.
            //
            RaiseEvent(new ExecutionConnectionDragCompletedEventArgs(ExecutionConnectionDragCompletedEvent, this, this.draggedOutNodeDataContext, this.draggingExecutionConnectionDataContext, this.draggedOutExecutionConnectorDataContext, connectorDataContextDraggedOver));

            this.IsDragging = false;
            this.IsNotDragging = true;
            this.IsDraggingExecutionConnection = false;
            this.IsDraggingConnection = false;
            this.IsNotDraggingExecutionConnection = true;
            this.IsNotDraggingConnection = true;
            this.draggedOutExecutionConnectorDataContext = null;
            this.draggedOutConnectorDataContext = null;
            this.draggedOutNodeDataContext = null;
            this.draggedOutExecutionConnectorItem = null;
            this.draggedOutConnectorItem = null;
            this.draggingExecutionConnectionDataContext = null;
            this.draggingConnectionDataContext = null;
        }

        /// <summary>
        /// This function does a hit test to determine which connector, if any, is under 'hitPoint'.
        /// </summary>
        private bool DetermineExecutionConnectorItemDraggedOver(Point hitPoint, out ExecutionConnectorItem executionConnectorItemDraggedOver, out object executionConnectorDataContextDraggedOver)
        {
            executionConnectorItemDraggedOver = null;
            executionConnectorDataContextDraggedOver = null;

            //
            // Run a hit test 
            //
            HitTestResult result = null;
            VisualTreeHelper.HitTest(nodeItemsControl, null,
                //
                // Result callback delegate.
                // This method is called when we have a result.
                //
                delegate (HitTestResult hitTestResult)
                {
                    result = hitTestResult;

                    return HitTestResultBehavior.Stop;
                },
                new PointHitTestParameters(hitPoint));

            if (result == null || result.VisualHit == null)
            {
                // Hit test failed.
                return false;
            }

            //
            // Actually want a reference to a 'ConnectorItem'.  
            // The hit test may have hit a UI element that is below 'ConnectorItem' so
            // search up the tree.
            //
            var hitItem = result.VisualHit as FrameworkElement;
            if (hitItem == null)
            {
                return false;
            }
            var executionConnectorItem = WpfUtils.FindVisualParentWithType<ExecutionConnectorItem>(hitItem);
            if (executionConnectorItem == null)
            {
                return false;
            }

            var networkView = executionConnectorItem.ParentNetworkView;
            if (networkView != this)
            {
                //
                // Ensure that dragging over a connector in another NetworkView doesn't
                // return a positive result.
                //
                return false;
            }

            object executionConnectorDataContext = executionConnectorItem;
            if (executionConnectorItem.DataContext != null)
            {
                //
                // If there is a data-context then grab it.
                // When we are using a view-model then it is the view-model
                // object we are interested in.
                //
                executionConnectorDataContext = executionConnectorItem.DataContext;
            }

            executionConnectorItemDraggedOver = executionConnectorItem;
            executionConnectorDataContextDraggedOver = executionConnectorDataContext;

            return true;
        }

























        /// <summary>
        /// Event raised when the user starts to drag a connector.
        /// </summary>
        private void ConnectorItem_DragStarted(object source, ConnectorItemDragStartedEventArgs e)
        {
            this.Focus();

            e.Handled = true;

            this.IsDragging = true;
            this.IsNotDragging = false;
            this.IsDraggingConnection = true;
            this.IsNotDraggingConnection = false;

            this.draggedOutConnectorItem = (ConnectorItem)e.OriginalSource;
            var nodeItem = this.draggedOutConnectorItem.ParentNodeItem;
            this.draggedOutNodeDataContext = nodeItem.DataContext != null ? nodeItem.DataContext : nodeItem;
            this.draggedOutConnectorDataContext = this.draggedOutConnectorItem.DataContext != null ? this.draggedOutConnectorItem.DataContext : this.draggedOutConnectorItem;

            //
            // Raise an event so that application code can create a connection and
            // add it to the view-model.
            //
            ConnectionDragStartedEventArgs eventArgs = new ConnectionDragStartedEventArgs(ConnectionDragStartedEvent, this, this.draggedOutNodeDataContext, this.draggedOutConnectorDataContext);
            RaiseEvent(eventArgs);

            //
            // Retrieve the the view-model object for the connection was created by application code.
            //
            this.draggingConnectionDataContext = eventArgs.Connection;

            if (draggingConnectionDataContext == null)
            {
                //
                // Application code didn't create any connection.
                //
                e.Cancel = true;
                return;
            }
        }

        /// <summary>
        /// Event raised while the user is dragging a connector.
        /// </summary>
        private void ConnectorItem_Dragging(object source, ConnectorItemDraggingEventArgs e)
        {
            e.Handled = true;

            Trace.Assert((ConnectorItem)e.OriginalSource == this.draggedOutConnectorItem);

            Point mousePoint = Mouse.GetPosition(this);
            //
            // Raise an event so that application code can compute intermediate connection points.
            //
            var connectionDraggingEventArgs =
                new ConnectionDraggingEventArgs(ConnectionDraggingEvent, this, 
                        this.draggedOutNodeDataContext, this.draggingConnectionDataContext, 
                        this.draggedOutConnectorDataContext);

            RaiseEvent(connectionDraggingEventArgs);

            //
            // Figure out if the connection has been dragged over a connector.
            //

            ConnectorItem connectorDraggedOver = null;
            object connectorDataContextDraggedOver = null;
            bool dragOverSuccess = DetermineConnectorItemDraggedOver(mousePoint, out connectorDraggedOver, out connectorDataContextDraggedOver);
            if (connectorDraggedOver != null)
            {
                //
                // Raise an event so that application code can specify if the connector
                // that was dragged over is valid or not.
                //
                var queryFeedbackEventArgs = 
                    new QueryConnectionFeedbackEventArgs(QueryConnectionFeedbackEvent, this, this.draggedOutNodeDataContext, this.draggingConnectionDataContext, 
                            this.draggedOutConnectorDataContext, connectorDataContextDraggedOver);

                RaiseEvent(queryFeedbackEventArgs);

                if (queryFeedbackEventArgs.FeedbackIndicator != null)
                {
                    //
                    // A feedback indicator was specified by the event handler.
                    // This is used to indicate whether the connection is good or bad!
                    //
                    AddFeedbackAdorner(connectorDraggedOver, queryFeedbackEventArgs.FeedbackIndicator);
                }
                else
                {
                    //
                    // No feedback indicator specified by the event handler.
                    // Clear any existing feedback indicator.
                    //
                    ClearFeedbackAdorner();
                }
            }
            else
            {
                //
                // Didn't drag over any valid connector.
                // Clear any existing feedback indicator.
                //
                ClearFeedbackAdorner();
            }
        }

        /// <summary>
        /// Event raised when the user has finished dragging a connector.
        /// </summary>
        private void ConnectorItem_DragCompleted(object source, ConnectorItemDragCompletedEventArgs e)
        {
            e.Handled = true;

            Trace.Assert((ConnectorItem)e.OriginalSource == this.draggedOutConnectorItem);

            Point mousePoint = Mouse.GetPosition(this);

            //
            // Figure out if the end of the connection was dropped on a connector.
            //
            ConnectorItem connectorDraggedOver = null;
            object connectorDataContextDraggedOver = null;
            DetermineConnectorItemDraggedOver(mousePoint, out connectorDraggedOver, out connectorDataContextDraggedOver);

            //
            // Now that connection dragging has completed, don't any feedback adorner.
            //
            ClearFeedbackAdorner();

            //
            // Raise an event to inform application code that connection dragging is complete.
            // The application code can determine if the connection between the two connectors
            // is valid and if so it is free to make the appropriate connection in the view-model.
            //
            RaiseEvent(new ConnectionDragCompletedEventArgs(ConnectionDragCompletedEvent, this, this.draggedOutNodeDataContext, this.draggingConnectionDataContext, this.draggedOutConnectorDataContext, connectorDataContextDraggedOver));

            this.IsDragging = false;
            this.IsNotDragging = true;
            this.IsDraggingExecutionConnection = false;
            this.IsDraggingConnection = false;
            this.IsNotDraggingExecutionConnection = true;
            this.IsNotDraggingConnection = true;
            this.draggedOutExecutionConnectorDataContext = null;
            this.draggedOutConnectorDataContext = null;
            this.draggedOutNodeDataContext = null;
            this.draggedOutExecutionConnectorItem = null;
            this.draggedOutConnectorItem = null;
            this.draggingExecutionConnectionDataContext = null;
            this.draggingConnectionDataContext = null;
        }

        /// <summary>
        /// This function does a hit test to determine which connector, if any, is under 'hitPoint'.
        /// </summary>
        private bool DetermineConnectorItemDraggedOver(Point hitPoint, out ConnectorItem connectorItemDraggedOver, out object connectorDataContextDraggedOver)
        {
            connectorItemDraggedOver = null;
            connectorDataContextDraggedOver = null;

            //
            // Run a hit test 
            //
            HitTestResult result = null;
            VisualTreeHelper.HitTest(nodeItemsControl, null, 
                //
                // Result callback delegate.
                // This method is called when we have a result.
                //
                delegate(HitTestResult hitTestResult)
                {
                    result = hitTestResult;

                    return HitTestResultBehavior.Stop;
                },
                new PointHitTestParameters(hitPoint));

            if (result == null || result.VisualHit == null)
            {
                // Hit test failed.
                return false;
            }

            //
            // Actually want a reference to a 'ConnectorItem'.  
            // The hit test may have hit a UI element that is below 'ConnectorItem' so
            // search up the tree.
            //
            var hitItem = result.VisualHit as FrameworkElement;
            if (hitItem == null)
            {
                return false;
            }
            var connectorItem = WpfUtils.FindVisualParentWithType<ConnectorItem>(hitItem);
			if (connectorItem == null)
            {
                return false;
            }

            var networkView = connectorItem.ParentNetworkView;
            if (networkView != this)
            {
                //
                // Ensure that dragging over a connector in another NetworkView doesn't
                // return a positive result.
                //
                return false;
            }

            object connectorDataContext = connectorItem;
            if (connectorItem.DataContext != null)
            {
                //
                // If there is a data-context then grab it.
                // When we are using a view-model then it is the view-model
                // object we are interested in.
                //
                connectorDataContext = connectorItem.DataContext;
            }

            connectorItemDraggedOver = connectorItem;
            connectorDataContextDraggedOver = connectorDataContext;

            return true;
        }
         
        /// <summary>
        /// Add a feedback adorner to a UI element.
        /// This is used to show when a connection can or can't be attached to a particular connector.
        /// 'indicator' will be a view-model object that is transformed into a UI element using a data-template.
        /// </summary>
        private void AddFeedbackAdorner(FrameworkElement adornedElement, object indicator)
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);

            if (feedbackAdorner != null)
            {
                if (feedbackAdorner.AdornedElement == adornedElement)
                {
                    // No change.
                    return;
                }

                adornerLayer.Remove(feedbackAdorner);
                feedbackAdorner = null;
            }

            //
            // Create a content control to contain 'indicator'.
            // The view-model object 'indicator' is transformed into a UI element using
            // normal WPF data-template rules.
            //
            ContentControl adornerElement = new ContentControl();
            adornerElement.HorizontalAlignment = HorizontalAlignment.Center;
            adornerElement.VerticalAlignment = VerticalAlignment.Center;
            adornerElement.Content = indicator;

            //
            // Create the adorner and add it to the adorner layer.
            //
            feedbackAdorner = new FrameworkElementAdorner(adornerElement, adornedElement);
            adornerLayer.Add(feedbackAdorner);
        }

        /// <summary>
        /// If there is an existing feedback adorner, remove it.
        /// </summary>
        private void ClearFeedbackAdorner()
        {
            if (feedbackAdorner == null)
            {
                return;
            }

            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
            adornerLayer.Remove(feedbackAdorner);
            feedbackAdorner = null;
        }

        #endregion Private Methods
    }
}
