const API_BASE_URL = 'https://jsonplaceholder.typicode.com';

export const apiClient = {
  get: async (endpoint,signal) => {
    const response = await fetch(`${API_BASE_URL}${endpoint}`,{
    method: 'GET',
    headers: {
      'Content-Type': 'application/json'
    },
    signal: signal
    });
    if (!response.ok) {
      throw new Error(`Error fetching ${endpoint}: ${response.statusText}`);
    }
    return response.json();
  },

  post: async (endpoint, data, signal) => {
    const response = await fetch(`${API_BASE_URL}${endpoint}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(data),
      signal: signal
    });
    if (!response.ok) {
      throw new Error(`Error posting to ${endpoint}: ${response.statusText}`);
    }
    return response.json();
  },

  put: async (endpoint, data, signal) => {
    const response = await fetch(`${API_BASE_URL}${endpoint}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(data),
      signal: signal
    });
    if (!response.ok) {
      throw new Error(`Error putting to ${endpoint}: ${response.statusText}`);
    }
    return response.json();
  },

  delete: async (endpoint, signal) => {
    const response = await fetch(`${API_BASE_URL}${endpoint}`, {
      method: 'DELETE',
      signal: signal
    });
    if (!response.ok) {
      throw new Error(`Error deleting ${endpoint}: ${response.statusText}`);
    }
    return response.json();
  },
};