import { render, screen, within } from '@testing-library/react';
import { readFileSync } from 'node:fs';
import { dirname, resolve } from 'node:path';
import { fileURLToPath } from 'node:url';
import { describe, expect, it } from 'vitest';
import { App } from '../src/App';
import shellFixture from '../../../fixtures/phase-0-workbench/expected/workbench-shell.json';

const paneModes = ['Visual', 'Text', 'Split'] as const;
const testDirectory = dirname(fileURLToPath(import.meta.url));

describe('phase 0 workbench shell', () => {
  it('defines the canonical shell fixture', () => {
    expect(shellFixture.branding).toEqual({
      eyebrow: 'Workbench',
      title: 'sysml2_editor',
    });
    expect(shellFixture.menus).toEqual(['File', 'Edit', 'View', 'Navigate', 'Model', 'Tools', 'Window', 'Help']);
    expect(shellFixture.openContexts.map((context) => context.label)).toEqual([
      'vehicle-platform/main',
      'vehicle-platform/concept-ev',
      'supplier-bms/dev',
    ]);
    expect(shellFixture.workspace.panes.map((pane) => pane.title)).toEqual([
      'System Architecture',
      'Branch Comparison',
      'SysML Text',
      'Diff',
    ]);
    expect(shellFixture.status.pills).toContain('Model is valid');
  });

  it('renders the phase 0 workbench shell with visible context identity', () => {
    render(<App />);

    expect(screen.getByLabelText('Application menu')).toHaveTextContent('sysml2_editor');
    expect(screen.getByLabelText('Application menu')).toHaveTextContent('Workbench');

    const mainMenu = screen.getByLabelText('Main menu');
    for (const menu of shellFixture.menus) {
      expect(within(mainMenu).getByRole('button', { name: menu })).toBeInTheDocument();
    }

    const openContexts = screen.getByLabelText('Open contexts');
    for (const context of shellFixture.openContexts) {
      expect(within(openContexts).getByText(context.label)).toBeInTheDocument();
    }
    expect(within(openContexts).getAllByText('Writable')).toHaveLength(2);
    expect(within(openContexts).getAllByText('Comparison')).toHaveLength(1);

    expect(screen.getByLabelText('Workbench layout')).toBeInTheDocument();
    expect(screen.getByRole('tree', { name: 'Model tree' })).toBeInTheDocument();
    expect(screen.getByLabelText('Model tree panel')).toBeInTheDocument();
    expect(screen.getByLabelText('Type palette panel')).toBeInTheDocument();
    expect(screen.getByLabelText('Inspector panel')).toBeInTheDocument();
    expect(screen.getByLabelText('Workspace status')).toBeInTheDocument();

    const repositoriesPanel = screen.getByLabelText('Repositories panel');
    expect(within(repositoriesPanel).getAllByRole('article')).toHaveLength(3);
    for (const repo of shellFixture.repositories) {
      expect(within(repositoriesPanel).getByText(repo.name, { selector: 'strong' })).toBeInTheDocument();
    }

    for (const pane of shellFixture.workspace.panes) {
      const article = screen.getByRole('article', { name: pane.title });
      expect(article).toHaveTextContent(pane.context);
      expect(article).toHaveTextContent(pane.summary);
      expect(article).toHaveTextContent(pane.kind);
      for (const mode of paneModes) {
        expect(within(article).getByRole('button', { name: mode })).toBeInTheDocument();
      }
    }

    const inspectorPanel = screen.getByLabelText('Inspector panel');
    expect(within(inspectorPanel).getByText(shellFixture.inspector.label, { selector: '.inspector__title' })).toBeInTheDocument();
    expect(within(inspectorPanel).getByText(shellFixture.inspector.qualifiedName, { selector: 'dd' })).toBeInTheDocument();
    expect(within(inspectorPanel).getByText(shellFixture.inspector.owner, { selector: 'dd' })).toBeInTheDocument();
    expect(within(inspectorPanel).getByText(shellFixture.inspector.file, { selector: 'dd' })).toBeInTheDocument();
    expect(within(inspectorPanel).getByText(shellFixture.inspector.summary, { selector: 'dd' })).toBeInTheDocument();

    for (const [key, value] of shellFixture.inspector.attributes) {
      expect(within(inspectorPanel).getByText(key)).toBeInTheDocument();
      expect(within(inspectorPanel).getByText(value)).toBeInTheDocument();
    }

    for (const pill of shellFixture.status.pills) {
      expect(screen.getByText(pill)).toBeInTheDocument();
    }
  });

  it('keeps responsive fallback rules for context-bearing panes', () => {
    const css = readFileSync(resolve(testDirectory, '../src/styles.css'), 'utf8');

    expect(css).toContain('@media (max-width: 1420px)');
    expect(css).toContain('@media (max-width: 1120px)');
    expect(css).toContain('.pane__context');
    expect(css).toContain('grid-template-columns: 1fr');
    expect(css).toContain('grid-template-rows: repeat(4, minmax(280px, 1fr))');
  });
});
