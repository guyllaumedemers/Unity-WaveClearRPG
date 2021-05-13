using UnityEngine;

namespace Characters.Shields {
    public class BasicShield : MonoBehaviour, IShield{
        
        public Stats ShieldStats { get; set; }
        
        public void Block() {
            Debug.Log($"{GetType().Name} Block");
        }
    }
}