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

using System;
using System.Collections;
using System.Collections.Generic;
using QRG.QuantumForge.Runtime;
using UnityEngine;

namespace QRG.QuantumForge.FaQtory
{
    public class ShowIcon : MonoBehaviour
    {
        public bool showKnownState = true;
        [SerializeField] private GameObject icon0;
        [SerializeField] private GameObject icon1;
        [SerializeField] private GameObject iconQuantum;
        [SerializeField] private GameObject iconUnknown;
        [SerializeField] private ProbabilityTracker probabilityTracker;

        void Update()
        {
            var probabilities = probabilityTracker.Probabilities;
            if (probabilities == null || probabilities.Length != 2) return;
            icon0.SetActive(false);
            icon1.SetActive(false);
            iconQuantum.SetActive(false);
            iconUnknown.SetActive(false);
            if (showKnownState)
            {
                if (probabilities[0].Probability > 0.9999) icon0.SetActive(true);
                else if (probabilities[1].Probability > 0.9999) icon1.SetActive(true);
                else iconQuantum.SetActive(true);
            }
            else
            {
                iconUnknown.SetActive(true);
            }
        }

        public void SetShowKnownState(bool showKnownState)
        {
            this.showKnownState = showKnownState;
        }

        void OnTriggerEnter(Collider otherCollider)
        {
            if (otherCollider.gameObject.GetComponent<Measure>()) showKnownState = true;
            //if (otherCollider.gameObject.GetComponent<Hadamard>()) showKnownState = false;
        }
    }
}
