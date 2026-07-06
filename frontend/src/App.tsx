import { useState } from 'react';
import aspireLogo from '/Aspire.png';
import { LiveTrackingConsole } from './LiveTrackingConsole'; // Imports your real-time tracking socket manager
import './App.css';

// TypeScript data layout template parameters matching your N-Layer C# DeliveryOrderModel contract
interface DeliveryOrderModel {
    f_uid: string;
    f_iid: number;
    f_pickup_address: string;
    f_status: number; // Evaluated from your OrderStatus Enum (0 = Pending, 1 = Assigned, etc.)
    f_total_cost: number;
}

function App() {
    const [pickupAddress, setPickupAddress] = useState('');
    const [orderInfo, setOrderInfo] = useState<DeliveryOrderModel | null>(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    // Asynchronous operational boundary handler to submit new orders to your backend API
    const handleDispatchOrder = async (e: React.FormEvent) => {
        e.preventDefault();
        setLoading(true);
        setError(null);

        try {
            // Directs payload execution to the routing endpoints configured on your server project
            const response = await fetch('/api/orders', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ pickupAddress: pickupAddress })
            });

            if (!response.ok) {
                throw new Error(`HTTP transaction failure! status: ${response.status}`);
            }

            const data: DeliveryOrderModel = await response.json();
            setOrderInfo(data); // Unlocks the live tracking console below
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Failed to commit logistics transaction to database');
            console.error('Error creating delivery order:', err);
        } finally {
            setLoading(false);
        }
    };

    // Helper method to convert raw enum positions to customer-facing validation strings
    const getStatusText = (statusCode: number) => {
        const statuses = ['Pending 📦', 'Driver Assigned 🎯', 'En Route 🚚', 'Delivered ✅', 'Cancelled ❌'];
        return statuses[statusCode] || 'Unknown Context Position';
    };

    return (
        <div className="app-container">
            <header className="app-header">
                <a
                    href="https://aspire.dev"
                    target="_blank"
                    rel="noopener noreferrer"
                    aria-label="Visit Aspire website (opens in new tab)"
                    className="logo-link"
                >
                    <img src={aspireLogo} className="logo" alt="Aspire logo" />
                </a>
                <h1 className="app-title">Wingman Delivery</h1>
                <p className="app-subtitle">Modern Cloud-Native Multi-Tenant Logistics Console</p>
            </header>

            <main className="main-content">
                <section className="weather-section" aria-labelledby="dispatch-heading">
                    <div className="card" style={{ maxWidth: '500px', margin: '0 auto' }}>
                        <div className="section-header">
                            <h2 id="dispatch-heading" className="section-title">Courier Dispatch Center</h2>
                        </div>

                        {/* Delivery Order Dispatch Input Form */}
                        <form onSubmit={handleDispatchOrder} style={{ display: 'flex', flexDirection: 'column', gap: '15px', marginTop: '15px' }}>
                            <div style={{ textAlign: 'left' }}>
                                <label htmlFor="pickup-input" style={{ fontWeight: '600', display: 'block', marginBottom: '5px' }}>
                                    Pickup Address:
                                </label>
                                <input
                                    id="pickup-input"
                                    type="text"
                                    value={pickupAddress}
                                    onChange={(e) => setPickupAddress(e.target.value)}
                                    placeholder="e.g. Hub Terminal A, New York"
                                    required
                                    style={{ width: '100%', padding: '10px', borderRadius: '4px', border: '1px solid #ccc', boxSizing: 'border-box' }}
                                />
                            </div>

                            <button
                                className="refresh-button"
                                style={{ width: '100%', justifyContent: 'center', background: '#007acc', color: 'white', border: 'none', padding: '12px', borderRadius: '4px' }}
                                disabled={loading}
                                type="submit"
                            >
                                <span>{loading ? 'Committing Transaction...' : 'Confirm Dispatch 📦'}</span>
                            </button>
                        </form>

                        {/* Display Error Message Container Box */}
                        {error && (
                            <div className="error-message" role="alert" aria-live="polite" style={{ marginTop: '20px' }}>
                                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" aria-hidden="true">
                                    <circle cx="12" cy="12" r="10" />
                                    <line x1="12" y1="8" x2="12" y2="12" />
                                    <line x1="12" y1="16" x2="12.01" y2="16" />
                                </svg>
                                <span>{error}</span>
                            </div>
                        )}

                        {/* Hydrates order confirmation context receipts once Dapper transaction returns successfully */}
                        {orderInfo && (
                            <div style={{ marginTop: '25px', padding: '20px', background: '#f9f9f9', border: '1px solid #e0e0e0', borderRadius: '6px', textAlign: 'left' }}>
                                <h3 style={{ color: '#2e7d32', marginTop: 0, fontSize: '1.1rem' }}>🎉 Transaction Committed to SQL Server!</h3>
                                <hr style={{ border: '0', borderTop: '1px solid #e0e0e0', margin: '10px 0' }} />
                                <p style={{ margin: '6px 0', fontSize: '14px' }}><strong>Order UID:</strong> <span style={{ fontFamily: 'monospace', background: '#eee', padding: '2px 4px', borderRadius: '3px' }}>{orderInfo.f_uid}</span></p>
                                <p style={{ margin: '6px 0', fontSize: '14px' }}><strong>Internal ID (Identity):</strong> {orderInfo.f_iid}</p>
                                <p style={{ margin: '6px 0', fontSize: '14px' }}><strong>Pickup Location:</strong> {orderInfo.f_pickup_address}</p>
                                <p style={{ margin: '6px 0', fontSize: '14px' }}><strong>Logistics Status:</strong> {getStatusText(orderInfo.f_status)}</p>
                                <p style={{ margin: '6px 0', fontSize: '14px' }}><strong>Total Cost Fee:</strong> ${orderInfo.f_total_cost.toFixed(2)}</p>

                                {/* Securely binds your tracking console module directly beneath the static order confirmation details */}
                                <LiveTrackingConsole orderUid={orderInfo.f_uid} />
                            </div>
                        )}
                    </div>
                </section>
            </main>

            <footer className="app-footer">
                <nav aria-label="Footer navigation">
                    <a href="https://aspire.dev" target="_blank" rel="noopener noreferrer">
                        Learn more about Aspire
                    </a>
                    <a
                        href="https://github.com"
                        target="_blank"
                        rel="noopener noreferrer"
                        className="github-link"
                        aria-label="View Aspire on GitHub (opens in new tab)"
                    >
                        <img src="/github.svg" alt="" width="24" height="24" aria-hidden="true" />
                        <span className="visually-hidden">GitHub</span>
                    </a>
                </nav>
            </footer>
        </div>
    );
}

export default App;
