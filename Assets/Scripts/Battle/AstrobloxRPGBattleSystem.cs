using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstrobloxRPG
{
    /// <summary>
    /// Main entry point for the RPG Battle System
    /// </summary>
    public class AstrobloxRPGBattleSystem : MonoBehaviour
    {
        [Header("Scene Setup")]
        [SerializeField] private MainSceneSetup sceneSetup;
        
        private void Awake()
        {
            Debug.Log("Astroblox RPG Battle System initialized!");
            Debug.Log("This is an auto-battle system that runs without player control.");
            Debug.Log("When the battle starts, agents will automatically take actions based on their speed.");
            Debug.Log("The battle ends when all players or all enemies are defeated.");
        }
    }
}
