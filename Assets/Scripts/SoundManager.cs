using System;
using System.Collections.Generic;
using UnityEngine;
    
    public class SoundManager : MonoBehaviour{
        public static SoundManager Instance { get; protected set; }
        
        public readonly Dictionary<string, AudioClip> PlayerAudioClips = new Dictionary<string, AudioClip>();
        public readonly Dictionary<string, AudioClip> ItemsAudioClips = new Dictionary<string, AudioClip>();

        public void Awake() {
            if (Instance == null) Instance = this;

            foreach (var clip in Resources.LoadAll<AudioClip>("PlayerSounds")) {
                PlayerAudioClips.Add(clip.name, clip);
            }

            foreach (var clip in Resources.LoadAll<AudioClip>("ItemSounds")) {
                ItemsAudioClips.Add(clip.name, clip);
            }
        }
    }