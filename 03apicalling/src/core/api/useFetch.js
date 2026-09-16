// useFetch.js

import { useEffect, useState } from 'react';
import { apiClient } from '../utils/apiClient';

function useFetch(url) {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    const controller = new AbortController();

    async function fetchData() {
      try {
        setLoading(true);
        setError(null);

        const result = await apiClient.get(url, controller.signal);

        setData(result);
      } catch (error) {

       
        if (error.name !== 'AbortError') {
          setError(error);
        }

      } finally {
        setLoading(false);
      }
    }

    fetchData();

    return () => {
      controller.abort();
    };

  }, [url]);

  return {
    data,
    loading,
    error
  };
}

export default useFetch;