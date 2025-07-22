# RobotPet

A simple **companion robot demo** built with C#, RoboDK, AForge.NET, and Emgu.CV.
The program captures video from a webcam, detects the user's face and emotion,
then drives a virtual / physical robot in RoboDK to perform an appropriate
action while responding with text.

---

## Features

1. **Real-time Emotion Detection**
   * Haar cascade face detection (`haarcascade_frontalface_default.xml`).
   * FER+ ONNX emotion model (`emotion-ferplus.onnx`).
2. **Text Fallback Emotion Parsing**
   * Detects keywords like "happy" or "sad" when the camera is unavailable.
3. **Robot Actions via RoboDK API**
   * `wave`, `hug`, and `idle` joint motions.
4. **Interactive CLI Dialogue Loop**
   * Type your message – the bot replies and performs an action.

---

## Directory Structure (excerpt)

```
wind/
├── RobotPet.cs            # main application file
├── RobotPet_README.md     # <–– you are here
└── ...                    # other unrelated projects
```

---

## Prerequisites

| Component | Version / Notes |
|-----------|-----------------|
| .NET SDK  | 8.0 or later    |
| **RoboDK** | Desktop app running & robot named `CompanionRobot` |
| AForge.NET | `AForge.Video`, `AForge.Video.DirectShow` via NuGet |
| Emgu.CV    | `Emgu.CV`, `Emgu.CV.runtime.windows`, `Emgu.CV.Dnn` |
| ONNX Model | `emotion-ferplus.onnx` in executable directory |
| Haar File  | `haarcascade_frontalface_default.xml` in executable directory |

---

## Getting Started

1. **Clone / open** the solution containing `RobotPet.cs`.
2. **Install NuGet packages**:
   ```powershell
   dotnet add package AForge.Video
   dotnet add package AForge.Video.DirectShow
   dotnet add package Emgu.CV
   dotnet add package Emgu.CV.runtime.windows
   dotnet add package Emgu.CV.Dnn
   ```
3. **Download resources**
   * Place `haarcascade_frontalface_default.xml` and `emotion-ferplus.onnx`
     next to the executable (or adjust the paths in code).
4. **Ensure RoboDK is open** with a robot station containing an item called
   `CompanionRobot`.
5. **Run** the program:
   ```powershell
   dotnet run --project RobotPet.csproj
   ```
6. **Interact** – type messages, smile, frown and watch the robot respond.

---

## Extending

* Add more emotions by expanding the `emotions` array and mapping to actions.
* Replace the simple keyword parser with an LLM (e.g. OpenAI) for richer text understanding.
* Deploy on a real robot by linking RoboDK to a physical arm.

---

## License

MIT – see `LICENSE` (create one if you plan to release publicly).
