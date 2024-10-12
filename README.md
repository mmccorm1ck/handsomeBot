## handsomeBot

## This project is a work in progress

A program for determining "optimal" plays in a pokemon VGC format game.

The program loads a team from a pokepaste, then the user enters the opponent's team of six.

The program learns the opponent's team as the game goes on, using https://github.com/smogon/damage-calc and inverting to determine EVs based on turn order and damage dealt.

UI is built using the Avalonia framework for linux compatibility - https://avaloniaui.net/

Dependencies:
- [DialogHost](https://github.com/AvaloniaUtils/DialogHost.Avalonia/tree/main)
- [ClearScript](https://github.com/microsoft/ClearScript)