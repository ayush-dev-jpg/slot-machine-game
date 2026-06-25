# 🎰 Slot Machine Game

A classic slot machine game built with Unity 6.

## 🎮 Game Overview

A fully functional slot machine game featuring:
- 3 spinning reels with 4 unique symbols (Seven 7, Cherry, Bell, BAR)
- Animated lever that triggers spins
- Win detection with payout multipliers
- Credit/balance system with betting
- Win popup celebrations
- Smooth reel spinning animations

## 🏆 Payout Table

| Combination | Payout |
|---|---|
| 🎰 Three 7s | 10x bet (JACKPOT!) |
| BAR BAR BAR | 5x bet |
| Bell Bell Bell | 4x bet |
| Cherry Cherry Cherry | 3x bet |
| Any two matching | 2x bet |
| Any Cherry | 1x bet (bet returned) |

## 🕹️ How to Play

1. Click **SPIN** button or pull the **lever** on the right
2. Watch all 3 reels spin simultaneously
3. Match symbols across the middle row to win
4. Your credits update automatically

## 🌐 How to Run WebGL Build

1. Go to the `/Build/WebGL` folder in this repository
2. Open `index.html` in a modern web browser (Chrome recommended)
3. Or host on a local server using VS Code Live Server extension

## ✨ Bonus Features

- Animated lever with pull animation (switches sprite on click)
- Weighted symbol probability (7 is rare, Cherry is common)
- Win popup with celebration message
- All 4 symbols cycle through each reel strip
- Smooth simultaneous reel spinning with staggered stops

## 💭 Thought Process & Approach

1. **Setup**: Created Canvas with background, slot frame, and reel container
2. **Reels**: Built reels programmatically via script — each reel has a mask window and a scrolling strip of symbols
3. **Spinning**: Used coroutines to animate strips scrolling downward, then snap to the correct symbol
4. **Win Detection**: Checks middle row symbols across all 3 reels and calculates payout
5. **UI**: TextMeshPro for credits, bet, and result display with gold styling
6. **Lever**: Sprite swap animation on click, connected to spin function
7. **Popup**: Win celebration panel using the provided popup sprite

## 📁 Project Structure

```
Assets/
├── Scripts/        # SlotMachineController.cs
├── Sprites/        # All game sprites (symbols, machine, lever)
├── Prefabs/        # (reserved for future prefabs)
├── Animations/     # (reserved for animations)
├── Sounds/         # (reserved for sounds)
└── UI/             # UI assets
```

## 🛠️ Built With

- Unity 6 (6000.3.15f1)
- C# scripting
- Unity UI (Canvas, Image, TextMeshPro)
- Unity Mask component for reel windows
