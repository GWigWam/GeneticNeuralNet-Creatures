using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.Iris {

    public class Program {
        private const int TrainSetSize = 130;
        private static Random random = new Random();

        private static void Main(string[] args) {
            var data = DataReader.ReadFromFile("data/iris.data")/*.OrderBy(i => random.Next())*/;

            Console.ReadKey();
        }
    }
}