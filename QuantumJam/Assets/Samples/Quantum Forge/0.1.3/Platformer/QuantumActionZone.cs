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
using QRG.QuantumForge.Platformer;
using QRG.QuantumForge.Runtime;
using UnityEngine;

namespace QRG.QuantumForge.Platformer
{
    public class QuantumActionZone : MonoBehaviour
    {
        [SerializeField] private GameObject _bottomZone;
        [SerializeField] private GameObject _topZone;
        [SerializeField] private GameObject _bottomPlayer;
        [SerializeField] private GameObject _topPlayer;

        [SerializeField] private float _activationDelay;

        private float _lastActivation = -1;

        public void OnZoneEnter()
        {
            if(Time.time - _lastActivation < _activationDelay)
            {
                return;
            }
            _lastActivation = Time.time;
            _topPlayer.transform.position = _topZone.transform.position;
            _bottomPlayer.transform.position = _bottomZone.transform.position;

            GetComponent<IQuantumAction>().apply();
        }


    }
}
