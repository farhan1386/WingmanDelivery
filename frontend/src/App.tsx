import { useState } from 'react';
import aspireLogo from '/Aspire.png';
import { OrderForm, LiveTrackingConsole } from './features/DeliveryOrder';
import { UserPage } from './features/User';
import { RolePage } from './features/Role';
import { PaymentPage } from './features/Payment';
import { NotificationPage } from './features/Notification';
import './App.css';

type DashboardView = 'delivery' | 'users' | 'roles' | 'payments' | 'notifications';

export default function App() {
    const [currentView, setCurrentView] = useState<DashboardView>('delivery');

    return (
        <div className="app-container" style={{ display: 'flex', flexDirection: 'column', minHeight: '100vh' }}>
            <header className="app-header" style={{ borderBottom: '1px solid #e2e8f0', paddingBottom: '15px' }}>
                <a
                    href="https://aspire.dev"
                    target="_blank"
                    rel="noopener noreferrer"
                    aria-label="Visit Aspire website"
                    className="logo-link"
                >
                    <img src={aspireLogo} className="logo" alt="Aspire logo" />
                </a>
                <h1 className="app-title">Wingman Delivery Console</h1>
                <p className="app-subtitle">Modern Cloud-Native Multi-Tenant Logistics System</p>
                
                <nav aria-label="Main system feature views" style={{ display: 'flex', justifyContent: 'center', gap: '10px', marginTop: '20px', flexWrap: 'wrap' }}>
                    <button 
                        type="button"
                        onClick={() => setCurrentView('delivery')}
                        style={{ padding: '8px 16px', borderRadius: '6px', border: '1px solid #cbd5e1', fontWeight: 600, cursor: 'pointer', backgroundColor: currentView === 'delivery' ? '#007acc' : '#fff', color: currentView === 'delivery' ? '#fff' : '#334155' }}
                    >
                        📦 Delivery Orders
                    </button>
                    <button 
                        type="button"
                        onClick={() => setCurrentView('users')}
                        style={{ padding: '8px 16px', borderRadius: '6px', border: '1px solid #cbd5e1', fontWeight: 600, cursor: 'pointer', backgroundColor: currentView === 'users' ? '#007acc' : '#fff', color: currentView === 'users' ? '#fff' : '#334155' }}
                    >
                        👥 Members Profiles
                    </button>
                    <button 
                        type="button"
                        onClick={() => setCurrentView('roles')}
                        style={{ padding: '8px 16px', borderRadius: '6px', border: '1px solid #cbd5e1', fontWeight: 600, cursor: 'pointer', backgroundColor: currentView === 'roles' ? '#007acc' : '#fff', color: currentView === 'roles' ? '#fff' : '#334155' }}
                    >
                        🛡️ Security Roles
                    </button>
                    <button 
                        type="button"
                        onClick={() => setCurrentView('payments')}
                        style={{ padding: '8px 16px', borderRadius: '6px', border: '1px solid #cbd5e1', fontWeight: 600, cursor: 'pointer', backgroundColor: currentView === 'payments' ? '#007acc' : '#fff', color: currentView === 'payments' ? '#fff' : '#334155' }}
                    >
                        💳 Payments Ledger
                    </button>
                    <button 
                        type="button"
                        onClick={() => setCurrentView('notifications')}
                        style={{ padding: '8px 16px', borderRadius: '6px', border: '1px solid #cbd5e1', fontWeight: 600, cursor: 'pointer', backgroundColor: currentView === 'notifications' ? '#007acc' : '#fff', color: currentView === 'notifications' ? '#fff' : '#334155' }}
                    >
                        🔔 Alert Center
                    </button>
                </nav>
            </header>

            <main className="main-content" style={{ flex: 1, padding: '20px 0' }}>
                {currentView === 'delivery' && (
                    <section className="weather-section" aria-labelledby="dispatch-heading">
                        <OrderForm />
                        <LiveTrackingConsole />
                    </section>
                )}

                {currentView === 'users' && (
                    <section aria-label="System account workspaces">
                        <UserPage />
                    </section>
                )}

                {currentView === 'roles' && (
                    <section aria-label="Role management workspace">
                        <RolePage />
                    </section>
                )}

                {currentView === 'payments' && (
                    <section aria-label="Financial gateway processing workflows">
                        <PaymentPage />
                    </section>
                )}

                {currentView === 'notifications' && (
                    <section aria-label="System broadcast messaging logs">
                        <NotificationPage />
                    </section>
                )}
            </main>

            <footer className="app-footer" style={{ borderTop: '1px solid #e2e8f0', marginTop: 'auto', paddingTop: '15px' }}>
                <nav aria-label="Footer navigation">
                    <a href="https://aspire.dev" target="_blank" rel="noopener noreferrer">
                        Learn more about Aspire
                    </a>
                    <a href="https://github.com" target="_blank" rel="noopener noreferrer" className="github-link">
                        View App on GitHub
                    </a>
                </nav>
            </footer>
        </div>
    );
}