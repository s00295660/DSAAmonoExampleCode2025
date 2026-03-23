using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DSATrees
{
    public class GenericTree<T>  where T : class,IComparable
    {
        private TNode<T> root;
        public TNode<T> Root { get => root; set => root = value; }
        public bool GoalFound = false;
        public GenericTree()
        {
            
        }

        public bool simplex_dfs(int Level, TNode<T> node,
                                    TNode<T> Goal, List<TNode<T>> visited,
                                    List<TNode<T>> solution)
        // This method uses the program stack to backtrack and the visited list to disallow cycles
        // it is dependent on the IComparable implementation of TNode<T> to compare nodes and determine if the goal has been found
        // Level is used for debugging purposes to show the depth of the search
        // Solution is used to store the path to the goal node if found
        // The method returns true if the goal is found and false otherwise.
        // This is used to backtrack and build the solution path
        // It does not use a frontier stack 
        {
            // Use LINQ to check if the node has been visited before.
            // This is necessary to disallow cycles in the search
            if (visited.Where(n => n.CompareTo(node) == 0).ToList().Count > 0)             {
                Console.WriteLine($"Already visited {node.Value.ToString()} at level {Level}");
                return false;
            }
            Console.WriteLine($"Visiting {node.Value.ToString()} at level {Level}");
            visited.Add(node);
            if (Goal.CompareTo(node) == 0)
                return true;
            else if (node.children.Count > 0)
                foreach (var child in node.children)
                    if (simplex_dfs(Level + 1, child, Goal, visited, solution))
                    {
                        solution.Add(child);
                        Console.WriteLine($"Found Goal Returning from {Level.ToString()} adding child {child.Value.ToString()}");
                        return true;
                    }
                    else Console.WriteLine($"Backtracking from {child.Value.ToString()} to {node.Value.ToString()} at level {Level}");
            return false;
        }

        /* Current is the current node we are visiting
         *  Path is the current Path to that node
         * frontier contains the candidates to be considered
         * visited disallows cycles
         * Goal is the Node we are looking for
         */
        public Stack<TNode<T>>? dfs(TNode<T> Current, Stack<TNode<T>> Path, 
                            Stack<TNode<T>> frontier,
                                List<TNode<T>> visited, TNode<T> Goal)
        
        {
            // Check if current is goal. Stopping condition for recursion.
            if (Current.CompareTo(Goal) == 0) { Path.Push(Current); return Path; }
            else  // process frontier
            {
                // if the current has not been visited or on back tracking some of the children have not been visited
                if (!visited.Contains(Current) || Current.children.Where(n => !visited.Contains(n)).Count() > 0) // allow for Back tracking
                {
                    if(!frontier.Contains(Current)) 
                        frontier.Push(Current);
                    Console.WriteLine($" Visiting {Current.Value.ToString()}");
                    if(!visited.Contains(Current))     
                        visited.Add(Current);
                    if(!Path.Contains(Current))
                        Path.Push(Current);
                    // if current has children or all paths have not been explored for current
                    if (Current.children.Count > 0 || Current.children.Where(n => !visited.Contains(n)).Count() > 0)
                    {
                        foreach (var child in Current.children)
                            if (!visited.Contains(child))
                            {
                                frontier.Push(child);
                                return dfs(child, Path, frontier, visited, Goal);
                            }
                    }
                    // if no children or all current children have been visited
                    else // it's a leaf node 
                    {    
                        if (frontier.Count > 0) // Path and frontier are the same here, popping the leaf node from both
                        {
                            Path.Pop();
                            frontier.Pop();
                        }
                        if(frontier.Count > 0)
                            if(Path.Contains(frontier.Peek())) Path.Pop(); // Remove the leaf nodes parent from the path
                                                                           // if it has been added
                                                                           // and pop the parent from the frontier to revisit it or 
                                                                           // it's unvisited predecessors
                            return dfs(frontier.Pop(), Path, frontier, visited, Goal);
                    }
                }
                else // Check a predecessor on the stack having no alternative children
                         // that have not been visited
                         // Enabling correct backtracking
                    {
                    // frontier could be empty here if we have popped and visited all => no solution
                    if (frontier.Count == 0) return null;
                        // Look at Current's parent
                        var top = frontier.Peek();
                        if (top.children.Where(n => !visited.Contains(n)).Count() < 1)
                                frontier.Pop();
                    if(frontier.Count() > 0)
                        if (Path.Contains(frontier.Peek())) Path.Pop(); // Popping the parent from the backtracked
                                                                    // path. It will be added again as we go forward
                    if (frontier.Count > 0)
                        // Check the forntier previous nodes
                        return dfs(frontier.Pop(), Path, frontier, visited, Goal);

                }
            }
            
            return null;
        }

        public List<TNode<T>> bfs(Queue<List<TNode<T>>> Paths , TNode<T> Goal)
        {
            // Paths are extended in reverese order
            if (Paths.Peek().Last().CompareTo(Goal) == 0)
                return Paths.Dequeue();
            // Extend paths dequeues the current 
            List<List<TNode<T>>> nodes = extendPaths(Paths);
            
            if (nodes.Count < 1)
            {
                // Path has been popped and has no successors
                return bfs(Paths, Goal);
            }
            else
            {
                foreach (var extension in nodes)
                    Paths.Enqueue(extension);
            }
            return bfs(Paths, Goal);
        }
        public List<List<TNode<T>>> extendPaths(Queue<List<TNode<T>>> Paths)
        {
            List<List<TNode<T>>> extendedPaths = new List<List<TNode<T>>>();
            if (Paths.Count > 0)
            {
                // Extend the current top most path
                List<TNode<T>> nodes = Paths.Dequeue();
                TNode<T> node = nodes.Last();
                if (node.children.Count > 0)
                {
                    foreach (var child in node.children)
                    {
                        List<TNode<T>> clone = new List<TNode<T>>(nodes);
                        clone.Add(child);
                        extendedPaths.Add(clone);
                    }

                }
            }
            return extendedPaths;
        }
        public void TraverseAcrossTree(TNode<T> node)
        {
            Console.WriteLine("Parent Node {0}", node.Value.ToString());

            if (node.children != null)
            {
                foreach (var item in node.children)
                {
                    Console.WriteLine($"{item.Value.ToString()} is a child of {node.Value.ToString()}");
                }
                foreach (var item in node.children)
                {
                    TraverseAcrossTree(item);
                }

            }

        }

        public TNode<T> Find(TNode<T> current,Stack<TNode<T>> frontier, TNode<T> other)
        {
            if (current != null)
            {
                if (current.CompareTo(other) == 0)
                {
                    return current;
                }
                else if (current.children.Count() > 0)
                {
                    foreach (var node in current.children)
                        frontier.Push(node);
                }

                // Check the children of the current node first
                if (frontier.Count > 0)
                {
                    return Find(frontier.Pop(), frontier, other);
                    }


            }
            // if we are at an null (leaf node)
            return Find(frontier.Pop(), frontier, other);
            
        }

        public TNode<T> InsertAfter(TNode<T> node, T child)
        {
            TNode<T> found = Find(root, new Stack<TNode<T>>(), node);
            if (found != null)
            {
                TNode<T> newNode = new TNode<T>(child, new List<T>());
                found.children.Add(newNode);
                return newNode;
            }
            return null;
        }

    }
}
