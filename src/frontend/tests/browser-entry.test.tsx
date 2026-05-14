import { screen, waitFor } from '@testing-library/react';
import { describe, expect, it } from 'vitest';

describe('browser entrypoint', () => {
  it('mounts the React application into the root element', async () => {
    document.body.innerHTML = '<div id="root"></div>';

    await import('../src/main');

    await waitFor(() => {
      expect(screen.getByLabelText('Application menu')).toHaveTextContent('sysml2_editor');
      expect(screen.getByLabelText('Workbench layout')).toBeInTheDocument();
    });
  });
});
