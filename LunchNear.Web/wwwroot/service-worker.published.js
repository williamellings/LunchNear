self.importScripts("./service-worker-assets.js");

const cachePrefix = "lunchnear-cache-";
const cacheName = `${cachePrefix}${self.assetsManifest.version}`;
const offlineAssets = [
    /\.dll$/,
    /\.pdb$/,
    /\.wasm$/,
    /\.html$/,
    /\.js$/,
    /\.json$/,
    /\.css$/,
    /\.woff$/,
    /\.woff2$/,
    /\.png$/,
    /\.jpg$/,
    /\.jpeg$/,
    /\.gif$/,
    /\.ico$/,
    /\.dat$/
];
const excludedAssets = [/^service-worker\.js$/];

self.addEventListener("install", event => event.waitUntil(onInstall()));
self.addEventListener("activate", event => event.waitUntil(onActivate()));
self.addEventListener("fetch", event => event.respondWith(onFetch(event)));

async function onInstall() {
    const assets = self.assetsManifest.assets
        .filter(asset => offlineAssets.some(pattern => pattern.test(asset.url)))
        .filter(asset => !excludedAssets.some(pattern => pattern.test(asset.url)))
        .map(asset => new Request(asset.url, { integrity: asset.hash, cache: "no-cache" }));

    const cache = await caches.open(cacheName);
    await cache.addAll(assets);
    self.skipWaiting();
}

async function onActivate() {
    const keys = await caches.keys();
    await Promise.all(
        keys
            .filter(key => key.startsWith(cachePrefix) && key !== cacheName)
            .map(key => caches.delete(key))
    );
    self.clients.claim();
}

async function onFetch(event) {
    if (event.request.method !== "GET") {
        return fetch(event.request);
    }

    const isNavigation = event.request.mode === "navigate";
    const request = isNavigation ? "index.html" : event.request;
    const cached = await caches.match(request);
    return cached || fetch(event.request);
}
