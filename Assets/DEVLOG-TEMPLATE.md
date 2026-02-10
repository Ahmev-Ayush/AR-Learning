# 📓 Development Log

## 🗓️ 2026-02-09 — Project Setup & First Steps

**Time Spent:** ~2 hours
**What I Did:**
- Created new Unity project with AR Foundation template
- Set up Android build settings
- Added AR Session and AR Session Origin to scene
- Tested basic camera feed on phone — ✅ working

**What I Learned:**
- AR Session Origin is the parent of the AR Camera
- Need to enable "ARCore Supported" in Player Settings

**Problems I Faced:**
- Build failed because Minimum API level was set to 24, changed to 28
- Camera permission wasn't asked — fixed by adding `uses-permission` in manifest

**AI Help Used:**
- Asked Copilot how to fix the API level error → [Chat Link](link-here)

**Screenshots:**
![First AR Camera Feed](Docs/screenshots/day1-camera.png)

---

## 🗓️ 2026-02-10 — Plane Detection

**Time Spent:** ~3 hours
**What I Did:**
- Added AR Plane Manager component
- Created a custom plane visualization prefab
- Planes now detected on horizontal surfaces

**What I Learned:**
- `ARPlaneManager` auto-detects planes and instantiates prefabs
- Can filter by `PlaneDetectionMode.Horizontal` or `.Vertical`

**Problems I Faced:**
- Planes were flickering — solved by adjusting the material's render queue

**AI Help Used:**
- Asked Gemini about plane flickering issue → [Chat Link](link-here)

**Screenshots:**
![Plane Detection Working](Docs/screenshots/day2-planes.gif)

---

<!-- Keep adding entries as you work -->