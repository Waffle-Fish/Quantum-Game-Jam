//# Copyright 2024 Quantum Realm Games
//#
//# Licensed under the Quantum Realm Games Quantum Forge Unity Toolkit 
//# license, Version 1.0 (the "License"); you may not use this file 
//# except in compliance with the License.
//# You may obtain a copy of the License at
//#
//#     https://www.quantumrealmgames.com/quantum_forge_toolkit_license
//#
//# Unless required by applicable law or agreed to in writing, software
//# distributed under the License is distributed on an "AS IS" BASIS,
//# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//# See the License for the specific language governing permissions and
//# limitations under the License.

using System.Collections;
using System.Collections.Generic;
using QRG.QuantumForge.Runtime;
using UnityEngine;
using UnityEngine.Events;

namespace QRG.QuantumForge.FaQtory
{
    public class FaQtoryFunction : MonoBehaviour
    {
        public UnityEvent onExecute;

        [SerializeField] private int numInputs = 2;
        [SerializeField] private List<QuantumProperty> input;
        [SerializeField] private QuantumProperty output;

        public bool isConstant = false;

        public void AddInput(QuantumProperty quantumProperty)
        {
            input.Add(quantumProperty);
            if (input.Count == numInputs && output != null)
            {
                Execute();
            }
        }

        public void Reset()
        {
            output = null;
            input.Clear();
        }

        public void Randomize()
        {
            isConstant = !isConstant;
        }

        public void SetOutput(QuantumProperty quantumProperty)
        {
            output = quantumProperty;
            if (input.Count == numInputs && output != null)
            {
                Execute();
            }
        }

        void Execute()
        {
            if (isConstant)
            {
                Debug.Log("Applying constant function cycle to output.");
                QuantumProperty.Cycle(output);
                
            }
            else
            {
                Debug.Log("Applying balanced function cycle to output.");
                // predicated cycle, 000->000, 010->011, 100->101, 110 -> 110 
                foreach (var inQ in input)
                {
                    QuantumProperty.Cycle(output, inQ.is_value(1));
                }
            }

            output.gameObject.GetComponent<ShowIcon>().showKnownState = false;

            output = null;
            input.Clear();
            onExecute.Invoke();
        }
    }
}
