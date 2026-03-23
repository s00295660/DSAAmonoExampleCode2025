using DSATrees;

namespace Trees2025
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GenericTree<string> t = new DSATrees.GenericTree<string>();

            t.Root = new TNode<string>("A", new List<TNode<string>>()
            {
                new TNode<string>("B", new List<TNode<string>>() // A->B
                {
                    new TNode<string>("X", // A->B->X
                    children: new List<TNode<string>>{ new TNode<string>("G")}), // A->B->X->G
                    new TNode<string>("G") // A->B->G
                }),
                new TNode<string>("C", children:new List<TNode<string>>() // A->C
                {
                    new TNode<string>("D"), // a->C->D
                    new TNode<string>("E") // A->C->E
                }) 
            });
            //---------------------------------------------------------------------------------------------------
            // The following code demonstrates the standard DFS method called with the defined tree
            // and the fomulated goal node "G".
            Stack<TNode<string>> frontier = new Stack<TNode<string>>();
            frontier.Push(t.Root);
            // the dfs method will return a stack representing the path
            // from the root to the goal node if a solution is found
            Stack<TNode<string>>? SolutionPath =  t.dfs(Current:t.Root, 
                                                        Path:new Stack<TNode<string>>() { },
                                                        frontier, 
                                                        visited:new List<TNode<string>>(), 
                                                        Goal:new TNode<string>("G"));
            if(SolutionPath != null)
            {
                var SolutionPathList = new List<TNode<string>>();
                reverse_stack(SolutionPathList, SolutionPath);
                Console.WriteLine("Full DFS Solution Path:");
                foreach (var node in reverse_stack(SolutionPathList, SolutionPath))
                    Console.WriteLine(node.Value);
            }
            else
                Console.WriteLine("No solution found");
            // ---------------------------------------------------------------------------------------------------
            // The following code demonstrates the simplex_dfs method called with the same tree and goal node "G"
            List<TNode<string>> simplex_solution = new List<TNode<string>>();
            // if there is a solution,
            // the simplex_dfs method will populate the simplex_solution list
            // with the path from the root to the goal node
            if (t.simplex_dfs(Level:0, node:t.Root, Goal:new TNode<string>("G"), 
                visited:new List<TNode<string>>(), simplex_solution))
                    if(simplex_solution.Count > 0)
                    {
                    simplex_solution.Add(t.Root);
                    simplex_solution.Reverse();
                    Console.WriteLine("Simplex DFS Solution Path:");
                        foreach (var node in simplex_solution)
                            Console.WriteLine(node.Value);
                    }
                    else
                        Console.WriteLine("No solution found with simplex DFS");

            Console.ReadLine();
        }
        public static List<TNode<string>> reverse_stack(List<TNode<string>> reverse, Stack<TNode<string>> stack)
        {
            if (stack.Count > 0)
            {
                TNode<string> node= stack.Pop();
                reverse_stack(reverse,stack);
                reverse.Add(node);  
            }

            return reverse;
        }
    }
}
