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
using UnityEngine;
using QRG.QuantumForge.Runtime;

namespace QRG.QuantumForge.Roshambo
{
    public class MeasureOnKey : MonoBehaviour
    {
        public QuantumProperty player1;
        public QuantumProperty player2;
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                QuantumProperty.Measure(player1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                QuantumProperty.Measure(player2);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                QuantumProperty.Measure(player1, player2);
            }
        }
    }
}
