[![Hyprnord](Assets/Hyprnord/HyprnordLogo.png "Hyprnord")](https://www.youtube.com/watch?v=mIKWoNQUwN4)

## [Nordic](https://www.nordtheme.com/) [Hyprland](https://hyprland.org/) Rice

## Setup

### Cloning

Simply run this:

```sh
git clone --recurse-submodules https://github.com/nnra6864/Ricerland.git ~
```

Cloning without `--recurse` will result in missing submodules such as nvim, shaders, ricer stuff etc.

### Updating

To pull all the latest changes, including submodules, run:
```sh
git pull --recurse
```

## Fonts

- [Maple Mono Normal NFP](https://github.com/subframe7536/maple-font)
- [Cascadia Code NF](https://github.com/microsoft/cascadia-code)
- [Hurmit NF](https://github.com/ryanoasis/nerd-fonts/releases/download/v3.4.0/Hermit.zip)

## Apps

- [Ricer](https://github.com/nnra6864/Ricer) - Rice with ease
- [Hyprmouse](https://github.com/nnra6864/Hyprmouse) - Control the mouse with your keyboard using Vim Motions `SUPER+M`
- [hyprshot](https://github.com/Gustash/Hyprshot) - Excellent screenshot utility `ALT+X` *region* `main_mod+ATL+X` *active window* `main_mod+CTRL+X` *window* `Print` *fullscreen*
- [fish](https://fishshell.com/) - Best shell

## Automatic Gain Control Blocking

I intentionally blocked almost all the apps from changing input volumes, it always drove me crazy.
If you find that your app of choice can't change the input volume, do the following.
1. Get your app's binary name
   ```sh
   pactl list clients | grep binary
   ```
2. Add it into the app list in `~/.config/pipewire/pipewire-pulse.conf.d/99-lock-mic-volume.conf`
   ```sh
   $EDITOR ~/.config/pipewire/pipewire-pulse.conf.d/99-lock-mic-volume.conf
   ```
3. Restart services
   ```sh
   systemctl --user restart pipewire pipewire-pulse
   ```

# ☦

```
   Ὤ
 Ὁ   Ν
Ι̅Ϲ̅ │ Χ̅Ϲ̅
───┼───
ΝΙ │ ΚΑ
   ☦
```

Εἰς δόξαν τοῦ Θεοῦ<br>
*To the glory of God*

Τῇ Ὑπεραγίᾳ Θεοτόκῳ δόξα<br>
*Glory to the Most Holy Theotokos*

Δόξα τῷ Θεῷ πάντων ἕνεκεν<br>
*Glory to God for all things*

ΑΜΗΝ

☦
