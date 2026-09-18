import React, { useState, useEffect } from 'react';
import { useMutation } from '@tanstack/react-query';
import { TkButton, TkInput, TkSelect } from '@takeoff-ui/react';
import { dataProvider } from '../../dataProvider';
import { overlayStyle, modalStyle } from '../../styles';
import { LookupSelect } from '../../shared/LookupSelect';
import { useFlows } from '../../flows/FlowProvider';

type VehicleRecord = {
  id: string | number;
  plate: string;
  brand: string;
  model: string;
  year?: number;
  capacity?: number;
  status: string;
};

interface VehicleEditProps {
  record: VehicleRecord | null;
  onClose: () => void;
  onSuccess: () => void;
}

export const VehicleEdit: React.FC<VehicleEditProps> = ({ record, onClose, onSuccess }) => {
  const [form, setForm] = useState<Partial<VehicleRecord>>(record ?? {});
  const setField = (name: string, value: unknown) => setForm((p) => ({ ...p, [name]: value }));
  const { triggerFlows } = useFlows();

  useEffect(() => {
    if (record) setForm({ ...record });
  }, [record]);

  const mutation = useMutation({
    mutationFn: (values: Partial<VehicleRecord>) =>
      dataProvider.update('Vehicle', record!.id, values),
    onSuccess: (_data, values) => { triggerFlows('update', 'Vehicle', values as Record<string, unknown>); onSuccess(); onClose(); },
    onError: (err: Error) => { window.dispatchEvent(new CustomEvent('app-toast', { detail: { type: 'error', message: err.message } })); },
  });

  if (!record) return null;

  return (
    <div style={overlayStyle} onClick={onClose}>
      <div style={modalStyle} onClick={(e) => e.stopPropagation()}>
        <div style={{ padding: '20px 28px 0', flexShrink: 0, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h2 style={{ margin: 0, fontSize: 18, fontWeight: 700 }}>Edit Vehicle</h2>
          <button onClick={onClose} style={{ background: 'none', border: 'none', fontSize: 20, cursor: 'pointer', color: '#666', padding: '4px 8px', borderRadius: 4 }} onMouseOver={(e) => (e.currentTarget.style.color = '#333')} onMouseOut={(e) => (e.currentTarget.style.color = '#666')}>✕</button>
        </div>
        <form onSubmit={(e) => { e.preventDefault(); mutation.mutate(form); }} style={{ display: 'flex', flexDirection: 'column', flex: 1, overflow: 'hidden' }}>
          <div style={{ flex: 1, overflowY: 'auto', padding: '20px 28px' }}>
            <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '16px' }}>
                <div>
                  <TkInput mode="text" label="Plate *" value={String(form.plate ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('plate', v))(e.detail)} />
                </div>
                <div>
                  <TkInput mode="text" label="Brand *" value={String(form.brand ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('brand', v))(e.detail)} />
                </div>
                <div>
                  <TkInput mode="text" label="Model *" value={String(form.model ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('model', v))(e.detail)} />
                </div>
                <div>
                  <TkInput mode="number" label="Year" value={String(form.year ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('year', Number(v)))(e.detail)} />
                </div>
                <div>
                  <TkInput mode="number" label="Capacity" value={String(form.capacity ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('capacity', Number(v)))(e.detail)} />
                </div>
                <div>
                  <LookupSelect label="Status *" value={String(form.status ?? '')} onChange={(v) => setField('status', v ? Number(v) : null)} searchable={false} options={[{ label: 'Available', value: '0' }, { label: 'InUse', value: '1' }, { label: 'UnderMaintenance', value: '2' }, { label: 'OutOfService', value: '3' }]} />
                </div>
            </div>
          </div>
          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 8, padding: '16px 28px', borderTop: '1px solid #e8e8e8', flexShrink: 0, background: '#fff' }}>
            <TkButton label="Cancel" variant="secondary" onTkClick={onClose} />
            <TkButton label={mutation.isPending ? 'Saving…' : 'Save Changes'} variant="primary" mode="submit" disabled={mutation.isPending} />
          </div>
        </form>
      </div>
    </div>
  );
};
