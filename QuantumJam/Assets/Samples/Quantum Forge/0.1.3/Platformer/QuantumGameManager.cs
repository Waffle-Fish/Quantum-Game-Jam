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
using UnityEngine.UI;
using Platformer;

namespace QRG.QuantumForge.Platformer
{
    public class QuantumGameManager : MonoBehaviour
    {
        public int coinsCounter = 0;

        public GameObject playerGameObject;
        [SerializeField] private PlayerController[] players;
        public GameObject deathPlayerPrefab;
        public Text coinText;

        void Start()
        {
            if (players == null || players.Length == 0)
            {
                Debug.LogError($"{gameObject.name}: Set Players in editor");
            }
        }

        void Update()
        {
            coinText.text = coinsCounter.ToString();
            foreach (var player in players)
            {
                if (player.deathState == true)
                {
                    playerGameObject.SetActive(false);
                    GameObject deathPlayer = (GameObject)Instantiate(deathPlayerPrefab, playerGameObject.transform.position, playerGameObject.transform.rotation);
                    deathPlayer.transform.localScale = new Vector3(playerGameObject.transform.localScale.x, playerGameObject.transform.localScale.y, playerGameObject.transform.localScale.z);
                    player.deathState = false;
                }
            }
            //Invoke("ReloadLevel", 3);

        }

        private void ReloadLevel()
        {
            Application.LoadLevel(Application.loadedLevel);
        }
    }
}