const { env } = require('process');

//const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
//  env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'https://localhost:7211';

const BFF_PORT = 5209; // Port HTTP de votre BFF
const BFF_HTTPS_PORT = 7211; // Port HTTPS de votre BFF

const target = `http://localhost:${BFF_PORT}`;

console.log('Configuration proxy activée:');
console.log(`Target: ${target}`);
console.log('Routes: /api/auth, /api/users');

const PROXY_CONFIG = [
  {
    context: [
      "/api/auth",
      "/api/users",
    ],
    target: target,
    secure: false,  //Cette option permet d'utiliser des certificats auto-signés pendant le développement, mais en production vous devriez utiliser des certificats valides
    logLevel: "debug",
    changeOrigin: true, // Important pour les requêtes cross-origin
    followRedirects: true, // Suit automatiquement les redirections
    onProxyReq: (proxyReq, req, res) => {
      console.log(`[Proxy] Requête: ${req.method} ${req.url} -> ${target}${req.url}`);
    },
    onProxyRes: (proxyRes, req, res) => {
      console.log(`[Proxy] Réponse: ${proxyRes.statusCode} pour ${req.url}`);
    },
    onError: (err, req, res) => {
      console.error(`[Proxy] Erreur: ${err.message}`);
      console.error(`[Proxy] Requête URL: ${req.url}`);
      console.error(`[Proxy] Target: ${target}`);
    }
  }
]

console.log('Proxy configuré vers:', target);
module.exports = PROXY_CONFIG;
