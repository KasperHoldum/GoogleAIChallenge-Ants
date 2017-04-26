using System;
using System.Collections.Generic;

namespace Ants.DataStructures.HPA
{
    public class TransitNode : Location
    {
        public List<TransitNode> Edges { get; set; }
        public Cluster Cluster { get; set; }

        public TransitNode(int col, int row, Cluster cluster)
            : base(col, row)
        {
            Cluster = cluster;
            Edges = new List<TransitNode>();
        }

        public double DistanceTo(TransitNode otherNode)
        {
#if DEBUG
            if (!otherNode.IsConnectedTo(this))
            {
                throw new ArgumentException("The nodes are not connected", "otherNode");
            }
#endif

            return this.Cluster.Id == otherNode.Cluster.Id ? this.Cluster.InternalDistance(this, otherNode) : 1;
        }

        private bool IsConnectedTo(TransitNode transitNode)
        {
            return Edges.Contains(transitNode);
        }

        public void ConnectTo(TransitNode n2)
        {
            if (!this.IsConnectedTo(n2))
                this.Edges.Add(n2);
            if (!n2.IsConnectedTo(this))
                n2.Edges.Add(this);
        }

        public void UnconnectEverything()
        {
            foreach (TransitNode transitNode in Edges)
            {
                transitNode.RemoveEdge(this);
            }
        }

        private void RemoveEdge(TransitNode transitNode)
        {
            this.Edges.Remove(transitNode);
        }
    }
}