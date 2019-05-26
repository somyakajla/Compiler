using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTBuilder
{
    /****************************************************/
    /*Information about symbols held in AST nodes and the symbol table*/
    /****************************************************/

    public class Attributes
    {
        public int attrVal;
        public string type { get; set; }

        public Attributes(int integerValue)
        {
            this.attrVal = integerValue;
        }
    }

    public class VariableAttributes : Attributes
    {
        public string variableType;
        public VariableAttributes(string varType) : base(0)
        {
            this.variableType = varType;
        }
    }

    public class TypeAttributes : Attributes
    {
        public TypeDescriptor TYPE { get; set; }
        public TypeAttributes(TypeDescriptor type) : base(0)
        {
            TYPE = type;
        }
    }

    public class MethodAttributes : Attributes
    {
        public string returnType { get; set; }
        public string methodName { get; set; }
        public MethodAttributes(string tp) : base(0)
        {
            type = tp;
        }
    }
}
