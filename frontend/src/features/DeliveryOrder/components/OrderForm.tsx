import { useState, type FormEvent } from 'react';
import type { DeliveryOrderModel } from '../types';
import { OrderStatus } from '../types';

export function OrderForm() {
    const [pickup, setPickup] = useState<string>('');
    const [orderInfo, setOrderInfo] = useState<DeliveryOrderModel | null>(null);
    const [loading, setLoading] = useState<boolean>(false);
    const [error, setError] = useState<string | null>(null);

    const handleDispatch = async (e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        setLoading(true);
        setError(null);

        try {
            // Hits the backend endpoint handled by your C# DeliveryOrderEndpoints mapping
            const response = await fetch('/api/delivery', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                // Binds precisely onto your required C# schema property rules
                body: JSON.stringify({
                    f_pickup_address: pickup,
                    f_status: OrderStatus.Pending,
                    f_total_cost: 0.00
                })
            });

            if (response.ok) {
                const data = await response.json() as DeliveryOrderModel;
                setOrderInfo(data);
                setPickup(''); 
            } else {
                const faultText = await response.text();
                setError(`Server Execution Boundary Fault: ${response.status} - ${faultText || 'Malformed Payload Request'}`);
            }
        } catch (err: unknown) {
            const errorMessage = err instanceof Error ? err.message : String(err);
            console.error("Network communication failure:", err);
            setError(`Network Communication Failure: ${errorMessage}`);
        } finally { 
            setLoading(false);
        }
    };

    return (
        <div style={{ maxWidth: '500px', margin: '40px auto', fontFamily: 'Segoe UI, -apple-system, sans-serif' }}>
            <form onSubmit={handleDispatch} style={{ padding: '30px', border: '1px solid #e2e8f0', borderRadius: '12px', background: '#ffffff', boxShadow: '0 4px 6px -1px rgba(0,0,0,0.05)' }}>
                <div style={{ display: 'flex', alignItems: 'center', gap: '10px', marginBottom: '10px' }}>
                    <span style={{ fontSize: '24px' }}>📦</span>
                    <h3 style={{ color: '#0284c7', margin: 0, fontSize: '20px', fontWeight: 600 }}>Courier Dispatch Center</h3>
                </div>
                <p style={{ color: '#64748b', fontSize: '14px', marginTop: 0, marginBottom: '20px' }}>
                    Submit structural logistics metadata directly through the Aspire orchestration layer down to active SQL transactional nodes.
                </p>

                {error && (
                    <div style={{ color: '#b91c1c', padding: '10px 14px', backgroundColor: '#fef2f2', border: '1px solid #fee2e2', borderRadius: '6px', marginBottom: '15px', fontSize: '13px' }}>
                        ⚠️ {error}
                    </div>
                )}

                <div style={{ marginBottom: '20px' }}>
                    <label style={{ display: 'block', marginBottom: '6px', fontWeight: '500', color: '#334155', fontSize: '14px' }}>
                        Pickup Address:
                    </label>
                    <input
                        type="text"
                        value={pickup}
                        // Note: Inline 'e' is implicitly typed by React context here and doesn't throw errors
                        onChange={(e) => setPickup(e.target.value)}
                        placeholder="e.g. Hub Terminal A, New York"
                        required
                        style={{ width: '93%', padding: '10px 14px', borderRadius: '6px', border: '1px solid #cbd5e1', fontSize: '14px', outline: 'none' }}
                    />
                </div>

                <button type="submit" disabled={loading} style={{ width: '100%', padding: '12px', background: '#0284c7', color: '#fff', border: 'none', borderRadius: '6px', cursor: 'pointer', fontWeight: '600', fontSize: '14px', transition: 'background 0.2s' }}>
                    {loading ? 'Committing Sql Transaction...' : 'Confirm Dispatch 🚀'}
                </button>
            </form>

            {/* Transaction Complete Receipt Display Metadata Card */}
            {orderInfo && (
                <div style={{ marginTop: '25px', padding: '20px', background: '#f0f9ff', border: '1px solid #bae6fd', borderRadius: '12px' }}>
                    <h4 style={{ color: '#0369a1', marginTop: 0, marginBottom: '12px', display: 'flex', alignItems: 'center', gap: '6px' }}>
                        ✅ Database Write Confirmed!
                    </h4>
                    <div style={{ display: 'flex', flexDirection: 'column', gap: '6px', fontSize: '14px', color: '#334155' }}>
                        <p style={{ margin: 0 }}><strong>UID Token:</strong> <span style={{ fontFamily: 'monospace', background: '#e0f2fe', padding: '2px 6px', borderRadius: '4px', fontSize: '12px' }}>{orderInfo.f_uid}</span></p>
                        <p style={{ margin: 0 }}><strong>Status Enum Code:</strong> {orderInfo.f_status ?? 0}</p>
                        <p style={{ margin: 0 }}><strong>Calculated Invoice:</strong> ${orderInfo.f_total_cost?.toFixed(2) || '0.00'}</p>
                        <p style={{ margin: 0 }}><strong>Route Endpoint:</strong> {orderInfo.f_pickup_address}</p>
                    </div>
                </div>
            )}
        </div>
    );
}