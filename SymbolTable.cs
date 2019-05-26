using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTBuilder
{
    public class SymbolTableEntry
    {
        public string NAME { get; set; }
        public Attributes TYPE { get; set; }
        public int LEVEL { get; set; }
    }

    public class SymbolTable : SymtabInterface
    {

        /// <summary>
        /// Should never be a negative integer 
        /// </summary>
        protected static int nestLevel = 0;

        // *** Declarations for your chosen implementation of the 
        // *** symbol table go here.  Some part of this implementation
        // *** should be what gets saved when a symbol table becomes
        // *** part of the Attributes of something like a class
        IDictionary<string, List<SymbolTableEntry>> symTable;

        List<List<SymbolTableEntry>> scopeDisplay;

        public SymbolTable()
        {
            // *** Do any  initialization necessary to create a global 
            // *** name scopre and then initialize it with built-in names ...
            symTable = new Dictionary<string, List<SymbolTableEntry>>();
            scopeDisplay = new List<List<SymbolTableEntry>>();
            scopeDisplay.Add(new List<SymbolTableEntry>());
            EnterPredefinedNames();
        }
        /// <summary>
        /// Enter predefined names into symbol table.  
        /// </summary>
        public void EnterPredefinedNames()
        {
            TypeAttributes attr = new TypeAttributes(new IntegerTypeDescriptor());
            enter("INT", attr);
            attr = new TypeAttributes(new BooleanTypeDescriptor());
            enter("BOOLEAN", attr);
            attr = new TypeAttributes(new VoidTypeDescriptor());
            enter("VOID", attr);
            attr = new TypeAttributes(new StringTypeDescriptor());
            enter("String", attr);
            attr = new TypeAttributes(new MSCorLibTypeDescriptor());
            enter("Write", attr);
            attr = new TypeAttributes(new MSCorLibTypeDescriptor());
            enter("WriteLine", attr);
        }
        public int CurrentNestLevel
        {
            get
            {
                return nestLevel;
            }
        }

        /// <summary> 
        /// Opens a new scope, retaining outer ones </summary>
        public virtual void incrNestLevel()
        {
            nestLevel++;
            scopeDisplay.Add(new List<SymbolTableEntry>());
        }

        private void DeleteSymTableEntry(SymbolTableEntry sym)
        {
            if(symTable.ContainsKey(sym.NAME))
            {
                List<SymbolTableEntry> symbolTableEntryList = symTable[sym.NAME];
                foreach (var item in symbolTableEntryList)
                {
                    if (item.LEVEL == sym.LEVEL)
                    {
                        if (symbolTableEntryList.Count() == 1)
                            symTable.Remove(sym.NAME);
                        else
                            symbolTableEntryList.Remove(item);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Closes the innermost scope and returns the HashTable used to implement the scope
        /// </summary>
        public virtual Hashtable decrNestLevel()
        {
            List<SymbolTableEntry> symTableEntryList = scopeDisplay[CurrentNestLevel];
            foreach(var item in symTableEntryList)
            {
                DeleteSymTableEntry(item);    
            }
            scopeDisplay.RemoveAt(CurrentNestLevel);
            nestLevel--;

            return null;  //necessary so that this method compiles until you implement it.
        }

        /// <summary>
        /// Enter the given symbol information into the symbol table.  If the given
        ///    symbol is already present at the current nest level, produce an error
        ///    message, but do NOT throw any exceptions from this method.
        /// </summary>
        public virtual void enter(string s, Attributes info)
        {
            SymbolTableEntry symbolTableEntry = RetrieveSymbol(s);

            if (symbolTableEntry != null)
            {
                return; // error message; symbol is already in the current scope dont add it 
            }
            else
            {
                SymbolTableEntry tableEntry = null;
                if (symTable.ContainsKey(s))
                {
                    List<SymbolTableEntry> existingSymbolTableEntryList = symTable[s];
                    if (existingSymbolTableEntryList != null)
                    {
                        tableEntry = AddSymbolTableEntry(s, info);
                        existingSymbolTableEntryList.Add(tableEntry);
                    }                   
                }
                else
                {
                    List<SymbolTableEntry> newSymbolTableEntryList = new List<SymbolTableEntry>();
                    tableEntry = AddSymbolTableEntry(s, info);
                    newSymbolTableEntryList.Add(tableEntry);
                    symTable.Add(s, newSymbolTableEntryList);
                }

                List<SymbolTableEntry> currentScopeList = scopeDisplay[CurrentNestLevel];
                if (!currentScopeList.Contains(tableEntry))
                    currentScopeList.Add(tableEntry);
            }

        }

        public virtual SymbolTableEntry AddSymbolTableEntry(string name, Attributes attributes)
        {
            SymbolTableEntry symbolTableEntry = new SymbolTableEntry();
            symbolTableEntry.NAME = name;
            symbolTableEntry.TYPE = attributes;
            symbolTableEntry.LEVEL = CurrentNestLevel;
            return symbolTableEntry;
        }

        public SymbolTableEntry RetrieveSymbol(string s)
        {
            if (symTable.ContainsKey(s))
            {
                List<SymbolTableEntry> symbolTableEntryList = symTable[s];
                foreach (var item in symbolTableEntryList)
                {
                    if (item.LEVEL == CurrentNestLevel)
                        return item;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the information associated with the innermost currently valid
        ///     declaration of the given symbol.  If there is no such valid declaration,
        ///     return null.  Do NOT throw any excpetions from this method.
        /// </summary>
        public virtual Attributes lookup(string s)
        {
            //if name s is not found, return null Attributes reference   
            if (!String.IsNullOrEmpty(s) && symTable.ContainsKey(s))
            {
                List<SymbolTableEntry> symbolTableEntryList = symTable[s];
                var len = symbolTableEntryList.Count();
                return symbolTableEntryList.ElementAt(len - 1).TYPE;
            }

            return null;
        }

        public virtual bool declaredLocally(string s)
        {
            return false;
        }

        public void PrintTable()
        {
            foreach (string key in symTable.Keys)
            {
                Console.WriteLine("Key: {0}", key);
            }
        }
        public virtual void @out(string s)
        {
            string tab = "";
            for (int i = 1; i <= CurrentNestLevel; ++i)
            {
                tab += "  ";
            }
            Console.WriteLine(tab + s);
        }

        public virtual void err(string s)
        {
            @out("Error: " + s);
            Console.Error.WriteLine("Error: " + s);
            Environment.Exit(-1);
        }

        // public virtual void @out(AbstractNode n, string s)
        // {
        //@out("" + n.NodeNum + ": " + s + " " + n);
        // }
        // public virtual void err(AbstractNode n, string s)
        // {
        //err("" + n.NodeNum + ": " + s + " " + n);
        // }

    }
}
