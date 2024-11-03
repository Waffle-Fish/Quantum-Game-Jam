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
using Unity.VisualScripting;
using UnityEngine.UI;

namespace QRG.QuantumForge.Roshambo
{
    [RequireComponent(typeof(ProbabilityTracker))]
    public class TokenDisplay : MonoBehaviour
    {
        [SerializeField][Range(0,1)] private float delay = 0.1f;
        [SerializeField] private Image player1;
        [SerializeField] private Image player2;

        [SerializeField] private Sprite rock;
        [SerializeField] private Sprite paper;
        [SerializeField] private Sprite scissors;

        private ProbabilityTracker probabilityTracker;

        void OnEnable()
        {
            probabilityTracker = GetComponent<ProbabilityTracker>();
        }

        void Start()
        {
            StartCoroutine(UpdateTokenCoroutine());
        }

        void UpdateTokens()
        {
            if (probabilityTracker != null)
            {
                var rand = Random.value;
                var probs = probabilityTracker.Probabilities;
                if (probs != null && probs.Length != 0)
                {
                    QuantumProperty.BasisProbability selected;
                    float totalProb = 0;
                    foreach (var basisProbability in probs)
                    {
                        totalProb += basisProbability.Probability;
                        if (rand < totalProb)
                        {
                            player1.sprite = basisProbability.QuditValues[0] switch
                            {
                                "rock" => rock,
                                "paper" => paper,
                                "scissors" => scissors,
                                _ => null
                            };

                            player2.sprite = basisProbability.QuditValues[1] switch
                            {
                                "rock" => rock,
                                "paper" => paper,
                                "scissors" => scissors,
                                _ => null
                            };
                            break;
                        }
                    }
                    

                    
                }
            }
        }

        IEnumerator UpdateTokenCoroutine()
        {
            while (true)
            {
                UpdateTokens();
                yield return new WaitForSeconds(delay);
            }
        }

    }
}
