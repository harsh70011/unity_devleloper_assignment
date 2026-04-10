# Make the Unity Elevator Project Live (WebGL)

## Option A: GitHub Pages (recommended)

1. Push this repository to GitHub.
2. In **Settings → Secrets and variables → Actions**, add:
   - `UNITY_LICENSE`
   - `UNITY_EMAIL`
   - `UNITY_PASSWORD`
3. Enable **GitHub Pages** and set source to **Deploy from branch**.
4. Run the workflow **Build and Deploy Unity WebGL** from the Actions tab.
5. Your project will be live at:
   - `https://<your-username>.github.io/<your-repo>/`

## Option B: itch.io

1. In Unity, switch build target to **WebGL**.
2. Build into a folder (for example `Build/WebGL`).
3. Zip the build folder contents.
4. Upload zip to itch.io as an HTML game and publish.

## Important

- This repo currently contains gameplay scripts and deployment automation.
- You must open the project in Unity 6, create the scene/prefabs/UI, and commit them so the CI pipeline can build a playable WebGL app.
