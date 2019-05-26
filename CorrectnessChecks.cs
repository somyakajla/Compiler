using System;

namespace ASTBuilder
{

    public class CorrectnessChecks
    {
        private SymtabInterface sti;
        public CorrectnessChecks(SymbolTable st)
        {
            this.sti = st;
        }
        private void oops(string s)
        {
            throw new Exception("aborting because: " + s);
        }
        private void check(bool b, string s)
        {
            if (!b)
            {
                oops(s);
            }
        }
        private int checkLevel(SymtabInterface sti)
        {
            check(sti.CurrentNestLevel >= 0, "Negative level");
            return sti.CurrentNestLevel;
        }
        public virtual void run()
        {
            Console.WriteLine("Starting correctness tests");
            sti = new SymbolTable();
            int level = checkLevel(sti);
            sti.incrNestLevel();
            check(sti.CurrentNestLevel == level + 1, "incremented level did not go up by 1");
            Attributes ti1 = new Attributes(5);
            sti.enter("xyzzy", ti1);
            sti.incrNestLevel();
            check(sti.lookup("xyzzy") == ti1, "Did not find right symbol info");
            Attributes ti2 = new Attributes(6);
            sti.enter("xyzzy", ti2);
            check(sti.lookup("xyzzy") == ti2, "Nested symbol information not returned");
            sti.decrNestLevel();
            check(sti.lookup("xyzzy") == ti1, "Should have gotten back told sym info");
            Console.WriteLine("End correctness tests");
            check(sti.lookup("notpresent") == null, "Failed to not find a nonexistent symbol");

            // push a whole bunch of symbols
            sti.incrNestLevel();
            for (int i = 0; i < 1000; ++i)
            {
                sti.enter("sym" + i, ti1);
            }
            // are they all there?
            for (int i = 0; i < 1000; ++i)
            {
                check(sti.lookup("sym" + i) != null, "missing sym");
            }
            // now pop that level and they should all be gone
            sti.decrNestLevel();
            for (int i = 0; i < 1000; ++i)
            {
                check(sti.lookup("sym" + i) == null, "sym should be gone");
            }

            // Try many symbols, one at each nest level
            for (int i = 1; i <= 70; ++i)
            {
                sti.incrNestLevel();
                sti.enter("sym" + i, new Attributes(i));
            }
            for (int i = 70; i >= 1; --i)
            {
                check(((Attributes)(sti.lookup("sym" + i))).attrVal == i, "bad symbol info");
            }
            for (int i = 70; i >= 2; --i)
            {
                sti.decrNestLevel();
                check(sti.lookup("sym" + i) == null, "sym should be gone");
                check(((Attributes)(sti.lookup("sym" + (i - 1)))).attrVal == i - 1, "bad symbol info");
            }


        }

    }

}