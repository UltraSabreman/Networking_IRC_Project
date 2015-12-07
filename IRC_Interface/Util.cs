using System;
using System.Text;

namespace IRC_Interface {
    /// <summary>
    /// Just some static utility functions that dont fit anywhere else.
    /// </summary>
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

        /// <summary>
        /// Converts a string to a byte array using UTF-8.
        /// </summary>
        /// <param name="str">A string</param>
        /// <returns>the corresponding byte array</returns>
        public static byte[] StoB(String str) {
            return new UTF8Encoding().GetBytes(str + "\n");
        }

        /// <summary>
        /// Converts a byte array to a string using 
        /// UTF-8
        /// </summary>
        /// <param name="bytes">a byte array</param>
        /// <returns>the corresponding string.</returns>
        public static String BtoS(byte[] bytes) {
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length).Replace("\0", String.Empty).Trim();
        }
    }
}
