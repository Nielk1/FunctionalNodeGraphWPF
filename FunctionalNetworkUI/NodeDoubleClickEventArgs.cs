using System.Collections;
using System.Windows;

namespace FunctionalNetworkUI
{
    /// <summary>
    /// Defines the event handler for NodeDragStarted events.
    /// </summary>
    public delegate void NodeDoubleClickEventHandler(object sender, NodeDoubleClickEventArgs e);


    public class NodeDoubleClickEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// The NodeItem's or their DataContext (when non-NULL).
        /// </summary>
        public ICollection nodes = null;

        /// <summary>
        /// Set to 'false' to disallow dragging.
        /// </summary>
        //private bool cancel = false;

        internal NodeDoubleClickEventArgs(RoutedEvent routedEvent, object source, ICollection nodes) :
            base(routedEvent, source)
        {
            this.nodes = nodes;
        }

        /// <summary>
        /// Set to 'false' to disallow dragging.
        /// </summary>
        //public bool Cancel
        //{
        //    get
        //    {
        //        return cancel;
        //    }
        //    set
        //    {
        //        cancel = value;
        //    }
        //}
    }
}