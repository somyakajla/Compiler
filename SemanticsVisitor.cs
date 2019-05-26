using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTBuilder
{
    public class SemanticsVisitor : IReflectiveVisitor
    {
        public string name { get; set; }  // this is to get identifiers name on top level for lookup.
        public string datatype { get; set; }

        // This method is the key to implenting the Reflective Visitor pattern
        // in C#, using the 'dynamic' type specification to accept any node type.
        // The subsequent call to VisitNode does dynamic lookup to find the
        // appropriate Version.
        protected static SymbolTable table = new SymbolTable();

        public virtual void Visit(dynamic node)
        {
            this.VisitNode(node);
        }

        protected static MethodAttributes currentMethod = null;
        protected void SetCurrentMethod(MethodAttributes m)
        {
            currentMethod = m;
        }
        protected MethodAttributes GetCurrentMethod()
        {
            return currentMethod;
        }

        public void CallError(string exeception)
        {
            throw new Exception("aborting because: " + exeception);
        }

        // *** You will need the following if you implement classes.

        //protected static ClassAttributes currentClass = null;

        //protected void SetCurrentClass(ClassAttributes c)
        //{
        //    currentClass = c;
        //}
        //protected ClassAttributes GetCurrentClass()
        //{
        //    return currentClass;
        //}

        // Call this method to begin the semantic checking process
        public void CheckSemantics(AbstractNode node)
        {
            if (node == null)
            {
                return;
            }
            TopDeclVisitor visitor = new TopDeclVisitor();
            node.Accept(visitor);
        }

        public virtual void VisitNode(AbstractNode node)
        {
            VisitChildren(node);
        }
        public virtual void VisitChildren(AbstractNode node)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("< In SemanticsVisitor.VistChildren for " + node.ClassName() + " >");
            Console.ResetColor();

            AbstractNode child = node.Child;
            while (child != null)
            {
                child.Accept(this);
                child = child.Sib;
            };
        }
        //Starting Node of an AST
        public void VisitNode(CompilationUnit node)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("< In SemanticsVisitor.VisitNode for " + node.ClassName() + " >");
            Console.ResetColor();

            TopDeclVisitor visitor = new TopDeclVisitor();
            AbstractNode child = node.Child;
            while (child != null)
            {
                child.Accept(visitor);
                child = child.Sib;
            };
        }       

        public virtual void VisitNode(Identifier node)
        {
            Console.WriteLine($"In Schematic Visitor {node.ClassName()}");
            this.name = node.Name;
        }

        public virtual void VisitNode(MethodCall node)
        {
            Console.WriteLine($"In Schematic Visitor Methiod call looking");
            var methodName = node.Child;
            methodName.Accept(this);
            var name = this.name;
            var attr = table.lookup(name);
            Console.WriteLine($"In Methiod call looking for {name}");
            if (attr == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("< In MethodCall.VisitNode Method Name not exist " + node.ClassName() + "<" + name +" >");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("< In MethodCall.VisitNode Method Name: " + node.ClassName() + "<" + name + " >");
                Console.ResetColor();
            }
            if (node.Child.Sib != null)
                node.Child.Sib.Accept(new TypeVisitor());
        }
        public virtual void VisitNode(Expression node)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("<" + node.ClassName() + ", " + node.exprKind.ToString() + ">: ");
            Console.ResetColor();

            var visitor = new TypeVisitor();
            AbstractNode child = node.Child;
            while (child != null)
            {
                child.Accept(visitor);
                child = child.Sib;
            };
            if (node.Child.Sib == null)
                node.NodeType = node.Child.NodeType;
            else
            {
                if (node.Child.NodeType?.type != node.Child.Sib.NodeType?.type)
                {
                    //CallError($"type mismatch {node.exprKind.ToString()} cannot perform on different datatype");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"type mismatch {node.exprKind.ToString()} cannot perform on different datatype  LHS: {node.Child.NodeType?.type} and RHS : {node.Child.Sib.NodeType?.type}");
                    Console.ResetColor();
                }
                else
                {
                    var boolExprList = new List<ExprKind>() { ExprKind.OP_GE, ExprKind.OP_GT, ExprKind.OP_LE, ExprKind.OP_LT, ExprKind.OP_EQ, ExprKind.OP_NE, ExprKind.OP_LAND, ExprKind.OP_LOR };
                    if (boolExprList.Contains(node.exprKind))
                        node.NodeType = new BooleanTypeDescriptor();
                    else
                        node.NodeType = node.Child.NodeType;
                }
            }
        }

        public virtual void VisitNode(SelectionStatement node)
        {
            Console.Write($"< In Schemantic Visitor {node.ClassName()}>");
            var typeVisitor = new TypeVisitor();
            var child = node.Child;
            while(child != null)
            {
                child.Accept(typeVisitor);
                if (child.NodeType.type != "bool")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"If Clause statement should have bool {node.Child.NodeType?.type}");
                    Console.ResetColor();
                }
                child = child.Sib;
            }
        }

    }
    public class LHSSemanticVisitor : SemanticsVisitor
    {
        public override void Visit(dynamic node)
        {
            this.VisitNode(node);
        }
        //check if Identifier is declared in Symbol table and assignable
        public override void VisitNode(Identifier node)
        {
            
        }

        bool isAssignable(Attributes attr)
        {
            return false;
        }
    }

    public class TopDeclVisitor : SemanticsVisitor
    {
        public override void Visit(dynamic node)
        {
            this.VisitNode(node);
        }

        public override void VisitNode(AbstractNode node)
        {
            VisitChildren(node);
        }

        //define method attribute, increase scope, and check body 
        public void VisitNode(MethodDeclaration node)
        {
            //VisitChildren(node);
            var typeVisitor = new TypeVisitor();
            AbstractNode firstChild = node.Child;

            var returnTypeNode = firstChild.Sib;
            returnTypeNode.Accept(typeVisitor);

            var attr = new MethodAttributes("Method");
            attr.returnType = CreateTypeDiscriptor(typeVisitor.type).type;
            var MethodSig = returnTypeNode.Sib;
            MethodSig.Child.Accept(this);  // visiting first child to get the name of method
            var methodName = this.name;
            attr.methodName = methodName;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("< In MethodDeclaration.VisitNode for " + node.ClassName() + " > " + name + " " + attr.returnType);
            Console.ResetColor();
            table.enter(name, attr);
            //table.PrintTable();
            SetCurrentMethod(attr);
            // Visiting Paramter of methods 
            if (MethodSig.Child.Sib != null)
                MethodSig.Child.Sib.Accept(this);

            var methodBodyNode = MethodSig.Sib;
            methodBodyNode.Accept(this);
        }

        public virtual void VisitNode(PrimitiveType node)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("< In TopDeclVisitor.VisitNode for " + node.ClassName() + " >");
            Console.ResetColor();
        }

        public void VisitNode(Block node)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("< In TopDeclVisitor.VisitNode for " + node.ClassName() + " >");
            Console.ResetColor();

            TopDeclVisitor visitor = new TopDeclVisitor();
            table.incrNestLevel();
            AbstractNode child = node.Child;
            while (child != null)
            {
                child.Accept(visitor);
                child = child.Sib;
            };
            table.decrNestLevel();
        }

        public override void VisitNode(Identifier node)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("< In TopDeclVisitor.VisitNode for " + node.Name + " >");
            Console.ResetColor();
            this.name = node.Name;
        }

        public virtual void VisitNode(LocalVariableDeclaration node)
        {
            var typeVisitor = new TypeVisitor();
            var typeSpecifier = node.Child;
            typeSpecifier.Accept(typeVisitor);
            this.datatype = typeVisitor.type;
            AbstractNode child = node.Child.Sib;
            child.Accept(this);
            //table.PrintTable();
        }

        public virtual void VisitNode(LocalVariableNames node)
        { 
            AbstractNode child = node.Child;
            while (child != null)
            {
                child.Accept(this);
                child.NodeType = CreateTypeDiscriptor(this.datatype);
                Console.WriteLine($"< In TopDeclVisitor.VisitNode for {node.ClassName()} and {this.name}>");
                table.enter(this.name, new TypeAttributes(child.NodeType));
                child = child.Sib;
            };
        }

        public virtual TypeDescriptor CreateTypeDiscriptor(string datatype)
        {
            if (datatype == "INT")
                return new IntegerTypeDescriptor();
            else if (datatype == "VOID")
                return new VoidTypeDescriptor();
            else if (datatype == "BOOLEAN")
                return new BooleanTypeDescriptor();
            else if (datatype == "int32")
                return new IntegerTypeDescriptor();
            return null;
        }

        public virtual void VisitNode(Parameter node)
        {
            var typeVisitor = new TypeVisitor();
            AbstractNode child = node.Child;
            child.Accept(typeVisitor); // to type of type specifier
            child.Sib.Accept(this);
            child.Sib.NodeType = CreateTypeDiscriptor(typeVisitor.type);
            table.enter(this.name, new TypeAttributes(child.Sib.NodeType));
        }
    }

    public class TypeVisitor : TopDeclVisitor
    {
        public string type { set; get; }
        public override void Visit(dynamic node)
        {
            this.VisitNode(node);
        }

        //check if identifier is in symbol table 
        public override void VisitNode(Identifier node)
        {
            //table.PrintTable();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("< In TypeVisitor.VisitNode Identifier for " + node.Name + " >");
            Console.ResetColor();
            var attr = table.lookup(node.Name);
            if (attr == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"<Identifier {node.Name} is not defined>");
                Console.ResetColor();
                //CallError($"Identifier {node.Name} is not defined");
            }
            try
            {
                if (attr.type == "Method")
                {
                    var nodeType = (MethodAttributes)attr;
                    if (nodeType != null)
                    {
                        node.NodeType = CreateTypeDiscriptor(nodeType.returnType);
                        this.name = node.Name;
                    }
                }
                else
                {
                    var nodeType = (TypeAttributes)attr;
                    if (nodeType != null)
                        node.NodeType = nodeType.TYPE;
                }
            }
            catch { }            
        }

        //check if primitive type is in symbol table and get type from its attribute
        public override void VisitNode(PrimitiveType node)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("< In TypeVisitor.VisitNode for " + node.ClassName() + " >");
            Console.ResetColor();
            this.type = node.Type.ToString();
        }

        public void VisitNode(INT_CONST node)
        {
            node.NodeType = new IntegerTypeDescriptor();
            Console.Write("<" + node.ClassName() + ", " + node.NodeType.type + ">: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(node.IntVal);
            Console.ResetColor();
        }

        public void VisitNode(STR_CONST node)
        {
            node.NodeType = new StringTypeDescriptor();
            Console.Write("<" + node.ClassName() + ", " + node.NodeType.type +">: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(node.StrVal);
            Console.ResetColor();
        }

        public override void VisitNode(Expression node)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("<" + node.ClassName() + ", " + node.exprKind.ToString() + ">: ");
            Console.ResetColor();
            var child = node.Child;
            while(child!= null)
            {
                child.Accept(this);
                child = child.Sib;
            }
            if (node.Child.Sib == null)
                node.NodeType = node.Child.NodeType;
            else
            {
                if (node.Child.NodeType?.type != node.Child.Sib.NodeType?.type)
                {
                    //CallError($"type mismatch {node.exprKind.ToString()} cannot perform on different datatype");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"type mismatch {node.exprKind.ToString()} cannot perform on different datatype  LHS: {node.Child.NodeType?.type} and RHS : {node.Child.Sib.NodeType?.type}");
                    Console.ResetColor();
                }
                else
                {
                    var boolExprList = new List<ExprKind>() { ExprKind.OP_GE, ExprKind.OP_GT, ExprKind.OP_LE, ExprKind.OP_LT, ExprKind.OP_EQ, ExprKind.OP_NE, ExprKind.OP_LAND, ExprKind.OP_LOR };
                    if (boolExprList.Contains(node.exprKind))
                        node.NodeType = new BooleanTypeDescriptor();
                    else
                        node.NodeType = node.Child.NodeType;
                }
            }                
        }

        public void VisitNode(ReturnStatement node)
        {
            VisitChildren(node);
            node.NodeType = node.Child.NodeType;
            var curMethod = GetCurrentMethod();
            var methodAttr = (MethodAttributes)table.lookup(curMethod.methodName);
            if (methodAttr.returnType != node.NodeType.type)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"< In ReturnType.VisitNode return type does match with method return type {node.ClassName()} <{methodAttr.returnType}> returntype <{node.NodeType.type}>>");
                Console.ResetColor();
            }
        }

        
        public override void VisitNode(MethodCall node)
        {
            var methodNameNode = node.Child;
            Console.WriteLine($"In TypeVistor:- Methiod call visiting");
            var visitor = new TypeVisitor();
            methodNameNode.Accept(visitor);
            var name = visitor.name;
            var attr = table.lookup(name);
            Console.WriteLine($"In TypeVistor:- Methiod call looking for {name}");
            if (attr == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("< In MethodCall.VisitNode Method Name not exist " + node.ClassName() + "<" + name + " >");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("< In MethodCall.VisitNode Method Name: " + node.ClassName() + "<" + name + $" {((MethodAttributes)attr).returnType}>");
                Console.ResetColor();
                node.NodeType = CreateTypeDiscriptor(((MethodAttributes)attr).returnType);
            }
            if (node.Child.Sib != null)
                node.Child.Sib.Accept(new TypeVisitor());
        }
    }
}
    