const CACHE_NAME = 'hudur-attendancepro-v1.0.0';
const STATIC_CACHE_NAME = 'hudur-static-v1.0.0';
const DYNAMIC_CACHE_NAME = 'hudur-dynamic-v1.0.0';

const STATIC_ASSETS = [
  '/',
  '/static/js/bundle.js',
  '/static/css/main.css',
  '/manifest.json',
  '/favicon.ico'
];

const API_CACHE_PATTERNS = [
  /\/api\/users/,
  /\/api\/settings/,
  /\/api\/analytics/
];

const CACHE_STRATEGIES = {
  CACHE_FIRST: 'cache-first',
  NETWORK_FIRST: 'network-first',
  STALE_WHILE_REVALIDATE: 'stale-while-revalidate',
  NETWORK_ONLY: 'network-only',
  CACHE_ONLY: 'cache-only'
};

self.addEventListener('install', (event) => {
  console.log('Service Worker: Installing...');
  
  event.waitUntil(
    caches.open(STATIC_CACHE_NAME)
      .then((cache) => {
        console.log('Service Worker: Caching static assets');
        return cache.addAll(STATIC_ASSETS);
      })
      .then(() => {
        console.log('Service Worker: Static assets cached');
        return self.skipWaiting();
      })
      .catch((error) => {
        console.error('Service Worker: Error caching static assets', error);
      })
  );
});

self.addEventListener('activate', (event) => {
  console.log('Service Worker: Activating...');
  
  event.waitUntil(
    caches.keys()
      .then((cacheNames) => {
        return Promise.all(
          cacheNames.map((cacheName) => {
            if (cacheName !== STATIC_CACHE_NAME && 
                cacheName !== DYNAMIC_CACHE_NAME &&
                cacheName !== CACHE_NAME) {
              console.log('Service Worker: Deleting old cache', cacheName);
              return caches.delete(cacheName);
            }
          })
        );
      })
      .then(() => {
        console.log('Service Worker: Activated');
        return self.clients.claim();
      })
  );
});

self.addEventListener('fetch', (event) => {
  const { request } = event;
  const url = new URL(request.url);
  
  if (request.method !== 'GET') {
    return;
  }

  if (url.origin === location.origin) {
    if (isStaticAsset(request.url)) {
      event.respondWith(handleStaticAsset(request));
    } else if (isAPIRequest(request.url)) {
      event.respondWith(handleAPIRequest(request));
    } else {
      event.respondWith(handleNavigationRequest(request));
    }
  }
});

function isStaticAsset(url) {
  return url.includes('/static/') || 
         url.includes('/assets/') ||
         url.endsWith('.js') ||
         url.endsWith('.css') ||
         url.endsWith('.png') ||
         url.endsWith('.jpg') ||
         url.endsWith('.jpeg') ||
         url.endsWith('.svg') ||
         url.endsWith('.ico');
}

function isAPIRequest(url) {
  return url.includes('/api/') || url.includes('/graphql');
}

function isCacheableAPI(url) {
  return API_CACHE_PATTERNS.some(pattern => pattern.test(url));
}

async function handleStaticAsset(request) {
  try {
    const cache = await caches.open(STATIC_CACHE_NAME);
    const cachedResponse = await cache.match(request);
    
    if (cachedResponse) {
      console.log('Service Worker: Serving static asset from cache', request.url);
      return cachedResponse;
    }
    
    const networkResponse = await fetch(request);
    
    if (networkResponse.ok) {
      console.log('Service Worker: Caching static asset', request.url);
      cache.put(request, networkResponse.clone());
    }
    
    return networkResponse;
  } catch (error) {
    console.error('Service Worker: Error handling static asset', error);
    return new Response('Asset not available offline', { status: 503 });
  }
}

async function handleAPIRequest(request) {
  const url = request.url;
  
  if (!isCacheableAPI(url)) {
    return fetch(request);
  }
  
  try {
    const cache = await caches.open(DYNAMIC_CACHE_NAME);
    
    if (request.method === 'GET') {
      const networkResponse = await fetch(request);
      
      if (networkResponse.ok) {
        console.log('Service Worker: Caching API response', url);
        cache.put(request, networkResponse.clone());
        
        setTimeout(() => {
          cache.delete(request);
        }, 5 * 60 * 1000);
      }
      
      return networkResponse;
    }
    
    return fetch(request);
  } catch (error) {
    console.error('Service Worker: Network error, trying cache', error);
    
    const cache = await caches.open(DYNAMIC_CACHE_NAME);
    const cachedResponse = await cache.match(request);
    
    if (cachedResponse) {
      console.log('Service Worker: Serving API response from cache', url);
      return cachedResponse;
    }
    
    return new Response(
      JSON.stringify({ 
        error: 'Network unavailable', 
        message: 'This data is not available offline' 
      }),
      { 
        status: 503,
        headers: { 'Content-Type': 'application/json' }
      }
    );
  }
}

async function handleNavigationRequest(request) {
  try {
    const networkResponse = await fetch(request);
    
    if (networkResponse.ok) {
      const cache = await caches.open(DYNAMIC_CACHE_NAME);
      cache.put(request, networkResponse.clone());
    }
    
    return networkResponse;
  } catch (error) {
    console.error('Service Worker: Navigation request failed', error);
    
    const cache = await caches.open(DYNAMIC_CACHE_NAME);
    const cachedResponse = await cache.match(request);
    
    if (cachedResponse) {
      return cachedResponse;
    }
    
    const fallbackCache = await caches.open(STATIC_CACHE_NAME);
    const fallbackResponse = await fallbackCache.match('/');
    
    return fallbackResponse || new Response('Page not available offline', { 
      status: 503,
      headers: { 'Content-Type': 'text/html' }
    });
  }
}

self.addEventListener('message', (event) => {
  const { type, payload } = event.data;
  
  switch (type) {
    case 'SKIP_WAITING':
      self.skipWaiting();
      break;
      
    case 'CLEAR_CACHE':
      clearAllCaches().then(() => {
        event.ports[0].postMessage({ success: true });
      });
      break;
      
    case 'GET_CACHE_SIZE':
      getCacheSize().then((size) => {
        event.ports[0].postMessage({ size });
      });
      break;
      
    case 'PREFETCH_RESOURCES':
      prefetchResources(payload.urls).then(() => {
        event.ports[0].postMessage({ success: true });
      });
      break;
  }
});

async function clearAllCaches() {
  const cacheNames = await caches.keys();
  return Promise.all(
    cacheNames.map(cacheName => caches.delete(cacheName))
  );
}

async function getCacheSize() {
  const cacheNames = await caches.keys();
  let totalSize = 0;
  
  for (const cacheName of cacheNames) {
    const cache = await caches.open(cacheName);
    const requests = await cache.keys();
    
    for (const request of requests) {
      const response = await cache.match(request);
      if (response) {
        const blob = await response.blob();
        totalSize += blob.size;
      }
    }
  }
  
  return totalSize;
}

async function prefetchResources(urls) {
  const cache = await caches.open(DYNAMIC_CACHE_NAME);
  
  return Promise.all(
    urls.map(async (url) => {
      try {
        const response = await fetch(url);
        if (response.ok) {
          await cache.put(url, response);
          console.log('Service Worker: Prefetched', url);
        }
      } catch (error) {
        console.error('Service Worker: Failed to prefetch', url, error);
      }
    })
  );
}

self.addEventListener('sync', (event) => {
  if (event.tag === 'background-sync') {
    event.waitUntil(handleBackgroundSync());
  }
});

async function handleBackgroundSync() {
  console.log('Service Worker: Handling background sync');
  
  try {
    const pendingRequests = await getPendingRequests();
    
    for (const request of pendingRequests) {
      try {
        await fetch(request.url, request.options);
        await removePendingRequest(request.id);
        console.log('Service Worker: Synced request', request.url);
      } catch (error) {
        console.error('Service Worker: Failed to sync request', request.url, error);
      }
    }
  } catch (error) {
    console.error('Service Worker: Background sync failed', error);
  }
}

async function getPendingRequests() {
  return [];
}

async function removePendingRequest(id) {
  return true;
}

self.addEventListener('push', (event) => {
  const options = {
    body: event.data ? event.data.text() : 'New notification',
    icon: '/favicon.ico',
    badge: '/favicon.ico',
    vibrate: [200, 100, 200],
    data: {
      dateOfArrival: Date.now(),
      primaryKey: 1
    },
    actions: [
      {
        action: 'explore',
        title: 'View Details',
        icon: '/icons/checkmark.png'
      },
      {
        action: 'close',
        title: 'Close',
        icon: '/icons/xmark.png'
      }
    ]
  };
  
  event.waitUntil(
    self.registration.showNotification('Hudur AttendancePro', options)
  );
});

self.addEventListener('notificationclick', (event) => {
  event.notification.close();
  
  if (event.action === 'explore') {
    event.waitUntil(
      clients.openWindow('/')
    );
  }
});
