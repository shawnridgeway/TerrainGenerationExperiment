using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Builder {
    public class Graph {
        private Node root;

        public Graph(Node root) {
            this.root = root;
        }

        public Node GetRoot() {
            return root;
        }
    }

    public class Node {
        private TerrainTransform transform;
        private Node[] parents;

        public Node(TerrainTransform transform) {
            this.transform = transform;
        }
    }
}