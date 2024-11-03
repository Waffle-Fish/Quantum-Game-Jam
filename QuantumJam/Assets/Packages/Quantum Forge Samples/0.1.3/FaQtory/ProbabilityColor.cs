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

namespace QRG.QuantumForge.FaQtory
{
    public class ProbabilityColor : MonoBehaviour
    {
        public Color color0;
        public Color color1;
        public ProbabilityTracker tracker;

        private Renderer _renderer;

        void Start()
        {
            _renderer = GetComponent<Renderer>();
        }

        // Update is called once per frame
        void Update()
        {
            var probabilities = tracker.Probabilities;
            if(probabilities.Length == 0)
            {
                return;
            }
            var color = Color.Lerp(color1, color0, probabilities[0].Probability);
            _renderer.material.color = color;
        }
    }
}
