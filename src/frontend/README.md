# Frontend

```text
src/frontend/
  app/
  components/
  features/
  hooks/
  styles/
  assets/
```

Responsibilities:

- `app`: bootstrap, routing, shell layout, top-level providers
- `components`: shared UI building blocks
- `features`: screen-level workflows such as model browser, editor, diff, and inspector
- `hooks`: shared React hooks
- `styles`: tokens, theme, and global styles
- `assets`: static images and icons

## Commands

Install dependencies:

```bash
npm install
```

Start the development server:

```bash
npm run dev
```

Run tests:

```bash
npm test
```

Run type checking:

```bash
npm run typecheck
```

Build production assets:

```bash
npm run build
```

Preview the production build:

```bash
npm run preview
```

The dev server runs on Vite's default `http://localhost:5173` unless Vite selects another port.
API calls under `/api` are proxied to the backend at `http://localhost:5087`.
