import React, { useState, useCallback, useEffect, useRef } from 'react';
import { useSearchParams } from 'react-router-dom';
import { TkButton } from '@takeoff-ui/react';
import { dataProvider } from '../../dataProvider';
import { useListQuery } from '../../shared/useListQuery';
import { useDeleteMutation } from '../../shared/useDeleteMutation';
import { DeleteConfirmDialog } from '../../shared/DeleteConfirmDialog';
import { ListPageLayout } from '../../shared/ListPageLayout';
import type { TableColumn } from '../../shared/ListPageLayout';
import { actionColumn } from '../../shared/ActionButtons';
import { VehicleRequestCreate } from './VehicleRequestCreate';
import { VehicleRequestEdit } from './VehicleRequestEdit';
import { useFlows } from '../../flows/FlowProvider';

// ---------------------------------------------------------------------------
// Types
// ---------------------------------------------------------------------------

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
  [key: string]: unknown;
};

// ---------------------------------------------------------------------------
// Column definition — edit this array to add/remove/reorder columns
// ---------------------------------------------------------------------------
//
// Override examples:
//   • Hide a column:     remove its entry from COLUMNS
//   • Add a custom col:  { field: 'fullName', header: 'Full Name', html: (row) => `${row.firstName} ${row.lastName}` }
//   • Enable filtering:  add searchable: true  or  filterType: 'text' | 'checkbox' | 'radio' | 'datepicker'
//   • Custom cell render: html: (row) => `<span style="color:green">${row.status}</span>`
//
// Shared components (src/shared/) can be edited to change behavior globally:
//   • ListPageLayout  — table wrapper, pagination, header layout
//   • ActionButtons   — edit/delete button styles, labels, and behavior
//   • useListQuery    — data fetching, sorting, filtering logic
//   • useDeleteMutation / DeleteConfirmDialog — delete flow
//
// Action buttons override: edit src/shared/ActionButtons.ts DEFAULT_CONFIG
// or pass custom config: actionColumn('id', { hasEdit: true, config: { edit: { label: 'View', style: '...' } } })
//

const COLUMNS: TableColumn[] = [
  { field: 'id', header: 'ID', sortable: true },
  { field: 'requestNo', header: "Request No", sortable: true, searchable: true, filterType: 'text' },
  { field: 'requestTypeId', header: "Request Type Id", sortable: true },
  { field: 'startDate', header: "Start Date", sortable: true, filterType: 'datepicker', html: (row: Record<string, unknown>) => row.startDate ? new Date(String(row.startDate)).toLocaleDateString() : '—' },
  { field: 'endDate', header: "End Date", sortable: true, filterType: 'datepicker', html: (row: Record<string, unknown>) => row.endDate ? new Date(String(row.endDate)).toLocaleDateString() : '—' },
  { field: 'destination', header: "Destination", sortable: true, searchable: true, filterType: 'text' },
  { field: 'purpose', header: "Purpose", sortable: true, searchable: true, filterType: 'text' },
  { field: 'driverName', header: "Driver Name", sortable: true, searchable: true, filterType: 'text' },
  { field: 'isPoolExternal', header: "Is Pool External", sortable: true, filterType: 'checkbox', filterOptions: [{ label: 'Yes', value: 'true' }, { label: 'No', value: 'false' }], html: (row: Record<string, unknown>) => row.isPoolExternal ? '<span style="color:#2e7d32;font-weight:600">Yes</span>' : '<span style="color:#999">No</span>' },
  { field: 'externalVehicleInfo', header: "External Vehicle Info", sortable: true, searchable: true, filterType: 'text' },
  { field: 'fleetNote', header: "Fleet Note", sortable: true, searchable: true, filterType: 'text' },
  { field: 'revisionNote', header: "Revision Note", sortable: true, searchable: true, filterType: 'text' },
  { field: 'status', header: "Status", sortable: true, filterType: 'radio', filterOptions: [{ label: "Draft", value: "0" }, { label: "PendingManagerApproval", value: "1" }, { label: "PendingFleetApproval", value: "2" }, { label: "Approved", value: "3" }, { label: "Revision", value: "4" }, { label: "Completed", value: "5" }, { label: "Cancelled", value: "6" }], html: (row: Record<string, unknown>) => { const m: Record<string, string> = {["0"]: "Draft", ["1"]: "PendingManagerApproval", ["2"]: "PendingFleetApproval", ["3"]: "Approved", ["4"]: "Revision", ["5"]: "Completed", ["6"]: "Cancelled"}; return m[String(row.status ?? '')] ?? String(row.status ?? '\u2014'); } },
  { field: 'departmentId', header: "Birim", sortable: true },
  { field: 'vehicleRequestTypeId', header: "Talep Türü", sortable: true },
  { field: 'vehicleId', header: "Araç", sortable: true },
];

// ---------------------------------------------------------------------------
// VehicleRequestList
// ---------------------------------------------------------------------------

export const VehicleRequestList: React.FC = () => {
  const list = useListQuery<VehicleRequestRecord>({ resource: 'VehicleRequest' });
  const [showCreate, setShowCreate] = useState(false);
  const [editRecord, setEditRecord] = useState<VehicleRequestRecord | null>(null);
  const [selectedRows, setSelectedRows] = useState<VehicleRequestRecord[]>([]);

  // ?focus=<id> ile gelindiginde kayit dogrudan acilir (My Tasks -> kayit)
  const [searchParams, setSearchParams] = useSearchParams();
  const focusId = searchParams.get('focus');
  const focusedRef = useRef<string | null>(null);
  useEffect(() => {
    if (!focusId || focusedRef.current === focusId) return;
    focusedRef.current = focusId;
    const clear = () => {
      const next = new URLSearchParams(searchParams);
      next.delete('focus');
      setSearchParams(next, { replace: true });
    };
    Promise.resolve(dataProvider.getOne('VehicleRequest', focusId))
      .then((rec) => { if (rec) setEditRecord(rec as VehicleRequestRecord); })
      .catch(() => { /* kayit bulunamadi — liste normal acilir */ })
      .finally(clear);
  }, [focusId]);
  const { triggerFlows } = useFlows();
  const del = useDeleteMutation('VehicleRequest', (ids) => { ids.forEach(id => triggerFlows('delete', 'VehicleRequest', { id })); });


  const columns = [...COLUMNS, ...actionColumn('id', { hasEdit: true, hasDelete: true })];

  const handleCrudAction = useCallback((action: string, id: string) => {
    const row = list.records.find((r) => String(r.id) === id);
    if (!row) return;
    if (action === 'edit') setEditRecord(row);
    if (action === 'delete') del.requestSingleDelete(row.id);
  }, [list.records]);

  return (
    <>
      <ListPageLayout
        title="VehicleRequest"
        subtitle={Object.keys(list.displayParams).length > 0 ? (
          <div style={{ fontSize: 13, color: '#666', marginTop: 4 }}>
            {Object.entries(list.displayParams).map(([k, v]) => (
              <span key={k} style={{ marginRight: 12 }}>{k}: <strong>{v}</strong></span>
            ))}
          </div>
        ) : undefined}
        records={list.records}
        columns={columns}
        dataKey="id"
        total={list.total}
        loading={list.isLoading}
        page={list.page}
        perPage={list.perPage}
        onPageChange={list.setPage}
        onPerPageChange={list.setPerPage}
        onTableRequest={list.handleTableRequest}
        selectionMode="checkbox"
        selectedRows={selectedRows}
        onSelectionChange={(rows) => setSelectedRows(rows as VehicleRequestRecord[])}
        onCrudAction={handleCrudAction}
        headerActions={<>
          {selectedRows.length > 0 && (
            <TkButton label={`Delete (${selectedRows.length})`} variant="danger" onTkClick={() => del.requestDelete(selectedRows.map(r => r.id), `${selectedRows.length} record(s)`)} />
          )}

          <TkButton label="+ Create VehicleRequest" variant="primary" onTkClick={() => setShowCreate(true)} />
        </>}
      />

      {list.isError && (
        <div style={{ padding: '10px 14px', background: '#fff3f3', border: '1px solid #f5c6c6', borderRadius: 6, color: '#c62828', fontSize: 13, marginBottom: 12 }}>
          Failed to load data: {(list.error as Error).message}
        </div>
      )}

      <DeleteConfirmDialog
        visible={!!del.deleteTarget}
        label={del.deleteTarget?.label ?? ''}
        isPending={del.isPending}
        onConfirm={del.confirmDelete}
        onCancel={() => del.setDeleteTarget(null)}
      />
      <VehicleRequestCreate open={showCreate} onClose={() => setShowCreate(false)} onSuccess={list.invalidate} />
      <VehicleRequestEdit record={editRecord} onClose={() => setEditRecord(null)} onSuccess={list.invalidate} />

    </>
  );
};

