# 🦇 Vampire Survivors Rogue-like 2D

[![Unity Version](https://img.shields.io/badge/Unity-2021.3+-blue.svg?style=for-the-badge&logo=unity)](https://unity.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg?style=for-the-badge)](LICENSE)
[![Platform](https://img.shields.io/badge/Platform-PC%20%7C%20Web-orange.svg?style=for-the-badge)](https://unity.com/)

A polished 2D time-survival rogue-lite game built with Unity, inspired by the hit game **Vampire Survivors**. Survive against hordes of monsters, collect experience, and upgrade your arsenal to become an unstoppable force.

---

## 🎮 Game Overview

In **Vampire Survivors Rogue-like 2D**, you take on the role of a hero fighting against endless waves of enemies. The goal is simple: survive as long as possible. As you defeat enemies, they drop experience gems that allow you to level up and choose from a variety of weapons and passive items.

### ✨ Key Features

- **🏹 Dynamic Combat System**: Automatic firing weapons allow you to focus on movement and positioning.
- **🆙 Rogue-lite Progression**: Choose from random upgrades every time you level up to create unique builds.
- **⚔️ Diverse Arsenal**:
  - **Active Weapons**: Whip, Axe, Magic Wand, Holy Aura, and more.
  - **Passive Items**: Boost your speed, health, damage, or pickup range.
- **👾 Challenging Enemies**: Multiple enemy types with unique behaviors, from simple chasers to charging beasts.
- **👤 Character Selection**: Unlock and play as different characters, each with their own starting stats and unique weapons.
- **🗺️ Infinite Map**: Explore a seamless, procedurally generated-like infinite environment.

---

## 🛠️ Tech Stack & Architecture

- **Engine**: Unity 2021.3 (LTS)
- **Language**: C#
- **UI Framework**: Unity UI + TextMeshPro
- **Design Patterns**:
  - **Scriptable Objects**: Used for data-driven design (Weapon stats, Character data, Enemy profiles).
  - **Singleton Pattern**: For central managers (GameManager, AudioManager).
  - **Observer Pattern**: For decoupling UI and game logic (Health & EXP events).

---

## 📁 Project Structure

```text
Assets/
├── Art/                # Sprites, Animations, and Visual Assets
├── Data/               # ScriptableObject Data (Characters, Weapons, Enemies)
├── Prefabs/            # Reusable Game Objects
├── Scenes/             # Main Menu and Gameplay Scenes
└── Scripts/            # Core Game Logic
    ├── Player/         # Movement, Stats, and Inventory
    ├── Enemy/          # AI and Stat Management
    ├── Weapon/         # Combat Logic and Evolutions
    ├── UI/             # HUD and Menu Systems
    └── Manager/        # High-level Game Flow
```

---

## 🚀 Getting Started

1.  **Clone the repository**:
    ```bash
    git clone https://github.com/your-username/Vampire-Survivors-Rogue-like2D.git
    ```
2.  **Open in Unity**: Launch Unity Hub and add the project folder.
3.  **Required Packages**: Ensure `TextMesh Pro` is imported (Unity will prompt you if it's missing).
4.  **Play the Game**: Open `Assets/Scenes/Main Menu.unity` and press **Play**.

---

## ⌨️ Controls

| Action | Control |
| :--- | :--- |
| **Movement** | `WASD` or `Arrow Keys` |
| **Confirm / Select** | `Mouse Click` or `Enter` |
| **Pause** | `ESC` |

---

## 📜 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🤝 Contributing

Contributions are welcome! If you have suggestions or find bugs, feel free to open an issue or submit a pull request.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

<p align="center">Made with ❤️ for Game Development</p>