using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Ants.DataStructures.BinarySpacePartitioning;

namespace Ants.Testing
{
    class GraphHelper
    {
        public static XDocument GenerateGraph<T>(KdTree<T> tree) where T : Location
        {
            XNamespace schema = "http://schemas.microsoft.com/vs/2009/dgml";
            XDocument document = new XDocument();
            //document.Add(new XAttribute("xmlns", "http://schemas.microsoft.com/vs/2009/dgml"));

            XElement root = new XElement(XName.Get("DirectedGraph", "http://schemas.microsoft.com/vs/2009/dgml"));
            //root.Add(new XAttribute("xmlns", "http://schemas.microsoft.com/vs/2009/dgml"));
            document.Add(root);

            XElement linksElement = new XElement("Links");
            root.Add(linksElement);
            XElement nodesElement = new XElement("Nodes");
            foreach (var node in tree.GetAllNodes())
            {
                linksElement.Add(GenerateLink(node));
                nodesElement.Add(GenerateNode(node));
            }
            root.Add(nodesElement);





            //foreach (var l in automaton.Transitions)
            //{

            //    foreach (XElement element in GenerateLinks(new List<State>() { l.To }, l.From, l.Symbol ?? l.Regex.ToString()))
            //    {
            //        linksElement.Add(element);
            //    }
            //}

            //foreach (XElement element in GenerateNodes(automaton.States, (state) => automaton.StartState == state, (state) => automaton.FinalStates.Contains(state)))
            //{
            //    nodesElement.Add(element);
            //}


            //return document;
            string newDoc = document.ToString();

            newDoc = newDoc.Replace("xmlns=\"\"", string.Empty);

            return XDocument.Parse(newDoc);
        }

        private static XElement GenerateNode<T>(KdNode<T> node) where T : Location
        {
            XElement element = new XElement("Node");
            element.Add(new XAttribute("Id", node.Value.ToString()));

            return element;
        }

        private static List<XElement> GenerateLink<T>(KdNode<T> node) where T : Location
        {
            List<XElement> linkElements = new List<XElement>();

            if (node.Left != null)
            {
                XElement element = new XElement("Link");
                element.Add(new XAttribute("Source", node.Value.ToString()));
                element.Add(new XAttribute("Target", node.Left.Value.ToString()));

                element.Add(new XAttribute("Label", "Left"));

                linkElements.Add(element);
            }

            if (node.Right != null)
            {
                XElement element = new XElement("Link");
                element.Add(new XAttribute("Source", node.Value.ToString()));
                element.Add(new XAttribute("Target", node.Right.Value.ToString()));

                element.Add(new XAttribute("Label", "Right"));

                linkElements.Add(element);
            }

            return linkElements;
        }
    }
}
