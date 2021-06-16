# Unity-Wave-Clear RPG

Welcome to Unity-Wave-Clear RPG, a group project made in the academic field.</br>
Useful scripts are located in Assets/Scripts/..

## Getting Started

This repository has for objective of showcasing what could be achieve with a group of four individuals under a week.
Some of the features presented here are obviously not mine and I won't take credit for them. __A detail list of the features I worked on will follow.__

## Content

```
NOTE : Scripts created by myself can be found here.
```
```
NOTE : Empty Scripts were meant to be additionnal features that couldn't be implemented due to the short period of time allowed for this project. 
```

* [Assets/Scripts/Inventory/DisplayInventory/base](https://github.com/guyllaumedemers/Unity-WaveClearRPG/tree/main/Assets/Scripts/Inventory/DisplayInventory/base) : Inventory UI 
* [Assets/Scripts/Inventory/ScriptableObjects/Inventory/base](https://github.com/guyllaumedemers/Unity-WaveClearRPG/tree/main/Assets/Scripts/Inventory/Scriptable%20Objects/Inventory/base) : InventoryObject 👉 *base class of the inventory scriptableObject*</br>
👉 *[item base class for serialzation can be found here](https://github.com/guyllaumedemers/Unity-WaveClearRPG/blob/main/Assets/Scripts/Inventory/Scriptable%20Objects/Items/ItemObject.cs)*
* [Assets/Scripts/Inventory/Database](https://github.com/guyllaumedemers/Unity-WaveClearRPG/tree/main/Assets/Scripts/Inventory/Database) : Serialization of scriptableObjects 
* [Assets/Scripts/Characters/Enemy](https://github.com/guyllaumedemers/Unity-WaveClearRPG/tree/main/Assets/Scripts/Characters/Enemy) : Behaviour Tree 👉 *can be found under the name BasicEnemyBehaviourTree*

```
NOTE : Serialization function are hold in the InventoryObject.cs available at the following path : 
```

#### Game Mechanics and Features

* Nav Mesh System
* A*
* Input System
* AI 👉 *using behaviour tree and a finite state machine*
* Inventory System 👉 *using scriptableObjects*
* Serialization (*of the inventory*)
* Audio Management
* Animation Management
* Level Management / Scene Loading

##### Contribution

* AI 👉 *using behaviour tree only*
* Inventory System (*using scriptableObjects*)
* Serialization (*of the inventory*)
