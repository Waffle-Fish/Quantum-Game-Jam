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
using TMPro;
using QRG.QuantumForge.Runtime;

namespace QRG.QuantumForge.FaQtory
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private Measure outputMeasureZone;
        public int health = 10;

        [SerializeField] private TextMeshProUGUI text;

        void Start()
        {
            outputMeasureZone.OnMeasureQuantumProperty.AddListener(OnOutputMeasured);
        }

        void Update()
        {
            text.text = health.ToString();
        }

        public void SubtractHealth(int n)
        {
            health -= n;
        }

        void OnOutputMeasured(QuantumProperty outputQuantumProperty)
        {
            int m = QuantumProperty.Measure(outputQuantumProperty)[0];
            if (m == 0) health += 2;
            else health -= 2;
        }
    }
}
