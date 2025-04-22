// test à lancer via la commande suivante -> node test-bff-connectivity.js

const http = require('http');
const https = require('https');

// Ports à tester - ajustez-les en fonction de votre configuration
const ports = [
  { port: 5209, protocol: 'http', description: 'BFF HTTP' },
  { port: 7211, protocol: 'https', description: 'BFF HTTPS' },
];

// Fonction pour tester un port
function testPort(port, protocol, description) {
  return new Promise((resolve, reject) => {
    const client = protocol === 'https' ? https : http;
    
    const options = {
      hostname: 'localhost',
      port: port,
      path: '/cors-test', // Utilisez l'endpoint que nous avons créé précédemment
      method: 'GET',
      timeout: 5000,
      rejectUnauthorized: false // Permet les certificats auto-signés
    };

    console.log(`Test de ${description} sur ${protocol}://localhost:${port}...`);

    const req = client.request(options, (res) => {
      let data = '';
      res.on('data', (chunk) => {
        data += chunk;
      });
      res.on('end', () => {
        if (res.statusCode >= 200 && res.statusCode < 300) {
          console.log(`✅ ${description} est accessible! (${res.statusCode})`);
          try {
            const jsonResponse = JSON.parse(data);
            console.log(`Réponse: ${JSON.stringify(jsonResponse, null, 2)}`);
          } catch(e) {
            console.log(`Réponse: ${data.substring(0, 100)}...`);
          }
          resolve(true);
        } else {
          console.log(`❌ ${description} a retourné ${res.statusCode}: ${data}`);
          resolve(false);
        }
      });
    });

    req.on('error', (e) => {
      console.log(`❌ ${description} n'est pas accessible: ${e.message}`);
      resolve(false);
    });

    req.on('timeout', () => {
      console.log(`❌ ${description} a expiré (timeout)`);
      req.destroy();
      resolve(false);
    });

    req.end();
  });
}

// Fonction principale pour tester tous les ports
async function testConnectivity() {
  console.log('Test de connectivité aux services...');
  
  for (const { port, protocol, description } of ports) {
    await testPort(port, protocol, description);
  }
  
  console.log('Tests terminés!');
}

// Exécuter les tests
testConnectivity();