using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTBuilder
{
    class PrintVisitor : IReflectiveVisitor
    {
        // This method is the key to implenting the Reflective Visitor pattern
        // in C#, using the 'dynamic' type specification to accept any node type.
        // The subsequent call to VisitNode does dynamic lookup to find the
        // appropriate Version.

        public void Visit(dynamic node)
        {
            this.VisitNode(node);
        }

        // Call this method to begin the tree printing process

        public void PrintTree(AbstractNode node, string prefix = "")
        {
            if (node == null)
            {
                return;
            }
            bool isLastChild = (node.Sib == null);

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(prefix);
            Console.Write(isLastChild ? "└─ " : "├─ ");
            Console.ResetColor();
            node.Accept(this);
            VisitChildren(node, prefix + (isLastChild ? "   " : "│ "));
        }
        public void VisitChildren(AbstractNode node, String prefix)
        {
            AbstractNode child = node.Child;
            while (child != null)
            {
                PrintTree(child, prefix);
                child = child.Sib;
            };
        }

        // This method is defined for a parameter of the parent class
        // AbstractNode, so it will be invoked on a node if there is not
        // a specialized method defined below.
        public void VisitNode(AbstractNode node)
        {
            Console.WriteLine("<" + node.ClassName() + ">");
        }

        // Here are three specialized VisitNode methods for terminals
        // You will be adding more methods here for other nodes that hold
        // information of interest beyond the class name.
        public void VisitNode(Identifier node)
        {
            if (node.NodeType != null)
                Console.Write("<" + node.ClassName() + ", " + node.NodeType.type + ">: ");
            else
                Console.Write("<" + node.ClassName() + ">: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(node.Name);
            Console.ResetColor();
        }

        public void VisitNode(INT_CONST node)
        {
            if (node.NodeType != null)
                Console.Write("<" + node.ClassName() + ", " + node.NodeType.type + ">: ");
            else
                Console.Write("<" + node.ClassName() + ">: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(node.IntVal);
            Console.ResetColor();
        }

        public void VisitNode(STR_CONST node)
        {
            if (node.NodeType != null)
                Console.Write("<" + node.ClassName() + ", " + node.NodeType.type +">: ");
            else
                Console.Write("<" + node.ClassName() + ">: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(node.StrVal);
            Console.ResetColor();
        }

        public void VisitNode(PrimitiveType node)
        {
            Console.Write("<" + node.ClassName() + ">: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(node.Type);
            Console.ResetColor();
        }

        public void VisitNode(Modifiers node)
        {
            Console.Write("<" + node.ClassName() + ">: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            foreach (object o in node.ModifierTokens)
            {
                Console.Write(o + " ");
            }
            Console.WriteLine("");
            Console.ResetColor();
        }

        public void VisitNode(Expression node)
        {
            if (node.NodeType != null)
                Console.Write("<" + node.ClassName() + ", " + node.NodeType.type + ">: ");
            else
                Console.Write("<" + node.ClassName() + ">: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(node.exprKind);
            Console.ResetColor();
        }

        public void VisitNode(IterationStatement node)
        {
            Console.Write("<" + node.ClassName() + ">: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("WHILE");
            Console.ResetColor();
        }
    }

}