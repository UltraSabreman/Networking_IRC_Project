using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRC_Interface {
    class Util {
        /// <summary>
        /// Lets you print stuff. Any number of objects.
        /// To change forground color, pass one console color and then another object (ie: print(ConsoleColor.Red, "this will be red")).
        /// To change both fore and back colors, path two console colors then another object (ie: print(ConsoleColor.Red, ConsoleColor.White, "this will be red on white")).
        /// </summary>
        /// <param name="stuff">Console colors, and objects to print.</param>
        public static void PrintLine() { PrintLine(null); }
        public static void PrintLine(params object[] stuff) { Print(stuff); Console.WriteLine(); }
        public static void Print(params object[] stuff) {
            if (stuff == null) {
                Console.WriteLine();
                return;
            }

            ConsoleColor oldf = Console.ForegroundColor;
            ConsoleColor oldb = Console.BackgroundColor;

            var enumerator = stuff.GetEnumerator();

            while (enumerator.MoveNext()) {
                Object o = enumerator.Current;

                if (o is ConsoleColor) {
                    Console.ForegroundColor = ((ConsoleColor)o);
                    enumerator.MoveNext();
                    if (enumerator.Current is ConsoleColor) {
                        Console.BackgroundColor = ((ConsoleColor)enumerator.Current);
                        enumerator.MoveNext();
                    }
                    Console.Write(enumerator.Current.ToString());
                } else
                    Console.Write(enumerator.Current.ToString());

                Console.ForegroundColor = oldf;
                Console.BackgroundColor = oldb;
            }
        }

        public static byte[] StoB(String str) {
            return new UTF8Encoding().GetBytes(str + "\n");
        }

        public static String BtoS(byte[] bytes) {
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length).Replace("\0", String.Empty).Trim();
        }
    }
}
