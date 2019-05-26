using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASTBuilder;

namespace ASTBuilder
{

    public class CompilationUnit : AbstractNode
    {
        // just for the compilation unit because it's the top node
        //public override AbstractNode LeftMostSibling => this;
        public override AbstractNode Sib => null;

        public CompilationUnit(AbstractNode classDecl)
        {
            adoptChildren(classDecl);
        }
    }

    public enum ModifierType { PUBLIC, STATIC, PRIVATE }

    public class Modifiers : AbstractNode
    {
        public List<ModifierType> ModifierTokens { get; set; } = new List<ModifierType>();

        public void AddModType(ModifierType type)
        {
            ModifierTokens.Add(type);
        }

        public Modifiers(ModifierType type) : base()
        {
            AddModType(type);
        }

    }
    public class Identifier : AbstractNode
    {
        public virtual string Name { get; protected set; }

        public Identifier(string s)
        {
            Name = s;
        }
    }
    public class INT_CONST : AbstractNode
    {
        public virtual string IntVal { get; protected set; }

        public INT_CONST(string s)
        {
            IntVal = s;
        }
    }

    public class STR_CONST : AbstractNode
    {
        public virtual string StrVal { get; protected set; }

        public STR_CONST(string s)
        {
            StrVal = s;
        }
    }


    public class MethodDeclaration : AbstractNode
    {
        public MethodDeclaration(
            AbstractNode modifiers,
            AbstractNode typeSpecifier,
            AbstractNode methodSignature,
            AbstractNode methodBody)
        {
            adoptChildren(modifiers);
            adoptChildren(typeSpecifier);
            adoptChildren(methodSignature);
            adoptChildren(methodBody);
        }

    }

    public enum EnumPrimitiveType { BOOLEAN, INT, VOID }
    public class PrimitiveType : AbstractNode
    {
        public EnumPrimitiveType Type { get; set; }
        public PrimitiveType(EnumPrimitiveType type)
        {
            Type = type;
        }

    }

    public class MethodBody : AbstractNode
    {
        public MethodBody(AbstractNode abstractNode)
        {
            adoptChildren(abstractNode);
        }
    }

    public class Parameter : AbstractNode
    {
        public Parameter(AbstractNode typeSpec, AbstractNode declName) : base()
        {
            adoptChildren(typeSpec);
            adoptChildren(declName);
        }
    }

    public class ParameterList : AbstractNode
    {
        public ParameterList(AbstractNode parameter) : base()
        {
            adoptChildren(parameter);
        }
    }

    public class MethodSignature : AbstractNode
    {
        public MethodSignature(AbstractNode name)
        {
            adoptChildren(name);
        }

        public MethodSignature(AbstractNode name, AbstractNode paramList)
        {
            adoptChildren(name);
            adoptChildren(paramList);
        }
    }

    public class MethodCall : AbstractNode
    {
        public MethodCall(AbstractNode name)
        {
            adoptChildren(name);
        }

        public MethodCall(AbstractNode name, AbstractNode paramList)
        {
            adoptChildren(name);
            adoptChildren(paramList);
        }
    }

    public enum ExprKind
    {
        EQUALS, OP_LOR, OP_LAND, PIPE, HAT, AND, OP_EQ,
        OP_NE, OP_GT, OP_LT, OP_LE, OP_GE, PLUSOP, MINUSOP,
        ASTERISK, RSLASH, PERCENT, UNARY, PRIMARY
    }
    public class Expression : AbstractNode
    {
        public ExprKind exprKind { get; set; }
        public Expression(AbstractNode expr, ExprKind kind)
        {
            adoptChildren(expr);
            exprKind = kind;
        }
        public Expression(AbstractNode lhs, ExprKind kind, AbstractNode rhs)
        {
            adoptChildren(lhs);
            adoptChildren(rhs);
            exprKind = kind;
        }
    }

    public class Block : AbstractNode
    {
        public Block(AbstractNode blkCode)
        {
            adoptChildren(blkCode);
        }
    }

    public class LocalItem : AbstractNode
    {
        public LocalItem(AbstractNode localItem)
        {
            adoptChildren(localItem);
        }
    }

    public class StructDeclaration : AbstractNode
    {
        public StructDeclaration(AbstractNode modifier, AbstractNode identifier, AbstractNode structBody)
        {
            adoptChildren(modifier);
            adoptChildren(identifier);
            adoptChildren(structBody);
        }
    }

    public class LocalVariableDeclaration : AbstractNode
    {
        public LocalVariableDeclaration(AbstractNode typeSpecifier, AbstractNode localVariableName)
        {
            adoptChildren(typeSpecifier);
            adoptChildren(localVariableName);
        }
    }

    public class ArgumentList : AbstractNode
    {
        public ArgumentList(AbstractNode expr)
        {
            adoptChildren(expr);
        }
    }

    public class SelectionStatement : AbstractNode
    {
        public SelectionStatement(AbstractNode expr, AbstractNode stmt, AbstractNode elseStmt)
        {
            adoptChildren(expr);
            adoptChildren(stmt);
            adoptChildren(elseStmt);
        }
        public SelectionStatement(AbstractNode expr, AbstractNode stmt)
        {
            adoptChildren(expr);
            adoptChildren(stmt);
        }
    }
    public class IterationStatement : AbstractNode
    {
        public IterationStatement(AbstractNode whileExpr, AbstractNode stmt)
        {
            adoptChildren(whileExpr);
            adoptChildren(stmt);
        }
    }
    public class ReturnStatement : AbstractNode
    {
        public ReturnStatement(AbstractNode retrn)
        {
            adoptChildren(retrn);
        }
    }

    public class LocalVariableNames : AbstractNode
    {
        public LocalVariableNames(AbstractNode localVarName)
        {
            adoptChildren(localVarName);
        }
    }

    public class LocalItems : AbstractNode
    {
        public LocalItems(AbstractNode localItems)
        {
            adoptChildren(localItems);
        }
    }

    public class ClassDeclaration : AbstractNode
    {
        public ClassDeclaration(AbstractNode modifiers, AbstractNode identifier, AbstractNode classbody)
        {
            adoptChildren(modifiers);
            adoptChildren(identifier);
            adoptChildren(classbody);
        }
    }

    public class ClassBody : AbstractNode
    {
        public ClassBody(AbstractNode classbdy)
        {
            adoptChildren(classbdy);
        }
    }
}
