# NGSPacker

This program opens and creates ICE format files that can be read by PSO2NGS clients.

It requires the .net 6.0 runtime to work. Please download from the following site.
<https://dotnet.microsoft.com/download/dotnet>

[日本語](./Readme-ja.md)

## Features

- Pack: Creates an \*.ice file of a model that can be read by PSO2 from the selected directory.
- Unpack: Extract the PSO2 game data.
- Export File List: Outputs the file list and the files contained in that file in csv format.
- Unpack by file list: The files specified in the line feed-delimited file list are output in a batch.

## Useful Link

- [PSO2 Modding Tutorial](http://www.pso-world.com/forums/showthread.php?237103-PSO2-Modding-Tutorial-2-0)
- [PSO2 File Directory](https://docs.google.com/spreadsheets/d/1GQwG49iYM1sgJhyAU5AWP-gboemzfIZjBGjTGEZSET4/edit?usp=sharing)
  - [PSO2-Modding](https://discord.com/invite/cV3QRkB) - Discord
- [Arks-Layer](https://arks-layer.com/) - Third party launcher. Required when enabling mods.
  - [Phantasy Star Fleet](https://discord.com/invite/pso2) - Discord
- [Checksum File](https://ngs.logue.dev/data/d4455ebc2bef618f29106da7692ebc1a) - Required when activating mods. Replace the file in `pso2_bin/data/win32`. (by Aida Enna of [ArksLayer](https://arks-layer.com/))
- [PSO2 Modding Database](https://pso2mod.com/)
- [PSO2 Mod Manager](https://github.com/PolCPP/PSO2-Mod-Manager)
- [PSO2NGS @ Nexus Mods](https://www.nexusmods.com/phantasystaronline2newgenesis)

## Disclaimer

The act of modifying game data (modding) does not infringe on the right to maintain integrity, and is within the scope of
the freedom of expression stipulated in Article 21 of the Japanese Constitution and the Fair Use law stipulated in Article 107 of the U.S. Copyright Act.

However, please keep in mind that modding is potentially damaging to the game environment and may result in a ban.
In particular, the dissemination of private internal data in a community that can be viewed by an unspecified number of people and the diversion to other works
(such as converting model data and diverting it) are acts that clearly deviate from the above law, and compensation for damages may result in litigation. [^1]

The authors are not responsible for any damage caused by that.
Please use it with common sense, and USE AT YOUR OWN RISK.

[^1]: For details, please see [Regarding Copyright Infringement and Illicit Activities](https://pso2.com/players/news/780/) on the official website.

## License

©2021-2023 by Logue. Licensed under the [MIT License](./LICENSE).

This progrem uses [Shadowth117](https://github.com/Shadowth117)'s [ZamboniLib](https://github.com/Shadowth117/ZamboniLib) and [PizzaCrust](https://github.com/PizzaCrust)'s [ooz-sys](https://github.com/PizzaCrust/ooz-sys).

All rights to the copyrighted works (images, data, audios, texts, etc.) used in "PSO2: NGS" are owned by SEGA Corporation or its licensors.
