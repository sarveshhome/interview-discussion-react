# 03apicalling

A React + Vite application demonstrating API calling patterns. This project shows how to structure HTTP requests using a centralized API client and a custom React hook for data fetching.

## why use 

Show that you understand separation of concerns, loading/error states, cancellation, reusable hooks, HTTP errors, and clean component design.


## Features

- **Centralized API Client** (`src/core/utils/apiClient.js`): A reusable wrapper around the native `fetch` API that handles GET, POST, PUT, and DELETE requests, including error handling and abort signals.
- **Custom React Hook** (`src/core/api/useFetch.js`): A reusable hook that manages loading, error, and data states for any API call, with automatic cleanup via `AbortController`.
- **Example Feature**: A `User` component that fetches and displays a list of users from [JSONPlaceholder](https://jsonplaceholder.typicode.com/).

## Project Structure

```
03apicalling/
├── public/                  # Static assets served by Vite
├── src/
│   ├── assets/              # Images and SVGs
│   ├── core/                # Core infrastructure
│   │   ├── api/
│   │   │   └── useFetch.js  # Custom hook for data fetching
│   │   └── utils/
│   │       └── apiClient.js  # HTTP client wrapper
│   ├── features/            # Feature-specific code
│   │   └── users/
│   │       └── presentation/
│   │           └── components/
│   │               └── user.jsx  # User list component
│   ├── App.jsx              # Root component
│   ├── main.jsx             # Entry point
│   └── index.css            # Global styles
├── index.html               # HTML entry
├── vite.config.js           # Vite configuration
├── package.json             # Dependencies and scripts
└── README.md                # This file
```

## Architecture

### API Client (`apiClient.js`)

The `apiClient` is a plain JavaScript object exported as a named export. It wraps the native `fetch` API to provide:

- A consistent base URL (`https://jsonplaceholder.typicode.com`)
- JSON content-type headers
- Automatic error throwing on non-OK responses
- Support for `AbortController` signals to cancel in-flight requests

Each method (`get`, `post`, `put`, `delete`) returns a Promise that resolves to the parsed JSON response.

### Custom Hook (`useFetch.js`)

The `useFetch` hook encapsulates the common data-fetching pattern:

- `useState` for `data`, `loading`, and `error`
- `useEffect` that triggers a fetch on mount and when the `url` changes
- An `AbortController` to cancel stale requests on unmount or URL change
- Returns `{ data, loading, error }` for the consuming component

### Components

The `User` component demonstrates using `useFetch` to fetch a list of users and render them, with handling for loading and error states.

## Getting Started

### Prerequisites

- Node.js (v18 or later)
- npm or yarn

### Installation

```bash
npm install
# or
yarn install
```

### Running the Development Server

```bash
npm run dev
# or
yarn dev
```

The app will start on `http://localhost:5173` (or the next available port) with Hot Module Replacement (HMR) enabled.

### Building for Production

```bash
npm run build
# or
yarn build
```

The production build output is placed in the `dist/` directory.

### Previewing the Production Build

```bash
npm run preview
# or
yarn preview
```

## Scripts

| Script        | Description                          |
|---------------|--------------------------------------|
| `dev`         | Start the Vite development server    |
| `build`       | Build the app for production         |
| `lint`        | Run ESLint to check code style       |
| `preview`     | Serve the production build locally   |

## Dependencies

- **React** (`react`, `react-dom`) — UI library
- **Vite** — build tool and dev server
- **@vitejs/plugin-react** — Vite plugin for React

## Learn More

- [Vite](https://vite.dev/)
- [React](https://react.dev/)
- [JSONPlaceholder](https://jsonplaceholder.typicode.com/) — fake API used in this project