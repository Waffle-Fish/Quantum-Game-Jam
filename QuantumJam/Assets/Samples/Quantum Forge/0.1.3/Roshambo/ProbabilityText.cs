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
using System.Text;
using UnityEngine;
using QRG.QuantumForge.Runtime;

namespace QRG.QuantumForge.Roshambo
{
    [RequireComponent(typeof(TMPro.TextMeshProUGUI))]
    public class ProbabilityText : MonoBehaviour
    {
        public ProbabilityTracker tracker;
        private TMPro.TextMeshProUGUI text;

        void OnEnable()
        {
            text = GetComponent<TMPro.TextMeshProUGUI>();
        }

        // Update is called once per frame
        void Update()
        {
            if (tracker != null && text != null)
            {
                if (tracker.Probabilities != null)
                {
                    var probs = tracker.Probabilities;
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < probs.Length; i++)
                    {
                        if (probs[i].Probability != 0.0f)
                        {
                            sb.AppendLine($"{probs[i].ToString()}");
                        }
                    }

                    text.text = sb.ToString();
                }
            }
        }
    }
}
