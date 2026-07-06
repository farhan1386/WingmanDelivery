import { useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';

interface LiveTrackingProps {
    orderUid: string;
}

export function LiveTrackingConsole({ orderUid }: LiveTrackingProps) {
    const [statusText, setStatusText] = useState("Pending 📦");
    const [coords, setCoords] = useState({ lat: 40.7128, lng: -74.0060 });
    const [telemetryLogs, setTelemetryLogs] = useState < string[] > ([]);

    useEffect(() => {
        // 1. Establish persistent web socket channel pointing to your C# Server Hub endpoint
        const connection = new signalR.HubConnectionBuilder()
            .withUrl('/hubs/tracking')
            .withAutomaticReconnect()
            .build();

        connection.start()
            .then(() => {
                console.log("Connected to Wingman Tracking Hub!");
                // Join the secure group for this specific package unique identifier string boundary
                connection.invoke("SubscribeToOrder", orderUid);

                // 2. Intercept live status updates pushed from your BackgroundWorker simulation loop
                connection.on("StatusUpdated", (newStatus: string) => {
                    const statuses: Record<string, string> = {
                        "DriverAssigned": "Driver Assigned 🎯",
                        "EnRoute": "En Route 🚚",
                        "Delivered": "Delivered ✅"
                    };
                    setStatusText(statuses[newStatus] || newStatus);
                    setTelemetryLogs(prev => [`[${new Date().toLocaleTimeString()}] Status shifted to: ${newStatus}`, ...prev]);
                });

                // 3. Intercept live GPS location feeds streamed from the background service
                connection.on("LocationUpdated", (lat: number, lng: number) => {
                    setCoords({ lat, lng });
                });
            })
            .catch(err => console.error("SignalR Connection Boundary Fault: ", err));

        return () => {
            connection.stop();
        };
    }, [orderUid]);

    return (
        <div style={{ marginTop: '25px', padding: '15px', border: '1px solid #b3e5fc', borderRadius: '6px', background: '#f0f4c3', textAlign: 'left' }}>
            <h3 style={{ color: '#0288d1', marginTop: 0, fontSize: '1rem' }}>📡 Live Satellite Control Tower</h3>
            <p style={{ margin: '5px 0', fontSize: '13px' }}><strong>Status State:</strong> <span style={{ fontWeight: 'bold' }}>{statusText}</span></p>

            {/* Dynamic Animated Status Progress Tracker */}
            <div style={{ background: '#ddd', height: '8px', borderRadius: '4px', margin: '12px 0', overflow: 'hidden' }}>
                <div style={{
                    height: '100%',
                    background: '#0288d1',
                    width: statusText.includes("Assigned") ? "40%" : statusText.includes("Route") ? "75%" : statusText.includes("Delivered") ? "100%" : "10%",
                    transition: 'width 1s ease-in-out'
                }} />
            </div>

            <div style={{ background: '#1e1e1e', color: '#00ff00', padding: '10px', borderRadius: '4px', fontFamily: 'monospace', fontSize: '12px', marginBottom: '10px' }}>
                🛰️ GPS TELEMETRY -- Lat: {coords.lat.toFixed(5)} | Lng: {coords.lng.toFixed(5)}
            </div>

            <div style={{ maxHeight: '80px', overflowY: 'auto', background: '#fff', padding: '8px', border: '1px solid #ccc', fontSize: '11px', fontFamily: 'monospace' }}>
                {telemetryLogs.length === 0 ? "Awaiting telemetry channel ping updates..." :
                    telemetryLogs.map((log, idx) => <div key={idx}>{log}</div>)
                }
            </div>
        </div>
    );
}
