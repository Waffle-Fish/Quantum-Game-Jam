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

namespace QRG.QuantumForge.FaQtory
{
    public class Draggable : MonoBehaviour
    {
        public float fixedY = 1.0f;
        
        private Vector3 _mouseOffset;
        private Plane _dragPlane;

        void Start()
        {
            _dragPlane = new Plane(Vector3.up, Vector3.up * fixedY);
        }

        private Vector3 GetMouseWorldPosition()
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        private Vector3 GetMouseWorldPositionOnDragPlane()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float distance;
            _dragPlane.Raycast(ray, out distance);
            return ray.GetPoint(distance);
        }

        private void OnMouseDown()
        {

            _mouseOffset = gameObject.transform.position - GetMouseWorldPosition();
        }

        private void OnMouseDrag()
        {
            transform.position = GetMouseWorldPositionOnDragPlane();// + _mouseOffset;
            //transform.position = new Vector3(transform.position.x, fixedY, transform.position.z);
        }
    }
}
