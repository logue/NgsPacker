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
  - [PSO2-Modding](https://discord.com/invite/cV3QRkB) - Discord ※日本語情報あり
- [Arks-Layer](https://arks-layer.com/) - Third party launcher. Required when enabling mods.
  - [Phantasy Star Fleet](https://discord.com/invite/pso2) - Discord
- [Checksum File](http://www.mediafire.com/file/85m6h56u5w3181g/checksum.zip/file) - Required when enabling mods.
- [PSO2 Modding Database](https://pso2mod.com/)
  - [PSO2 Mod Manager](https://github.com/PolCPP/PSO2-Mod-Manager) - Official PSO2 Modding Database Client, But not maintained.
- [PSO2NGS @ Nexus Mods](https://www.nexusmods.com/phantasystaronline2newgenesis)

## Disclaimer

The act of modifying game data (Modding) does not infringe the right to retain identity under the Patent Law and Copyright Law from the precedents of past trials, and is within the scope of freedom of expression.
However, keep in mind that mods are potentially damaging to the gaming environment and can be banned.
In particular, distribution or diversion of private internal data to an unspecified number of people in an open place clearly violates the rules and may be sued.

The author does not take any responsibility for any damage caused by that.
USE AT YOUR OWN RISK.

## License

[MIT](./LICENSE)

This tool uses [Shadowth117](https://github.com/Shadowth117)'s [ZamboniLib](https://github.com/Shadowth117/ZamboniLib) and [PizzaCrust](https://github.com/PizzaCrust)'s [ooz-sys](https://github.com/PizzaCrust/ooz-sys).

All rights to the copyrighted works (images, data, audios, texts, etc.) used in "PSO2: NGS" are owned by SEGA Corporation or its licensors.
