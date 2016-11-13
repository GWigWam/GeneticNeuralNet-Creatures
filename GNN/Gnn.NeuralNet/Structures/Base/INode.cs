using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.NeuralNet.Structures.Base {

    public interface INode {

        event Action OutputChanged;

        float Output { get; }
    }
}