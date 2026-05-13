import { fireEvent, render, screen, within } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import { App } from '../src/App';
import branchDiffFixture from '../../../fixtures/branch-divergence/expected/diff.json';
import multiContextFixture from '../../../fixtures/branch-divergence/expected/multi-context-view.json';

function getPane(title: string) {
  const pane = screen.getAllByRole('article').find((article) => article.getAttribute('aria-label') === title);
  if (!pane) {
    throw new Error(`Missing pane: ${title}`);
  }

  return pane as HTMLElement;
}

describe('phase 3 git-native workflow', () => {
  it('defines the canonical branch comparison fixtures', () => {
    expect(branchDiffFixture.baseWorkspaceId).toBe('workspace-branch-divergence-base');
    expect(branchDiffFixture.headWorkspaceId).toBe('workspace-branch-divergence-head');
    expect(branchDiffFixture.added[0].name).toBe('ThermalMonitor');
    expect(branchDiffFixture.changedFiles[0]).toEqual({ path: 'model/root.sysml', status: 'Modified' });
    expect(branchDiffFixture.traceLinks[0].relationship).toBe('AddsModelItem');

    expect(multiContextFixture.viewId).toBe('view-branch-divergence-base-head');
    expect(multiContextFixture.contexts.map((context) => context.workspaceId)).toEqual([
      'workspace-branch-divergence-base',
      'workspace-branch-divergence-head',
    ]);
    expect(multiContextFixture.projections[1].nodeIds).toContain('66666666-6666-4666-8666-666666666666');
    expect(multiContextFixture.crossContextTraceLinks[0].stableId).toBe('66666666-bbbb-4bbb-8bbb-666666666666');
  });

  it('switches branch context and shows scoped multi-context comparison identity', () => {
    render(<App />);

    const switcher = screen.getByLabelText('Branch switcher');
    expect(within(switcher).getByRole('button', { name: 'head' })).toHaveAttribute('aria-pressed', 'true');
    expect(screen.getByLabelText('Workspace status')).toHaveTextContent('Active branch: head');

    fireEvent.click(within(switcher).getByRole('button', { name: 'base' }));
    expect(within(switcher).getByRole('button', { name: 'base' })).toHaveAttribute('aria-pressed', 'true');
    expect(screen.getByLabelText('Workspace status')).toHaveTextContent('Active branch: base');

    const comparisonPane = getPane('Branch Comparison');
    expect(within(comparisonPane).getByLabelText('Multi-context comparison projection')).toHaveTextContent('workspace-branch-divergence-base');
    expect(within(comparisonPane).getByLabelText('Multi-context comparison projection')).toHaveTextContent('workspace-branch-divergence-head');
    expect(comparisonPane).toHaveTextContent('MultiContextViewDto view-branch-divergence-base-head');
    expect(comparisonPane).toHaveTextContent('AddsModelItem');
  });

  it('renders visual diff overlays, text diff, split comparison, commit preview, and conflict assistance', () => {
    render(<App />);

    const diffPane = getPane('Diff');
    fireEvent.click(within(diffPane).getByRole('button', { name: 'Visual' }));
    expect(within(diffPane).getByLabelText('Branch comparison summary')).toHaveTextContent('model/root.sysml');
    expect(within(diffPane).getByLabelText('Branch comparison summary')).toHaveTextContent('ThermalMonitor');
    expect(within(diffPane).getByLabelText('Branch comparison summary')).toHaveTextContent('Branch-to-branch trace');
    expect(within(diffPane).getByLabelText('Branch comparison summary')).toHaveTextContent('Local changes overlay');
    expect(within(diffPane).getByLabelText('Branch comparison summary')).toHaveTextContent('Merge conflict assistance');

    fireEvent.click(within(diffPane).getByRole('button', { name: 'Text' }));
    expect(within(diffPane).getByRole('textbox', { name: 'BatterySystem semantic branch diff' })).toHaveTextContent(
      'workspace-branch-divergence-base:base workspace-branch-divergence-head:head',
    );
    expect(within(diffPane).getByText(/trace link: 66666666-bbbb-4bbb-8bbb-666666666666 AddsModelItem/)).toBeInTheDocument();

    fireEvent.click(within(diffPane).getByRole('button', { name: 'Split' }));
    expect(within(diffPane).getByRole('textbox', { name: 'workspace-branch-divergence-base / base' })).toHaveTextContent('part def BatteryPack;');
    expect(within(diffPane).getByRole('textbox', { name: 'workspace-branch-divergence-head / head / active head' })).toHaveTextContent('part def ThermalMonitor;');

    const inspector = screen.getByLabelText('Inspector panel');
    expect(within(inspector).getByLabelText('Commit panel')).toHaveTextContent('Add ThermalMonitor part definition to Vehicle architecture');
    expect(within(inspector).getByLabelText('Commit panel')).toHaveTextContent('No blocking conflict detected');
    fireEvent.click(within(inspector).getByRole('button', { name: 'Create Commit' }));
    expect(within(inspector).getByLabelText('Commit panel')).toHaveTextContent('Commit prepared: Add ThermalMonitor part definition to Vehicle architecture');
  });
});
