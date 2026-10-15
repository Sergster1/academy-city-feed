# Airship weather feed

Tiny GitHub Pages site that publishes `weather.json` (current New York weather, Open-Meteo, no API key)
every 30 minutes. The VRChat world's airship screen (`unity/AirshipWeather.cs`) reads it.

## One-time setup (about 3 minutes)
0. Move `weather.yml` into `.github/workflows/weather.yml` (it was saved at the top level because tools can't write into `.github`).
1. Create a **public** repo on GitHub (e.g. `academy-city-feed`) and push this folder to `main`.
2. Repo **Settings > Pages > Build and deployment > Source: GitHub Actions**.
3. **Actions** tab > "Update weather feed" > **Run workflow** (the push also triggers it).
4. Your feed URL is `https://<your-username>.github.io/<repo>/weather.json` (open it to check).

## In Unity
Put the URL into `AirshipWeather.feedUrl` on the airship screen object. `github.io` is a VRChat trusted domain.
If the feed is down the screen keeps showing the static Academy City advert.
