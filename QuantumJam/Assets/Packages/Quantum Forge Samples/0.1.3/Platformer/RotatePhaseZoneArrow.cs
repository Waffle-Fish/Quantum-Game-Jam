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

namespace QRG.QuantumForge.Platformer
{
    public class RotatePhaseZoneArrow : MonoBehaviour
    {
        [SerializeField] private PhaseRotate _phaseRotate;

        [SerializeField] private bool _rotateX;
        [SerializeField] private bool _rotateY;
        [SerializeField] private bool _rotateZ;

        private Vector3 _initialRotation;

        // Start is called before the first frame update
        void Start()
        {
            _initialRotation = transform.localEulerAngles;
        }

        // Update is called once per frame
        void Update()
        {
            if (_phaseRotate == null)
            {
                Debug.Log("Set phase tracker to rotate by phase.");
                return;
            }
            var rotation = _initialRotation;
            var angle = _phaseRotate.Radians * 180 / Mathf.PI;
            if (_rotateX)
            {
                rotation.x += angle;
            }
            if (_rotateY)
            {
                rotation.y += angle;
            }
            if (_rotateZ)
            {
                rotation.z += angle;
            }
            transform.localEulerAngles = rotation;
            
        }
    }
}
