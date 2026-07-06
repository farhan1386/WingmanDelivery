import React, { useState } from 'react';

export function OrderForm() {
    const [pickup, setPickup] = useState('');
    const [orderInfo, setOrderInfo] = useState(null);
    const [loading, setLoading] = useState(false);

    const handleDispatch = async (e) => {
        e.preventDefault();
        setLoading(true);

        try {
            // Hits the custom POST route group we mapped in your C# Minimal API Server
            const response = await fetch('/api/orders', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ pickupAddress: pickup })
            });

            if (response.ok) {
                const data = await response.json();
                setOrderInfo(data); // Stores the saved DeliveryOrderModel
            } else {
                alert("Server responded with an execution boundary fault.");
            }
        } catch (err) {
            console.error("Network communication failure:", err);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div style={{ maxWidth: '450px', margin: '40px auto', fontFamily: 'Segoe UI, sans-serif' }}>
            <form onSubmit={handleDispatch} style={{ padding: '25px', border: '1px solid #ddd', borderRadius: '8px', background: '#fff' }}>
                <h3 style={{ color: '#007acc', marginTop: 0 }}>📦 Wingman Courier Dispatch</h3>
                <p style={{ color: '#666', fontSize: '14px' }}>Submit a pickup request location directly into your Dapper SQL Server transaction engine.</p>

                <div style={{ marginBottom: '15px' }}>
                    <label style={{ display: 'block', marginBottom: '5px', fontWeight: '500' }}>Pickup Address:</label>
                    <input
                        type="text"
                        value={pickup}
                        onChange={(e) => setPickup(e.target.value)}
                        placeholder="e.g. Hub Terminal A, New York"
                        required
                        style={{ width: '95%', padding: '10px', borderRadius: '4px', border: '1px solid #ccc' }}
                    />
                </div>

                <button type="submit" disabled={loading} style={{ width: '100%', padding: '12px', background: '#007acc', color: '#fff', border: 'none', borderRadius: '4px', cursor: 'pointer', fontWeight: '600' }}>
                    {loading ? 'Committing Transaction...' : 'Confirm Dispatch'}
                </button>
            </form>

            {orderInfo && (
                <div style={{ marginTop: '20px', padding: '15px', background: '#e1f5fe', border: '1px solid #b3e5fc', borderRadius: '6px' }}>
                    <h4 style={{ color: '#0288d1', marginTop: 0 }}>✅ Database Commit Confirmed!</h4>
                    <p style={{ margin: '5px 0', fontSize: '14px' }}><strong>UID:</strong> <span style={{ fontFamily: 'monospace' }}>{orderInfo.f_uid}</span></p>
                    <p style={{ margin: '5px 0', fontSize: '14px' }}><strong>Status Code:</strong> {orderInfo.f_status}</p>
                    <p style={{ margin: '5px 0', fontSize: '14px' }}><strong>Calculated Fee:</strong> ${orderInfo.f_total_cost}</p>
                </div>
            )}
        </div>
    );
}
