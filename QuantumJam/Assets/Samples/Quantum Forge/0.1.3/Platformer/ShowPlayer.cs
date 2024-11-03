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
using System.Linq;
using UnityEngine;
using QRG.QuantumForge.Runtime;

namespace QRG.QuantumForge.Platformer
{
    public class ShowPlayer : MonoBehaviour
    {
        [SerializeField] private ProbabilityTracker probabilityTracker;

        [SerializeField] private GameObject topPlayer;
        [SerializeField] private GameObject bottomPlayer;

        void Start()
        {
            if (probabilityTracker == null)
            {
                probabilityTracker = GetComponent<ProbabilityTracker>();
            }
            if(probabilityTracker == null)
            {
                Debug.LogError($"{gameObject.name}: No ProbabilityTracker found on this object. Set ProbabilityTracker to track");
            }

            if (topPlayer == null)
            {
                Debug.LogError($"{gameObject.name}: No topPlayer found on this object. Set topPlayer to track");
            }
            if(bottomPlayer == null)
            {
                Debug.LogError($"{gameObject.name}: No bottomPlayer found on this object. Set bottomPlayer to track");
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (probabilityTracker != null)
            {
                var probabilities = probabilityTracker.Probabilities;
                if (probabilities != null && probabilities.Length > 0)
                {
                    float topProbability = 0.0f;
                    float bottomProbability = 0.0f;

                    topProbability = probabilities.First(x => x.QuditValues[0] == "top").Probability;
                    bottomProbability = probabilities.First(x => x.QuditValues[0] == "bottom").Probability;

                    topPlayer.SetActive(topProbability > 0.0f);
                    bottomPlayer.SetActive(bottomProbability > 0.0f);
                }
            }

        }
    }
}
