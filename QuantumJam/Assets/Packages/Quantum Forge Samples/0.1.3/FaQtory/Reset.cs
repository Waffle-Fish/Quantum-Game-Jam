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

using QRG.QuantumForge.FaQtory;
using System.Collections;
using System.Collections.Generic;
using QRG.QuantumForge.Runtime;
using UnityEngine;

namespace QRG.QuantumForge.FaQtory
{
    public class Reset : MonoBehaviour
    {
        [SerializeField] private Transform outputSpawn;
        [SerializeField] private Transform input1Spawn;
        [SerializeField] private Transform input2Spawn;
        [SerializeField] private FaQtoryFunction faqtoryFunction;

        [SerializeField] private QuantumProperty output;
        [SerializeField] private QuantumProperty input1;
        [SerializeField] private QuantumProperty input2;

        public void DoReset(float delay)
        {
            StartCoroutine(ResetCoroutine(delay));
        }

        void SetValue(QuantumProperty quantumProperty, int value)
        {
            var m = QuantumProperty.Measure(quantumProperty)[0];
            while (m != value)
            {
                QuantumProperty.Cycle(quantumProperty);
                m = QuantumProperty.Measure(quantumProperty)[0];
            }
        }

        private IEnumerator ResetCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);

            faqtoryFunction.Reset();

            output.gameObject.transform.position = outputSpawn.position;
            output.GetComponent<ShowIcon>().SetShowKnownState(true);
            input1.gameObject.transform.position = input1Spawn.position;
            input1.GetComponent<ShowIcon>().SetShowKnownState(true);
            input2.gameObject.transform.position = input2Spawn.position;
            input2.GetComponent<ShowIcon>().SetShowKnownState(true);

            SetValue(output, 1);
            SetValue(input1, 0);
            SetValue(input2, 0);
        }
    }
}
