import { fireEvent, render, screen, within } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import { App } from '../src/App';
import editingFixture from '../../../fixtures/phase-2-editing/expected/edit-session.json';

function getPane(title: string) {
  const pane = screen.getAllByRole('article').find((article) => article.getAttribute('aria-label') === title);
  if (!pane) {
    throw new Error(`Missing pane: ${title}`);
  }

  return pane as HTMLElement;
}

describe('phase 2 visual editing', () => {
  it('defines the canonical editing fixture snapshot', () => {
    expect(editingFixture.context.isWritable).toBe(true);
    expect(editingFixture.selection.label).toBe('BatterySystem');
    expect(editingFixture.editing.selectedTool).toBe('Part');
    expect(editingFixture.editing.draftElement.name).toBe('thermalBarrier');
    expect(editingFixture.editing.snapshots.afterAdd).toContain('  part thermalBarrier: ThermalBarrier;');
    expect(editingFixture.editing.snapshots.afterRename).toContain('  part coolingBarrier: ThermalBarrier;');
    expect(editingFixture.editing.snapshots.afterUndo).not.toContain('  part coolingBarrier: ThermalBarrier;');
  });

  it('creates, renames, and reverts a draft part in the split architecture editor', () => {
    render(<App />);

    const architecturePane = getPane('System Architecture');
    fireEvent.click(within(architecturePane).getByRole('button', { name: 'Split' }));

    expect(within(architecturePane).getByText('No draft elements yet.')).toBeInTheDocument();

    const portChip = screen.getByRole('button', { name: 'Port' });
    fireEvent.click(portChip);
    expect(portChip).toHaveAttribute('aria-pressed', 'true');

    const partChip = screen.getByRole('button', { name: 'Part' });
    fireEvent.click(partChip);
    expect(partChip).toHaveAttribute('aria-pressed', 'true');

    const addPartButton = within(architecturePane).getByRole('button', { name: 'Add Part' });
    fireEvent.click(addPartButton);

    const draftName = within(architecturePane).getByLabelText('Draft name') as HTMLInputElement;
    expect(draftName).toHaveValue('thermalBarrier');

    const preview = within(architecturePane).getByRole('textbox', { name: 'Generated SysML preview' });
    expect(preview).toHaveTextContent('part thermalBarrier: ThermalBarrier;');

    fireEvent.change(draftName, { target: { value: 'coolingBarrier' } });
    expect(draftName).toHaveValue('coolingBarrier');
    expect(preview).toHaveTextContent('part coolingBarrier: ThermalBarrier;');

    fireEvent.click(within(architecturePane).getByRole('button', { name: 'Undo' }));
    expect(draftName).toHaveValue('thermalBarrier');
    expect(preview).toHaveTextContent('part thermalBarrier: ThermalBarrier;');
    expect(preview).not.toHaveTextContent('part coolingBarrier: ThermalBarrier;');

    fireEvent.click(within(architecturePane).getByRole('button', { name: 'Undo' }));
    expect(within(architecturePane).getByText('No draft elements yet.')).toBeInTheDocument();
    expect(within(architecturePane).getByLabelText('Draft name')).toBeDisabled();
    expect(preview).not.toHaveTextContent('part thermalBarrier: ThermalBarrier;');
    expect(preview).toHaveTextContent('part def BatterySystem {');

    fireEvent.click(within(architecturePane).getByRole('button', { name: 'Redo' }));
    expect(draftName).toHaveValue('thermalBarrier');
    expect(preview).toHaveTextContent('part thermalBarrier: ThermalBarrier;');

    fireEvent.click(within(architecturePane).getByRole('button', { name: 'Redo' }));
    expect(draftName).toHaveValue('coolingBarrier');
    expect(preview).toHaveTextContent('part coolingBarrier: ThermalBarrier;');
  });
});
