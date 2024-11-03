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
using UnityEngine;
using QRG.QuantumForge.Runtime;

namespace QRG.QuantumForge.Roshambo
{
    [Serializable]
    public class QuantumAction : MonoBehaviour
    {
        [SerializeField] protected string actionName;
        public string ActionName => actionName;

        [SerializeField] protected QuantumProperty[] targetProperties;
        public QuantumProperty[] TargetProperties => targetProperties;

        [SerializeField] protected KeyCode key;
        public KeyCode Key => key;

        public virtual void apply()
        {
            Debug.LogError("Base action apply. You should never see this.");
        }
    }

    [Serializable]
    public class Hadamard : QuantumAction
    {
        public Hadamard(QuantumProperty[] props)
        {
            key = KeyCode.H;
            actionName = "Hadamard";
            this.targetProperties = props;
        }

        public override void apply()
        {
            foreach (var prop in targetProperties)
            {
                Debug.Log($"Applying Hadamard to {prop}");
                prop.Hadamard();
            }
        }
    }

    [Serializable]
    public class Cycle : QuantumAction
    {
        public Cycle(QuantumProperty[] props)
        {
            key = KeyCode.C;
            actionName = "Cycle";
            this.targetProperties = props;
        }
        public override void apply()
        {
            foreach (var quantumProperty in targetProperties)
            {
                quantumProperty.Cycle();
            }
        }
    }

    [Serializable]
    public class NCycle : QuantumAction
    {
        public NCycle(QuantumProperty[] props)
        {
            key = KeyCode.N;
            actionName = "NCycle";
            this.targetProperties = props;
        }

        public override void apply()
        {
            if (targetProperties.Length != 2)
            {
                Debug.LogError("Must supply exactly 2 QuantumProperties to Entangle");
            }
            else
            {
                QuantumProperty.NCycle(targetProperties[0], targetProperties[1]);
            }
        }
    }

    [Serializable]
    public class CompoundAction : QuantumAction
    {
        public QuantumAction[] actions;

        public CompoundAction(QuantumAction[] actions)
        {
            key = KeyCode.None;
            actionName = "CompoundAction";
            this.actions = actions;
        }

        public override void apply()
        {
            foreach (var action in actions)
            {
                action.apply();
            }
        }
    }
}
