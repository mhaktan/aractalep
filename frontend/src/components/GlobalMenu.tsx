import React, { useState, useEffect } from 'react';
import { NavLink, useLocation } from 'react-router-dom';
import { hasRole } from '../roles';
import { clearAuth } from '../dataProvider';

// ---------------------------------------------------------------------------
// Types
// ---------------------------------------------------------------------------

interface MenuItem {
  label: string;
  path: string;
  icon?: string;
  requiredRoles?: string[];
  children?: MenuItem[];
}

/** Filter menu items by user roles (recursive for groups) */
const filterByRole = (items: MenuItem[]): MenuItem[] =>
  items
    .filter((item) => hasRole(item.requiredRoles ?? []))
    .map((item) =>
      item.children ? { ...item, children: filterByRole(item.children) } : item
    )
    .filter((item) => !item.children || item.children.length > 0);

// ---------------------------------------------------------------------------
// Menu Configuration
// ---------------------------------------------------------------------------

const MENU_ITEMS: MenuItem[] = [
  {
    "label": "My Tasks",
    "path": "/tasks",
    "icon": "task_alt"
  },
  {
    "label": "Dashboard",
    "path": "/dashboard",
    "icon": "dashboard"
  },
  {
    "label": "Birim",
    "path": "/Department"
  },
  {
    "label": "Talep Türü",
    "path": "/VehicleRequestType"
  },
  {
    "label": "Araç",
    "path": "/Vehicle"
  },
  {
    "label": "Araç Talebi",
    "path": "/VehicleRequest"
  },
  {
    "label": "Administration",
    "path": "#",
    "icon": "admin_panel_settings",
    "requiredRoles": [
      "Admin"
    ],
    "children": [
      {
        "label": "Users",
        "path": "/users",
        "icon": "people"
      },
      {
        "label": "Roles",
        "path": "/roles",
        "icon": "security"
      }
    ]
  }
];

// ---------------------------------------------------------------------------
// Icon Component (Material Symbols)
// ---------------------------------------------------------------------------

const Icon: React.FC<{ name?: string; size?: number; className?: string }> = ({ name, size = 20, className }) => {
  if (!name) return null;
  return (
    <span
      className={`material-symbols-outlined ${className ?? ''}`}
      style={{ fontSize: size, lineHeight: 1, flexShrink: 0, userSelect: 'none' }}
    >
      {name}
    </span>
  );
};

// ---------------------------------------------------------------------------
// Menu Item Components
// ---------------------------------------------------------------------------

const MenuLink: React.FC<{
  item: MenuItem;
  collapsed: boolean;
  depth?: number;
}> = ({ item, collapsed, depth = 0 }) => (
  <NavLink
    to={item.path}
    title={collapsed ? item.label : undefined}
    style={({ isActive }) => ({
      display: 'flex',
      alignItems: 'center',
      gap: collapsed ? 0 : 10,
      padding: collapsed ? '10px 0' : `9px ${depth > 0 ? 12 : 12}px 9px ${12 + depth * 16}px`,
      borderRadius: 6,
      textDecoration: 'none',
      fontSize: depth > 0 ? 13 : 14,
      marginBottom: 2,
      justifyContent: collapsed ? 'center' : 'flex-start',
      background: isActive ? 'var(--primary-50)' : 'transparent',
      color: isActive ? 'var(--primary-500)' : '#555555',
      fontWeight: isActive ? 600 : 400,
      transition: 'background 0.15s, color 0.15s',
    })}
  >
    <Icon name={item.icon ?? (depth > 0 ? 'circle' : 'article')} size={depth > 0 ? 16 : 20} />
    {!collapsed && <span style={{ overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>{item.label}</span>}
  </NavLink>
);

const MenuGroup: React.FC<{
  item: MenuItem;
  collapsed: boolean;
}> = ({ item, collapsed }) => {
  const location = useLocation();
  const isChildActive = item.children?.some((c) => location.pathname.startsWith(c.path)) ?? false;
  const [open, setOpen] = useState(isChildActive);

  useEffect(() => {
    if (isChildActive && !open) setOpen(true);
  }, [isChildActive]);

  // Close popup when switching between collapsed/expanded
  useEffect(() => {
    if (collapsed) setOpen(false);
  }, [collapsed]);

  const iconRef = React.useRef<HTMLDivElement>(null);
  const popupRef = React.useRef<HTMLDivElement>(null);
  const [popupPos, setPopupPos] = React.useState({ top: 0, left: 0 });

  React.useEffect(() => {
    if (!open || !collapsed) return;
    // Position popup next to icon
    if (iconRef.current) {
      const rect = iconRef.current.getBoundingClientRect();
      setPopupPos({ top: rect.top, left: rect.right + 4 });
    }
    // Close on outside click
    const handler = (e: MouseEvent) => {
      if (popupRef.current && !popupRef.current.contains(e.target as Node) &&
          iconRef.current && !iconRef.current.contains(e.target as Node)) setOpen(false);
    };
    document.addEventListener('mousedown', handler);
    return () => document.removeEventListener('mousedown', handler);
  }, [open, collapsed]);

  if (collapsed) {
    return (
      <div title={item.label}>
        <div
          ref={iconRef}
          style={{
            display: 'flex',
            justifyContent: 'center',
            padding: '10px 0',
            cursor: 'pointer',
            color: isChildActive ? 'var(--primary-500)' : '#555555',
            borderRadius: 6,
            background: isChildActive ? 'var(--primary-50)' : 'transparent',
            marginBottom: 2,
          }}
          onClick={() => setOpen(!open)}
        >
          <Icon name={item.icon ?? 'folder'} />
        </div>
        {open && (
          <div ref={popupRef} style={{
            position: 'fixed', top: popupPos.top, left: popupPos.left,
            background: '#fff', borderRadius: 8, border: '1px solid #e0e0e0',
            boxShadow: '0 4px 16px rgba(0,0,0,0.12)', minWidth: 180,
            zIndex: 99999, overflow: 'hidden', padding: '4px 0',
          }}>
            <div style={{ padding: '6px 12px', fontSize: 11, color: '#999', fontWeight: 600, textTransform: 'uppercase' }}>
              {item.label}
            </div>
            {item.children?.map((child) => (
              <div key={child.path} onClick={() => setOpen(false)}>
                <MenuLink item={child} collapsed={false} depth={0} />
              </div>
            ))}
          </div>
        )}
      </div>
    );
  }

  return (
    <div>
      <div
        onClick={() => setOpen(!open)}
        style={{
          display: 'flex',
          alignItems: 'center',
          gap: 10,
          padding: '9px 12px',
          borderRadius: 6,
          cursor: 'pointer',
          fontSize: 14,
          marginBottom: 2,
          color: isChildActive ? 'var(--primary-500)' : '#555555',
          fontWeight: isChildActive ? 600 : 400,
          background: isChildActive ? 'var(--primary-50)' : 'transparent',
          transition: 'background 0.15s',
        }}
      >
        <Icon name={item.icon ?? 'folder'} />
        <span style={{ flex: 1, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>{item.label}</span>
        <Icon name={open ? 'expand_less' : 'expand_more'} size={18} />
      </div>
      {open && (
        <div>
          {item.children?.map((child) => (
            <MenuLink key={child.path} item={child} collapsed={false} depth={1} />
          ))}
        </div>
      )}
    </div>
  );
};

// ---------------------------------------------------------------------------
// GlobalMenu Component
// ---------------------------------------------------------------------------

// ---------------------------------------------------------------------------
// User Dropdown — top-right user menu with logout
// ---------------------------------------------------------------------------

const UserDropdown: React.FC = () => {
  const [open, setOpen] = useState(false);
  const ref = React.useRef<HTMLDivElement>(null);
  const btnRef = React.useRef<HTMLButtonElement>(null);
  const [pos, setPos] = React.useState({ top: 0, right: 0 });

  React.useEffect(() => {
    if (!open) return;
    const handler = (e: MouseEvent) => {
      if (ref.current && !ref.current.contains(e.target as Node) &&
          btnRef.current && !btnRef.current.contains(e.target as Node)) setOpen(false);
    };
    document.addEventListener('mousedown', handler);
    return () => document.removeEventListener('mousedown', handler);
  }, [open]);

  const handleToggle = () => {
    if (!open && btnRef.current) {
      const rect = btnRef.current.getBoundingClientRect();
      setPos({ top: rect.bottom + 4, right: window.innerWidth - rect.right });
    }
    setOpen(!open);
  };

  // Try to get user info from localStorage (set after login)
  const userName = localStorage.getItem('_user_name') || 'User';
  const userEmail = localStorage.getItem('_user_email') || '';
  const initials = userName.slice(0, 2).toUpperCase();

  return (
    <div style={{ position: 'relative' }}>
      <button
        ref={btnRef}
        onClick={handleToggle}
        style={{
          display: 'flex', alignItems: 'center', gap: 8,
          padding: '4px 12px', borderRadius: 8,
          border: '1px solid #e0e0e0', background: '#fff',
          cursor: 'pointer', fontSize: 13, color: '#333',
        }}
      >
        <div style={{
          width: 28, height: 28, borderRadius: '50%',
          background: 'var(--primary-500)', color: '#fff',
          display: 'flex', alignItems: 'center', justifyContent: 'center',
          fontSize: 11, fontWeight: 700,
        }}>
          {initials}
        </div>
        <span style={{ maxWidth: 120, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
          {userName}
        </span>
        <Icon name={open ? 'expand_less' : 'expand_more'} size={16} />
      </button>

      {open && (
        <div ref={ref} style={{
          position: 'fixed', top: pos.top, right: pos.right,
          background: '#fff', borderRadius: 8, border: '1px solid #e0e0e0',
          boxShadow: '0 4px 16px rgba(0,0,0,0.1)', minWidth: 200,
          zIndex: 10000, overflow: 'hidden',
        }}>
          {userEmail && (
            <div style={{ padding: '12px 16px', borderBottom: '1px solid #f0f0f0', fontSize: 12, color: '#999' }}>
              {userEmail}
            </div>
          )}
          <UserMenuItem icon="person" label="Profile" onClick={() => setOpen(false)} />
          <UserMenuItem icon="settings" label="Settings" onClick={() => setOpen(false)} />
          <div style={{ height: 1, background: '#f0f0f0' }} />
          <UserMenuItem icon="logout" label="Logout" onClick={() => { clearAuth(); window.location.reload(); setOpen(false); }} danger />
        </div>
      )}
    </div>
  );
};

const UserMenuItem: React.FC<{ icon: string; label: string; onClick: () => void; danger?: boolean }> = ({ icon, label, onClick, danger }) => (
  <button
    onClick={onClick}
    style={{
      display: 'flex', alignItems: 'center', gap: 10, width: '100%',
      padding: '10px 16px', border: 'none', background: 'none',
      cursor: 'pointer', fontSize: 13, color: danger ? '#d32f2f' : '#333',
      textAlign: 'left',
    }}
    onMouseEnter={(e) => { (e.currentTarget).style.background = '#f5f5f5'; }}
    onMouseLeave={(e) => { (e.currentTarget).style.background = 'none'; }}
  >
    <Icon name={icon} size={18} />
    {label}
  </button>
);

// ---------------------------------------------------------------------------
// GlobalMenu
// ---------------------------------------------------------------------------

export const GlobalMenu: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [collapsed, setCollapsed] = useState(false);

  // Auto-collapse sidebar on small screens instead of switching to mobile drawer
  useEffect(() => {
    const onResize = () => {
      if (window.innerWidth < 768) setCollapsed(true);
    };
    onResize();
    window.addEventListener('resize', onResize);
    return () => window.removeEventListener('resize', onResize);
  }, []);

  const sidebarWidth = collapsed ? 64 : 240;

  const sidebar = (
    <aside
      style={{
        width: sidebarWidth,
        height: '100dvh',
        borderRight: '1px solid var(--tof-color-border, #e8e8e8)',
        background: '#ffffff',
        display: 'flex',
        flexDirection: 'column',
        flexShrink: 0,
        transition: 'width 0.2s ease',
        overflow: 'hidden',
      }}
    >
      {/* Header */}
      <div
        style={{
          padding: collapsed ? '16px 8px' : '16px 12px',
          display: 'flex',
          alignItems: 'center',
          justifyContent: collapsed ? 'center' : 'space-between',
          minHeight: 56,
          flexShrink: 0,
        }}
      >
        {!collapsed && (
          <div style={{
            fontWeight: 700,
            fontSize: 17,
            color: 'var(--primary-500)',
            display: 'flex',
            alignItems: 'center',
            overflow: 'hidden',
            whiteSpace: 'nowrap',
          }}>
            <img src="data:image/png;base64,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" alt="" style={{ height: 24, marginRight: 8 }} />{!collapsed && "aractalep"}
          </div>
        )}
        <button
          onClick={() => setCollapsed(!collapsed)}
          title={collapsed ? 'Expand menu' : 'Collapse menu'}
          style={{
            background: 'none',
            border: 'none',
            cursor: 'pointer',
            padding: 4,
            borderRadius: 4,
            color: '#555555',
            display: 'flex',
            alignItems: 'center',
          }}
        >
          <Icon name={collapsed ? 'menu_open' : 'menu'} size={22} />
        </button>
      </div>

      {/* Navigation */}
      <nav style={{ flex: 1, overflow: 'auto', padding: collapsed ? '0 8px' : '0 8px' }}>
        {filterByRole(MENU_ITEMS).map((item) =>
          item.children && item.children.length > 0 ? (
            <MenuGroup key={item.label} item={item} collapsed={collapsed} />
          ) : (
            <MenuLink key={item.path} item={item} collapsed={collapsed} />
          )
        )}
      </nav>

      {/* Footer — version info */}
      <div style={{ flexShrink: 0, padding: '8px 12px', borderTop: '1px solid #e8e8e8', fontSize: 10, color: '#bbb', textAlign: 'center' }}>
        Powered by Archipid
      </div>
    </aside>
  );

  return (
    <div style={{ display: 'flex', height: '100dvh', overflow: 'hidden', fontFamily: 'var(--tof-font-family, system-ui, sans-serif)' }}>
      {/* Sidebar — always inline, collapses to icon-only on small screens */}
      {sidebar}

      {/* Main content */}
      <div style={{ flex: 1, display: 'flex', flexDirection: 'column', overflow: 'hidden', minWidth: 0, minHeight: 0 }}>
        {/* Top header bar */}
        <div style={{
          display: 'flex', alignItems: 'center', justifyContent: 'flex-end',
          padding: '8px 24px', borderBottom: '1px solid #e8e8e8', background: '#fff',
          flexShrink: 0, gap: 12,
        }}>
          <UserDropdown />
        </div>

        <main style={{ flex: 1, overflow: 'auto', padding: '32px 36px', background: 'var(--tof-color-bg-subtle, #f8f9fa)' }}>
          {children}
        </main>
      </div>
    </div>
  );
};
