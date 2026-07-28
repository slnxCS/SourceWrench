# SourceWrench

> A modern cross-platform CLI and .NET library for extracting and converting Source Engine assets.

SourceWrench is an open-source toolkit for working with Source Engine resources. The project aims to replace legacy extraction utilities with a modern, extensible and scriptable solution built on .NET.

The project consists of two parts:

- **SourceWrench CLI** — command-line interface.
- **SourceWrench.CoreLib** — reusable .NET library for reading Source Engine formats.

> **Status:** Early development. APIs and CLI commands are subject to change.

---

## Features

### Implemented

- ✅ VPK extraction

### Planned

- 📦 VPK archive support
- 🗺 BSP map parsing
- 🧱 VMF parsing
- 🎮 MDL model parsing
- 📐 VVD vertex data parsing
- 🔺 VTX index data parsing
- 🖼 VTF texture parsing

---

## Export formats (planned)

SourceWrench is designed to export Source Engine assets into modern and widely supported formats.

### Models

- glTF 2.0 *(primary target)*
- OBJ
- FBX *(planned)*

### Textures

- PNG
- TGA

### Audio

- WAV

### Maps

- glTF
- JSON scene description *(planned)*

---

## Why glTF?

glTF is intended to become the primary export format because it is:

- Open standard
- Compact
- Supports skeletal animations
- Supports PBR materials
- Supported by Unity, Godot, Unreal Engine and Blender
- Easy to process programmatically

---

## Usage

Extract a VPK archive:

```bash
SourceWrench extract \
    --source "pak01_dir.vpk" \
    --output "./ExtractedFiles"
```

Display help:

```bash
SourceWrench --help
```

---

## Roadmap

- [x] VPK extraction
- [ ] VPK browser
- [ ] BSP parser
- [ ] MDL parser
- [ ] VVD parser
- [ ] VTX parser
- [ ] VTF parser
- [ ] VMF parser
- [ ] glTF exporter
- [ ] PNG exporter
- [ ] Interactive CLI
- [ ] Search & indexing
- [ ] NuGet package

---

## Building

Requirements:

- .NET 8 SDK

Clone the repository:

```bash
git clone https://github.com/<username>/SourceWrench.git
cd SourceWrench
```

Build:

```bash
dotnet build
```

Run:

```bash
dotnet run -- --help
```

---

## Project goals

- Modern architecture
- Cross-platform
- No external extraction tools
- Scriptable command-line interface
- Reusable .NET library
- Extensible design
- Support for all Source Engine games

---

## License

This project is licensed under the **GNU General Public License v3.0**.

See the [LICENSE](LICENSE) file for details.
