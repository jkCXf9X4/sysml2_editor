import { fireEvent, render, screen, within } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import { App } from '../src/App';
import browserFixture from '../../../fixtures/phase-1-browser/expected/browser-state.json';
import traceLinksFixture from '../../../fixtures/multi-file-modular/expected/trace-links.json';
import diagnosticsFixture from '../../../fixtures/invalid-input/expected/diagnostics.json';

function getPane(title: string) {
  const pane = screen.getAllByRole('article').find((article) => article.getAttribute('aria-label') === title);
  if (!pane) {
    throw new Error(`Missing pane: ${title}`);
  }

  return pane as HTMLElement;
}

describe('phase 1 read-only model browser', () => {
  it('describes the browser fixture for the read-only slice', () => {
    expect(browserFixture.context.isWritable).toBe(false);
    expect(browserFixture.selection.label).toBe('BatterySystem');
    expect(browserFixture.selection.qualifiedName).toBe('VehiclePlatform::Vehicle::Powertrain::BatterySystem');
    expect(browserFixture.tree.visibleLabels).toContain('2.1.1 Powertrain');
    expect(browserFixture.source.readOnly).toBe(true);
    expect(browserFixture.traceability.itemTraces).toContain('Satisfies: REQ-0021 Battery capacity');
    expect(traceLinksFixture.map((link) => link.kind)).toContain('ItemToFile');
    expect(diagnosticsFixture[0].severity).toBe('Error');
  });

  it('keeps tree, graph, inspector, and source panes synchronized', () => {
    render(<App />);

    const tree = screen.getAllByRole('tree', { name: 'Model tree' })[0];
    fireEvent.click(within(tree).getByRole('button', { name: 'Part Definition 2.1.1 Powertrain' }));

    const inspector = screen.getByLabelText('Inspector panel');
    expect(within(inspector).getByText('Powertrain', { selector: '.inspector__title' })).toBeInTheDocument();
    expect(within(inspector).getByText('VehiclePlatform::Vehicle::Powertrain', { selector: 'dd' })).toBeInTheDocument();
    expect(within(inspector).getByText('Vehicle', { selector: 'dd' })).toBeInTheDocument();
    expect(within(inspector).getByText('model/2_SystemArchitecture/2.1_Vehicle/2.1.1_Powertrain/Powertrain.sysml', { selector: 'dd' })).toBeInTheDocument();
    expect(within(inspector).getByText('Electric', { selector: 'dd' })).toBeInTheDocument();

    const sourcePane = getPane('SysML Text');
    expect(within(sourcePane).getByRole('textbox', { name: 'model/2_SystemArchitecture/2.1_Vehicle/2.1.1_Powertrain/Powertrain.sysml' })).toBeInTheDocument();
    expect(within(sourcePane).getByText('part Powertrain {')).toBeInTheDocument();
    expect(within(sourcePane).getByText('port out hvPos: ElectricPowerOut;')).toBeInTheDocument();

    const architecturePane = getPane('System Architecture');
    fireEvent.click(within(architecturePane).getByRole('button', { name: /BatterySystem/ }));
    expect(within(inspector).getByText('BatterySystem', { selector: '.inspector__title' })).toBeInTheDocument();
    expect(within(sourcePane).getByRole('textbox', { name: 'model/2_SystemArchitecture/2.1_Vehicle/2.1.1_Powertrain/BatterySystem.sysml' })).toBeInTheDocument();
  });

  it('filters tree rows without losing the browser context', () => {
    render(<App />);

    const tree = screen.getAllByRole('tree', { name: 'Model tree' })[0];
    fireEvent.change(screen.getAllByPlaceholderText('Search model...')[0], { target: { value: 'Battery' } });

    expect(within(tree).getByRole('button', { name: 'Part Definition BatterySystem' })).toBeInTheDocument();
    expect(within(tree).queryByRole('button', { name: '2.1.1 Powertrain' })).not.toBeInTheDocument();
    expect(within(tree).queryByRole('button', { name: 'Inverter' })).not.toBeInTheDocument();
    expect(screen.getAllByRole('contentinfo', { name: 'Workspace status' })[0]).toHaveTextContent('SysML v2');
  });

  it('surfaces source ownership and trace links in the trace view', () => {
    render(<App />);

    const sourcePane = getPane('SysML Text');
    fireEvent.click(within(sourcePane).getByRole('button', { name: 'Visual' }));

    expect(within(sourcePane).getByText('Trace links')).toBeInTheDocument();
    expect(sourcePane).toHaveTextContent('read-only browser, exact file ownership preserved');
    expect(within(sourcePane).getByText('model/2_SystemArchitecture/2.1_Vehicle/2.1.1_Powertrain/BatterySystem.sysml')).toBeInTheDocument();
    expect(within(sourcePane).getByText(/Satisfies: REQ-0021 Battery capacity/)).toBeInTheDocument();
    expect(within(sourcePane).getByText(/Allocated from: SYS-001 Powertrain system/)).toBeInTheDocument();
  });
});
