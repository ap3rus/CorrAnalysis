using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Ap.CorrAnalysis.Framework
{
    public class PartialSortingHelper<TKey, TValue> where TKey : IComparable
    {
        public static Node<TKey, TValue> GetKthMaximum(
            List<Node<TKey, TValue>> inputList, int k)
        {
            return FindKthMaximum(inputList, k).LastOrDefault();
        }

        public static IEnumerable<Node<TKey, TValue>> GetMaximumKSortedElements(List<Node<TKey, TValue>> inputList, int k)
        {
            return FindKthMaximum(inputList, k);
        }

        private static IEnumerable<Node<TKey, TValue>> FindKthMaximum(List<Node<TKey, TValue>> inputList, int k)
        {
            List<List<Node<TKey, TValue>>> outputTree = GetOutputTree(inputList);
            return GetKthMax(outputTree, k);
        }

        private static IEnumerable<Node<TKey, TValue>> GetKthMax(IReadOnlyList<List<Node<TKey, TValue>>> outputTree, int k)
        {
            int size = outputTree.Count;
            if (k < 0)
            {
                throw new ArgumentOutOfRangeException("k", k, "K should not be less than zero.");
            }

            if (k == 0)
            {
                yield break;
            }

            Node<TKey, TValue> rootNode = outputTree[size - 1][0];
            yield return rootNode;
            if (k == 1)
            {
                yield break;
            }

            var defeatedNodes = new List<Node<TKey, TValue>>();
            defeatedNodes.AddRange(rootNode.DefeatedNodesList);
            Node<TKey, TValue> ithMaxNode = rootNode;

            for (int i = 1; i < k; i++)
            {
                ithMaxNode = GetIthMaxNode(defeatedNodes, ithMaxNode);
                defeatedNodes.AddRange(ithMaxNode.DefeatedNodesList);
            }

            Node<TKey, TValue> kthMaxNode = rootNode;
            for (int i = 1; i < k; i++)
            {
                kthMaxNode = GetIthMaxNode(defeatedNodes, kthMaxNode);
                yield return kthMaxNode;
            }
        }

        private static Node<TKey, TValue> GetIthMaxNode(IEnumerable<Node<TKey, TValue>> defeatedNodes,
            Node<TKey, TValue> iMaxusOneTh)
        {
            Node<TKey, TValue> ithMax = Node<TKey, TValue>.Default;
            foreach (var n in defeatedNodes)
            {
                if (n.CompareTo(iMaxusOneTh) > 0)
                {
                    continue;
                }
                if ((n.CompareTo(iMaxusOneTh) < 0) && (n.CompareTo(ithMax) > 0))
                {
                    ithMax = n;
                }
            }
            return ithMax;
        }

        public static List<List<Node<TKey, TValue>>> GetOutputTree(
            List<Node<TKey, TValue>> inputList)
        {
            int size = inputList.Count;
            double treeDepth = Math.Log(size)/Math.Log(2);
            int intTreeDepth = (int) (Math.Ceiling(treeDepth)) + 1;
            var outputTreeList = new List<List<Node<TKey, TValue>>> { inputList };

            List<Node<TKey, TValue>> currentRow = inputList;
            for (int i = 1; i < intTreeDepth; i++)
            {
                List<Node<TKey, TValue>> nextRow = GetNextRow(currentRow);
                outputTreeList.Add(nextRow);
                currentRow = nextRow;
            }
            return outputTreeList;
        }

        private static List<Node<TKey, TValue>> GetNextRow(
            List<Node<TKey, TValue>> values)
        {
            int rowSize = GetNextRowSize(values);
            var row = new List<Node<TKey, TValue>>(
                rowSize);
            for (int j = 0; j < values.Count; j++)
            {
                if (j == (values.Count - 1))
                {
                    row.Add(values[j]);
                }
                else
                {
                    row.Add(GetMax(values[j], values[++j]));
                }
            }
            return row;
        }

        private static Node<TKey, TValue> GetMax(Node<TKey, TValue> node1,
                                              Node<TKey, TValue> node2)
        {
            if (node1.CompareTo(node2) > 0)
            {
                node1.AddDefeatedNode(node2);
                return node1;
            }
            else
            {
                node2.AddDefeatedNode(node1);
                return node2;
            }
        }

        private static int GetNextRowSize(List<Node<TKey, TValue>> values)
        {
            return (int) Math.Ceiling(values.Count/2.0);
        }
    }

    public class Node<TKey, TValue> : IComparable where TKey : IComparable
    {
        public static readonly Node<TKey, TValue> Default = new Node<TKey, TValue>(default(TKey), default(TValue), true);

        private readonly bool _readOnly;
        private readonly IList<Node<TKey, TValue>> _defeatedNodesList;
        private TValue _value;

        public static implicit operator TKey(Node<TKey, TValue> d)
        {
            return d.Key;
        }

        public static implicit operator Node<TKey, TValue>(TKey d)
        {
            return new Node<TKey, TValue>(d, default(TValue));
        }

        public Node(TKey key, TValue value)
            :this(key,value,false)
        {
        }

        private Node(TKey key, TValue value, bool readOnly)
        {
            Key = key;
            _value = value;
            if (readOnly)
            {
                _defeatedNodesList = new ReadOnlyCollection<Node<TKey, TValue>>(new List<Node<TKey, TValue>>());
            }
            else
            {
                _defeatedNodesList = new List<Node<TKey, TValue>>();
            }

            _readOnly = readOnly;
        }

        public TKey Key { get; private set; }

        public TValue Value
        {
            get { return _value; }
            set
            {
                if (_readOnly){throw new NotSupportedException("Instance is read-only.");}
                _value = value;
            }
        }

        public IList<Node<TKey, TValue>> DefeatedNodesList
        {
            get { return _defeatedNodesList; }
        }

        public int CompareTo(object o)
        {
            var other = (Node<TKey, TValue>)o;
            if (ReferenceEquals(other, Default)) return 1;
            return Key.CompareTo(other.Key);
        }

        public void AddDefeatedNode(Node<TKey, TValue> node)
        {
            if (_readOnly) { throw new NotSupportedException("Instance is read-only."); }

            _defeatedNodesList.Add(node);
        }


        public override string ToString()
        {
            return new StringBuilder().Append(Key).ToString();
        }
    }
}