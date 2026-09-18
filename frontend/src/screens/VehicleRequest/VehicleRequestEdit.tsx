import React, { useState, useEffect } from 'react';
import { useMutation } from '@tanstack/react-query';
import { TkButton, TkDatepicker, TkInput, TkSelect } from '@takeoff-ui/react';
import { dataProvider } from '../../dataProvider';
import { overlayStyle, modalStyle } from '../../styles';
import { LookupSelect } from '../../shared/LookupSelect';
import { useFlows } from '../../flows/FlowProvider';

type VehicleRequestRecord = {
  id: string | number;
  requestNo: string;
  requestTypeId: number;
  startDate: string;
  endDate: string;
  destination?: string;
  purpose: string;
  driverName?: string;
  isPoolExternal: boolean;
  externalVehicleInfo?: string;
  fleetNote?: string;
  revisionNote?: string;
  status: string;
  departmentId: string;
  vehicleRequestTypeId: string;
  vehicleId: string;
};

interface VehicleRequestEditProps {
  record: VehicleRequestRecord | null;
  onClose: () => void;
  onSuccess: () => void;
}

export const VehicleRequestEdit: React.FC<VehicleRequestEditProps> = ({ record, onClose, onSuccess }) => {
  const [form, setForm] = useState<Partial<VehicleRequestRecord>>(record ?? {});
  const setField = (name: string, value: unknown) => setForm((p) => ({ ...p, [name]: value }));
  const { triggerFlows } = useFlows();

  useEffect(() => {
    if (record) setForm({ ...record });
  }, [record]);

  const mutation = useMutation({
    mutationFn: (values: Partial<VehicleRequestRecord>) =>
      dataProvider.update('VehicleRequest', record!.id, values),
    onSuccess: (_data, values) => { triggerFlows('update', 'VehicleRequest', values as Record<string, unknown>); onSuccess(); onClose(); },
    onError: (err: Error) => { window.dispatchEvent(new CustomEvent('app-toast', { detail: { type: 'error', message: err.message } })); },
  });

  if (!record) return null;

  return (
    <div style={overlayStyle} onClick={onClose}>
      <div style={modalStyle} onClick={(e) => e.stopPropagation()}>
        <div style={{ padding: '20px 28px 0', flexShrink: 0, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h2 style={{ margin: 0, fontSize: 18, fontWeight: 700 }}>Edit VehicleRequest</h2>
          <button onClick={onClose} style={{ background: 'none', border: 'none', fontSize: 20, cursor: 'pointer', color: '#666', padding: '4px 8px', borderRadius: 4 }} onMouseOver={(e) => (e.currentTarget.style.color = '#333')} onMouseOut={(e) => (e.currentTarget.style.color = '#666')}>✕</button>
        </div>
        <form onSubmit={(e) => { e.preventDefault(); mutation.mutate(form); }} style={{ display: 'flex', flexDirection: 'column', flex: 1, overflow: 'hidden' }}>
          <div style={{ flex: 1, overflowY: 'auto', padding: '20px 28px' }}>
            <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '16px' }}>
                <div>
                  <TkInput mode="text" label="Request No *" value={String(form.requestNo ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('requestNo', v))(e.detail)} />
                </div>
                <div>
                  <TkInput mode="number" label="Request Type Id *" value={String(form.requestTypeId ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('requestTypeId', Number(v)))(e.detail)} />
                </div>
                <div>
                  <TkDatepicker label="Start Date *" value={String(form.startDate ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('startDate', v))(e.detail)} />
                </div>
                <div>
                  <TkDatepicker label="End Date *" value={String(form.endDate ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('endDate', v))(e.detail)} />
                </div>
                <div>
                  <TkInput mode="text" label="Destination" value={String(form.destination ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('destination', v))(e.detail)} />
                </div>
                <div>
                  <TkInput mode="text" label="Purpose *" value={String(form.purpose ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('purpose', v))(e.detail)} />
                </div>
                <div>
                  <TkInput mode="text" label="Driver Name" value={String(form.driverName ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('driverName', v))(e.detail)} />
                </div>
                <div>
                  <label style={{ display: 'flex', alignItems: 'center', gap: 8, fontSize: 14 }}>
                    <input type="checkbox" checked={!!form.isPoolExternal} onChange={(e) => ((v) => setField('isPoolExternal', v))(e.target.checked)} style={{ width: 16, height: 16 }} />
                    Is Pool External *
                  </label>
                </div>
                <div>
                  <TkInput mode="text" label="External Vehicle Info" value={String(form.externalVehicleInfo ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('externalVehicleInfo', v))(e.detail)} />
                </div>
                <div>
                  <TkInput mode="text" label="Fleet Note" value={String(form.fleetNote ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('fleetNote', v))(e.detail)} />
                </div>
                <div>
                  <TkInput mode="text" label="Revision Note" value={String(form.revisionNote ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('revisionNote', v))(e.detail)} />
                </div>
                <div>
                  <LookupSelect label="Status *" value={String(form.status ?? '')} onChange={(v) => setField('status', v ? Number(v) : null)} searchable={false} options={[{ label: 'Draft', value: '0' }, { label: 'PendingManagerApproval', value: '1' }, { label: 'PendingFleetApproval', value: '2' }, { label: 'Approved', value: '3' }, { label: 'Revision', value: '4' }, { label: 'Completed', value: '5' }, { label: 'Cancelled', value: '6' }]} />
                </div>
                <div>
                  <LookupSelect label="Birim *" resource="Department" value={String(form.departmentId ?? '')} onChange={(v) => setField('departmentId', v)} displayField="name" />
                </div>
                <div>
                  <LookupSelect label="Talep Türü *" resource="VehicleRequestType" value={String(form.vehicleRequestTypeId ?? '')} onChange={(v) => setField('vehicleRequestTypeId', v)} displayField="name" />
                </div>
                <div>
                  <LookupSelect label="Araç *" resource="Vehicle" value={String(form.vehicleId ?? '')} onChange={(v) => setField('vehicleId', v)} displayField="plate" />
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
